using System;
using System.Diagnostics;
using UnityEngine.Pool;

namespace UnityEngine.Localization.Pseudo
{
	[DebuggerDisplay("Writable: {Text}")]
	public class WritableMessageFragment : MessageFragment
	{
		public string Text
		{
			get
			{
				return this.ToString();
			}
			set
			{
				base.Initialize(base.Message, value);
			}
		}

		internal static readonly ObjectPool<WritableMessageFragment> Pool = new ObjectPool<WritableMessageFragment>(() => new WritableMessageFragment(), null, null, null, false, 10, 10000);
	}
}
