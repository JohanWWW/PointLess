using Interpreter.Framework.Extern;
using Interpreter.Framework.Extern.Mapping;
using System;
using System.Numerics;

namespace ZeroPointCLI.ExternLibraries
{
    [ExternAPI(Name = "Test")]
    public class TestLibrary : ExternAPI
    {
        [ExternEndpoint]
        public string Greet(string name)
        {
            return $"Hello {name}! How are you today?";
        }

        [ExternEndpoint]
        public int DoMath(
            [ExternParameter] int a, 
            [ExternParameter] int b)
        {
            return a + b;
        }

        [ExternEndpoint]
        public int Factorial(int x) => x == 1 ? 1 : x * Factorial(x - 1);

        [ExternEndpoint]
        public void IsThisTrue(bool b)
        {
            if (b)
            {
                Console.WriteLine("Yes, this is true!");
            }
            else
            {
                Console.WriteLine("No, this is false!");
            }
        }

        [ExternEndpoint]
        public IArrayAdapter DoSomethingWithAnArray(IArrayAdapter array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array.ElementAt<BigInteger>(i) * 2;
            }

            return array;
        }
    }
}
