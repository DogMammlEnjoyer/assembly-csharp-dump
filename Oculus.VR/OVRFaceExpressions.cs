using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/move-face-tracking/")]
[Feature(Feature.FaceTracking)]
public class OVRFaceExpressions : MonoBehaviour, IReadOnlyCollection<float>, IEnumerable<float>, IEnumerable, OVRFaceExpressions.WeightProvider
{
	public bool FaceTrackingEnabled
	{
		get
		{
			return OVRPlugin.faceTracking2Enabled;
		}
	}

	public bool ValidExpressions { get; private set; }

	public bool EyeFollowingBlendshapesValid { get; private set; }

	public bool AreVisemesValid { get; private set; }

	private void Awake()
	{
		this._onPermissionGranted = new Action<string>(this.OnPermissionGranted);
	}

	private void OnEnable()
	{
		OVRFaceExpressions._trackingInstanceCount++;
		if (!this.StartFaceTracking())
		{
			base.enabled = false;
		}
	}

	private void OnPermissionGranted(string permissionId)
	{
		if (permissionId == OVRPermissionsRequester.GetPermissionId(OVRPermissionsRequester.Permission.FaceTracking) || permissionId == OVRPermissionsRequester.GetPermissionId(OVRPermissionsRequester.Permission.RecordAudio))
		{
			OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
			base.enabled = true;
		}
	}

	private OVRPlugin.FaceTrackingDataSource[] GetRequestedFaceTrackingDataSources()
	{
		OVRRuntimeSettings runtimeSettings = OVRRuntimeSettings.GetRuntimeSettings();
		if (runtimeSettings.RequestsAudioFaceTracking && runtimeSettings.RequestsVisualFaceTracking)
		{
			return new OVRPlugin.FaceTrackingDataSource[]
			{
				OVRPlugin.FaceTrackingDataSource.Visual,
				OVRPlugin.FaceTrackingDataSource.Audio
			};
		}
		if (runtimeSettings.RequestsVisualFaceTracking)
		{
			return new OVRPlugin.FaceTrackingDataSource[1];
		}
		if (runtimeSettings.RequestsAudioFaceTracking)
		{
			return new OVRPlugin.FaceTrackingDataSource[]
			{
				OVRPlugin.FaceTrackingDataSource.Audio
			};
		}
		return new OVRPlugin.FaceTrackingDataSource[0];
	}

