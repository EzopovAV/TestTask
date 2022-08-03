using System;

namespace StreamingChart.Models
{
	public class Sample
	{
		public int Id { get; set; }
		public decimal StockPrice { get; set; }
		public string CompanyName { get; set; }
		public DateTime SamplingTime { get; set; }
	}
}
