using System;
using System.Collections.Generic;

namespace g3
{
	public static class MeshTransforms
	{
		public static void Translate(IDeformableMesh mesh, Vector3d v)
		{
			MeshTransforms.Translate(mesh, v.x, v.y, v.z);
		}

		public static void Translate(IDeformableMesh mesh, double tx, double ty, double tz)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					vertex.x += tx;
					vertex.y += ty;
					vertex.z += tz;
					mesh.SetVertex(i, vertex);
				}
			}
		}

		public static Vector3d Rotate(Vector3d pos, Vector3d origin, Quaternionf rotation)
		{
			Vector3d vector3d = pos - origin;
			vector3d = rotation * (Vector3f)vector3d;
			return vector3d + origin;
		}

		public static Frame3f Rotate(Frame3f f, Vector3d origin, Quaternionf rotation)
		{
			f.Rotate(rotation);
			f.Origin = (Vector3f)MeshTransforms.Rotate(f.Origin, origin, rotation);
			return f;
		}

		public static Frame3f Rotate(Frame3f f, Vector3d origin, Quaterniond rotation)
		{
			f.Rotate((Quaternionf)rotation);
			f.Origin = (Vector3f)MeshTransforms.Rotate(f.Origin, origin, rotation);
			return f;
		}

		public static void Rotate(IDeformableMesh mesh, Vector3d origin, Quaternionf rotation)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vector3d = mesh.GetVertex(i);
					vector3d -= origin;
					vector3d = rotation * (Vector3f)vector3d;
					vector3d += origin;
					mesh.SetVertex(i, vector3d);
				}
			}
		}

		public static Vector3d Rotate(Vector3d pos, Vector3d origin, Quaterniond rotation)
		{
			return rotation * (pos - origin) + origin;
		}

		public static void Rotate(IDeformableMesh mesh, Vector3d origin, Quaterniond rotation)
		{
			bool hasVertexNormals = mesh.HasVertexNormals;
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vNewPos = rotation * (mesh.GetVertex(i) - origin) + origin;
					mesh.SetVertex(i, vNewPos);
					if (hasVertexNormals)
					{
						mesh.SetVertexNormal(i, (Vector3f)(rotation * mesh.GetVertexNormal(i)));
					}
				}
			}
		}

		public static void Scale(IDeformableMesh mesh, Vector3d scale, Vector3d origin)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					vertex.x -= origin.x;
					vertex.y -= origin.y;
					vertex.z -= origin.z;
					vertex.x *= scale.x;
					vertex.y *= scale.y;
					vertex.z *= scale.z;
					vertex.x += origin.x;
					vertex.y += origin.y;
					vertex.z += origin.z;
					mesh.SetVertex(i, vertex);
				}
			}
		}

		public static void Scale(IDeformableMesh mesh, double sx, double sy, double sz)
		{
			MeshTransforms.Scale(mesh, new Vector3d(sx, sy, sz), Vector3d.Zero);
		}

		public static void Scale(IDeformableMesh mesh, double s)
		{
			MeshTransforms.Scale(mesh, s, s, s);
		}

		public static void ToFrame(IDeformableMesh mesh, Frame3f f)
		{
			int maxVertexID = mesh.MaxVertexID;
			bool hasVertexNormals = mesh.HasVertexNormals;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					Vector3d vNewPos = f.ToFrameP(ref vertex);
					mesh.SetVertex(i, vNewPos);
					if (hasVertexNormals)
					{
						Vector3f vertexNormal = mesh.GetVertexNormal(i);
						Vector3f vNewNormal = f.ToFrameV(ref vertexNormal);
						mesh.SetVertexNormal(i, vNewNormal);
					}
				}
			}
		}

		public static void FromFrame(IDeformableMesh mesh, Frame3f f)
		{
			int maxVertexID = mesh.MaxVertexID;
			bool hasVertexNormals = mesh.HasVertexNormals;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					Vector3d vNewPos = f.FromFrameP(ref vertex);
					mesh.SetVertex(i, vNewPos);
					if (hasVertexNormals)
					{
						Vector3f vertexNormal = mesh.GetVertexNormal(i);
						Vector3f vNewNormal = f.FromFrameV(ref vertexNormal);
						mesh.SetVertexNormal(i, vNewNormal);
					}
				}
			}
		}

		public static Vector3d ConvertZUpToYUp(Vector3d v)
		{
			return new Vector3d(v.x, v.z, -v.y);
		}

		public static Vector3f ConvertZUpToYUp(Vector3f v)
		{
			return new Vector3f(v.x, v.z, -v.y);
		}

		public static Frame3f ConvertZUpToYUp(Frame3f f)
		{
			return new Frame3f(MeshTransforms.ConvertZUpToYUp(f.Origin), MeshTransforms.ConvertZUpToYUp(f.X), MeshTransforms.ConvertZUpToYUp(f.Y), MeshTransforms.ConvertZUpToYUp(f.Z));
		}

		public static void ConvertZUpToYUp(IDeformableMesh mesh)
		{
			int maxVertexID = mesh.MaxVertexID;
			bool hasVertexNormals = mesh.HasVertexNormals;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					mesh.SetVertex(i, new Vector3d(vertex.x, vertex.z, -vertex.y));
					if (hasVertexNormals)
					{
						Vector3f vertexNormal = mesh.GetVertexNormal(i);
						mesh.SetVertexNormal(i, new Vector3f(vertexNormal.x, vertexNormal.z, -vertexNormal.y));
					}
				}
			}
		}

		public static Vector3d ConvertYUpToZUp(Vector3d v)
		{
			return new Vector3d(v.x, -v.z, v.y);
		}

		public static Vector3f ConvertYUpToZUp(Vector3f v)
		{
			return new Vector3f(v.x, -v.z, v.y);
		}

		public static Frame3f ConvertYUpToZUp(Frame3f f)
		{
			return new Frame3f(MeshTransforms.ConvertYUpToZUp(f.Origin), MeshTransforms.ConvertYUpToZUp(f.X), MeshTransforms.ConvertYUpToZUp(f.Y), MeshTransforms.ConvertYUpToZUp(f.Z));
		}

		public static void ConvertYUpToZUp(IDeformableMesh mesh)
		{
			int maxVertexID = mesh.MaxVertexID;
			bool hasVertexNormals = mesh.HasVertexNormals;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					mesh.SetVertex(i, new Vector3d(vertex.x, -vertex.z, vertex.y));
					if (hasVertexNormals)
					{
						Vector3f vertexNormal = mesh.GetVertexNormal(i);
						mesh.SetVertexNormal(i, new Vector3f(vertexNormal.x, -vertexNormal.z, vertexNormal.y));
					}
				}
			}
		}

		public static Vector3d FlipLeftRightCoordSystems(Vector3d v)
		{
			return new Vector3d(v.x, v.y, -v.z);
		}

		public static Vector3f FlipLeftRightCoordSystems(Vector3f v)
		{
			return new Vector3f(v.x, v.y, -v.z);
		}

		public static Frame3f FlipLeftRightCoordSystems(Frame3f f)
		{
			throw new NotImplementedException("this doesn't work...frame becomes broken somehow?");
		}

		public static void FlipLeftRightCoordSystems(IDeformableMesh mesh)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vertex = mesh.GetVertex(i);
					vertex.z = -vertex.z;
					mesh.SetVertex(i, vertex);
					if (mesh.HasVertexNormals)
					{
						Vector3f vertexNormal = mesh.GetVertexNormal(i);
						vertexNormal.z = -vertexNormal.z;
						mesh.SetVertexNormal(i, vertexNormal);
					}
				}
			}
			if (mesh is DMesh3)
			{
				(mesh as DMesh3).ReverseOrientation(false);
				return;
			}
			throw new Exception("argh don't want this in IDeformableMesh...but then for SimpleMesh??");
		}

		public static void VertexNormalOffset(IDeformableMesh mesh, double offsetDistance)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vNewPos = mesh.GetVertex(i) + offsetDistance * mesh.GetVertexNormal(i);
					mesh.SetVertex(i, vNewPos);
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh mesh, Func<Vector3d, Vector3d> TransformF)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vNewPos = TransformF(mesh.GetVertex(i));
					mesh.SetVertex(i, vNewPos);
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh mesh, Func<Vector3d, Vector3f, Vector3d> TransformF)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3d vNewPos = TransformF(mesh.GetVertex(i), mesh.GetVertexNormal(i));
					mesh.SetVertex(i, vNewPos);
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh mesh, Func<Vector3d, Vector3f, Vector3dTuple2> TransformF)
		{
			int maxVertexID = mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (mesh.IsVertex(i))
				{
					Vector3dTuple2 vector3dTuple = TransformF(mesh.GetVertex(i), mesh.GetVertexNormal(i));
					mesh.SetVertex(i, vector3dTuple.V0);
					mesh.SetVertexNormal(i, (Vector3f)vector3dTuple.V1);
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh mesh, TransformSequence xform)
		{
			int maxVertexID = mesh.MaxVertexID;
			if (mesh.HasVertexNormals)
			{
				for (int i = 0; i < maxVertexID; i++)
				{
					if (mesh.IsVertex(i))
					{
						mesh.SetVertex(i, xform.TransformP(mesh.GetVertex(i)));
						mesh.SetVertexNormal(i, (Vector3f)xform.TransformV(mesh.GetVertexNormal(i)));
					}
				}
				return;
			}
			for (int j = 0; j < maxVertexID; j++)
			{
				if (mesh.IsVertex(j))
				{
					mesh.SetVertex(j, xform.TransformP(mesh.GetVertex(j)));
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh mesh, IEnumerable<int> vertices, Func<Vector3d, int, Vector3d> TransformF)
		{
			foreach (int num in vertices)
			{
				if (mesh.IsVertex(num))
				{
					Vector3d vNewPos = TransformF(mesh.GetVertex(num), num);
					mesh.SetVertex(num, vNewPos);
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh mesh, IEnumerable<int> vertices, Func<int, int> MapV, Func<Vector3d, int, int, Vector3d> TransformF)
		{
			foreach (int num in vertices)
			{
				int num2 = MapV(num);
				if (mesh.IsVertex(num2))
				{
					Vector3d vNewPos = TransformF(mesh.GetVertex(num2), num, num2);
					mesh.SetVertex(num2, vNewPos);
				}
			}
		}

		public static void PerVertexTransform(IDeformableMesh targetMesh, IDeformableMesh sourceMesh, int[] mapV, Func<Vector3d, int, int, Vector3d> TransformF)
		{
			foreach (int num in sourceMesh.VertexIndices())
			{
				int num2 = mapV[num];
				if (targetMesh.IsVertex(num2))
				{
					Vector3d vNewPos = TransformF(targetMesh.GetVertex(num2), num, num2);
					targetMesh.SetVertex(num2, vNewPos);
				}
			}
		}
	}
}
