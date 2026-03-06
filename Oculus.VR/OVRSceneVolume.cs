using System;
using Meta.XR.Util;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(OVRSceneAnchor))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#further-scene-model-unity-components")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRSceneVolume : MonoBehaviour, IOVRSceneComponent
{
	public float Width { get; private set; }

	public float Height { get; private set; }

	public float Depth { get; private set; }

	public Vector3 Dimensions
	{
		get
		{
			return new Vector3(this.Width, this.Height, this.Depth);
		}
	}

	public Vector3 Offset { get; private set; }

	public bool ScaleChildren
	{
		get
		{
			return this._scaleChildren;
		}
		set
		{
			this._scaleChildren = value;
			if (this._scaleChildren && this._sceneAnchor.Space.Valid)
			{
				this.SetChildScale();
			}
		}
	}

	public bool OffsetChildren
	{
		get
		{
			return this._offsetChildren;
		}
		set
		{
			this._offsetChildren = value;
			if (this._offsetChildren && this._sceneAnchor.Space.Valid)
			{
				this.SetChildOffset();
			}
		}
	}

	private void Awake()
	{
		this._sceneAnchor = base.GetComponent<OVRSceneAnchor>();
		if (this._sceneAnchor.Space.Valid)
		{
			((IOVRSceneComponent)this).Initialize();
		}
	}

	void IOVRSceneComponent.Initialize()
	{
		this.UpdateTransform();
	}

	private void SetChildScale()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			OVRSceneObjectTransformType ovrsceneObjectTransformType;
			if (!child.TryGetComponent<OVRSceneObjectTransformType>(out ovrsceneObjectTransformType) || ovrsceneObjectTransformType.TransformType == OVRSceneObjectTransformType.Transformation.Volume)
			{
				child.localScale = this.Dimensions;
			}
		}
	}

	private void SetChildOffset()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			OVRSceneObjectTransformType ovrsceneObjectTransformType;
			if (!child.TryGetComponent<OVRSceneObjectTransformType>(out ovrsceneObjectTransformType) || ovrsceneObjectTransformType.TransformType == OVRSceneObjectTransformType.Transformation.Volume)
			{
				child.localPosition = this.Offset;
			}
		}
	}

	internal void UpdateTransform()
	{
		OVRPlugin.Boundsf boundsf;
		if (OVRPlugin.GetSpaceBoundingBox3D(this._sceneAnchor.Space, out boundsf))
		{
			this.Width = boundsf.Size.w;
			this.Height = boundsf.Size.h;
			this.Depth = boundsf.Size.d;
			Vector3 position = base.transform.position;
			Vector3 vector = base.transform.TransformPoint(boundsf.Pos.FromVector3f() + boundsf.Size.FromSize3f());
			Vector3 vector2 = base.transform.TransformPoint(boundsf.Pos.FromVector3f() + boundsf.Size.FromSize3f() / 2f);
			vector2.y = vector.y;
			this.Offset = new Vector3(vector2.x - position.x, vector2.z - position.z, vector2.y - position.y);
			if (this.ScaleChildren)
			{
				this.SetChildScale();
			}
			if (this.OffsetChildren)
			{
				this.SetChildOffset();
			}
		}
	}

	[Tooltip("When enabled, scales the child transforms according to the dimensions of this volume. If both Volume and Plane components exist on the game object, the volume takes precedence.")]
	[SerializeField]
	internal bool _scaleChildren = true;

	[Tooltip("When enabled, offsets the child transforms according to the offset of this volume. If both Volume and Plane components exist on the game object, the volume takes precedence.")]
	[SerializeField]
	internal bool _offsetChildren;

	private OVRSceneAnchor _sceneAnchor;
}
