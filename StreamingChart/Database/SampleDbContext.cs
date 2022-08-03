using StreamingChart.Models;
using System.Data.Entity;

namespace StreamingChart.Database
{
	internal class SampleDbContext : DbContext
	{
		public SampleDbContext() : base("DbConnection")
		{
		}

		public DbSet<Sample> Samples { get; set; }
	}
}
