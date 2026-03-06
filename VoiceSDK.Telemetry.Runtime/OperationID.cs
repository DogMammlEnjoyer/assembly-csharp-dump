using System;

namespace Meta.Voice.TelemetryUtilities
{
	public readonly struct OperationID
	{
		private string Value { get; }

		public OperationID(string value)
		{
			if (value == null)
			{
				value = Guid.NewGuid().ToString();
			}
			this.Value = value;
		}

		public static OperationID Create(string value = null)
		{
			return new OperationID(value);
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

		public static implicit operator string(OperationID correlationId)
		{
			return correlationId.Value;
		}

		public static explicit operator OperationID(string value)
		{
			return new OperationID(value);
		}

		public static implicit operator OperationID(Guid value)
		{
			return new OperationID(value.ToString());
		}

		public override bool Equals(object obj)
		{
			if (obj is OperationID)
			{
				OperationID operationID = (OperationID)obj;
				return this.Value == operationID.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this.IsAssigned)
			{
				return this.Value.GetHashCode();
			}
			return 0;
		}

		public static readonly OperationID INVALID;
	}
}
