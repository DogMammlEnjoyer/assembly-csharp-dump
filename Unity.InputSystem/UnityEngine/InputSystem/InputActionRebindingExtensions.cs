using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputActionRebindingExtensions
	{
		public static PrimitiveValue? GetParameterValue(this InputAction action, string name, InputBinding bindingMask = default(InputBinding))
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return action.GetParameterValue(new InputActionRebindingExtensions.ParameterOverride(name, bindingMask, default(PrimitiveValue)));
		}

		private static PrimitiveValue? GetParameterValue(this InputAction action, InputActionRebindingExtensions.ParameterOverride parameterOverride)
		{
			parameterOverride.bindingMask.action = action.name;
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			using (InputActionRebindingExtensions.ParameterEnumerator enumerator = new InputActionRebindingExtensions.ParameterEnumerable(orCreateActionMap.m_State, parameterOverride, orCreateActionMap.m_MapIndexInState).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					InputActionRebindingExtensions.Parameter parameter = enumerator.Current;
					return new PrimitiveValue?(PrimitiveValue.FromObject(parameter.field.GetValue(parameter.instance)));
				}
			}
			return null;
		}

		public static PrimitiveValue? GetParameterValue(this InputAction action, string name, int bindingIndex)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (bindingIndex < 0)
			{
				throw new ArgumentOutOfRangeException("bindingIndex");
			}
			int index = action.BindingIndexOnActionToBindingIndexOnMap(bindingIndex);
			InputBinding bindingMask = new InputBinding
			{
				id = action.GetOrCreateActionMap().bindings[index].id
			};
			return action.GetParameterValue(name, bindingMask);
		}

		public unsafe static TValue? GetParameterValue<TObject, TValue>(this InputAction action, Expression<Func<TObject, TValue>> expr, InputBinding bindingMask = default(InputBinding)) where TValue : struct
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (expr == null)
			{
				throw new ArgumentNullException("expr");
			}
			InputActionRebindingExtensions.ParameterOverride parameterOverride = InputActionRebindingExtensions.ExtractParameterOverride<TObject, TValue>(expr, bindingMask, default(PrimitiveValue));
			PrimitiveValue? parameterValue = action.GetParameterValue(parameterOverride);
			if (parameterValue == null)
			{
				return null;
			}
			if (Type.GetTypeCode(typeof(TValue)) == parameterValue.Value.type)
			{
				PrimitiveValue value = parameterValue.Value;
				TValue value2 = default(TValue);
				UnsafeUtility.MemCpy(UnsafeUtility.AddressOf<TValue>(ref value2), (void*)value.valuePtr, (long)UnsafeUtility.SizeOf<TValue>());
				return new TValue?(value2);
			}
			return new TValue?((TValue)((object)Convert.ChangeType(parameterValue.Value.ToObject(), typeof(TValue))));
		}

		public static void ApplyParameterOverride<TObject, TValue>(this InputAction action, Expression<Func<TObject, TValue>> expr, TValue value, InputBinding bindingMask = default(InputBinding)) where TValue : struct
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (expr == null)
			{
				throw new ArgumentNullException("expr");
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			bindingMask.action = action.name;
			InputActionRebindingExtensions.ParameterOverride parameterOverride = InputActionRebindingExtensions.ExtractParameterOverride<TObject, TValue>(expr, bindingMask, PrimitiveValue.From<TValue>(value));
			InputActionRebindingExtensions.ApplyParameterOverride(orCreateActionMap.m_State, orCreateActionMap.m_MapIndexInState, ref orCreateActionMap.m_ParameterOverrides, ref orCreateActionMap.m_ParameterOverridesCount, parameterOverride);
		}

		public static void ApplyParameterOverride<TObject, TValue>(this InputActionMap actionMap, Expression<Func<TObject, TValue>> expr, TValue value, InputBinding bindingMask = default(InputBinding)) where TValue : struct
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (expr == null)
			{
				throw new ArgumentNullException("expr");
			}
			actionMap.ResolveBindingsIfNecessary();
			InputActionRebindingExtensions.ParameterOverride parameterOverride = InputActionRebindingExtensions.ExtractParameterOverride<TObject, TValue>(expr, bindingMask, PrimitiveValue.From<TValue>(value));
			InputActionRebindingExtensions.ApplyParameterOverride(actionMap.m_State, actionMap.m_MapIndexInState, ref actionMap.m_ParameterOverrides, ref actionMap.m_ParameterOverridesCount, parameterOverride);
		}

		public static void ApplyParameterOverride<TObject, TValue>(this InputActionAsset asset, Expression<Func<TObject, TValue>> expr, TValue value, InputBinding bindingMask = default(InputBinding)) where TValue : struct
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (expr == null)
			{
				throw new ArgumentNullException("expr");
			}
			asset.ResolveBindingsIfNecessary();
			InputActionRebindingExtensions.ParameterOverride parameterOverride = InputActionRebindingExtensions.ExtractParameterOverride<TObject, TValue>(expr, bindingMask, PrimitiveValue.From<TValue>(value));
			InputActionRebindingExtensions.ApplyParameterOverride(asset.m_SharedStateForAllMaps, -1, ref asset.m_ParameterOverrides, ref asset.m_ParameterOverridesCount, parameterOverride);
		}

		private static InputActionRebindingExtensions.ParameterOverride ExtractParameterOverride<TObject, TValue>(Expression<Func<TObject, TValue>> expr, InputBinding bindingMask = default(InputBinding), PrimitiveValue value = default(PrimitiveValue))
		{
			if (expr == null)
			{
				throw new ArgumentException("Expression must be a LambdaExpression but was a " + expr.GetType().Name + " instead", "expr");
			}
			MemberExpression memberExpression = expr.Body as MemberExpression;
			if (memberExpression == null)
			{
				UnaryExpression unaryExpression = expr.Body as UnaryExpression;
				if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
				{
					MemberExpression memberExpression2 = unaryExpression.Operand as MemberExpression;
					if (memberExpression2 != null)
					{
						memberExpression = memberExpression2;
						goto IL_8D;
					}
				}
				throw new ArgumentException("Body in LambdaExpression must be a MemberExpression (x.name) but was a " + expr.GetType().Name + " instead", "expr");
			}
			IL_8D:
			string objectRegistrationName;
			if (typeof(InputProcessor).IsAssignableFrom(typeof(TObject)))
			{
				objectRegistrationName = InputProcessor.s_Processors.FindNameForType(typeof(TObject));
			}
			else if (typeof(IInputInteraction).IsAssignableFrom(typeof(TObject)))
			{
				objectRegistrationName = InputInteraction.s_Interactions.FindNameForType(typeof(TObject));
			}
			else
			{
				if (!typeof(InputBindingComposite).IsAssignableFrom(typeof(TObject)))
				{
					throw new ArgumentException("Given type must be an InputProcessor, IInputInteraction, or InputBindingComposite (was " + typeof(TObject).Name + ")", "TObject");
				}
				objectRegistrationName = InputBindingComposite.s_Composites.FindNameForType(typeof(TObject));
			}
			return new InputActionRebindingExtensions.ParameterOverride(objectRegistrationName, memberExpression.Member.Name, bindingMask, value);
		}

		public static void ApplyParameterOverride(this InputActionMap actionMap, string name, PrimitiveValue value, InputBinding bindingMask = default(InputBinding))
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			actionMap.ResolveBindingsIfNecessary();
			InputActionRebindingExtensions.ApplyParameterOverride(actionMap.m_State, actionMap.m_MapIndexInState, ref actionMap.m_ParameterOverrides, ref actionMap.m_ParameterOverridesCount, new InputActionRebindingExtensions.ParameterOverride(name, bindingMask, value));
		}

		public static void ApplyParameterOverride(this InputActionAsset asset, string name, PrimitiveValue value, InputBinding bindingMask = default(InputBinding))
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			asset.ResolveBindingsIfNecessary();
			InputActionRebindingExtensions.ApplyParameterOverride(asset.m_SharedStateForAllMaps, -1, ref asset.m_ParameterOverrides, ref asset.m_ParameterOverridesCount, new InputActionRebindingExtensions.ParameterOverride(name, bindingMask, value));
		}

		public static void ApplyParameterOverride(this InputAction action, string name, PrimitiveValue value, InputBinding bindingMask = default(InputBinding))
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			bindingMask.action = action.name;
			InputActionRebindingExtensions.ApplyParameterOverride(orCreateActionMap.m_State, orCreateActionMap.m_MapIndexInState, ref orCreateActionMap.m_ParameterOverrides, ref orCreateActionMap.m_ParameterOverridesCount, new InputActionRebindingExtensions.ParameterOverride(name, bindingMask, value));
		}

		public static void ApplyParameterOverride(this InputAction action, string name, PrimitiveValue value, int bindingIndex)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (bindingIndex < 0)
			{
				throw new ArgumentOutOfRangeException("bindingIndex");
			}
			int index = action.BindingIndexOnActionToBindingIndexOnMap(bindingIndex);
			InputBinding bindingMask = new InputBinding
			{
				id = action.GetOrCreateActionMap().bindings[index].id
			};
			action.ApplyParameterOverride(name, value, bindingMask);
		}

		private static void ApplyParameterOverride(InputActionState state, int mapIndex, ref InputActionRebindingExtensions.ParameterOverride[] parameterOverrides, ref int parameterOverridesCount, InputActionRebindingExtensions.ParameterOverride parameterOverride)
		{
			bool flag = false;
			if (parameterOverrides != null)
			{
				for (int i = 0; i < parameterOverridesCount; i++)
				{
					ref InputActionRebindingExtensions.ParameterOverride ptr = ref parameterOverrides[i];
					if (string.Equals(ptr.objectRegistrationName, parameterOverride.objectRegistrationName, StringComparison.OrdinalIgnoreCase) && string.Equals(ptr.parameter, parameterOverride.parameter, StringComparison.OrdinalIgnoreCase) && ptr.bindingMask == parameterOverride.bindingMask)
					{
						flag = true;
						ptr = parameterOverride;
						break;
					}
				}
			}
			if (!flag)
			{
				ArrayHelpers.AppendWithCapacity<InputActionRebindingExtensions.ParameterOverride>(ref parameterOverrides, ref parameterOverridesCount, parameterOverride, 10);
			}
			foreach (InputActionRebindingExtensions.Parameter parameter in new InputActionRebindingExtensions.ParameterEnumerable(state, parameterOverride, mapIndex))
			{
				InputActionMap actionMap = state.GetActionMap(parameter.bindingIndex);
				ref InputBinding binding = ref state.GetBinding(parameter.bindingIndex);
				InputActionRebindingExtensions.ParameterOverride? parameterOverride2 = InputActionRebindingExtensions.ParameterOverride.Find(actionMap, ref binding, parameterOverride.parameter, parameterOverride.objectRegistrationName);
				if (parameterOverride2 != null)
				{
					TypeCode typeCode = Type.GetTypeCode(parameter.field.FieldType);
					parameter.field.SetValue(parameter.instance, parameterOverride2.Value.value.ConvertTo(typeCode).ToObject());
				}
			}
		}

		public static int GetBindingIndex(this InputAction action, InputBinding bindingMask)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				if (bindingMask.Matches(bindings[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int GetBindingIndex(this InputActionMap actionMap, InputBinding bindingMask)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			ReadOnlyArray<InputBinding> bindings = actionMap.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				if (bindingMask.Matches(bindings[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int GetBindingIndex(this InputAction action, string group = null, string path = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			return action.GetBindingIndex(new InputBinding(path, null, group, null, null, null));
		}

		public static InputBinding? GetBindingForControl(this InputAction action, InputControl control)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			int bindingIndexForControl = action.GetBindingIndexForControl(control);
			if (bindingIndexForControl == -1)
			{
				return null;
			}
			return new InputBinding?(action.bindings[bindingIndexForControl]);
		}

		public unsafe static int GetBindingIndexForControl(this InputAction action, InputControl control)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			orCreateActionMap.ResolveBindingsIfNecessary();
			InputActionState state = orCreateActionMap.m_State;
			InputControl[] controls = state.controls;
			int totalControlCount = state.totalControlCount;
			InputActionState.BindingState* bindingStates = state.bindingStates;
			int* controlIndexToBindingIndex = state.controlIndexToBindingIndex;
			int actionIndexInState = action.m_ActionIndexInState;
			for (int i = 0; i < totalControlCount; i++)
			{
				if (controls[i] == control)
				{
					int num = controlIndexToBindingIndex[i];
					if (bindingStates[num].actionIndex == actionIndexInState)
					{
						int bindingIndexInMap = state.GetBindingIndexInMap(num);
						return action.BindingIndexOnMapToBindingIndexOnAction(bindingIndexInMap);
					}
				}
			}
			return -1;
		}

		public static string GetBindingDisplayString(this InputAction action, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0, string group = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			InputBinding bindingMask;
			if (!string.IsNullOrEmpty(group))
			{
				bindingMask = InputBinding.MaskByGroup(group);
			}
			else
			{
				InputBinding? inputBinding = action.FindEffectiveBindingMask();
				if (inputBinding != null)
				{
					bindingMask = inputBinding.Value;
				}
				else
				{
					bindingMask = default(InputBinding);
				}
			}
			return action.GetBindingDisplayString(bindingMask, options);
		}

		public static string GetBindingDisplayString(this InputAction action, InputBinding bindingMask, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			string text = string.Empty;
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				if (!bindings[i].isPartOfComposite && bindingMask.Matches(bindings[i]))
				{
					string bindingDisplayString = action.GetBindingDisplayString(i, options);
					if (text != "")
					{
						text = text + " | " + bindingDisplayString;
					}
					else
					{
						text = bindingDisplayString;
					}
				}
			}
			return text;
		}

		public static string GetBindingDisplayString(this InputAction action, int bindingIndex, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			string text;
			string text2;
			return action.GetBindingDisplayString(bindingIndex, out text, out text2, options);
		}

		public unsafe static string GetBindingDisplayString(this InputAction action, int bindingIndex, out string deviceLayoutName, out string controlPath, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			deviceLayoutName = null;
			controlPath = null;
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int count = bindings.Count;
			if (bindingIndex < 0 || bindingIndex >= count)
			{
				throw new ArgumentOutOfRangeException(string.Format("Binding index {0} is out of range on action '{1}' with {2} bindings", bindingIndex, action, bindings.Count), "bindingIndex");
			}
			if (!bindings[bindingIndex].isComposite)
			{
				InputControl control = null;
				InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
				orCreateActionMap.ResolveBindingsIfNecessary();
				InputActionState state = orCreateActionMap.m_State;
				int bindingIndexInMap = action.BindingIndexOnActionToBindingIndexOnMap(bindingIndex);
				int bindingIndexInState = state.GetBindingIndexInState(orCreateActionMap.m_MapIndexInState, bindingIndexInMap);
				InputActionState.BindingState* ptr = state.bindingStates + bindingIndexInState;
				if (ptr->controlCount > 0)
				{
					control = state.controls[ptr->controlStartIndex];
				}
				InputBinding inputBinding = bindings[bindingIndex];
				if (string.IsNullOrEmpty(inputBinding.effectiveInteractions))
				{
					inputBinding.overrideInteractions = action.interactions;
				}
				else if (!string.IsNullOrEmpty(action.interactions))
				{
					inputBinding.overrideInteractions = inputBinding.effectiveInteractions + ";action.interactions";
				}
				return inputBinding.ToDisplayString(out deviceLayoutName, out controlPath, options, control);
			}
			string name = NameAndParameters.Parse(bindings[bindingIndex].effectivePath).name;
			int firstPartIndex = bindingIndex + 1;
			int num = firstPartIndex;
			while (num < count && bindings[num].isPartOfComposite)
			{
				num++;
			}
			int partCount = num - firstPartIndex;
			string[] partStrings = new string[partCount];
			for (int i = 0; i < partCount; i++)
			{
				string text = action.GetBindingDisplayString(firstPartIndex + i, options);
				if (string.IsNullOrEmpty(text))
				{
					text = " ";
				}
				partStrings[i] = text;
			}
			string displayFormatString = InputBindingComposite.GetDisplayFormatString(name);
			if (string.IsNullOrEmpty(displayFormatString))
			{
				return StringHelpers.Join<string>("/", partStrings);
			}
			return StringHelpers.ExpandTemplateString(displayFormatString, delegate(string fragment)
			{
				string text2 = string.Empty;
				for (int j = 0; j < partCount; j++)
				{
					if (string.Equals(bindings[firstPartIndex + j].name, fragment, StringComparison.InvariantCultureIgnoreCase))
					{
						if (!string.IsNullOrEmpty(text2))
						{
							text2 = text2 + "|" + partStrings[j];
						}
						else
						{
							text2 = partStrings[j];
						}
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = " ";
				}
				return text2;
			});
		}

		public static void ApplyBindingOverride(this InputAction action, string newPath, string group = null, string path = null)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action.ApplyBindingOverride(new InputBinding
			{
				overridePath = newPath,
				groups = group,
				path = path
			});
		}

		public static void ApplyBindingOverride(this InputAction action, InputBinding bindingOverride)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bool enabled = action.enabled;
			if (enabled)
			{
				action.Disable();
			}
			bindingOverride.action = action.name;
			action.GetOrCreateActionMap().ApplyBindingOverride(bindingOverride);
			if (enabled)
			{
				action.Enable();
				action.RequestInitialStateCheckOnEnabledAction();
			}
		}

		public static void ApplyBindingOverride(this InputAction action, int bindingIndex, InputBinding bindingOverride)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			int bindingIndex2 = action.BindingIndexOnActionToBindingIndexOnMap(bindingIndex);
			bindingOverride.action = action.name;
			action.GetOrCreateActionMap().ApplyBindingOverride(bindingIndex2, bindingOverride);
		}

		public static void ApplyBindingOverride(this InputAction action, int bindingIndex, string path)
		{
			if (path == null)
			{
				throw new ArgumentException("Binding path cannot be null", "path");
			}
			action.ApplyBindingOverride(bindingIndex, new InputBinding
			{
				overridePath = path
			});
		}

		public static int ApplyBindingOverride(this InputActionMap actionMap, InputBinding bindingOverride)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			InputBinding[] bindings = actionMap.m_Bindings;
			if (bindings == null)
			{
				return 0;
			}
			int num = bindings.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (bindingOverride.Matches(ref bindings[i], (InputBinding.MatchOptions)0))
				{
					bindings[i].overridePath = bindingOverride.overridePath;
					bindings[i].overrideInteractions = bindingOverride.overrideInteractions;
					bindings[i].overrideProcessors = bindingOverride.overrideProcessors;
					num2++;
				}
			}
			if (num2 > 0)
			{
				actionMap.OnBindingModified();
			}
			return num2;
		}

		public static void ApplyBindingOverride(this InputActionMap actionMap, int bindingIndex, InputBinding bindingOverride)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			InputBinding[] bindings = actionMap.m_Bindings;
			int num = (bindings != null) ? bindings.Length : 0;
			if (bindingIndex < 0 || bindingIndex >= num)
			{
				throw new ArgumentOutOfRangeException("bindingIndex", string.Format("Cannot apply override to binding at index {0} in map '{1}' with only {2} bindings", bindingIndex, actionMap, num));
			}
			actionMap.m_Bindings[bindingIndex].overridePath = bindingOverride.overridePath;
			actionMap.m_Bindings[bindingIndex].overrideInteractions = bindingOverride.overrideInteractions;
			actionMap.m_Bindings[bindingIndex].overrideProcessors = bindingOverride.overrideProcessors;
			actionMap.OnBindingModified();
		}

		public static void RemoveBindingOverride(this InputAction action, int bindingIndex)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action.ApplyBindingOverride(bindingIndex, default(InputBinding));
		}

		public static void RemoveBindingOverride(this InputAction action, InputBinding bindingMask)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bindingMask.overridePath = null;
			bindingMask.overrideInteractions = null;
			bindingMask.overrideProcessors = null;
			action.ApplyBindingOverride(bindingMask);
		}

		private static void RemoveBindingOverride(this InputActionMap actionMap, InputBinding bindingMask)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			bindingMask.overridePath = null;
			bindingMask.overrideInteractions = null;
			bindingMask.overrideProcessors = null;
			actionMap.ApplyBindingOverride(bindingMask);
		}

		public static void RemoveAllBindingOverrides(this IInputActionCollection2 actions)
		{
			if (actions == null)
			{
				throw new ArgumentNullException("actions");
			}
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				foreach (InputAction inputAction in actions)
				{
					InputActionMap orCreateActionMap = inputAction.GetOrCreateActionMap();
					InputBinding[] bindings = orCreateActionMap.m_Bindings;
					int num = bindings.LengthSafe<InputBinding>();
					for (int i = 0; i < num; i++)
					{
						ref InputBinding ptr = ref bindings[i];
						if (ptr.TriggersAction(inputAction))
						{
							ptr.RemoveOverrides();
						}
					}
					orCreateActionMap.OnBindingModified();
				}
			}
		}

		public static void RemoveAllBindingOverrides(this InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			string name = action.name;
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			InputBinding[] bindings = orCreateActionMap.m_Bindings;
			if (bindings == null)
			{
				return;
			}
			int num = bindings.Length;
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(bindings[i].action, name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					bindings[i].overridePath = null;
					bindings[i].overrideInteractions = null;
					bindings[i].overrideProcessors = null;
				}
			}
			orCreateActionMap.OnBindingModified();
		}

		public static void ApplyBindingOverrides(this InputActionMap actionMap, IEnumerable<InputBinding> overrides)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (overrides == null)
			{
				throw new ArgumentNullException("overrides");
			}
			foreach (InputBinding bindingOverride in overrides)
			{
				actionMap.ApplyBindingOverride(bindingOverride);
			}
		}

		public static void RemoveBindingOverrides(this InputActionMap actionMap, IEnumerable<InputBinding> overrides)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (overrides == null)
			{
				throw new ArgumentNullException("overrides");
			}
			foreach (InputBinding bindingMask in overrides)
			{
				actionMap.RemoveBindingOverride(bindingMask);
			}
		}

		public static int ApplyBindingOverridesOnMatchingControls(this InputAction action, InputControl control)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int count = bindings.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = InputControlPath.TryFindControl(control, bindings[i].path, 0);
				if (inputControl != null)
				{
					action.ApplyBindingOverride(i, inputControl.path);
					num++;
				}
			}
			return num;
		}

		public static int ApplyBindingOverridesOnMatchingControls(this InputActionMap actionMap, InputControl control)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			ReadOnlyArray<InputAction> actions = actionMap.actions;
			int count = actions.Count;
			int result = 0;
			for (int i = 0; i < count; i++)
			{
				result = actions[i].ApplyBindingOverridesOnMatchingControls(control);
			}
			return result;
		}

		public static string SaveBindingOverridesAsJson(this IInputActionCollection2 actions)
		{
			if (actions == null)
			{
				throw new ArgumentNullException("actions");
			}
			List<InputActionMap.BindingOverrideJson> list = new List<InputActionMap.BindingOverrideJson>();
			foreach (InputBinding binding in actions.bindings)
			{
				actions.AddBindingOverrideJsonTo(binding, list, null);
			}
			if (list.Count == 0)
			{
				return string.Empty;
			}
			return JsonUtility.ToJson(new InputActionMap.BindingOverrideListJson
			{
				bindings = list
			});
		}

		public static string SaveBindingOverridesAsJson(this InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			bool isSingletonAction = action.isSingletonAction;
			InputActionMap orCreateActionMap = action.GetOrCreateActionMap();
			List<InputActionMap.BindingOverrideJson> list = new List<InputActionMap.BindingOverrideJson>();
			foreach (InputBinding binding in action.bindings)
			{
				if (isSingletonAction || binding.TriggersAction(action))
				{
					orCreateActionMap.AddBindingOverrideJsonTo(binding, list, isSingletonAction ? action : null);
				}
			}
			if (list.Count == 0)
			{
				return string.Empty;
			}
			return JsonUtility.ToJson(new InputActionMap.BindingOverrideListJson
			{
				bindings = list
			});
		}

		private static void AddBindingOverrideJsonTo(this IInputActionCollection2 actions, InputBinding binding, List<InputActionMap.BindingOverrideJson> list, InputAction action = null)
		{
			if (!binding.hasOverrides)
			{
				return;
			}
			if (action == null)
			{
				action = actions.FindAction(binding.action, false);
			}
			string actionName = (action != null && !action.isSingletonAction) ? (action.actionMap.name + "/" + action.name) : "";
			InputActionMap.BindingOverrideJson item = InputActionMap.BindingOverrideJson.FromBinding(binding, actionName);
			list.Add(item);
		}

		public static void LoadBindingOverridesFromJson(this IInputActionCollection2 actions, string json, bool removeExisting = true)
		{
			if (actions == null)
			{
				throw new ArgumentNullException("actions");
			}
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				if (removeExisting)
				{
					actions.RemoveAllBindingOverrides();
				}
				actions.LoadBindingOverridesFromJsonInternal(json);
			}
		}

		public static void LoadBindingOverridesFromJson(this InputAction action, string json, bool removeExisting = true)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				if (removeExisting)
				{
					action.RemoveAllBindingOverrides();
				}
				action.GetOrCreateActionMap().LoadBindingOverridesFromJsonInternal(json);
			}
		}

		private static void LoadBindingOverridesFromJsonInternal(this IInputActionCollection2 actions, string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return;
			}
			foreach (InputActionMap.BindingOverrideJson bindingOverrideJson in JsonUtility.FromJson<InputActionMap.BindingOverrideListJson>(json).bindings)
			{
				if (!string.IsNullOrEmpty(bindingOverrideJson.id))
				{
					InputAction action;
					int num = actions.FindBinding(new InputBinding
					{
						m_Id = bindingOverrideJson.id
					}, out action);
					if (num != -1)
					{
						action.ApplyBindingOverride(num, InputActionMap.BindingOverrideJson.ToBinding(bindingOverrideJson));
						continue;
					}
				}
				Debug.LogWarning("Could not override binding as no existing binding was found with the id: " + bindingOverrideJson.id);
			}
		}

		public static InputActionRebindingExtensions.RebindingOperation PerformInteractiveRebinding(this InputAction action, int bindingIndex = -1)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			InputActionRebindingExtensions.RebindingOperation rebindingOperation = new InputActionRebindingExtensions.RebindingOperation().WithAction(action).OnMatchWaitForAnother(0.05f).WithControlsExcluding("<Pointer>/delta").WithControlsExcluding("<Pointer>/position").WithControlsExcluding("<Touchscreen>/touch*/position").WithControlsExcluding("<Touchscreen>/touch*/delta").WithControlsExcluding("<Mouse>/clickCount").WithMatchingEventsBeingSuppressed(true);
			if (rebindingOperation.expectedControlType != "Button")
			{
				rebindingOperation.WithCancelingThrough("<Keyboard>/escape");
			}
			if (bindingIndex >= 0)
			{
				ReadOnlyArray<InputBinding> bindings = action.bindings;
				if (bindingIndex >= bindings.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Binding index {0} is out of range for action '{1}' with {2} bindings", bindingIndex, action, bindings.Count), "bindings");
				}
				if (bindings[bindingIndex].isComposite)
				{
					throw new InvalidOperationException(string.Format("Cannot perform rebinding on composite binding '{0}' of '{1}'", bindings[bindingIndex], action));
				}
				rebindingOperation.WithTargetBinding(bindingIndex);
			}
			return rebindingOperation;
		}

		internal static InputActionRebindingExtensions.DeferBindingResolutionWrapper DeferBindingResolution()
		{
			if (InputActionRebindingExtensions.s_DeferBindingResolutionWrapper == null)
			{
				InputActionRebindingExtensions.s_DeferBindingResolutionWrapper = new InputActionRebindingExtensions.DeferBindingResolutionWrapper();
			}
			InputActionRebindingExtensions.s_DeferBindingResolutionWrapper.Acquire();
			return InputActionRebindingExtensions.s_DeferBindingResolutionWrapper;
		}

		private static InputActionRebindingExtensions.DeferBindingResolutionWrapper s_DeferBindingResolutionWrapper;

		internal struct Parameter
		{
			public object instance;

			public FieldInfo field;

			public int bindingIndex;
		}

		private struct ParameterEnumerable : IEnumerable<InputActionRebindingExtensions.Parameter>, IEnumerable
		{
			public ParameterEnumerable(InputActionState state, InputActionRebindingExtensions.ParameterOverride parameter, int mapIndex = -1)
			{
				this.m_State = state;
				this.m_Parameter = parameter;
				this.m_MapIndex = mapIndex;
			}

			public InputActionRebindingExtensions.ParameterEnumerator GetEnumerator()
			{
				return new InputActionRebindingExtensions.ParameterEnumerator(this.m_State, this.m_Parameter, this.m_MapIndex);
			}

			IEnumerator<InputActionRebindingExtensions.Parameter> IEnumerable<InputActionRebindingExtensions.Parameter>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private InputActionState m_State;

			private InputActionRebindingExtensions.ParameterOverride m_Parameter;

			private int m_MapIndex;
		}

		private struct ParameterEnumerator : IEnumerator<InputActionRebindingExtensions.Parameter>, IEnumerator, IDisposable
		{
			public ParameterEnumerator(InputActionState state, InputActionRebindingExtensions.ParameterOverride parameter, int mapIndex = -1)
			{
				this = default(InputActionRebindingExtensions.ParameterEnumerator);
				this.m_State = state;
				this.m_ParameterName = parameter.parameter;
				this.m_MapIndex = mapIndex;
				this.m_ObjectType = parameter.objectType;
				this.m_MayBeComposite = (this.m_ObjectType == null || typeof(InputBindingComposite).IsAssignableFrom(this.m_ObjectType));
				this.m_MayBeProcessor = (this.m_ObjectType == null || typeof(InputProcessor).IsAssignableFrom(this.m_ObjectType));
				this.m_MayBeInteraction = (this.m_ObjectType == null || typeof(IInputInteraction).IsAssignableFrom(this.m_ObjectType));
				this.m_BindingMask = parameter.bindingMask;
				this.Reset();
			}

			private bool MoveToNextBinding()
			{
				InputActionState.BindingState bindingState;
				for (;;)
				{
					this.m_BindingCurrentIndex++;
					if (this.m_BindingCurrentIndex >= this.m_BindingEndIndex)
					{
						break;
					}
					ref InputBinding binding = ref this.m_State.GetBinding(this.m_BindingCurrentIndex);
					bindingState = this.m_State.GetBindingState(this.m_BindingCurrentIndex);
					if ((bindingState.processorCount != 0 || bindingState.interactionCount != 0 || binding.isComposite) && (!this.m_MayBeComposite || this.m_MayBeProcessor || this.m_MayBeInteraction || binding.isComposite) && (!this.m_MayBeProcessor || this.m_MayBeComposite || this.m_MayBeInteraction || bindingState.processorCount != 0) && (!this.m_MayBeInteraction || this.m_MayBeComposite || this.m_MayBeProcessor || bindingState.interactionCount != 0) && this.m_BindingMask.Matches(ref binding, (InputBinding.MatchOptions)0))
					{
						goto Block_12;
					}
				}
				return false;
				Block_12:
				if (this.m_MayBeComposite)
				{
					InputBinding binding;
					this.m_CurrentBindingIsComposite = binding.isComposite;
				}
				this.m_ProcessorCurrentIndex = bindingState.processorStartIndex - 1;
				this.m_ProcessorEndIndex = bindingState.processorStartIndex + bindingState.processorCount;
				this.m_InteractionCurrentIndex = bindingState.interactionStartIndex - 1;
				this.m_InteractionEndIndex = bindingState.interactionStartIndex + bindingState.interactionCount;
				return true;
			}

			private bool MoveToNextInteraction()
			{
				while (this.m_InteractionCurrentIndex < this.m_InteractionEndIndex)
				{
					this.m_InteractionCurrentIndex++;
					if (this.m_InteractionCurrentIndex == this.m_InteractionEndIndex)
					{
						break;
					}
					IInputInteraction instance = this.m_State.interactions[this.m_InteractionCurrentIndex];
					if (this.FindParameter(instance))
					{
						return true;
					}
				}
				return false;
			}

			private bool MoveToNextProcessor()
			{
				while (this.m_ProcessorCurrentIndex < this.m_ProcessorEndIndex)
				{
					this.m_ProcessorCurrentIndex++;
					if (this.m_ProcessorCurrentIndex == this.m_ProcessorEndIndex)
					{
						break;
					}
					InputProcessor instance = this.m_State.processors[this.m_ProcessorCurrentIndex];
					if (this.FindParameter(instance))
					{
						return true;
					}
				}
				return false;
			}

			private bool FindParameter(object instance)
			{
				if (this.m_ObjectType != null && !this.m_ObjectType.IsInstanceOfType(instance))
				{
					return false;
				}
				FieldInfo field = instance.GetType().GetField(this.m_ParameterName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				if (field == null)
				{
					return false;
				}
				this.m_CurrentParameter = field;
				this.m_CurrentObject = instance;
				return true;
			}

			public bool MoveNext()
			{
				while (!this.m_MayBeInteraction || !this.MoveToNextInteraction())
				{
					if (this.m_MayBeProcessor && this.MoveToNextProcessor())
					{
						return true;
					}
					if (!this.MoveToNextBinding())
					{
						return false;
					}
					if (this.m_MayBeComposite && this.m_CurrentBindingIsComposite)
					{
						int compositeOrCompositeBindingIndex = this.m_State.GetBindingState(this.m_BindingCurrentIndex).compositeOrCompositeBindingIndex;
						InputBindingComposite instance = this.m_State.composites[compositeOrCompositeBindingIndex];
						if (this.FindParameter(instance))
						{
							return true;
						}
					}
				}
				return true;
			}

			public unsafe void Reset()
			{
				this.m_CurrentObject = null;
				this.m_CurrentParameter = null;
				this.m_InteractionCurrentIndex = 0;
				this.m_InteractionEndIndex = 0;
				this.m_ProcessorCurrentIndex = 0;
				this.m_ProcessorEndIndex = 0;
				this.m_CurrentBindingIsComposite = false;
				if (this.m_MapIndex < 0)
				{
					this.m_BindingCurrentIndex = -1;
					this.m_BindingEndIndex = this.m_State.totalBindingCount;
					return;
				}
				this.m_BindingCurrentIndex = this.m_State.mapIndices[this.m_MapIndex].bindingStartIndex - 1;
				this.m_BindingEndIndex = this.m_State.mapIndices[this.m_MapIndex].bindingStartIndex + this.m_State.mapIndices[this.m_MapIndex].bindingCount;
			}

			public InputActionRebindingExtensions.Parameter Current
			{
				get
				{
					return new InputActionRebindingExtensions.Parameter
					{
						instance = this.m_CurrentObject,
						field = this.m_CurrentParameter,
						bindingIndex = this.m_BindingCurrentIndex
					};
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			private InputActionState m_State;

			private int m_MapIndex;

			private int m_BindingCurrentIndex;

			private int m_BindingEndIndex;

			private int m_InteractionCurrentIndex;

			private int m_InteractionEndIndex;

			private int m_ProcessorCurrentIndex;

			private int m_ProcessorEndIndex;

			private InputBinding m_BindingMask;

			private Type m_ObjectType;

			private string m_ParameterName;

			private bool m_MayBeInteraction;

			private bool m_MayBeProcessor;

			private bool m_MayBeComposite;

			private bool m_CurrentBindingIsComposite;

			private object m_CurrentObject;

			private FieldInfo m_CurrentParameter;
		}

		internal struct ParameterOverride
		{
			public Type objectType
			{
				get
				{
					Type result;
					if ((result = InputProcessor.s_Processors.LookupTypeRegistration(this.objectRegistrationName)) == null)
					{
						result = (InputInteraction.s_Interactions.LookupTypeRegistration(this.objectRegistrationName) ?? InputBindingComposite.s_Composites.LookupTypeRegistration(this.objectRegistrationName));
					}
					return result;
				}
			}

			public ParameterOverride(string parameterName, InputBinding bindingMask, PrimitiveValue value = default(PrimitiveValue))
			{
				int num = parameterName.IndexOf(':');
				if (num < 0)
				{
					this.objectRegistrationName = null;
					this.parameter = parameterName;
				}
				else
				{
					this.objectRegistrationName = parameterName.Substring(0, num);
					this.parameter = parameterName.Substring(num + 1);
				}
				this.bindingMask = bindingMask;
				this.value = value;
			}

			public ParameterOverride(string objectRegistrationName, string parameterName, InputBinding bindingMask, PrimitiveValue value = default(PrimitiveValue))
			{
				this.objectRegistrationName = objectRegistrationName;
				this.parameter = parameterName;
				this.bindingMask = bindingMask;
				this.value = value;
			}

			public static InputActionRebindingExtensions.ParameterOverride? Find(InputActionMap actionMap, ref InputBinding binding, string parameterName, string objectRegistrationName)
			{
				InputActionRebindingExtensions.ParameterOverride? first = InputActionRebindingExtensions.ParameterOverride.Find(actionMap.m_ParameterOverrides, actionMap.m_ParameterOverridesCount, ref binding, parameterName, objectRegistrationName);
				InputActionAsset asset = actionMap.asset;
				InputActionRebindingExtensions.ParameterOverride? second = (asset != null) ? InputActionRebindingExtensions.ParameterOverride.Find(asset.m_ParameterOverrides, asset.m_ParameterOverridesCount, ref binding, parameterName, objectRegistrationName) : null;
				return InputActionRebindingExtensions.ParameterOverride.PickMoreSpecificOne(first, second);
			}

			private static InputActionRebindingExtensions.ParameterOverride? Find(InputActionRebindingExtensions.ParameterOverride[] overrides, int overrideCount, ref InputBinding binding, string parameterName, string objectRegistrationName)
			{
				InputActionRebindingExtensions.ParameterOverride? parameterOverride = null;
				for (int i = 0; i < overrideCount; i++)
				{
					ref InputActionRebindingExtensions.ParameterOverride ptr = ref overrides[i];
					if (string.Equals(parameterName, ptr.parameter, StringComparison.OrdinalIgnoreCase) && ptr.bindingMask.Matches(binding) && (ptr.objectRegistrationName == null || string.Equals(ptr.objectRegistrationName, objectRegistrationName, StringComparison.OrdinalIgnoreCase)))
					{
						if (parameterOverride == null)
						{
							parameterOverride = new InputActionRebindingExtensions.ParameterOverride?(ptr);
						}
						else
						{
							parameterOverride = InputActionRebindingExtensions.ParameterOverride.PickMoreSpecificOne(parameterOverride, new InputActionRebindingExtensions.ParameterOverride?(ptr));
						}
					}
				}
				return parameterOverride;
			}

			private static InputActionRebindingExtensions.ParameterOverride? PickMoreSpecificOne(InputActionRebindingExtensions.ParameterOverride? first, InputActionRebindingExtensions.ParameterOverride? second)
			{
				if (first == null)
				{
					return second;
				}
				if (second == null)
				{
					return first;
				}
				if (first.Value.objectRegistrationName != null && second.Value.objectRegistrationName == null)
				{
					return first;
				}
				if (second.Value.objectRegistrationName != null && first.Value.objectRegistrationName == null)
				{
					return second;
				}
				InputActionRebindingExtensions.ParameterOverride parameterOverride = first.Value;
				if (parameterOverride.bindingMask.effectivePath != null)
				{
					parameterOverride = second.Value;
					if (parameterOverride.bindingMask.effectivePath == null)
					{
						return first;
					}
				}
				parameterOverride = second.Value;
				if (parameterOverride.bindingMask.effectivePath != null)
				{
					parameterOverride = first.Value;
					if (parameterOverride.bindingMask.effectivePath == null)
					{
						return second;
					}
				}
				parameterOverride = first.Value;
				if (parameterOverride.bindingMask.action != null)
				{
					parameterOverride = second.Value;
					if (parameterOverride.bindingMask.action == null)
					{
						return first;
					}
				}
				parameterOverride = second.Value;
				if (parameterOverride.bindingMask.action != null)
				{
					parameterOverride = first.Value;
					if (parameterOverride.bindingMask.action == null)
					{
						return second;
					}
				}
				return first;
			}

			public string objectRegistrationName;

			public string parameter;

			public InputBinding bindingMask;

			public PrimitiveValue value;
		}

		public sealed class RebindingOperation : IDisposable
		{
			public InputAction action
			{
				get
				{
					return this.m_ActionToRebind;
				}
			}

			public InputBinding? bindingMask
			{
				get
				{
					return this.m_BindingMask;
				}
			}

			public InputControlList<InputControl> candidates
			{
				get
				{
					return this.m_Candidates;
				}
			}

			public ReadOnlyArray<float> scores
			{
				get
				{
					return new ReadOnlyArray<float>(this.m_Scores, 0, this.m_Candidates.Count);
				}
			}

			public ReadOnlyArray<float> magnitudes
			{
				get
				{
					return new ReadOnlyArray<float>(this.m_Magnitudes, 0, this.m_Candidates.Count);
				}
			}

			public InputControl selectedControl
			{
				get
				{
					if (this.m_Candidates.Count == 0)
					{
						return null;
					}
					return this.m_Candidates[0];
				}
			}

			public bool started
			{
				get
				{
					return (this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.Started) > (InputActionRebindingExtensions.RebindingOperation.Flags)0;
				}
			}

			public bool completed
			{
				get
				{
					return (this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.Completed) > (InputActionRebindingExtensions.RebindingOperation.Flags)0;
				}
			}

			public bool canceled
			{
				get
				{
					return (this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.Canceled) > (InputActionRebindingExtensions.RebindingOperation.Flags)0;
				}
			}

			public double startTime
			{
				get
				{
					return this.m_StartTime;
				}
			}

			public float timeout
			{
				get
				{
					return this.m_Timeout;
				}
			}

			public string expectedControlType
			{
				get
				{
					return this.m_ExpectedLayout;
				}
			}

			public InputActionRebindingExtensions.RebindingOperation WithAction(InputAction action)
			{
				this.ThrowIfRebindInProgress();
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				if (action.enabled)
				{
					throw new InvalidOperationException(string.Format("Cannot rebind action '{0}' while it is enabled", action));
				}
				this.m_ActionToRebind = action;
				if (!string.IsNullOrEmpty(action.expectedControlType))
				{
					this.WithExpectedControlType(action.expectedControlType);
				}
				else if (action.type == InputActionType.Button)
				{
					this.WithExpectedControlType("Button");
				}
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithMatchingEventsBeingSuppressed(bool value = true)
			{
				this.ThrowIfRebindInProgress();
				if (value)
				{
					this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.SuppressMatchingEvents;
				}
				else
				{
					this.m_Flags &= ~InputActionRebindingExtensions.RebindingOperation.Flags.SuppressMatchingEvents;
				}
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithCancelingThrough(string binding)
			{
				this.ThrowIfRebindInProgress();
				this.m_CancelBinding = binding;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithCancelingThrough(InputControl control)
			{
				this.ThrowIfRebindInProgress();
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				return this.WithCancelingThrough(control.path);
			}

			public InputActionRebindingExtensions.RebindingOperation WithExpectedControlType(string layoutName)
			{
				this.ThrowIfRebindInProgress();
				this.m_ExpectedLayout = new InternedString(layoutName);
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithExpectedControlType(Type type)
			{
				this.ThrowIfRebindInProgress();
				if (type != null && !typeof(InputControl).IsAssignableFrom(type))
				{
					throw new ArgumentException("Type '" + type.Name + "' is not an InputControl", "type");
				}
				this.m_ControlType = type;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithExpectedControlType<TControl>() where TControl : InputControl
			{
				this.ThrowIfRebindInProgress();
				return this.WithExpectedControlType(typeof(TControl));
			}

			public InputActionRebindingExtensions.RebindingOperation WithTargetBinding(int bindingIndex)
			{
				if (bindingIndex < 0)
				{
					throw new ArgumentOutOfRangeException("bindingIndex");
				}
				this.m_TargetBindingIndex = bindingIndex;
				if (this.m_ActionToRebind != null && bindingIndex < this.m_ActionToRebind.bindings.Count)
				{
					InputBinding inputBinding = this.m_ActionToRebind.bindings[bindingIndex];
					if (inputBinding.isPartOfComposite)
					{
						string nameOfComposite = this.m_ActionToRebind.ChangeBinding(bindingIndex).PreviousCompositeBinding(null).binding.GetNameOfComposite();
						string name = inputBinding.name;
						string expectedControlLayoutName = InputBindingComposite.GetExpectedControlLayoutName(nameOfComposite, name);
						if (!string.IsNullOrEmpty(expectedControlLayoutName))
						{
							this.WithExpectedControlType(expectedControlLayoutName);
						}
					}
					InputActionMap actionMap = this.action.actionMap;
					InputActionAsset inputActionAsset = (actionMap != null) ? actionMap.asset : null;
					if (inputActionAsset != null && !string.IsNullOrEmpty(inputBinding.groups))
					{
						string[] array = inputBinding.groups.Split(';', StringSplitOptions.None);
						for (int i = 0; i < array.Length; i++)
						{
							string group = array[i];
							int num = inputActionAsset.controlSchemes.IndexOf((InputControlScheme x) => group.Equals(x.bindingGroup, StringComparison.InvariantCultureIgnoreCase));
							if (num != -1)
							{
								foreach (InputControlScheme.DeviceRequirement deviceRequirement in inputActionAsset.controlSchemes[num].deviceRequirements)
								{
									this.WithControlsHavingToMatchPath(deviceRequirement.controlPath);
								}
							}
						}
					}
				}
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithBindingMask(InputBinding? bindingMask)
			{
				this.m_BindingMask = bindingMask;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithBindingGroup(string group)
			{
				return this.WithBindingMask(new InputBinding?(new InputBinding
				{
					groups = group
				}));
			}

			public InputActionRebindingExtensions.RebindingOperation WithoutGeneralizingPathOfSelectedControl()
			{
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.DontGeneralizePathOfSelectedControl;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithRebindAddingNewBinding(string group = null)
			{
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.AddNewBinding;
				this.m_BindingGroupForNewBinding = group;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithMagnitudeHavingToBeGreaterThan(float magnitude)
			{
				this.ThrowIfRebindInProgress();
				if (magnitude < 0f)
				{
					throw new ArgumentException(string.Format("Magnitude has to be positive but was {0}", magnitude), "magnitude");
				}
				this.m_MagnitudeThreshold = magnitude;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithoutIgnoringNoisyControls()
			{
				this.ThrowIfRebindInProgress();
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.DontIgnoreNoisyControls;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithControlsHavingToMatchPath(string path)
			{
				this.ThrowIfRebindInProgress();
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException("path");
				}
				for (int i = 0; i < this.m_IncludePathCount; i++)
				{
					if (string.Compare(this.m_IncludePaths[i], path, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						return this;
					}
				}
				ArrayHelpers.AppendWithCapacity<string>(ref this.m_IncludePaths, ref this.m_IncludePathCount, path, 10);
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithControlsExcluding(string path)
			{
				this.ThrowIfRebindInProgress();
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException("path");
				}
				for (int i = 0; i < this.m_ExcludePathCount; i++)
				{
					if (string.Compare(this.m_ExcludePaths[i], path, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						return this;
					}
				}
				ArrayHelpers.AppendWithCapacity<string>(ref this.m_ExcludePaths, ref this.m_ExcludePathCount, path, 10);
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation WithTimeout(float timeInSeconds)
			{
				this.m_Timeout = timeInSeconds;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnComplete(Action<InputActionRebindingExtensions.RebindingOperation> callback)
			{
				this.m_OnComplete = callback;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnCancel(Action<InputActionRebindingExtensions.RebindingOperation> callback)
			{
				this.m_OnCancel = callback;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnPotentialMatch(Action<InputActionRebindingExtensions.RebindingOperation> callback)
			{
				this.m_OnPotentialMatch = callback;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnGeneratePath(Func<InputControl, string> callback)
			{
				this.m_OnGeneratePath = callback;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnComputeScore(Func<InputControl, InputEventPtr, float> callback)
			{
				this.m_OnComputeScore = callback;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnApplyBinding(Action<InputActionRebindingExtensions.RebindingOperation, string> callback)
			{
				this.m_OnApplyBinding = callback;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation OnMatchWaitForAnother(float seconds)
			{
				this.m_WaitSecondsAfterMatch = seconds;
				return this;
			}

			public InputActionRebindingExtensions.RebindingOperation Start()
			{
				if (this.started)
				{
					return this;
				}
				if (this.m_ActionToRebind != null && this.m_ActionToRebind.bindings.Count == 0 && (this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.AddNewBinding) == (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					throw new InvalidOperationException(string.Format("Action '{0}' must have at least one existing binding or must be used with WithRebindingAddNewBinding()", this.action));
				}
				if (this.m_ActionToRebind == null && this.m_OnApplyBinding == null)
				{
					throw new InvalidOperationException("Must either have an action (call WithAction()) to apply binding to or have a custom callback to apply the binding (call OnApplyBinding())");
				}
				this.m_StartTime = InputState.currentTime;
				if (this.m_WaitSecondsAfterMatch > 0f || this.m_Timeout > 0f)
				{
					this.HookOnAfterUpdate();
					this.m_LastMatchTime = -1.0;
				}
				this.HookOnEvent();
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.Started;
				this.m_Flags &= ~InputActionRebindingExtensions.RebindingOperation.Flags.Canceled;
				this.m_Flags &= ~InputActionRebindingExtensions.RebindingOperation.Flags.Completed;
				return this;
			}

			public void Cancel()
			{
				if (!this.started)
				{
					return;
				}
				this.OnCancel();
			}

			public void Complete()
			{
				if (!this.started)
				{
					return;
				}
				this.OnComplete();
			}

			public void AddCandidate(InputControl control, float score, float magnitude = -1f)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				int num = this.m_Candidates.IndexOf(control);
				if (num != -1)
				{
					this.m_Scores[num] = score;
				}
				else
				{
					int count = this.m_Candidates.Count;
					int count2 = this.m_Candidates.Count;
					this.m_Candidates.Add(control);
					ArrayHelpers.AppendWithCapacity<float>(ref this.m_Scores, ref count, score, 10);
					ArrayHelpers.AppendWithCapacity<float>(ref this.m_Magnitudes, ref count2, magnitude, 10);
				}
				this.SortCandidatesByScore();
			}

			public void RemoveCandidate(InputControl control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				int num = this.m_Candidates.IndexOf(control);
				if (num == -1)
				{
					return;
				}
				int count = this.m_Candidates.Count;
				this.m_Candidates.RemoveAt(num);
				this.m_Scores.EraseAtWithCapacity(ref count, num);
			}

			public void Dispose()
			{
				this.UnhookOnEvent();
				this.UnhookOnAfterUpdate();
				this.m_Candidates.Dispose();
				this.m_LayoutCache.Clear();
			}

			~RebindingOperation()
			{
				this.Dispose();
			}

			public InputActionRebindingExtensions.RebindingOperation Reset()
			{
				this.Cancel();
				this.m_ActionToRebind = null;
				this.m_BindingMask = null;
				this.m_ControlType = null;
				this.m_ExpectedLayout = default(InternedString);
				this.m_IncludePathCount = 0;
				this.m_ExcludePathCount = 0;
				this.m_TargetBindingIndex = -1;
				this.m_BindingGroupForNewBinding = null;
				this.m_CancelBinding = null;
				this.m_MagnitudeThreshold = 0.2f;
				this.m_Timeout = 0f;
				this.m_WaitSecondsAfterMatch = 0f;
				this.m_Flags = (InputActionRebindingExtensions.RebindingOperation.Flags)0;
				Dictionary<InputControl, float> startingActuations = this.m_StartingActuations;
				if (startingActuations != null)
				{
					startingActuations.Clear();
				}
				return this;
			}

			private void HookOnEvent()
			{
				if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.OnEventHooked) != (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					return;
				}
				if (this.m_OnEventDelegate == null)
				{
					this.m_OnEventDelegate = new Action<InputEventPtr, InputDevice>(this.OnEvent);
				}
				InputSystem.onEvent += this.m_OnEventDelegate;
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.OnEventHooked;
			}

			private void UnhookOnEvent()
			{
				if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.OnEventHooked) == (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					return;
				}
				InputSystem.onEvent -= this.m_OnEventDelegate;
				this.m_Flags &= ~InputActionRebindingExtensions.RebindingOperation.Flags.OnEventHooked;
			}

			private unsafe void OnEvent(InputEventPtr eventPtr, InputDevice device)
			{
				FourCC type = eventPtr.type;
				if (type != 1398030676 && type != 1145852993)
				{
					return;
				}
				bool flag = false;
				bool flag2 = false;
				InputControlExtensions.Enumerate enumerate = InputControlExtensions.Enumerate.IncludeSyntheticControls | InputControlExtensions.Enumerate.IncludeNonLeafControls;
				if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.DontIgnoreNoisyControls) != (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					enumerate |= InputControlExtensions.Enumerate.IncludeNoisyControls;
				}
				foreach (InputControl inputControl in eventPtr.EnumerateControls(enumerate, device, 0f))
				{
					void* statePtrFromStateEventUnchecked = inputControl.GetStatePtrFromStateEventUnchecked(eventPtr, type);
					if (!string.IsNullOrEmpty(this.m_CancelBinding) && InputControlPath.Matches(this.m_CancelBinding, inputControl) && inputControl.HasValueChangeInState(statePtrFromStateEventUnchecked))
					{
						this.OnCancel();
						break;
					}
					if ((this.m_ExcludePathCount <= 0 || !InputActionRebindingExtensions.RebindingOperation.HavePathMatch(inputControl, this.m_ExcludePaths, this.m_ExcludePathCount)) && (this.m_IncludePathCount <= 0 || InputActionRebindingExtensions.RebindingOperation.HavePathMatch(inputControl, this.m_IncludePaths, this.m_IncludePathCount)) && (!(this.m_ControlType != null) || this.m_ControlType.IsInstanceOfType(inputControl)) && (this.m_ExpectedLayout.IsEmpty() || !(this.m_ExpectedLayout != inputControl.m_Layout) || InputControlLayout.s_Layouts.IsBasedOn(this.m_ExpectedLayout, inputControl.m_Layout)))
					{
						if (inputControl.CheckStateIsAtDefault(statePtrFromStateEventUnchecked, null))
						{
							if (!this.m_StartingActuations.ContainsKey(inputControl))
							{
								this.m_StartingActuations.Add(inputControl, 0f);
							}
							this.m_StartingActuations[inputControl] = 0f;
						}
						else
						{
							flag2 = true;
							float num = inputControl.EvaluateMagnitude(statePtrFromStateEventUnchecked);
							if (num >= 0f)
							{
								float magnitude;
								if (!this.m_StartingActuations.TryGetValue(inputControl, out magnitude))
								{
									magnitude = inputControl.magnitude;
									this.m_StartingActuations.Add(inputControl, magnitude);
								}
								if (Mathf.Abs(magnitude - num) < this.m_MagnitudeThreshold)
								{
									continue;
								}
							}
							float num2;
							if (this.m_OnComputeScore != null)
							{
								num2 = this.m_OnComputeScore(inputControl, eventPtr);
							}
							else
							{
								num2 = num;
								if (!inputControl.synthetic)
								{
									num2 += 1f;
								}
							}
							int num3 = this.m_Candidates.IndexOf(inputControl);
							if (num3 != -1)
							{
								if (this.m_Scores[num3] < num2)
								{
									flag = true;
									this.m_Scores[num3] = num2;
									if (this.m_WaitSecondsAfterMatch > 0f)
									{
										this.m_LastMatchTime = InputState.currentTime;
									}
								}
							}
							else
							{
								int count = this.m_Candidates.Count;
								int count2 = this.m_Candidates.Count;
								this.m_Candidates.Add(inputControl);
								ArrayHelpers.AppendWithCapacity<float>(ref this.m_Scores, ref count, num2, 10);
								ArrayHelpers.AppendWithCapacity<float>(ref this.m_Magnitudes, ref count2, num, 10);
								flag = true;
								if (this.m_WaitSecondsAfterMatch > 0f)
								{
									this.m_LastMatchTime = InputState.currentTime;
								}
							}
						}
					}
				}
				if (flag2 && (this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.SuppressMatchingEvents) != (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					eventPtr.handled = true;
				}
				if (flag && !this.canceled)
				{
					if (this.m_OnPotentialMatch != null)
					{
						this.SortCandidatesByScore();
						this.m_OnPotentialMatch(this);
						return;
					}
					if (this.m_WaitSecondsAfterMatch <= 0f)
					{
						this.OnComplete();
						return;
					}
					this.SortCandidatesByScore();
				}
			}

			private void SortCandidatesByScore()
			{
				int count = this.m_Candidates.Count;
				if (count <= 1)
				{
					return;
				}
				for (int i = 1; i < count; i++)
				{
					int num = i;
					while (num > 0 && this.m_Scores[num - 1] < this.m_Scores[num])
					{
						int index = num - 1;
						this.m_Scores.SwapElements(num, index);
						this.m_Candidates.SwapElements(num, index);
						this.m_Magnitudes.SwapElements(num, index);
						num--;
					}
				}
			}

			private static bool HavePathMatch(InputControl control, string[] paths, int pathCount)
			{
				for (int i = 0; i < pathCount; i++)
				{
					if (InputControlPath.MatchesPrefix(paths[i], control))
					{
						return true;
					}
				}
				return false;
			}

			private void HookOnAfterUpdate()
			{
				if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.OnAfterUpdateHooked) != (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					return;
				}
				if (this.m_OnAfterUpdateDelegate == null)
				{
					this.m_OnAfterUpdateDelegate = new Action(this.OnAfterUpdate);
				}
				InputSystem.onAfterUpdate += this.m_OnAfterUpdateDelegate;
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.OnAfterUpdateHooked;
			}

			private void UnhookOnAfterUpdate()
			{
				if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.OnAfterUpdateHooked) == (InputActionRebindingExtensions.RebindingOperation.Flags)0)
				{
					return;
				}
				InputSystem.onAfterUpdate -= this.m_OnAfterUpdateDelegate;
				this.m_Flags &= ~InputActionRebindingExtensions.RebindingOperation.Flags.OnAfterUpdateHooked;
			}

			private void OnAfterUpdate()
			{
				if (this.m_LastMatchTime < 0.0 && this.m_Timeout > 0f && InputState.currentTime - this.m_StartTime > (double)this.m_Timeout)
				{
					this.Cancel();
					return;
				}
				if (this.m_WaitSecondsAfterMatch <= 0f)
				{
					return;
				}
				if (this.m_LastMatchTime < 0.0)
				{
					return;
				}
				if (InputState.currentTime >= this.m_LastMatchTime + (double)this.m_WaitSecondsAfterMatch)
				{
					this.Complete();
				}
			}

			private void OnComplete()
			{
				this.SortCandidatesByScore();
				if (this.m_Candidates.Count > 0)
				{
					InputControl inputControl = this.m_Candidates[0];
					string text = inputControl.path;
					if (this.m_OnGeneratePath != null)
					{
						string text2 = this.m_OnGeneratePath(inputControl);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text2;
						}
						else if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.DontGeneralizePathOfSelectedControl) == (InputActionRebindingExtensions.RebindingOperation.Flags)0)
						{
							text = this.GeneratePathForControl(inputControl);
						}
					}
					else if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.DontGeneralizePathOfSelectedControl) == (InputActionRebindingExtensions.RebindingOperation.Flags)0)
					{
						text = this.GeneratePathForControl(inputControl);
					}
					if (this.m_OnApplyBinding != null)
					{
						this.m_OnApplyBinding(this, text);
					}
					else if ((this.m_Flags & InputActionRebindingExtensions.RebindingOperation.Flags.AddNewBinding) != (InputActionRebindingExtensions.RebindingOperation.Flags)0)
					{
						this.m_ActionToRebind.AddBinding(text, null, null, this.m_BindingGroupForNewBinding);
					}
					else if (this.m_TargetBindingIndex >= 0)
					{
						if (this.m_TargetBindingIndex >= this.m_ActionToRebind.bindings.Count)
						{
							throw new InvalidOperationException(string.Format("Target binding index {0} out of range for action '{1}' with {2} bindings", this.m_TargetBindingIndex, this.m_ActionToRebind, this.m_ActionToRebind.bindings.Count));
						}
						this.m_ActionToRebind.ApplyBindingOverride(this.m_TargetBindingIndex, text);
					}
					else if (this.m_BindingMask != null)
					{
						InputBinding value = this.m_BindingMask.Value;
						value.overridePath = text;
						this.m_ActionToRebind.ApplyBindingOverride(value);
					}
					else
					{
						this.m_ActionToRebind.ApplyBindingOverride(text, null, null);
					}
				}
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.Completed;
				Action<InputActionRebindingExtensions.RebindingOperation> onComplete = this.m_OnComplete;
				if (onComplete != null)
				{
					onComplete(this);
				}
				this.ResetAfterMatchCompleted();
			}

			private void OnCancel()
			{
				this.m_Flags |= InputActionRebindingExtensions.RebindingOperation.Flags.Canceled;
				Action<InputActionRebindingExtensions.RebindingOperation> onCancel = this.m_OnCancel;
				if (onCancel != null)
				{
					onCancel(this);
				}
				this.ResetAfterMatchCompleted();
			}

			private void ResetAfterMatchCompleted()
			{
				this.m_Flags &= ~InputActionRebindingExtensions.RebindingOperation.Flags.Started;
				this.m_Candidates.Clear();
				this.m_Candidates.Capacity = 0;
				this.m_StartTime = -1.0;
				this.m_StartingActuations.Clear();
				this.UnhookOnEvent();
				this.UnhookOnAfterUpdate();
			}

			private void ThrowIfRebindInProgress()
			{
				if (this.started)
				{
					throw new InvalidOperationException("Cannot reconfigure rebinding while operation is in progress");
				}
			}

			private string GeneratePathForControl(InputControl control)
			{
				InputDevice device = control.device;
				InternedString str = InputControlLayout.s_Layouts.FindLayoutThatIntroducesControl(control, this.m_LayoutCache);
				if (this.m_PathBuilder == null)
				{
					this.m_PathBuilder = new StringBuilder();
				}
				else
				{
					this.m_PathBuilder.Length = 0;
				}
				control.BuildPath(str, this.m_PathBuilder);
				return this.m_PathBuilder.ToString();
			}

			public const float kDefaultMagnitudeThreshold = 0.2f;

			private InputAction m_ActionToRebind;

			private InputBinding? m_BindingMask;

			private Type m_ControlType;

			private InternedString m_ExpectedLayout;

			private int m_IncludePathCount;

			private string[] m_IncludePaths;

			private int m_ExcludePathCount;

			private string[] m_ExcludePaths;

			private int m_TargetBindingIndex = -1;

			private string m_BindingGroupForNewBinding;

			private string m_CancelBinding;

			private float m_MagnitudeThreshold = 0.2f;

			private float[] m_Scores;

			private float[] m_Magnitudes;

			private double m_LastMatchTime;

			private double m_StartTime;

			private float m_Timeout;

			private float m_WaitSecondsAfterMatch;

			private InputControlList<InputControl> m_Candidates;

			private Action<InputActionRebindingExtensions.RebindingOperation> m_OnComplete;

			private Action<InputActionRebindingExtensions.RebindingOperation> m_OnCancel;

			private Action<InputActionRebindingExtensions.RebindingOperation> m_OnPotentialMatch;

			private Func<InputControl, string> m_OnGeneratePath;

			private Func<InputControl, InputEventPtr, float> m_OnComputeScore;

			private Action<InputActionRebindingExtensions.RebindingOperation, string> m_OnApplyBinding;

			private Action<InputEventPtr, InputDevice> m_OnEventDelegate;

			private Action m_OnAfterUpdateDelegate;

			private InputControlLayout.Cache m_LayoutCache;

			private StringBuilder m_PathBuilder;

			private InputActionRebindingExtensions.RebindingOperation.Flags m_Flags;

			private Dictionary<InputControl, float> m_StartingActuations = new Dictionary<InputControl, float>();

			[Flags]
			private enum Flags
			{
				Started = 1,
				Completed = 2,
				Canceled = 4,
				OnEventHooked = 8,
				OnAfterUpdateHooked = 16,
				DontIgnoreNoisyControls = 64,
				DontGeneralizePathOfSelectedControl = 128,
				AddNewBinding = 256,
				SuppressMatchingEvents = 512
			}
		}

		internal class DeferBindingResolutionWrapper : IDisposable
		{
			public void Acquire()
			{
				InputActionMap.s_DeferBindingResolution++;
			}

			public void Dispose()
			{
				if (InputActionMap.s_DeferBindingResolution > 0)
				{
					InputActionMap.s_DeferBindingResolution--;
				}
				if (InputActionMap.s_DeferBindingResolution == 0)
				{
					InputActionState.DeferredResolutionOfBindings();
				}
			}
		}
	}
}
