using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcrwss.Entities
{
    public class AnalyticInfo: INotifyPropertyChanged
    {
        public string TableId { get; set; }
        [Display(Name = "SS Id")]
        public string SessionId { get; set; }
        public DateTime SessionDts { get; set; }
        public string Time { get; set; }
        public string BetResult { get; set; }
        [Display(Name = "F Benefit ")]
        public double FollowBenefit { get; set; }
        [Display(Name = "R Benefit")]
        public double ReverseBenefit { get; set; }
        [Display(Name = "F HH")]
        public double FollowHH { get; set; }
        [Display(Name = "R HH")]

        public bool RealBetFollow { get; set; }

        public bool RealBetReverse { get; set; }

        //public double RealBenefitFollow { get; set; }

        //public double RealBenefitReverse { get; set; }
        public double ReverseHH { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
