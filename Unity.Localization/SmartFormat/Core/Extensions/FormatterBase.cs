using System;

namespace UnityEngine.Localization.SmartFormat.Core.Extensions
{
	public abstract class FormatterBase : IFormatter, ISerializationCallbackReceiver
	{
		public string[] Names
		{
			get
			{
				return this.m_Names;
			}
			set
			{
				this.m_Names = value;
			}
		}

		public abstract string[] DefaultNames { get; }

		public abstract bool TryEvaluateFormat(IFormattingInfo formattingInfo);

		public virtual void OnAfterDeserialize()
		{
			if (this.Names == null || this.Names.Length == 0)
			{
				this.Names = this.DefaultNames;
			}
		}

		public void OnBeforeSerialize()
		{
		}

		[SerializeField]
		private string[] m_Names;
	}
}
