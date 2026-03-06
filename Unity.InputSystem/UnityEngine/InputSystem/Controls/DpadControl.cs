using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Controls
{
	public class DpadControl : Vector2Control
	{
		[InputControl(name = "x", layout = "DpadAxis", useStateFrom = "right", synthetic = true)]
		[InputControl(name = "y", layout = "DpadAxis", useStateFrom = "up", synthetic = true)]
		[InputControl(bit = 0U, displayName = "Up")]
		public ButtonControl up { get; set; }

		[InputControl(bit = 1U, displayName = "Down")]
		public ButtonControl down { get; set; }

		[InputControl(bit = 2U, displayName = "Left")]
		public ButtonControl left { get; set; }

		[InputControl(bit = 3U, displayName = "Right")]
		public ButtonControl right { get; set; }

		public DpadControl()
		{
			this.m_StateBlock.sizeInBits = 4U;
			this.m_StateBlock.format = InputStateBlock.FormatBit;
		}

		protected override void FinishSetup()
		{
			this.up = base.GetChildControl<ButtonControl>("up");
			this.down = base.GetChildControl<ButtonControl>("down");
			this.left = base.GetChildControl<ButtonControl>("left");
			this.right = base.GetChildControl<ButtonControl>("right");
			base.FinishSetup();
		}

		public unsafe override Vector2 ReadUnprocessedValueFromState(void* statePtr)
		{
			bool up = this.up.ReadValueFromStateWithCaching(statePtr) >= this.up.pressPointOrDefault;
			bool down = this.down.ReadValueFromStateWithCaching(statePtr) >= this.down.pressPointOrDefault;
			bool left = this.left.ReadValueFromStateWithCaching(statePtr) >= this.left.pressPointOrDefault;
			bool right = this.right.ReadValueFromStateWithCaching(statePtr) >= this.right.pressPointOrDefault;
			return DpadControl.MakeDpadVector(up, down, left, right, true);
		}

		public unsafe override void WriteValueIntoState(Vector2 value, void* statePtr)
		{
			bool flag = this.up.IsValueConsideredPressed(value.y);
			bool flag2 = this.down.IsValueConsideredPressed(value.y * -1f);
			bool flag3 = this.left.IsValueConsideredPressed(value.x * -1f);
			bool flag4 = this.right.IsValueConsideredPressed(value.x);
			this.up.WriteValueIntoState((flag && !flag2) ? value.y : 0f, statePtr);
			this.down.WriteValueIntoState((flag2 && !flag) ? (value.y * -1f) : 0f, statePtr);
			this.left.WriteValueIntoState((flag3 && !flag4) ? (value.x * -1f) : 0f, statePtr);
			this.right.WriteValueIntoState((flag4 && !flag3) ? value.x : 0f, statePtr);
		}

		public static Vector2 MakeDpadVector(bool up, bool down, bool left, bool right, bool normalize = true)
		{
			float num = up ? 1f : 0f;
			float num2 = down ? -1f : 0f;
			float num3 = left ? -1f : 0f;
			float num4 = right ? 1f : 0f;
			Vector2 vector = new Vector2(num3 + num4, num + num2);
			if (normalize && vector.x != 0f && vector.y != 0f)
			{
				vector = new Vector2(vector.x * 0.707107f, vector.y * 0.707107f);
			}
			return vector;
		}

		public static Vector2 MakeDpadVector(float up, float down, float left, float right)
		{
			return new Vector2(-left + right, up - down);
		}

		[InputControlLayout(hideInUI = true)]
		public class DpadAxisControl : AxisControl
		{
			public int component { get; set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.component = ((base.name == "x") ? 0 : 1);
				this.m_StateBlock = this.m_Parent.m_StateBlock;
			}

			public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
			{
				return ((DpadControl)this.m_Parent).ReadUnprocessedValueFromState(statePtr)[this.component];
			}
		}

		internal enum ButtonBits
		{
			Up,
			Down,
			Left,
			Right
		}
	}
}
