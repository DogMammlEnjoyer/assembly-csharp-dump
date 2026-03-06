using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AI.Navigation
{
	[ExecuteAlways]
	[DefaultExecutionOrder(-103)]
	[AddComponentMenu("Navigation/NavMesh Modifier", 32)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshModifier.html")]
	public class NavMeshModifier : MonoBehaviour
	{
		public bool overrideArea
		{
			get
			{
				return this.m_OverrideArea;
			}
			set
			{
				this.m_OverrideArea = value;
			}
		}

		public int area
		{
			get
			{
				return this.m_Area;
			}
			set
			{
				this.m_Area = value;
			}
		}

		public bool overrideGenerateLinks
		{
			get
			{
				return this.m_OverrideGenerateLinks;
			}
			set
			{
				this.m_OverrideGenerateLinks = value;
			}
		}

		public bool generateLinks
		{
			get
			{
				return this.m_GenerateLinks;
			}
			set
			{
				this.m_GenerateLinks = value;
			}
		}

		public bool ignoreFromBuild
		{
			get
			{
				return this.m_IgnoreFromBuild;
			}
			set
			{
				this.m_IgnoreFromBuild = value;
			}
		}

		public bool applyToChildren
		{
			get
			{
				return this.m_ApplyToChildren;
			}
			set
			{
				this.m_ApplyToChildren = value;
			}
		}

		public static List<NavMeshModifier> activeModifiers
		{
			get
			{
				if (NavMeshModifier.s_RebuildNavMeshModifiers)
				{
					NavMeshModifier.s_NavMeshModifiers = NavMeshModifier.s_NavMeshModifiersSet.ToList<NavMeshModifier>();
					NavMeshModifier.s_RebuildNavMeshModifiers = false;
				}
				return NavMeshModifier.s_NavMeshModifiers;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ClearNavMeshModifiers()
		{
			NavMeshModifier.s_NavMeshModifiers.Clear();
			NavMeshModifier.s_NavMeshModifiersSet.Clear();
		}

		private void OnEnable()
		{
			this.RegisterModifier();
		}

		private void OnDisable()
		{
			this.UnregisterModifier();
		}

		private void RegisterModifier()
		{
			if (NavMeshModifier.s_NavMeshModifiersSet.Add(this))
			{
				NavMeshModifier.s_RebuildNavMeshModifiers = true;
			}
		}

		private void UnregisterModifier()
		{
			if (NavMeshModifier.s_NavMeshModifiersSet.Remove(this))
			{
				NavMeshModifier.s_RebuildNavMeshModifiers = true;
			}
		}

		public bool AffectsAgentType(int agentTypeID)
		{
			return this.m_AffectedAgents.Count != 0 && (this.m_AffectedAgents[0] == -1 || this.m_AffectedAgents.IndexOf(agentTypeID) != -1);
		}

		[SerializeField]
		[HideInInspector]
		private byte m_SerializedVersion;

		[SerializeField]
		private bool m_OverrideArea;

		[SerializeField]
		private int m_Area;

		[SerializeField]
		private bool m_OverrideGenerateLinks;

		[SerializeField]
		private bool m_GenerateLinks;

		[SerializeField]
		private bool m_IgnoreFromBuild;

		[SerializeField]
		private bool m_ApplyToChildren = true;

		[SerializeField]
		private List<int> m_AffectedAgents = new List<int>(new int[]
		{
			-1
		});

		private static bool s_RebuildNavMeshModifiers = true;

		private static List<NavMeshModifier> s_NavMeshModifiers = new List<NavMeshModifier>();

		private static readonly HashSet<NavMeshModifier> s_NavMeshModifiersSet = new HashSet<NavMeshModifier>();
	}
}
