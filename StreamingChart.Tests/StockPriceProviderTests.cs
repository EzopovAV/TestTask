using Moq;
using NUnit.Framework;
using StreamingChart.SystemCover.Interfaces;
using System;

namespace StreamingChart.Tests
{
	[TestFixture]
    public class StockPriceProviderTests
    {
	    [Test]
	    public void ExceptionTest()
	    {
		    Assert.Throws<ArgumentNullException>(() => 
			    new StockPriceProvider(null).GetCurrentStockPrice());
	    }

	    [Test]
	    public void ReturnPositiveValueTest()
	    {
		    var random = new Mock<IRandom>();
		    random.Setup(r => r.NextDouble()).Returns(0.17);

			var stockPriceProvider = new StockPriceProvider(random.Object);

			Assert.That(stockPriceProvider.GetCurrentStockPrice() > 0);
	    }

	    [Test]
	    public void ReturnDifferentValuesTest()
	    {
		    var random = new Mock<IRandom>();
		    random.Setup(r => r.NextDouble()).Returns(0.17);

		    var stockPrice1 = new StockPriceProvider(random.Object).GetCurrentStockPrice();

		    random.Setup(r => r.NextDouble()).Returns(0.3);
		    var stockPrice2 = new StockPriceProvider(random.Object).GetCurrentStockPrice();

			Assert.AreNotEqual(stockPrice1, stockPrice2);
	    }
	}
}
