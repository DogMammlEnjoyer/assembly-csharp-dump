using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[DisallowMultipleComponent]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#ovrsceneanchor")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public sealed class OVRSceneAnchor : MonoBehaviour
{
	public OVRSpace Space { get; private set; }

	public Guid Uuid { get; private set; }

	public OVRAnchor Anchor { get; private set; }

	public bool IsTracked { get; internal set; }

	private bool IsComponentSupported(OVRPlugin.SpaceComponentType spaceComponentType)
	{
		return (this._supportedComponents.Count != 0 || this.Anchor.GetSupportedComponents(this._supportedComponents)) && this._supportedComponents.Contains(spaceComponentType);
	}

	internal bool IsComponentEnabled(OVRPlugin.SpaceComponentType spaceComponentType)
	{
		bool flag;
		bool flag2;
		return this.IsComponentSupported(spaceComponentType) && OVRPlugin.GetSpaceComponentStatus(this.Space, spaceComponentType, out flag, out flag2) && flag;
	}

	private void SyncComponent<T>(OVRPlugin.SpaceComponentType spaceComponentType) where T : MonoBehaviour, IOVRSceneComponent
	{
		if (!this.IsComponentEnabled(spaceComponentType))
		{
			return;
		}
		T component = base.GetComponent<T>();
		if (component)
		{
			component.Initialize();
			return;
		}
		base.gameObject.AddComponent<T>();
	}

	internal void ClearPoseCache()
	{
		this._pose = null;
	}

	public void Initialize(OVRAnchor anchor)
	{
		OVRSpace space = anchor.Handle;
		Guid uuid = anchor.Uuid;
		if (this.Space.Valid)
		{
			throw new InvalidOperationException(string.Format("[{0}] {1} has already been initialized.", uuid, "OVRSceneAnchor"));
		}
		if (!space.Valid)
		{
			throw new ArgumentException(string.Format("[{0}] {1} must be valid.", uuid, "space"), "space");
		}
		this.Space = space;
		this.Uuid = uuid;
		this.Anchor = anchor;
		this.ClearPoseCache();
		OVRSceneAnchor.SceneAnchors[this.Uuid] = this;
		OVRSceneAnchor.SceneAnchorsList.Add(this);
		int num;
		OVRSceneAnchor.AnchorReferenceCountDictionary.TryGetValue(this.Space, out num);
		OVRSceneAnchor.AnchorReferenceCountDictionary[this.Space] = num + 1;
		this._isLocatable = this.IsComponentSupported(OVRPlugin.SpaceComponentType.Locatable);
		if (base.enabled && this._isLocatable)
		{
			this.TryUpdateTransform(false);
		}
		this.SyncComponent<OVRSemanticClassification>(OVRPlugin.SpaceComponentType.SemanticLabels);
		this.SyncComponent<OVRSceneVolume>(OVRPlugin.SpaceComponentType.Bounded3D);
		this.SyncComponent<OVRScenePlane>(OVRPlugin.SpaceComponentType.Bounded2D);
	}

	public void InitializeFrom(OVRSceneAnchor other)
	{
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		this.Initialize(other.Anchor);
	}

	public static void GetSceneAnchors(List<OVRSceneAnchor> anchors)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		anchors.Clear();
		anchors.AddRange(OVRSceneAnchor.SceneAnchorsList);
	}

	public static void GetSceneAnchorsOfType<T>(List<T> anchors) where T : Object
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		anchors.Clear();
		using (List<OVRSceneAnchor>.Enumerator enumerator = OVRSceneAnchor.SceneAnchorsList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T item;
				if (enumerator.Current.TryGetComponent<T>(out item))
				{
					anchors.Add(item);
				}
			}
		}
	}

	internal bool TryUpdateTransform(bool useCache)
	{
		if (!this.Space.Valid || !base.enabled || !this._isLocatable)
		{
			return false;
		}
		if (!useCache || this._pose == null)
		{
			OVRPlugin.Posef value;
			OVRPlugin.SpaceLocationFlags value2;
			if (!(this.IsTracked = (OVRPlugin.TryLocateSpace(this.Space, OVRPlugin.GetTrackingOriginType(), out value, out value2) && value2.IsOrientationValid() && value2.IsPositionValid())))
			{
				return false;
			}
			this._pose = new OVRPlugin.Posef?(value);
		}
		OVRPose ovrpose = new OVRPose
		{
			position = this._pose.Value.Position.FromFlippedZVector3f(),
			orientation = this._pose.Value.Orientation.FromFlippedZQuatf() * OVRSceneAnchor.RotateY180
		}.ToWorldSpacePose(Camera.main);
		base.transform.SetPositionAndRotation(ovrpose.position, ovrpose.orientation);
		return true;
	}

	private void OnDestroy()
	{
		OVRSceneAnchor.SceneAnchors.Remove(this.Uuid);
		OVRSceneAnchor.SceneAnchorsList.Remove(this);
		if (!this.Space.Valid)
		{
			return;
		}
		int num;
		if (!OVRSceneAnchor.AnchorReferenceCountDictionary.TryGetValue(this.Space, out num))
		{
			return;
		}
		if (num == 1)
		{
			if (this.Space.Valid)
			{
				OVRPlugin.DestroySpace(this.Space);
			}
			OVRSceneAnchor.AnchorReferenceCountDictionary.Remove(this.Space);
			return;
		}
		OVRSceneAnchor.AnchorReferenceCountDictionary[this.Space] = num - 1;
	}

	private static readonly Quaternion RotateY180 = Quaternion.Euler(0f, 180f, 0f);

	private OVRPlugin.Posef? _pose;

	private bool _isLocatable;

	private readonly List<OVRPlugin.SpaceComponentType> _supportedComponents = new List<OVRPlugin.SpaceComponentType>();

	private static readonly Dictionary<OVRSpace, int> AnchorReferenceCountDictionary = new Dictionary<OVRSpace, int>();

	internal static readonly Dictionary<Guid, OVRSceneAnchor> SceneAnchors = new Dictionary<Guid, OVRSceneAnchor>();

	internal static readonly List<OVRSceneAnchor> SceneAnchorsList = new List<OVRSceneAnchor>();
}
