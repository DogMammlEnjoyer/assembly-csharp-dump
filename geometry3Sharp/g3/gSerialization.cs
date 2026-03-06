using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace g3
{
	public static class gSerialization
	{
		public static void Store(Vector2f v, BinaryWriter writer)
		{
			writer.Write(v.x);
			writer.Write(v.y);
		}

		public static void Restore(ref Vector2f v, BinaryReader reader)
		{
			v.x = reader.ReadSingle();
			v.y = reader.ReadSingle();
		}

		public static void Store(Vector2d v, BinaryWriter writer)
		{
			writer.Write(v.x);
			writer.Write(v.y);
		}

		public static void Restore(ref Vector2d v, BinaryReader reader)
		{
			v.x = reader.ReadDouble();
			v.y = reader.ReadDouble();
		}

		public static void Store(Vector3f v, BinaryWriter writer)
		{
			writer.Write(v.x);
			writer.Write(v.y);
			writer.Write(v.z);
		}

		public static void Restore(ref Vector3f v, BinaryReader reader)
		{
			v.x = reader.ReadSingle();
			v.y = reader.ReadSingle();
			v.z = reader.ReadSingle();
		}

		public static void Store(Vector3d v, BinaryWriter writer)
		{
			writer.Write(v.x);
			writer.Write(v.y);
			writer.Write(v.z);
		}

		public static void Restore(ref Vector3d v, BinaryReader reader)
		{
			v.x = reader.ReadDouble();
			v.y = reader.ReadDouble();
			v.z = reader.ReadDouble();
		}

		public static void Store(Quaternionf q, BinaryWriter writer)
		{
			writer.Write(q.x);
			writer.Write(q.y);
			writer.Write(q.z);
			writer.Write(q.w);
		}

		public static void Restore(ref Quaternionf q, BinaryReader reader)
		{
			q.x = reader.ReadSingle();
			q.y = reader.ReadSingle();
			q.z = reader.ReadSingle();
			q.w = reader.ReadSingle();
		}

		public static void Store(Frame3f vFrame, BinaryWriter writer)
		{
			gSerialization.Store(vFrame.Origin, writer);
			gSerialization.Store(vFrame.Rotation, writer);
		}

		public static void Restore(ref Frame3f vFrame, BinaryReader reader)
		{
			Vector3f zero = Vector3f.Zero;
			Quaternionf identity = Quaternionf.Identity;
			gSerialization.Restore(ref zero, reader);
			gSerialization.Restore(ref identity, reader);
			vFrame = new Frame3f(zero, identity);
		}

		public static void Store(AxisAlignedBox2d b, BinaryWriter writer)
		{
			gSerialization.Store(b.Min, writer);
			gSerialization.Store(b.Max, writer);
		}

		public static void Restore(ref AxisAlignedBox2d b, BinaryReader reader)
		{
			gSerialization.Restore(ref b.Min, reader);
			gSerialization.Restore(ref b.Max, reader);
		}

		public static void Store(List<int> values, BinaryWriter writer)
		{
			writer.Write(values.Count);
			for (int i = 0; i < values.Count; i++)
			{
				writer.Write(values[i]);
			}
		}

		public static void Restore(List<int> values, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				values.Add(reader.ReadInt32());
			}
		}

		public static void Store(List<float> values, BinaryWriter writer)
		{
			writer.Write(values.Count);
			for (int i = 0; i < values.Count; i++)
			{
				writer.Write(values[i]);
			}
		}

		public static void Restore(List<float> values, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				values.Add(reader.ReadSingle());
			}
		}

		public static void Store(List<double> values, BinaryWriter writer)
		{
			writer.Write(values.Count);
			for (int i = 0; i < values.Count; i++)
			{
				writer.Write(values[i]);
			}
		}

		public static void Restore(List<double> values, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				values.Add(reader.ReadDouble());
			}
		}

		public static void Store(DCurve3 curve, BinaryWriter writer)
		{
			writer.Write(curve.Closed);
			writer.Write(curve.VertexCount);
			for (int i = 0; i < curve.VertexCount; i++)
			{
				writer.Write(curve[i].x);
				writer.Write(curve[i].y);
				writer.Write(curve[i].z);
			}
		}

		public static void Restore(DCurve3 curve, BinaryReader reader)
		{
			curve.Closed = reader.ReadBoolean();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				double x = reader.ReadDouble();
				double y = reader.ReadDouble();
				double z = reader.ReadDouble();
				curve.AppendVertex(new Vector3d(x, y, z));
			}
		}

		public static void Store(PolyLine2d polyline, BinaryWriter writer)
		{
			writer.Write(polyline.VertexCount);
			for (int i = 0; i < polyline.VertexCount; i++)
			{
				writer.Write(polyline[i].x);
				writer.Write(polyline[i].y);
			}
		}

		public static void Restore(PolyLine2d polyline, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				double x = reader.ReadDouble();
				double y = reader.ReadDouble();
				polyline.AppendVertex(new Vector2d(x, y));
			}
		}

		public static void Store(Polygon2d polygon, BinaryWriter writer)
		{
			writer.Write(polygon.VertexCount);
			for (int i = 0; i < polygon.VertexCount; i++)
			{
				writer.Write(polygon[i].x);
				writer.Write(polygon[i].y);
			}
		}

		public static void Restore(Polygon2d polygon, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				double x = reader.ReadDouble();
				double y = reader.ReadDouble();
				polygon.AppendVertex(new Vector2d(x, y));
			}
		}

		public static void Store(GeneralPolygon2d polygon, BinaryWriter writer)
		{
			gSerialization.Store(polygon.Outer, writer);
			writer.Write(polygon.Holes.Count);
			for (int i = 0; i < polygon.Holes.Count; i++)
			{
				gSerialization.Store(polygon.Holes[i], writer);
			}
		}

		public static void Restore(GeneralPolygon2d polygon, BinaryReader reader)
		{
			Polygon2d polygon2d = new Polygon2d();
			gSerialization.Restore(polygon2d, reader);
			polygon.Outer = polygon2d;
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Polygon2d polygon2d2 = new Polygon2d();
				gSerialization.Restore(polygon2d2, reader);
				polygon.AddHole(polygon2d2, false, true);
			}
		}

		public static void Store(Segment2d segment, BinaryWriter writer)
		{
			writer.Write(segment.Center.x);
			writer.Write(segment.Center.y);
			writer.Write(segment.Direction.x);
			writer.Write(segment.Direction.y);
			writer.Write(segment.Extent);
		}

		public static void Restore(ref Segment2d segment, BinaryReader reader)
		{
			segment.Center.x = reader.ReadDouble();
			segment.Center.y = reader.ReadDouble();
			segment.Direction.x = reader.ReadDouble();
			segment.Direction.y = reader.ReadDouble();
			segment.Extent = reader.ReadDouble();
		}

		public static void Store(Arc2d arc, BinaryWriter writer)
		{
			writer.Write(arc.Center.x);
			writer.Write(arc.Center.y);
			writer.Write(arc.Radius);
			writer.Write(arc.AngleStartDeg);
			writer.Write(arc.AngleEndDeg);
			writer.Write(arc.IsReversed);
		}

		public static void Restore(ref Arc2d arc, BinaryReader reader)
		{
			arc.Center.x = reader.ReadDouble();
			arc.Center.y = reader.ReadDouble();
			arc.Radius = reader.ReadDouble();
			arc.AngleStartDeg = reader.ReadDouble();
			arc.AngleEndDeg = reader.ReadDouble();
			arc.IsReversed = reader.ReadBoolean();
		}

		public static void Store(Circle2d circle, BinaryWriter writer)
		{
			writer.Write(circle.Center.x);
			writer.Write(circle.Center.y);
			writer.Write(circle.Radius);
			writer.Write(circle.IsReversed);
		}

		public static void Restore(ref Circle2d circle, BinaryReader reader)
		{
			circle.Center.x = reader.ReadDouble();
			circle.Center.y = reader.ReadDouble();
			circle.Radius = reader.ReadDouble();
			circle.IsReversed = reader.ReadBoolean();
		}

		public static void Store(ParametricCurveSequence2 sequence, BinaryWriter writer)
		{
			writer.Write(sequence.IsClosed);
			writer.Write(sequence.Count);
			foreach (IParametricCurve2d curve in sequence.Curves)
			{
				gSerialization.Store(curve, writer);
			}
		}

		public static void Restore(ref ParametricCurveSequence2 sequence, BinaryReader reader)
		{
			sequence.IsClosed = reader.ReadBoolean();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				IParametricCurve2d c;
				gSerialization.Restore(out c, reader);
				sequence.Append(c);
			}
		}

		public static void Store(IParametricCurve2d curve, BinaryWriter writer)
		{
			if (curve is Segment2d)
			{
				writer.Write(1);
				gSerialization.Store((Segment2d)curve, writer);
				return;
			}
			if (curve is Circle2d)
			{
				writer.Write(2);
				gSerialization.Store((Circle2d)curve, writer);
				return;
			}
			if (curve is Arc2d)
			{
				writer.Write(3);
				gSerialization.Store((Arc2d)curve, writer);
				return;
			}
			if (curve is ParametricCurveSequence2)
			{
				writer.Write(100);
				gSerialization.Store(curve as ParametricCurveSequence2, writer);
			}
		}

		public static void Restore(out IParametricCurve2d curve, BinaryReader reader)
		{
			curve = null;
			int num = reader.ReadInt32();
			if (num == 1)
			{
				Segment2d segment2d = default(Segment2d);
				gSerialization.Restore(ref segment2d, reader);
				curve = segment2d;
				return;
			}
			if (num == 2)
			{
				Circle2d circle2d = new Circle2d(Vector2d.Zero, 1.0);
				gSerialization.Restore(ref circle2d, reader);
				curve = circle2d;
				return;
			}
			if (num == 3)
			{
				Arc2d arc2d = new Arc2d(Vector2d.Zero, 1.0, 0.0, 1.0);
				gSerialization.Restore(ref arc2d, reader);
				curve = arc2d;
				return;
			}
			if (num == 100)
			{
				ParametricCurveSequence2 parametricCurveSequence = new ParametricCurveSequence2();
				gSerialization.Restore(ref parametricCurveSequence, reader);
				curve = parametricCurveSequence;
				return;
			}
			throw new Exception("gSerialization.Restore: IParametricCurve2D : unknown curve type " + num.ToString());
		}

		public static void Store(PlanarSolid2d solid, BinaryWriter writer)
		{
			gSerialization.Store(solid.Outer, writer);
			writer.Write(solid.Holes.Count);
			for (int i = 0; i < solid.Holes.Count; i++)
			{
				gSerialization.Store(solid.Holes[i], writer);
			}
		}

		public static void Restore(PlanarSolid2d solid, BinaryReader reader)
		{
			IParametricCurve2d loop;
			gSerialization.Restore(out loop, reader);
			solid.SetOuter(loop, true);
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				IParametricCurve2d hole;
				gSerialization.Restore(out hole, reader);
				solid.AddHole(hole);
			}
		}

		public static void Store(DMesh3 mesh, BinaryWriter writer)
		{
			writer.Write(gSerialization.DMesh3Version);
			int components = (int)mesh.Components;
			writer.Write(components);
			gSerialization.Store(mesh.VerticesBuffer, writer);
			gSerialization.Store(mesh.TrianglesBuffer, writer);
			gSerialization.Store(mesh.EdgesBuffer, writer);
			gSerialization.Store(mesh.EdgesRefCounts.RawRefCounts, writer);
			if ((mesh.Components & MeshComponents.VertexNormals) != MeshComponents.None)
			{
				gSerialization.Store(mesh.NormalsBuffer, writer);
			}
			if ((mesh.Components & MeshComponents.VertexColors) != MeshComponents.None)
			{
				gSerialization.Store(mesh.ColorsBuffer, writer);
			}
			if ((mesh.Components & MeshComponents.VertexUVs) != MeshComponents.None)
			{
				gSerialization.Store(mesh.UVBuffer, writer);
			}
			if ((mesh.Components & MeshComponents.FaceGroups) != MeshComponents.None)
			{
				gSerialization.Store(mesh.GroupsBuffer, writer);
			}
		}

		public static void Restore(DMesh3 mesh, BinaryReader reader)
		{
			if (reader.ReadInt32() != gSerialization.DMesh3Version)
			{
				throw new Exception("gSerialization.Restore: Incorrect DMesh3Version!");
			}
			int num = reader.ReadInt32();
			gSerialization.Restore(mesh.VerticesBuffer, reader);
			gSerialization.Restore(mesh.TrianglesBuffer, reader);
			gSerialization.Restore(mesh.EdgesBuffer, reader);
			gSerialization.Restore(mesh.EdgesRefCounts.RawRefCounts, reader);
			if ((num & 1) != 0)
			{
				mesh.EnableVertexNormals(Vector3f.AxisY);
				gSerialization.Restore(mesh.NormalsBuffer, reader);
			}
			else
			{
				mesh.DiscardVertexNormals();
			}
			if ((num & 2) != 0)
			{
				mesh.EnableVertexColors(Vector3f.One);
				gSerialization.Restore(mesh.ColorsBuffer, reader);
			}
			else
			{
				mesh.DiscardVertexColors();
			}
			if ((num & 4) != 0)
			{
				mesh.EnableVertexUVs(Vector2f.Zero);
				gSerialization.Restore(mesh.UVBuffer, reader);
			}
			else
			{
				mesh.DiscardVertexUVs();
			}
			if ((num & 8) != 0)
			{
				mesh.EnableTriangleGroups(0);
				gSerialization.Restore(mesh.GroupsBuffer, reader);
			}
			else
			{
				mesh.DiscardTriangleGroups();
			}
			mesh.RebuildFromEdgeRefcounts();
		}

		public static void Store(DVector<double> vec, BinaryWriter writer)
		{
			byte[] array = new byte[vec.BlockCount * 8];
			int length = vec.Length;
			writer.Write(length);
			foreach (DVector<double>.DBlock dblock in vec.BlockIterator())
			{
				Buffer.BlockCopy(dblock.data, 0, array, 0, dblock.usedCount * 8);
				writer.Write(array, 0, dblock.usedCount * 8);
			}
		}

		public static void Restore(DVector<double> vec, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			byte[] array = reader.ReadBytes(num * 8);
			double[] array2 = new double[num];
			Buffer.BlockCopy(array, 0, array2, 0, array.Length);
			vec.Initialize(array2);
		}

		public static void Store(DVector<float> vec, BinaryWriter writer)
		{
			byte[] array = new byte[vec.BlockCount * 4];
			int length = vec.Length;
			writer.Write(length);
			foreach (DVector<float>.DBlock dblock in vec.BlockIterator())
			{
				Buffer.BlockCopy(dblock.data, 0, array, 0, dblock.usedCount * 4);
				writer.Write(array, 0, dblock.usedCount * 4);
			}
		}

		public static void Restore(DVector<float> vec, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			byte[] array = reader.ReadBytes(num * 4);
			float[] array2 = new float[num];
			Buffer.BlockCopy(array, 0, array2, 0, array.Length);
			vec.Initialize(array2);
		}

		public static void Store(DVector<int> vec, BinaryWriter writer)
		{
			byte[] array = new byte[vec.BlockCount * 4];
			int length = vec.Length;
			writer.Write(length);
			foreach (DVector<int>.DBlock dblock in vec.BlockIterator())
			{
				Buffer.BlockCopy(dblock.data, 0, array, 0, dblock.usedCount * 4);
				writer.Write(array, 0, dblock.usedCount * 4);
			}
		}

		public static void Restore(DVector<int> vec, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			byte[] array = reader.ReadBytes(num * 4);
			int[] array2 = new int[num];
			Buffer.BlockCopy(array, 0, array2, 0, array.Length);
			vec.Initialize(array2);
		}

		public static void Store(DVector<short> vec, BinaryWriter writer)
		{
			byte[] array = new byte[vec.BlockCount * 2];
			int length = vec.Length;
			writer.Write(length);
			foreach (DVector<short>.DBlock dblock in vec.BlockIterator())
			{
				Buffer.BlockCopy(dblock.data, 0, array, 0, dblock.usedCount * 2);
				writer.Write(array, 0, dblock.usedCount * 2);
			}
		}

		public static void Restore(DVector<short> vec, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			byte[] array = reader.ReadBytes(num * 2);
			short[] array2 = new short[num];
			Buffer.BlockCopy(array, 0, array2, 0, array.Length);
			vec.Initialize(array2);
		}

		public static void Store(string s, BinaryWriter writer)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			writer.Write(bytes.Length);
			writer.Write(bytes);
		}

		public static void Restore(ref string s, BinaryReader reader)
		{
			int count = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(count);
			s = Encoding.UTF8.GetString(bytes);
		}

		public static void Store(string[] s, BinaryWriter writer)
		{
			writer.Write(s.Length);
			for (int i = 0; i < s.Length; i++)
			{
				gSerialization.Store(s[i], writer);
			}
		}

		public static void Restore(ref string[] s, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			s = new string[num];
			for (int i = 0; i < num; i++)
			{
				gSerialization.Restore(ref s[i], reader);
			}
		}

		public static int DMesh3Version = 1;
	}
}
