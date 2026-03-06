using System;

namespace Liv.Lck
{
	internal class LckAudioSoftLimiter : ILckAudioLimiter
	{
		public LckAudioSoftLimiter(float threshold = 0.6f, float kneeWidth = 0.2f, float ratio = 2f, float makeUpGain = 1f, float attackTime = 0.01f, float releaseTime = 0.1f)
		{
			this._threshold = threshold;
			this._kneeWidth = kneeWidth;
			this._ratio = ratio;
			this._makeUpGain = makeUpGain;
			this._attackTime = attackTime;
			this._releaseTime = releaseTime;
			this._envelope = 0f;
		}

		public float ApplyLimiter(float audioIn, int sampleRate)
		{
			float absSample = Math.Abs(audioIn);
			float attackCoeff = LckAudioLimiterUtils.CalculateAttackCoefficient(this._attackTime, sampleRate);
			float releaseCoeff = LckAudioLimiterUtils.CalculateReleaseCoefficient(this._releaseTime, sampleRate);
			float kneeStart = this._threshold - this._kneeWidth / 2f;
			float kneeEnd = this._threshold + this._kneeWidth / 2f;
			float gainReduction = this.CalculateSoftKneeGainReduction(absSample, kneeStart, kneeEnd);
			this._envelope = LckAudioLimiterUtils.UpdateEnvelope(gainReduction, this._envelope, attackCoeff, releaseCoeff);
			return LckAudioLimiterUtils.ApplyGainReduction(audioIn, this._envelope, this._makeUpGain);
		}

		private float CalculateSoftKneeGainReduction(float absSample, float kneeStart, float kneeEnd)
		{
			if (absSample > kneeStart && absSample < kneeEnd)
			{
				float num = (absSample - kneeStart) / this._kneeWidth;
				float num2 = num * num * (3f - 2f * num);
				return (kneeStart + num2 * (absSample - kneeStart) / this._ratio) / absSample;
			}
			if (absSample >= kneeEnd)
			{
				float num3 = (absSample - this._threshold) / this._ratio;
				return (this._threshold + num3) / absSample;
			}
			return 1f;
		}

		private readonly float _threshold;

		private readonly float _kneeWidth;

		private readonly float _ratio;

		private readonly float _makeUpGain;

		private readonly float _attackTime;

		private readonly float _releaseTime;

		private float _envelope;
	}
}
