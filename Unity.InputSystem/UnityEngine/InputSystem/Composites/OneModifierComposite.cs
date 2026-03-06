using System;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DisplayStringFormat("{modifier}+{binding}")]
	[DisplayName("Binding With One Modifier")]
	public class OneModifierComposite : InputBindingComposite
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
			if (this.ModifierIsPressed(ref context))
			{
				return context.EvaluateMagnitude(this.binding);
			}
			return 0f;
		}

		public unsafe override void ReadValue(ref InputBindingCompositeContext context, void* buffer, int bufferSize)
		{
			if (this.ModifierIsPressed(ref context))
			{
				context.ReadValue(this.binding, buffer, bufferSize);
				return;
			}
			UnsafeUtility.MemClear(buffer, (long)this.m_ValueSizeInBytes);
		}

		private bool ModifierIsPressed(ref InputBindingCompositeContext context)
		{
			bool flag = context.ReadValueAsButton(this.modifier);
			if (flag && this.m_BindingIsButton && this.modifiersOrder == OneModifierComposite.ModifiersOrder.Ordered)
			{
				double pressTime = context.GetPressTime(this.binding);
				return context.GetPressTime(this.modifier) <= pressTime;
			}
			return flag;
		}

		protected override void FinishSetup(ref InputBindingCompositeContext context)
		{
			OneModifierComposite.DetermineValueTypeAndSize(ref context, this.binding, out this.m_ValueType, out this.m_ValueSizeInBytes, out this.m_BindingIsButton);
			if (this.modifiersOrder == OneModifierComposite.ModifiersOrder.Default)
			{
				if (this.overrideModifiersNeedToBePressedFirst)
				{
					this.modifiersOrder = OneModifierComposite.ModifiersOrder.Unordered;
					return;
				}
				this.modifiersOrder = (InputSystem.settings.shortcutKeysConsumeInput ? OneModifierComposite.ModifiersOrder.Ordered : OneModifierComposite.ModifiersOrder.Unordered);
			}
		}

		public override object ReadValueAsObject(ref InputBindingCompositeContext context)
		{
			if (context.ReadValueAsButton(this.modifier))
			{
				return context.ReadValueAsObject(this.binding);
			}
			return null;
		}

		internal static void DetermineValueTypeAndSize(ref InputBindingCompositeContext context, int part, out Type valueType, out int valueSizeInBytes, out bool isButton)
		{
			valueSizeInBytes = 0;
			isButton = true;
			Type type = null;
			foreach (InputBindingCompositeContext.PartBinding partBinding in context.controls)
			{
				if (partBinding.part == part)
				{
					Type valueType2 = partBinding.control.valueType;
					if (type == null || valueType2.IsAssignableFrom(type))
					{
						type = valueType2;
					}
					else if (!type.IsAssignableFrom(valueType2))
					{
						type = typeof(Object);
					}
					valueSizeInBytes = Math.Max(partBinding.control.valueSizeInBytes, valueSizeInBytes);
					isButton &= partBinding.control.isButton;
				}
			}
			valueType = type;
		}

		[InputControl(layout = "Button")]
		public int modifier;

		[InputControl]
		public int binding;

		[Tooltip("Obsolete please use modifiers Order. If enabled, this will override the Input Consumption setting, allowing the modifier keys to be pressed after the button and the composite will still trigger.")]
		[Obsolete("Use ModifiersOrder.Unordered with 'modifiersOrder' instead")]
		public bool overrideModifiersNeedToBePressedFirst;

		[Tooltip("By default it follows the Input Consumption setting to determine if the modifers keys need to be pressed first.")]
		public OneModifierComposite.ModifiersOrder modifiersOrder;

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
