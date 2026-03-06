using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public struct TagHandle
	{
		public static TagHandle GetExistingTag(string tagName)
		{
			return new TagHandle
			{
				_tagIndex = TagHandle.ExtractTagThrowing(tagName)
			};
		}

		public override string ToString()
		{
			return TagHandle.TagToString(this._tagIndex);
		}

		[FreeFunction]
		[NativeThrows]
		[NativeHeader("Runtime/Export/Scripting/GameObject.bindings.h")]
		private unsafe static uint ExtractTagThrowing(string tagName)
		{
			uint result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tagName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tagName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = TagHandle.ExtractTagThrowing_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		private static string TagToString(uint tagIndex)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				TagHandle.TagToString_Injected(tagIndex, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint ExtractTagThrowing_Injected(ref ManagedSpanWrapper tagName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TagToString_Injected(uint tagIndex, out ManagedSpanWrapper ret);

		private uint _tagIndex;
	}
}
