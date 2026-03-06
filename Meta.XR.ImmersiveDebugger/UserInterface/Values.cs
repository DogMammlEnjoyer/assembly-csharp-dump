using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	public class Values : Controller
	{
		internal List<Value> GetValues
		{
			get
			{
				return this._values;
			}
		}

		internal Watch Watch { get; private set; }

		internal ImageStyle BackgroundStyle
		{
			set
			{
				foreach (Value value2 in this._values)
				{
					value2.BackgroundStyle = value;
				}
			}
		}

		internal TextStyle TextStyle
		{
			set
			{
				foreach (Value value2 in this._values)
				{
					value2.TextStyle = value;
				}
			}
		}

		internal void Setup(Watch watch)
		{
			if (watch == this.Watch)
			{
				return;
			}
			this.Watch = watch;
			foreach (Value controller in this._values)
			{
				base.Owner.Remove(controller, true);
			}
			this._values.Clear();
			int num = 0;
			for (;;)
			{
				int num2 = num;
				Watch watch2 = this.Watch;
				int? num3 = (watch2 != null) ? new int?(watch2.NumberOfValues) : null;
				if (!(num2 < num3.GetValueOrDefault() & num3 != null))
				{
					break;
				}
				Value value = base.Owner.Append<Value>(string.Format("value {0}", num));
				value.LayoutStyle = Style.Instantiate<LayoutStyle>("MemberValueDynamic");
				value.TextStyle = Style.Load<TextStyle>("MemberValue");
				value.BackgroundStyle = Style.Load<ImageStyle>("MemberValueBackground");
				this._values.Add(value);
				num++;
			}
		}

		private void Update()
		{
			Watch watch = this.Watch;
			if (watch == null || !watch.Valid)
			{
				return;
			}
			string[] values = this.Watch.Values;
			int num = this.Watch.NumberOfValues;
			foreach (Value value in this._values)
			{
				value.Content = values[--num];
			}
		}

		protected readonly List<Value> _values = new List<Value>();
	}
}
