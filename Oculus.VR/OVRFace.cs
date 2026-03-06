using System;
using Meta.XR.Util;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[HelpURL("https://developer.oculus.com/documentation/unity/move-face-tracking/")]
[Feature(Feature.FaceTracking)]
public class OVRFace : MonoBehaviour
{
	public OVRFaceExpressions FaceExpressions
	{
		get
		{
			return this._faceExpressions;
		}
		set
		{
			this._faceExpressions = value;
		}
	}

	public float BlendShapeStrengthMultiplier
	{
		get
		{
			return this._blendShapeStrengthMultiplier;
		}
		set
		{
			this._blendShapeStrengthMultiplier = value;
		}
	}

	protected SkinnedMeshRenderer SkinnedMesh
	{
		get
		{
			return this._skinnedMeshRenderer;
		}
	}

	internal SkinnedMeshRenderer RetrieveSkinnedMeshRenderer()
	{
		return base.GetComponent<SkinnedMeshRenderer>();
	}

	internal OVRFaceExpressions SearchFaceExpressions()
	{
		return base.gameObject.GetComponentInParent<OVRFaceExpressions>();
	}

	protected virtual void Awake()
	{
		if (this._faceExpressions == null)
		{
			this._faceExpressions = this.SearchFaceExpressions();
			Debug.Log("Found OVRFaceExpression reference in " + this._faceExpressions.name + " due to unassigned field.");
		}
		if (this._meshWeightsProviderObject != null)
		{
			this._meshWeightsProvider = this._meshWeightsProviderObject.GetComponent<OVRFace.IMeshWeightsProvider>();
		}
	}

	private void OnEnable()
	{
		OVRManager ovrmanager = Object.FindAnyObjectByType<OVRManager>();
		if (ovrmanager != null && ovrmanager.SimultaneousHandsAndControllersEnabled)
		{
			Debug.LogWarning("Please note that currently, face tracking and simultaneous hands and controllers cannot be enabled at the same time on Quest 2", this);
			return;
		}
	}

	protected virtual void Start()
	{
		this._skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
		this._meshWeightsProviderObject != null;
	}

	protected virtual void Update()
	{
		if (!this._faceExpressions.FaceTrackingEnabled || !this._faceExpressions.enabled)
		{
			return;
		}
		if (this._meshWeightsProvider != null)
		{
			this._meshWeightsProvider.UpdateWeights(this._faceExpressions);
		}
		if (this._faceExpressions.ValidExpressions)
		{
			int blendShapeCount = this._skinnedMeshRenderer.sharedMesh.blendShapeCount;
			for (int i = 0; i < blendShapeCount; i++)
			{
				float value;
				if (this.GetWeightValue(i, out value))
				{
					this._skinnedMeshRenderer.SetBlendShapeWeight(i, Mathf.Clamp(value, 0f, 100f));
				}
			}
		}
	}

	protected internal virtual OVRFaceExpressions.FaceExpression GetFaceExpression(int blendShapeIndex)
	{
		return OVRFaceExpressions.FaceExpression.Invalid;
	}

	protected internal virtual bool GetWeightValue(int blendShapeIndex, out float weightValue)
	{
		if (this._meshWeightsProvider != null)
		{
			bool weightValue2 = this._meshWeightsProvider.GetWeightValue(blendShapeIndex, out weightValue);
			weightValue *= this._blendShapeStrengthMultiplier;
			return weightValue2;
		}
		OVRFaceExpressions.FaceExpression faceExpression = this.GetFaceExpression(blendShapeIndex);
		if (faceExpression >= OVRFaceExpressions.FaceExpression.Max || faceExpression < OVRFaceExpressions.FaceExpression.BrowLowererL)
		{
			weightValue = 0f;
			return false;
		}
		weightValue = this._faceExpressions[faceExpression] * this._blendShapeStrengthMultiplier;
		return true;
	}

	[SerializeField]
	[Tooltip("The OVRFaceExpressions Component to fetch the Face Tracking weights from that are to be applied")]
	protected internal OVRFaceExpressions _faceExpressions;

	[SerializeField]
	[Tooltip("A multiplier to the weights read from the OVRFaceExpressions to exaggerate facial expressions")]
	protected internal float _blendShapeStrengthMultiplier = 100f;

	[SerializeField]
	[Tooltip("Optional component that contains IMeshWeightsProvider.")]
	protected internal GameObject _meshWeightsProviderObject;

	private SkinnedMeshRenderer _skinnedMeshRenderer;

	private OVRFace.IMeshWeightsProvider _meshWeightsProvider;

	public interface IMeshWeightsProvider
	{
		void UpdateWeights(OVRFaceExpressions faceExpressions);

		bool GetWeightValue(int blendshapeIndex, out float weightValue);
	}
}
