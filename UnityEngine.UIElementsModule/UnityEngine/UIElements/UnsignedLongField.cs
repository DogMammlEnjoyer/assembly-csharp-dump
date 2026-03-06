using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class UnsignedLongField : TextValueField<ulong>
	{
		private UnsignedLongField.UnsignedLongInput unsignedLongInput
		{
			get
			{
				return (UnsignedLongField.UnsignedLongInput)base.textInputBase;
			}
		}

		protected override string ValueToString(ulong v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override ulong StringToValue(string str)
		{
			ulong num;
			ExpressionEvaluator.Expression obj;
			bool flag = UINumericFieldsUtils.TryConvertStringToULong(str, base.textInputBase.originalText, out num, out obj);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(obj);
			}
			return flag ? num : base.rawValue;
		}

		public UnsignedLongField() : this(null, 1000)
		{
		}

		public UnsignedLongField(int maxLength) : this(null, maxLength)
		{
		}

		public UnsignedLongField(string label, int maxLength = 1000) : base(label, maxLength, new UnsignedLongField.UnsignedLongInput())
		{
			base.AddToClassList(UnsignedLongField.ussClassName);
			base.labelElement.AddToClassList(UnsignedLongField.labelUssClassName);
			base.visualInput.AddToClassList(UnsignedLongField.inputUssClassName);
			base.AddLabelDragger<ulong>();
		}

		internal override bool CanTryParse(string textString)
		{
			ulong num;
			return ulong.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, ulong startValue)
		{
			this.unsignedLongInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-unsigned-long-field";

		public new static readonly string labelUssClassName = UnsignedLongField.ussClassName + "__label";

		public new static readonly string inputUssClassName = UnsignedLongField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextInputBaseField<ulong>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextInputBaseField<ulong>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new UnsignedLongField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<UnsignedLongField, UnsignedLongField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextValueFieldTraits<ulong, UxmlUnsignedLongAttributeDescription>
		{
		}

		private class UnsignedLongInput : TextValueField<ulong>.TextValueInput
		{
			private UnsignedLongField parentUnsignedLongField
			{
				get
				{
					return (UnsignedLongField)base.parent;
				}
			}

			internal UnsignedLongInput()
			{
				base.formatString = UINumericFieldsUtils.k_IntFieldFormatString;
			}

			protected override string allowedCharacters
			{
				get
				{
					return UINumericFieldsUtils.k_AllowedCharactersForInt;
				}
			}

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, ulong startValue)
			{
				double num = NumericFieldDraggerUtility.CalculateIntDragSensitivity(startValue);
				float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				ulong value = this.StringToValue(base.text);
				long niceDelta = (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num);
				value = this.ClampToMinMaxULongValue(niceDelta, value);
				bool isDelayed = this.parentUnsignedLongField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(value);
				}
				else
				{
					this.parentUnsignedLongField.value = value;
				}
			}

			private ulong ClampToMinMaxULongValue(long niceDelta, ulong value)
			{
				ulong num = (ulong)Math.Abs(niceDelta);
				bool flag = niceDelta > 0L;
				ulong result;
				if (flag)
				{
					bool flag2 = num > ulong.MaxValue - value;
					if (flag2)
					{
						result = ulong.MaxValue;
					}
					else
					{
						result = value + num;
					}
				}
				else
				{
					bool flag3 = num > value;
					if (flag3)
					{
						result = 0UL;
					}
					else
					{
						result = value - num;
					}
				}
				return result;
			}

			protected override string ValueToString(ulong v)
			{
				return v.ToString(base.formatString);
			}

			protected override ulong StringToValue(string str)
			{
				return this.parentUnsignedLongField.StringToValue(str);
			}
		}
	}
}
