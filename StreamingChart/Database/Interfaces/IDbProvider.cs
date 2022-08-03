using StreamingChart.Models;
using System.Collections.Generic;

namespace StreamingChart.Database.Interfaces
{
	public interface IDbProvider
	{
		void AddSamples(IEnumerable<Sample> samples);
		IEnumerable<Sample> GetSamples(int limit);
	}
}
