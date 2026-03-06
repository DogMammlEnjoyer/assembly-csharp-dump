using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Modio.Unity.UI.Components.Selectables;
using Modio.Unity.UI.Navigation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels
{
	public class ModioDebugMenu : MonoBehaviour
	{
		public void Awake()
		{
			this._buttonPrefab.gameObject.SetActive(false);
			this._togglePrefab.gameObject.SetActive(false);
			this._textPrefab.gameObject.SetActive(false);
			if (this._labelPrefab != null)
			{
				this._labelPrefab.gameObject.SetActive(false);
			}
		}

		public void SetToDefaults()
		{
			Action onSetToDefaults = this._onSetToDefaults;
			if (onSetToDefaults == null)
			{
				return;
			}
			onSetToDefaults();
		}

		public void AddButton(string text, Action onClick)
		{
			ModioUIButton modioUIButton = Object.Instantiate<ModioUIButton>(this._buttonPrefab, this._buttonPrefab.transform.parent, false);
			modioUIButton.gameObject.SetActive(true);
			modioUIButton.GetComponentInChildren<TMP_Text>().text = text;
			modioUIButton.onClick.AddListener(delegate()
			{
				onClick();
			});
		}

		public void AddToggle(string text, Func<bool> initialValueGetter, Action<bool> onToggle)
		{
			ModioUIToggle toggle = Object.Instantiate<ModioUIToggle>(this._togglePrefab, this._buttonPrefab.transform.parent, false);
			toggle.gameObject.SetActive(true);
			toggle.GetComponentInChildren<TMP_Text>().text = text;
			this._onSetToDefaults = (Action)Delegate.Combine(this._onSetToDefaults, new Action(delegate()
			{
				toggle.isOn = initialValueGetter();
			}));
			toggle.onValueChanged.AddListener(delegate(bool b)
			{
				onToggle(b);
			});
		}

		public void AddLabel(string text)
		{
			TMP_Text tmp_Text = Object.Instantiate<TMP_Text>(this._labelPrefab, this._labelPrefab.transform.parent, false);
			tmp_Text.gameObject.SetActive(true);
			tmp_Text.text = text;
		}

		public void AddTextField(string text, Func<string> initialValueGetter, Action<string> onSubmitted)
		{
			ModioDebugMenu.<>c__DisplayClass10_0 CS$<>8__locals1 = new ModioDebugMenu.<>c__DisplayClass10_0();
			CS$<>8__locals1.initialValueGetter = initialValueGetter;
			CS$<>8__locals1.onSubmitted = onSubmitted;
			ModioInputFieldSelectionWrapper modioInputFieldSelectionWrapper = Object.Instantiate<ModioInputFieldSelectionWrapper>(this._textPrefab, this._buttonPrefab.transform.parent, false);
			modioInputFieldSelectionWrapper.gameObject.SetActive(true);
			modioInputFieldSelectionWrapper.GetComponentInChildren<TMP_Text>().text = text;
			CS$<>8__locals1.inputField = modioInputFieldSelectionWrapper.GetComponentInChildren<TMP_InputField>();
			this._onSetToDefaults = (Action)Delegate.Combine(this._onSetToDefaults, new Action(delegate()
			{
				CS$<>8__locals1.inputField.text = CS$<>8__locals1.initialValueGetter();
			}));
			CS$<>8__locals1.inputField.onDeselect.AddListener(new UnityAction<string>(CS$<>8__locals1.<AddTextField>g__OnTextFieldSubmit|1));
			CS$<>8__locals1.inputField.onSubmit.AddListener(new UnityAction<string>(CS$<>8__locals1.<AddTextField>g__OnTextFieldSubmit|1));
		}

		public void AddTextField(string text, Func<int> initialValueGetter, Action<int> onSubmitted)
		{
			this.AddTextField(text, () => initialValueGetter().ToString(), delegate(string s)
			{
				onSubmitted(int.Parse(s));
			});
		}

		public void AddTextField(string text, Func<long> initialValueGetter, Action<long> onSubmitted)
		{
			this.AddTextField(text, () => initialValueGetter().ToString(), delegate(string s)
			{
				onSubmitted((long)int.Parse(s));
			});
		}

		public void AddAllMethodsOrPropertiesWithAttribute<T>(Func<T, bool> predicate = null) where T : Attribute
		{
			ModioDebugMenu.<>c__DisplayClass13_0<T> CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (Type type in assemblies[i].GetTypes())
				{
					MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					MethodInfo[] array = methods;
					for (int k = 0; k < array.Length; k++)
					{
						MethodInfo methodInfo = array[k];
						T customAttribute = methodInfo.GetCustomAttribute<T>();
						if (customAttribute != null && (predicate == null || predicate(customAttribute)))
						{
							if (methodInfo.GetParameters().Length > 0)
							{
								Debug.LogError(string.Concat(new string[]
								{
									"Can't handle method ",
									methodInfo.Name,
									" on type ",
									type.Name,
									" because it has more than one parameter"
								}));
							}
							else
							{
								this.AddButton(ModioDebugMenu.Nicify(type.Name + ": " + methodInfo.Name), delegate
								{
									methodInfo.Invoke(null, null);
								});
							}
						}
					}
					PropertyInfo[] array2 = properties;
					for (int k = 0; k < array2.Length; k++)
					{
						ModioDebugMenu.<>c__DisplayClass13_2<T> CS$<>8__locals3 = new ModioDebugMenu.<>c__DisplayClass13_2<T>();
						CS$<>8__locals3.propertyInfo = array2[k];
						T customAttribute2 = CS$<>8__locals3.propertyInfo.GetCustomAttribute<T>();
						if (customAttribute2 != null && (predicate == null || predicate(customAttribute2)))
						{
							ModioDebugMenu.<>c__DisplayClass13_3<T> CS$<>8__locals4;
							CS$<>8__locals4.propertyName = ModioDebugMenu.Nicify(type.Name + ": " + CS$<>8__locals3.propertyInfo.Name);
							if (CS$<>8__locals3.propertyInfo.PropertyType == typeof(bool))
							{
								this.AddToggle(CS$<>8__locals4.propertyName, () => (bool)CS$<>8__locals3.propertyInfo.GetValue(null), delegate(bool b)
								{
									CS$<>8__locals3.propertyInfo.SetValue(null, b);
								});
							}
							if (CS$<>8__locals3.propertyInfo.PropertyType == typeof(string))
							{
								CS$<>8__locals3.<AddAllMethodsOrPropertiesWithAttribute>g__HookUpField|9((object o) => (string)o, (string s) => s, ref CS$<>8__locals1, ref CS$<>8__locals4);
							}
							if (CS$<>8__locals3.propertyInfo.PropertyType == typeof(int))
							{
								CS$<>8__locals3.<AddAllMethodsOrPropertiesWithAttribute>g__HookUpField|9((object o) => o.ToString(), (string s) => int.Parse(s), ref CS$<>8__locals1, ref CS$<>8__locals4);
							}
							if (CS$<>8__locals3.propertyInfo.PropertyType == typeof(long))
							{
								CS$<>8__locals3.<AddAllMethodsOrPropertiesWithAttribute>g__HookUpField|9((object o) => o.ToString(), (string s) => long.Parse(s), ref CS$<>8__locals1, ref CS$<>8__locals4);
							}
							else
							{
								Debug.LogWarning(string.Format("{0} hit property of unhandled type {1}", "ModioDebugMenu", CS$<>8__locals3.propertyInfo.PropertyType));
							}
						}
					}
				}
			}
		}

		public static string Nicify(string name)
		{
			return Regex.Replace(name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
		}

		[SerializeField]
		private ModioUIButton _buttonPrefab;

		[SerializeField]
		private ModioUIToggle _togglePrefab;

		[SerializeField]
		private ModioInputFieldSelectionWrapper _textPrefab;

		[SerializeField]
		private TMP_Text _labelPrefab;

		private Action _onSetToDefaults;
	}
}
