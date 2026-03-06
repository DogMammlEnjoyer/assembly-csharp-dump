using System;
using UnityEngine;

public class OVRMeshRenderer : MonoBehaviour
{
	public bool IsInitialized { get; private set; }

	public bool IsDataValid { get; private set; }

	public bool IsDataHighConfidence { get; private set; }

	public bool ShouldUseSystemGestureMaterial { get; private set; }

	private void Awake()
	{
		if (this._dataProvider == null)
		{
			this._dataProvider = base.GetComponent<OVRMeshRenderer.IOVRMeshRendererDataProvider>();
		}
		if (this._ovrMesh == null)
		{
			this._ovrMesh = base.GetComponent<OVRMesh>();
		}
		if (this._ovrSkeleton == null)
		{
			this._ovrSkeleton = base.GetComponent<OVRSkeleton>();
		}
	}

	private void Start()
	{
		if (this._ovrMesh == null)
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
		return !this.IsInitialized && (!(this._ovrMesh == null) && (!(this._ovrMesh != null) || this._ovrMesh.IsInitialized)) && (!(this._ovrSkeleton != null) || this._ovrSkeleton.IsInitialized);
	}

	public void ForceRebind()
	{
		this.IsInitialized = false;
		this.Initialize();
	}

	private void Initialize()
	{
		this._skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
		if (!this._skinnedMeshRenderer)
		{
			this._skinnedMeshRenderer = base.gameObject.AddComponent<SkinnedMeshRenderer>();
		}
		this._skinnedMeshRenderer.sharedMesh = this._ovrMesh.Mesh;
		this._originalMaterial = this._skinnedMeshRenderer.sharedMaterial;
		if (this._ovrSkeleton != null)
		{
			OVRSkeleton.SkeletonType skeletonType = this._ovrSkeleton.GetSkeletonType();
			int currentNumSkinnableBones = this._ovrSkeleton.GetCurrentNumSkinnableBones();
			Matrix4x4[] array = new Matrix4x4[currentNumSkinnableBones];
			Transform[] array2 = new Transform[currentNumSkinnableBones];
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			int num = 0;
			while (num < currentNumSkinnableBones && num < this._ovrSkeleton.Bones.Count)
			{
				array2[num] = this._ovrSkeleton.Bones[num].Transform;
				array[num] = this._ovrSkeleton.BindPoses[num].Transform.worldToLocalMatrix * localToWorldMatrix;
				if (skeletonType.IsOpenXRHandSkeleton())
				{
					array[num] *= OVRMeshRenderer._openXRFixup;
				}
				num++;
			}
			this._ovrMesh.Mesh.bindposes = array;
			this._skinnedMeshRenderer.bones = array2;
			this._skinnedMeshRenderer.updateWhenOffscreen = true;
		}
		this.IsInitialized = true;
	}

	private void Update()
	{
		this.IsDataValid = false;
		this.IsDataHighConfidence = false;
		this.ShouldUseSystemGestureMaterial = false;
		if (this.IsInitialized)
		{
			bool flag = false;
			if (this._dataProvider != null)
			{
				OVRMeshRenderer.MeshRendererData meshRendererData = this._dataProvider.GetMeshRendererData();
				this.IsDataValid = meshRendererData.IsDataValid;
				this.IsDataHighConfidence = meshRendererData.IsDataHighConfidence;
				this.ShouldUseSystemGestureMaterial = meshRendererData.ShouldUseSystemGestureMaterial;
				flag = (meshRendererData.IsDataValid && meshRendererData.IsDataHighConfidence);
			}
			if (this._confidenceBehavior == OVRMeshRenderer.ConfidenceBehavior.ToggleRenderer && this._skinnedMeshRenderer != null && this._skinnedMeshRenderer.enabled != flag)
			{
				this._skinnedMeshRenderer.enabled = flag;
			}
			if (this._systemGestureBehavior == OVRMeshRenderer.SystemGestureBehavior.SwapMaterial && this._skinnedMeshRenderer != null)
			{
				if (this.ShouldUseSystemGestureMaterial && this._systemGestureMaterial != null && this._skinnedMeshRenderer.sharedMaterial != this._systemGestureMaterial)
				{
					this._skinnedMeshRenderer.sharedMaterial = this._systemGestureMaterial;
					return;
				}
				if (!this.ShouldUseSystemGestureMaterial && this._originalMaterial != null && this._skinnedMeshRenderer.sharedMaterial != this._originalMaterial)
				{
					this._skinnedMeshRenderer.sharedMaterial = this._originalMaterial;
				}
			}
		}
	}

	[SerializeField]
	private OVRMeshRenderer.IOVRMeshRendererDataProvider _dataProvider;

	[SerializeField]
	private OVRMesh _ovrMesh;

	[SerializeField]
	private OVRSkeleton _ovrSkeleton;

	[SerializeField]
	private OVRMeshRenderer.ConfidenceBehavior _confidenceBehavior = OVRMeshRenderer.ConfidenceBehavior.ToggleRenderer;

	[SerializeField]
	private OVRMeshRenderer.SystemGestureBehavior _systemGestureBehavior = OVRMeshRenderer.SystemGestureBehavior.SwapMaterial;

	[SerializeField]
	private Material _systemGestureMaterial;

	private Material _originalMaterial;

	private SkinnedMeshRenderer _skinnedMeshRenderer;

	private static readonly Matrix4x4 _openXRFixup = Matrix4x4.Rotate(new Quaternion(0f, 1f, 0f, 0f));

	public interface IOVRMeshRendererDataProvider
	{
		OVRMeshRenderer.MeshRendererData GetMeshRendererData();
	}

	public struct MeshRendererData
	{
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
}
