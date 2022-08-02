using StreamingChart.Interfaces;
using StreamingChart.Models;
using System;

namespace StreamingChart
{
	internal class StockObserver : IStockObserver
	{
		private readonly IStockPriceProvider _stockPriceProvider;
		private readonly string _companyName;
		private Exception _innerException;

		public StockObserver(IStockPriceProvider stockPriceProvider, string companyName)
		{
			_stockPriceProvider = stockPriceProvider ?? throw new ArgumentNullException(nameof(stockPriceProvider));
			
			if (string.IsNullOrEmpty(companyName))
			{
				throw new ArgumentNullException(nameof(companyName));
			}
			_companyName = companyName;
		}

		public Exception InnerException => _innerException;
		public bool TryRefresh(ref Sequence<Sample> sequence)
		{
			var newSample = new Sample
			{
				CompanyName = _companyName,
				SamplingTime = DateTime.UtcNow
			};

			try
			{
				newSample.StockPrice = _stockPriceProvider.GetCurrentStockPrice();
				sequence.PutNewSample(newSample);

				return true;
			}
			catch (Exception e)
			{
				_innerException = e;
				return false;
			}
		}
	}
}
