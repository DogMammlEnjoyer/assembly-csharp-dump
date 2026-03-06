using System;

namespace UnityEngine.InputSystem.Users
{
	public struct InputUserAccountHandle : IEquatable<InputUserAccountHandle>
	{
		public string apiName
		{
			get
			{
				return this.m_ApiName;
			}
		}

		public ulong handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		public InputUserAccountHandle(string apiName, ulong handle)
		{
			if (string.IsNullOrEmpty(apiName))
			{
				throw new ArgumentNullException("apiName");
			}
			this.m_ApiName = apiName;
			this.m_Handle = handle;
		}

		public override string ToString()
		{
			if (this.m_ApiName == null)
			{
				return base.ToString();
			}
			return string.Format("{0}({1})", this.m_ApiName, this.m_Handle);
		}

		public bool Equals(InputUserAccountHandle other)
		{
			return string.Equals(this.apiName, other.apiName) && object.Equals(this.handle, other.handle);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is InputUserAccountHandle && this.Equals((InputUserAccountHandle)obj);
		}

		public static bool operator ==(InputUserAccountHandle left, InputUserAccountHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputUserAccountHandle left, InputUserAccountHandle right)
		{
			return !left.Equals(right);
		}

		public override int GetHashCode()
		{
			return ((this.apiName != null) ? this.apiName.GetHashCode() : 0) * 397 ^ this.handle.GetHashCode();
		}

		private string m_ApiName;

		private ulong m_Handle;
	}
}
