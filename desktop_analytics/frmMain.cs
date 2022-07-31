using bcrwss.Entities;
using business.Statistics;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Syncfusion.Windows.Forms.Chart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace desktop_analytics
{
    public partial class frmMain : Form
    {
        List<AnalyticCollection> collectionList;
        List<AnalyticInfo> rawAnalyticsData;
        List<AnalyticInfo> tempDataGridSource;
        RetryPolicy policy;


        public frmMain()
        {
            InitializeComponent();
        }

        private void InitializeCompnentValue()
        {
            this.fDate.Value = DateTime.Now.AddDays(-30);
            this.tDate.Value = DateTime.Now;
            this.policy = Policy
                .Handle<Exception>()
                .RetryForever();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            InitializeCompnentValue();

            policy.Execute(() => RetrieveData());

            DisplayAnalyzeData();
        }

        private void RetrieveData()
        {
            string[] patterns = new string[] { "PP", "PPP", "PPPP", "PPPPP", "PPPPPP", "PPPPPPP", "BB", "BBB", "BBBB", "BBBBB" };
            cboPlayType.DataSource = patterns;

            collectionList = new List<AnalyticCollection>();

            foreach (var pattern in patterns)
            {
                //IStatistics statistic = new ConsoleType1324();
                IStatistics statistic = new Type1324_TrackTrend(this.fDate.Value, this.tDate.Value);
                var analyticCollection = statistic.Run(pattern, enableProgressbar: true);
                analyticCollection.Pattern = pattern;
                collectionList.Add(analyticCollection);
            }
        }

        private void DisplayAnalyzeData()
        {
            if (collectionList == null)
            {
                return;
            }
            var pattern = cboPlayType.SelectedItem.ToString();
            var datasource = collectionList.Where(r => r.Pattern == pattern).FirstOrDefault();
            var filteredData = GetFilteredData(datasource.AnalyticsInfoDetails);
            lblProfit.Text = $"Follow Profit: {datasource.FinalProfitFollow}   Reverse Profit: {datasource.FinalProfitReverse}";
            rawAnalyticsData = filteredData;
            tempDataGridSource = filteredData;
            BuildChartSource();
        }

        private void DisplayGrid(List<AnalyticInfo> datasource)
        {
            this.pager.DataSource = datasource;
            this.pager.PageSize = 80;
            this.pager.Refresh();
            this.lblTotal.Text = "Total data:" + datasource.Count;
            this.grid.DataSource = this.pager.PagedSource;
        }

        private List<AnalyticInfo> GetFilteredData(List<AnalyticInfo> lst)
        {
            return lst
                .Where(r => r.SessionDts >= fDate.Value && r.SessionDts <= tDate.Value)
                .OrderBy(r => r.SessionDts)
                .ToList();
        }

        private void cboPlayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayAnalyzeData();
        }

        private void BuildChartSource()
        {
            var chartType = cboBetType.SelectedItem.ToString();

            var currentChartSource = new List<AnalyticInfo>();
            if (chartType == "Follow") 
            {
                currentChartSource = rawAnalyticsData.Where(r => r.RealBetFollow == true).ToList();
            }
            else
            {
                currentChartSource = rawAnalyticsData.Where(r => r.RealBetReverse == true).ToList();
            }
             
            var totalBenefitFollow = 0.0;
            // Group by date
            var dateGroup = currentChartSource
                .GroupBy(r => r.SessionDts.ToString("dd/MM"))
                .Select(g => new
                {
                    Dts = g.Key,
                    FBenefit = g.Sum(s => s.FollowBenefit),
                    FHH = g.Sum(s=>s.FollowHH),
                    RBenefit = g.Sum(s => s.ReverseBenefit),
                    RHH = g.Sum(s=>s.ReverseHH),
                    TotalBet = g.Count()
                })
                .ToList();

            this.chartF.Series.Clear();
            ChartSeries seriesFB = new ChartSeries("Benefit");
            ChartSeries seriesFHH = new ChartSeries("HH");
            for (int i = 0; i < dateGroup.Count; i++)
            {
                if(chartType == "Follow")
                {
                    totalBenefitFollow += dateGroup[i].FBenefit + dateGroup[i].FHH;
                    seriesFB.Points.Add(dateGroup[i].Dts, dateGroup[i].FBenefit);
                    seriesFHH.Points.Add(dateGroup[i].Dts, dateGroup[i].FHH);
                }
                else if(chartType=="Reverse")
                {
                    totalBenefitFollow += dateGroup[i].RBenefit + dateGroup[i].RHH;
                    seriesFB.Points.Add(dateGroup[i].Dts, dateGroup[i].RBenefit);
                    seriesFHH.Points.Add(dateGroup[i].Dts, dateGroup[i].RHH);
                }
            }
            seriesFB.Type = ChartSeriesType.Column;
            seriesFB.Text = seriesFB.Name;
            seriesFHH.Type = ChartSeriesType.Column;
            seriesFHH.Text = seriesFHH.Name;
            this.chartF.Series.Add(seriesFB);
            this.chartF.Series.Add(seriesFHH);
            this.chartF.Text = $"{chartType} {totalBenefitFollow}";
        }


        private void chart_ChartRegionClick(object sender, ChartRegionMouseEventArgs e)
        {
            var chartType = cboBetType.SelectedItem.ToString();
            var currentChartSource = new List<AnalyticInfo>();
            if (chartType == "Follow")
            {
                currentChartSource = rawAnalyticsData.Where(r => r.RealBetFollow == true).ToList();
            }
            else
            {
                currentChartSource = rawAnalyticsData.Where(r => r.RealBetReverse == true).ToList();
            }

            var idx = e.Region.PointIndex;
            if (idx < 0)
            {
                return;
            }

            var dateGroupDetail = currentChartSource
                .OrderBy(r=>r.SessionDts)
                .GroupBy(r => r.SessionDts.ToString("dd/MM"))
                .ToList();
            var analyticInfoByDateDetail = dateGroupDetail[idx].ToList();
            var selectedDate = dateGroupDetail[idx].Key;

            var dateGroupFull = rawAnalyticsData
                .OrderBy(r => r.SessionDts)
                .GroupBy(r => r.SessionDts.ToString("dd/MM"))
                .Where(r=>r.Key == selectedDate)
                .ToList();

            
            var analyticInfoByDateFull = dateGroupFull[0].ToList();

            BuildTableChart(analyticInfoByDateDetail, chartTableDetail, dateGroupDetail[idx].Key);

            BuildTableChart(analyticInfoByDateFull, chartTableFull, dateGroupFull[0].Key);
        }

        private void BuildTableChart(List<AnalyticInfo> datasource, ChartControl chart, string dateStr)
        {
            chart.Text = dateStr;
            var tableGroup = datasource
                .GroupBy(r => r.TableId)
                .Select(g => new
                {
                    Table = g.Key,
                    FBenefit = g.Sum(s => (s.FollowBenefit + s.FollowHH)),
                    //FHH = g.Sum(s => s.FollowHH),
                    RBenefit = g.Sum(s => (s.ReverseBenefit + s.ReverseHH)),
                    //RHH = g.Sum(s => s.ReverseHH),
                    TotalBet = g.Count()
                })
                .ToList();

            chart.Series.Clear();
            ChartSeries series = new ChartSeries("Benefit");
            for (int i = 0; i < tableGroup.Count; i++)
            {
                series.Points.Add(tableGroup[i].Table, tableGroup[i].FBenefit);
            }
            series.Type = ChartSeriesType.Bar;
            series.Text = series.Name;
            chart.Series.Add(series);

            tempDataGridSource = datasource;
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            DisplayAnalyzeData();
        }

        private void rangeSlider_ValueChanged(object sender, EventArgs e)
        {
            var sTime = rangeSlider.SliderMin;
            var eTime = rangeSlider.SliderMax;
            if (tempDataGridSource.Count > 0)
            {
                var tempSourceByHour = tempDataGridSource.Where(r => r.SessionDts.Hour >= sTime && r.SessionDts.Hour < eTime).ToList();
                DisplayGrid(tempSourceByHour);
            }
        }

        private void chartCount_ChartRegionClick(object sender, ChartRegionMouseEventArgs e)
        {
            
        }

        private void cboBetType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboFilterData_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chartTableFull_ChartRegionClick(object sender, ChartRegionMouseEventArgs e)
        {
            var idx = e.Region.PointIndex;
            if (idx < 0)
            {
                return;
            }

            var tableId = chartTableFull.Series[0].Points[idx].Category;
            if (tableId != null)
            {
                var tableData = tempDataGridSource.Where(r => r.TableId == tableId).OrderBy(r => r.SessionDts).ToList();

                BuildChartTableTrend(tableData, tableId);
            }
        }

        private void chartTableDetail_ChartRegionClick(object sender, ChartRegionMouseEventArgs e)
        {
            var idx = e.Region.PointIndex;
            if (idx < 0)
            {
                return;
            }

            var tableId = chartTableDetail.Series[0].Points[idx].Category;
            if (tableId != null)
            {
                var tableData = tempDataGridSource.Where(r => r.TableId == tableId).OrderBy(r => r.SessionDts).ToList();

                BuildChartTableTrend(tableData, tableId);
            }
        }

        private void BuildChartTableTrend(List<AnalyticInfo> datasource, string tableName)
        {
            chartCount.Text = tableName;
            chartCount.Series.Clear();
            // All series
            ChartSeries series = new ChartSeries("Benefit");
            var accumulateBenefit = 0.0;
            for (int i = 0; i < datasource.Count; i++)
            {
                accumulateBenefit += datasource[i].FollowBenefit + datasource[i].FollowHH;
                series.Points.Add(i, accumulateBenefit);
            }
            series.Type = ChartSeriesType.Line;
            series.Text = series.Name;
            this.chartCount.Series.Add(series);

            // Filtered series
            var dataSourceFilterd = new List<AnalyticInfo>();
            var chartType = cboBetType.SelectedItem.ToString();
            accumulateBenefit = 0.0;

            if (chartType == "Follow")
            {
                dataSourceFilterd = datasource.Where(r => r.RealBetFollow == true).ToList();
            }
            else
            {
                dataSourceFilterd = datasource.Where(r => r.RealBetReverse == true).ToList();
            }
            ChartSeries seriesFiltered = new ChartSeries("Filtered");
            for (int i = 0; i < dataSourceFilterd.Count; i++)
            {
                accumulateBenefit += dataSourceFilterd[i].FollowBenefit + dataSourceFilterd[i].FollowHH;
                seriesFiltered.Points.Add(i, accumulateBenefit);
            }
            seriesFiltered.Type = ChartSeriesType.Line;
            seriesFiltered.Text = seriesFiltered.Name;
            this.chartCount.Series.Add(seriesFiltered);

            this.DisplayGrid(datasource);
        }
    }
}
