using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	public static class HashSetExtensions
	{
		public static void ExceptWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this NativeParallelHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}

		public static void ExceptWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Remove(item);
			}
		}

		public static void IntersectWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T item in other)
			{
				if (container.Contains(item))
				{
					other2.Add(item);
				}
			}
			container.Clear();
			ref container.UnionWith(other2);
			other2.Dispose();
		}

		public static void UnionWith<[IsUnmanaged] T>(this UnsafeParallelHashSet<T> container, UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T item in other)
			{
				container.Add(item);
			}
		}
	}
}
