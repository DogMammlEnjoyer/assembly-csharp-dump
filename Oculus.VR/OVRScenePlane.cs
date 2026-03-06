using System;
using System.Collections.Generic;
using Meta.XR.Util;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(OVRSceneAnchor))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#further-scene-model-unity-components")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRScenePlane : MonoBehaviour, IOVRSceneComponent
{
	public float Width { get; private set; }

	public float Height { get; private set; }

	public Vector2 Offset { get; private set; }

	public Vector2 Dimensions
	{
		get
		{
			return new Vector2(this.Width, this.Height);
		}
	}

	public IReadOnlyList<Vector2> Boundary
	{
		get
		{
			return this._boundary;
		}
	}

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

	private void SetChildScale()
	{
		OVRSceneVolume ovrsceneVolume;
		bool flag = base.TryGetComponent<OVRSceneVolume>(out ovrsceneVolume);
		int i = 0;
		while (i < base.transform.childCount)
		{
			Transform child = base.transform.GetChild(i);
			OVRSceneObjectTransformType ovrsceneObjectTransformType;
			if (child.TryGetComponent<OVRSceneObjectTransformType>(out ovrsceneObjectTransformType))
			{
				if (ovrsceneObjectTransformType.TransformType == OVRSceneObjectTransformType.Transformation.Plane)
				{
					goto IL_3B;
				}
			}
			else if (!flag || !ovrsceneVolume.ScaleChildren)
			{
				goto IL_3B;
			}
			IL_5D:
			i++;
			continue;
			IL_3B:
			child.localScale = new Vector3(this.Width, this.Height, child.localScale.z);
			goto IL_5D;
		}
	}

	private void SetChildOffset()
	{
		OVRSceneVolume ovrsceneVolume;
		bool flag = base.TryGetComponent<OVRSceneVolume>(out ovrsceneVolume);
		int i = 0;
		while (i < base.transform.childCount)
		{
			Transform child = base.transform.GetChild(i);
			OVRSceneObjectTransformType ovrsceneObjectTransformType;
			if (child.TryGetComponent<OVRSceneObjectTransformType>(out ovrsceneObjectTransformType))
			{
				if (ovrsceneObjectTransformType.TransformType == OVRSceneObjectTransformType.Transformation.Plane)
				{
					goto IL_3B;
				}
			}
			else if (!flag || !ovrsceneVolume.OffsetChildren)
			{
				goto IL_3B;
			}
			IL_61:
			i++;
			continue;
			IL_3B:
			child.localPosition = new Vector3(this.Offset.x, this.Offset.y, 0f);
			goto IL_61;
		}
	}

	internal void UpdateTransform()
	{
		OVRPlugin.Rectf rectf;
		if (OVRPlugin.GetSpaceBoundingBox2D(base.GetComponent<OVRSceneAnchor>().Space, out rectf))
		{
			this.Width = rectf.Size.w;
			this.Height = rectf.Size.h;
			Vector2 a = base.transform.TransformPoint(rectf.Pos.FromVector2f() + rectf.Size.FromSizef() / 2f);
			Vector2 b = new Vector2(base.transform.position.x, base.transform.position.y);
			this.Offset = a - b;
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

	private void Awake()
	{
		this._sceneAnchor = base.GetComponent<OVRSceneAnchor>();
		if (this._sceneAnchor.Space.Valid)
		{
			((IOVRSceneComponent)this).Initialize();
		}
	}

	private void Start()
	{
		this.RequestBoundary();
	}

	void IOVRSceneComponent.Initialize()
	{
		this.UpdateTransform();
	}

	internal void ScheduleGetLengthJob()
	{
		if (this._jobHandle != null)
		{
			return;
		}
		bool flag;
		bool flag2;
		if (!OVRPlugin.GetSpaceComponentStatus(this._sceneAnchor.Space, OVRPlugin.SpaceComponentType.Bounded2D, out flag, out flag2))
		{
			return;
		}
		if (!flag || flag2)
		{
			return;
		}
		this._boundaryLength = new NativeArray<int>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		this._jobHandle = new JobHandle?(new OVRScenePlane.GetBoundaryLengthJob
		{
			Length = this._boundaryLength,
			Space = this._sceneAnchor.Space
		}.Schedule(default(JobHandle)));
		this._boundaryRequested = false;
	}

	internal void RequestBoundary()
	{
		this._boundaryRequested = true;
		if (base.enabled)
		{
			this.ScheduleGetLengthJob();
		}
	}

	private void Update()
	{
		if (this._jobHandle != null && this._jobHandle.GetValueOrDefault().IsCompleted)
		{
			this._jobHandle.Value.Complete();
			this._jobHandle = null;
			if (this._boundaryLength.IsCreated)
			{
				int num = this._boundaryLength[0];
				this._boundaryLength.Dispose();
				if (num < 3)
				{
					this.ScheduleGetLengthJob();
					return;
				}
				using (new OVRProfilerScope("Schedule GetBoundaryJob"))
				{
					this._boundaryBuffer = new NativeArray<Vector2>(num, Allocator.TempJob, NativeArrayOptions.ClearMemory);
					if (!this._previousBoundary.IsCreated)
					{
						this._previousBoundary = new NativeArray<Vector2>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
					}
					this._jobHandle = new JobHandle?(new OVRScenePlane.GetBoundaryJob
					{
						Space = this._sceneAnchor.Space,
						Boundary = this._boundaryBuffer,
						PreviousBoundary = this._previousBoundary
					}.Schedule(default(JobHandle)));
					return;
				}
			}
			if (this._boundaryBuffer.IsCreated)
			{
				using (new OVRProfilerScope("Copy boundary"))
				{
					if (this._previousBoundary.Length == 0 || float.IsNaN(this._previousBoundary[0].x))
					{
						if (this._previousBoundary.IsCreated)
						{
							this._previousBoundary.Dispose();
						}
						this._previousBoundary = new NativeArray<Vector2>(this._boundaryBuffer.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
						this._previousBoundary.CopyFrom(this._boundaryBuffer);
						this._boundary.Clear();
						foreach (Vector2 vector in this._previousBoundary)
						{
							this._boundary.Add(new Vector2(-vector.x, vector.y));
						}
					}
				}
				this._boundaryBuffer.Dispose();
				OVRScenePlaneMeshFilter ovrscenePlaneMeshFilter;
				if (base.TryGetComponent<OVRScenePlaneMeshFilter>(out ovrscenePlaneMeshFilter))
				{
					ovrscenePlaneMeshFilter.RequestMeshGeneration();
					return;
				}
			}
			else if (this._boundaryRequested)
			{
				this.ScheduleGetLengthJob();
			}
			return;
		}
	}

	private void OnDisable()
	{
		if (this._boundaryLength.IsCreated)
		{
			this._boundaryLength.Dispose(this._jobHandle.GetValueOrDefault());
		}
		if (this._boundaryBuffer.IsCreated)
		{
			this._boundaryBuffer.Dispose(this._jobHandle.GetValueOrDefault());
		}
		if (this._previousBoundary.IsCreated)
		{
			this._previousBoundary.Dispose(this._jobHandle.GetValueOrDefault());
		}
		this._previousBoundary = default(NativeArray<Vector2>);
		this._boundaryBuffer = default(NativeArray<Vector2>);
		this._boundaryLength = default(NativeArray<int>);
		this._jobHandle = null;
	}

	[Tooltip("When enabled, scales the child transforms according to the dimensions of this plane. If both Volume and Plane components exist on the game object, the volume takes precedence.")]
	[SerializeField]
	internal bool _scaleChildren = true;

	[Tooltip("When enabled, offsets the child transforms according to the offset of this plane. If both Volume and Plane components exist on the game object, the volume takes precedence.")]
	[SerializeField]
	internal bool _offsetChildren = true;

	internal JobHandle? _jobHandle;

	private NativeArray<Vector2> _previousBoundary;

	private NativeArray<int> _boundaryLength;

	private NativeArray<Vector2> _boundaryBuffer;

	private bool _boundaryRequested;

	private OVRSceneAnchor _sceneAnchor;

	private readonly List<Vector2> _boundary = new List<Vector2>();

	private struct GetBoundaryLengthJob : IJob
	{
		public void Execute()
		{
			int num;
			this.Length[0] = (OVRPlugin.GetSpaceBoundary2DCount(this.Space, out num) ? num : 0);
		}

		public OVRSpace Space;

		[WriteOnly]
		public NativeArray<int> Length;
	}

	private struct GetBoundaryJob : IJob
	{
		private bool HasBoundaryChanged()
		{
			if (!this.PreviousBoundary.IsCreated)
			{
				return true;
			}
			if (this.Boundary.Length != this.PreviousBoundary.Length)
			{
				return true;
			}
			int length = this.Boundary.Length;
			for (int i = 0; i < length; i++)
			{
				if (Vector2.SqrMagnitude(this.Boundary[i] - this.PreviousBoundary[i]) > 1E-06f)
				{
					return true;
				}
			}
			return false;
		}

		private static void SetNaN(NativeArray<Vector2> array)
		{
			if (array.Length > 0)
			{
				array[0] = new Vector2(float.NaN, float.NaN);
			}
		}

		public void Execute()
		{
			if (OVRPlugin.GetSpaceBoundary2D(this.Space, this.Boundary) && this.HasBoundaryChanged())
			{
				OVRScenePlane.GetBoundaryJob.SetNaN(this.PreviousBoundary);
				return;
			}
			OVRScenePlane.GetBoundaryJob.SetNaN(this.Boundary);
		}

		public OVRSpace Space;

		public NativeArray<Vector2> Boundary;

		public NativeArray<Vector2> PreviousBoundary;
	}
}
