using business.DAL;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace business.Statistics
{
    public class ConsoleType1324: BaseBcrStatistics, IStatistics
    {
        public ConsoleType1324(DateTime fdate, DateTime tDate): base(fdate, tDate)
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
    }
}
