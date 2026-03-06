using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace System.Dynamic
{
	/// <summary>Describes arguments in the dynamic binding process.</summary>
	public sealed class CallInfo
	{
		/// <summary>Creates a new PositionalArgumentInfo.</summary>
		/// <param name="argCount">The number of arguments.</param>
		/// <param name="argNames">The argument names.</param>
		public CallInfo(int argCount, params string[] argNames) : this(argCount, argNames)
		{
		}

		/// <summary>Creates a new CallInfo that represents arguments in the dynamic binding process.</summary>
		/// <param name="argCount">The number of arguments.</param>
		/// <param name="argNames">The argument names.</param>
		public CallInfo(int argCount, IEnumerable<string> argNames)
		{
			ContractUtils.RequiresNotNull(argNames, "argNames");
			ReadOnlyCollection<string> readOnlyCollection = argNames.ToReadOnly<string>();
			if (argCount < readOnlyCollection.Count)
			{
				throw Error.ArgCntMustBeGreaterThanNameCnt();
			}
			ContractUtils.RequiresNotNullItems<string>(readOnlyCollection, "argNames");
			this.ArgumentCount = argCount;
			this.ArgumentNames = readOnlyCollection;
		}

		/// <summary>The number of arguments.</summary>
		/// <returns>The number of arguments.</returns>
		public int ArgumentCount { get; }

		/// <summary>The argument names.</summary>
		/// <returns>The read-only collection of argument names.</returns>
		public ReadOnlyCollection<string> ArgumentNames { get; }

		/// <summary>Serves as a hash function for the current <see cref="T:System.Dynamic.CallInfo" />.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Dynamic.CallInfo" />.</returns>
		public override int GetHashCode()
		{
			return this.ArgumentCount ^ this.ArgumentNames.ListHashCode<string>();
		}

		/// <summary>Determines whether the specified CallInfo instance is considered equal to the current.</summary>
		/// <param name="obj">The instance of <see cref="T:System.Dynamic.CallInfo" /> to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current one otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			CallInfo callInfo = obj as CallInfo;
			return callInfo != null && this.ArgumentCount == callInfo.ArgumentCount && this.ArgumentNames.ListEquals(callInfo.ArgumentNames);
		}
	}
}
