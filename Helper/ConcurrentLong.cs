using System.Threading;

namespace Helper
{
	public struct ConcurrentLong
	{
		private long _value;

		// Standardkonstruktor für C# 7.3
		public ConcurrentLong(long initial)
		{
			_value = initial;
		}

		public long Value
		{
			get => Interlocked.Read(ref _value);
			set => Interlocked.Exchange(ref _value, value);
		}

		public static ConcurrentLong operator ++(ConcurrentLong x)
		{
			Interlocked.Increment(ref x._value);
			return x;
		}

		public static ConcurrentLong operator --(ConcurrentLong x)
		{
			Interlocked.Decrement(ref x._value);
			return x;
		}

		public static implicit operator long(ConcurrentLong x) => x.Value;

		public static implicit operator ConcurrentLong(long v) => new ConcurrentLong(v);

		public static ConcurrentLong operator +(ConcurrentLong x, long y)
		{
			Interlocked.Add(ref x._value, y);
			return x;
		}

		public static ConcurrentLong operator -(ConcurrentLong x, long y)
		{
			Interlocked.Add(ref x._value, -y);
			return x;
		}
	}
}