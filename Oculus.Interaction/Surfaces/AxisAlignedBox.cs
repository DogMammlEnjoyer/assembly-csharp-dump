using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class AxisAlignedBox : MonoBehaviour, ISurface
	{
		public Vector3 Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		public Bounds Bounds
		{
			get
			{
				return new Bounds(base.transform.position, this._size);
			}
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			hit = default(SurfaceHit);
			Vector3 vector = Vector3.Min(Vector3.Max(point, this.Bounds.min), this.Bounds.max);
			AxisAlignedBox.BoxSurface value = this.FindClosestBoxSide(point);
			hit.Normal = this.ClosestSurfaceNormal(point, new AxisAlignedBox.BoxSurface?(value));
			if (!this.IsWithinVolume(point))
			{
				hit.Point = vector;
				hit.Distance = (point - vector).magnitude;
				return maxDistance <= 0f || hit.Distance <= maxDistance;
			}
			switch (value)
			{
			case AxisAlignedBox.BoxSurface.XMin:
				vector.x = this.Bounds.min.x;
				break;
			case AxisAlignedBox.BoxSurface.YMin:
				vector.y = this.Bounds.min.y;
				break;
			case AxisAlignedBox.BoxSurface.ZMin:
				vector.z = this.Bounds.min.z;
				break;
			case AxisAlignedBox.BoxSurface.XMax:
				vector.x = this.Bounds.max.x;
				break;
			case AxisAlignedBox.BoxSurface.YMax:
				vector.y = this.Bounds.max.y;
				break;
			case AxisAlignedBox.BoxSurface.ZMax:
				vector.z = this.Bounds.max.z;
				break;
			}
			hit.Point = vector;
			hit.Distance = Vector3.Distance(hit.Point, point);
			return maxDistance <= 0f || hit.Distance <= maxDistance;
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			hit = default(SurfaceHit);
			float num = 1f;
			Ray ray2 = ray;
			float x = num / ray2.direction.x;
			float num2 = 1f;
			ray2 = ray;
			float y = num2 / ray2.direction.y;
			float num3 = 1f;
			ray2 = ray;
			Vector3 b = new Vector3(x, y, num3 / ray2.direction.z);
			Vector3 min = this.Bounds.min;
			ray2 = ray;
			Vector3 vector = Vector3.Scale(min - ray2.origin, b);
			Vector3 max = this.Bounds.max;
			ray2 = ray;
			Vector3 vector2 = Vector3.Scale(max - ray2.origin, b);
			float num4 = Mathf.Max(Mathf.Max(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector.y, vector2.y)), Mathf.Min(vector.z, vector2.z));
			float num5 = Mathf.Min(Mathf.Min(Mathf.Max(vector.x, vector2.x), Mathf.Max(vector.y, vector2.y)), Mathf.Max(vector.z, vector2.z));
			if (num5 < 0f)
			{
				hit.Distance = num5;
				return false;
			}
			if (num4 > num5)
			{
				hit.Distance = num5;
				return false;
			}
			hit.Distance = num4;
			if (maxDistance > 0f && hit.Distance > maxDistance)
			{
				return false;
			}
			if (Mathf.Sign(num4) != Mathf.Sign(num5))
			{
				hit.Distance = Mathf.Max(num5, num4);
			}
			ray2 = ray;
			Vector3 origin = ray2.origin;
			ray2 = ray;
			hit.Point = origin + ray2.direction * hit.Distance;
			hit.Normal = this.ClosestSurfaceNormal(hit.Point, null);
			return true;
		}

		protected void Start()
		{
			if (base.GetComponent<MeshFilter>())
			{
				this._size = Vector3.Scale(base.transform.localScale, base.GetComponent<MeshFilter>().mesh.bounds.size);
			}
			if (this._size.magnitude == 0f)
			{
				this._size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			this.Size = this._size;
		}

		private bool IsWithinVolume(Vector3 point)
		{
			return this.Bounds.Contains(point);
		}

		private AxisAlignedBox.BoxSurface FindClosestBoxSide(Vector3 point)
		{
			Vector3 vector = base.transform.position - point;
			Vector3 extents = this.Bounds.extents;
			this._distances[AxisAlignedBox.BoxSurface.XMin] = extents.x - vector.x;
			this._distances[AxisAlignedBox.BoxSurface.YMin] = extents.y - vector.y;
			this._distances[AxisAlignedBox.BoxSurface.ZMin] = extents.z - vector.z;
			this._distances[AxisAlignedBox.BoxSurface.XMax] = extents.x + vector.x;
			this._distances[AxisAlignedBox.BoxSurface.YMax] = extents.y + vector.y;
			this._distances[AxisAlignedBox.BoxSurface.ZMax] = extents.z + vector.z;
			AxisAlignedBox.BoxSurface boxSurface = AxisAlignedBox.BoxSurface.XMin;
			foreach (AxisAlignedBox.BoxSurface boxSurface2 in this._distances.Keys)
			{
				if (this._distances[boxSurface2] < this._distances[boxSurface])
				{
					boxSurface = boxSurface2;
				}
			}
			return boxSurface;
		}

		private Vector3 ClosestSurfaceNormal(Vector3 point, AxisAlignedBox.BoxSurface? side = null)
		{
			Vector3 result;
			switch (side ?? this.FindClosestBoxSide(point))
			{
			case AxisAlignedBox.BoxSurface.XMin:
				result = new Vector3(-1f, 0f, 0f);
				break;
			case AxisAlignedBox.BoxSurface.YMin:
				result = new Vector3(0f, -1f, 0f);
				break;
			case AxisAlignedBox.BoxSurface.ZMin:
				result = new Vector3(0f, 0f, -1f);
				break;
			case AxisAlignedBox.BoxSurface.XMax:
				result = new Vector3(1f, 0f, 0f);
				break;
			case AxisAlignedBox.BoxSurface.YMax:
				result = new Vector3(0f, 1f, 0f);
				break;
			case AxisAlignedBox.BoxSurface.ZMax:
				result = new Vector3(0f, 0f, 1f);
				break;
			default:
				throw new NotImplementedException();
			}
			return result;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[SerializeField]
		[Tooltip("Size of the axis-aligned box, default to mesh size")]
		private Vector3 _size = new Vector3(0f, 0f, 0f);

		private readonly Dictionary<AxisAlignedBox.BoxSurface, float> _distances = new Dictionary<AxisAlignedBox.BoxSurface, float>
		{
			{
				AxisAlignedBox.BoxSurface.XMin,
				0f
			},
			{
				AxisAlignedBox.BoxSurface.YMin,
				0f
			},
			{
				AxisAlignedBox.BoxSurface.ZMin,
				0f
			},
			{
				AxisAlignedBox.BoxSurface.XMax,
				0f
			},
			{
				AxisAlignedBox.BoxSurface.YMax,
				0f
			},
			{
				AxisAlignedBox.BoxSurface.ZMax,
				0f
			}
		};

		private enum BoxSurface
		{
			XMin,
			YMin,
			ZMin,
			XMax,
			YMax,
			ZMax
		}
	}
}
