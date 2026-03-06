using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	/// <summary>Represents the convert dynamic operation at the call site, providing the binding semantic and the details about the operation.</summary>
	public abstract class ConvertBinder : DynamicMetaObjectBinder
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.ConvertBinder" />.</summary>
		/// <param name="type">The type to convert to.</param>
		/// <param name="explicit">Is true if the conversion should consider explicit conversions; otherwise, false.</param>
		protected ConvertBinder(Type type, bool @explicit)
		{
			ContractUtils.RequiresNotNull(type, "type");
			this.Type = type;
			this.Explicit = @explicit;
		}

		/// <summary>The type to convert to.</summary>
		/// <returns>The <see cref="T:System.Type" /> object that represents the type to convert to.</returns>
		public Type Type { get; }

		/// <summary>Gets the value indicating if the conversion should consider explicit conversions.</summary>
		/// <returns>
		///     <see langword="True" /> if there is an explicit conversion, otherwise <see langword="false" />.</returns>
		public bool Explicit { get; }

		/// <summary>Performs the binding of the dynamic convert operation if the target dynamic object cannot bind.</summary>
		/// <param name="target">The target of the dynamic convert operation.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public DynamicMetaObject FallbackConvert(DynamicMetaObject target)
		{
			return this.FallbackConvert(target, null);
		}

		/// <summary>When overridden in the derived class, performs the binding of the dynamic convert operation if the target dynamic object cannot bind.</summary>
		/// <param name="target">The target of the dynamic convert operation.</param>
		/// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public abstract DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

		/// <summary>Performs the binding of the dynamic convert operation.</summary>
		/// <param name="target">The target of the dynamic convert operation.</param>
		/// <param name="args">An array of arguments of the dynamic convert operation.</param>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.Requires(args == null || args.Length == 0, "args");
			return target.BindConvert(this);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		/// <summary>The result type of the operation.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the result type of the operation.</returns>
		public sealed override Type ReturnType
		{
			get
			{
				return this.Type;
			}
		}
	}
}
