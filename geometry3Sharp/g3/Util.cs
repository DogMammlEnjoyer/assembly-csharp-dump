using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace g3
{
	public static class Util
	{
		public static void gBreakToDebugger()
		{
		}

		[Conditional("DEBUG")]
		public static void gDevAssert(bool bValue, string message = "gDevAssert")
		{
			if (bValue)
			{
				return;
			}
			if (Util.DebugBreakOnDevAssert)
			{
				Debugger.Break();
				return;
			}
			throw new Exception(message);
		}

		public static bool IsRunningOnMono()
		{
			return Type.GetType("Mono.Runtime") != null;
		}

		public static bool IsBitSet(byte b, int pos)
		{
			return ((int)b & 1 << pos) != 0;
		}

		public static bool IsBitSet(int n, int pos)
		{
			return (n & 1 << pos) != 0;
		}

		public static bool IsTextString(byte[] array)
		{
			foreach (byte b in array)
			{
				if (b > 127)
				{
					return false;
				}
				if (b < 32 && b != 9 && b != 10 && b != 13)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsBinaryFile(string path, int max_search_len = -1)
		{
			bool result;
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				result = Util.IsBinaryStream(fileStream, max_search_len);
			}
			return result;
		}

		public static bool IsBinaryStream(Stream streamIn, int max_search_len = -1)
		{
			int num = 0;
			int num2 = 0;
			StreamReader streamReader = new StreamReader(streamIn);
			bool result = false;
			int num3;
			while ((num3 = streamReader.Read()) != -1)
			{
				if (num++ >= max_search_len)
				{
					IL_3F:
					streamIn.Seek(0L, SeekOrigin.Begin);
					return result;
				}
				if (Util.IsASCIIControlChar(num3))
				{
					break;
				}
				if (num3 == 0)
				{
					num2++;
					if (num2 >= 2)
					{
						break;
					}
				}
				else
				{
					num2 = 0;
				}
			}
			result = true;
			goto IL_3F;
		}

		public static bool IsASCIIControlChar(int ch)
		{
			return (ch > 0 && ch <= 8) || (ch > 13 && ch <= 26);
		}

		public static string ToHexString(byte[] bytes, bool upperCase = false)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
			}
			return stringBuilder.ToString();
		}

		public static float ParseInt(string s, int nDefault)
		{
			float result;
			try
			{
				result = (float)int.Parse(s);
			}
			catch
			{
				result = (float)nDefault;
			}
			return result;
		}

		public static float ParseFloat(string s, float fDefault)
		{
			float result;
			try
			{
				result = float.Parse(s);
			}
			catch
			{
				result = fDefault;
			}
			return result;
		}

		public static double ParseDouble(string s, double fDefault)
		{
			double result;
			try
			{
				result = double.Parse(s);
			}
			catch
			{
				result = fDefault;
			}
			return result;
		}

		public static float[] BufferCopy(float[] from, float[] to)
		{
			if (from == null)
			{
				return null;
			}
			if (to.Length != from.Length)
			{
				to = new float[from.Length];
			}
			Buffer.BlockCopy(from, 0, to, 0, from.Length * 4);
			return to;
		}

		public static int[] BufferCopy(int[] from, int[] to)
		{
			if (from == null)
			{
				return null;
			}
			if (to.Length != from.Length)
			{
				to = new int[from.Length];
			}
			Buffer.BlockCopy(from, 0, to, 0, from.Length * 4);
			return to;
		}

		public static string MakeFloatFormatString(int i, int nPrecision)
		{
			return string.Format("{{{0}:F{1}}}", i, nPrecision);
		}

		public static string MakeVec3FormatString(int i0, int i1, int i2, int nPrecision)
		{
			return string.Format("{{{0}:F{3}}} {{{1}:F{3}}} {{{2}:F{3}}}", new object[]
			{
				i0,
				i1,
				i2,
				nPrecision
			});
		}

		public static string ToSecMilli(TimeSpan t)
		{
			return string.Format("{0}", t.TotalSeconds);
		}

		public static T[] AppendArrays<T>(params object[] args)
		{
			int num = args.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += (args[i] as T[]).Length;
			}
			T[] array = new T[num2];
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				T[] array2 = args[j] as T[];
				Array.Copy(array2, 0, array, num3, array2.Length);
				num3 += array2.Length;
			}
			return array;
		}

		public static byte[] StructureToByteArray(object obj)
		{
			int num = Marshal.SizeOf(obj);
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(obj, intPtr, true);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return array;
		}

		public static void ByteArrayToStructure(byte[] bytearray, ref object obj)
		{
			int num = Marshal.SizeOf(obj);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(bytearray, 0, intPtr, num);
			obj = Marshal.PtrToStructure(intPtr, obj.GetType());
			Marshal.FreeHGlobal(intPtr);
		}

		public static void WriteDebugMesh(IMesh mesh, string sPath)
		{
			WriteOptions defaults = WriteOptions.Defaults;
			defaults.bWriteGroups = true;
			defaults.bPerVertexColors = true;
			defaults.bPerVertexNormals = true;
			defaults.bPerVertexUVs = true;
			StandardMeshWriter.WriteFile(sPath, new List<WriteMesh>
			{
				new WriteMesh(mesh, "")
			}, defaults);
		}

		public static void WriteDebugMeshAndMarkers(IMesh mesh, List<Vector3d> Markers, string sPath)
		{
			WriteOptions defaults = WriteOptions.Defaults;
			defaults.bWriteGroups = true;
			List<WriteMesh> list = new List<WriteMesh>
			{
				new WriteMesh(mesh, "")
			};
			double f = BoundsUtil.Bounds(mesh).Diagonal.Length * 0.009999999776482582;
			foreach (Vector3d center in Markers)
			{
				TrivialBox3Generator trivialBox3Generator = new TrivialBox3Generator();
				trivialBox3Generator.Box = new Box3d(center, f * Vector3d.One);
				trivialBox3Generator.Generate();
				DMesh3 dmesh = new DMesh3(true, false, false, false);
				trivialBox3Generator.MakeMesh(dmesh);
				list.Add(new WriteMesh(dmesh, ""));
			}
			StandardMeshWriter.WriteFile(sPath, list, defaults);
		}

		public static bool DebugBreakOnDevAssert;
	}
}
