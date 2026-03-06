using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[Feature(Feature.BodyTracking)]
public class OVRSkeletonRenderer : MonoBehaviour
{
	public bool IsInitialized { get; private set; }

	public bool IsDataValid { get; private set; }

	public bool IsDataHighConfidence { get; private set; }

	public bool ShouldUseSystemGestureMaterial { get; private set; }

	private void Awake()
	{
		if (this._dataProvider == null)
		{
			this._dataProvider = base.GetComponent<OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider>();
		}
		if (this._ovrSkeleton == null)
		{
			this._ovrSkeleton = base.GetComponent<OVRSkeleton>();
		}
	}

	private void Start()
	{
		if (this._ovrSkeleton == null)
		{
			base.enabled = false;
			return;
		}
		if (this.ShouldInitialize())
		{
			this.Initialize();
		}
	}

	private bool ShouldInitialize()
	{
		return !this.IsInitialized && this._ovrSkeleton.IsInitialized;
	}

	private void Initialize()
	{
		this._boneVisualizations = new List<OVRSkeletonRenderer.BoneVisualization>();
		this._capsuleVisualizations = new List<OVRSkeletonRenderer.CapsuleVisualization>();
		this._ovrSkeleton = base.GetComponent<OVRSkeleton>();
		this._skeletonGO = new GameObject("SkeletonRenderer");
		this._skeletonGO.transform.SetParent(base.transform, false);
		if (this._skeletonMaterial == null)
		{
			this._skeletonDefaultMaterial = new Material(Shader.Find("Diffuse"));
			this._skeletonMaterial = this._skeletonDefaultMaterial;
		}
		if (this._capsuleMaterial == null)
		{
			this._capsuleDefaultMaterial = new Material(Shader.Find("Diffuse"));
			this._capsuleMaterial = this._capsuleDefaultMaterial;
		}
		if (this._systemGestureMaterial == null)
		{
			this._systemGestureDefaultMaterial = new Material(Shader.Find("Diffuse"));
			this._systemGestureDefaultMaterial.color = Color.blue;
			this._systemGestureMaterial = this._systemGestureDefaultMaterial;
		}
		if (this._ovrSkeleton.IsInitialized)
		{
			for (int i = 0; i < this._ovrSkeleton.Bones.Count; i++)
			{
				OVRSkeletonRenderer.BoneVisualization item = new OVRSkeletonRenderer.BoneVisualization(this._skeletonGO, this._skeletonMaterial, this._systemGestureMaterial, this._scale, this._ovrSkeleton.Bones[i].Transform, this._ovrSkeleton.Bones[i].Transform.parent);
				this._boneVisualizations.Add(item);
			}
			if (this._renderPhysicsCapsules && this._ovrSkeleton.Capsules != null)
			{
				for (int j = 0; j < this._ovrSkeleton.Capsules.Count; j++)
				{
					OVRSkeletonRenderer.CapsuleVisualization item2 = new OVRSkeletonRenderer.CapsuleVisualization(this._skeletonGO, this._capsuleMaterial, this._systemGestureMaterial, this._scale, this._ovrSkeleton.Capsules[j]);
					this._capsuleVisualizations.Add(item2);
				}
			}
			this.IsInitialized = true;
		}
	}

	public void Update()
	{
		this.IsDataValid = false;
		this.IsDataHighConfidence = false;
		this.ShouldUseSystemGestureMaterial = false;
		if (this.IsInitialized)
		{
			bool shouldRender = false;
			if (this._dataProvider != null)
			{
				OVRSkeletonRenderer.SkeletonRendererData skeletonRendererData = this._dataProvider.GetSkeletonRendererData();
				this.IsDataValid = skeletonRendererData.IsDataValid;
				this.IsDataHighConfidence = skeletonRendererData.IsDataHighConfidence;
				this.ShouldUseSystemGestureMaterial = skeletonRendererData.ShouldUseSystemGestureMaterial;
				shouldRender = (skeletonRendererData.IsDataValid && skeletonRendererData.IsDataHighConfidence);
				if (skeletonRendererData.IsDataValid)
				{
					this._scale = skeletonRendererData.RootScale;
				}
			}
			for (int i = 0; i < this._boneVisualizations.Count; i++)
			{
				this._boneVisualizations[i].Update(this._scale, shouldRender, this.ShouldUseSystemGestureMaterial, this._confidenceBehavior, this._systemGestureBehavior);
			}
			for (int j = 0; j < this._capsuleVisualizations.Count; j++)
			{
				this._capsuleVisualizations[j].Update(this._scale, shouldRender, this.ShouldUseSystemGestureMaterial, this._confidenceBehavior, this._systemGestureBehavior);
			}
		}
	}

