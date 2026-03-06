using System;

namespace Liv.Lck
{
	internal class LckAudioHardLimiter : ILckAudioLimiter
	{
		public LckAudioHardLimiter(float threshold = 0.6f, float ratio = 2f, float makeUpGain = 1f, float attackTime = 0.01f, float releaseTime = 0.1f)
		{
			this._threshold = threshold;
			this._ratio = ratio;
			this._makeUpGain = makeUpGain;
			this._attackTime = attackTime;
			this._releaseTime = releaseTime;
			this._envelope = 0f;
		}

		public float ApplyLimiter(float audioIn, int sampleRate)
		{
			float num = Math.Abs(audioIn);
			float attackCoeff = LckAudioLimiterUtils.CalculateAttackCoefficient(this._attackTime, sampleRate);
			float releaseCoeff = LckAudioLimiterUtils.CalculateReleaseCoefficient(this._releaseTime, sampleRate);
			float gainReduction = (num > this._threshold) ? (this._threshold / num) : 1f;
			this._envelope = LckAudioLimiterUtils.UpdateEnvelope(gainReduction, this._envelope, attackCoeff, releaseCoeff);
			return LckAudioLimiterUtils.ApplyGainReduction(audioIn, this._envelope, this._makeUpGain);
		}

		private readonly float _threshold;

		private readonly float _ratio;

		private readonly float _makeUpGain;

		private readonly float _attackTime;

		private readonly float _releaseTime;

		private float _envelope;
	}
}
