using System;
using System.Collections.Generic;
using System.Text;

namespace business.Statistics
{
    public class ConsoleTypeConstant: BaseBcrStatistics, IStatistics
    {
        public ConsoleTypeConstant(DateTime fdate, DateTime tDate) : base(fdate, tDate)
        {
            MoneyStratergy = "11111111111111111111";
        }
        protected override AnalyticSessionOutput ProcessFunc(string resultString, string pattern)
        {
            var sessionOutput = new AnalyticSessionOutput();

            if (resultString.Contains(pattern))
            {
                var idxPattern = resultString.IndexOf(pattern);
                if (idxPattern > -1)
                {
                    try
                    {
                        var analyticResult = "";
                        if(idxPattern + pattern.Length + MoneyStratergy.Length > resultString.Length)
                        {
                            analyticResult = resultString.Substring(idxPattern + pattern.Length);
                        }
                        else
                        {
                            analyticResult = resultString.Substring(idxPattern + pattern.Length, MoneyStratergy.Length);
                        }
                        var moneyFollow = PlayFollow(pattern[pattern.Length - 1].ToString(), analyticResult);
                        var moneyReverse = PlayReverse(pattern[pattern.Length - 1].ToString(), analyticResult);
                        //Console.WriteLine(session.ResultString);
                        //WriteLineColor(resultString, pattern, analyticResult.Length, $"Follow: {moneyFollow} Reserve: {moneyReverse}");
                        //Console.WriteLine("========================");

                        sessionOutput.FollowResultMoney = moneyFollow.Item1;
                        sessionOutput.ReverseResultMoney = moneyReverse.Item1;
                        sessionOutput.FollowHoahongMoney = moneyFollow.Item2;
                        sessionOutput.ReverseHoahongMoney = moneyReverse.Item2;
                        return sessionOutput;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return null;
        }

        protected override Tuple<double, double> PlayFollow(string lastResult, string analyticResult)
        {
            string pickedSide = lastResult;
            if (analyticResult.Length > 0)
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
                    if (moneyResult > 0)
                    {
                        break;
                    }
                }
                return new Tuple<double, double>(moneyResult, hoahong);
            }
            return new Tuple<double, double>(0, 0);
        }

        protected override Tuple<double, double> PlayReverse(string lastResult, string analyticResult)
        {
            string pickedSide = lastResult == "B" ? "P" : "B";
            if (analyticResult.Length > 0)
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
                    if (moneyResult > 0)
                    {
                        break;
                    }
                }
                return new Tuple<double, double>(moneyResult, hoahong);
            }
            return new Tuple<double, double>(0, 0);
        }

        protected override double CalculateMoney(int order, bool isWon, string type)
        {
            return base.CalculateMoney(order, isWon, type);
        }
    }
}
