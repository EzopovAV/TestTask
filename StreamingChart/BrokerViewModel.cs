using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StreamingChart
{
	public class BrokerViewModel : INotifyPropertyChanged
	{
		public BrokerViewModel(long id, string name)
		{
			Id = id;
			Name = name;
		}

		#region Binding propertes
		public event PropertyChangedEventHandler PropertyChanged;

		public long Id { get; }

		public string Name { get; }
		
		private decimal _stock;
		public decimal Stock
		{
			get => _stock;
			set
			{
				_stock = value;
				OnPropertyChanged(nameof(_stock));
			}
		}

		private decimal _money;
		public decimal Money
		{
			get => _money;
			set
			{
				_money = value;
				OnPropertyChanged(nameof(_money));
			}
		}

		private DateTime _lastActivityDataTime;
		public DateTime LastActivityDataTime
		{
			get => _lastActivityDataTime;
			set
			{
				_lastActivityDataTime = value;
				OnPropertyChanged(nameof(_lastActivityDataTime));
			}
		}

		public string LastActivityActionString { get; set; }

		protected void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		#endregion
	}
}
