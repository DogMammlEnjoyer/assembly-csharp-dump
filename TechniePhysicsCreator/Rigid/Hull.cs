using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator.Rigid
{
	[Serializable]
	public class Hull : IHull
	{
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int NumSelectedTriangles
		{
			get
			{
				return this.selectedFaces.Count;
			}
		}

		public Vector3[] CachedTriangleVertices
		{
			get
			{
				return this.cachedTriangleVertices.ToArray();
			}
			set
			{
				this.cachedTriangleVertices.Clear();
				this.cachedTriangleVertices.AddRange(value);
			}
		}

		public void Destroy()
		{
		}

		public bool ContainsAutoMesh(Mesh m)
		{
			if (this.autoMeshes != null)
			{
				Mesh[] array = this.autoMeshes;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == m)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsTriangleSelected(int triIndex, Renderer renderer, Mesh targetMesh)
		{
			return this.selectedFaces.Contains(triIndex);
		}

		public int[] GetSelectedFaces()
		{
			return this.selectedFaces.ToArray();
		}

		public void ClearSelectedFaces()
		{
			this.selectedFaces.Clear();
			this.cachedTriangleVertices.Clear();
		}

		public void AddToSelection(int newTriangleIndex, Mesh srcMesh)
		{
			if (this.selectedFaces.Contains(newTriangleIndex))
			{
				return;
			}
			this.selectedFaces.Add(newTriangleIndex);
			Utils.UpdateCachedVertices(this, srcMesh);
		}

		public void RemoveFromSelection(int existingTriangleIndex, Mesh srcMesh)
		{
			this.selectedFaces.Remove(existingTriangleIndex);
			Utils.UpdateCachedVertices(this, srcMesh);
		}

		public void SetSelectedFaces(List<int> newSelectedFaceIndices, Mesh srcMesh)
		{
			this.selectedFaces.Clear();
			this.selectedFaces.AddRange(newSelectedFaceIndices);
			Utils.UpdateCachedVertices(this, srcMesh);
		}

		public int GetSelectedFaceIndex(int index)
		{
			return this.selectedFaces[index];
		}

		public void FindConvexHull(Vector3[] meshVertices, int[] meshIndices, out Vector3[] hullVertices, out int[] hullIndices, bool showErrorInLog)
		{
			QHullUtil.FindConvexHull(this.name, this.selectedFaces.ToArray(), meshVertices, meshIndices, out hullVertices, out hullIndices, false);
		}

		public List<Triangle> FindSelectedTriangles(Vector3[] meshVertices, int[] meshIndices)
		{
			List<Triangle> list = new List<Triangle>();
			foreach (int num in this.selectedFaces)
			{
				int num2 = meshIndices[num * 3];
				int num3 = meshIndices[num * 3 + 1];
				int num4 = meshIndices[num * 3 + 2];
				Vector3 p = meshVertices[num2];
				Vector3 p2 = meshVertices[num3];
				Vector3 p3 = meshVertices[num4];
				Triangle item = new Triangle(p, p2, p3);
				list.Add(item);
			}
			return list;
		}

		public void FindTriangles(Vector3[] meshVertices, int[] meshIndices, out Vector3[] hullVertices, out int[] hullIndices)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (int num in this.selectedFaces)
			{
				int num2 = meshIndices[num * 3];
				int num3 = meshIndices[num * 3 + 1];
				int num4 = meshIndices[num * 3 + 2];
				Vector3 item = meshVertices[num2];
				Vector3 item2 = meshVertices[num3];
				Vector3 item3 = meshVertices[num4];
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
			}
			hullVertices = list.ToArray();
			hullIndices = new int[hullVertices.Length];
			for (int i = 0; i < hullIndices.Length; i++)
			{
				hullIndices[i] = i;
			}
		}

		public Vector3[] GetSelectedVertices(Vector3[] meshVertices, int[] meshIndices)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (int num in this.selectedFaces)
			{
				int num2 = meshIndices[num * 3];
				int num3 = meshIndices[num * 3 + 1];
				int num4 = meshIndices[num * 3 + 2];
				dictionary[num2] = num2;
				dictionary[num3] = num3;
				dictionary[num4] = num4;
			}
			List<Vector3> list = new List<Vector3>();
			foreach (int num5 in dictionary.Keys)
			{
				list.Add(meshVertices[num5]);
			}
			return list.ToArray();
		}

		public void GenerateCollisionMesh(Vector3[] meshVertices, int[] meshIndices, Mesh[] autoHulls, float faceThickness)
		{
			this.hasColliderError = false;
			this.noInputError = false;
			if (this.selectedFaces.Count == 0)
			{
				this.noInputError = true;
			}
			if (this.type == HullType.Box)
			{
				if (this.selectedFaces.Count > 0)
				{
					if (!this.isChildCollider)
					{
						Vector3 vector2;
						Vector3 vector = vector2 = meshVertices[meshIndices[this.selectedFaces[0] * 3]];
						for (int i = 0; i < this.selectedFaces.Count; i++)
						{
							int num = this.selectedFaces[i];
							Vector3 point = meshVertices[meshIndices[num * 3]];
							Vector3 point2 = meshVertices[meshIndices[num * 3 + 1]];
							Vector3 point3 = meshVertices[meshIndices[num * 3 + 2]];
							Utils.Inflate(point, ref vector, ref vector2);
							Utils.Inflate(point2, ref vector, ref vector2);
							Utils.Inflate(point3, ref vector, ref vector2);
						}
						this.collisionBox.collisionBox.center = (vector + vector2) * 0.5f;
						this.collisionBox.collisionBox.size = vector2 - vector;
						this.collisionBox.boxRotation = Quaternion.identity;
						return;
					}
					if (this.boxFitMethod == BoxFitMethod.MinimumVolume)
					{
						RotatedBoxFitter rotatedBoxFitter = new RotatedBoxFitter();
						this.collisionBox = rotatedBoxFitter.Fit(this, meshVertices, meshIndices);
						return;
					}
					if (this.boxFitMethod == BoxFitMethod.AlignFaces)
					{
						new FaceAlignmentBoxFitter().Fit(this, meshVertices, meshIndices);
						return;
					}
					if (this.boxFitMethod == BoxFitMethod.AxisAligned)
					{
						new AxisAlignedBoxFitter().Fit(this, meshVertices, meshIndices);
						return;
					}
				}
			}
			else if (this.type == HullType.Capsule)
			{
				if (this.isChildCollider)
				{
					RotatedCapsuleFitter rotatedCapsuleFitter = new RotatedCapsuleFitter();
					this.collisionCapsule = rotatedCapsuleFitter.Fit(this, meshVertices, meshIndices);
					return;
				}
				AlignedCapsuleFitter alignedCapsuleFitter = new AlignedCapsuleFitter();
				this.collisionCapsule = alignedCapsuleFitter.Fit(this, meshVertices, meshIndices);
				return;
			}
			else
			{
				if (this.type == HullType.Sphere)
				{
					SphereFitter sphereFitter = new SphereFitter();
					this.collisionSphere = sphereFitter.Fit(this, meshVertices, meshIndices);
					return;
				}
				if (this.type == HullType.ConvexHull)
				{
					if (this.collisionMesh == null)
					{
						this.collisionMesh = new Mesh();
					}
					this.collisionMesh.name = this.name;
					this.collisionMesh.triangles = new int[0];
					this.collisionMesh.vertices = new Vector3[0];
					this.GenerateConvexHull(this, meshVertices, meshIndices, this.collisionMesh);
					return;
				}
				if (this.type == HullType.Face)
				{
					if (this.faceCollisionMesh == null)
					{
						this.faceCollisionMesh = new Mesh();
					}
					this.faceCollisionMesh.name = this.name;
					this.faceCollisionMesh.triangles = new int[0];
					this.faceCollisionMesh.vertices = new Vector3[0];
					this.GenerateFace(this, meshVertices, meshIndices, faceThickness);
					return;
				}
				if (this.type == HullType.FaceAsBox)
				{
					if (this.selectedFaces.Count > 0)
					{
						if (this.isChildCollider)
						{
							Vector3[] vertices = this.ExtractUniqueVertices(this, meshVertices, meshIndices);
							Vector3 vector3 = Hull.CalcPrimaryAxis(this, meshVertices, meshIndices, !this.isChildCollider);
							Vector3 rhs = (Vector3.Dot(vector3, Vector3.up) > 0.8f) ? Vector3.right : Vector3.up;
							Vector3 rhs2 = Vector3.Cross(vector3, rhs);
							Vector3 primaryUp = Vector3.Cross(vector3, rhs2);
							float num2 = 0f;
							float num3 = float.MaxValue;
							Vector3 vector4 = Vector3.zero;
							Vector3 vector5 = Vector3.zero;
							Quaternion rotation = Quaternion.identity;
							float num4 = 5f;
							float num5 = 0.05f;
							for (float num6 = 0f; num6 <= 360f; num6 += num4)
							{
								Vector3 vector6;
								Vector3 vector7;
								Quaternion quaternion;
								float num7 = Hull.CalcRequiredArea(num6, vector3, primaryUp, vertices, out vector6, out vector7, out quaternion);
								if (num7 < num3)
								{
									num2 = num6;
									num3 = num7;
									vector4 = vector6;
									vector5 = vector7;
									rotation = quaternion;
								}
							}
							float num8 = num2 - num4;
							float num9 = num2 + num4;
							for (float num10 = num8; num10 <= num9; num10 += num5)
							{
								Vector3 vector8;
								Vector3 vector9;
								Quaternion quaternion2;
								float num11 = Hull.CalcRequiredArea(num10, vector3, primaryUp, vertices, out vector8, out vector9, out quaternion2);
								if (num11 < num3)
								{
									num3 = num11;
									vector4 = vector8;
									vector5 = vector9;
									rotation = quaternion2;
								}
							}
							Vector3 point4 = (vector4 + vector5) / 2f;
							Vector3 vector10 = vector5 - vector4;
							float num12 = vector10.z - faceThickness;
							point4.z += num12 * 0.5f;
							vector10.z += num12;
							this.faceBoxCenter = rotation * point4;
							this.faceBoxSize = vector10;
							this.faceAsBoxRotation = rotation;
							return;
						}
						Vector3[] array = this.ExtractUniqueVertices(this, meshVertices, meshIndices);
						Vector3 vector11 = Hull.CalcPrimaryAxis(this, meshVertices, meshIndices, !this.isChildCollider);
						Vector3 vector13;
						Vector3 vector12 = vector13 = array[0];
						Vector3[] array2 = array;
						for (int j = 0; j < array2.Length; j++)
						{
							Utils.Inflate(array2[j], ref vector12, ref vector13);
						}
						Vector3 vector14 = (vector12 + vector13) / 2f;
						Vector3 vector15 = vector13 - vector12;
						if (Mathf.Abs(vector11.x) > 0f)
						{
							float num13 = (vector11.x > 0f) ? 1f : -1f;
							float num14 = vector15.x - faceThickness;
							vector14.x += num14 * 0.5f * num13;
							vector15.x += num14;
						}
						else if (Mathf.Abs(vector11.y) > 0f)
						{
							float num15 = (vector11.y > 0f) ? 1f : -1f;
							float num16 = vector15.y - faceThickness;
							vector14.y += num16 * 0.5f * num15;
							vector15.y += num16;
						}
						else
						{
							float num17 = (vector11.z > 0f) ? 1f : -1f;
							float num18 = vector15.z - faceThickness;
							vector14.z += num18 * 0.5f * num17;
							vector15.z += num18;
						}
						this.faceBoxCenter = vector14;
						this.faceBoxSize = vector15;
						this.faceAsBoxRotation = Quaternion.identity;
						return;
					}
				}
				else if (this.type == HullType.Auto)
				{
					if (this.collisionMesh == null)
					{
						this.collisionMesh = new Mesh();
					}
					this.collisionMesh.name = string.Format("{0} bounds", this.name);
					this.collisionMesh.triangles = new int[0];
					this.collisionMesh.vertices = new Vector3[0];
					this.GenerateConvexHull(this, meshVertices, meshIndices, this.collisionMesh);
					List<Mesh> list = new List<Mesh>();
					if (this.selectedFaces.Count == meshIndices.Length / 3)
					{
						list.AddRange(autoHulls);
					}
					else
					{
						foreach (Mesh inputMesh in autoHulls)
						{
							Mesh mesh = Utils.Clip(this.collisionMesh, inputMesh);
							if (mesh != null)
							{
								list.Add(mesh);
							}
						}
					}
					for (int k = 0; k < list.Count; k++)
					{
						list[k].name = string.Format("{0}.{1}", this.name, k + 1);
					}
					List<Mesh> list2 = new List<Mesh>();
					if (this.autoMeshes != null)
					{
						list2.AddRange(this.autoMeshes);
					}
					while (list2.Count > list.Count)
					{
						list2.RemoveAt(list2.Count - 1);
					}
					while (list2.Count < list.Count)
					{
						list2.Add(new Mesh());
					}
					for (int l = 0; l < list.Count; l++)
					{
						list2[l].Clear();
						list2[l].name = list[l].name;
						list2[l].vertices = list[l].vertices;
						list2[l].triangles = list[l].triangles;
					}
					this.autoMeshes = list2.ToArray();
				}
			}
		}

		private Vector3[] ExtractUniqueVertices(Hull hull, Vector3[] meshVertices, int[] meshIndices)
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < hull.selectedFaces.Count; i++)
			{
				int num = hull.selectedFaces[i];
				Vector3 vector = meshVertices[meshIndices[num * 3]];
				Vector3 vector2 = meshVertices[meshIndices[num * 3 + 1]];
				Vector3 vector3 = meshVertices[meshIndices[num * 3 + 2]];
				if (!Hull.Contains(list, vector))
				{
					list.Add(vector);
				}
				if (!Hull.Contains(list, vector2))
				{
					list.Add(vector2);
				}
				if (!Hull.Contains(list, vector3))
				{
					list.Add(vector3);
				}
			}
			return list.ToArray();
		}

		private static bool Contains(List<Vector3> list, Vector3 p)
		{
			using (List<Vector3>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Vector3.Distance(enumerator.Current, p) < 0.0001f)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void GenerateConvexHull(Hull hull, Vector3[] meshVertices, int[] meshIndices, Mesh destMesh)
		{
			Vector3[] vertices;
			int[] array;
			QHullUtil.FindConvexHull(hull.name, hull.selectedFaces.ToArray(), meshVertices, meshIndices, out vertices, out array, true);
			hull.numColliderFaces = array.Length / 3;
			Console.output.Log(string.Concat(new string[]
			{
				"Calculated collider for '",
				hull.name,
				"' has ",
				hull.numColliderFaces.ToString(),
				" faces"
			}));
			if (hull.numColliderFaces >= 256)
			{
				hull.hasColliderError = true;
				hull.enableInflation = true;
			}
			hull.collisionMesh.vertices = vertices;
			hull.collisionMesh.triangles = array;
			hull.collisionMesh.RecalculateBounds();
			hull.faceCollisionMesh = null;
		}

		private void GenerateFace(Hull hull, Vector3[] meshVertices, int[] meshIndices, float faceThickness)
		{
			int count = hull.selectedFaces.Count;
			Vector3[] array = new Vector3[count * 3 * 2];
			for (int i = 0; i < hull.selectedFaces.Count; i++)
			{
				int num = hull.selectedFaces[i];
				Vector3 vector = meshVertices[meshIndices[num * 3]];
				Vector3 vector2 = meshVertices[meshIndices[num * 3 + 1]];
				Vector3 vector3 = meshVertices[meshIndices[num * 3 + 2]];
				Vector3 normalized = (vector2 - vector).normalized;
				Vector3 a = Vector3.Cross((vector3 - vector).normalized, normalized);
				int num2 = i * 3 * 2;
				array[num2] = vector;
				array[num2 + 1] = vector2;
				array[num2 + 2] = vector3;
				array[num2 + 3] = vector + a * faceThickness;
				array[num2 + 4] = vector2 + a * faceThickness;
				array[num2 + 5] = vector3 + a * faceThickness;
			}
			int[] array2 = new int[count * 3 * 2];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = j;
			}
			hull.faceCollisionMesh.vertices = array;
			hull.faceCollisionMesh.triangles = array2;
			hull.faceCollisionMesh.RecalculateBounds();
			hull.collisionMesh = null;
		}

		private static float CalcRequiredArea(float angleDeg, Vector3 primaryAxis, Vector3 primaryUp, Vector3[] vertices, out Vector3 min, out Vector3 max, out Quaternion outBasis)
		{
			if (vertices.Length == 0)
			{
				min = Vector3.zero;
				max = Vector3.zero;
				outBasis = Quaternion.identity;
				return 0f;
			}
			Vector3 upwards = Quaternion.AngleAxis(angleDeg, primaryAxis) * primaryUp;
			Quaternion quaternion = Quaternion.LookRotation(primaryAxis, upwards);
			Quaternion rotation = Quaternion.Inverse(quaternion);
			Vector3 vector = rotation * vertices[0];
			min = vector;
			max = vector;
			foreach (Vector3 point in vertices)
			{
				Utils.Inflate(rotation * point, ref min, ref max);
			}
			outBasis = quaternion;
			Vector3 vector2 = max - min;
			return vector2.x * vector2.y;
		}

		private static Vector3 CalcPrimaryAxis(Hull hull, Vector3[] meshVertices, int[] meshIndices, bool snapToAxies)
		{
			int num = 0;
			Vector3 a = Vector3.zero;
			for (int i = 0; i < hull.selectedFaces.Count; i++)
			{
				int num2 = hull.selectedFaces[i];
				Vector3 b = meshVertices[meshIndices[num2 * 3]];
				Vector3 a2 = meshVertices[meshIndices[num2 * 3 + 1]];
				Vector3 a3 = meshVertices[meshIndices[num2 * 3 + 2]];
				Vector3 normalized = (a2 - b).normalized;
				Vector3 normalized2 = (a3 - b).normalized;
				Vector3 b2 = Vector3.Cross(normalized, normalized2);
				a += b2;
				num++;
			}
			Vector3 vector = a / (float)num;
			if (vector.magnitude < 0.0001f)
			{
				return Vector3.up;
			}
			if (!snapToAxies)
			{
				return vector.normalized;
			}
			float num3 = Mathf.Abs(vector.x);
			float num4 = Mathf.Abs(vector.y);
			float num5 = Mathf.Abs(vector.z);
			if (num3 > num4 && num3 > num5)
			{
				return new Vector3(((double)vector.x > 0.0) ? 1f : -1f, 0f, 0f);
			}
			if (num4 > num5)
			{
				return new Vector3(0f, ((double)vector.y > 0.0) ? 1f : -1f, 0f);
			}
			return new Vector3(0f, 0f, ((double)vector.z > 0.0) ? 1f : -1f);
		}

		public string name = "<unnamed hull>";

		public bool isVisible = true;

		public HullType type = HullType.ConvexHull;

		public Color colour = Color.white;

		public PhysicsMaterial material;

		public bool enableInflation;

		public float inflationAmount = 0.01f;

		public BoxFitMethod boxFitMethod = BoxFitMethod.MinimumVolume;

		public bool isTrigger;

		public bool isChildCollider;

		[SerializeField]
		private List<int> selectedFaces = new List<int>();

		public List<Vector3> cachedTriangleVertices = new List<Vector3>();

		public Mesh collisionMesh;

		public BoxDef collisionBox;

		public Sphere collisionSphere;

		public Mesh faceCollisionMesh;

		public Vector3 faceBoxCenter;

		public Vector3 faceBoxSize;

		public Quaternion faceAsBoxRotation;

		public CapsuleDef collisionCapsule;

		public Mesh[] autoMeshes = new Mesh[0];

		public bool hasColliderError;

		public int numColliderFaces;

		public bool noInputError;
	}
}
