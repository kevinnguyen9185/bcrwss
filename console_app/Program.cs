using business.Statistics;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            string[] patterns = new string[] { "PP", "PPP", "PPPP", "PPPPP", "PPPPPP", "PPPPPPP", "BB", "BBB", "BBBB", "BBBBB"};
            foreach(var pattern in patterns)
            {
                var statistic = new ConsoleType1324();
                Console.WriteLine($"Pattern: {pattern}");
                statistic.Run(pattern);
                Console.WriteLine("===================END=================");
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
