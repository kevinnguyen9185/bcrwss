using bcrwss.Entities;
using System;

namespace business.Statistics
{
    public interface IStatistics
    {
        AnalyticCollection Run(string pattern, bool enableProgressbar = true);
    }
}
