using System;
using System.Reflection;

namespace NetGuard.Functions
{
    internal class Callback
    {
        public static void init()
        {
            byte[] assemblyToLoad = (byte[])AppDomain.CurrentDomain.GetData("Assembly");
            Program._assembly = Assembly.Load(assemblyToLoad);

            Type _dll = Program._assembly.GetType("Module.Main");
            object _instance = Activator.CreateInstance(_dll);

            MethodInfo _programFunction = _dll.GetMethod("StartProgram", BindingFlags.Public | BindingFlags.Instance);

            object[] args = (object[])AppDomain.CurrentDomain.GetData("args");

            _programFunction.Invoke(_instance, args);
        }
    }
}