using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Input Button Binder")]
	[VFXBinder("Input/Button")]
	internal class VFXInputButtonBinder : VFXBinderBase
	{
		public string ButtonProperty
		{
			get
			{
				return (string)this.m_ButtonProperty;
			}
			set
			{
				this.m_ButtonProperty = value;
			}
		}

		public string ButtonSmoothProperty
		{
			get
			{
				return (string)this.m_ButtonSmoothProperty;
			}
			set
			{
				this.m_ButtonSmoothProperty = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return component.HasBool(this.m_ButtonProperty) && (!this.UseButtonSmooth || component.HasFloat(this.m_ButtonSmoothProperty));
		}

		private void Start()
		{
		}

		public override void UpdateBinding(VisualEffect component)
		{
		}

		public override string ToString()
		{
			return string.Format("Input Button: '{0}' -> {1}", this.m_ButtonSmoothProperty, this.ButtonName.ToString());
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_ButtonParameter")]
		protected ExposedProperty m_ButtonProperty = "ButtonDown";

		[VFXPropertyBinding(new string[]
		{
			"System.Single"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_ButtonSmoothParameter")]
		protected ExposedProperty m_ButtonSmoothProperty = "KeySmooth";

		public string ButtonName = "Action";

		public float SmoothSpeed = 2f;

		public bool UseButtonSmooth = true;
	}
}
