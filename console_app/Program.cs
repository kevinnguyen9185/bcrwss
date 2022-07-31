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
            string[] patterns = new string[] { "PP", "PPP", "PPPP", "PPPPP", "PPPPPP", "PPPPPPP", "BB", "BBB", "BBBB", "BBBBB" };
            //string[] patterns = new string[] { "BBBB" };
            foreach (var pattern in patterns)
            {
                //IStatistics statistic = new ConsoleType1324();
                IStatistics statistic = new ConsoleType1324(DateTime.Now.AddDays(-30), DateTime.Now);
                Console.WriteLine($"Pattern: {pattern}");
                statistic.Run(pattern, enableProgressbar: true);
                Console.WriteLine("===================END=================");
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
