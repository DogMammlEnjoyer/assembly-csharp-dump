using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fusion
{
	public static class LogUtils
	{
		public unsafe static LogUtils.DumpDeferredPtr<T> GetDump<[IsUnmanaged] T>(T* ptr) where T : struct, ValueType, ILogDumpable
		{
			return new LogUtils.DumpDeferredPtr<T>(ptr);
		}

		public static LogUtils.DumpDeferredClass GetDump<T>(T obj) where T : class, ILogDumpable
		{
			return new LogUtils.DumpDeferredClass(obj);
		}

		public readonly struct DumpDeferredPtr<[IsUnmanaged] T> where T : struct, ValueType, ILogDumpable
		{
			public unsafe DumpDeferredPtr(T* ptr)
			{
				this.<ptr>P = ptr;
			}

			public unsafe override string ToString()
			{
				if (this.<ptr>P != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					this.<ptr>P->Dump(stringBuilder);
					return stringBuilder.ToString();
				}
				return "null";
			}

			[CompilerGenerated]
			private unsafe readonly T* <ptr>P;
		}

		public readonly struct DumpDeferredStruct<[IsUnmanaged] T> where T : struct, ValueType, ILogDumpable
		{
			public DumpDeferredStruct(T obj)
			{
				this.<obj>P = obj;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				T t = this.<obj>P;
				t.Dump(stringBuilder);
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private readonly T <obj>P;
		}

		public readonly struct DumpDeferredClass
		{
			public DumpDeferredClass(ILogDumpable obj)
			{
				this.Obj = obj;
			}

			public override string ToString()
			{
				if (this.Obj != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					this.Obj.Dump(stringBuilder);
					return stringBuilder.ToString();
				}
				return "null";
			}

			public readonly ILogDumpable Obj;
		}
	}
}
