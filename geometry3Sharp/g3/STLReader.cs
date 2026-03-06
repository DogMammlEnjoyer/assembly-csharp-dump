using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace g3
{
	public class STLReader : IMeshReader
	{
		public event ParsingMessagesHandler warningEvent;

		private void ParseArguments(CommandArgumentSet args)
		{
			if (args.Integers.ContainsKey("-stl-weld-strategy"))
			{
				this.RebuildStrategy = (STLReader.Strategy)args.Integers["-stl-weld-strategy"];
			}
			if (args.Flags.ContainsKey("-want-tri-attrib"))
			{
				this.WantPerTriAttribs = true;
			}
		}

		private void append_vertex(float x, float y, float z)
		{
			this.Objects.Last<STLReader.STLSolid>().Vertices.Append(x, y, z);
		}

		public IOReadResult Read(BinaryReader reader, ReadOptions options, IMeshBuilder builder)
		{
			if (options.CustomFlags != null)
			{
				this.ParseArguments(options.CustomFlags);
			}
			reader.ReadBytes(80);
			int num = reader.ReadInt32();
			this.Objects = new List<STLReader.STLSolid>();
			this.Objects.Add(new STLReader.STLSolid());
			int num2 = 50;
			IntPtr intPtr = Marshal.AllocHGlobal(num2);
			Type type = default(STLReader.stl_triangle).GetType();
			DVector<short> dvector = new DVector<short>();
			try
			{
				for (int i = 0; i < num; i++)
				{
					byte[] array = reader.ReadBytes(50);
					if (array.Length < 50)
					{
						break;
					}
					Marshal.Copy(array, 0, intPtr, num2);
					STLReader.stl_triangle stl_triangle = (STLReader.stl_triangle)Marshal.PtrToStructure(intPtr, type);
					this.append_vertex(stl_triangle.ax, stl_triangle.ay, stl_triangle.az);
					this.append_vertex(stl_triangle.bx, stl_triangle.by, stl_triangle.bz);
					this.append_vertex(stl_triangle.cx, stl_triangle.cy, stl_triangle.cz);
					dvector.Add(stl_triangle.attrib);
				}
			}
			catch (Exception ex)
			{
				return new IOReadResult(IOCode.GenericReaderError, "exception: " + ex.Message);
			}
			Marshal.FreeHGlobal(intPtr);
			if (this.Objects.Count == 1)
			{
				this.Objects[0].TriAttribs = dvector;
			}
			foreach (STLReader.STLSolid solid in this.Objects)
			{
				this.BuildMesh(solid, builder);
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		public IOReadResult Read(TextReader reader, ReadOptions options, IMeshBuilder builder)
		{
			if (options.CustomFlags != null)
			{
				this.ParseArguments(options.CustomFlags);
			}
			bool flag = false;
			this.Objects = new List<STLReader.STLSolid>();
			int num = 0;
			while (reader.Peek() >= 0)
			{
				string text = reader.ReadLine();
				num++;
				string[] array = text.Split(null, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0)
				{
					if (array[0].Equals("vertex", StringComparison.OrdinalIgnoreCase))
					{
						float x = (array.Length > 1) ? float.Parse(array[1]) : 0f;
						float y = (array.Length > 2) ? float.Parse(array[2]) : 0f;
						float z = (array.Length > 3) ? float.Parse(array[3]) : 0f;
						this.append_vertex(x, y, z);
					}
					else if (array[0].Equals("facet", StringComparison.OrdinalIgnoreCase))
					{
						if (!flag)
						{
							this.Objects.Add(new STLReader.STLSolid
							{
								Name = "unknown_solid"
							});
							flag = true;
						}
					}
					else if (array[0].Equals("solid", StringComparison.OrdinalIgnoreCase))
					{
						STLReader.STLSolid stlsolid = new STLReader.STLSolid();
						if (array.Length == 2)
						{
							stlsolid.Name = array[1];
						}
						else
						{
							stlsolid.Name = "object_" + this.Objects.Count.ToString();
						}
						this.Objects.Add(stlsolid);
						flag = true;
					}
					else if (array[0].Equals("endsolid", StringComparison.OrdinalIgnoreCase))
					{
						flag = false;
					}
				}
			}
			foreach (STLReader.STLSolid solid in this.Objects)
			{
				this.BuildMesh(solid, builder);
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		protected virtual void BuildMesh(STLReader.STLSolid solid, IMeshBuilder builder)
		{
			if (this.RebuildStrategy == STLReader.Strategy.AutoBestResult)
			{
				DMesh3 existingMesh = this.BuildMesh_Auto(solid);
				builder.AppendNewMesh(existingMesh);
			}
			else if (this.RebuildStrategy == STLReader.Strategy.IdenticalVertexWeld)
			{
				DMesh3 existingMesh2 = this.BuildMesh_IdenticalWeld(solid);
				builder.AppendNewMesh(existingMesh2);
			}
			else if (this.RebuildStrategy == STLReader.Strategy.TolerantVertexWeld)
			{
				DMesh3 existingMesh3 = this.BuildMesh_TolerantWeld(solid, this.WeldTolerance);
				builder.AppendNewMesh(existingMesh3);
			}
			else
			{
				this.BuildMesh_NoMerge(solid, builder);
			}
			if (this.WantPerTriAttribs && solid.TriAttribs != null && builder.SupportsMetaData)
			{
				builder.AppendMetaData(STLReader.PerTriAttribMetadataName, solid.TriAttribs);
			}
		}

		protected virtual void BuildMesh_NoMerge(STLReader.STLSolid solid, IMeshBuilder builder)
		{
			builder.AppendNewMesh(false, false, false, false);
			DVectorArray3f vertices = solid.Vertices;
			int num = vertices.Count / 3;
			for (int i = 0; i < num; i++)
			{
				Vector3f vector3f = vertices[3 * i];
				int i2 = builder.AppendVertex((double)vector3f.x, (double)vector3f.y, (double)vector3f.z);
				Vector3f vector3f2 = vertices[3 * i + 1];
				int j = builder.AppendVertex((double)vector3f2.x, (double)vector3f2.y, (double)vector3f2.z);
				Vector3f vector3f3 = vertices[3 * i + 2];
				int k = builder.AppendVertex((double)vector3f3.x, (double)vector3f3.y, (double)vector3f3.z);
				builder.AppendTriangle(i2, j, k);
			}
		}

		protected virtual DMesh3 BuildMesh_Auto(STLReader.STLSolid solid)
		{
			DMesh3 dmesh = this.BuildMesh_IdenticalWeld(solid);
			int num;
			if (!this.check_for_cracks(dmesh, out num, this.WeldTolerance))
			{
				return dmesh;
			}
			DMesh3 dmesh2 = this.BuildMesh_TolerantWeld(solid, this.WeldTolerance);
			if (this.count_boundary_edges(dmesh2) < num)
			{
				return dmesh2;
			}
			return dmesh;
		}

		protected int count_boundary_edges(DMesh3 mesh)
		{
			int num = 0;
			foreach (int num2 in mesh.BoundaryEdgeIndices())
			{
				num++;
			}
			return num;
		}

		protected bool check_for_cracks(DMesh3 mesh, out int boundary_edge_count, double crack_tol = 9.999999974752427E-07)
		{
			boundary_edge_count = 0;
			MeshVertexSelection meshVertexSelection = new MeshVertexSelection(mesh);
			foreach (int eID in mesh.BoundaryEdgeIndices())
			{
				Index2i edgeV = mesh.GetEdgeV(eID);
				meshVertexSelection.Select(edgeV.a);
				meshVertexSelection.Select(edgeV.b);
				boundary_edge_count++;
			}
			if (meshVertexSelection.Count == 0)
			{
				return false;
			}
			PointHashGrid3d<int> pointHashGrid3d = new PointHashGrid3d<int>(mesh.CachedBounds.MaxDim / 128.0, -1);
			foreach (int num in meshVertexSelection)
			{
				Vector3d v = mesh.GetVertex(num);
				if (pointHashGrid3d.FindNearestInRadius(v, crack_tol, (int existing_vid) => v.Distance(mesh.GetVertex(existing_vid)), null).Key != -1)
				{
					return true;
				}
				pointHashGrid3d.InsertPoint(num, v);
			}
			return false;
		}

		protected virtual DMesh3 BuildMesh_IdenticalWeld(STLReader.STLSolid solid)
		{
			DMesh3Builder dmesh3Builder = new DMesh3Builder();
			dmesh3Builder.AppendNewMesh(false, false, false, false);
			DVectorArray3f vertices = solid.Vertices;
			int count = vertices.Count;
			int[] array = new int[count];
			Dictionary<Vector3f, int> dictionary = new Dictionary<Vector3f, int>();
			for (int i = 0; i < count; i++)
			{
				Vector3f vector3f = vertices[i];
				int num;
				if (dictionary.TryGetValue(vector3f, out num))
				{
					array[i] = num;
				}
				else
				{
					int num2 = dmesh3Builder.AppendVertex((double)vector3f.x, (double)vector3f.y, (double)vector3f.z);
					dictionary[vector3f] = num2;
					array[i] = num2;
				}
			}
			this.append_mapped_triangles(solid, dmesh3Builder, array);
			return dmesh3Builder.Meshes[0];
		}

		protected virtual DMesh3 BuildMesh_TolerantWeld(STLReader.STLSolid solid, double weld_tolerance)
		{
			DMesh3Builder dmesh3Builder = new DMesh3Builder();
			dmesh3Builder.AppendNewMesh(false, false, false, false);
			DVectorArray3f vertices = solid.Vertices;
			int count = vertices.Count;
			int[] array = new int[count];
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			for (int i = 0; i < count; i++)
			{
				empty.Contain(vertices[i]);
			}
			int num = 256;
			if (count > 100000)
			{
				num = 512;
			}
			if (count > 1000000)
			{
				num = 1024;
			}
			if (count > 2000000)
			{
				num = 2048;
			}
			if (count > 5000000)
			{
				num = 4096;
			}
			PointHashGrid3d<int> pointHashGrid3d = new PointHashGrid3d<int>(empty.MaxDim / (double)((float)num), -1);
			Vector3f[] pos = new Vector3f[count];
			for (int j = 0; j < count; j++)
			{
				Vector3f v = vertices[j];
				KeyValuePair<int, double> keyValuePair = pointHashGrid3d.FindNearestInRadius(v, weld_tolerance, (int vid) => (double)v.Distance(pos[vid]), null);
				if (keyValuePair.Key == -1)
				{
					int num2 = dmesh3Builder.AppendVertex((double)v.x, (double)v.y, (double)v.z);
					pointHashGrid3d.InsertPoint(num2, v);
					array[j] = num2;
					pos[num2] = v;
				}
				else
				{
					array[j] = keyValuePair.Key;
				}
			}
			this.append_mapped_triangles(solid, dmesh3Builder, array);
			return dmesh3Builder.Meshes[0];
		}

		private void append_mapped_triangles(STLReader.STLSolid solid, DMesh3Builder builder, int[] mapV)
		{
			int num = solid.Vertices.Count / 3;
			for (int i = 0; i < num; i++)
			{
				int num2 = mapV[3 * i];
				int num3 = mapV[3 * i + 1];
				int num4 = mapV[3 * i + 2];
				if (num2 != num3 && num2 != num4 && num3 != num4)
				{
					builder.AppendTriangle(num2, num3, num4);
				}
			}
		}

		private void emit_warning(string sMessage)
		{
			string key = sMessage.Substring(0, 15);
			int num = this.warningCount.ContainsKey(key) ? this.warningCount[key] : 0;
			num++;
			this.warningCount[key] = num;
			if (num > 10)
			{
				return;
			}
			if (num == 10)
			{
				sMessage += " (additional message surpressed)";
			}
			ParsingMessagesHandler parsingMessagesHandler = this.warningEvent;
			if (parsingMessagesHandler != null)
			{
				parsingMessagesHandler(sMessage, null);
			}
		}

		public STLReader.Strategy RebuildStrategy = STLReader.Strategy.AutoBestResult;

		public double WeldTolerance = 9.999999974752427E-07;

		public bool WantPerTriAttribs;

		public static string PerTriAttribMetadataName = "tri_attrib";

		private Dictionary<string, int> warningCount = new Dictionary<string, int>();

		public const string StrategyFlag = "-stl-weld-strategy";

		public const string PerTriAttribFlag = "-want-tri-attrib";

		private List<STLReader.STLSolid> Objects;

		public enum Strategy
		{
			NoProcessing,
			IdenticalVertexWeld,
			TolerantVertexWeld,
			AutoBestResult
		}

		protected class STLSolid
		{
			public string Name;

			public DVectorArray3f Vertices = new DVectorArray3f(0);

			public DVector<short> TriAttribs;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct stl_triangle
		{
			public float nx;

			public float ny;

			public float nz;

			public float ax;

			public float ay;

			public float az;

			public float bx;

			public float by;

			public float bz;

			public float cx;

			public float cy;

			public float cz;

			public short attrib;
		}
	}
}
