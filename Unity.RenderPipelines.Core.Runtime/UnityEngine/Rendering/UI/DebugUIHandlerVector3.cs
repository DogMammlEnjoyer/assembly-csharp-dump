using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerVector3 : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.Vector3Field>();
			this.m_Container = base.GetComponent<DebugUIHandlerContainer>();
			this.nameLabel.text = this.m_Field.displayName;
			this.fieldX.getter = (() => this.m_Field.GetValue().x);
			this.fieldX.setter = delegate(float v)
			{
				this.SetValue(v, true, false, false);
			};
			this.fieldX.nextUIHandler = this.fieldY;
			this.SetupSettings(this.fieldX);
			this.fieldY.getter = (() => this.m_Field.GetValue().y);
			this.fieldY.setter = delegate(float v)
			{
				this.SetValue(v, false, true, false);
			};
			this.fieldY.previousUIHandler = this.fieldX;
			this.fieldY.nextUIHandler = this.fieldZ;
			this.SetupSettings(this.fieldY);
			this.fieldZ.getter = (() => this.m_Field.GetValue().z);
			this.fieldZ.setter = delegate(float v)
			{
				this.SetValue(v, false, false, true);
			};
			this.fieldZ.previousUIHandler = this.fieldY;
			this.SetupSettings(this.fieldZ);
		}

		private void SetValue(float v, bool x = false, bool y = false, bool z = false)
		{
			Vector3 value = this.m_Field.GetValue();
			if (x)
			{
				value.x = v;
			}
			if (y)
			{
				value.y = v;
			}
			if (z)
			{
				value.z = v;
			}
			this.m_Field.SetValue(value);
		}

		private void SetupSettings(DebugUIHandlerIndirectFloatField field)
		{
			field.parentUIHandler = this;
			field.incStepGetter = (() => this.m_Field.incStep);
			field.incStepMultGetter = (() => this.m_Field.incStepMult);
			field.decimalsGetter = (() => (float)this.m_Field.decimals);
			field.Init();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			if (fromNext || !this.valueToggle.isOn)
			{
				this.nameLabel.color = this.colorSelected;
			}
			else if (this.valueToggle.isOn)
			{
				if (this.m_Container.IsDirectChild(previous))
				{
					this.nameLabel.color = this.colorSelected;
				}
				else
				{
					DebugUIHandlerWidget lastItem = this.m_Container.GetLastItem();
					DebugManager.instance.ChangeSelection(lastItem, false);
				}
			}
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
		}

		public override void OnIncrement(bool fast)
		{
			this.valueToggle.isOn = true;
		}

		public override void OnDecrement(bool fast)
		{
			this.valueToggle.isOn = false;
		}

		public override void OnAction()
		{
			this.valueToggle.isOn = !this.valueToggle.isOn;
		}

		public override DebugUIHandlerWidget Next()
		{
			if (!this.valueToggle.isOn || this.m_Container == null)
			{
				return base.Next();
			}
			DebugUIHandlerWidget firstItem = this.m_Container.GetFirstItem();
			if (firstItem == null)
			{
				return base.Next();
			}
			return firstItem;
		}

		public Text nameLabel;

		public UIFoldout valueToggle;

		public DebugUIHandlerIndirectFloatField fieldX;

		public DebugUIHandlerIndirectFloatField fieldY;

		public DebugUIHandlerIndirectFloatField fieldZ;

		private DebugUI.Vector3Field m_Field;

		private DebugUIHandlerContainer m_Container;
	}
}
