using System;
using System.Collections.Generic;

namespace Modio.Images
{
	[Serializable]
	public struct ImageReference : IEquatable<ImageReference>
	{
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.Url);
			}
		}

		public string Url { readonly get; private set; }

		internal ImageReference(string url)
		{
			this.Url = url;
		}

		public static bool operator ==(ImageReference left, ImageReference right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ImageReference left, ImageReference right)
		{
			return !left.Equals(right);
		}

		public bool Equals(ImageReference other)
		{
			return this.Url == other.Url;
		}

		public override bool Equals(object obj)
		{
			if (obj is ImageReference)
			{
				ImageReference other = (ImageReference)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this.Url == null)
			{
				return 0;
			}
			return this.Url.GetHashCode();
		}

		private sealed class UrlEqualityComparer : IEqualityComparer<ImageReference>
		{
			public bool Equals(ImageReference x, ImageReference y)
			{
				return x.Url == y.Url;
			}

			public int GetHashCode(ImageReference obj)
			{
				if (obj.Url == null)
				{
					return 0;
				}
				return obj.Url.GetHashCode();
			}
		}
	}
}
