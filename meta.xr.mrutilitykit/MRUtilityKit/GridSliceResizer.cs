using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_grid_slice_resizer")]
	[RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class GridSliceResizer : MonoBehaviour
	{
		private void Awake()
		{
			this._meshFilter = base.GetComponent<MeshFilter>();
			if (this.OriginalMesh != null)
			{
				this._currentMesh = this.OriginalMesh;
				this._meshFilter.sharedMesh = this.OriginalMesh;
			}
			else
			{
				this._currentMesh = (this.OriginalMesh = this._meshFilter.sharedMesh);
			}
			this._currentSize = this.OriginalMesh.bounds.size;
			this._cachedBorderXNegative = this.BorderXNegative;
			this._cachedBorderYNegative = this.BorderYNegative;
			this._cachedBorderZNegative = this.BorderZNegative;
			this._cachedBorderXPositive = this.BorderXPositive;
			this._cachedBorderYPositive = this.BorderYPositive;
			this._cachedBorderZPositive = this.BorderZPositive;
			this._meshCollider = base.GetComponent<MeshCollider>();
		}

		private void Start()
		{
			OVRTelemetry.Start(651896136, 0, -1L).Send();
		}

		public void Update()
		{
			if ((Application.isPlaying && !this.UpdateInPlayMode) || !this._meshFilter || !this.ShouldResize())
			{
				return;
			}
			Mesh mesh = this.ProcessVertices();
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			mesh.Optimize();
			this._meshFilter.sharedMesh = mesh;
			this._currentSize = base.transform.lossyScale;
			this.UpdateCachedValues();
			if (!this._meshCollider)
			{
				base.TryGetComponent<MeshCollider>(out this._meshCollider);
				if (!this._meshCollider)
				{
					return;
				}
			}
			this._meshCollider.sharedMesh = null;
			this._meshCollider.sharedMesh = mesh;
		}

		private void OnDestroy()
		{
			if (this.OriginalMesh)
			{
				this._meshFilter.mesh = this.OriginalMesh;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			float d = this.OriginalMesh.bounds.max.x / 10f;
			Vector3 a = base.transform.TransformPoint(this.PivotOffset);
			Vector3 from = a + Vector3.left * d * 0.5f;
			Vector3 from2 = a + Vector3.down * d * 0.5f;
			Vector3 from3 = a + Vector3.back * d * 0.5f;
			Gizmos.DrawRay(from, Vector3.right * d);
			Gizmos.DrawRay(from2, Vector3.up * d);
			Gizmos.DrawRay(from3, Vector3.forward * d);
		}

		private void OnDrawGizmosSelected()
		{
			if (this._meshFilter == null)
			{
				return;
			}
			Gizmos.matrix = base.transform.localToWorldMatrix;
			GridSliceResizer.Method[] array = new GridSliceResizer.Method[]
			{
				this.ScalingX,
				this.ScalingY,
				this.ScalingZ
			};
			float[] array2 = new float[]
			{
				this.BorderXNegative,
				this.BorderYNegative,
				this.BorderZNegative
			};
			float[] array3 = new float[]
			{
				this.BorderXPositive,
				this.BorderYPositive,
				this.BorderZPositive
			};
			Vector3 lossyScale = base.transform.lossyScale;
			Vector3 b = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Vector3 vector = Vector3.Scale(this.OriginalMesh.bounds.min, b);
			Vector3 vector2 = Vector3.Scale(this.OriginalMesh.bounds.max, b);
			new Bounds((vector + vector2) * 0.5f, vector2 - vector);
			for (int i = 0; i <= 2; i++)
			{
				Gizmos.color = this._axisGizmosColors[i];
				this.DrawBorderCubeGizmo(array[i], array2[i], array3[i], i);
			}
			Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
			Gizmos.DrawWireCube(this._boundingBox.center, this._boundingBox.size);
		}

		public Mesh ProcessVertices()
		{
			Mesh currentMesh = this._currentMesh;
			Vector3 lossyScale = base.transform.lossyScale;
			this.BorderXNegative = Mathf.Max(this.BorderXNegative, 0.01f);
			this.BorderYNegative = Mathf.Max(this.BorderYNegative, 0.01f);
			this.BorderZNegative = Mathf.Max(this.BorderZNegative, 0.01f);
			this.BorderXPositive = Mathf.Max(this.BorderXPositive, 0.01f);
			this.BorderYPositive = Mathf.Max(this.BorderYPositive, 0.01f);
			this.BorderZPositive = Mathf.Max(this.BorderZPositive, 0.01f);
			this._pivotTransform.SetColumn(3, -this.PivotOffset);
			this._scaledInvPivotTransform.SetColumn(3, Vector3.Scale(lossyScale, this.PivotOffset));
			Vector3 vector = this._pivotTransform.MultiplyPoint3x4(Vector3.Min(currentMesh.bounds.min, this.PivotOffset));
			Vector3 vector2 = this._pivotTransform.MultiplyPoint3x4(Vector3.Max(currentMesh.bounds.max, this.PivotOffset));
			this._boundingBox = new Bounds((vector + vector2) * 0.5f, vector2 - vector);
			this._scaledBoundingBox = this.ScaleBounds(this._boundingBox, base.transform.lossyScale);
			Vector3 a = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Vector3 vector3 = new Vector3(this.BorderXPositive, this.BorderYPositive, this.BorderZPositive);
			Vector3 vector4 = new Vector3(this.BorderXNegative, this.BorderYNegative, this.BorderZNegative);
			GridSliceResizer.Method[] array = new GridSliceResizer.Method[]
			{
				this.ScalingX,
				this.ScalingY,
				this.ScalingZ
			};
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			Vector3 zero5 = Vector3.zero;
			Vector3 zero6 = Vector3.zero;
			Vector3 zero7 = Vector3.zero;
			Vector3 zero8 = Vector3.zero;
			Vector3 zero9 = Vector3.zero;
			Vector3 zero10 = Vector3.zero;
			Vector3 zero11 = Vector3.zero;
			Vector3 zero12 = Vector3.zero;
			bool[] array2 = new bool[]
			{
				(this.StretchCenter & GridSliceResizer.StretchCenterAxis.X) > (GridSliceResizer.StretchCenterAxis)0,
				(this.StretchCenter & GridSliceResizer.StretchCenterAxis.Y) > (GridSliceResizer.StretchCenterAxis)0,
				(this.StretchCenter & GridSliceResizer.StretchCenterAxis.Z) > (GridSliceResizer.StretchCenterAxis)0
			};
			for (int i = 0; i < 3; i++)
			{
				switch (array[i])
				{
				case GridSliceResizer.Method.SLICE:
					vector3[i] = vector4[i];
					break;
				case GridSliceResizer.Method.SCALE:
					array2[i] = true;
					vector4[i] = (vector3[i] = 1f);
					break;
				}
				float num = this._boundingBox.max[i];
				float num2 = this._boundingBox.min[i];
				zero[i] = num - (1f - vector3[i]) * Mathf.Abs(num);
				zero2[i] = num2 + (1f - vector4[i]) * Mathf.Abs(num2);
				zero3[i] = Mathf.Abs(num - zero[i]);
				zero4[i] = Mathf.Abs(num2 - zero2[i]);
				zero5[i] = num - zero3[i];
				zero6[i] = num2 + zero4[i];
				zero7[i] = this._scaledBoundingBox.max[i] - zero3[i];
				zero8[i] = this._scaledBoundingBox.min[i] + zero4[i];
				zero9[i] = Mathf.Max(0f, zero7[i] / zero5[i]);
				zero10[i] = Mathf.Max(0f, zero8[i] / zero6[i]);
				zero11[i] = this._scaledBoundingBox.max[i] / zero3[i];
				zero12[i] = this._scaledBoundingBox.min[i] / zero4[i];
			}
			Vector3[] vertices = currentMesh.vertices;
			Vector3[] array3 = new Vector3[vertices.Length];
			bool[] array4 = new bool[3];
			for (int j = 0; j < vertices.Length; j++)
			{
				Vector3 vector5 = array3[j] = this._pivotTransform.MultiplyPoint3x4(vertices[j]);
				for (int k = 0; k < 3; k++)
				{
					if (0f <= vector5[k] && vector5[k] <= zero[k] && vector5[k] > zero7[k])
					{
						array4[k] = true;
					}
					else if (zero2[k] <= vector5[k] && vector5[k] <= 0f && vector5[k] < zero8[k])
					{
						array4[k] = true;
					}
				}
			}
			for (int l = 0; l < array3.Length; l++)
			{
				Vector3 b = array3[l];
				for (int m = 0; m < 3; m++)
				{
					if (vector4[m] != 0f && vector3[m] != 0f)
					{
						if (0f <= b[m] && b[m] <= zero[m] && (array2[m] || array4[m]))
						{
							ref Vector3 ptr = ref b;
							int index = m;
							ptr[index] *= zero9[m];
						}
						else if (zero2[m] <= b[m] && b[m] <= 0f && (array2[m] || array4[m]))
						{
							ref Vector3 ptr = ref b;
							int index = m;
							ptr[index] *= zero10[m];
						}
						else if (zero[m] < b[m])
						{
							b[m] = zero[m] * zero9[m] + (b[m] - zero[m]);
							if (zero7[m] < 0f)
							{
								ref Vector3 ptr = ref b;
								int index = m;
								ptr[index] *= zero11[m];
							}
						}
						else if (b[m] < zero2[m])
						{
							b[m] = zero2[m] * zero10[m] - (zero2[m] - b[m]);
							if (zero8[m] > 0f)
							{
								ref Vector3 ptr = ref b;
								int index = m;
								ptr[index] *= -zero12[m];
							}
						}
						vertices[l] = Vector3.Scale(a, b);
					}
				}
			}
			Mesh mesh = Object.Instantiate<Mesh>(currentMesh);
			mesh.vertices = vertices;
			return mesh;
		}

		private bool ShouldResize()
		{
			return this._currentSize != base.transform.lossyScale || Math.Abs(this._cachedBorderXNegative - this.BorderXNegative) > Mathf.Epsilon || Math.Abs(this._cachedBorderYNegative - this.BorderYNegative) > Mathf.Epsilon || Math.Abs(this._cachedBorderZNegative - this.BorderZNegative) > Mathf.Epsilon || Math.Abs(this._cachedBorderXPositive - this.BorderXPositive) > Mathf.Epsilon || Math.Abs(this._cachedBorderYPositive - this.BorderYPositive) > Mathf.Epsilon || Math.Abs(this._cachedBorderZPositive - this.BorderZPositive) > Mathf.Epsilon;
		}

		private void UpdateCachedValues()
		{
			this._cachedBorderXNegative = this.BorderXNegative;
			this._cachedBorderXPositive = this.BorderXPositive;
			this._cachedBorderYNegative = this.BorderYNegative;
			this._cachedBorderYPositive = this.BorderYPositive;
			this._cachedBorderZNegative = this.BorderZNegative;
			this._cachedBorderZPositive = this.BorderZPositive;
		}

		private void DrawBorderCubeGizmo(GridSliceResizer.Method scalingMethod, float borderNegative, float borderPositive, int axis)
		{
			Vector3 lossyScale = base.transform.lossyScale;
			Vector3 scale = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Bounds originalScaledBounds = this.ScaleBounds(this._boundingBox, scale);
			Vector3 size = this._boundingBox.size;
			switch (scalingMethod)
			{
			case GridSliceResizer.Method.SLICE:
				this.DrawPositiveDrawBorderForAxis(borderNegative, axis, originalScaledBounds, size);
				this.DrawNegativeBorderForAxis(borderNegative, axis, originalScaledBounds, size);
				return;
			case GridSliceResizer.Method.SLICE_WITH_ASYMMETRICAL_BORDER:
				this.DrawPositiveDrawBorderForAxis(borderPositive, axis, originalScaledBounds, size);
				this.DrawNegativeBorderForAxis(borderNegative, axis, originalScaledBounds, size);
				break;
			case GridSliceResizer.Method.SCALE:
				break;
			default:
				return;
			}
		}

		private void DrawNegativeBorderForAxis(float borderNegative, int axis, Bounds originalScaledBounds, Vector3 boundingBoxSize)
		{
			boundingBoxSize[axis] = 0f;
			Vector3 center = this._boundingBox.center;
			center[axis] = this._boundingBox.min[axis] - (originalScaledBounds.min[axis] - (-Mathf.Abs(originalScaledBounds.min[axis] - this.PivotOffset[axis]) * borderNegative + this.PivotOffset[axis]));
			if (center[axis] - this.PivotOffset[axis] > 0f)
			{
				center[axis] = this._boundingBox.min[axis];
			}
			if (this.PivotOffset[axis] < this._boundingBox.min[axis])
			{
				center[axis] = Mathf.Max(this.PivotOffset[axis] * base.transform.lossyScale[axis], center[axis]);
			}
			Gizmos.DrawWireCube(center, boundingBoxSize);
		}

		private void DrawPositiveDrawBorderForAxis(float borderNegative, int axis, Bounds originalScaledBounds, Vector3 boundingBoxSize)
		{
			boundingBoxSize[axis] = 0f;
			Vector3 center = this._boundingBox.center;
			center[axis] = this._boundingBox.max[axis] - (originalScaledBounds.max[axis] - (Mathf.Abs(originalScaledBounds.max[axis] - this.PivotOffset[axis]) * borderNegative + this.PivotOffset[axis]));
			if (center[axis] + this.PivotOffset[axis] < 0f)
			{
				center[axis] = this._boundingBox.max[axis];
			}
			if (this.PivotOffset[axis] > this._boundingBox.max[axis])
			{
				center[axis] = Mathf.Min(this.PivotOffset[axis] * base.transform.lossyScale[axis], center[axis]);
			}
			Gizmos.DrawWireCube(center, boundingBoxSize);
		}

		private Bounds ScaleBounds(Bounds originalBounds, Vector3 scale)
		{
			Vector3 vector = Vector3.Scale(originalBounds.min, scale);
			Vector3 vector2 = Vector3.Scale(originalBounds.max, scale);
			return new Bounds((vector + vector2) * 0.5f, vector2 - vector);
		}

		[Tooltip("Represents the offset from the pivot point of the mesh. This offset is used to adjust the origin of scaling operations.")]
		public Vector3 PivotOffset;

		[Tooltip("Specifies the proportion of the mesh along the positive X-axis that is protected from scaling.")]
		[Space(15f)]
		public GridSliceResizer.Method ScalingX;

		[Tooltip("Specifies the proportion of the mesh along the negative X-axis that is protected from scaling.")]
		[Range(0f, 1f)]
		public float BorderXNegative;

		[Tooltip("Specifies the proportion of the mesh along the positive X-axis that is protected from scaling.")]
		[Range(0f, 1f)]
		public float BorderXPositive;

		[Tooltip(" Defines the scaling method to be applied along the Y-axis of the mesh.")]
		[Space(15f)]
		public GridSliceResizer.Method ScalingY;

		[Tooltip("Specifies the proportion of the mesh along the negative Y-axis that is protected from scaling.")]
		[Range(0f, 1f)]
		public float BorderYNegative;

		[Tooltip("Specifies the proportion of the mesh along the positive Y-axis that is protected from scaling.")]
		[Range(0f, 1f)]
		public float BorderYPositive;

		[Tooltip("Defines the scaling method to be applied along the Z-axis of the mesh.")]
		[Space(15f)]
		public GridSliceResizer.Method ScalingZ;

		[Tooltip("Specifies the proportion of the mesh along the negative Z-axis that is protected from scaling.")]
		[Range(0f, 1f)]
		public float BorderZNegative;

		[Tooltip("Specifies the proportion of the mesh along the positive Z-axis that is protected from scaling.")]
		[Range(0f, 1f)]
		public float BorderZPositive;

		[Tooltip("Specifies which axes should allow the center part of the object to stretch.This setting is used to control the stretching behavior of the central section of the mesh allowing for selective stretching along specified axes.")]
		public GridSliceResizer.StretchCenterAxis StretchCenter;

		[Tooltip("Indicates whether the resizer should update the mesh in play mode.When set to true, the mesh will continue to be updated based on the scaling settings during runtime.This can be useful for dynamic scaling effects but may impact performance if used excessively.")]
		public bool UpdateInPlayMode = true;

		[Tooltip("The original mesh before any modifications. This mesh is used as the baseline for all scaling operations")]
		public Mesh OriginalMesh;

		private readonly Color[] _axisGizmosColors = new Color[]
		{
			new Color(1f, 0f, 0f, 0.5f),
			new Color(0f, 1f, 0f, 0.5f),
			new Color(0f, 0f, 1f, 0.5f)
		};

		private float _cachedBorderXNegative;

		private float _cachedBorderXPositive;

		private float _cachedBorderYNegative;

		private float _cachedBorderYPositive;

		private float _cachedBorderZNegative;

		private float _cachedBorderZPositive;

		private const float _minBorderSize = 0.01f;

		private MeshFilter _meshFilter;

		private Vector3 _currentSize;

		private Bounds _boundingBox;

		private Bounds _scaledBoundingBox;

		private Matrix4x4 _pivotTransform = Matrix4x4.identity;

		private Matrix4x4 _scaledInvPivotTransform = Matrix4x4.identity;

		private Mesh _currentMesh;

		private MeshCollider _meshCollider;

		public enum Method
		{
			SLICE,
			SLICE_WITH_ASYMMETRICAL_BORDER,
			SCALE
		}

		[Flags]
		public enum StretchCenterAxis
		{
			X = 1,
			Y = 2,
			Z = 4
		}
	}
}