	private void OnDestroy()
	{
		if (this._skeletonDefaultMaterial != null)
		{
			Object.DestroyImmediate(this._skeletonDefaultMaterial, false);
		}
		if (this._capsuleDefaultMaterial != null)
		{
			Object.DestroyImmediate(this._capsuleDefaultMaterial, false);
		}
		if (this._systemGestureDefaultMaterial != null)
		{
			Object.DestroyImmediate(this._systemGestureDefaultMaterial, false);
		}
	}

	[SerializeField]
	private OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider _dataProvider;

	[SerializeField]
	private OVRSkeletonRenderer.ConfidenceBehavior _confidenceBehavior = OVRSkeletonRenderer.ConfidenceBehavior.ToggleRenderer;

	[SerializeField]
	private OVRSkeletonRenderer.SystemGestureBehavior _systemGestureBehavior = OVRSkeletonRenderer.SystemGestureBehavior.SwapMaterial;

	[SerializeField]
	private bool _renderPhysicsCapsules;

	[SerializeField]
	private Material _skeletonMaterial;

	private Material _skeletonDefaultMaterial;

	[SerializeField]
	private Material _capsuleMaterial;

	private Material _capsuleDefaultMaterial;

	[SerializeField]
	private Material _systemGestureMaterial;

	private Material _systemGestureDefaultMaterial;

	private const float LINE_RENDERER_WIDTH = 0.005f;

	private List<OVRSkeletonRenderer.BoneVisualization> _boneVisualizations;

	private List<OVRSkeletonRenderer.CapsuleVisualization> _capsuleVisualizations;

	private OVRSkeleton _ovrSkeleton;

	private GameObject _skeletonGO;

	private float _scale;

	private static readonly Quaternion _capsuleRotationOffset = Quaternion.Euler(0f, 0f, 90f);

	public interface IOVRSkeletonRendererDataProvider
	{
		OVRSkeletonRenderer.SkeletonRendererData GetSkeletonRendererData();
	}

	public struct SkeletonRendererData
	{
		public float RootScale { readonly get; set; }

		public bool IsDataValid { readonly get; set; }

		public bool IsDataHighConfidence { readonly get; set; }

		public bool ShouldUseSystemGestureMaterial { readonly get; set; }
	}

	public enum ConfidenceBehavior
	{
		None,
		ToggleRenderer
	}

	public enum SystemGestureBehavior
	{
		None,
		SwapMaterial
	}

	private class BoneVisualization
	{
		public BoneVisualization(GameObject rootGO, Material renderMat, Material systemGestureMat, float scale, Transform begin, Transform end)
		{
			this.RenderMaterial = renderMat;
			this.SystemGestureMaterial = systemGestureMat;
			this.BoneBegin = begin;
			this.BoneEnd = end;
			this.BoneGO = new GameObject(begin.name);
			this.BoneGO.transform.SetParent(rootGO.transform, false);
			this.Line = this.BoneGO.AddComponent<LineRenderer>();
			this.Line.sharedMaterial = this.RenderMaterial;
			this.Line.useWorldSpace = true;
			this.Line.positionCount = 2;
			this.Line.SetPosition(0, this.BoneBegin.position);
			this.Line.SetPosition(1, this.BoneEnd.position);
			this.Line.startWidth = 0.005f * scale;
			this.Line.endWidth = 0.005f * scale;
		}

