using System;
using Unity;

namespace System.Security.Authentication.ExtendedProtection
{
	/// <summary>Contains APIs used for token binding.</summary>
	public class TokenBinding
	{
		internal TokenBinding(TokenBindingType bindingType, byte[] rawData)
		{
			this.BindingType = bindingType;
			this._rawTokenBindingId = rawData;
		}

		/// <summary>Gets the raw token binding Id.</summary>
		/// <returns>The raw token binding Id. The first byte of the raw Id, which represents the binding type, is removed.</returns>
		public byte[] GetRawTokenBindingId()
		{
			if (this._rawTokenBindingId == null)
			{
				return null;
			}
			return (byte[])this._rawTokenBindingId.Clone();
		}

		/// <summary>Gets the token binding type.</summary>
		/// <returns>The token binding type.</returns>
		public TokenBindingType BindingType { get; private set; }

		internal TokenBinding()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private byte[] _rawTokenBindingId;
	}
}
