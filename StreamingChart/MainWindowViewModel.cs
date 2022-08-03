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
using System.Windows;
using StreamingChart.Database;
using StreamingChart.Database.Interfaces;
using StreamingChart.Properties;

namespace StreamingChart
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private const int TimerTimeInSeconds = 1;
		private const int BufferSize = 100;
		private const string CompanyName = "Perm Dynamics";
		
		private readonly IStockObserver _stockObserver;
		private readonly IDbProvider _dbProvider;
		private readonly Timer _timerRefreshStockPrice;

		private Sequence<Sample> _sequenceRealTime;
		private Sequence<Sample> _sequenceDbLoaded;
		private Sequence<Sample> _sequenceCurrent;

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

			_sequenceRealTime = new Sequence<Sample>(BufferSize);
			_stockObserver = new StockObserver(new StockPriceProvider(new RandomDefault()), CompanyName);

			_dbProvider = new DbProvider();

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
				RefreshWpfPlot(_sequenceCurrent);
				OnPropertyChanged(nameof(_showAverageValue));
			}
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			set
			{
				_isBusy = value;
				OnPropertyChanged(nameof(IsBusy));
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		#endregion

		#region Commands

		private RelayCommand _saveSamplesCommand;
		public RelayCommand SaveSamplesCommand =>
			_saveSamplesCommand ??
			(_saveSamplesCommand = new RelayCommand(obj =>
			{
				try
				{
					IsBusy = true;
					_dbProvider.AddSamples(_sequenceRealTime);

					MessageBox.Show(Resources.MessegeAfterSavingData, Resources.Notification,
						MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
				finally
				{
					IsBusy = false;
				}
			}));

		private RelayCommand _loadSamplesCommand;
		public RelayCommand LoadSamplesCommand =>
			_loadSamplesCommand ??
			(_loadSamplesCommand = new RelayCommand(obj =>
			{
				try
				{
					IsBusy = true;

					var samples = _dbProvider.GetSamples(100).ToArray();
					_sequenceDbLoaded = new Sequence<Sample>(samples.Count());
					foreach (var sample in samples)
					{
						_sequenceDbLoaded.PutNewSample(sample);
					}

					_sequenceCurrent = _sequenceDbLoaded;
					RefreshWpfPlot(_sequenceDbLoaded);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, Resources.Error, MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
				finally
				{
					IsBusy = false;
				}
			}));

		#endregion

		private void RefreshStockPrice(object obj)
		{
			_stockObserver.TryRefresh(ref _sequenceRealTime);
			_sequenceCurrent = _sequenceRealTime;

			RefreshWpfPlot(_sequenceRealTime);
		}

		private void RefreshWpfPlot(Sequence<Sample> sequence)
		{
			if (sequence == null)
			{
				return;
			}

			if (!_plotPrepared)
			{
				_scatterPlot = ChartWpfPlot.Plot.AddScatter(
					sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
					sequence.Select(s => (double)s.StockPrice).ToArray());
				ChartWpfPlot.Plot.XAxis.DateTimeFormat(true);
				_plotPrepared = true;
			}

			if (_showAverageValue)
			{
				var averageValue = (double)sequence.Average(s => s.StockPrice);

				if (_averagePlot == null)
				{
					_averagePlot = ChartWpfPlot.Plot.AddScatter(
						sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
						sequence.Select(s => averageValue).ToArray(),
						markerSize: 0);

					ChartWpfPlot.Plot.AddText(
						Math.Round(averageValue, 4).ToString(CultureInfo.InvariantCulture),
						_averagePlot.Xs.First(), averageValue);
				}
				else
				{
					_averagePlot.Update(
						sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
						sequence.Select(s => averageValue).ToArray());

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
				sequence.Select(s => s.SamplingTime.ToOADate()).ToArray(),
				sequence.Select(s => (double)s.StockPrice).ToArray());

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
