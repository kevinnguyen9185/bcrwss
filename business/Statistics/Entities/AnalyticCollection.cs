using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcrwss.Entities
{
    public class AnalyticCollection : IDisposable
    {
        public AnalyticCollection()
        {
            //OrdersListDetails = new OrderInfoRepository().GetListOrdersDetails(2000);
        }

        private List<AnalyticInfo> _analyticInfoDetails;

        /// <summary>
        /// Gets or sets the orders details.
        /// </summary>
        /// <value>The orders details.</value>
        public List<AnalyticInfo> AnalyticsInfoDetails
        {
            get { return _analyticInfoDetails; }
            set { _analyticInfoDetails = value; }
        }

        public string Pattern { get; set; }

        public string Stratergy { get; set; }

        public double FinalProfitFollow { get; set; }

        public double FinalProfitReverse { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isdisposable)
        {
            if (this.AnalyticsInfoDetails != null)
            {
                this.AnalyticsInfoDetails.Clear();
            }
        }
    }
}
