using System;

namespace System.Diagnostics.Contracts
{
	/// <summary>Enables you to set contract and tool options at assembly, type, or method granularity.</summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[Conditional("CONTRACTS_FULL")]
	public sealed class ContractOptionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Contracts.ContractOptionAttribute" /> class by using the provided category, setting, and enable/disable value.</summary>
		/// <param name="category">The category for the option to be set.</param>
		/// <param name="setting">The option setting.</param>
		/// <param name="enabled">
		///   <see langword="true" /> to enable the option; <see langword="false" /> to disable the option.</param>
		public ContractOptionAttribute(string category, string setting, bool enabled)
		{
			this._category = category;
			this._setting = setting;
			this._enabled = enabled;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Contracts.ContractOptionAttribute" /> class by using the provided category, setting, and value.</summary>
		/// <param name="category">The category of the option to be set.</param>
		/// <param name="setting">The option setting.</param>
		/// <param name="value">The value for the setting.</param>
		public ContractOptionAttribute(string category, string setting, string value)
		{
			this._category = category;
			this._setting = setting;
			this._value = value;
		}

		/// <summary>Gets the category of the option.</summary>
		/// <returns>The category of the option.</returns>
		public string Category
		{
			get
			{
				return this._category;
			}
		}

		/// <summary>Gets the setting for the option.</summary>
		/// <returns>The setting for the option.</returns>
		public string Setting
		{
			get
			{
				return this._setting;
			}
		}

		/// <summary>Determines if an option is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if the option is enabled; otherwise, <see langword="false" />.</returns>
		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
		}

		/// <summary>Gets the value for the option.</summary>
		/// <returns>The value for the option.</returns>
		public string Value
		{
			get
			{
				return this._value;
			}
		}

		private string _category;

		private string _setting;

		private bool _enabled;

		private string _value;
	}
}
