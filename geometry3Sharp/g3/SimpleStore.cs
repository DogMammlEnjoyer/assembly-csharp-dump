using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class SimpleStore
	{
		public SimpleStore()
		{
		}

		public SimpleStore(object[] objs)
		{
			this.Add(objs);
		}

		public void Add(object[] objs)
		{
			foreach (object obj in objs)
			{
				if (obj is DMesh3)
				{
					this.Meshes.Add(obj as DMesh3);
				}
				else if (obj is string)
				{
					this.Strings.Add(obj as string);
				}
				else if (obj is List<int>)
				{
					this.IntLists.Add(obj as List<int>);
				}
				else if (obj is IEnumerable<int>)
				{
					this.IntLists.Add(new List<int>(obj as IEnumerable<int>));
				}
				else
				{
					if (!(obj is Vector3d))
					{
						throw new Exception("SimpleStore: unknown type " + obj.GetType().ToString());
					}
					this.Points.Add((Vector3d)obj);
				}
			}
		}

		public static void Store(string sPath, object[] objs)
		{
			SimpleStore s = new SimpleStore(objs);
			SimpleStore.Store(sPath, s);
		}

		public static void Store(string sPath, SimpleStore s)
		{
			using (FileStream fileStream = new FileStream(sPath, FileMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					binaryWriter.Write(s.Meshes.Count);
					for (int i = 0; i < s.Meshes.Count; i++)
					{
						gSerialization.Store(s.Meshes[i], binaryWriter);
					}
					binaryWriter.Write(s.Points.Count);
					for (int j = 0; j < s.Points.Count; j++)
					{
						gSerialization.Store(s.Points[j], binaryWriter);
					}
					binaryWriter.Write(s.Strings.Count);
					for (int k = 0; k < s.Strings.Count; k++)
					{
						gSerialization.Store(s.Strings[k], binaryWriter);
					}
					binaryWriter.Write(s.IntLists.Count);
					for (int l = 0; l < s.IntLists.Count; l++)
					{
						gSerialization.Store(s.IntLists[l], binaryWriter);
					}
				}
			}
		}

		public static SimpleStore Restore(string sPath)
		{
			SimpleStore simpleStore = new SimpleStore();
			using (FileStream fileStream = new FileStream(sPath, FileMode.Open))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						DMesh3 dmesh = new DMesh3(true, false, false, false);
						gSerialization.Restore(dmesh, binaryReader);
						simpleStore.Meshes.Add(dmesh);
					}
					int num2 = binaryReader.ReadInt32();
					for (int j = 0; j < num2; j++)
					{
						Vector3d zero = Vector3d.Zero;
						gSerialization.Restore(ref zero, binaryReader);
						simpleStore.Points.Add(zero);
					}
					int num3 = binaryReader.ReadInt32();
					for (int k = 0; k < num3; k++)
					{
						string item = null;
						gSerialization.Restore(ref item, binaryReader);
						simpleStore.Strings.Add(item);
					}
					int num4 = binaryReader.ReadInt32();
					for (int l = 0; l < num4; l++)
					{
						List<int> list = new List<int>();
						gSerialization.Restore(list, binaryReader);
						simpleStore.IntLists.Add(list);
					}
				}
			}
			return simpleStore;
		}

		public List<DMesh3> Meshes = new List<DMesh3>();

		public List<Vector3d> Points = new List<Vector3d>();

		public List<string> Strings = new List<string>();

		public List<List<int>> IntLists = new List<List<int>>();
	}
}
