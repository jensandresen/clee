using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Clee.Tests.Builders
{
    internal class AssemblyBuilder
    {
        public Assembly Build()
        {
            var name = new AssemblyName("MyTestAssembly");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

            return assemblyBuilder;
        }
    }
}