		public void Update(float scale, bool shouldRender, bool shouldUseSystemGestureMaterial, OVRSkeletonRenderer.ConfidenceBehavior confidenceBehavior, OVRSkeletonRenderer.SystemGestureBehavior systemGestureBehavior)
		{
			this.Line.SetPosition(0, this.BoneBegin.position);
			this.Line.SetPosition(1, this.BoneEnd.position);
			this.Line.startWidth = 0.005f * scale;
			this.Line.endWidth = 0.005f * scale;
			if (confidenceBehavior == OVRSkeletonRenderer.ConfidenceBehavior.ToggleRenderer)
			{
				this.Line.enabled = shouldRender;
			}
			if (systemGestureBehavior == OVRSkeletonRenderer.SystemGestureBehavior.SwapMaterial)
			{
				if (shouldUseSystemGestureMaterial && this.Line.sharedMaterial != this.SystemGestureMaterial)
				{
					this.Line.sharedMaterial = this.SystemGestureMaterial;
					return;
				}
				if (!shouldUseSystemGestureMaterial && this.Line.sharedMaterial != this.RenderMaterial)
				{
					this.Line.sharedMaterial = this.RenderMaterial;
				}
			}
		}

		private GameObject BoneGO;

		private Transform BoneBegin;

		private Transform BoneEnd;

		private LineRenderer Line;

		private Material RenderMaterial;

		private Material SystemGestureMaterial;
	}

	private class CapsuleVisualization
	{
		public CapsuleVisualization(GameObject rootGO, Material renderMat, Material systemGestureMat, float scale, OVRBoneCapsule boneCapsule)
		{
			this.RenderMaterial = renderMat;
			this.SystemGestureMaterial = systemGestureMat;
			this.BoneCapsule = boneCapsule;
			this.CapsuleGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			Object.Destroy(this.CapsuleGO.GetComponent<CapsuleCollider>());
			this.Renderer = this.CapsuleGO.GetComponent<MeshRenderer>();
			this.Renderer.sharedMaterial = this.RenderMaterial;
			this.capsuleScale = Vector3.one;
			this.capsuleScale.y = boneCapsule.CapsuleCollider.height / 2f;
			this.capsuleScale.x = boneCapsule.CapsuleCollider.radius * 2f;
			this.capsuleScale.z = boneCapsule.CapsuleCollider.radius * 2f;
			this.CapsuleGO.transform.localScale = this.capsuleScale * scale;
		}

		public void Update(float scale, bool shouldRender, bool shouldUseSystemGestureMaterial, OVRSkeletonRenderer.ConfidenceBehavior confidenceBehavior, OVRSkeletonRenderer.SystemGestureBehavior systemGestureBehavior)
		{
			if (confidenceBehavior == OVRSkeletonRenderer.ConfidenceBehavior.ToggleRenderer && this.CapsuleGO.activeSelf != shouldRender)
			{
				this.CapsuleGO.SetActive(shouldRender);
			}
			this.CapsuleGO.transform.rotation = this.BoneCapsule.CapsuleCollider.transform.rotation * OVRSkeletonRenderer._capsuleRotationOffset;
			this.CapsuleGO.transform.position = this.BoneCapsule.CapsuleCollider.transform.TransformPoint(this.BoneCapsule.CapsuleCollider.center);
			this.CapsuleGO.transform.localScale = this.capsuleScale * scale;
			if (systemGestureBehavior == OVRSkeletonRenderer.SystemGestureBehavior.SwapMaterial)
			{
				if (shouldUseSystemGestureMaterial && this.Renderer.sharedMaterial != this.SystemGestureMaterial)
				{
					this.Renderer.sharedMaterial = this.SystemGestureMaterial;
					return;
				}
				if (!shouldUseSystemGestureMaterial && this.Renderer.sharedMaterial != this.RenderMaterial)
				{
					this.Renderer.sharedMaterial = this.RenderMaterial;
				}
			}
		}

		private GameObject CapsuleGO;

		private OVRBoneCapsule BoneCapsule;

		private Vector3 capsuleScale;

		private MeshRenderer Renderer;

		private Material RenderMaterial;

		private Material SystemGestureMaterial;
	}
}
