using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace UnityEngine.Splines
{
	[ExecuteInEditMode]
	[AddComponentMenu("Splines/Spline Instantiate")]
	public class SplineInstantiate : SplineComponent
	{
		[Obsolete("Use Container instead.", false)]
		public SplineContainer container
		{
			get
			{
				return this.Container;
			}
		}

		public SplineContainer Container
		{
			get
			{
				return this.m_Container;
			}
			set
			{
				this.m_Container = value;
			}
		}

		public SplineInstantiate.InstantiableItem[] itemsToInstantiate
		{
			get
			{
				return this.m_ItemsToInstantiate.ToArray();
			}
			set
			{
				this.m_DeprecatedInstances.AddRange(this.m_Instances);
				this.m_ItemsToInstantiate.Clear();
				this.m_ItemsToInstantiate.AddRange(value);
			}
		}

		[Obsolete("Use InstantiateMethod instead.", false)]
		public SplineInstantiate.Method method
		{
			get
			{
				return this.InstantiateMethod;
			}
		}

		public SplineInstantiate.Method InstantiateMethod
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				this.m_Method = value;
			}
		}

		[Obsolete("Use CoordinateSpace instead.", false)]
		public SplineInstantiate.Space space
		{
			get
			{
				return this.CoordinateSpace;
			}
		}

		public SplineInstantiate.Space CoordinateSpace
		{
			get
			{
				return this.m_Space;
			}
			set
			{
				this.m_Space = value;
			}
		}

		public float MinSpacing
		{
			get
			{
				return this.m_Spacing.x;
			}
			set
			{
				this.m_Spacing = new Vector2(value, this.m_Spacing.y);
				this.ValidateSpacing();
			}
		}

		public float MaxSpacing
		{
			get
			{
				return this.m_Spacing.y;
			}
			set
			{
				this.m_Spacing = new Vector2(this.m_Spacing.x, value);
				this.ValidateSpacing();
			}
		}

		[Obsolete("Use UpAxis instead.", false)]
		public SplineComponent.AlignAxis upAxis
		{
			get
			{
				return this.UpAxis;
			}
		}

		public SplineComponent.AlignAxis UpAxis
		{
			get
			{
				return this.m_Up;
			}
			set
			{
				this.m_Up = value;
			}
		}

		[Obsolete("Use ForwardAxis instead.", false)]
		public SplineComponent.AlignAxis forwardAxis
		{
			get
			{
				return this.ForwardAxis;
			}
		}

		public SplineComponent.AlignAxis ForwardAxis
		{
			get
			{
				return this.m_Forward;
			}
			set
			{
				this.m_Forward = value;
				this.ValidateAxis();
			}
		}

		[Obsolete("Use MinPositionOffset instead.", false)]
		public Vector3 minPositionOffset
		{
			get
			{
				return this.MinPositionOffset;
			}
		}

		public Vector3 MinPositionOffset
		{
			get
			{
				return this.m_PositionOffset.min;
			}
			set
			{
				this.m_PositionOffset.min = value;
				this.m_PositionOffset.CheckMinMax();
			}
		}

		[Obsolete("Use MaxPositionOffset instead.", false)]
		public Vector3 maxPositionOffset
		{
			get
			{
				return this.MaxPositionOffset;
			}
		}

		public Vector3 MaxPositionOffset
		{
			get
			{
				return this.m_PositionOffset.max;
			}
			set
			{
				this.m_PositionOffset.max = value;
				this.m_PositionOffset.CheckMinMax();
			}
		}

		[Obsolete("Use PositionSpace instead.", false)]
		public SplineInstantiate.OffsetSpace positionSpace
		{
			get
			{
				return this.PositionSpace;
			}
		}

		public SplineInstantiate.OffsetSpace PositionSpace
		{
			get
			{
				return this.m_PositionOffset.space;
			}
			set
			{
				this.m_PositionOffset.space = value;
				this.m_PositionOffset.CheckCustomSpace(this.m_Space);
			}
		}

		[Obsolete("Use MinRotationOffset instead.", false)]
		public Vector3 minRotationOffset
		{
			get
			{
				return this.MinRotationOffset;
			}
		}

		public Vector3 MinRotationOffset
		{
			get
			{
				return this.m_RotationOffset.min;
			}
			set
			{
				this.m_RotationOffset.min = value;
				this.m_RotationOffset.CheckMinMax();
			}
		}

		[Obsolete("Use MaxRotationOffset instead.", false)]
		public Vector3 maxRotationOffset
		{
			get
			{
				return this.MaxRotationOffset;
			}
		}

		public Vector3 MaxRotationOffset
		{
			get
			{
				return this.m_RotationOffset.max;
			}
			set
			{
				this.m_RotationOffset.max = value;
				this.m_RotationOffset.CheckMinMax();
			}
		}

		[Obsolete("Use RotationSpace instead.", false)]
		public SplineInstantiate.OffsetSpace rotationSpace
		{
			get
			{
				return this.RotationSpace;
			}
		}

		public SplineInstantiate.OffsetSpace RotationSpace
		{
			get
			{
				return this.m_RotationOffset.space;
			}
			set
			{
				this.m_RotationOffset.space = value;
				this.m_RotationOffset.CheckCustomSpace(this.m_Space);
			}
		}

		[Obsolete("Use MinScaleOffset instead.", false)]
		public Vector3 minScaleOffset
		{
			get
			{
				return this.MinScaleOffset;
			}
		}

		public Vector3 MinScaleOffset
		{
			get
			{
				return this.m_ScaleOffset.min;
			}
			set
			{
				this.m_ScaleOffset.min = value;
				this.m_ScaleOffset.CheckMinMax();
			}
		}

		[Obsolete("Use MaxScaleOffset instead.", false)]
		public Vector3 maxScaleOffset
		{
			get
			{
				return this.MaxScaleOffset;
			}
		}

		public Vector3 MaxScaleOffset
		{
			get
			{
				return this.m_ScaleOffset.max;
			}
			set
			{
				this.m_ScaleOffset.max = value;
				this.m_ScaleOffset.CheckMinMax();
			}
		}

		[Obsolete("Use ScaleSpace instead.", false)]
		public SplineInstantiate.OffsetSpace scaleSpace
		{
			get
			{
				return this.ScaleSpace;
			}
		}

		public SplineInstantiate.OffsetSpace ScaleSpace
		{
			get
			{
				return this.m_ScaleOffset.space;
			}
			set
			{
				this.m_ScaleOffset.space = value;
				this.m_ScaleOffset.CheckCustomSpace(this.m_Space);
			}
		}

		internal GameObject InstancesRoot
		{
			get
			{
				return this.m_InstancesRoot;
			}
		}

		private Transform instancesRootTransform
		{
			get
			{
				if (this.m_InstancesRoot == null)
				{
					this.m_InstancesRoot = new GameObject("root-" + base.GetInstanceID().ToString());
					this.m_InstancesRoot.hideFlags |= HideFlags.HideAndDontSave;
					this.m_InstancesRoot.transform.parent = base.transform;
					this.m_InstancesRoot.transform.localPosition = Vector3.zero;
					this.m_InstancesRoot.transform.localRotation = Quaternion.identity;
				}
				return this.m_InstancesRoot.transform;
			}
		}

		internal List<GameObject> instances
		{
			get
			{
				return this.m_Instances;
			}
		}

		private float maxProbability
		{
			get
			{
				return this.m_MaxProbability;
			}
			set
			{
				if (this.m_MaxProbability != value)
				{
					this.m_MaxProbability = value;
					this.m_InstancesCacheDirty = true;
				}
			}
		}

		public int Seed
		{
			get
			{
				return this.m_Seed;
			}
			set
			{
				this.m_Seed = value;
				this.m_InstancesCacheDirty = true;
			}
		}

		private void OnEnable()
		{
			if (this.m_Seed == 0)
			{
				this.m_Seed = base.GetInstanceID();
			}
			this.CheckChildrenValidity();
			Spline.Changed += this.OnSplineChanged;
			this.UpdateInstances();
		}

		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChanged;
			this.Clear();
		}

		private void UndoRedoPerformed()
		{
			this.m_InstancesCacheDirty = true;
			this.m_SplineDirty = true;
		}

		private void OnValidate()
		{
			this.ValidateSpacing();
			this.m_SplineDirty = this.m_AutoRefresh;
			this.EnsureItemsValidity();
			this.m_PositionOffset.CheckMinMaxValidity();
			this.m_RotationOffset.CheckMinMaxValidity();
			this.m_ScaleOffset.CheckMinMaxValidity();
		}

		private void EnsureItemsValidity()
		{
			float num = 0f;
			for (int i = 0; i < this.m_ItemsToInstantiate.Count; i++)
			{
				SplineInstantiate.InstantiableItem instantiableItem = this.m_ItemsToInstantiate[i];
				if (instantiableItem.Prefab != null)
				{
					if (base.transform.IsChildOf(instantiableItem.Prefab.transform))
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"Instantiating a parent of the SplineInstantiate object itself is not permitted (",
							instantiableItem.Prefab.name,
							" is a parent of ",
							base.transform.gameObject.name,
							")."
						}), this);
						instantiableItem.Prefab = null;
						this.m_ItemsToInstantiate[i] = instantiableItem;
					}
					else
					{
						num += instantiableItem.Probability;
					}
				}
			}
			this.maxProbability = num;
		}

		private void CheckChildrenValidity()
		{
			List<int> list = (from sInstantiate in base.GetComponents<SplineInstantiate>()
			select sInstantiate.GetInstanceID()).ToList<int>();
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				GameObject gameObject = base.transform.GetChild(i).gameObject;
				if (gameObject.name.StartsWith("root-"))
				{
					bool flag = true;
					foreach (int num in list)
					{
						if (gameObject.name.Equals("root-" + num.ToString()))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						Object.Destroy(gameObject);
					}
				}
			}
		}

		private void ValidateSpacing()
		{
			float num = Mathf.Max(0.1f, this.m_Spacing.x);
			if (this.m_Method != SplineInstantiate.Method.LinearDistance)
			{
				float b = float.IsNaN(this.m_Spacing.y) ? num : Mathf.Max(0.1f, this.m_Spacing.y);
				this.m_Spacing = new Vector2(num, Mathf.Max(num, b));
				return;
			}
			if (this.m_Method == SplineInstantiate.Method.LinearDistance)
			{
				float y = float.IsNaN(this.m_Spacing.y) ? this.m_Spacing.y : num;
				this.m_Spacing = new Vector2(num, y);
			}
		}

		private void ValidateAxis()
		{
			if (this.m_Forward == this.m_Up || this.m_Forward == (this.m_Up + 3) % (SplineComponent.AlignAxis)6)
			{
				this.m_Forward = (this.m_Forward + 1) % (SplineComponent.AlignAxis)6;
			}
		}

		internal void SetSplineDirty(Spline spline)
		{
			if (this.m_Container != null && this.m_Container.Splines.Contains(spline) && this.m_AutoRefresh)
			{
				this.UpdateInstances();
			}
		}

		private void InitContainer()
		{
			if (this.m_Container == null)
			{
				this.m_Container = base.GetComponent<SplineContainer>();
			}
		}

		public void Clear()
		{
			this.SetDirty();
			this.TryClearCache();
		}

		public void SetDirty()
		{
			this.m_InstancesCacheDirty = true;
		}

		private void TryClearCache()
		{
			if (!this.m_InstancesCacheDirty)
			{
				for (int i = 0; i < this.m_Instances.Count; i++)
				{
					if (this.m_Instances[i] == null)
					{
						this.m_InstancesCacheDirty = true;
						break;
					}
				}
			}
			if (this.m_InstancesCacheDirty)
			{
				for (int j = this.m_Instances.Count - 1; j >= 0; j--)
				{
					Object.Destroy(this.m_Instances[j]);
				}
				Object.Destroy(this.m_InstancesRoot);
				this.m_Instances.Clear();
				this.m_InstancesCacheDirty = false;
			}
		}

		private void ClearDeprecatedInstances()
		{
			foreach (GameObject obj in this.m_DeprecatedInstances)
			{
				Object.Destroy(obj);
			}
			this.m_DeprecatedInstances.Clear();
		}

		public void Randomize()
		{
			this.Seed = Random.Range(int.MinValue, int.MaxValue);
			this.m_SplineDirty = true;
		}

		private void Update()
		{
			if (this.m_SplineDirty)
			{
				this.UpdateInstances();
			}
		}

		public void UpdateInstances()
		{
			this.ClearDeprecatedInstances();
			this.TryClearCache();
			if (this.m_Container == null)
			{
				this.InitContainer();
			}
			if (this.m_Container == null || this.m_Container.Splines.Count == 0 || this.m_ItemsToInstantiate.Count == 0)
			{
				return;
			}
			Random.State state = Random.state;
			Random.InitState(this.m_Seed);
			int num = 0;
			int num2 = 0;
			this.m_LengthsCache.Clear();
			float num3 = 0f;
			for (int i = 0; i < this.m_Container.Splines.Count; i++)
			{
				float num4 = this.m_Container.CalculateLength(i);
				this.m_LengthsCache.Add(num4);
				num3 += num4;
			}
			float num5 = Random.Range(this.m_Spacing.x, this.m_Spacing.y);
			float num6 = 0f;
			float num7 = 0f;
			if (this.m_Method == SplineInstantiate.Method.InstanceCount)
			{
				if (num5 == 1f)
				{
					num6 = num3 / 2f;
				}
				else if (num5 < 1f)
				{
					num6 = num3 + 1f;
				}
				if (this.m_Container.Splines.Count == 1)
				{
					num7 = num3 / (float)(this.m_Container.Splines[0].Closed ? ((int)num5) : ((int)num5 - 1));
				}
				else
				{
					num7 = num3 / (float)((int)num5 - 1);
				}
			}
			this.EnsureItemsValidity();
			for (int j = 0; j < this.m_Container.Splines.Count; j++)
			{
				Spline spline = this.m_Container.Splines[j];
				using (NativeSpline spline2 = new NativeSpline(spline, this.m_Container.transform.localToWorldMatrix, Allocator.TempJob))
				{
					float num8 = this.m_LengthsCache[j];
					bool flag = false;
					if (this.m_Method == SplineInstantiate.Method.InstanceCount)
					{
						if (num6 > num8 + 0.001f && num6 <= num3 + 0.001f)
						{
							num6 -= num8;
							flag = true;
						}
					}
					else
					{
						num6 = 0f;
					}
					this.m_TimesCache.Clear();
					int num9 = 0;
					while (num6 <= num8 + 0.001f && !flag && this.SpawnPrefab(num))
					{
						this.m_TimesCache.Add(num6 / num8);
						if (this.m_Method == SplineInstantiate.Method.SpacingDistance)
						{
							num5 = Random.Range(this.m_Spacing.x, this.m_Spacing.y);
							num6 += num5;
						}
						else if (this.m_Method == SplineInstantiate.Method.InstanceCount)
						{
							if (num5 > 1f)
							{
								float num10 = num6;
								num6 += num7;
								if (num10 < num8 && num6 > num8 + 0.001f)
								{
									num6 -= num8;
									flag = true;
								}
							}
							else
							{
								num6 += num3;
							}
						}
						else if (this.m_Method == SplineInstantiate.Method.LinearDistance)
						{
							if (float.IsNaN(this.m_Spacing.y))
							{
								MeshFilter meshFilter = this.m_Instances[num].GetComponent<MeshFilter>();
								Vector3 vector = Vector3.right;
								if (this.m_Forward == SplineComponent.AlignAxis.ZAxis || this.m_Forward == SplineComponent.AlignAxis.NegativeZAxis)
								{
									vector = Vector3.forward;
								}
								if (this.m_Forward == SplineComponent.AlignAxis.YAxis || this.m_Forward == SplineComponent.AlignAxis.NegativeYAxis)
								{
									vector = Vector3.up;
								}
								if (meshFilter == null)
								{
									meshFilter = this.m_Instances[num].GetComponentInChildren<MeshFilter>();
									if (meshFilter != null)
									{
										vector = Vector3.Scale(meshFilter.transform.InverseTransformDirection(this.m_Instances[num].transform.TransformDirection(vector)), meshFilter.transform.lossyScale);
									}
								}
								if (meshFilter != null)
								{
									Bounds bounds = meshFilter.sharedMesh.bounds;
									MeshFilter[] componentsInChildren = meshFilter.GetComponentsInChildren<MeshFilter>();
									for (int k = 0; k < componentsInChildren.Length; k++)
									{
										Bounds bounds2 = componentsInChildren[k].sharedMesh.bounds;
										bounds.size = new Vector3(Mathf.Max(bounds.size.x, bounds2.size.x), Mathf.Max(bounds.size.z, bounds2.size.z), Mathf.Max(bounds.size.z, bounds2.size.z));
									}
									num5 = Vector3.Scale(bounds.size, vector).magnitude;
								}
							}
							else
							{
								num5 = Random.Range(this.m_Spacing.x, this.m_Spacing.y);
							}
							float num11;
							spline2.GetPointAtLinearDistance(this.m_TimesCache[num9], num5, out num11);
							num6 = ((num11 >= 1f) ? (num8 + 1f) : (num11 * num8));
						}
						num++;
						num9++;
					}
					for (int l = this.m_Instances.Count - 1; l >= num; l--)
					{
						if (this.m_Instances[l] != null)
						{
							Object.Destroy(this.m_Instances[l]);
							this.m_Instances.RemoveAt(l);
						}
					}
					for (int m = num2; m < num; m++)
					{
						GameObject gameObject = this.m_Instances[m];
						float t = this.m_TimesCache[m - num2];
						float3 @float;
						float3 float2;
						float3 float3;
						spline2.Evaluate(t, out @float, out float2, out float3);
						gameObject.transform.position = @float;
						if (this.m_Method == SplineInstantiate.Method.LinearDistance)
						{
							float2 = spline2.EvaluatePosition((m + 1 < num) ? this.m_TimesCache[m + 1 - num2] : 1f) - @float;
						}
						float3 float4 = math.normalizesafe(float3, default(float3));
						float3 float5 = math.normalizesafe(float2, default(float3));
						if (this.m_Space == SplineInstantiate.Space.World)
						{
							float4 = Vector3.up;
							float5 = Vector3.forward;
						}
						else if (this.m_Space == SplineInstantiate.Space.Local)
						{
							float4 = base.transform.TransformDirection(Vector3.up);
							float5 = base.transform.TransformDirection(Vector3.forward);
						}
						float3 forward = math.normalizesafe(base.GetAxis(this.m_Forward), default(float3));
						float3 up = math.normalizesafe(base.GetAxis(this.m_Up), default(float3));
						Quaternion rhs = Quaternion.Inverse(quaternion.LookRotationSafe(forward, up));
						gameObject.transform.rotation = quaternion.LookRotationSafe(float5, float4) * rhs;
						float3 float6 = float4;
						float3 float7 = float5;
						if (this.m_PositionOffset.hasOffset)
						{
							if (this.m_PositionOffset.hasCustomSpace)
							{
								this.GetCustomSpaceAxis(this.m_PositionOffset.space, float3, float2, gameObject.transform, out float6, out float7);
							}
							Vector3 nextOffset = this.m_PositionOffset.GetNextOffset();
							Vector3 normalized = Vector3.Cross(float6, float7).normalized;
							gameObject.transform.position += nextOffset.x * normalized + nextOffset.y * float6 + nextOffset.z * float7;
						}
						if (this.m_ScaleOffset.hasOffset)
						{
							float6 = float4;
							float7 = float5;
							if (this.m_ScaleOffset.hasCustomSpace)
							{
								this.GetCustomSpaceAxis(this.m_ScaleOffset.space, float3, float2, gameObject.transform, out float6, out float7);
							}
							float6 = gameObject.transform.InverseTransformDirection(float6).normalized;
							float7 = gameObject.transform.InverseTransformDirection(float7).normalized;
							Vector3 nextOffset2 = this.m_ScaleOffset.GetNextOffset();
							Vector3 normalized2 = Vector3.Cross(float6, float7).normalized;
							gameObject.transform.localScale += nextOffset2.x * normalized2 + nextOffset2.y * float6 + nextOffset2.z * float7;
						}
						if (this.m_RotationOffset.hasOffset)
						{
							float6 = float4;
							float7 = float5;
							if (this.m_RotationOffset.hasCustomSpace)
							{
								this.GetCustomSpaceAxis(this.m_RotationOffset.space, float3, float2, gameObject.transform, out float6, out float7);
								if (this.m_RotationOffset.space == SplineInstantiate.OffsetSpace.Object)
								{
									rhs = quaternion.identity;
								}
							}
							Vector3 nextOffset3 = this.m_RotationOffset.GetNextOffset();
							Vector3 normalized3 = Vector3.Cross(float6, float7).normalized;
							float7 = Quaternion.AngleAxis(nextOffset3.y, float6) * Quaternion.AngleAxis(nextOffset3.x, normalized3) * float7;
							float6 = Quaternion.AngleAxis(nextOffset3.x, normalized3) * Quaternion.AngleAxis(nextOffset3.z, float7) * float6;
							gameObject.transform.rotation = quaternion.LookRotationSafe(float7, float6) * rhs;
						}
					}
					num2 = num;
				}
			}
			this.m_SplineDirty = false;
			Random.state = state;
		}

		private bool SpawnPrefab(int index)
		{
			int index2 = (this.m_ItemsToInstantiate.Count == 1) ? 0 : this.GetPrefabIndex();
			this.m_CurrentItem = this.m_ItemsToInstantiate[index2];
			if (this.m_CurrentItem.Prefab == null)
			{
				return false;
			}
			if (index >= this.m_Instances.Count)
			{
				this.m_Instances.Add(Object.Instantiate<GameObject>(this.m_CurrentItem.Prefab, this.instancesRootTransform));
			}
			this.m_Instances[index].transform.localPosition = this.m_CurrentItem.Prefab.transform.localPosition;
			this.m_Instances[index].transform.localRotation = this.m_CurrentItem.Prefab.transform.localRotation;
			this.m_Instances[index].transform.localScale = this.m_CurrentItem.Prefab.transform.localScale;
			return true;
		}

		private void GetCustomSpaceAxis(SplineInstantiate.OffsetSpace space, float3 splineUp, float3 direction, Transform instanceTransform, out float3 customUp, out float3 customForward)
		{
			customUp = Vector3.up;
			customForward = Vector3.forward;
			if (space == SplineInstantiate.OffsetSpace.Local)
			{
				customUp = base.transform.TransformDirection(Vector3.up);
				customForward = base.transform.TransformDirection(Vector3.forward);
				return;
			}
			if (space == SplineInstantiate.OffsetSpace.Spline)
			{
				customUp = splineUp;
				customForward = direction;
				return;
			}
			if (space == SplineInstantiate.OffsetSpace.Object)
			{
				customUp = instanceTransform.TransformDirection(Vector3.up);
				customForward = instanceTransform.TransformDirection(Vector3.forward);
			}
		}

		private int GetPrefabIndex()
		{
			float num = Random.Range(0f, this.m_MaxProbability);
			float num2 = 0f;
			for (int i = 0; i < this.m_ItemsToInstantiate.Count; i++)
			{
				if (!(this.m_ItemsToInstantiate[i].Prefab == null))
				{
					float probability = this.m_ItemsToInstantiate[i].Probability;
					if (num < num2 + probability)
					{
						return i;
					}
					num2 += probability;
				}
			}
			return 0;
		}

		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
		{
			if (this.m_Container != null && this.m_Container.Spline == spline)
			{
				this.m_SplineDirty = this.m_AutoRefresh;
			}
		}

		[SerializeField]
		private SplineContainer m_Container;

		[SerializeField]
		private List<SplineInstantiate.InstantiableItem> m_ItemsToInstantiate = new List<SplineInstantiate.InstantiableItem>();

		[SerializeField]
		private SplineInstantiate.Method m_Method = SplineInstantiate.Method.SpacingDistance;

		[SerializeField]
		private SplineInstantiate.Space m_Space;

		[SerializeField]
		private Vector2 m_Spacing = new Vector2(1f, 1f);

		[SerializeField]
		private SplineComponent.AlignAxis m_Up = SplineComponent.AlignAxis.YAxis;

		[SerializeField]
		private SplineComponent.AlignAxis m_Forward = SplineComponent.AlignAxis.ZAxis;

		[SerializeField]
		private SplineInstantiate.Vector3Offset m_PositionOffset;

		[SerializeField]
		private SplineInstantiate.Vector3Offset m_RotationOffset;

		[SerializeField]
		private SplineInstantiate.Vector3Offset m_ScaleOffset;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("m_Instances")]
		private List<GameObject> m_DeprecatedInstances = new List<GameObject>();

		private const string k_InstancesRootName = "root-";

		private GameObject m_InstancesRoot;

		private readonly List<GameObject> m_Instances = new List<GameObject>();

		private bool m_InstancesCacheDirty;

		[SerializeField]
		private bool m_AutoRefresh = true;

		private SplineInstantiate.InstantiableItem m_CurrentItem;

		private bool m_SplineDirty;

		private float m_MaxProbability = 1f;

		[SerializeField]
		private int m_Seed;

		private List<float> m_TimesCache = new List<float>();

		private List<float> m_LengthsCache = new List<float>();

		public enum OffsetSpace
		{
			[InspectorName("Spline Element")]
			Spline,
			[InspectorName("Spline Object")]
			Local,
			[InspectorName("World Space")]
			World,
			[InspectorName("Instantiated Object")]
			Object
		}

		[Serializable]
		internal struct Vector3Offset
		{
			public bool hasOffset
			{
				get
				{
					return (this.setup & SplineInstantiate.Vector3Offset.Setup.HasOffset) > SplineInstantiate.Vector3Offset.Setup.None;
				}
			}

			public bool hasCustomSpace
			{
				get
				{
					return (this.setup & SplineInstantiate.Vector3Offset.Setup.HasCustomSpace) > SplineInstantiate.Vector3Offset.Setup.None;
				}
			}

			internal Vector3 GetNextOffset()
			{
				if ((this.setup & SplineInstantiate.Vector3Offset.Setup.HasOffset) != SplineInstantiate.Vector3Offset.Setup.None)
				{
					return new Vector3(this.randomX ? Random.Range(this.min.x, this.max.x) : this.min.x, this.randomY ? Random.Range(this.min.y, this.max.y) : this.min.y, this.randomZ ? Random.Range(this.min.z, this.max.z) : this.min.z);
				}
				return Vector3.zero;
			}

			internal void CheckMinMaxValidity()
			{
				this.max.x = Mathf.Max(this.min.x, this.max.x);
				this.max.y = Mathf.Max(this.min.y, this.max.y);
				this.max.z = Mathf.Max(this.min.z, this.max.z);
			}

			internal void CheckMinMax()
			{
				this.CheckMinMaxValidity();
				if (this.max.magnitude > 0f)
				{
					this.setup |= SplineInstantiate.Vector3Offset.Setup.HasOffset;
					return;
				}
				this.setup &= ~SplineInstantiate.Vector3Offset.Setup.HasOffset;
			}

			internal void CheckCustomSpace(SplineInstantiate.Space instanceSpace)
			{
				if (this.space == (SplineInstantiate.OffsetSpace)instanceSpace)
				{
					this.setup &= ~SplineInstantiate.Vector3Offset.Setup.HasCustomSpace;
					return;
				}
				this.setup |= SplineInstantiate.Vector3Offset.Setup.HasCustomSpace;
			}

			public SplineInstantiate.Vector3Offset.Setup setup;

			public Vector3 min;

			public Vector3 max;

			public bool randomX;

			public bool randomY;

			public bool randomZ;

			public SplineInstantiate.OffsetSpace space;

			[Flags]
			public enum Setup
			{
				None = 0,
				HasOffset = 1,
				HasCustomSpace = 2
			}
		}

		[Serializable]
		public struct InstantiableItem
		{
			[HideInInspector]
			[Obsolete("Use Prefab instead.", false)]
			public GameObject prefab;

			[FormerlySerializedAs("prefab")]
			public GameObject Prefab;

			[HideInInspector]
			[Obsolete("Use Probability instead.", false)]
			public float probability;

			[FormerlySerializedAs("probability")]
			public float Probability;
		}

		public enum Method
		{
			[InspectorName("Instance Count")]
			InstanceCount,
			[InspectorName("Spline Distance")]
			SpacingDistance,
			[InspectorName("Linear Distance")]
			LinearDistance
		}

		public enum Space
		{
			[InspectorName("Spline Element")]
			Spline,
			[InspectorName("Spline Object")]
			Local,
			[InspectorName("World Space")]
			World
		}
	}
}
