using System;

namespace Liv.Lck
{
	[Serializable]
	public struct CameraResolutionDescriptor
	{
		public CameraResolutionDescriptor(uint width = 512U, uint height = 512U)
		{
			this.Width = width;
			this.Height = height;
		}

		public bool IsValid()
		{
			return this.Width > 0U && this.Height > 0U;
		}

		public CameraResolutionDescriptor GetResolutionInOrientation(LckCameraOrientation orientation)
		{
			CameraResolutionDescriptor result;
			if (orientation != LckCameraOrientation.Portrait)
			{
				if (orientation == LckCameraOrientation.Landscape)
				{
					result = new CameraResolutionDescriptor(Math.Max(this.Width, this.Height), Math.Min(this.Width, this.Height));
				}
				else
				{
					result = this;
				}
			}
			else
			{
				result = new CameraResolutionDescriptor(Math.Min(this.Width, this.Height), Math.Max(this.Width, this.Height));
			}
			return result;
		}

		public uint Width;

		public uint Height;
	}
}
