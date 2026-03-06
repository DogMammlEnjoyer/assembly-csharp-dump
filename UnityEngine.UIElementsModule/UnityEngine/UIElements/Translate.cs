using System;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public struct Translate : IEquatable<Translate>
	{
		public Translate(Length x, Length y, float z)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = z;
			this.m_isNone = false;
		}

		public Translate(Length x, Length y)
		{
			this = new Translate(x, y, 0f);
		}

		internal Translate(Vector3 v)
		{
			this = new Translate(v.x, v.y, v.z);
		}

		public static implicit operator Translate(Vector3 v)
		{
			return new Translate(v);
		}

		public static implicit operator Translate(Vector2 v)
		{
			return new Translate(v);
		}

		public static Translate None()
		{
			return new Translate
			{
				m_isNone = true
			};
		}

		public Length x
		{
			get
			{
				return this.m_X;
			}
			set
			{
				this.m_X = value;
			}
		}

		public Length y
		{
			get
			{
				return this.m_Y;
			}
			set
			{
				this.m_Y = value;
			}
		}

		public float z
		{
			get
			{
				return this.m_Z;
			}
			set
			{
				this.m_Z = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool IsNone()
		{
			return this.m_isNone;
		}

		public static bool operator ==(Translate lhs, Translate rhs)
		{
			return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y && lhs.m_Z == rhs.m_Z && lhs.m_isNone == rhs.m_isNone;
		}

		public static bool operator !=(Translate lhs, Translate rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Translate other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Translate)
			{
				Translate other = (Translate)obj;
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
			return this.m_X.GetHashCode() * 793 ^ this.m_Y.GetHashCode() * 791 ^ this.m_Z.GetHashCode() * 571;
		}

		public override string ToString()
		{
			string text = this.m_Z.ToString(CultureInfo.InvariantCulture.NumberFormat);
			return string.Concat(new string[]
			{
				this.m_X.ToString(),
				" ",
				this.m_Y.ToString(),
				" ",
				text
			});
		}

		private Length m_X;

		private Length m_Y;

		private float m_Z;

		private bool m_isNone;

		internal class PropertyBag : ContainerPropertyBag<Translate>
		{
			public PropertyBag()
			{
				base.AddProperty<Length>(new Translate.PropertyBag.XProperty());
				base.AddProperty<Length>(new Translate.PropertyBag.YProperty());
				base.AddProperty<float>(new Translate.PropertyBag.ZProperty());
			}

			private class XProperty : Property<Translate, Length>
			{
				public override string Name { get; } = "x";

				public override bool IsReadOnly { get; } = 0;

				public override Length GetValue(ref Translate container)
				{
					return container.x;
				}

				public override void SetValue(ref Translate container, Length value)
				{
					container.x = value;
				}
			}

			private class YProperty : Property<Translate, Length>
			{
				public override string Name { get; } = "y";

				public override bool IsReadOnly { get; } = 0;

				public override Length GetValue(ref Translate container)
				{
					return container.y;
				}

				public override void SetValue(ref Translate container, Length value)
				{
					container.y = value;
				}
			}

			private class ZProperty : Property<Translate, float>
			{
				public override string Name { get; } = "z";

				public override bool IsReadOnly { get; } = 0;

				public override float GetValue(ref Translate container)
				{
					return container.z;
				}

				public override void SetValue(ref Translate container, float value)
				{
					container.z = value;
				}
			}
		}
	}
}
