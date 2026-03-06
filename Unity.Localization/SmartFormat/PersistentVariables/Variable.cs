using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[Serializable]
	public class Variable<T> : IVariableValueChanged, IVariable
	{
		[UxmlAttribute("value")]
		public T ValueUXML
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		public event Action<IVariable> ValueChanged;

		public T Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (this.m_Value != null && this.m_Value.Equals(value))
				{
					return;
				}
				this.m_Value = value;
				this.SendValueChangedEvent();
			}
		}

		public object GetSourceValue(ISelectorInfo _)
		{
			return this.Value;
		}

		private void SendValueChangedEvent()
		{
			Action<IVariable> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(this);
		}

		public override string ToString()
		{
			T value = this.Value;
			return value.ToString();
		}

		[SerializeField]
		private T m_Value;

		[CompilerGenerated]
		[Serializable]
		public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Variable<T>.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("ValueUXML", "value", null, Array.Empty<string>())
				});
			}

			public override object CreateInstance()
			{
				return new Variable<T>();
			}

			public override void Deserialize(object obj)
			{
				Variable<T> variable = (Variable<T>)obj;
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.ValueUXML_UxmlAttributeFlags))
				{
					variable.ValueUXML = this.ValueUXML;
				}
			}

			[UxmlAttribute("value")]
			[SerializeField]
			private T ValueUXML;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags ValueUXML_UxmlAttributeFlags;
		}
	}
}
