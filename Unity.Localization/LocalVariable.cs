using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	internal class LocalVariable
	{
		[UxmlAttribute]
		[Delayed]
		public string Name { get; set; }

		[UxmlObjectReference]
		public IVariable Variable { get; set; }

		[CompilerGenerated]
		[Serializable]
		internal class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(LocalVariable.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("Name", "name", null, Array.Empty<string>()),
					new UxmlAttributeNames("Variable", "variable", null, Array.Empty<string>())
				});
			}

			public override object CreateInstance()
			{
				return new LocalVariable();
			}

			public override void Deserialize(object obj)
			{
				LocalVariable localVariable = (LocalVariable)obj;
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Name_UxmlAttributeFlags))
				{
					localVariable.Name = this.Name;
				}
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Variable_UxmlAttributeFlags))
				{
					if (this.Variable != null)
					{
						IVariable variable = (IVariable)this.Variable.CreateInstance();
						this.Variable.Deserialize(variable);
						localVariable.Variable = variable;
						return;
					}
					localVariable.Variable = null;
				}
			}

			[UxmlObjectReference]
			[SerializeReference]
			private UnityEngine.UIElements.UxmlSerializedData Variable;

			[Delayed]
			[SerializeField]
			private string Name;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Variable_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Name_UxmlAttributeFlags;
		}
	}
}
