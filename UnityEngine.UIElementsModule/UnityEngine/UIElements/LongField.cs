using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class LongField : TextValueField<long>
	{
		private LongField.LongInput longInput
		{
			get
			{
				return (LongField.LongInput)base.textInputBase;
			}
		}

		protected override string ValueToString(long v)
		{
			return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected override long StringToValue(string str)
		{
			long num;
			ExpressionEvaluator.Expression obj;
			bool flag = UINumericFieldsUtils.TryConvertStringToLong(str, base.textInputBase.originalText, out num, out obj);
			Action<ExpressionEvaluator.Expression> expressionEvaluated = this.expressionEvaluated;
			if (expressionEvaluated != null)
			{
				expressionEvaluated(obj);
			}
			return flag ? num : base.rawValue;
		}

		public LongField() : this(null, 1000)
		{
		}

		public LongField(int maxLength) : this(null, maxLength)
		{
		}

		public LongField(string label, int maxLength = 1000) : base(label, maxLength, new LongField.LongInput())
		{
			base.AddToClassList(LongField.ussClassName);
			base.labelElement.AddToClassList(LongField.labelUssClassName);
			base.visualInput.AddToClassList(LongField.inputUssClassName);
			base.AddLabelDragger<long>();
		}

		internal override bool CanTryParse(string textString)
		{
			long num;
			return long.TryParse(textString, out num);
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue)
		{
			this.longInput.ApplyInputDeviceDelta(delta, speed, startValue);
		}

		public new static readonly string ussClassName = "unity-long-field";

		public new static readonly string labelUssClassName = LongField.ussClassName + "__label";

		public new static readonly string inputUssClassName = LongField.ussClassName + "__input";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextInputBaseField<long>.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				TextInputBaseField<long>.UxmlSerializedData.Register();
			}

			public override object CreateInstance()
			{
				return new LongField();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<LongField, LongField.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextValueFieldTraits<long, UxmlLongAttributeDescription>
		{
		}

		private class LongInput : TextValueField<long>.TextValueInput
		{
			private LongField parentLongField
			{
				get
				{
					return (LongField)base.parent;
				}
			}

			internal LongInput()
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

			public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue)
			{
				double num = (double)NumericFieldDraggerUtility.CalculateIntDragSensitivity(startValue);
				float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
				long value = this.StringToValue(base.text);
				long niceDelta = (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num);
				value = this.ClampMinMaxLongValue(niceDelta, value);
				bool isDelayed = this.parentLongField.isDelayed;
				if (isDelayed)
				{
					base.text = this.ValueToString(value);
				}
				else
				{
					this.parentLongField.value = value;
				}
			}

			private long ClampMinMaxLongValue(long niceDelta, long value)
			{
				long num = Math.Abs(niceDelta);
				bool flag = niceDelta > 0L;
				long result;
				if (flag)
				{
					bool flag2 = value > 0L && num > long.MaxValue - value;
					if (flag2)
					{
						result = long.MaxValue;
					}
					else
					{
						result = value + niceDelta;
					}
				}
				else
				{
					bool flag3 = value < 0L && value < long.MinValue + num;
					if (flag3)
					{
						result = long.MinValue;
					}
					else
					{
						result = value - num;
					}
				}
				return result;
			}

			protected override string ValueToString(long v)
			{
				return v.ToString(base.formatString);
			}

			protected override long StringToValue(string str)
			{
				return this.parentLongField.StringToValue(str);
			}
		}
	}
}
