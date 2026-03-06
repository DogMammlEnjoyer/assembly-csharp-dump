using System;

namespace UnityEngine.Localization.Pseudo
{
	public class Mirror : IPseudoLocalizationMethod
	{
		public void Transform(Message message)
		{
			foreach (MessageFragment messageFragment in message.Fragments)
			{
				WritableMessageFragment writableMessageFragment = messageFragment as WritableMessageFragment;
				if (writableMessageFragment != null)
				{
					this.MirrorFragment(writableMessageFragment);
				}
			}
		}

		private void MirrorFragment(WritableMessageFragment writableMessageFragment)
		{
			char[] array = new char[writableMessageFragment.Length];
			int i = writableMessageFragment.Length - 1;
			int num;
			for (int j = writableMessageFragment.Length - 1; j >= 0; j--)
			{
				if (writableMessageFragment[j] == '\n')
				{
					array[j] = '\n';
					num = j + 1;
					while (i > j)
					{
						array[num++] = writableMessageFragment[i--];
					}
					i = j - 1;
				}
			}
			num = 0;
			while (i >= 0)
			{
				array[num++] = writableMessageFragment[i--];
			}
			writableMessageFragment.Text = new string(array);
		}
	}
}
