using System;

namespace System.IO
{
	internal struct DisableMediaInsertionPrompt : IDisposable
	{
		public static DisableMediaInsertionPrompt Create()
		{
			DisableMediaInsertionPrompt result = default(DisableMediaInsertionPrompt);
			result._disableSuccess = Interop.Kernel32.SetThreadErrorMode(1U, out result._oldMode);
			return result;
		}

		public void Dispose()
		{
			if (this._disableSuccess)
			{
				uint num;
				Interop.Kernel32.SetThreadErrorMode(this._oldMode, out num);
			}
		}

		private bool _disableSuccess;

		private uint _oldMode;
	}
}
