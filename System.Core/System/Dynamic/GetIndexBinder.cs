using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	/// <summary>Represents the dynamic get index operation at the call site, providing the binding semantic and the details about the operation.</summary>
	public abstract class GetIndexBinder : DynamicMetaObjectBinder
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.GetIndexBinder" />.</summary>
		/// <param name="callInfo">The signature of the arguments at the call site.</param>
		protected GetIndexBinder(CallInfo callInfo)
		{
			ContractUtils.RequiresNotNull(callInfo, "callInfo");
			this.CallInfo = callInfo;
		}

		/// <summary>The result type of the operation.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the result type of the operation.</returns>
		public sealed override Type ReturnType
		{
			get
			{
				return typeof(object);
			}
		}

		/// <summary>Gets the signature of the arguments at the call site.</summary>
		/// <returns>The signature of the arguments at the call site.</returns>
		public CallInfo CallInfo { get; }

		/// <summary>Performs the binding of the dynamic get index operation.</summary>
		/// <param name="target">The target of the dynamic get index operation.</param>
		/// <param name="args">An array of arguments of the dynamic get index operation.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNullItems<DynamicMetaObject>(args, "args");
			return target.BindGetIndex(this, args);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		/// <summary>Performs the binding of the dynamic get index operation if the target dynamic object cannot bind.</summary>
		/// <param name="target">The target of the dynamic get index operation.</param>
		/// <param name="indexes">The arguments of the dynamic get index operation.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes)
		{
			return this.FallbackGetIndex(target, indexes, null);
		}

		/// <summary>When overridden in the derived class, performs the binding of the dynamic get index operation if the target dynamic object cannot bind.</summary>
		/// <param name="target">The target of the dynamic get index operation.</param>
		/// <param name="indexes">The arguments of the dynamic get index operation.</param>
		/// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public abstract DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion);
	}
}
