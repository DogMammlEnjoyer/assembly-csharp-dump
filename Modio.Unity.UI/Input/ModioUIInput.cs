using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modio.Unity.UI.Input
{
	public static class ModioUIInput
	{
		public static bool IsUsingGamepad { get; private set; }

		public static bool SuppressNoInputListenerWarning { get; set; }

		public static bool AnyBindingsExist { get; private set; }

		public static event Action<bool> SwappedControlScheme;

		public static void PressedAction(ModioUIInput.ModioAction action)
		{
			List<ValueTuple<Action, int>> list;
			if (!ModioUIInput.Handlers.TryGetValue(action, out list))
			{
				return;
			}
			ModioUIInput.CachedHandlersForCurrentCall.Clear();
			foreach (ValueTuple<Action, int> valueTuple in list)
			{
				if (valueTuple.Item2 != Time.frameCount)
				{
					ModioUIInput.CachedHandlersForCurrentCall.Add(valueTuple.Item1);
				}
			}
			foreach (Action action2 in ModioUIInput.CachedHandlersForCurrentCall)
			{
				if (action2 != null)
				{
					action2();
				}
			}
			ModioUIInput.CachedHandlersForCurrentCall.Clear();
		}

		public static void AddHandler(ModioUIInput.ModioAction action, Action onPressed)
		{
			if (!ModioUIInput.SuppressNoInputListenerWarning)
			{
				Debug.LogWarning("Modio's input system appears to be running without an input listener. You might not have controller and full keyboard support. Ensure you have ModioUI_InputCapture added to your scene\nIf you are using Unity's InputSystem, you can extract the following file: \"Assets\\Plugins\\ModioUI\\InputPackages\\InputSystem\\ModioInputListener_InputSystem.zip\"");
				ModioUIInput.SuppressNoInputListenerWarning = true;
			}
			List<ValueTuple<Action, int>> list;
			if (!ModioUIInput.Handlers.TryGetValue(action, out list))
			{
				list = new List<ValueTuple<Action, int>>();
				ModioUIInput.Handlers[action] = list;
			}
			list.Add(new ValueTuple<Action, int>(onPressed, Time.frameCount));
			if (list.Count == 1)
			{
				ModioUIInput.GetInputPromptDisplayInfo(action).UpdateListenerInfo(true);
			}
		}

		public static void RemoveHandler(ModioUIInput.ModioAction action, Action onPressed)
		{
			List<ValueTuple<Action, int>> list;
			if (!ModioUIInput.Handlers.TryGetValue(action, out list))
			{
				return;
			}
			bool flag = false;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].Item1 == onPressed)
				{
					list.RemoveAt(i);
					flag = true;
				}
			}
			if (flag && list.Count == 0)
			{
				ModioUIInput.GetInputPromptDisplayInfo(action).UpdateListenerInfo(false);
			}
		}

		public static void ControlSchemeChanged(bool isController)
		{
			ModioUIInput.IsUsingGamepad = isController;
			Action<bool> swappedControlScheme = ModioUIInput.SwappedControlScheme;
			if (swappedControlScheme == null)
			{
				return;
			}
			swappedControlScheme(isController);
		}

		public static void SetButtonPrompts(ModioUIInput.ModioAction action, List<string> textPrompts, List<Sprite> icons)
		{
			ModioUIInput.InputPromptDisplayInfo inputPromptDisplayInfo = ModioUIInput.GetInputPromptDisplayInfo(action);
			List<ValueTuple<Action, int>> list;
			bool hasListeners = ModioUIInput.Handlers.TryGetValue(action, out list) && list.Count > 0;
			inputPromptDisplayInfo.UpdateInfo(textPrompts, icons, hasListeners);
		}

		public static ModioUIInput.InputPromptDisplayInfo GetInputPromptDisplayInfo(ModioUIInput.ModioAction action)
		{
			ModioUIInput.InputPromptDisplayInfo inputPromptDisplayInfo;
			if (!ModioUIInput.Prompts.TryGetValue(action, out inputPromptDisplayInfo))
			{
				inputPromptDisplayInfo = new ModioUIInput.InputPromptDisplayInfo();
				ModioUIInput.Prompts[action] = inputPromptDisplayInfo;
			}
			return inputPromptDisplayInfo;
		}

		public static Vector2 GetRawCursor()
		{
			Func<Vector2> rawCursorProvider = ModioUIInput.RawCursorProvider;
			if (rawCursorProvider == null)
			{
				return Vector2.zero;
			}
			return rawCursorProvider();
		}

		[TupleElementNames(new string[]
		{
			"action",
			"frameAdded"
		})]
		private static readonly Dictionary<ModioUIInput.ModioAction, List<ValueTuple<Action, int>>> Handlers = new Dictionary<ModioUIInput.ModioAction, List<ValueTuple<Action, int>>>();

		private static readonly Dictionary<ModioUIInput.ModioAction, ModioUIInput.InputPromptDisplayInfo> Prompts = new Dictionary<ModioUIInput.ModioAction, ModioUIInput.InputPromptDisplayInfo>();

		private static readonly List<Action> CachedHandlersForCurrentCall = new List<Action>();

		public static Func<Vector2> RawCursorProvider;

		public class InputPromptDisplayInfo
		{
			public List<Sprite> Icons { get; private set; }

			public List<string> TextPrompts { get; private set; }

			public bool InputHasListeners { get; private set; }

			public event Action<ModioUIInput.InputPromptDisplayInfo> OnUpdated;

			public virtual void UpdateInfo(List<string> textPrompts, List<Sprite> icons, bool hasListeners)
			{
				if (textPrompts != null && textPrompts.Count > 0)
				{
					if (this.TextPrompts == null)
					{
						this.TextPrompts = new List<string>();
					}
					this.TextPrompts.Clear();
					this.TextPrompts.AddRange(textPrompts);
					ModioUIInput.AnyBindingsExist = true;
				}
				else
				{
					List<string> textPrompts2 = this.TextPrompts;
					if (textPrompts2 != null)
					{
						textPrompts2.Clear();
					}
				}
				if (icons != null && icons.Count > 0)
				{
					if (this.Icons == null)
					{
						this.Icons = new List<Sprite>();
					}
					this.Icons.Clear();
					this.Icons.AddRange(icons);
					ModioUIInput.AnyBindingsExist = true;
				}
				else
				{
					List<Sprite> icons2 = this.Icons;
					if (icons2 != null)
					{
						icons2.Clear();
					}
				}
				this.InputHasListeners = hasListeners;
				Action<ModioUIInput.InputPromptDisplayInfo> onUpdated = this.OnUpdated;
				if (onUpdated == null)
				{
					return;
				}
				onUpdated(this);
			}

			public void UpdateListenerInfo(bool hasListeners)
			{
				if (this.InputHasListeners == hasListeners)
				{
					return;
				}
				this.InputHasListeners = hasListeners;
				Action<ModioUIInput.InputPromptDisplayInfo> onUpdated = this.OnUpdated;
				if (onUpdated == null)
				{
					return;
				}
				onUpdated(this);
			}
		}

		public enum ModioAction
		{
			Cancel,
			Subscribe,
			Report,
			Filter,
			Sort,
			Search,
			TabLeft,
			TabRight,
			BuyTokens,
			FilterLeft,
			FilterRight,
			FilterClear,
			MoreOptions,
			SearchClear,
			SearchPageLeft,
			SearchPageRight,
			MoreFromThisCreator,
			DeveloperMenu
		}
	}
}
