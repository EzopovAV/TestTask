using StreamingChart.Interfaces;
using StreamingChart.SystemCover.Interfaces;
using System;

namespace StreamingChart
{
	public class StockPriceProvider : IStockPriceProvider
	{
		private readonly IRandom _random;

		public StockPriceProvider(IRandom random)
		{
			_random = random ?? throw new ArgumentNullException(nameof(random));
		}
		public decimal GetCurrentStockPrice()
		{
			return (decimal)(100 * _random.NextDouble());
		}
	}
}
