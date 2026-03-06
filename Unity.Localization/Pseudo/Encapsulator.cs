using System;

namespace UnityEngine.Localization.Pseudo
{
	[Serializable]
	public class Encapsulator : IPseudoLocalizationMethod
	{
		public string Start
		{
			get
			{
				return this.m_Start;
			}
			set
			{
				this.m_Start = value;
			}
		}

		public string End
		{
			get
			{
				return this.m_End;
			}
			set
			{
				this.m_End = value;
			}
		}

		public void Transform(Message message)
		{
			ReadOnlyMessageFragment item = message.CreateReadonlyTextFragment(this.Start);
			ReadOnlyMessageFragment item2 = message.CreateReadonlyTextFragment(this.End);
			message.Fragments.Insert(0, item);
			message.Fragments.Add(item2);
		}

		[SerializeField]
		private string m_Start = "[";

		[SerializeField]
		private string m_End = "]";
	}
}
