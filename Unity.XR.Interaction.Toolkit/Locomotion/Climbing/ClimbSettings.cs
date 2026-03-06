using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[Serializable]
	public class ClimbSettings
	{
		public bool allowFreeXMovement
		{
			get
			{
				return this.m_AllowFreeXMovement;
			}
			set
			{
				this.m_AllowFreeXMovement = value;
			}
		}

		public bool allowFreeYMovement
		{
			get
			{
				return this.m_AllowFreeYMovement;
			}
			set
			{
				this.m_AllowFreeYMovement = value;
			}
		}

		public bool allowFreeZMovement
		{
			get
			{
				return this.m_AllowFreeZMovement;
			}
			set
			{
				this.m_AllowFreeZMovement = value;
			}
		}

		[SerializeField]
		[Tooltip("Controls whether to allow unconstrained movement along the climb interactable's x-axis.")]
		private bool m_AllowFreeXMovement = true;

		[SerializeField]
		[Tooltip("Controls whether to allow unconstrained movement along the climb interactable's y-axis.")]
		private bool m_AllowFreeYMovement = true;

		[SerializeField]
		[Tooltip("Controls whether to allow unconstrained movement along the climb interactable's z-axis.")]
		private bool m_AllowFreeZMovement = true;
	}
}
