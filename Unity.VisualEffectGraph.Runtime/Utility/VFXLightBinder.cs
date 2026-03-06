using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Light Binder")]
	[VFXBinder("Utility/Light")]
	internal class VFXLightBinder : VFXBinderBase
	{
		public string ColorProperty
		{
			get
			{
				return (string)this.m_ColorProperty;
			}
			set
			{
				this.m_ColorProperty = value;
			}
		}

		public string BrightnessProperty
		{
			get
			{
				return (string)this.m_BrightnessProperty;
			}
			set
			{
				this.m_ColorProperty = value;
			}
		}

		public string RadiusProperty
		{
			get
			{
				return (string)this.m_RadiusProperty;
			}
			set
			{
				this.m_RadiusProperty = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && (!this.BindColor || component.HasVector4(this.ColorProperty)) && (!this.BindBrightness || component.HasFloat(this.BrightnessProperty)) && (!this.BindRadius || component.HasFloat(this.RadiusProperty));
		}

		public override void UpdateBinding(VisualEffect component)
		{
			if (this.BindColor)
			{
				component.SetVector4(this.ColorProperty, this.Target.color);
			}
			if (this.BindBrightness)
			{
				component.SetFloat(this.BrightnessProperty, this.Target.intensity);
			}
			if (this.BindRadius)
			{
				component.SetFloat(this.RadiusProperty, this.Target.range);
			}
		}

		public override string ToString()
		{
			return string.Format("Light : '{0}' -> {1}", this.m_ColorProperty, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Color"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_ColorParameter")]
		protected ExposedProperty m_ColorProperty = "Color";

		[VFXPropertyBinding(new string[]
		{
			"System.Single"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_BrightnessParameter")]
		protected ExposedProperty m_BrightnessProperty = "Brightness";

		[VFXPropertyBinding(new string[]
		{
			"System.Single"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_RadiusParameter")]
		protected ExposedProperty m_RadiusProperty = "Radius";

		public Light Target;

		public bool BindColor = true;

		public bool BindBrightness;

		public bool BindRadius;
	}
}
