using System;
using NetGuard.PluginTest;

public class SamplePlugin : IPlugin
{
    public int Execute(object[] args)
    {
        ushort opcode = (ushort)args[0];

        switch(opcode)
        {
            case 0x2002:
            case 0x7001:
            case 0x7007:
                {
                    Console.WriteLine("Its working?");
                    return 1;
                }

            default: 
                return -1;
        }
    }
}