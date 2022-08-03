using StreamingChart.Database.Interfaces;
using StreamingChart.Models;
using System.Collections.Generic;
using System.Linq;

namespace StreamingChart.Database
{
	public class DbProvider : IDbProvider
	{
		public void AddSamples(IEnumerable<Sample> samples)
		{
			using (var db = new SampleDbContext())
			{
				foreach (var sample in samples)
				{
					db.Samples.Add(sample);
				}

				db.SaveChanges();
			}
		}

		public IEnumerable<Sample> GetSamples(int limit)
		{
			using (var db = new SampleDbContext())
			{
				return db.Samples.Take(limit).ToArray();
			}
		}
	}
}
