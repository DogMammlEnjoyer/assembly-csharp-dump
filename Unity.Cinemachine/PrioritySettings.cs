using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct PrioritySettings
	{
		public int Value
		{
			readonly get
			{
				if (!this.Enabled)
				{
					return 0;
				}
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
				this.Enabled = true;
			}
		}

		public static implicit operator int(PrioritySettings prioritySettings)
		{
			return prioritySettings.Value;
		}

		public static implicit operator PrioritySettings(int priority)
		{
			return new PrioritySettings
			{
				Value = priority,
				Enabled = true
			};
		}

		[Tooltip("Enable this to expose the Priority field")]
		public bool Enabled;

		[Tooltip("Priority to use.  0 is default.  Camera with highest priority is prioritized.")]
		[SerializeField]
		private int m_Value;
	}
}
