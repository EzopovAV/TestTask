using ScottPlot;
using ScottPlot.Plottable;
using StreamingChart.Interfaces;
using StreamingChart.Models;
using StreamingChart.SystemCover;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace StreamingChart
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private const int TimerTimeInSeconds = 1;
		private const int BufferSize = 100;
		private const string CompanyName = "Perm Dynamics";
		
		private readonly IStockObserver _stockObserver;
		private readonly Timer _timerRefreshStockPrice;

		private Sequence<Sample> _sequence;
		private bool _plotPrepared;
		private ScatterPlot _scatterPlot;
		private ScatterPlot _averagePlot;

		public MainWindowViewModel()
		{
			ChartWpfPlot = new WpfPlot();
			Brokers = new ObservableCollection<BrokerViewModel>
			{
				new BrokerViewModel(12345, "Tom")
				{
					Stock = 17,
					Money = 1030,
					LastActivityDataTime = DateTime.Now, 
					LastActivityActionString = "Bought 15 shares at a rate of 1.8"
				},
				new BrokerViewModel(78946, "Nina")
				{
					Money = 2000
				}
			};

			_sequence = new Sequence<Sample>(BufferSize);
			_stockObserver = new StockObserver(new StockPriceProvider(new RandomDefault()), CompanyName);

			_timerRefreshStockPrice = new Timer(RefreshStockPrice);
		}

		#region Binding propertes
		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<BrokerViewModel> Brokers { get; }

		public WpfPlot ChartWpfPlot { get; set; }

		private bool _getRealTimeData;
		public bool GetRealTimeData
		{
			get => _getRealTimeData;
			set
			{
				_getRealTimeData = value;
				SetTimerSetting(_timerRefreshStockPrice, value);
				OnPropertyChanged(nameof(_getRealTimeData));
			}
		}

		private bool _showAverageValue;
		public bool ShowAverageValue
		{
			get => _showAverageValue;
			set
			{
				_showAverageValue = value;
				OnPropertyChanged(nameof(_showAverageValue));
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		#endregion

		private void RefreshStockPrice(object obj)
		{
			_stockObserver.TryRefresh(ref _sequence);

			if (!_plotPrepared)
			{
				_scatterPlot = ChartWpfPlot.Plot.AddScatter(
					_sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
					_sequence.Select(s => (double)s.StockPrice).ToArray());
				ChartWpfPlot.Plot.XAxis.DateTimeFormat(true);
				_plotPrepared = true;
			}

			if (_showAverageValue)
			{
				var averageValue = (double)_sequence.Average(s => s.StockPrice);

				if (_averagePlot == null)
				{
					_averagePlot = ChartWpfPlot.Plot.AddScatter(
						_sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
						_sequence.Select(s => averageValue).ToArray(),
						markerSize: 0);

					ChartWpfPlot.Plot.AddText(
						Math.Round(averageValue, 4).ToString(CultureInfo.InvariantCulture),
						_averagePlot.Xs.First(), averageValue);
				}
				else
				{
					_averagePlot.Update(
						_sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
						_sequence.Select(s => averageValue).ToArray());

					ChartWpfPlot.Plot.Clear(typeof(Text));
					ChartWpfPlot.Plot.AddText(
						Math.Round(averageValue, 4).ToString(CultureInfo.InvariantCulture),
						_averagePlot.Xs.First(), averageValue);
				}
			}
			else if (_averagePlot != null)
			{
				ChartWpfPlot.Plot.Clear();
				ChartWpfPlot.Plot.Add(_scatterPlot);
				_averagePlot = null;
			}

			_scatterPlot.Update(
				_sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
				_sequence.Select(s => (double)s.StockPrice).ToArray());

			ChartWpfPlot.Plot.AxisAuto();
			ChartWpfPlot.Dispatcher.Invoke(() => ChartWpfPlot.Refresh());
		}

		private void SetTimerSetting(Timer timer, bool needWork)
		{
			var timeSpan = needWork
				? TimeSpan.FromSeconds(TimerTimeInSeconds)
				: TimeSpan.FromMilliseconds(-1);

			timer?.Change(timeSpan, timeSpan);
		}
	}
}
