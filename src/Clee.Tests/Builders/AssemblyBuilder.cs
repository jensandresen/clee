using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Clee.Tests.Builders
{
    internal class AssemblyBuilder
    {
        public Assembly Build()
        {
            return BuildAsAssemblyBuilder();
        }

        public System.Reflection.Emit.AssemblyBuilder BuildAsAssemblyBuilder()
        {
            var name = new AssemblyName("MyTestAssembly");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

            return assemblyBuilder;
        }
    }
}