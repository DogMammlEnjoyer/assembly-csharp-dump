using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandVisual : MonoBehaviour, IHandVisual
	{
		public IHand Hand { get; private set; }

		public event Action WhenHandVisualUpdated = delegate()
		{
		};

		public bool IsVisible
		{
			get
			{
				return this.SkinnedMeshRenderer != null && this.SkinnedMeshRenderer.enabled;
			}
		}

		public IList<Transform> Joints
		{
			get
			{
				return this._openXRJointTransforms;
			}
		}

		public Transform Root
		{
			get
			{
				return this._openXRRoot;
			}
			private set
			{
				this._openXRRoot = value;
			}
		}

		private SkinnedMeshRenderer SkinnedMeshRenderer
		{
			get
			{
				return this._openXRSkinnedMeshRenderer;
			}
			set
			{
				this._openXRSkinnedMeshRenderer = value;
			}
		}

		private MaterialPropertyBlockEditor HandMaterialPropertyBlockEditor
		{
			get
			{
				return this._openXRHandMaterialPropertyBlockEditor;
			}
			set
			{
				this._openXRHandMaterialPropertyBlockEditor = value;
			}
		}

		public bool ForceOffVisibility
		{
			get
			{
				return this._forceOffVisibility;
			}
			set
			{
				this._forceOffVisibility = value;
				if (this._started)
				{
					this.UpdateVisibility();
				}
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			if (this.Root == null && this.Joints.Count > 0 && this.Joints[0] != null)
			{
				this.Root = this.Joints[0].parent;
			}
			if (this._root != null)
			{
				this._root.gameObject.SetActive(false);
			}
			if (this._openXRRoot != null)
			{
				this._openXRRoot.gameObject.SetActive(true);
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this.HandMaterialPropertyBlockEditor != null)
			{
				this._wristScalePropertyId = Shader.PropertyToID("_WristScale");
			}
			this.UpdateVisibility();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.UpdateSkeleton;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started && this._hand != null)
			{
				this.Hand.WhenHandUpdated -= this.UpdateSkeleton;
			}
		}

		private void UpdateVisibility()
		{
			if (!this._updateVisibility)
			{
				return;
			}
			if (!this.Hand.IsTrackedDataValid)
			{
				if (this.IsVisible || this.ForceOffVisibility)
				{
					this.SkinnedMeshRenderer.enabled = false;
					return;
				}
			}
			else
			{
				if (!this.IsVisible && !this.ForceOffVisibility)
				{
					this.SkinnedMeshRenderer.enabled = true;
					return;
				}
				if (this.IsVisible && this.ForceOffVisibility)
				{
					this.SkinnedMeshRenderer.enabled = false;
				}
			}
		}

		public void UpdateSkeleton()
		{
			this.UpdateVisibility();
			if (!this.Hand.IsTrackedDataValid)
			{
				this.WhenHandVisualUpdated();
				return;
			}
			Pose pose;
			if (this._updateRootPose && this.Root != null && this.Hand.GetRootPose(out pose))
			{
				this.Root.position = pose.position;
				this.Root.rotation = pose.rotation;
			}
			if (this._updateRootScale && this.Root != null)
			{
				float num = (this.Root.parent != null) ? this.Root.parent.lossyScale.x : 1f;
				this.Root.localScale = this.Hand.Scale / num * Vector3.one;
			}
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (!this.Hand.GetJointPosesLocal(out readOnlyHandJointPoses))
			{
				return;
			}
			for (int i = 0; i < 26; i++)
			{
				if (!(this.Joints[i] == null))
				{
					Transform transform = this.Joints[i];
					Pose pose2 = readOnlyHandJointPoses[i];
					transform.SetPose(pose2, Space.Self);
				}
			}
			if (this.HandMaterialPropertyBlockEditor != null)
			{
				this.HandMaterialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._wristScalePropertyId, this.Hand.Scale);
				this.HandMaterialPropertyBlockEditor.UpdateMaterialPropertyBlock();
			}
			this.WhenHandVisualUpdated();
		}

		public Transform GetTransformByHandJointId(HandJointId handJointId)
		{
			return this.Joints[(int)handJointId];
		}

		public Pose GetJointPose(HandJointId jointId, Space space)
		{
			return this.GetTransformByHandJointId(jointId).GetPose(space);
		}

		public void InjectAllHandSkeletonVisual(IHand hand, SkinnedMeshRenderer skinnedMeshRenderer)
		{
			this.InjectHand(hand);
			this.InjectSkinnedMeshRenderer(skinnedMeshRenderer);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
		{
			this.SkinnedMeshRenderer = skinnedMeshRenderer;
		}

		public void InjectOptionalUpdateRootPose(bool updateRootPose)
		{
			this._updateRootPose = updateRootPose;
		}

		public void InjectOptionalUpdateRootScale(bool updateRootScale)
		{
			this._updateRootScale = updateRootScale;
		}

		public void InjectOptionalRoot(Transform root)
		{
			this.Root = root;
		}

		public void InjectOptionalMaterialPropertyBlockEditor(MaterialPropertyBlockEditor editor)
		{
			this.HandMaterialPropertyBlockEditor = editor;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private bool _updateRootPose = true;

		[SerializeField]
		private bool _updateRootScale = true;

		[SerializeField]
		private bool _updateVisibility = true;

		[HideInInspector]
		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		[HideInInspector]
		[SerializeField]
		[Optional]
		private Transform _root;

		[HideInInspector]
		[SerializeField]
		[Optional]
		private MaterialPropertyBlockEditor _handMaterialPropertyBlockEditor;

		[HideInInspector]
		[SerializeField]
		private List<Transform> _jointTransforms = new List<Transform>();

		[SerializeField]
		private SkinnedMeshRenderer _openXRSkinnedMeshRenderer;

		[SerializeField]
		[Optional]
		private Transform _openXRRoot;

		[SerializeField]
		[Optional]
		private MaterialPropertyBlockEditor _openXRHandMaterialPropertyBlockEditor;

		[HideInInspector]
		[SerializeField]
		private List<Transform> _openXRJointTransforms = new List<Transform>();

		private int _wristScalePropertyId;

		private bool _forceOffVisibility;

		private bool _started;
	}
}
