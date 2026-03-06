using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DesignTimeVisible(false)]
	[DisplayStringFormat("{modifier1}+{modifier2}+{button}")]
	public class ButtonWithTwoModifiers : InputBindingComposite<float>
	{
		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (this.ModifiersArePressed(ref context))
			{
				return context.ReadValue<float>(this.button);
			}
			return 0f;
		}

		private bool ModifiersArePressed(ref InputBindingCompositeContext context)
		{
			bool flag = context.ReadValueAsButton(this.modifier1) && context.ReadValueAsButton(this.modifier2);
			if (flag && this.modifiersOrder == ButtonWithTwoModifiers.ModifiersOrder.Ordered)
			{
				double pressTime = context.GetPressTime(this.button);
				double pressTime2 = context.GetPressTime(this.modifier1);
				double pressTime3 = context.GetPressTime(this.modifier2);
				return pressTime2 <= pressTime && pressTime3 <= pressTime;
			}
			return flag;
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return this.ReadValue(ref context);
		}

		protected override void FinishSetup(ref InputBindingCompositeContext context)
		{
			if (this.modifiersOrder == ButtonWithTwoModifiers.ModifiersOrder.Default)
			{
				if (this.overrideModifiersNeedToBePressedFirst)
				{
					this.modifiersOrder = ButtonWithTwoModifiers.ModifiersOrder.Unordered;
					return;
				}
				this.modifiersOrder = (InputSystem.settings.shortcutKeysConsumeInput ? ButtonWithTwoModifiers.ModifiersOrder.Ordered : ButtonWithTwoModifiers.ModifiersOrder.Unordered);
			}
		}

		[InputControl(layout = "Button")]
		public int modifier1;

		[InputControl(layout = "Button")]
		public int modifier2;

		[InputControl(layout = "Button")]
		public int button;

		[Tooltip("Obsolete please use modifiers Order. If enabled, this will override the Input Consumption setting, allowing the modifier keys to be pressed after the button and the composite will still trigger.")]
		[Obsolete("Use ModifiersOrder.Unordered with 'modifiersOrder' instead")]
		public bool overrideModifiersNeedToBePressedFirst;

		[Tooltip("By default it follows the Input Consumption setting to determine if the modifers keys need to be pressed first.")]
		public ButtonWithTwoModifiers.ModifiersOrder modifiersOrder;

		public enum ModifiersOrder
		{
			Default,
			Ordered,
			Unordered
		}
	}
}
