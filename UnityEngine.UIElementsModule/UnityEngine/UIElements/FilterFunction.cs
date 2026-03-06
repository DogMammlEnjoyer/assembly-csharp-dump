using System;
using System.Text;
using Unity.Properties;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal struct FilterFunction : IEquatable<FilterFunction>
	{
		public FilterFunctionType type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		internal FixedBuffer4<FilterParameter> parameters
		{
			get
			{
				return this.m_Parameters;
			}
			set
			{
				this.m_Parameters = value;
			}
		}

		public int parameterCount
		{
			get
			{
				return this.m_ParameterCount;
			}
		}

		public FilterFunctionDefinition customDefinition
		{
			get
			{
				return this.m_CustomDefinition;
			}
			set
			{
				this.m_CustomDefinition = value;
			}
		}

		public unsafe void AddParameter(FilterParameter p)
		{
			int parameterCount = this.m_ParameterCount;
			bool flag = parameterCount >= 4;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("FilterFunction.AddParameter only support up to {0} parameters", 4));
			}
			*this.m_Parameters[parameterCount] = p;
			this.m_ParameterCount++;
		}

		public unsafe void SetParameter(int index, FilterParameter p)
		{
			bool flag = index < 0 || index >= this.m_ParameterCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("FilterFunction.SetParameter index out of range");
			}
			*this.m_Parameters[index] = p;
		}

		public unsafe FilterParameter GetParameter(int index)
		{
			bool flag = index < 0 || index >= this.parameterCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return *this.m_Parameters[index];
		}

		public void ClearParameters()
		{
			this.m_ParameterCount = 0;
		}

		public FilterFunction(FilterFunctionType type)
		{
			this.m_ParameterCount = 0;
			this.m_Type = type;
			this.m_CustomDefinition = null;
			this.m_Parameters = default(FixedBuffer4<FilterParameter>);
		}

		public FilterFunction(FilterFunctionDefinition filterDef)
		{
			this.m_ParameterCount = 0;
			bool flag = filterDef == null;
			if (flag)
			{
				throw new ArgumentNullException("filterDef");
			}
			this.m_Type = FilterFunctionType.Custom;
			this.m_CustomDefinition = filterDef;
			this.m_Parameters = default(FixedBuffer4<FilterParameter>);
		}

		internal FilterFunction(FilterFunctionType type, FixedBuffer4<FilterParameter> parameters, int paramCount)
		{
			this.m_ParameterCount = 0;
			this.m_Type = type;
			this.m_CustomDefinition = null;
			this.m_Parameters = parameters;
			this.m_ParameterCount = paramCount;
			FilterFunctionDefinition definition = this.GetDefinition();
			bool flag = definition != null;
			if (flag)
			{
				int num = this.GetDefinition().parameters.Length;
				bool flag2 = num != paramCount;
				if (flag2)
				{
					Debug.LogError(string.Format("Invalid parameter count provided with filter of type {0}: provided {1} but expected {2}", type, paramCount, num));
				}
			}
		}

		internal FilterFunction(FilterFunctionDefinition customDefinition, FixedBuffer4<FilterParameter> parameters, int paramCount)
		{
			this.m_ParameterCount = 0;
			this.m_Type = FilterFunctionType.Custom;
			this.m_CustomDefinition = customDefinition;
			this.m_Parameters = parameters;
			this.m_ParameterCount = paramCount;
			bool flag = customDefinition != null;
			if (flag)
			{
				FilterParameterDeclaration[] parameters2 = customDefinition.parameters;
				int num = (parameters2 != null) ? parameters2.Length : 0;
				bool flag2 = num != paramCount;
				if (flag2)
				{
					Debug.LogError(string.Format("Invalid parameter count provided with custom filter function definition {0}: provided {1} but expected {2}", customDefinition, paramCount, num));
				}
			}
		}

		internal FilterFunctionDefinition GetDefinition()
		{
			bool flag = this.m_Type == FilterFunctionType.Custom;
			FilterFunctionDefinition result;
			if (flag)
			{
				result = this.m_CustomDefinition;
			}
			else
			{
				result = FilterFunctionDefinitionUtils.GetBuiltinDefinition(this.m_Type);
			}
			return result;
		}

		public unsafe static bool operator ==(FilterFunction lhs, FilterFunction rhs)
		{
			bool flag = lhs.m_CustomDefinition != rhs.m_CustomDefinition;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					bool flag2 = *lhs.m_Parameters[i] != *rhs.m_Parameters[i];
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public static bool operator !=(FilterFunction lhs, FilterFunction rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(FilterFunction other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is FilterFunction)
			{
				FilterFunction other = (FilterFunction)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.m_Parameters.GetHashCode() * 397 ^ ((this.m_CustomDefinition != null) ? this.m_CustomDefinition.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			FilterFunctionDefinition definition = this.GetDefinition();
			bool flag = !string.IsNullOrEmpty((definition != null) ? definition.filterName : null);
			if (flag)
			{
				stringBuilder.Append(definition.filterName);
			}
			else
			{
				bool flag2 = this.type == FilterFunctionType.Custom;
				if (flag2)
				{
					stringBuilder.Append("custom");
				}
				else
				{
					bool flag3 = this.type == FilterFunctionType.None;
					if (flag3)
					{
						stringBuilder.Append("none");
					}
				}
			}
			stringBuilder.Append("(");
			for (int i = 0; i < this.parameterCount; i++)
			{
				bool flag4 = i > 0;
				if (flag4)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(this.parameters[i].ToString());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		[SerializeField]
		private FilterFunctionType m_Type;

		[SerializeField]
		private FixedBuffer4<FilterParameter> m_Parameters;

		[SerializeField]
		private int m_ParameterCount;

		[SerializeField]
		private FilterFunctionDefinition m_CustomDefinition;

		internal class PropertyBag : ContainerPropertyBag<FilterFunction>
		{
			public PropertyBag()
			{
				base.AddProperty<FixedBuffer4<FilterParameter>>(new FilterFunction.PropertyBag.ParametersProperty());
				base.AddProperty<FilterFunctionDefinition>(new FilterFunction.PropertyBag.FilterFunctionDefinitionProperty());
			}

			private class ParametersProperty : Property<FilterFunction, FixedBuffer4<FilterParameter>>
			{
				public override string Name { get; } = "parameters";

				public override bool IsReadOnly { get; } = 0;

				public override FixedBuffer4<FilterParameter> GetValue(ref FilterFunction container)
				{
					return container.parameters;
				}

				public override void SetValue(ref FilterFunction container, FixedBuffer4<FilterParameter> value)
				{
					container.parameters = value;
				}
			}

			private class FilterFunctionDefinitionProperty : Property<FilterFunction, FilterFunctionDefinition>
			{
				public override string Name { get; } = "customDefinition";

				public override bool IsReadOnly { get; } = 0;

				public override FilterFunctionDefinition GetValue(ref FilterFunction container)
				{
					return container.customDefinition;
				}

				public override void SetValue(ref FilterFunction container, FilterFunctionDefinition value)
				{
					container.customDefinition = value;
				}
			}
		}
	}
}
