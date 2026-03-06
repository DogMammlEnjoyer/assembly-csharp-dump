using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Input Key Press Binder")]
	[VFXBinder("Input/Key")]
	internal class VFXInputKeyBinder : VFXBinderBase
	{
		public string KeyProperty
		{
			get
			{
				return (string)this.m_KeyProperty;
			}
			set
			{
				this.m_KeyProperty = value;
			}
		}

		public string KeySmoothProperty
		{
			get
			{
				return (string)this.m_KeySmoothProperty;
			}
			set
			{
				this.m_KeySmoothProperty = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return component.HasBool(this.m_KeyProperty) && (!this.UseKeySmooth || component.HasFloat(this.m_KeySmoothProperty));
		}

		private void Start()
		{
		}

		public override void UpdateBinding(VisualEffect component)
		{
		}

		public override string ToString()
		{
			return string.Format("Key: '{0}' -> {1}", this.m_KeySmoothProperty, this.Key.ToString());
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_KeyParameter")]
		protected ExposedProperty m_KeyProperty = "KeyDown";

		[VFXPropertyBinding(new string[]
		{
			"System.Single"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_KeySmoothParameter")]
		protected ExposedProperty m_KeySmoothProperty = "KeySmooth";

		public KeyCode Key = KeyCode.Space;

		public float SmoothSpeed = 2f;

		public bool UseKeySmooth = true;
	}
}
