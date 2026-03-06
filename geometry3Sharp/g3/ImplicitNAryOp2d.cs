using System;
using System.Collections.Generic;

namespace g3
{
	public abstract class ImplicitNAryOp2d : ImplicitOperator2d, ImplicitField2d
	{
		public ImplicitNAryOp2d()
		{
			this.m_vChildren = new List<ImplicitField2d>();
		}

		public void AddChild(ImplicitField2d pField)
		{
			this.m_vChildren.Add(pField);
		}

		public virtual float Value(float fX, float fY)
		{
			return 0f;
		}

		public virtual void Gradient(float fX, float fY, ref float fGX, ref float fGY)
		{
			float num = this.Value(fX, fY);
			fGX = (this.Value(fX + 0.001f, fY) - num) / 0.001f;
			fGY = (this.Value(fX, fY + 0.001f) - num) / 0.001f;
		}

		public virtual AxisAlignedBox2f Bounds
		{
			get
			{
				AxisAlignedBox2f result = default(AxisAlignedBox2f);
				for (int i = 0; i < this.m_vChildren.Count; i++)
				{
					result.Contain(this.m_vChildren[i].Bounds);
				}
				return result;
			}
		}

		protected List<ImplicitField2d> m_vChildren;
	}
}
