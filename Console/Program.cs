using System;
using Microsoft.VisualBasic.Devices;

namespace Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App.Run(args, Environment.ProcessorCount, new ComputerInfo().TotalPhysicalMemory);
        }
    }
}