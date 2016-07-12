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

            tStorage.tEngine tstorage = new tEngine();
            tstorage.open_storage("test");
            //tstorage.create("root");
            tstorage.create("root::sub", true);
            tstorage.create("root::sub::23", 1);
            tstorage.commit();

            swatch.Stop();

            Console.WriteLine("elapsed msec: {0}, ticks: {1}", swatch.ElapsedMilliseconds, swatch.ElapsedTicks);
            Console.ReadKey();
        }
    }
}
