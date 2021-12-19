using ZeroPointCLI.Extensions;
using Interpreter;
using Interpreter.Environment;
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
using Interpreter.Models;
using Interpreter.Runtime;
using Interpreter.Framework.Extern;

namespace ZeroPointCLI
{
    public class Program
    {
        private static readonly string _projectDirectory = Environment.CurrentDirectory;

        public static void Main(string[] args)
        {
            if (args.Length is 0)
            {
                string version = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion
                    .ToString();

                string info = $"ZeroPoint Command Line Interface v{version}";
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
                try
                {
                    string[] programArgs = (args.Length > 1 && args[1] is "-a") ? 
                            args[2..] : 
                            Array.Empty<string>();
                    Run(programArgs);
                }
                catch (SyntaxException e)
                {
                    int lineNumber = e.LanguageSyntaxException.Line;
                    int startCol = e.LanguageSyntaxException.StartColumn;
                    int endCol = e.LanguageSyntaxException.EndColumn;

                    string title = e.ToString();
                    title += "\n" + string.Concat(Enumerable.Repeat("-", title.Length)); // Underline

                    Console.WriteLine(title);

                    if (File.Exists(e.AffectedFilePath))
                    {
                        string affectedLine = GetLineFromFile(e.AffectedFilePath, lineNumber - 1);
                        if (affectedLine.Length > 1)
                        {
                            for (int i = 0; i < affectedLine.Length; i++)
                            {
                                if (i >= startCol && i < endCol)
                                {
                                    var tmp = Console.ForegroundColor;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(affectedLine[i]);
                                    Console.ForegroundColor = tmp;
                                    continue;
                                }
                                Console.Write(affectedLine[i]);
                            }
                            Console.WriteLine();
                            string whitespace = string.Empty;
                            for (int i = 0; i < startCol; i++)
                            {
                                if (affectedLine[i] is '\t')
                                {
                                    whitespace += '\t';
                                    continue;
                                }
                                whitespace += ' ';
                            }
                            Console.WriteLine(whitespace + '^');
                            if (e?.LanguageSyntaxException?.AdditionalMessage != null)
                                Console.WriteLine(whitespace + e.LanguageSyntaxException.AdditionalMessage);
                            else
                                Console.WriteLine(whitespace + "Unexpected proceeding token");
                        }
                    }
                }
                catch (InterpreterRuntimeException e)
                {
                    int lineNumber = e.LineNumber;
                    int startCol = e.StartColumn;
                    int endCol = e.StopColumn;

                    Console.WriteLine("Unhandled runtime exception");
                    Console.WriteLine("---------------------------");

                    if (File.Exists(e.AffectedFilePath))
                    {
                        string affectedLine = GetLineFromFile(e.AffectedFilePath, lineNumber - 1);
                        if (affectedLine.Length > 1)
                        {
                            for (int i = 0; i < affectedLine.Length; i++)
                            {
                                if (i >= startCol && i < endCol)
                                {
                                    var tmp = Console.ForegroundColor;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(affectedLine[i]);
                                    Console.ForegroundColor = tmp;
                                    continue;
                                }
                                Console.Write(affectedLine[i]);
                            }
                            Console.WriteLine();
                        }
                    }

                    Console.WriteLine(e);

                    if (e.CausedBy != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Inner exception");
                        Console.WriteLine("---------------");
                        Console.WriteLine(e.CausedBy);
                    }
                }
                catch (LanguageException e)
                {
                    Console.WriteLine("Unhandled exception");
                    Console.WriteLine("-------------------");
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"at {e.FilePath}:{e.LineNumber}:{e.StartColumn}");
                }
            }
            else
            {
                Console.WriteLine($"Unknown command: {string.Concat(' ', args)}");
            }
        }

