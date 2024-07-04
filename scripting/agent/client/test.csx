using System;
using DynamicPluginTest.Test;

public class SamplePlugin : IPlugin
{
    public int Execute(object[] args)
    {
        return 1;
    }
}
