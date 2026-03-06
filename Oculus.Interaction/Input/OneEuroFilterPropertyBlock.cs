using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Serializable]
	public struct OneEuroFilterPropertyBlock
	{
		public float MinCutoff
		{
			get
			{
				return this._minCutoff;
			}
		}

		public float Beta
		{
			get
			{
				return this._beta;
			}
		}

		public float DCutoff
		{
			get
			{
				return this._dCutoff;
			}
		}

		private static float DefaultMinCutoff
		{
			get
			{
				return 1f;
			}
		}

		private static float DefaultBeta
		{
			get
			{
				return 0f;
			}
		}

		private static float DefaultDCutoff
		{
			get
			{
				return 1f;
			}
		}

		public OneEuroFilterPropertyBlock(float minCutoff, float beta, float dCutoff)
		{
			this._minCutoff = minCutoff;
			this._beta = beta;
			this._dCutoff = dCutoff;
		}

		public OneEuroFilterPropertyBlock(float minCutoff, float beta)
		{
			this._minCutoff = minCutoff;
			this._beta = beta;
			this._dCutoff = OneEuroFilterPropertyBlock.DefaultDCutoff;
		}

		public static OneEuroFilterPropertyBlock Default
		{
			get
			{
				return new OneEuroFilterPropertyBlock
				{
					_minCutoff = OneEuroFilterPropertyBlock.DefaultMinCutoff,
					_beta = OneEuroFilterPropertyBlock.DefaultBeta,
					_dCutoff = OneEuroFilterPropertyBlock.DefaultDCutoff
				};
			}
		}

		[SerializeField]
		[Tooltip("Decrease min cutoff until jitter is eliminated")]
		public float _minCutoff;

		[SerializeField]
		[Tooltip("Increase beta from zero to reduce lag")]
		public float _beta;

		[SerializeField]
		[Tooltip("Smaller values of dCutoff smooth more but slow accuracy")]
		public float _dCutoff;
	}
}
