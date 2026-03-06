using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Audio
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	[Serializable]
	public class AudioAffordanceTheme
	{
		protected AudioAffordanceTheme()
		{
			this.m_List = new List<AudioAffordanceThemeData>
			{
				new AudioAffordanceThemeData
				{
					stateName = "disabled"
				},
				new AudioAffordanceThemeData
				{
					stateName = "idle"
				},
				new AudioAffordanceThemeData
				{
					stateName = "hovered"
				},
				new AudioAffordanceThemeData
				{
					stateName = "hoveredPriority"
				},
				new AudioAffordanceThemeData
				{
					stateName = "selected"
				},
				new AudioAffordanceThemeData
				{
					stateName = "activated"
				},
				new AudioAffordanceThemeData
				{
					stateName = "focused"
				}
			};
		}

		internal void ValidateTheme()
		{
			if (this.m_List == null)
			{
				return;
			}
			int count = this.m_List.Count;
			int num = (int)AffordanceStateShortcuts.stateCount - count;
			if (num > 0)
			{
				AudioAffordanceThemeData audioAffordanceThemeData;
				if (count < 2)
				{
					audioAffordanceThemeData = new AudioAffordanceThemeData
					{
						stateName = "idle"
					};
				}
				else
				{
					audioAffordanceThemeData = this.m_List[1];
				}
				while (num-- > 0)
				{
					AudioAffordanceThemeData item = new AudioAffordanceThemeData
					{
						stateName = audioAffordanceThemeData.stateName,
						stateEntered = audioAffordanceThemeData.stateEntered,
						stateExited = audioAffordanceThemeData.stateExited
					};
					this.m_List.Add(item);
					byte b = (byte)(this.m_List.Count - 1);
					string nameForIndex = AffordanceStateShortcuts.GetNameForIndex(b);
					this.m_List[(int)b].stateName = nameForIndex;
					Debug.LogWarning(string.Format("Found missing state {0} \"{1}\" in your affordance theme. Adding missing state with idle state data.", b, nameForIndex));
				}
			}
		}

		public AudioAffordanceThemeData GetAffordanceThemeDataForIndex(byte stateIndex)
		{
			if ((int)stateIndex >= this.m_List.Count)
			{
				return null;
			}
			return this.m_List[(int)stateIndex];
		}

		[SerializeField]
		private List<AudioAffordanceThemeData> m_List;
	}
}
