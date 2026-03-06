using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class AnimationCurveParameter : VolumeParameter<AnimationCurve>
	{
		public AnimationCurveParameter(AnimationCurve value, bool overrideState = false) : base(value, overrideState)
		{
		}

		public override void Interp(AnimationCurve lhsCurve, AnimationCurve rhsCurve, float t)
		{
			this.m_Value = lhsCurve;
			KeyframeUtility.InterpAnimationCurve(ref this.m_Value, rhsCurve, t);
		}

		public override void SetValue(VolumeParameter parameter)
		{
			this.m_Value.CopyFrom(((AnimationCurveParameter)parameter).m_Value);
		}

		public override object Clone()
		{
			return new AnimationCurveParameter(new AnimationCurve(base.GetValue<AnimationCurve>().keys), this.overrideState);
		}

		public override int GetHashCode()
		{
			return this.overrideState.GetHashCode() * 23 + this.value.GetHashCode();
		}
	}
}
