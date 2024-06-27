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

            Type _dll = Program._assembly.GetType("GatewayContext.Main");
            object _instance = Activator.CreateInstance(_dll);

            MethodInfo _programFunction = _dll.GetMethod("StartProgram", BindingFlags.Public | BindingFlags.Instance);

            object[] args = new[] { Program.discordId, Program.discordName, Program.date };

            _programFunction.Invoke(_instance, args);
        }
    }
}