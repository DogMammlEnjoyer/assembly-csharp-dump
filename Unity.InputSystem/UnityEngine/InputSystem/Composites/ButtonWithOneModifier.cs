using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DesignTimeVisible(false)]
	[DisplayStringFormat("{modifier}+{button}")]
	public class ButtonWithOneModifier : InputBindingComposite<float>
	{
		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (this.ModifierIsPressed(ref context))
			{
				return context.ReadValue<float>(this.button);
			}
			return 0f;
		}

		private bool ModifierIsPressed(ref InputBindingCompositeContext context)
		{
			bool flag = context.ReadValueAsButton(this.modifier);
			if (flag && this.modifiersOrder == ButtonWithOneModifier.ModifiersOrder.Ordered)
			{
				double pressTime = context.GetPressTime(this.button);
				return context.GetPressTime(this.modifier) <= pressTime;
			}
			return flag;
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return this.ReadValue(ref context);
		}

		protected override void FinishSetup(ref InputBindingCompositeContext context)
		{
			if (this.modifiersOrder == ButtonWithOneModifier.ModifiersOrder.Default)
			{
				if (this.overrideModifiersNeedToBePressedFirst)
				{
					this.modifiersOrder = ButtonWithOneModifier.ModifiersOrder.Unordered;
					return;
				}
				this.modifiersOrder = (InputSystem.settings.shortcutKeysConsumeInput ? ButtonWithOneModifier.ModifiersOrder.Ordered : ButtonWithOneModifier.ModifiersOrder.Unordered);
			}
		}

		[InputControl(layout = "Button")]
		public int modifier;

		[InputControl(layout = "Button")]
		public int button;

		[Tooltip("Obsolete please use modifiers Order. If enabled, this will override the Input Consumption setting, allowing the modifier keys to be pressed after the button and the composite will still trigger.")]
		[Obsolete("Use ModifiersOrder.Unordered with 'modifiersOrder' instead")]
		public bool overrideModifiersNeedToBePressedFirst;

		[Tooltip("By default it follows the Input Consumption setting to determine if the modifers keys need to be pressed first.")]
		public ButtonWithOneModifier.ModifiersOrder modifiersOrder;

		public enum ModifiersOrder
		{
			Default,
			Ordered,
			Unordered
		}
	}
}
