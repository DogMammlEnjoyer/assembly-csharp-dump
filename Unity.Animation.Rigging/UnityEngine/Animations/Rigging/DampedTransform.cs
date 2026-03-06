using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Damped Transform")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/DampedTransform.html")]
	public class DampedTransform : RigConstraint<DampedTransformJob, DampedTransformData, DampedTransformJobBinder<DampedTransformData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Data.dampPosition = Mathf.Clamp01(this.m_Data.dampPosition);
			this.m_Data.dampRotation = Mathf.Clamp01(this.m_Data.dampRotation);
		}
	}
}
