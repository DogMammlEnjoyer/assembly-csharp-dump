using System;

namespace Fusion
{
	public struct NetworkRunnerUpdaterDefaultInvokeSettings : IEquatable<NetworkRunnerUpdaterDefaultInvokeSettings>
	{
		public bool Equals(NetworkRunnerUpdaterDefaultInvokeSettings other)
		{
			return this.ReferencePlayerLoopSystem == other.ReferencePlayerLoopSystem && this.AddMode == other.AddMode;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkRunnerUpdaterDefaultInvokeSettings)
			{
				NetworkRunnerUpdaterDefaultInvokeSettings other = (NetworkRunnerUpdaterDefaultInvokeSettings)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return ((this.ReferencePlayerLoopSystem != null) ? this.ReferencePlayerLoopSystem.GetHashCode() : 0) * 397 ^ (int)this.AddMode;
		}

		public override string ToString()
		{
			string format = "[{0}, {1}]";
			Type referencePlayerLoopSystem = this.ReferencePlayerLoopSystem;
			return string.Format(format, (referencePlayerLoopSystem != null) ? referencePlayerLoopSystem.FullName : null, this.AddMode);
		}

		public static bool operator ==(NetworkRunnerUpdaterDefaultInvokeSettings left, NetworkRunnerUpdaterDefaultInvokeSettings right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NetworkRunnerUpdaterDefaultInvokeSettings left, NetworkRunnerUpdaterDefaultInvokeSettings right)
		{
			return !left.Equals(right);
		}

		public Type ReferencePlayerLoopSystem;

		public UnityPlayerLoopSystemAddMode AddMode;
	}
}
