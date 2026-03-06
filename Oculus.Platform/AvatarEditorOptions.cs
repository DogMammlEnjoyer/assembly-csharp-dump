using System;

namespace Oculus.Platform
{
	public class AvatarEditorOptions
	{
		public AvatarEditorOptions()
		{
			this.Handle = CAPI.ovr_AvatarEditorOptions_Create();
		}

		public void SetSourceOverride(string value)
		{
			CAPI.ovr_AvatarEditorOptions_SetSourceOverride(this.Handle, value);
		}

		public static explicit operator IntPtr(AvatarEditorOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~AvatarEditorOptions()
		{
			CAPI.ovr_AvatarEditorOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
