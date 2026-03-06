using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[Serializable]
	public class TeleportVolumeDestinationSettings
	{
		public bool enableDestinationEvaluationDelay
		{
			get
			{
				return this.m_EnableDestinationEvaluationDelay;
			}
			set
			{
				this.m_EnableDestinationEvaluationDelay = value;
			}
		}

		public float destinationEvaluationDelayTime
		{
			get
			{
				return this.m_DestinationEvaluationDelayTime;
			}
			set
			{
				this.m_DestinationEvaluationDelayTime = value;
			}
		}

		public bool pollForDestinationChange
		{
			get
			{
				return this.m_PollForDestinationChange;
			}
			set
			{
				this.m_PollForDestinationChange = value;
			}
		}

		public float destinationPollFrequency
		{
			get
			{
				return this.m_DestinationPollFrequency;
			}
			set
			{
				this.m_DestinationPollFrequency = value;
			}
		}

		public Object destinationFilterObject
		{
			get
			{
				return this.m_DestinationFilterObject;
			}
			set
			{
				this.m_DestinationFilterObject = value;
				this.m_DestinationEvaluationFilter = (value as ITeleportationVolumeAnchorFilter);
				this.m_AssignedFilter = true;
			}
		}

		public ITeleportationVolumeAnchorFilter destinationEvaluationFilter
		{
			get
			{
				if (!this.m_AssignedFilter)
				{
					this.m_DestinationEvaluationFilter = (this.m_DestinationFilterObject as ITeleportationVolumeAnchorFilter);
					this.m_AssignedFilter = true;
				}
				return this.m_DestinationEvaluationFilter;
			}
			set
			{
				this.m_DestinationEvaluationFilter = value;
				this.m_AssignedFilter = true;
			}
		}

		[SerializeField]
		[Tooltip("Whether to delay evaluation of the destination anchor until the user has hovered over the volume for a certain amount of time.")]
		private bool m_EnableDestinationEvaluationDelay;

		[SerializeField]
		[Tooltip("The amount of time, in seconds, for which the user must hover over the volume before it designates a destination anchor.")]
		private float m_DestinationEvaluationDelayTime = 1f;

		[SerializeField]
		[Tooltip("Whether to periodically query the filter for its calculated destination. If the determined anchor is not the current destination, the volume will initiate re-evaluation of the destination anchor.")]
		private bool m_PollForDestinationChange;

		[SerializeField]
		[Tooltip("The amount of time, in seconds, between queries to the filter for its calculated destination anchor.")]
		private float m_DestinationPollFrequency = 1f;

		[SerializeField]
		[RequireInterface(typeof(ITeleportationVolumeAnchorFilter))]
		[Tooltip("The anchor filter used to evaluate a teleportation destination. If set to None, the volume will use the anchor furthest from the user as the destination.")]
		private Object m_DestinationFilterObject;

		private ITeleportationVolumeAnchorFilter m_DestinationEvaluationFilter;

		[NonSerialized]
		private bool m_AssignedFilter;
	}
}
