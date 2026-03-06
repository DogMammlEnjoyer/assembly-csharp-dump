using System;
using Unity.Properties;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class SetValueVisitor<TSrcValue> : PathVisitor
	{
		public ConverterGroup group { get; set; }

		public override void Reset()
		{
			base.Reset();
			this.Value = default(TSrcValue);
			this.group = null;
		}

		protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
		{
			bool isReadOnly = property.IsReadOnly;
			if (isReadOnly)
			{
				base.ReturnCode = VisitReturnCode.AccessViolation;
			}
			else
			{
				TValue value2;
				bool flag = this.group != null && this.group.TryConvert<TSrcValue, TValue>(ref this.Value, out value2);
				if (flag)
				{
					property.SetValue(ref container, value2);
				}
				else
				{
					TValue value3;
					bool flag2 = ConverterGroups.TryConvert<TSrcValue, TValue>(ref this.Value, out value3);
					if (flag2)
					{
						property.SetValue(ref container, value3);
					}
					else
					{
						base.ReturnCode = VisitReturnCode.InvalidCast;
					}
				}
			}
		}

		public static readonly ObjectPool<SetValueVisitor<TSrcValue>> Pool = new ObjectPool<SetValueVisitor<TSrcValue>>(() => new SetValueVisitor<TSrcValue>(), delegate(SetValueVisitor<TSrcValue> v)
		{
			v.Reset();
		}, null, null, true, 10, 10000);

		public TSrcValue Value;
	}
}
