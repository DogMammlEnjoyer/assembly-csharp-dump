using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Pool;

namespace UnityEngine.Localization.Pseudo
{
	public class Message
	{
		public string Original { get; private set; }

		public List<MessageFragment> Fragments { get; private set; } = new List<MessageFragment>();

		public int Length
		{
			get
			{
				int num = 0;
				foreach (MessageFragment messageFragment in this.Fragments)
				{
					num += messageFragment.Length;
				}
				return num;
			}
		}

		public WritableMessageFragment CreateTextFragment(string original, int start, int end)
		{
			WritableMessageFragment writableMessageFragment = WritableMessageFragment.Pool.Get();
			writableMessageFragment.Initialize(this, original, start, end);
			return writableMessageFragment;
		}

		public WritableMessageFragment CreateTextFragment(string original)
		{
			WritableMessageFragment writableMessageFragment = WritableMessageFragment.Pool.Get();
			writableMessageFragment.Initialize(this, original);
			return writableMessageFragment;
		}

		public ReadOnlyMessageFragment CreateReadonlyTextFragment(string original, int start, int end)
		{
			ReadOnlyMessageFragment readOnlyMessageFragment = ReadOnlyMessageFragment.Pool.Get();
			readOnlyMessageFragment.Initialize(this, original, start, end);
			return readOnlyMessageFragment;
		}

		public ReadOnlyMessageFragment CreateReadonlyTextFragment(string original)
		{
			ReadOnlyMessageFragment readOnlyMessageFragment = ReadOnlyMessageFragment.Pool.Get();
			readOnlyMessageFragment.Initialize(this, original);
			return readOnlyMessageFragment;
		}

		public void ReplaceFragment(MessageFragment original, MessageFragment replacement)
		{
			int num = this.Fragments.IndexOf(original);
			if (num == -1)
			{
				throw new Exception("Can not replace Fragment " + original.ToString() + " that is not part of the message.");
			}
			this.Fragments[num] = replacement;
			this.ReleaseFragment(original);
		}

		public void ReleaseFragment(MessageFragment fragment)
		{
			WritableMessageFragment writableMessageFragment = fragment as WritableMessageFragment;
			if (writableMessageFragment != null)
			{
				WritableMessageFragment.Pool.Release(writableMessageFragment);
				return;
			}
			ReadOnlyMessageFragment readOnlyMessageFragment = fragment as ReadOnlyMessageFragment;
			if (readOnlyMessageFragment != null)
			{
				ReadOnlyMessageFragment.Pool.Release(readOnlyMessageFragment);
			}
		}

		internal static Message CreateMessage(string text)
		{
			Message message = Message.Pool.Get();
			message.Fragments.Add(message.CreateTextFragment(text));
			message.Original = text;
			return message;
		}

		internal void Release()
		{
			foreach (MessageFragment fragment in this.Fragments)
			{
				this.ReleaseFragment(fragment);
			}
			this.Fragments.Clear();
			Message.Pool.Release(this);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder;
			string result;
			using (StringBuilderPool.Get(out stringBuilder))
			{
				foreach (MessageFragment messageFragment in this.Fragments)
				{
					messageFragment.BuildString(stringBuilder);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		internal static readonly ObjectPool<Message> Pool = new ObjectPool<Message>(() => new Message(), null, null, null, false, 10, 10000);
	}
}
