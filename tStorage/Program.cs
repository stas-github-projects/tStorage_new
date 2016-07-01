using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace tStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch swatch = new Stopwatch();

            swatch.Start();

            swatch.Stop();
            Console.WriteLine("elapsed msec: {0}, ticks: {1}", swatch.ElapsedMilliseconds, swatch.ElapsedTicks);
            Console.ReadKey();
        }
    }
}
