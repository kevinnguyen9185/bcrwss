using business.DAL;
using LiteDB;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace business.Statistics
{
    public class ConsoleType1324
    {
        LiteDatabase db;
        List<Entity.BcrTable> bcrTables = new List<Entity.BcrTable>();

        public ConsoleType1324()
        {
            db = new LiteDatabase(Config.DataPath);
            using (db)
            {
                var bcrTablesCollection = db.GetCollection<Entity.BcrTable>();
                bcrTables = bcrTablesCollection.FindAll().ToList();
            }
        }

        public void Run(string pattern = "BBBBBB")
        {
            if (bcrTables.Count == 0)
            {
                return;
            }

            var totalResultFollow = 0.0;
            var totalResultReserve = 0.0;
            var hoahongFollow = 0.0;
            var hoahongReserve = 0.0;
            var noBet = 0;

            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };
            using (var pbar = new ProgressBar(bcrTables.Count, "Bcr Tables", options))
            {
                foreach (var table in bcrTables)
                {
                    foreach (var session in table.Sessions)
                    {
                        if (session.ResultString.Contains(pattern))
                        {
                            var idxPattern = session.ResultString.IndexOf(pattern);
                            if (idxPattern > -1)
                            {
                                try
                                {
                                    var analyticResult = session.ResultString.Substring(idxPattern + pattern.Length, 4);
                                    var moneyFollow = PlayFollow(pattern[pattern.Length - 1].ToString(), analyticResult);
                                    var moneyReverse = PlayReverse(pattern[pattern.Length - 1].ToString(), analyticResult);
                                    //Console.WriteLine(session.ResultString);
                                    //WriteLineColor(session.ResultString, pattern, analyticResult.Length, $"Follow: {moneyFollow} Reserve: {moneyReverse}");
                                    //Console.WriteLine("========================");
                                    totalResultFollow += moneyFollow.Item1;
                                    totalResultReserve += moneyReverse.Item1;
                                    hoahongFollow += moneyFollow.Item2;
                                    hoahongReserve += moneyReverse.Item2;
                                    noBet++;
                                    pbar.Tick();
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(noBet);

            var benefit = totalResultFollow + hoahongFollow;
            Console.ForegroundColor = benefit > 0 ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine($"Total benefit Follow: {totalResultFollow} / {hoahongFollow}");

            benefit = totalResultReserve + hoahongReserve;
            Console.ForegroundColor = benefit > 0 ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine($"Total benefit Reserve: {totalResultReserve} / {hoahongReserve}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void WriteLineColor(string sessionResult, string pattern, int analyticResultLength, string money)
        {
            var startPattern = sessionResult.IndexOf(pattern);
            var endPattern = startPattern + pattern.Length;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(sessionResult.Substring(0, startPattern));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(pattern);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(sessionResult.Substring(endPattern, analyticResultLength));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(sessionResult.Substring(endPattern + analyticResultLength));
            Console.Write($" {money}");
            Console.WriteLine();
        }

        private double CalculateMoney(int order, bool isWon, string type)
        {
            int moneyFactor = 0;
            switch (order)
            {
                case 1:
                    moneyFactor = 1;
                    break;
                case 2:
                    moneyFactor = 3;
                    break;
                case 3:
                    moneyFactor = 2;
                    break;
                case 4:
                    moneyFactor = 4;
                    break;
            }
            if (isWon)
            {
                return type == "P" ? moneyFactor : moneyFactor * 0.9;
            }
            else
            {
                return -moneyFactor;
            }
        }

        private Tuple<double, double> PlayFollow(string lastResult, string analyticResult)
        {
            string pickedSide = lastResult;
            if (analyticResult.Length >= 4)
            {
                int order = 0;
                double moneyResult = 0;
                double hoahong = 0.0;
                foreach(var result in analyticResult)
                {
                    order++;
                    bool isWon = false;
                    if(result.ToString() == lastResult)
                    {
                        isWon = true;
                    }
                    var profit = CalculateMoney(order, isWon, pickedSide);
                    moneyResult += profit;
                    hoahong += Math.Abs(profit) * 0.008;
                }
                return new Tuple<double, double>(moneyResult, hoahong);
            }
            return new Tuple<double, double>(0, 0);
        }

        public Tuple<double, double> PlayReverse(string lastResult, string analyticResult)
        {
            string pickedSide = lastResult == "B" ? "P" : "B";
            if (analyticResult.Length >= 4)
            {
                int order = 0;
                double moneyResult = 0;
                double hoahong = 0.0;
                foreach (var result in analyticResult)
                {
                    order++;
                    bool isWon = false;
                    if (result.ToString() == pickedSide)
                    {
                        isWon = true;
                    }
                    var profit = CalculateMoney(order, isWon, pickedSide);
                    moneyResult += profit;
                    hoahong += Math.Abs(profit) * 0.008;
                }
                return new Tuple<double, double>(moneyResult, hoahong);
            }
            return new Tuple<double, double>(0, 0);
        }
    }
}
