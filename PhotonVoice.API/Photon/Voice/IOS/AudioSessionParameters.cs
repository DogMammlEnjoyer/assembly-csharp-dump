using System;

namespace Photon.Voice.IOS
{
	[Serializable]
	public struct AudioSessionParameters
	{
		public int CategoryOptionsToInt()
		{
			int num = 0;
			if (this.CategoryOptions != null)
			{
				for (int i = 0; i < this.CategoryOptions.Length; i++)
				{
					num |= (int)this.CategoryOptions[i];
				}
			}
			return num;
		}

		public override string ToString()
		{
			string text = "[";
			if (this.CategoryOptions != null)
			{
				for (int i = 0; i < this.CategoryOptions.Length; i++)
				{
					text += this.CategoryOptions[i].ToString();
					if (i != this.CategoryOptions.Length - 1)
					{
						text += ", ";
					}
				}
			}
			text += "]";
			return string.Format("category = {0}, mode = {1}, options = {2}", this.Category, this.Mode, text);
		}

		public AudioSessionCategory Category;

		public AudioSessionMode Mode;

		public AudioSessionCategoryOption[] CategoryOptions;
	}
}
