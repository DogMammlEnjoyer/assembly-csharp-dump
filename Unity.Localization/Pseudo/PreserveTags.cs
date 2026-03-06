using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.Localization.Pseudo
{
	[Serializable]
	public class PreserveTags : IPseudoLocalizationMethod
	{
		public char Opening
		{
			get
			{
				return this.m_Opening;
			}
			set
			{
				this.m_Opening = value;
			}
		}

		public char Closing
		{
			get
			{
				return this.m_Closing;
			}
			set
			{
				this.m_Closing = value;
			}
		}

		public void Transform(Message message)
		{
			List<MessageFragment> list;
			using (CollectionPool<List<MessageFragment>, MessageFragment>.Get(out list))
			{
				for (int i = 0; i < message.Fragments.Count; i++)
				{
					int num = 0;
					int num2 = -1;
					MessageFragment messageFragment = message.Fragments[i];
					WritableMessageFragment writableMessageFragment = messageFragment as WritableMessageFragment;
					if (writableMessageFragment != null)
					{
						for (int j = 0; j < messageFragment.Length; j++)
						{
							if (messageFragment[j] == this.m_Opening)
							{
								num2 = j;
							}
							else if (messageFragment[j] == this.m_Closing && num2 != -1)
							{
								int end = j + 1;
								if (num != num2)
								{
									list.Add(writableMessageFragment.CreateTextFragment(num, num2));
								}
								list.Add(writableMessageFragment.CreateReadonlyTextFragment(num2, end));
								num2 = -1;
								num = j + 1;
							}
						}
						message.ReleaseFragment(messageFragment);
						if (num != messageFragment.Length)
						{
							list.Add(writableMessageFragment.CreateTextFragment(num, messageFragment.Length));
						}
					}
					else
					{
						list.Add(messageFragment);
					}
				}
				message.Fragments.Clear();
				message.Fragments.AddRange(list);
			}
		}

		[SerializeField]
		private char m_Opening = '<';

		[SerializeField]
		private char m_Closing = '>';
	}
}
