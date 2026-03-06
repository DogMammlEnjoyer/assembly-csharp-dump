using System;
using Meta.XR.Util;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[HelpURL("https://developer.oculus.com/documentation/unity/move-face-tracking/")]
[Feature(Feature.FaceTracking)]
public class OVRCustomFace : OVRFace
{
	public OVRFaceExpressions.FaceExpression[] Mappings
	{
		get
		{
			return this._mappings;
		}
		set
		{
			this._mappings = value;
		}
	}

	protected OVRCustomFace.RetargetingType RetargetingValue
	{
		get
		{
			return this.retargetingType;
		}
		set
		{
			this.retargetingType = value;
		}
	}

	protected bool AllowDuplicateMapping
	{
		get
		{
			return this._allowDuplicateMapping;
		}
		set
		{
			this._allowDuplicateMapping = value;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	protected internal override OVRFaceExpressions.FaceExpression GetFaceExpression(int blendShapeIndex)
	{
		return this._mappings[blendShapeIndex];
	}

	protected internal virtual ValueTuple<string[], OVRFaceExpressions.FaceExpression[]> GetCustomBlendShapeNameAndExpressionPairs()
	{
		string[] names = Enum.GetNames(typeof(OVRFaceExpressions.FaceExpression));
		OVRFaceExpressions.FaceExpression[] item = (OVRFaceExpressions.FaceExpression[])Enum.GetValues(typeof(OVRFaceExpressions.FaceExpression));
		return new ValueTuple<string[], OVRFaceExpressions.FaceExpression[]>(names, item);
	}

	[SerializeField]
	[Tooltip("The mapping between Face Expressions to the blend shapes available on the shared mesh of the skinned mesh renderer")]
	internal OVRFaceExpressions.FaceExpression[] _mappings;

	[SerializeField]
	[HideInInspector]
	internal OVRCustomFace.RetargetingType retargetingType;

	[SerializeField]
	[Tooltip("Allow duplicates when mapping blend shapes to Face Expressions")]
	internal bool _allowDuplicateMapping = true;

	public enum RetargetingType
	{
		OculusFace,
		Custom
	}
}
