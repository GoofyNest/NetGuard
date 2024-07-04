using System;
using DynamicPluginDllTest.Test;

public class SamplePlugin : IPlugin
{
    public int Execute(object[] args)
    {
        return 1;
    }
}