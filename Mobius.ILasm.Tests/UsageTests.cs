using System;
using System.IO;
using System.Runtime.Loader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobius.ILasm.Core;

namespace Mobius.ILasm.Tests
{
    [TestClass]
    public class UsageTests
    {
        [TestMethod]
        public void AssembleLoadIntoAssemblyLoadContextAndExecute()
        {
            var logger = new Logger();
            var driver = new Driver(logger, Driver.Target.Dll, false, false, false);
            var cil = File.ReadAllText(@"./trivial/helloworldconsole.il");
            using var memoryStream = new MemoryStream();

            driver.Assemble(new[] { cil }, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var assemblyContext = new AssemblyLoadContext(null);
            var assembly = assemblyContext.LoadFromStream(memoryStream);
            var entryPoint = assembly.EntryPoint;
            var result = entryPoint?.Invoke(null, new object[] { Array.Empty<string>() });

            Assert.AreEqual(44, result);
        }
    }
}
