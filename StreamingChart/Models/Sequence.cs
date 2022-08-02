using System;
using System.Collections;
using System.Collections.Generic;

namespace StreamingChart.Models
{
	public class Sequence<T> : IEnumerable<T>
	{
		private readonly T[] _samples;
		private int _head;
		private int _tail = -1;
		private int _length;

		public Sequence(int capacity)
		{
			if (capacity < 1)
			{
				throw new ArgumentException(@"Capacity can not be less that 1", nameof(capacity));
			}
			_samples = new T[capacity];
			Capacity = capacity;
		}

		public int Length => _length;
		public int Capacity { get; }

		public void PutNewSample(T sample)
		{
			_tail = GetNextIndex(_tail);
			_samples[_tail] = sample;

			if (_tail == _head && _length != 0)
			{
				_head = GetNextIndex(_head);
			}
			else
			{
				_length++;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_length == 0)
			{
				yield break;
			}

			var currentIndex = _head;

			yield return _samples[currentIndex];
			while (currentIndex != _tail)
			{
				currentIndex = GetNextIndex(currentIndex);
				yield return _samples[currentIndex];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this).GetEnumerator();
		}

		private int GetNextIndex(int currentIndex)
		{
			return (currentIndex + 1) % Capacity;
		}
	}
}
