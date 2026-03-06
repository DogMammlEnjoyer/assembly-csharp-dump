using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[StackTraceHidden]
	internal static class ThrowHelper
	{
		internal static void ThrowArgumentNullException(ExceptionArgument argument)
		{
			throw ThrowHelper.CreateArgumentNullException(argument);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentNullException(ExceptionArgument argument)
		{
			return new ArgumentNullException(argument.ToString());
		}

		internal static void ThrowArrayTypeMismatchException()
		{
			throw ThrowHelper.CreateArrayTypeMismatchException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArrayTypeMismatchException()
		{
			return new ArrayTypeMismatchException();
		}

		internal static void ThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type)
		{
			throw ThrowHelper.CreateArgumentException_InvalidTypeWithPointersNotSupported(type);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_InvalidTypeWithPointersNotSupported(Type type)
		{
			return new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", type));
		}

		internal static void ThrowArgumentException_DestinationTooShort()
		{
			throw ThrowHelper.CreateArgumentException_DestinationTooShort();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_DestinationTooShort()
		{
			return new ArgumentException("Destination is too short.");
		}

		internal static void ThrowIndexOutOfRangeException()
		{
			throw ThrowHelper.CreateIndexOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateIndexOutOfRangeException()
		{
			return new IndexOutOfRangeException();
		}

		internal static void ThrowArgumentOutOfRangeException()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException()
		{
			return new ArgumentOutOfRangeException();
		}

		internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException(argument);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException(ExceptionArgument argument)
		{
			return new ArgumentOutOfRangeException(argument.ToString());
		}

		internal static void ThrowArgumentOutOfRangeException_PrecisionTooLarge()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_PrecisionTooLarge();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_PrecisionTooLarge()
		{
			return new ArgumentOutOfRangeException("precision", SR.Format("Precision cannot be larger than {0}.", 99));
		}

		internal static void ThrowArgumentOutOfRangeException_SymbolDoesNotFit()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_SymbolDoesNotFit();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_SymbolDoesNotFit()
		{
			return new ArgumentOutOfRangeException("symbol", "Format specifier was invalid.");
		}

		internal static void ThrowInvalidOperationException()
		{
			throw ThrowHelper.CreateInvalidOperationException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException()
		{
			return new InvalidOperationException();
		}

		internal static void ThrowInvalidOperationException_OutstandingReferences()
		{
			throw ThrowHelper.CreateInvalidOperationException_OutstandingReferences();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_OutstandingReferences()
		{
			return new InvalidOperationException("Release all references before disposing this instance.");
		}

		internal static void ThrowInvalidOperationException_UnexpectedSegmentType()
		{
			throw ThrowHelper.CreateInvalidOperationException_UnexpectedSegmentType();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_UnexpectedSegmentType()
		{
			return new InvalidOperationException("Unexpected segment type.");
		}

		internal static void ThrowInvalidOperationException_EndPositionNotReached()
		{
			throw ThrowHelper.CreateInvalidOperationException_EndPositionNotReached();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_EndPositionNotReached()
		{
			return new InvalidOperationException("End position was not reached during enumeration.");
		}

		internal static void ThrowArgumentOutOfRangeException_PositionOutOfRange()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_PositionOutOfRange();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_PositionOutOfRange()
		{
			return new ArgumentOutOfRangeException("position");
		}

		internal static void ThrowArgumentOutOfRangeException_OffsetOutOfRange()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_OffsetOutOfRange();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_OffsetOutOfRange()
		{
			return new ArgumentOutOfRangeException("offset");
		}

		internal static void ThrowObjectDisposedException_ArrayMemoryPoolBuffer()
		{
			throw ThrowHelper.CreateObjectDisposedException_ArrayMemoryPoolBuffer();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateObjectDisposedException_ArrayMemoryPoolBuffer()
		{
			return new ObjectDisposedException("ArrayMemoryPoolBuffer");
		}

		internal static void ThrowFormatException_BadFormatSpecifier()
		{
			throw ThrowHelper.CreateFormatException_BadFormatSpecifier();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateFormatException_BadFormatSpecifier()
		{
			return new FormatException("Format specifier was invalid.");
		}

		internal static void ThrowArgumentException_OverlapAlignmentMismatch()
		{
			throw ThrowHelper.CreateArgumentException_OverlapAlignmentMismatch();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_OverlapAlignmentMismatch()
		{
			return new ArgumentException("Overlapping spans have mismatching alignment.");
		}

		internal static void ThrowNotSupportedException()
		{
			throw ThrowHelper.CreateThrowNotSupportedException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateThrowNotSupportedException()
		{
			return new NotSupportedException();
		}

		public static bool TryFormatThrowFormatException(out int bytesWritten)
		{
			bytesWritten = 0;
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
			return false;
		}

		public static bool TryParseThrowFormatException<T>(out T value, out int bytesConsumed)
		{
			value = default(T);
			bytesConsumed = 0;
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
			return false;
		}

		public static void ThrowArgumentValidationException<T>(ReadOnlySequenceSegment<T> startSegment, int startIndex, ReadOnlySequenceSegment<T> endSegment)
		{
			throw ThrowHelper.CreateArgumentValidationException<T>(startSegment, startIndex, endSegment);
		}

		private static Exception CreateArgumentValidationException<T>(ReadOnlySequenceSegment<T> startSegment, int startIndex, ReadOnlySequenceSegment<T> endSegment)
		{
			if (startSegment == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.startSegment);
			}
			if (endSegment == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.endSegment);
			}
			if (startSegment != endSegment && startSegment.RunningIndex > endSegment.RunningIndex)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endSegment);
			}
			if (startSegment.Memory.Length < startIndex)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.startIndex);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endIndex);
		}

		public static void ThrowArgumentValidationException(Array array, int start)
		{
			throw ThrowHelper.CreateArgumentValidationException(array, start);
		}

		private static Exception CreateArgumentValidationException(Array array, int start)
		{
			if (array == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.array);
			}
			if (start > array.Length)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
		}

		public static void ThrowStartOrEndArgumentValidationException(long start)
		{
			throw ThrowHelper.CreateStartOrEndArgumentValidationException(start);
		}

		private static Exception CreateStartOrEndArgumentValidationException(long start)
		{
			if (start < 0L)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
		}

		internal static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
		{
			throw new ArgumentException(Environment.GetResourceString("The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.", new object[]
			{
				key,
				targetType
			}), "key");
		}

		internal static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
		{
			throw new ArgumentException(Environment.GetResourceString("The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.", new object[]
			{
				value,
				targetType
			}), "value");
		}

		internal static void ThrowKeyNotFoundException()
		{
			throw new KeyNotFoundException();
		}

		internal static void ThrowArgumentException(ExceptionResource resource)
		{
			throw new ArgumentException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowArgumentException(ExceptionResource resource, ExceptionArgument argument)
		{
			throw new ArgumentException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)), ThrowHelper.GetArgumentName(argument));
		}

		internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
		{
			if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
			{
				throw new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), string.Empty);
			}
			throw new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowInvalidOperationException(ExceptionResource resource)
		{
			throw new InvalidOperationException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowSerializationException(ExceptionResource resource)
		{
			throw new SerializationException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowSecurityException(ExceptionResource resource)
		{
			throw new SecurityException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowNotSupportedException(ExceptionResource resource)
		{
			throw new NotSupportedException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowUnauthorizedAccessException(ExceptionResource resource)
		{
			throw new UnauthorizedAccessException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowObjectDisposedException(string objectName, ExceptionResource resource)
		{
			throw new ObjectDisposedException(objectName, Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
		}

		internal static void ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
		{
			throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
		}

		internal static void ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen()
		{
			throw new InvalidOperationException("Enumeration has either not started or has already finished.");
		}

		internal static void ThrowInvalidOperationException_InvalidOperation_EnumNotStarted()
		{
			throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
		}

		internal static void ThrowInvalidOperationException_InvalidOperation_EnumEnded()
		{
			throw new InvalidOperationException("Enumeration already finished.");
		}

		internal static void ThrowInvalidOperationException_InvalidOperation_NoValue()
		{
			throw new InvalidOperationException("Nullable object must have a value.");
		}

		private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument, string resource)
		{
			return new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), resource);
		}

		internal static void ThrowArgumentOutOfRange_IndexException()
		{
			throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.index, "Index was out of range. Must be non-negative and less than the size of the collection.");
		}

		internal static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException()
		{
			throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.index, "Non-negative number required.");
		}

		internal static void ThrowArgumentException_Argument_InvalidArrayType()
		{
			throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
		}

		private static ArgumentException GetAddingDuplicateWithKeyArgumentException(object key)
		{
			return new ArgumentException(SR.Format("An item with the same key has already been added. Key: {0}", key));
		}

		internal static void ThrowAddingDuplicateWithKeyArgumentException(object key)
		{
			throw ThrowHelper.GetAddingDuplicateWithKeyArgumentException(key);
		}

		private static KeyNotFoundException GetKeyNotFoundException(object key)
		{
			throw new KeyNotFoundException(SR.Format("The given key '{0}' was not present in the dictionary.", key.ToString()));
		}

		internal static void ThrowKeyNotFoundException(object key)
		{
			throw ThrowHelper.GetKeyNotFoundException(key);
		}

		internal static void ThrowInvalidTypeWithPointersNotSupported(Type targetType)
		{
			throw new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", targetType));
		}

		internal static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported()
		{
			throw ThrowHelper.GetInvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
		}

		internal static InvalidOperationException GetInvalidOperationException(string str)
		{
			return new InvalidOperationException(str);
		}

		internal static void ThrowArraySegmentCtorValidationFailedExceptions(Array array, int offset, int count)
		{
			throw ThrowHelper.GetArraySegmentCtorValidationFailedException(array, offset, count);
		}

		private static Exception GetArraySegmentCtorValidationFailedException(Array array, int offset, int count)
		{
			if (array == null)
			{
				return ThrowHelper.GetArgumentNullException(ExceptionArgument.array);
			}
			if (offset < 0)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count < 0)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
			}
			return ThrowHelper.GetArgumentException(ExceptionResource.Argument_InvalidOffLen);
		}

		private static ArgumentException GetArgumentException(ExceptionResource resource)
		{
			return new ArgumentException(resource.ToString());
		}

		private static ArgumentNullException GetArgumentNullException(ExceptionArgument argument)
		{
			return new ArgumentNullException(ThrowHelper.GetArgumentName(argument));
		}

		internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, ExceptionArgument argName)
		{
			if (value == null && default(T) != null)
			{
				ThrowHelper.ThrowArgumentNullException(argName);
			}
		}

		internal static string GetArgumentName(ExceptionArgument argument)
		{
			string result;
			switch (argument)
			{
			case ExceptionArgument.obj:
				result = "obj";
				break;
			case ExceptionArgument.dictionary:
				result = "dictionary";
				break;
			case ExceptionArgument.dictionaryCreationThreshold:
				result = "dictionaryCreationThreshold";
				break;
			case ExceptionArgument.array:
				result = "array";
				break;
			case ExceptionArgument.info:
				result = "info";
				break;
			case ExceptionArgument.key:
				result = "key";
				break;
			case ExceptionArgument.collection:
				result = "collection";
				break;
			case ExceptionArgument.list:
				result = "list";
				break;
			case ExceptionArgument.match:
				result = "match";
				break;
			case ExceptionArgument.converter:
				result = "converter";
				break;
			case ExceptionArgument.queue:
				result = "queue";
				break;
			case ExceptionArgument.stack:
				result = "stack";
				break;
			case ExceptionArgument.capacity:
				result = "capacity";
				break;
			case ExceptionArgument.index:
				result = "index";
				break;
			case ExceptionArgument.startIndex:
				result = "startIndex";
				break;
			case ExceptionArgument.value:
				result = "value";
				break;
			case ExceptionArgument.count:
				result = "count";
				break;
			case ExceptionArgument.arrayIndex:
				result = "arrayIndex";
				break;
			case ExceptionArgument.name:
				result = "name";
				break;
			case ExceptionArgument.mode:
				result = "mode";
				break;
			case ExceptionArgument.item:
				result = "item";
				break;
			case ExceptionArgument.options:
				result = "options";
				break;
			case ExceptionArgument.view:
				result = "view";
				break;
			case ExceptionArgument.sourceBytesToCopy:
				result = "sourceBytesToCopy";
				break;
			default:
				return string.Empty;
			}
			return result;
		}

		private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
		{
			return new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), resource.ToString());
		}

		internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index()
		{
			throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
		}

		internal static void ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count()
		{
			throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
		}

		internal static string GetResourceName(ExceptionResource resource)
		{
			string result;
			switch (resource)
			{
			case ExceptionResource.Argument_ImplementIComparable:
				result = "At least one object must implement IComparable.";
				break;
			case ExceptionResource.Argument_InvalidType:
				result = "The type of arguments passed into generic comparer methods is invalid.";
				break;
			case ExceptionResource.Argument_InvalidArgumentForComparison:
				result = "Type of argument is not compatible with the generic comparer.";
				break;
			case ExceptionResource.Argument_InvalidRegistryKeyPermissionCheck:
				result = "The specified RegistryKeyPermissionCheck value is invalid.";
				break;
			case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
				result = "Non-negative number required.";
				break;
			case ExceptionResource.Arg_ArrayPlusOffTooSmall:
				result = "Destination array is not long enough to copy all the items in the collection. Check array index and length.";
				break;
			case ExceptionResource.Arg_NonZeroLowerBound:
				result = "The lower bound of target array must be zero.";
				break;
			case ExceptionResource.Arg_RankMultiDimNotSupported:
				result = "Only single dimensional arrays are supported for the requested action.";
				break;
			case ExceptionResource.Arg_RegKeyDelHive:
				result = "Cannot delete a registry hive's subtree.";
				break;
			case ExceptionResource.Arg_RegKeyStrLenBug:
				result = "Registry key names should not be greater than 255 characters.";
				break;
			case ExceptionResource.Arg_RegSetStrArrNull:
				result = "RegistryKey.SetValue does not allow a String[] that contains a null String reference.";
				break;
			case ExceptionResource.Arg_RegSetMismatchedKind:
				result = "The type of the value object did not match the specified RegistryValueKind or the object could not be properly converted.";
				break;
			case ExceptionResource.Arg_RegSubKeyAbsent:
				result = "Cannot delete a subkey tree because the subkey does not exist.";
				break;
			case ExceptionResource.Arg_RegSubKeyValueAbsent:
				result = "No value exists with that name.";
				break;
			case ExceptionResource.Argument_AddingDuplicate:
				result = "An item with the same key has already been added.";
				break;
			case ExceptionResource.Serialization_InvalidOnDeser:
				result = "OnDeserialization method was called while the object was not being deserialized.";
				break;
			case ExceptionResource.Serialization_MissingKeys:
				result = "The Keys for this Hashtable are missing.";
				break;
			case ExceptionResource.Serialization_NullKey:
				result = "One of the serialized keys is null.";
				break;
			case ExceptionResource.Argument_InvalidArrayType:
				result = "Target array type is not compatible with the type of items in the collection.";
				break;
			case ExceptionResource.NotSupported_KeyCollectionSet:
				result = "Mutating a key collection derived from a dictionary is not allowed.";
				break;
			case ExceptionResource.NotSupported_ValueCollectionSet:
				result = "Mutating a value collection derived from a dictionary is not allowed.";
				break;
			case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
				result = "capacity was less than the current size.";
				break;
			case ExceptionResource.ArgumentOutOfRange_Index:
				result = "Index was out of range. Must be non-negative and less than the size of the collection.";
				break;
			case ExceptionResource.Argument_InvalidOffLen:
				result = "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.";
				break;
			case ExceptionResource.Argument_ItemNotExist:
				result = "The specified item does not exist in this KeyedCollection.";
				break;
			case ExceptionResource.ArgumentOutOfRange_Count:
				result = "Count must be positive and count must refer to a location within the string/array/collection.";
				break;
			case ExceptionResource.ArgumentOutOfRange_InvalidThreshold:
				result = "The specified threshold for creating dictionary is out of range.";
				break;
			case ExceptionResource.ArgumentOutOfRange_ListInsert:
				result = "Index must be within the bounds of the List.";
				break;
			case ExceptionResource.NotSupported_ReadOnlyCollection:
				result = "Collection is read-only.";
				break;
			case ExceptionResource.InvalidOperation_CannotRemoveFromStackOrQueue:
				result = "Removal is an invalid operation for Stack or Queue.";
				break;
			case ExceptionResource.InvalidOperation_EmptyQueue:
				result = "Queue empty.";
				break;
			case ExceptionResource.InvalidOperation_EnumOpCantHappen:
				result = "Enumeration has either not started or has already finished.";
				break;
			case ExceptionResource.InvalidOperation_EnumFailedVersion:
				result = "Collection was modified; enumeration operation may not execute.";
				break;
			case ExceptionResource.InvalidOperation_EmptyStack:
				result = "Stack empty.";
				break;
			case ExceptionResource.ArgumentOutOfRange_BiggerThanCollection:
				result = "Larger than collection size.";
				break;
			case ExceptionResource.InvalidOperation_EnumNotStarted:
				result = "Enumeration has not started. Call MoveNext.";
				break;
			case ExceptionResource.InvalidOperation_EnumEnded:
				result = "Enumeration already finished.";
				break;
			case ExceptionResource.NotSupported_SortedListNestedWrite:
				result = "This operation is not supported on SortedList nested types because they require modifying the original SortedList.";
				break;
			case ExceptionResource.InvalidOperation_NoValue:
				result = "Nullable object must have a value.";
				break;
			case ExceptionResource.InvalidOperation_RegRemoveSubKey:
				result = "Registry key has subkeys and recursive removes are not supported by this method.";
				break;
			case ExceptionResource.Security_RegistryPermission:
				result = "Requested registry access is not allowed.";
				break;
			case ExceptionResource.UnauthorizedAccess_RegistryNoWrite:
				result = "Cannot write to the registry key.";
				break;
			case ExceptionResource.ObjectDisposed_RegKeyClosed:
				result = "Cannot access a closed registry key.";
				break;
			case ExceptionResource.NotSupported_InComparableType:
				result = "A type must implement IComparable<T> or IComparable to support comparison.";
				break;
			case ExceptionResource.Argument_InvalidRegistryOptionsCheck:
				result = "The specified RegistryOptions value is invalid.";
				break;
			case ExceptionResource.Argument_InvalidRegistryViewCheck:
				result = "The specified RegistryView value is invalid.";
				break;
			default:
				return string.Empty;
			}
			return result;
		}

		internal static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
		{
			throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
	}
}