        private static string GetLineFromFile(string path, int line) => File.ReadLines(path).ElementAt(line);

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
        ///       |       |__program.0p
        ///       |
        ///       |__project.json
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
                    using var fileStream = File.Create(Path.Combine("system", "compile-order.txt"));
                    var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(content));
                    fileStream.Write(readOnlySpan);
                    continue;
                }

                byte[] value = defaultLibrariesEnumerator.Value as byte[];

                var keyRegex = new Regex(@"^([A-Za-z0-9]+)Source$");
                var keyMatch = keyRegex.Match(key);
                string filename = keyMatch.Groups[1].Value.FirstToLower() + ".0p";

                using (var fileStream = File.Create(Path.Combine("system", filename)))
                {
                    var readOnlySpan = new ReadOnlySpan<byte>(value);
                    fileStream.Write(readOnlySpan);
                }
            }

            Directory.CreateDirectory("source");

            byte[] programTemplate = ProjectGeneration.ProjectFiles.program;

            using (var fileStream = File.Create(Path.Combine("source", "program.0p")))
            {
                var readOnlySpan = new ReadOnlySpan<byte>(programTemplate);
                fileStream.Write(readOnlySpan);
            }

            var projectFileModel = new ProjectModel
            {
                Include = new[] { Path.Combine("source", "program.0p") },
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

            using (var fileStream = File.Create("project.json"))
            {
                var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(json));
                fileStream.Write(readOnlySpan);
            }
        }

        private static void Run(string[] args)
        {
            var environment = new RuntimeEnvironment();

            var nativeImplementations = GetNativeImplementations();
            var externApis = GetExternApis();

            // Load system code
            var libFiles = new Dictionary<string, string>();
            var libFilePaths = new Dictionary<string, string>();
            
            string[] libs = Directory.GetFiles("system", "*.0p");

            foreach (string lib in libs)
            {
                string libSource = File.ReadAllText(lib);

                var regex = new Regex(@"^(?:[\w\d]+(\\|/))*([\w\d]+)\.0p$");
                var match = regex.Match(lib);

                string name = match.Groups[2].Value;
                libFiles.Add(name, libSource);
                libFilePaths.Add(name, Path.Combine(_projectDirectory, lib));
            }

            string[] compileOrder = File.ReadAllLines(Path.Combine("system", "compile-order.txt"));

            // Build and run all library sources
            foreach (string lib in compileOrder)
            {
                var compiler = new ASTMapper(nativeImplementations, externApis);

                RootModel compiledTree;
                try
                {
                    compiledTree = compiler.ToAST(libFiles[lib]);
                }
                catch (LanguageSyntaxException e)
                {
                    throw new SyntaxException(Path.Combine(_projectDirectory, libFilePaths[lib]), e);
                }

                var libInterpreter = CreateInterpreter(lib, libFilePaths[lib], environment);

                // Interpret and add bindings to environment
                libInterpreter.Interpret(compiledTree);
            }

            ProjectModel project = LoadProjectConfigurations("project.json");

            var sourceFiles = new Dictionary<string, string>();
            var sourceFilePaths = new Dictionary<string, string>();

            foreach (string srcPath in project.Include)
            {
                string source = File.ReadAllText(srcPath);

                var regex = new Regex(@"^(?:[\w\d]+(\\|/))*([\w\d]+)\.0p$");
                var match = regex.Match(srcPath);

                string srcName = match.Groups[2].Value;

                sourceFiles.Add(srcName, source);
                sourceFilePaths.Add(srcName, Path.Combine(_projectDirectory, srcPath));
            }

            foreach (string srcName in project.CompileOrder)
            {
                var compiler = new ASTMapper(null, externApis); // TODO: remove arguments

                RootModel compiledTree;
                try
                {
                    compiledTree = compiler.ToAST(sourceFiles[srcName]);
                }
                catch (LanguageSyntaxException e)
                {
                    throw new SyntaxException(Path.Combine(_projectDirectory, sourceFilePaths[srcName]), e);
                }

                var sourceInterpreter = CreateInterpreter(srcName, sourceFilePaths[srcName], environment);
                sourceInterpreter.Interpret(compiledTree);
            }

            var entryPointMethod = environment
                .GetNamespace(project.EntryPoint.Namespace)
                    .Scope.GetLocalValue(project.EntryPoint.Method) as IBinaryOperable<MethodData>;

            if (args.Length is 0)
                entryPointMethod.Value.GetOverload(1).GetConsumer().Invoke(new IBinaryOperable[] { VoidOperable.Void });
            else
                entryPointMethod.Value.GetOverload(1).GetConsumer().Invoke(args.Select(a => new StringOperable(a)).ToArray());
        }

        private static NativeImplementationBase[] GetNativeImplementations()
        {
            Type[] types = Assembly.Load(nameof(NativeLibraries)).GetTypes().Where(t => t.IsPublic).ToArray();
            NativeImplementationBase[] implementations = new NativeImplementationBase[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                implementations[i] = Activator.CreateInstance(types[i]) as NativeImplementationBase;
            }

            return implementations;
        }

        private static ExternAPI[] GetExternApis()
        {
            Type[] types = Assembly.Load(string.Join('.', nameof(ZeroPointCLI), nameof(ExternLibraries))).GetTypes().Where(t => t.IsPublic && t.GetCustomAttribute<ExternAPIAttribute>() != null).ToArray();
            ExternAPI[] implementations = new ExternAPI[types.Length];

            for (int i = 0; i < types.Length; i++)
                implementations[i] = Activator.CreateInstance(types[i]) as ExternAPI;

            return implementations;
        }

        private static ProjectModel LoadProjectConfigurations(string path)
        {
            string jsonString = File.ReadAllText(path);

            var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(jsonString));
            ProjectModel projectModel = JsonSerializer.Deserialize<ProjectModel>(readOnlySpan);

            return projectModel;
        }

        private static Interpreter.Interpreter CreateInterpreter(string fileName, string filePath, RuntimeEnvironment environment)
        {
            var ns = new Namespace(fileName);
            environment.AddNamespace(ns);
            return new Interpreter.Interpreter(ns, environment, filePath);
        }
    }
}
