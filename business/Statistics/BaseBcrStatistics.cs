using bcrwss.Entities;
using business.DAL;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace business.Statistics
{
    public class BaseBcrStatistics : IStatistics
    {
        LiteDatabase db;
        protected List<Entity.BcrTable> bcrTables = new List<Entity.BcrTable>();
        private string[] blackListTableIds = new string[]
        {
            "DragonTiger00001",
            "SuperSicBo000001",
            "pctte34dt6bqbtps",
            "ok37hvy3g7bofp4l",
            "SalPrivBac000001",
            "n7ltqx5j25sr7xbe",
            "SalPrivBac000004",
            "k2oswnib7jjaaznw", //"Baccarat Kiểm Soát Bóp"
            "zixzea8nrf1675oh", //"Baccarat Squeeze"
            "ovu5h6b3ujb4y53w", //"Baccarat Tốc độ Không Hoa hồng C"
            "LightningBac0001", //"Baccarat nhanh"
            "ocye5hmxbsoyrcii", //"Baccarat Tốc độ Không Hoa hồng B"
            "NoCommBac0000001", //"Baccarat Không Hoa hồng"
            "ndgv76kehfuaaeec", //"Baccarat Tốc độ Không hoa hồng A"
        };

        public string MoneyStratergy { get; set; }

        public BaseBcrStatistics(DateTime fdate, DateTime tDate)
        {
            db = new LiteDatabase(Config.DataPath);
            using (db)
            {
                var bcrTablesCollection = db.GetCollection<Entity.BcrTable>();
                bcrTables = bcrTablesCollection
                    .Find(
                        r => !blackListTableIds.Contains(r.TableId)
                    )
                    .ToList();
            }
        }

        public virtual AnalyticCollection Run(string pattern, bool enableProgressbar = true)
        {
            var analyticCollection = new AnalyticCollection();
            analyticCollection.AnalyticsInfoDetails = new List<AnalyticInfo>();
            if (bcrTables.Count == 0)
            {
                return null;
            }

            var totalResultFollow = 0.0;
            var totalResultReserve = 0.0;
            var hoahongFollow = 0.0;
            var hoahongReserve = 0.0;
            var noBet = 0;

            //bcrTables.RemoveAll(r => blackListTableIds.Contains(r.TableId));

            foreach (var table in bcrTables)
            {
                foreach (var session in table.Sessions)
                {
                    var sessionOutput = ProcessFunc(session.ResultString, pattern);
                    if (sessionOutput != null)
                    {
                        totalResultFollow += sessionOutput.FollowResultMoney;
                        totalResultReserve += sessionOutput.ReverseResultMoney;
                        hoahongFollow += sessionOutput.FollowHoahongMoney;
                        hoahongReserve += sessionOutput.ReverseHoahongMoney;

                        analyticCollection.AnalyticsInfoDetails.Add(new AnalyticInfo
                        {
                            FollowBenefit = sessionOutput.FollowResultMoney,
                            ReverseBenefit = sessionOutput.ReverseResultMoney,
                            FollowHH = sessionOutput.FollowHoahongMoney,
                            ReverseHH = sessionOutput.ReverseHoahongMoney,
                            TableId = table.TableId,
                            SessionId = session.SessionId,
                            BetResult = sessionOutput.AnalyticString,
                            SessionDts = session.Dts,
                            Time = session.Dts.ToString("hh:mm")
                        });
                        noBet++;
                    }
                }
            }

            analyticCollection.FinalProfitFollow = totalResultFollow + hoahongFollow;
            analyticCollection.FinalProfitReverse = totalResultReserve + hoahongReserve;

            Console.WriteLine(noBet);

            var benefit = totalResultFollow + hoahongFollow;
            Console.ForegroundColor = benefit > 0 ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine($"Total benefit Follow: {totalResultFollow} / {hoahongFollow}");

            benefit = totalResultReserve + hoahongReserve;
            Console.ForegroundColor = benefit > 0 ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine($"Total benefit Reserve: {totalResultReserve} / {hoahongReserve}");
            Console.ForegroundColor = ConsoleColor.White;

            return analyticCollection;
        }

        protected virtual AnalyticSessionOutput ProcessFunc(string resultString, string pattern)
        {
            var sessionOutput = new AnalyticSessionOutput();

            if (resultString.Contains(pattern))
            {
                var idxPattern = resultString.IndexOf(pattern);
                if (idxPattern > -1)
                {
                    try
                    {
                        var analyticResult = resultString.Substring(idxPattern + pattern.Length, MoneyStratergy.Length);
                        var moneyFollow = PlayFollow(pattern[pattern.Length - 1].ToString(), analyticResult);
                        var moneyReverse = PlayReverse(pattern[pattern.Length - 1].ToString(), analyticResult);
                        //Console.WriteLine(session.ResultString);
                        //WriteLineColor(session.ResultString, pattern, analyticResult.Length, $"Follow: {moneyFollow} Reserve: {moneyReverse}");
                        //Console.WriteLine("========================");
                        sessionOutput.FollowResultMoney = moneyFollow.Item1;
                        sessionOutput.ReverseResultMoney = moneyReverse.Item1;
                        sessionOutput.FollowHoahongMoney = moneyFollow.Item2;
                        sessionOutput.ReverseHoahongMoney = moneyReverse.Item2;
                        sessionOutput.AnalyticString = resultString.Substring(0, idxPattern) + "|" + pattern + "|" + analyticResult;
                        return sessionOutput;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return null;
        }

        protected void WriteLineColor(string sessionResult, string pattern, int analyticResultLength, string money)
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


        /// <summary>
        /// Calculate money
        /// </summary>
        /// <param name="order">1-index base</param>
        /// <param name="isWon"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual double CalculateMoney(int order, bool isWon, string type)
        {
            int moneyFactor = 0;

            moneyFactor = Convert.ToInt32(MoneyStratergy[order - 1].ToString());

            if (isWon)
            {
                return type == "P" ? moneyFactor : moneyFactor * 0.9;
            }
            else
            {
                return -moneyFactor;
            }
        }

        protected virtual Tuple<double, double> PlayFollow(string lastResult, string analyticResult)
        {
            string pickedSide = lastResult;
            if (analyticResult.Length >= 4)
            {
                int order = 0;
                double moneyResult = 0;
                double hoahong = 0.0;
                foreach (var result in analyticResult)
                {
                    order++;
                    bool isWon = false;
                    if (result.ToString() == lastResult)
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

        protected virtual Tuple<double, double> PlayReverse(string lastResult, string analyticResult)
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
