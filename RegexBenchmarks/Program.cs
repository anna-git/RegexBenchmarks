﻿
using BenchmarkDotNet.Running;

public class Program
{

    public static void Main(string[] args)
    {
        var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
    }
}