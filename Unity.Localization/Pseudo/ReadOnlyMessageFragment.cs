using System;
using System.Diagnostics;
using UnityEngine.Pool;

namespace UnityEngine.Localization.Pseudo
{
	[DebuggerDisplay("ReadOnly: {Text}")]
	public class ReadOnlyMessageFragment : MessageFragment
	{
		public string Text
		{
			get
			{
				return this.ToString();
			}
		}

		internal static readonly ObjectPool<ReadOnlyMessageFragment> Pool = new ObjectPool<ReadOnlyMessageFragment>(() => new ReadOnlyMessageFragment(), null, null, null, false, 10, 10000);
	}
}
