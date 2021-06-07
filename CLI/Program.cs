using PointlessCLI.Extensions;
using Interpreter;
using Interpreter.Environment;
using Interpreter.NativeImplementations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PointlessCLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length is 0)
            {
                string version = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion
                    .ToString();

                string info = $"Pointless Command Line Interface v{version}";
                Console.WriteLine(info);
                Console.WriteLine(string.Concat(Enumerable.Repeat('=', info.Length)));
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine();
                Console.WriteLine("Creates necessary files for your new project");
                Console.WriteLine("  new");
                Console.WriteLine();
                Console.WriteLine("Execute the current project");
                Console.WriteLine("  run [-a <arg list>]");

                return;
            }

            if (args[0] is "new")
            {
                Console.WriteLine($"This operation is going to create files in current directory which is \"{Environment.CurrentDirectory}\"");
                Console.Write("Type 'yes' to continue: ");
                string input = Console.ReadLine();
                if (input is "yes")
                {
                    Initialize();
                }
                else
                {
                    Console.WriteLine("The operation was interrupted");
                }
            }
            else if (args[0] is "run")
            {
                if (args.Length > 1 && args[1] is "-a")
                {
                    string[] programArgs = args[2..];
                    Run(programArgs);
                }
                else
                {
                    string[] programArgs = Array.Empty<string>();
                    Run(programArgs);
                }
            }
            else
            {
                Console.WriteLine($"Unknown command: {string.Concat(' ', args)}");
            }
        }

        /// <summary>
        /// Initializes a project directory
        /// <code>
        /// Project
        ///       |__system
        ///       |       |__...
        ///       |       |__...
        ///       |       |__...
        ///       |       |__compile-order.txt
        ///       |
        ///       |__source
        ///       |       |__program.pl
        ///       |
        ///       |__plproj.json
        /// </code>
        /// </summary>
        private static void Initialize()
        {
            Directory.CreateDirectory("system");

            var defaultLibraries = ProjectGeneration.StandardLibraries.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            IDictionaryEnumerator defaultLibrariesEnumerator = defaultLibraries.GetEnumerator();
            while (defaultLibrariesEnumerator.MoveNext())
            {
                string key = defaultLibrariesEnumerator.Key as string;

                if (key is "CompileOrder")
                {
                    string content = defaultLibrariesEnumerator.Value as string;
                    using var fileStream = File.Create("system\\compile-order.txt");
                    var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(content));
                    fileStream.Write(readOnlySpan);
                    continue;
                }

                byte[] value = defaultLibrariesEnumerator.Value as byte[];

                var keyRegex = new Regex(@"^([A-Za-z0-9]+)Source$");
                var keyMatch = keyRegex.Match(key);
                string filename = keyMatch.Groups[1].Value.FirstToLower() + ".ptls";

                using (var fileStream = File.Create(Path.Combine("system", filename)))
                {
                    var readOnlySpan = new ReadOnlySpan<byte>(value);
                    fileStream.Write(readOnlySpan);
                }
            }

            Directory.CreateDirectory("source");

            byte[] programTemplate = ProjectGeneration.ProjectFiles.program;

            using (var fileStream = File.Create("source\\program.ptls"))
            {
                var readOnlySpan = new ReadOnlySpan<byte>(programTemplate);
                fileStream.Write(readOnlySpan);
            }

            var projectFileModel = new ProjectModel
            {
                Include = new[] { "source\\program.ptls" },
                CompileOrder = new[] { "program" },
                EntryPoint = new ProjectEntryPointModel
                {
                    Namespace = "program",
                    Method = "main"
                }
            };

            string json = JsonSerializer.Serialize(projectFileModel, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            using (var fileStream = File.Create("plproj.json"))
            {
                var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(json));
                fileStream.Write(readOnlySpan);
            }
        }

        private static void Run(string[] args)
        {
            var environment = new RuntimeEnvironment();

            var stdImplementation = new StdNativeImplementations();

            // Load system code
            var libSources = new Dictionary<string, string>();
            string[] libs = Directory.GetFiles("system", "*.ptls");

            foreach (string lib in libs)
            {
                string libSource = File.ReadAllText(lib);

                var regex = new Regex(@"^(?:[\w\d]+\\)*([\w\d]+)\.ptls$");
                var match = regex.Match(lib);

                string name = match.Groups[1].Value;
                libSources[name] = libSource;
            }

            string[] compileOrder = File.ReadAllLines("system\\compile-order.txt");

            // Build and run all library sources
            foreach (string lib in compileOrder)
            {
                var compiler = new ASTMapper(stdImplementation);
                var compiledTree = compiler.ToAST(libSources[lib]);

                var libInterpreter = CreateInterpreter(lib, environment);

                // Interpret and add bindings to environment
                libInterpreter.Interpret(compiledTree);
            }

            ProjectModel project = LoadProjectConfigurations("plproj.json");

            var sources = new Dictionary<string, string>();

            foreach (string srcPath in project.Include)
            {
                string source = File.ReadAllText(srcPath);

                var regex = new Regex(@"^(?:[\w\d]+\\)*([\w\d]+)\.ptls$");
                var match = regex.Match(srcPath);

                string srcName = match.Groups[1].Value;

                sources[srcName] = source;
            }

            foreach (string srcName in project.CompileOrder)
            {
                var compiler = new ASTMapper();
                var compiledTree = compiler.ToAST(sources[srcName]);

                var sourceInterpreter = CreateInterpreter(srcName, environment);
                sourceInterpreter.Interpret(compiledTree);
            }

            var entryPoint = environment
                .GetNamespace(project.EntryPoint.Namespace)
                    .Scope.GetLocalVariable(project.EntryPoint.Method) as Action<IList<dynamic>>;

            if (args.Length is 0)
                entryPoint.Invoke(new List<dynamic> { null });
            else
                entryPoint.Invoke(args.Select(a => (dynamic)a).ToList());
        }

        private static ProjectModel LoadProjectConfigurations(string path)
        {
            string jsonString = File.ReadAllText(path);

            var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(jsonString));
            ProjectModel projectModel = JsonSerializer.Deserialize<ProjectModel>(readOnlySpan);

            return projectModel;
        }

        private static Interpreter.Interpreter CreateInterpreter(string filename, RuntimeEnvironment environment)
        {
            var ns = new Namespace(filename);
            environment.AddNamespace(ns);
            return new Interpreter.Interpreter(ns, environment);
        }
    }
}
