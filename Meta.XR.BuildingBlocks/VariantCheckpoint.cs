using System;
using UnityEngine;

namespace Meta.XR.BuildingBlocks
{
	[Serializable]
	public class VariantCheckpoint
	{
		public string MemberName
		{
			get
			{
				return this._memberName;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
		}

		public VariantCheckpoint(string memberName, string value)
		{
			this._memberName = memberName;
			this._value = value;
		}

		[SerializeField]
		protected string _memberName;

		[SerializeField]
		protected string _value;
	}
}
