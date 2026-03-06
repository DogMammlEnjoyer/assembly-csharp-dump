using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace g3
{
	public class BufferUtil
	{
		public static void SetVertex3(double[] v, int i, double x, double y, double z)
		{
			v[3 * i] = x;
			v[3 * i + 1] = y;
			v[3 * i + 2] = z;
		}

		public static void SetVertex3(float[] v, int i, float x, float y, float z)
		{
			v[3 * i] = x;
			v[3 * i + 1] = y;
			v[3 * i + 2] = z;
		}

		public static void SetVertex2(double[] v, int i, double x, double y)
		{
			v[2 * i] = x;
			v[2 * i + 1] = y;
		}

		public static void SetVertex2(float[] v, int i, float x, float y)
		{
			v[2 * i] = x;
			v[2 * i + 1] = y;
		}

		public static void SetTriangle(int[] v, int i, int a, int b, int c)
		{
			v[3 * i] = a;
			v[3 * i + 1] = b;
			v[3 * i + 2] = c;
		}

		public static double Dot(double[] a, double[] b)
		{
			double num = 0.0;
			for (int i = 0; i < a.Length; i++)
			{
				num += a[i] * b[i];
			}
			return num;
		}

		public static void MultiplyAdd(double[] dest, double multiply, double[] add)
		{
			for (int i = 0; i < dest.Length; i++)
			{
				dest[i] += multiply * add[i];
			}
		}

		public static void MultiplyAdd(double[] dest, double[] multiply, double[] add)
		{
			for (int i = 0; i < dest.Length; i++)
			{
				dest[i] += multiply[i] * add[i];
			}
		}

		public static double MultiplyAdd_GetSqrSum(double[] dest, double multiply, double[] add)
		{
			double num = 0.0;
			for (int i = 0; i < dest.Length; i++)
			{
				dest[i] += multiply * add[i];
				num += dest[i] * dest[i];
			}
			return num;
		}

		public static double DistanceSquared(double[] a, double[] b)
		{
			double num = 0.0;
			for (int i = 0; i < a.Length; i++)
			{
				num += (a[i] - b[i]) * (a[i] - b[i]);
			}
			return num;
		}

		public static void ParallelDot(double[] a, double[][] b, double[][] result)
		{
			int num = a.Length;
			int count = b.Length;
			gParallel.BlockStartEnd(0, num - 1, delegate(int i0, int i1)
			{
				for (int j = i0; j <= i1; j++)
				{
					for (int k = 0; k < count; k++)
					{
						result[k][j] = a[j] * b[k][j];
					}
				}
			}, 1000, false);
		}

		public static double[][] AllocNxM(int N, int M)
		{
			double[][] array = new double[N][];
			for (int i = 0; i < N; i++)
			{
				array[i] = new double[M];
			}
			return array;
		}

		public static double[][] InitNxM(int N, int M, double[][] init)
		{
			double[][] array = BufferUtil.AllocNxM(N, M);
			for (int i = 0; i < N; i++)
			{
				Array.Copy(init[i], array[i], M);
			}
			return array;
		}

		public static int CountValid<T>(T[] data, Func<T, bool> FilterF, int max_i = -1)
		{
			int num = (max_i == -1) ? data.Length : max_i;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (FilterF(data[i]))
				{
					num2++;
				}
			}
			return num2;
		}

		public static int FilterInPlace<T>(T[] data, Func<T, bool> FilterF, int max_i = -1)
		{
			int num = (max_i == -1) ? data.Length : max_i;
			int result = 0;
			for (int i = 0; i < num; i++)
			{
				if (FilterF(data[i]))
				{
					data[result++] = data[i];
				}
			}
			return result;
		}

		public static T[] Filter<T>(T[] data, Func<T, bool> FilterF, int max_i = -1)
		{
			int num = (max_i == -1) ? data.Length : max_i;
			int num2 = BufferUtil.CountValid<T>(data, FilterF, -1);
			if (num2 == 0)
			{
				return null;
			}
			T[] array = new T[num2];
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				if (FilterF(data[i]))
				{
					array[num3++] = data[i];
				}
			}
			return array;
		}

		public static Vector3d[] ToVector3d<T>(IEnumerable<T> values)
		{
			Vector3d[] array = null;
			int num = values.Count<T>();
			int num2 = 0;
			int num3 = 0;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(float))
			{
				num /= 3;
				array = new Vector3d[num];
				using (IEnumerator<float> enumerator = (values as IEnumerable<float>).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						float num4 = enumerator.Current;
						array[num2][num3++] = (double)num4;
						if (num3 == 3)
						{
							num3 = 0;
							num2++;
						}
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(double))
			{
				num /= 3;
				array = new Vector3d[num];
				using (IEnumerator<double> enumerator2 = (values as IEnumerable<double>).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						double value = enumerator2.Current;
						array[num2][num3++] = value;
						if (num3 == 3)
						{
							num3 = 0;
							num2++;
						}
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(Vector3f))
			{
				array = new Vector3d[num];
				using (IEnumerator<Vector3f> enumerator3 = (values as IEnumerable<Vector3f>).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Vector3f v = enumerator3.Current;
						array[num2++] = v;
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(Vector3d))
			{
				array = new Vector3d[num];
				using (IEnumerator<Vector3d> enumerator4 = (values as IEnumerable<Vector3d>).GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Vector3d vector3d = enumerator4.Current;
						array[num2++] = vector3d;
					}
					return array;
				}
			}
			throw new NotSupportedException("ToVector3d: unknown type " + typeFromHandle.ToString());
		}

		public static Vector3f[] ToVector3f<T>(IEnumerable<T> values)
		{
			Vector3f[] array = null;
			int num = values.Count<T>();
			int num2 = 0;
			int num3 = 0;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(float))
			{
				num /= 3;
				array = new Vector3f[num];
				using (IEnumerator<float> enumerator = (values as IEnumerable<float>).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						float value = enumerator.Current;
						array[num2][num3++] = value;
						if (num3 == 3)
						{
							num3 = 0;
							num2++;
						}
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(double))
			{
				num /= 3;
				array = new Vector3f[num];
				using (IEnumerator<double> enumerator2 = (values as IEnumerable<double>).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						double num4 = enumerator2.Current;
						array[num2][num3++] = (float)num4;
						if (num3 == 3)
						{
							num3 = 0;
							num2++;
						}
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(Vector3f))
			{
				array = new Vector3f[num];
				using (IEnumerator<Vector3f> enumerator3 = (values as IEnumerable<Vector3f>).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Vector3f vector3f = enumerator3.Current;
						array[num2++] = vector3f;
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(Vector3d))
			{
				array = new Vector3f[num];
				using (IEnumerator<Vector3d> enumerator4 = (values as IEnumerable<Vector3d>).GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Vector3d v = enumerator4.Current;
						array[num2++] = (Vector3f)v;
					}
					return array;
				}
			}
			throw new NotSupportedException("ToVector3d: unknown type " + typeFromHandle.ToString());
		}

		public static Index3i[] ToIndex3i<T>(IEnumerable<T> values)
		{
			Index3i[] array = null;
			int num = values.Count<T>();
			int num2 = 0;
			int num3 = 0;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int))
			{
				num /= 3;
				array = new Index3i[num];
				using (IEnumerator<int> enumerator = (values as IEnumerable<int>).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int value = enumerator.Current;
						array[num2][num3++] = value;
						if (num3 == 3)
						{
							num3 = 0;
							num2++;
						}
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(Index3i))
			{
				array = new Index3i[num];
				using (IEnumerator<Index3i> enumerator2 = (values as IEnumerable<Index3i>).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Index3i index3i = enumerator2.Current;
						array[num2++] = index3i;
					}
					return array;
				}
			}
			if (typeFromHandle == typeof(Vector3i))
			{
				array = new Index3i[num];
				using (IEnumerator<Vector3i> enumerator3 = (values as IEnumerable<Vector3i>).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Vector3i v = enumerator3.Current;
						array[num2++] = v;
					}
					return array;
				}
			}
			throw new NotSupportedException("ToVector3d: unknown type " + typeFromHandle.ToString());
		}

		public static int[] ToInt(byte[] buffer)
		{
			int num = 4;
			int num2 = buffer.Length / num;
			int[] array = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = BitConverter.ToInt32(buffer, i * num);
			}
			return array;
		}

		public static short[] ToShort(byte[] buffer)
		{
			int num = 2;
			int num2 = buffer.Length / num;
			short[] array = new short[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = BitConverter.ToInt16(buffer, i * num);
			}
			return array;
		}

		public static double[] ToDouble(byte[] buffer)
		{
			int num = 8;
			int num2 = buffer.Length / num;
			double[] array = new double[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = BitConverter.ToDouble(buffer, i * num);
			}
			return array;
		}

		public static float[] ToFloat(byte[] buffer)
		{
			int num = 4;
			int num2 = buffer.Length / num;
			float[] array = new float[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = BitConverter.ToSingle(buffer, i * num);
			}
			return array;
		}

		public static VectorArray3d ToVectorArray3d(byte[] buffer)
		{
			int num = 8;
			int num2 = buffer.Length / num / 3;
			VectorArray3d vectorArray3d = new VectorArray3d(num2, false);
			for (int i = 0; i < num2; i++)
			{
				double a = BitConverter.ToDouble(buffer, 3 * i * num);
				double b = BitConverter.ToDouble(buffer, (3 * i + 1) * num);
				double c = BitConverter.ToDouble(buffer, (3 * i + 2) * num);
				vectorArray3d.Set(i, a, b, c);
			}
			return vectorArray3d;
		}

		public static VectorArray2f ToVectorArray2f(byte[] buffer)
		{
			int num = 4;
			int num2 = buffer.Length / num / 2;
			VectorArray2f vectorArray2f = new VectorArray2f(num2);
			for (int i = 0; i < num2; i++)
			{
				float a = BitConverter.ToSingle(buffer, 2 * i * num);
				float b = BitConverter.ToSingle(buffer, (2 * i + 1) * num);
				vectorArray2f.Set(i, a, b);
			}
			return vectorArray2f;
		}

		public static VectorArray3f ToVectorArray3f(byte[] buffer)
		{
			int num = 4;
			int num2 = buffer.Length / num / 3;
			VectorArray3f vectorArray3f = new VectorArray3f(num2);
			for (int i = 0; i < num2; i++)
			{
				float a = BitConverter.ToSingle(buffer, 3 * i * num);
				float b = BitConverter.ToSingle(buffer, (3 * i + 1) * num);
				float c = BitConverter.ToSingle(buffer, (3 * i + 2) * num);
				vectorArray3f.Set(i, a, b, c);
			}
			return vectorArray3f;
		}

		public static VectorArray3i ToVectorArray3i(byte[] buffer)
		{
			int num = 4;
			int num2 = buffer.Length / num / 3;
			VectorArray3i vectorArray3i = new VectorArray3i(num2);
			for (int i = 0; i < num2; i++)
			{
				int a = BitConverter.ToInt32(buffer, 3 * i * num);
				int b = BitConverter.ToInt32(buffer, (3 * i + 1) * num);
				int c = BitConverter.ToInt32(buffer, (3 * i + 2) * num);
				vectorArray3i.Set(i, a, b, c, false);
			}
			return vectorArray3i;
		}

		public static IndexArray4i ToIndexArray4i(byte[] buffer)
		{
			int num = 4;
			int num2 = buffer.Length / num / 4;
			IndexArray4i indexArray4i = new IndexArray4i(num2);
			for (int i = 0; i < num2; i++)
			{
				int a = BitConverter.ToInt32(buffer, 4 * i * num);
				int b = BitConverter.ToInt32(buffer, (4 * i + 1) * num);
				int c = BitConverter.ToInt32(buffer, (4 * i + 2) * num);
				int d = BitConverter.ToInt32(buffer, (4 * i + 3) * num);
				indexArray4i.Set(i, a, b, c, d);
			}
			return indexArray4i;
		}

		public static byte[] ToBytes(int[] array)
		{
			byte[] array2 = new byte[array.Length * 4];
			Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
			return array2;
		}

		public static byte[] ToBytes(short[] array)
		{
			byte[] array2 = new byte[array.Length * 2];
			Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
			return array2;
		}

		public static byte[] ToBytes(float[] array)
		{
			byte[] array2 = new byte[array.Length * 4];
			Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
			return array2;
		}

		public static byte[] ToBytes(double[] array)
		{
			byte[] array2 = new byte[array.Length * 8];
			Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
			return array2;
		}

		public static byte[] CompressZLib(byte[] buffer, bool bFast)
		{
			MemoryStream memoryStream = new MemoryStream();
			DeflateStream deflateStream = new DeflateStream(memoryStream, bFast ? CompressionLevel.Fastest : CompressionLevel.Optimal, true);
			deflateStream.Write(buffer, 0, buffer.Length);
			deflateStream.Close();
			memoryStream.Position = 0L;
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Read(array, 0, array.Length);
			byte[] array2 = new byte[array.Length + 4];
			Buffer.BlockCopy(array, 0, array2, 4, array.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, array2, 0, 4);
			return array2;
		}

		public static byte[] DecompressZLib(byte[] zBuffer)
		{
			MemoryStream memoryStream = new MemoryStream();
			int num = BitConverter.ToInt32(zBuffer, 0);
			memoryStream.Write(zBuffer, 4, zBuffer.Length - 4);
			byte[] array = new byte[num];
			memoryStream.Position = 0L;
			new DeflateStream(memoryStream, CompressionMode.Decompress).Read(array, 0, array.Length);
			return array;
		}
	}
}
