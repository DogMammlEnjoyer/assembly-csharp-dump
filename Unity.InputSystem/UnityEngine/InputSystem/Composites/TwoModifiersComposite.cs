using System;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DisplayStringFormat("{modifier1}+{modifier2}+{binding}")]
	[DisplayName("Binding With Two Modifiers")]
	public class TwoModifiersComposite : InputBindingComposite
	{
		public override Type valueType
		{
			get
			{
				return this.m_ValueType;
			}
		}

		public override int valueSizeInBytes
		{
			get
			{
				return this.m_ValueSizeInBytes;
			}
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			if (this.ModifiersArePressed(ref context))
			{
				return context.EvaluateMagnitude(this.binding);
			}
			return 0f;
		}

		public unsafe override void ReadValue(ref InputBindingCompositeContext context, void* buffer, int bufferSize)
		{
			if (this.ModifiersArePressed(ref context))
			{
				context.ReadValue(this.binding, buffer, bufferSize);
				return;
			}
			UnsafeUtility.MemClear(buffer, (long)this.m_ValueSizeInBytes);
		}

		private bool ModifiersArePressed(ref InputBindingCompositeContext context)
		{
			bool flag = context.ReadValueAsButton(this.modifier1) && context.ReadValueAsButton(this.modifier2);
			if (flag && this.m_BindingIsButton && this.modifiersOrder == TwoModifiersComposite.ModifiersOrder.Ordered)
			{
				double pressTime = context.GetPressTime(this.binding);
				double pressTime2 = context.GetPressTime(this.modifier1);
				double pressTime3 = context.GetPressTime(this.modifier2);
				return pressTime2 <= pressTime && pressTime3 <= pressTime;
			}
			return flag;
		}

		protected override void FinishSetup(ref InputBindingCompositeContext context)
		{
			OneModifierComposite.DetermineValueTypeAndSize(ref context, this.binding, out this.m_ValueType, out this.m_ValueSizeInBytes, out this.m_BindingIsButton);
			if (this.modifiersOrder == TwoModifiersComposite.ModifiersOrder.Default)
			{
				if (this.overrideModifiersNeedToBePressedFirst)
				{
					this.modifiersOrder = TwoModifiersComposite.ModifiersOrder.Unordered;
					return;
				}
				this.modifiersOrder = (InputSystem.settings.shortcutKeysConsumeInput ? TwoModifiersComposite.ModifiersOrder.Ordered : TwoModifiersComposite.ModifiersOrder.Unordered);
			}
		}

		public override object ReadValueAsObject(ref InputBindingCompositeContext context)
		{
			if (context.ReadValueAsButton(this.modifier1) && context.ReadValueAsButton(this.modifier2))
			{
				return context.ReadValueAsObject(this.binding);
			}
			return null;
		}

		[InputControl(layout = "Button")]
		public int modifier1;

		[InputControl(layout = "Button")]
		public int modifier2;

		[InputControl]
		public int binding;

		[Tooltip("Obsolete please use modifiers Order. If enabled, this will override the Input Consumption setting, allowing the modifier keys to be pressed after the button and the composite will still trigger.")]
		[Obsolete("Use ModifiersOrder.Unordered with 'modifiersOrder' instead")]
		public bool overrideModifiersNeedToBePressedFirst;

		[Tooltip("By default it follows the Input Consumption setting to determine if the modifers keys need to be pressed first.")]
		public TwoModifiersComposite.ModifiersOrder modifiersOrder;

		private int m_ValueSizeInBytes;

		private Type m_ValueType;

		private bool m_BindingIsButton;

		public enum ModifiersOrder
		{
			Default,
			Ordered,
			Unordered
		}
	}
}