	private bool StartFaceTracking()
	{
		if (!OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.FaceTracking) && !OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.RecordAudio))
		{
			OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
			OVRPermissionsRequester.PermissionGranted += this._onPermissionGranted;
			return false;
		}
		if (!OVRPlugin.StartFaceTracking2(this.GetRequestedFaceTrackingDataSources()))
		{
			Debug.LogWarning("[OVRFaceExpressions] Failed to start face tracking.");
			return false;
		}
		OVRPlugin.SetFaceTrackingVisemesEnabled(OVRRuntimeSettings.GetRuntimeSettings().EnableFaceTrackingVisemesOutput);
		return true;
	}

	private void OnDisable()
	{
		if (--OVRFaceExpressions._trackingInstanceCount == 0)
		{
			OVRPlugin.StopFaceTracking2();
		}
	}

	private void OnDestroy()
	{
		OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
	}

	private void Update()
	{
		this.ValidExpressions = (OVRPlugin.GetFaceState2(OVRPlugin.Step.Render, -1, ref this._currentFaceState) && this._currentFaceState.Status.IsValid);
		this.EyeFollowingBlendshapesValid = (this.ValidExpressions && this._currentFaceState.Status.IsEyeFollowingBlendshapesValid);
		this.AreVisemesValid = (OVRPlugin.GetFaceVisemesState(OVRPlugin.Step.Render, ref this._currentFaceVisemesState) == OVRPlugin.Result.Success && this._currentFaceVisemesState.IsValid);
	}

	public float this[OVRFaceExpressions.FaceExpression expression]
	{
		get
		{
			this.CheckValidity();
			if (expression < OVRFaceExpressions.FaceExpression.BrowLowererL || expression >= OVRFaceExpressions.FaceExpression.Max)
			{
				throw new ArgumentOutOfRangeException("expression", expression, string.Format("Value must be between 0 to {0}", 70));
			}
			return this._currentFaceState.ExpressionWeights[(int)expression];
		}
	}

	public float GetWeight(OVRFaceExpressions.FaceExpression expression)
	{
		return this[expression];
	}

	public bool TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression expression, out float weight)
	{
		if (!this.ValidExpressions || expression < OVRFaceExpressions.FaceExpression.BrowLowererL || expression >= OVRFaceExpressions.FaceExpression.Max)
		{
			weight = 0f;
			return false;
		}
		weight = this._currentFaceState.ExpressionWeights[(int)expression];
		return true;
	}

	public float GetViseme(OVRFaceExpressions.FaceViseme viseme)
	{
		this.CheckVisemesValidity();
		if (viseme < OVRFaceExpressions.FaceViseme.SIL || viseme >= OVRFaceExpressions.FaceViseme.Count)
		{
			throw new ArgumentOutOfRangeException("viseme", viseme, string.Format("Value must be between 0 to {0}", 15));
		}
		return this._currentFaceVisemesState.Visemes[(int)viseme];
	}

	public bool TryGetFaceViseme(OVRFaceExpressions.FaceViseme viseme, out float weight)
	{
		if (!this.AreVisemesValid || viseme < OVRFaceExpressions.FaceViseme.SIL || viseme >= OVRFaceExpressions.FaceViseme.Count)
		{
			weight = 0f;
			return false;
		}
		weight = this._currentFaceVisemesState.Visemes[(int)viseme];
		return true;
	}

	public void CopyVisemesTo(float[] array, int startIndex = 0)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (startIndex < 0 || startIndex >= array.Length)
		{
			throw new ArgumentOutOfRangeException("startIndex", startIndex, string.Format("Value must be between 0 to {0}", array.Length - 1));
		}
		if (array.Length - startIndex < 15)
		{
			throw new ArgumentException(string.Format("Capacity is too small - required {0}, available {1}.", 15, array.Length - startIndex), "array");
		}
		this.CheckVisemesValidity();
		for (int i = 0; i < 15; i++)
		{
			array[i + startIndex] = this._currentFaceVisemesState.Visemes[i];
		}
	}

	public bool TryGetWeightConfidence(OVRFaceExpressions.FaceRegionConfidence region, out float weightConfidence)
	{
		if (!this.ValidExpressions || region < OVRFaceExpressions.FaceRegionConfidence.Lower || region >= OVRFaceExpressions.FaceRegionConfidence.Max)
		{
			weightConfidence = 0f;
			return false;
		}
		weightConfidence = this._currentFaceState.ExpressionWeightConfidences[(int)region];
		return true;
	}

	public bool TryGetFaceTrackingDataSource(out OVRFaceExpressions.FaceTrackingDataSource dataSource)
	{
		dataSource = (OVRFaceExpressions.FaceTrackingDataSource)this._currentFaceState.DataSource;
		return this.ValidExpressions;
	}

	internal void CheckValidity()
	{
		if (!this.ValidExpressions)
		{
			throw new InvalidOperationException("Face expressions are not valid at this time. Use ValidExpressions to check for validity.");
		}
	}

	internal void CheckVisemesValidity()
	{
		if (!this.AreVisemesValid)
		{
			throw new InvalidOperationException("Face visemes are not valid at this time. Use AreVisemesValid to check for validity.");
		}
	}

	public void CopyTo(float[] array, int startIndex = 0)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (startIndex < 0 || startIndex >= array.Length)
		{
			throw new ArgumentOutOfRangeException("startIndex", startIndex, string.Format("Value must be between 0 to {0}", array.Length - 1));
		}
		if (array.Length - startIndex < 70)
		{
			throw new ArgumentException(string.Format("Capacity is too small - required {0}, available {1}.", 70, array.Length - startIndex), "array");
		}
		this.CheckValidity();
		for (int i = 0; i < 70; i++)
		{
			array[i + startIndex] = this._currentFaceState.ExpressionWeights[i];
		}
	}

	public float[] ToArray()
	{
		float[] array = new float[70];
		this.CopyTo(array, 0);
		return array;
	}

	public OVRFaceExpressions.FaceExpressionsEnumerator GetEnumerator()
	{
		return new OVRFaceExpressions.FaceExpressionsEnumerator(this._currentFaceState.ExpressionWeights);
	}

	IEnumerator<float> IEnumerable<float>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public int Count
	{
		get
		{
			float[] expressionWeights = this._currentFaceState.ExpressionWeights;
			if (expressionWeights == null)
			{
				return 0;
			}
			return expressionWeights.Length;
		}
	}

	private OVRPlugin.FaceState _currentFaceState;

	private OVRPlugin.FaceVisemesState _currentFaceVisemesState;

	private const OVRPermissionsRequester.Permission FaceTrackingPermission = OVRPermissionsRequester.Permission.FaceTracking;

	private const OVRPermissionsRequester.Permission RecordAudioPermission = OVRPermissionsRequester.Permission.RecordAudio;

	private Action<string> _onPermissionGranted;

	private static int _trackingInstanceCount;

	public interface WeightProvider
	{
		float GetWeight(OVRFaceExpressions.FaceExpression expression);
	}

	public enum FaceRegionConfidence
	{
		Lower,
		Upper,
		Max
	}

	public enum FaceTrackingDataSource
	{
		Visual,
		Audio,
		[InspectorName(null)]
		Count
	}

	public enum FaceExpression
	{
		[InspectorName("None")]
		Invalid = -1,
		BrowLowererL,
		BrowLowererR,
		CheekPuffL,
		CheekPuffR,
		CheekRaiserL,
		CheekRaiserR,
		CheekSuckL,
		CheekSuckR,
		ChinRaiserB,
		ChinRaiserT,
		DimplerL,
		DimplerR,
		EyesClosedL,
		EyesClosedR,
		EyesLookDownL,
		EyesLookDownR,
		EyesLookLeftL,
		EyesLookLeftR,
		EyesLookRightL,
		EyesLookRightR,
		EyesLookUpL,
		EyesLookUpR,
		InnerBrowRaiserL,
		InnerBrowRaiserR,
		JawDrop,
		JawSidewaysLeft,
		JawSidewaysRight,
		JawThrust,
		LidTightenerL,
		LidTightenerR,
		LipCornerDepressorL,
		LipCornerDepressorR,
		LipCornerPullerL,
		LipCornerPullerR,
		LipFunnelerLB,
		LipFunnelerLT,
		LipFunnelerRB,
		LipFunnelerRT,
		LipPressorL,
		LipPressorR,
		LipPuckerL,
		LipPuckerR,
		LipStretcherL,
		LipStretcherR,
		LipSuckLB,
		LipSuckLT,
		LipSuckRB,
		LipSuckRT,
		LipTightenerL,
		LipTightenerR,
		LipsToward,
		LowerLipDepressorL,
		LowerLipDepressorR,
		MouthLeft,
		MouthRight,
		NoseWrinklerL,
		NoseWrinklerR,
		OuterBrowRaiserL,
		OuterBrowRaiserR,
		UpperLidRaiserL,
		UpperLidRaiserR,
		UpperLipRaiserL,
		UpperLipRaiserR,
		TongueTipInterdental,
		TongueTipAlveolar,
		TongueFrontDorsalPalate,
		TongueMidDorsalPalate,
		TongueBackDorsalVelar,
		TongueOut,
		TongueRetreat,
		[InspectorName(null)]
		Max
	}

	public struct FaceExpressionsEnumerator : IEnumerator<float>, IEnumerator, IDisposable
	{
		internal FaceExpressionsEnumerator(float[] array)
		{
			this._faceExpressions = array;
			this._index = -1;
			float[] faceExpressions = this._faceExpressions;
			this._count = ((faceExpressions != null) ? faceExpressions.Length : 0);
		}

		public bool MoveNext()
		{
			int num = this._index + 1;
			this._index = num;
			return num < this._count;
		}

		public float Current
		{
			get
			{
				return this._faceExpressions[this._index];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public void Reset()
		{
			this._index = -1;
		}

		public void Dispose()
		{
		}

		private float[] _faceExpressions;

		private int _index;

		private int _count;
	}

	public enum FaceViseme
	{
		[InspectorName("None")]
		Invalid = -1,
		SIL,
		PP,
		FF,
		TH,
		DD,
		KK,
		CH,
		SS,
		NN,
		RR,
		AA,
		E,
		IH,
		OH,
		OU,
		[InspectorName(null)]
		Count
	}
}
