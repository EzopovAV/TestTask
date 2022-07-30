using StreamingChart.SystemCover.Interfaces;
using System;

namespace StreamingChart.SystemCover
{
	internal class RandomDefault : IRandom
	{
		private readonly Random _random;
		public RandomDefault()
		{
			_random = new Random();
		}
		public double NextDouble()
		{
			return _random.NextDouble();
		}
	}
}
