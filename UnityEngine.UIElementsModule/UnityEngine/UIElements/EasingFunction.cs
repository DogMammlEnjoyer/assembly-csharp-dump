using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public struct EasingFunction : IEquatable<EasingFunction>
	{
		public EasingMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		public EasingFunction(EasingMode mode)
		{
			this.m_Mode = mode;
		}

		public static implicit operator EasingFunction(EasingMode easingMode)
		{
			return new EasingFunction(easingMode);
		}

		public static bool operator ==(EasingFunction lhs, EasingFunction rhs)
		{
			return lhs.m_Mode == rhs.m_Mode;
		}

		public static bool operator !=(EasingFunction lhs, EasingFunction rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(EasingFunction other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is EasingFunction)
			{
				EasingFunction other = (EasingFunction)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override string ToString()
		{
			return this.m_Mode.ToString();
		}

		public override int GetHashCode()
		{
			return (int)this.m_Mode;
		}

		private EasingMode m_Mode;

		internal class PropertyBag : ContainerPropertyBag<EasingFunction>
		{
			public PropertyBag()
			{
				base.AddProperty<EasingMode>(new EasingFunction.PropertyBag.ModeProperty());
			}

			private class ModeProperty : Property<EasingFunction, EasingMode>
			{
				public override string Name { get; } = "mode";

				public override bool IsReadOnly { get; } = 0;

				public override EasingMode GetValue(ref EasingFunction container)
				{
					return container.mode;
				}

				public override void SetValue(ref EasingFunction container, EasingMode value)
				{
					container.mode = value;
				}
			}
		}
	}
}
