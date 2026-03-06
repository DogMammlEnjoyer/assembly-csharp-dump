using System;

namespace Fusion
{
	public abstract class DoIfAttributeBase : DecoratingPropertyAttribute
	{
		protected DoIfAttributeBase(string conditionMember, double compareToValue, CompareOperator compare)
		{
			this.ConditionMember = conditionMember;
			this.Compare = compare;
			this._doubleValue = compareToValue;
			this._isDouble = true;
		}

		protected DoIfAttributeBase(string conditionMember, long compareToValue, CompareOperator compare)
		{
			this.ConditionMember = conditionMember;
			this.Compare = compare;
			this._longValue = compareToValue;
			this._isDouble = false;
		}

		protected DoIfAttributeBase(string conditionMember, bool compareToValue, CompareOperator compare)
		{
			this.ConditionMember = conditionMember;
			this.Compare = compare;
			this._longValue = (compareToValue ? 1L : 0L);
			this._isDouble = false;
		}

		public double _doubleValue;

		public bool _isDouble;

		public long _longValue;

		public CompareOperator Compare;

		public string ConditionMember;

		public bool ErrorOnConditionMemberNotFound = true;
	}
}
