using Mobius.ILasm.Core;
using Mobius.ILasm.interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mobius.ILasm.Tests
{
    [TestClass]
    public class ILasmTests
    {
        [TestMethod]
        public void Test_helloworldconsole()
        {
            AssembleAndVerify("./trivial/helloworldconsole.il");
        }

        // This test is turned off as the ./trivial/*.il files do not
        // have .entrypoints, among other things.
        // [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetIlTestFiles), DynamicDataSourceType.Method)]
        public void TestTrivialFiles(string filename)
        {
            AssembleAndVerify(filename);
        }

        private static void AssembleAndVerify(string filename)
        {
            // try
            // {
            var logger = new Logger();
            var driver = new Driver(logger, Driver.Target.Exe, false, false, false);
            using var memoryStream = new MemoryStream();
            var success = driver.Assemble(new[] { File.ReadAllText(filename) }, memoryStream);

            var buffer = memoryStream.ToArray();
            var assembly = Assembly.Load(buffer);
            var entryPoint = assembly.EntryPoint;

            if (entryPoint is not null)
            {
                var result = entryPoint.Invoke(null, new object[] { Array.Empty<string>() });
                // var result = entryPoint.Invoke(null, Array.Empty<object>());

                var assertLine = File.ReadLines(filename)
                                     .FirstOrDefault(line => line.StartsWith("// Assert result"));
                if (assertLine is not null)
                {
                    var match = Regex.Match(assertLine, @"\/\/ Assert result (\d+)");
                    Assert.IsTrue(match.Success);

                    var expected = int.Parse(match.Groups[1].Value);
                    Assert.AreEqual(expected, result);
                }
            }
            // }
            // catch (Exception e)
            // {
            //     if (e is AssertFailedException)
            //     {
            //         throw;
            //     }
            //     else
            //     {
            //         Debug.WriteLine(filename);
            //     }
            // }
        }

        public static IEnumerable<object[]> GetIlTestFiles()
        {
            foreach (var filename in Directory.GetFiles("./trivial/", "*.il").OrderBy(f => f))
            {
                yield return new object[] { filename };
            }
        }
    }

    // TODO: Mock it with Moq
    internal class Logger : ILogger
    {
        public Logger() { }

        public void Error(string message) { }

        public void Error(Mono.ILASM.Location location, string message) { }

        public void Info(string message) { }

        public void Warning(string message) { }

        public void Warning(Mono.ILASM.Location location, string message) { }
    }
}
