using System;
using System.Collections.Generic;
using Meta.XR.Util;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_m_r_u_k_anchor")]
	[Feature(Feature.Scene)]
	public class MRUKAnchor : MonoBehaviour
	{
		[Obsolete("Use 'Label' instead.")]
		public List<string> AnchorLabels
		{
			get
			{
				return Utilities.SceneLabelsEnumToList(this.Label);
			}
		}

		internal Pose InitialPose { get; set; } = Pose.identity;

		internal Pose DeltaPose
		{
			get
			{
				Quaternion rotation = base.transform.rotation * Quaternion.Inverse(this.InitialPose.rotation);
				return new Pose(base.transform.position - rotation * this.InitialPose.position, rotation);
			}
		}

		public MRUKAnchor.SceneLabels Label { get; internal set; }

		public Rect? PlaneRect { get; internal set; }

		public Bounds? VolumeBounds { get; internal set; }

		public List<Vector2> PlaneBoundary2D { get; internal set; }

		public OVRAnchor Anchor { get; internal set; } = OVRAnchor.Null;

		public MRUKRoom Room { get; internal set; }

		public MRUKAnchor ParentAnchor { get; internal set; }

		public List<MRUKAnchor> ChildAnchors { get; internal set; } = new List<MRUKAnchor>();

		[Obsolete("Use PlaneRect.HasValue instead.")]
		public bool HasPlane
		{
			get
			{
				return this.PlaneRect != null;
			}
		}

		[Obsolete("Use VolumeBounds.HasValue instead.")]
		public bool HasVolume
		{
			get
			{
				return this.VolumeBounds != null;
			}
		}

		[Obsolete("Use HasValidHandle instead.")]
		public bool IsLocal
		{
			get
			{
				return this.HasValidHandle;
			}
		}

		public bool HasValidHandle
		{
			get
			{
				return this.Anchor.Handle > 0UL;
			}
		}

		internal Mesh Mesh
		{
			get
			{
				return this.GlobalMesh;
			}
			set
			{
				this._mesh = value;
			}
		}

		public Mesh GlobalMesh
		{
			get
			{
				if (!this._mesh)
				{
					this._mesh = this.LoadGlobalMeshTriangles();
				}
				return this._mesh;
			}
			private set
			{
				this._mesh = value;
			}
		}

		public bool Raycast(Ray ray, float maxDist, out RaycastHit hitInfo, MRUKAnchor.ComponentType componentTypes = MRUKAnchor.ComponentType.All)
		{
			Vector3 origin = base.transform.InverseTransformPoint(ray.origin);
			Vector3 direction = base.transform.InverseTransformDirection(ray.direction);
			Ray localRay = new Ray(origin, direction);
			RaycastHit raycastHit = default(RaycastHit);
			RaycastHit raycastHit2 = default(RaycastHit);
			bool flag = (componentTypes & MRUKAnchor.ComponentType.Plane) != MRUKAnchor.ComponentType.None && this.RaycastPlane(localRay, maxDist, out raycastHit);
			bool flag2 = (componentTypes & MRUKAnchor.ComponentType.Volume) != MRUKAnchor.ComponentType.None && this.RaycastVolume(localRay, maxDist, out raycastHit2);
			if (flag && flag2)
			{
				if (raycastHit.distance < raycastHit2.distance)
				{
					hitInfo = raycastHit;
					return true;
				}
				hitInfo = raycastHit2;
				return true;
			}
			else
			{
				if (flag)
				{
					hitInfo = raycastHit;
					return true;
				}
				if (flag2)
				{
					hitInfo = raycastHit2;
					return true;
				}
				hitInfo = default(RaycastHit);
				return false;
			}
		}

		public bool IsPositionInBoundary(Vector2 position)
		{
			return this.PlaneBoundary2D != null && this.PlaneBoundary2D.Count != 0 && Utilities.IsPositionInPolygon(position, this.PlaneBoundary2D);
		}

		public void AddChildReference(MRUKAnchor childObj)
		{
			if (childObj != null)
			{
				this.ChildAnchors.Add(childObj);
			}
		}

		public void ClearChildReferences()
		{
			this.ChildAnchors.Clear();
		}

		public float GetDistanceToSurface(Vector3 position, MRUKAnchor.ComponentType componentTypes = MRUKAnchor.ComponentType.All)
		{
			Vector3 vector;
			Vector3 vector2;
			return this.GetClosestSurfacePosition(position, out vector, out vector2, componentTypes);
		}

		public float GetClosestSurfacePosition(Vector3 testPosition, out Vector3 closestPosition, MRUKAnchor.ComponentType componentTypes = MRUKAnchor.ComponentType.All)
		{
			Vector3 vector;
			return this.GetClosestSurfacePosition(testPosition, out closestPosition, out vector, componentTypes);
		}

		public float GetClosestSurfacePosition(Vector3 testPosition, out Vector3 closestPosition, out Vector3 normal, MRUKAnchor.ComponentType componentTypes = MRUKAnchor.ComponentType.All)
		{
			float result = float.PositiveInfinity;
			closestPosition = Vector3.zero;
			normal = Vector3.zero;
			if ((componentTypes & MRUKAnchor.ComponentType.Volume) != MRUKAnchor.ComponentType.None && this.VolumeBounds != null)
			{
				Bounds value = this.VolumeBounds.Value;
				Vector3 vector = base.transform.InverseTransformPoint(testPosition);
				if (value.Contains(vector))
				{
					vector -= value.center;
					float num = float.MaxValue;
					int num2 = -1;
					for (int i = 0; i < 3; i++)
					{
						float num3 = value.extents[i] - Mathf.Abs(vector[i]);
						if (num3 < num)
						{
							num = num3;
							num2 = i;
						}
					}
					float num4 = Mathf.Sign(vector[num2]);
					ref Vector3 ptr = ref vector;
					int index = num2;
					ptr[index] += num * num4;
					result = -num;
					Vector3 vector2 = default(Vector3);
					index = num2;
					vector2[index] = num4;
					Vector3 direction = vector2;
					normal = base.transform.TransformDirection(direction);
					closestPosition = base.transform.TransformPoint(vector + value.center);
				}
				else
				{
					closestPosition = value.ClosestPoint(vector);
					Vector3 vector3 = vector - closestPosition;
					if (Mathf.Abs(vector3.x) > Mathf.Abs(vector3.y) && Mathf.Abs(vector3.x) > Mathf.Abs(vector3.z))
					{
						normal = new Vector3(Mathf.Sign(vector3.x), 0f, 0f);
					}
					else if (Mathf.Abs(vector3.y) > Mathf.Abs(vector3.z))
					{
						normal = new Vector3(0f, Mathf.Sign(vector3.y), 0f);
					}
					else
					{
						normal = new Vector3(0f, 0f, Mathf.Sign(vector3.z));
					}
					closestPosition = base.transform.TransformPoint(closestPosition);
					normal = base.transform.TransformDirection(normal);
					result = Vector3.Distance(closestPosition, testPosition);
				}
			}
			else if ((componentTypes & MRUKAnchor.ComponentType.Plane) != MRUKAnchor.ComponentType.None && this.PlaneRect != null)
			{
				Rect value2 = this.PlaneRect.Value;
				Vector3 vector4 = base.transform.InverseTransformPoint(testPosition);
				vector4.z = 0f;
				if (vector4.x > value2.max.x)
				{
					vector4.x = value2.max.x;
				}
				else if (vector4.x < value2.min.x)
				{
					vector4.x = value2.min.x;
				}
				if (vector4.y > value2.max.y)
				{
					vector4.y = value2.max.y;
				}
				else if (vector4.y < value2.min.y)
				{
					vector4.y = value2.min.y;
				}
				closestPosition = base.transform.TransformPoint(vector4);
				result = Vector3.Distance(closestPosition, testPosition);
				normal = base.transform.forward;
			}
			return result;
		}

		public Vector3 GetAnchorCenter()
		{
			if (this.VolumeBounds != null)
			{
				return base.transform.TransformPoint(this.VolumeBounds.Value.center);
			}
			return base.transform.position;
		}

		[Obsolete("Use PlaneRect and VolumeBounds properties instead")]
		public Vector3 GetAnchorSize()
		{
			Vector3 result = Vector3.one;
			if (this.HasPlane)
			{
				result = new Vector3(this.PlaneRect.Value.size.x, this.PlaneRect.Value.size.y, 1f);
			}
			if (this.HasVolume)
			{
				result = this.VolumeBounds.Value.size;
			}
			return result;
		}

		private bool RaycastPlane(Ray localRay, float maxDist, out RaycastHit hitInfo)
		{
			hitInfo = default(RaycastHit);
			if (this.PlaneRect == null)
			{
				return false;
			}
			if (localRay.direction.z >= 0f)
			{
				return false;
			}
			Plane plane = new Plane(Vector3.forward, 0f);
			float num;
			if (plane.Raycast(localRay, out num) && num < maxDist)
			{
				Vector3 point = localRay.GetPoint(num);
				if (this.IsPositionInBoundary(new Vector2(point.x, point.y)))
				{
					hitInfo.point = base.transform.TransformPoint(point);
					hitInfo.normal = base.transform.forward;
					hitInfo.distance = num;
					return true;
				}
			}
			return false;
		}

		private bool RaycastVolume(Ray localRay, float maxDist, out RaycastHit hitInfo)
		{
			hitInfo = default(RaycastHit);
			if (this.VolumeBounds == null)
			{
				return false;
			}
			int index = 0;
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			Bounds value = this.VolumeBounds.Value;
			for (int i = 0; i < 3; i++)
			{
				if (Mathf.Abs(localRay.direction[i]) > Mathf.Epsilon)
				{
					float num3 = (value.min[i] - localRay.origin[i]) / localRay.direction[i];
					float num4 = (value.max[i] - localRay.origin[i]) / localRay.direction[i];
					if (num3 > num4)
					{
						float num5 = num4;
						float num6 = num3;
						num3 = num5;
						num4 = num6;
					}
					if (num3 > num2)
					{
						num2 = num3;
						index = i;
					}
					if (num4 < num)
					{
						num = num4;
					}
				}
				else if (localRay.origin[i] < value.min[i] || localRay.origin[i] > value.max[i])
				{
					num2 = float.PositiveInfinity;
					break;
				}
			}
			if (num2 >= 0f && num2 <= num && num2 < maxDist)
			{
				Vector3 point = localRay.GetPoint(num2);
				Vector3 zero = Vector3.zero;
				zero[index] = (float)((localRay.direction[index] > 0f) ? -1 : 1);
				hitInfo.point = base.transform.TransformPoint(point);
				hitInfo.normal = base.transform.TransformDirection(zero);
				hitInfo.distance = num2;
				return true;
			}
			return false;
		}

		public Vector3[] GetBoundsFaceCenters()
		{
			if (this.VolumeBounds != null)
			{
				Vector3[] array = new Vector3[6];
				Vector3 size = this.VolumeBounds.Value.size;
				Vector3 a = base.transform.position - base.transform.forward * size.z * 0.5f;
				array[0] = base.transform.position;
				array[1] = a - base.transform.forward * size.z * 0.5f;
				array[2] = a + base.transform.right * size.x * 0.5f;
				array[3] = a - base.transform.right * size.x * 0.5f;
				array[4] = a + base.transform.up * size.y * 0.5f;
				array[5] = a - base.transform.up * size.y * 0.5f;
				return array;
			}
			if (this.PlaneRect != null)
			{
				return new Vector3[]
				{
					base.transform.position
				};
			}
			return null;
		}

		public bool IsPositionInVolume(Vector3 worldPosition, bool testVerticalBounds, float distanceBuffer = 0f)
		{
			if (this.VolumeBounds == null)
			{
				return false;
			}
			Vector3 vector = base.transform.InverseTransformPoint(worldPosition);
			Bounds value = this.VolumeBounds.Value;
			value.Expand(distanceBuffer);
			if (testVerticalBounds)
			{
				return value.Contains(vector);
			}
			return vector.x >= value.min.x && vector.x <= value.max.x && vector.z >= value.min.z && vector.z <= value.max.z;
		}

		public Mesh LoadGlobalMeshTriangles()
		{
			if (!this.HasAnyLabel(MRUKAnchor.SceneLabels.GLOBAL_MESH))
			{
				return null;
			}
			return this.LoadObjectMeshTriangles() ?? new Mesh();
		}

		internal Mesh LoadObjectMeshTriangles()
		{
			OVRTriangleMesh ovrtriangleMesh;
			this.Anchor.TryGetComponent<OVRTriangleMesh>(out ovrtriangleMesh);
			int num;
			int num2;
			if (!ovrtriangleMesh.TryGetCounts(out num, out num2))
			{
				return null;
			}
			Mesh mesh = new Mesh
			{
				indexFormat = ((num > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16)
			};
			Mesh result;
			using (NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(num, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				using (NativeArray<int> indices = new NativeArray<int>(num2 * 3, Allocator.Temp, NativeArrayOptions.ClearMemory))
				{
					if (!ovrtriangleMesh.TryGetMesh(nativeArray, indices))
					{
						result = mesh;
					}
					else
					{
						mesh.SetVertices<Vector3>(nativeArray);
						mesh.SetIndices<int>(indices, MeshTopology.Triangles, 0, true, 0);
						result = mesh;
					}
				}
			}
			return result;
		}

		[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
		public bool HasLabel(string label)
		{
			return this.HasAnyLabel(Utilities.StringLabelToEnum(label));
		}

		[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
		public bool HasAnyLabel(List<string> labels)
		{
			return this.HasAnyLabel(Utilities.StringLabelsToEnum(labels));
		}

		public bool HasAnyLabel(MRUKAnchor.SceneLabels labelFlags)
		{
			return (this.Label & labelFlags) > (MRUKAnchor.SceneLabels)0;
		}

		[Obsolete("Use 'Label' instead.")]
		public MRUKAnchor.SceneLabels GetLabelsAsEnum()
		{
			return this.Label;
		}

		private Mesh _mesh;

		[Flags]
		public enum SceneLabels
		{
			FLOOR = 1,
			CEILING = 2,
			WALL_FACE = 4,
			TABLE = 8,
			COUCH = 16,
			DOOR_FRAME = 32,
			WINDOW_FRAME = 64,
			OTHER = 128,
			STORAGE = 256,
			BED = 512,
			SCREEN = 1024,
			LAMP = 2048,
			PLANT = 4096,
			WALL_ART = 8192,
			GLOBAL_MESH = 16384,
			INVISIBLE_WALL_FACE = 32768
		}

		[Flags]
		public enum ComponentType
		{
			None = 0,
			Plane = 1,
			Volume = 2,
			All = 3
		}
	}
}
