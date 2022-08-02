using NUnit.Framework;
using StreamingChart.Models;
using System.Linq;

namespace StreamingChart.Tests
{
	[TestFixture]
	public class SequenceTests
	{
		[Test]
		[TestCase(1)]
		[TestCase(17)]
		public void CreatingTest(int expectedCapacity)
		{
			var sequence = new Sequence<int>(expectedCapacity);

			Assert.That(sequence.Capacity == expectedCapacity);
			Assert.That(sequence.Length == 0);
			Assert.That(sequence.ToArray().Length == 0);
		}

		[Test]
		public void FullFillingTest()
		{
			var inputArray = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
			var sequence = new Sequence<int>(inputArray.Length);

			foreach (var t in inputArray)
			{
				sequence.PutNewSample(t);
			}

			var result = sequence.ToArray();

			Assert.AreEqual(result, inputArray);
		}

		[Test]
		public void PartialFillingTest()
		{
			var inputArray = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
			var sequence = new Sequence<int>(inputArray.Length * 3);

			foreach (var t in inputArray)
			{
				sequence.PutNewSample(t);
			}

			var result = sequence.ToArray();

			Assert.AreEqual(result, inputArray);
		}

		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[TestCase(6)]
		[TestCase(7)]
		public void OverflowTest(int overflow)
		{
			var inputArray = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
			var sequence = new Sequence<int>(inputArray.Length - overflow);

			foreach (var t in inputArray)
			{
				sequence.PutNewSample(t);
			}

			var result = sequence.ToArray();
			var expectedArray = inputArray.Skip(overflow).ToArray();

			Assert.AreEqual(result, expectedArray);
		}
	}
}
