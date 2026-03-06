using System;
using UnityEngine;

namespace Unity.XR.CoreUtils.Datums
{
	[Serializable]
	public abstract class DatumProperty<TValue, TDatum> where TDatum : Datum<TValue>
	{
		protected DatumProperty()
		{
			this.m_UseConstant = false;
		}

		protected DatumProperty(TValue value)
		{
			this.m_UseConstant = true;
			this.m_ConstantValue = value;
		}

		protected DatumProperty(TDatum datum)
		{
			this.m_UseConstant = false;
			this.m_Variable = datum;
		}

		public TValue Value
		{
			get
			{
				if (this.m_UseConstant)
				{
					return this.m_ConstantValue;
				}
				if (!(this.Datum != null))
				{
					return default(TValue);
				}
				return this.Datum.Value;
			}
			set
			{
				if (this.m_UseConstant)
				{
					this.m_ConstantValue = value;
					return;
				}
				this.Datum.Value = value;
			}
		}

		protected Datum<TValue> Datum
		{
			get
			{
				return this.m_Variable;
			}
		}

		protected TValue ConstantValue
		{
			get
			{
				return this.m_ConstantValue;
			}
		}

		public static implicit operator TValue(DatumProperty<TValue, TDatum> datumProperty)
		{
			return datumProperty.Value;
		}

		[SerializeField]
		private bool m_UseConstant;

		[SerializeField]
		private TValue m_ConstantValue;

		[SerializeField]
		private TDatum m_Variable;
	}
}
