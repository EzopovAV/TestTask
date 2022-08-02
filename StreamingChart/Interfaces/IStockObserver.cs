using StreamingChart.Models;

namespace StreamingChart.Interfaces
{
	public interface IStockObserver
	{
		bool TryRefresh(ref Sequence<Sample> sequence);
	}
}
