using System;

namespace Meta.Voice.Logging
{
	public readonly struct CorrelationID
	{
		private string Value { get; }

		private CorrelationID(string value)
		{
			this.Value = value;
		}

		public bool IsAssigned
		{
			get
			{
				return this.Value != null;
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		public static implicit operator string(CorrelationID correlationId)
		{
			return correlationId.Value;
		}

		public static explicit operator CorrelationID(string value)
		{
			return new CorrelationID(value);
		}

		public static implicit operator CorrelationID(Guid value)
		{
			return new CorrelationID(value.ToString());
		}

		public override bool Equals(object obj)
		{
			if (obj is CorrelationID)
			{
				CorrelationID correlationID = (CorrelationID)obj;
				return this.Value == correlationID.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}
	}
}
