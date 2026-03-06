using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Animations.Rigging
{
	[RequireComponent(typeof(Animator))]
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[AddComponentMenu("Animation Rigging/Setup/Rig Builder")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/RiggingWorkflow.html#rig-builder-component")]
	public class RigBuilder : MonoBehaviour, IAnimationWindowPreview, IRigEffectorHolder
	{
		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				this.Build();
			}
			RigBuilder.OnAddRigBuilderCallback onAddRigBuilderCallback = RigBuilder.onAddRigBuilder;
			if (onAddRigBuilderCallback == null)
			{
				return;
			}
			onAddRigBuilderCallback(this);
		}

		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				this.Clear();
			}
			RigBuilder.OnRemoveRigBuilderCallback onRemoveRigBuilderCallback = RigBuilder.onRemoveRigBuilder;
			if (onRemoveRigBuilderCallback == null)
			{
				return;
			}
			onRemoveRigBuilderCallback(this);
		}

		private void OnDestroy()
		{
			this.Clear();
		}

		public void Evaluate(float deltaTime)
		{
			if (!this.graph.IsValid())
			{
				return;
			}
			this.SyncLayers();
			this.graph.Evaluate(deltaTime);
		}

		private void Update()
		{
			if (!this.graph.IsValid())
			{
				return;
			}
			this.SyncLayers();
		}

		public void SyncLayers()
		{
			if (this.m_RuntimeRigLayers == null)
			{
				return;
			}
			this.syncSceneToStreamLayer.Update(this.m_RuntimeRigLayers);
			int i = 0;
			int num = this.m_RuntimeRigLayers.Length;
			while (i < num)
			{
				if (this.m_RuntimeRigLayers[i].IsValid() && this.m_RuntimeRigLayers[i].active)
				{
					this.m_RuntimeRigLayers[i].Update();
				}
				i++;
			}
		}

		public bool Build()
		{
			if (this.m_IsInPreview)
			{
				return false;
			}
			this.Clear();
			Animator component = base.GetComponent<Animator>();
			if (component == null || this.layers.Count == 0)
			{
				return false;
			}
			IRigLayer[] runtimeRigLayers = this.layers.ToArray();
			this.m_RuntimeRigLayers = runtimeRigLayers;
			this.graph = RigBuilderUtils.BuildPlayableGraph(component, this.m_RuntimeRigLayers, this.syncSceneToStreamLayer);
			if (!this.graph.IsValid())
			{
				return false;
			}
			this.graph.Play();
			return true;
		}

		public bool Build(PlayableGraph graph)
		{
			if (this.m_IsInPreview)
			{
				return false;
			}
			this.Clear();
			Animator component = base.GetComponent<Animator>();
			if (component == null || this.layers.Count == 0)
			{
				return false;
			}
			IRigLayer[] runtimeRigLayers = this.layers.ToArray();
			this.m_RuntimeRigLayers = runtimeRigLayers;
			RigBuilderUtils.BuildPlayableGraph(graph, component, this.m_RuntimeRigLayers, this.syncSceneToStreamLayer);
			return true;
		}

		public void Clear()
		{
			if (this.m_IsInPreview)
			{
				return;
			}
			if (this.graph.IsValid())
			{
				this.graph.Destroy();
			}
			if (this.m_RuntimeRigLayers != null)
			{
				IRigLayer[] runtimeRigLayers = this.m_RuntimeRigLayers;
				for (int i = 0; i < runtimeRigLayers.Length; i++)
				{
					runtimeRigLayers[i].Reset();
				}
				this.m_RuntimeRigLayers = null;
			}
			this.syncSceneToStreamLayer.Reset();
		}

		public void StartPreview()
		{
			this.m_IsInPreview = true;
			if (!base.enabled)
			{
				return;
			}
			if (this.m_RuntimeRigLayers == null)
			{
				IRigLayer[] array = this.layers.ToArray();
				this.m_RuntimeRigLayers = array;
			}
			Animator component = base.GetComponent<Animator>();
			if (component != null)
			{
				IRigLayer[] array = this.m_RuntimeRigLayers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Initialize(component);
				}
			}
		}

		public void StopPreview()
		{
			this.m_IsInPreview = false;
			if (!base.enabled)
			{
				return;
			}
			if (Application.isPlaying)
			{
				return;
			}
			this.Clear();
		}

		public void UpdatePreviewGraph(PlayableGraph graph)
		{
			if (!base.enabled)
			{
				return;
			}
			if (!graph.IsValid() || this.m_RuntimeRigLayers == null)
			{
				return;
			}
			this.syncSceneToStreamLayer.Update(this.m_RuntimeRigLayers);
			foreach (IRigLayer rigLayer in this.m_RuntimeRigLayers)
			{
				if (rigLayer.IsValid() && rigLayer.active)
				{
					rigLayer.Update();
				}
			}
		}

		public Playable BuildPreviewGraph(PlayableGraph graph, Playable inputPlayable)
		{
			if (!base.enabled)
			{
				return inputPlayable;
			}
			if (this.m_RuntimeRigLayers == null)
			{
				this.StartPreview();
			}
			Animator component = base.GetComponent<Animator>();
			if (component == null || this.m_RuntimeRigLayers == null || this.m_RuntimeRigLayers.Length == 0)
			{
				return inputPlayable;
			}
			foreach (RigBuilderUtils.PlayableChain playableChain in RigBuilderUtils.BuildPlayables(component, graph, this.m_RuntimeRigLayers, this.syncSceneToStreamLayer))
			{
				if (playableChain.playables != null && playableChain.playables.Length != 0)
				{
					playableChain.playables[0].AddInput(inputPlayable, 0, 1f);
					inputPlayable = playableChain.playables[playableChain.playables.Length - 1];
				}
			}
			return inputPlayable;
		}

		public List<RigLayer> layers
		{
			get
			{
				if (this.m_RigLayers == null)
				{
					this.m_RigLayers = new List<RigLayer>();
				}
				return this.m_RigLayers;
			}
			set
			{
				this.m_RigLayers = value;
			}
		}

		private SyncSceneToStreamLayer syncSceneToStreamLayer
		{
			get
			{
				if (this.m_SyncSceneToStreamLayer == null)
				{
					this.m_SyncSceneToStreamLayer = new SyncSceneToStreamLayer();
				}
				return this.m_SyncSceneToStreamLayer;
			}
			set
			{
				this.m_SyncSceneToStreamLayer = value;
			}
		}

		public PlayableGraph graph { get; private set; }

		[SerializeField]
		private List<RigLayer> m_RigLayers;

		private IRigLayer[] m_RuntimeRigLayers;

		private SyncSceneToStreamLayer m_SyncSceneToStreamLayer;

		[SerializeField]
		private List<RigEffectorData> m_Effectors = new List<RigEffectorData>();

		private bool m_IsInPreview;

		public static RigBuilder.OnAddRigBuilderCallback onAddRigBuilder;

		public static RigBuilder.OnRemoveRigBuilderCallback onRemoveRigBuilder;

		public delegate void OnAddRigBuilderCallback(RigBuilder rigBuilder);

		public delegate void OnRemoveRigBuilderCallback(RigBuilder rigBuilder);
	}
}
