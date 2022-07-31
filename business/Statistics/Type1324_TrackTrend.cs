using bcrwss.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace business.Statistics
{
    public class Type1324_TrackTrend : BaseBcrStatistics, IStatistics
    {

        const int NUMBER_SESSION_CHECK = 10;
        const double MAX_LOSE = -15;
        public Type1324_TrackTrend(DateTime fdate, DateTime tDate) : base(fdate, tDate)
        {
            this.MoneyStratergy = "1324";
        }

        protected override double CalculateMoney(int order, bool isWon, string type)
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

        public override AnalyticCollection Run(string pattern, bool enableProgressbar = true)
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

            foreach (var table in bcrTables)
            {
                var tableSessions = table.Sessions.OrderBy(r => r.Dts).ToList();
                var processingDts = GetDate(tableSessions[0].Dts);
                var analyticalIndex = 0;
                var lastAnalyticalIndexOfDate = 0;
                var first5sessionBenefitFollow = 0.0;
                var first5sessionBenefitReverse = 0.0;
                var tableProfitAccumulateFollow = 0.0;
                var tableProfitAccumulateReverse = 0.0;
                var stopLoseStringFollow = false;
                var stopLoseStringReverse = false;

                foreach (var session in tableSessions)
                {
                    var sessionOutput = ProcessFunc(session.ResultString, pattern);
                    if (sessionOutput != null)
                    {
                        var realBetFollow = false;
                        var realBetReverse = false;
                        var sessionDts = GetDate(session.Dts);
                        if (sessionDts != processingDts)
                        {
                            processingDts = sessionDts;
                            lastAnalyticalIndexOfDate = analyticalIndex;
                            first5sessionBenefitFollow = 0.0;
                            first5sessionBenefitReverse = 0.0;
                            tableProfitAccumulateFollow = 0.0;
                            tableProfitAccumulateReverse = 0.0;
                            stopLoseStringReverse = false;
                            stopLoseStringFollow = false;
                        }

                        totalResultFollow += sessionOutput.FollowResultMoney;
                        totalResultReserve += sessionOutput.ReverseResultMoney;
                        hoahongFollow += sessionOutput.FollowHoahongMoney;
                        hoahongReserve += sessionOutput.ReverseHoahongMoney;

                        if (analyticalIndex >= lastAnalyticalIndexOfDate + NUMBER_SESSION_CHECK)
                        {
                            if (!stopLoseStringFollow && tableProfitAccumulateFollow < MAX_LOSE)
                            {
                                stopLoseStringFollow = true;
                            }
                            if (!stopLoseStringReverse && tableProfitAccumulateReverse < MAX_LOSE)
                            {
                                stopLoseStringReverse = true;
                            }

                            realBetFollow = first5sessionBenefitFollow > 0 && !stopLoseStringFollow;
                            realBetReverse = first5sessionBenefitReverse > 0 &&  !stopLoseStringReverse;

                            tableProfitAccumulateFollow += sessionOutput.FollowResultMoney;
                            tableProfitAccumulateReverse += sessionOutput.ReverseResultMoney;
                        }

                        var analyticInfo = new AnalyticInfo
                        {
                            FollowBenefit = sessionOutput.FollowResultMoney,
                            ReverseBenefit = sessionOutput.ReverseResultMoney,
                            FollowHH = sessionOutput.FollowHoahongMoney,
                            ReverseHH = sessionOutput.ReverseHoahongMoney,
                            TableId = table.TableId,
                            SessionId = session.SessionId,
                            BetResult = sessionOutput.AnalyticString,
                            SessionDts = session.Dts,
                            RealBetFollow = realBetFollow,
                            RealBetReverse = realBetReverse,
                            Time = session.Dts.ToString("hh:mm")
                        };

                        analyticCollection.AnalyticsInfoDetails.Add(analyticInfo);

                        if (analyticalIndex < lastAnalyticalIndexOfDate + NUMBER_SESSION_CHECK)
                        {
                            first5sessionBenefitFollow += sessionOutput.FollowResultMoney;
                            first5sessionBenefitReverse += sessionOutput.ReverseResultMoney;
                        }

                        noBet++;
                        analyticalIndex++;
                    }
                }
            }

            analyticCollection.FinalProfitFollow = totalResultFollow + hoahongFollow;
            analyticCollection.FinalProfitReverse = totalResultReserve + hoahongReserve;

            return analyticCollection;
        }

        private string GetDate(DateTime dts)
        {
            return dts.ToString("ddMMyyy");
        }
    }

    public class FixedSizedQueue<T>
    {
        public FixedSizedQueue(int limit)
        {
            Limit = limit;
        }

        private object lockObject = new object();

        public int Limit { get; set; }
        public void Enqueue(T obj)
        {
            if(q == null)
            {
                q = new ConcurrentQueue<T>();
            }
            q.Enqueue(obj);
            lock (lockObject)
            {
                T overflow;
                while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }

        public ConcurrentQueue<T> q { get; set; }
    }
}
