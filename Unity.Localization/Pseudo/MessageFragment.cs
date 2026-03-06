using System;
using System.Text;

namespace UnityEngine.Localization.Pseudo
{
	public abstract class MessageFragment
	{
		public int Length
		{
			get
			{
				if (this.m_StartIndex != -1)
				{
					return this.m_EndIndex - this.m_StartIndex;
				}
				return this.m_OriginalString.Length;
			}
		}

		public Message Message { get; private set; }

		internal void Initialize(Message parent, string original, int start, int end)
		{
			this.Message = parent;
			this.m_OriginalString = original;
			this.m_StartIndex = start;
			this.m_EndIndex = end;
			this.m_CachedToString = null;
		}

		internal void Initialize(Message parent, string text)
		{
			this.Message = parent;
			this.m_OriginalString = text;
			this.m_StartIndex = -1;
			this.m_EndIndex = -1;
			this.m_CachedToString = null;
		}

		public WritableMessageFragment CreateTextFragment(int start, int end)
		{
			WritableMessageFragment writableMessageFragment = WritableMessageFragment.Pool.Get();
			int start2 = (this.m_StartIndex == -1) ? start : (this.m_StartIndex + start);
			int end2 = (this.m_StartIndex == -1) ? end : (this.m_StartIndex + end);
			writableMessageFragment.Initialize(this.Message, this.m_OriginalString, start2, end2);
			return writableMessageFragment;
		}

		public ReadOnlyMessageFragment CreateReadonlyTextFragment(int start, int end)
		{
			ReadOnlyMessageFragment readOnlyMessageFragment = ReadOnlyMessageFragment.Pool.Get();
			int start2 = (this.m_StartIndex == -1) ? start : (this.m_StartIndex + start);
			int end2 = (this.m_StartIndex == -1) ? end : (this.m_StartIndex + end);
			readOnlyMessageFragment.Initialize(this.Message, this.m_OriginalString, start2, end2);
			return readOnlyMessageFragment;
		}

		public override string ToString()
		{
			if (this.m_CachedToString == null)
			{
				this.m_CachedToString = ((this.m_StartIndex == -1) ? this.m_OriginalString : this.m_OriginalString.Substring(this.m_StartIndex, this.m_EndIndex - this.m_StartIndex));
			}
			return this.m_CachedToString;
		}

		internal void BuildString(StringBuilder builder)
		{
			if (this.m_StartIndex == -1)
			{
				builder.Append(this.m_OriginalString);
				return;
			}
			builder.Append(this.m_OriginalString, this.m_StartIndex, this.m_EndIndex - this.m_StartIndex);
		}

		public char this[int index]
		{
			get
			{
				int num = (this.m_StartIndex == -1) ? 0 : this.m_StartIndex;
				return this.m_OriginalString[num + index];
			}
		}

		protected string m_OriginalString;

		protected int m_StartIndex;

		protected int m_EndIndex;

		private string m_CachedToString;
	}
}
