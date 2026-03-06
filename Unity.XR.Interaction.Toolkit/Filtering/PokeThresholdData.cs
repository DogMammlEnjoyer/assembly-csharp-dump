using System;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[Serializable]
	public class PokeThresholdData
	{
		public PokeAxis pokeDirection
		{
			get
			{
				return this.m_PokeDirection;
			}
			set
			{
				this.m_PokeDirection = value;
			}
		}

		public float interactionDepthOffset
		{
			get
			{
				return this.m_InteractionDepthOffset;
			}
			set
			{
				this.m_InteractionDepthOffset = value;
			}
		}

		public bool enablePokeAngleThreshold
		{
			get
			{
				return this.m_EnablePokeAngleThreshold;
			}
			set
			{
				this.m_EnablePokeAngleThreshold = value;
			}
		}

		public float pokeAngleThreshold
		{
			get
			{
				return this.m_PokeAngleThreshold;
			}
			set
			{
				this.m_PokeAngleThreshold = value;
			}
		}

		public float GetSelectEntranceVectorDotThreshold()
		{
			return Mathf.Cos(0.017453292f * this.m_PokeAngleThreshold);
		}

		[SerializeField]
		[Tooltip("The axis along which the poke interaction will be constrained.")]
		private PokeAxis m_PokeDirection = PokeAxis.Z;

		[SerializeField]
		[Tooltip("Distance along the poke interactable interaction axis that allows for a poke to be triggered sooner/with less precision.")]
		private float m_InteractionDepthOffset;

		[SerializeField]
		[Tooltip("When enabled, the filter will check that a poke action is started and moves within the poke angle threshold along the poke direction axis.")]
		private bool m_EnablePokeAngleThreshold = true;

		[SerializeField]
		[Tooltip("The maximum allowed angle (in degrees) from the poke direction axis that will trigger a select interaction.")]
		[Range(0f, 89.9f)]
		private float m_PokeAngleThreshold = 45f;
	}
}
