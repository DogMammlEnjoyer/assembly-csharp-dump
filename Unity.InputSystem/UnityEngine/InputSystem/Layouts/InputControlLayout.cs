using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Layouts
{
	public class InputControlLayout
	{
		public static InternedString DefaultVariant
		{
			get
			{
				return InputControlLayout.s_DefaultVariant;
			}
		}

		public InternedString name
		{
			get
			{
				return this.m_Name;
			}
		}

		public string displayName
		{
			get
			{
				return this.m_DisplayName ?? this.m_Name;
			}
		}

		public Type type
		{
			get
			{
				return this.m_Type;
			}
		}

		public InternedString variants
		{
			get
			{
				return this.m_Variants;
			}
		}

		public FourCC stateFormat
		{
			get
			{
				return this.m_StateFormat;
			}
		}

		public int stateSizeInBytes
		{
			get
			{
				return this.m_StateSizeInBytes;
			}
		}

		public IEnumerable<InternedString> baseLayouts
		{
			get
			{
				return this.m_BaseLayouts;
			}
		}

		public IEnumerable<InternedString> appliedOverrides
		{
			get
			{
				return this.m_AppliedOverrides;
			}
		}

		public ReadOnlyArray<InternedString> commonUsages
		{
			get
			{
				return new ReadOnlyArray<InternedString>(this.m_CommonUsages);
			}
		}

		public ReadOnlyArray<InputControlLayout.ControlItem> controls
		{
			get
			{
				return new ReadOnlyArray<InputControlLayout.ControlItem>(this.m_Controls);
			}
		}

		public bool updateBeforeRender
		{
			get
			{
				return this.m_UpdateBeforeRender.GetValueOrDefault();
			}
		}

		public bool isDeviceLayout
		{
			get
			{
				return typeof(InputDevice).IsAssignableFrom(this.m_Type);
			}
		}

		public bool isControlLayout
		{
			get
			{
				return !this.isDeviceLayout;
			}
		}

		public bool isOverride
		{
			get
			{
				return (this.m_Flags & InputControlLayout.Flags.IsOverride) > (InputControlLayout.Flags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_Flags |= InputControlLayout.Flags.IsOverride;
					return;
				}
				this.m_Flags &= ~InputControlLayout.Flags.IsOverride;
			}
		}

		public bool isGenericTypeOfDevice
		{
			get
			{
				return (this.m_Flags & InputControlLayout.Flags.IsGenericTypeOfDevice) > (InputControlLayout.Flags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_Flags |= InputControlLayout.Flags.IsGenericTypeOfDevice;
					return;
				}
				this.m_Flags &= ~InputControlLayout.Flags.IsGenericTypeOfDevice;
			}
		}

		public bool hideInUI
		{
			get
			{
				return (this.m_Flags & InputControlLayout.Flags.HideInUI) > (InputControlLayout.Flags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_Flags |= InputControlLayout.Flags.HideInUI;
					return;
				}
				this.m_Flags &= ~InputControlLayout.Flags.HideInUI;
			}
		}

		public bool isNoisy
		{
			get
			{
				return (this.m_Flags & InputControlLayout.Flags.IsNoisy) > (InputControlLayout.Flags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_Flags |= InputControlLayout.Flags.IsNoisy;
					return;
				}
				this.m_Flags &= ~InputControlLayout.Flags.IsNoisy;
			}
		}

		public bool? canRunInBackground
		{
			get
			{
				if ((this.m_Flags & InputControlLayout.Flags.CanRunInBackgroundIsSet) == (InputControlLayout.Flags)0)
				{
					return null;
				}
				return new bool?((this.m_Flags & InputControlLayout.Flags.CanRunInBackground) > (InputControlLayout.Flags)0);
			}
			internal set
			{
				if (value == null)
				{
					this.m_Flags &= ~InputControlLayout.Flags.CanRunInBackgroundIsSet;
					return;
				}
				this.m_Flags |= InputControlLayout.Flags.CanRunInBackgroundIsSet;
				if (value.Value)
				{
					this.m_Flags |= InputControlLayout.Flags.CanRunInBackground;
					return;
				}
				this.m_Flags &= ~InputControlLayout.Flags.CanRunInBackground;
			}
		}

		public InputControlLayout.ControlItem this[string path]
		{
			get
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException("path");
				}
				if (this.m_Controls != null)
				{
					for (int i = 0; i < this.m_Controls.Length; i++)
					{
						if (this.m_Controls[i].name == path)
						{
							return this.m_Controls[i];
						}
					}
				}
				throw new KeyNotFoundException(string.Format("Cannot find control '{0}' in layout '{1}'", path, this.name));
			}
		}

		public InputControlLayout.ControlItem? FindControl(InternedString path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (this.m_Controls == null)
			{
				return null;
			}
			for (int i = 0; i < this.m_Controls.Length; i++)
			{
				if (this.m_Controls[i].name == path)
				{
					return new InputControlLayout.ControlItem?(this.m_Controls[i]);
				}
			}
			return null;
		}

		public InputControlLayout.ControlItem? FindControlIncludingArrayElements(string path, out int arrayIndex)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			arrayIndex = -1;
			if (this.m_Controls == null)
			{
				return null;
			}
			int num = 0;
			int num2 = path.Length;
			while (num2 > 0 && char.IsDigit(path[num2 - 1]))
			{
				num2--;
				num *= 10;
				num += (int)(path[num2] - '0');
			}
			int num3 = 0;
			if (num2 < path.Length && num2 > 0)
			{
				num3 = num2;
			}
			for (int i = 0; i < this.m_Controls.Length; i++)
			{
				ref InputControlLayout.ControlItem ptr = ref this.m_Controls[i];
				if (string.Compare(ptr.name, path, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return new InputControlLayout.ControlItem?(ptr);
				}
				if (ptr.isArray && num3 > 0 && num3 == ptr.name.length && string.Compare(ptr.name.ToString(), 0, path, 0, num3, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					arrayIndex = num;
					return new InputControlLayout.ControlItem?(ptr);
				}
			}
			return null;
		}

		public Type GetValueType()
		{
			return TypeHelpers.GetGenericTypeArgumentFromHierarchy(this.type, typeof(InputControl<>), 0);
		}

		public static InputControlLayout FromType(string name, Type type)
		{
			List<InputControlLayout.ControlItem> list = new List<InputControlLayout.ControlItem>();
			InputControlLayoutAttribute customAttribute = type.GetCustomAttribute(true);
			FourCC stateFormat = default(FourCC);
			if (customAttribute != null && customAttribute.stateType != null)
			{
				InputControlLayout.AddControlItems(customAttribute.stateType, list, name);
				if (typeof(IInputStateTypeInfo).IsAssignableFrom(customAttribute.stateType))
				{
					stateFormat = ((IInputStateTypeInfo)Activator.CreateInstance(customAttribute.stateType)).format;
				}
			}
			else
			{
				InputControlLayout.AddControlItems(type, list, name);
			}
			if (customAttribute != null && !string.IsNullOrEmpty(customAttribute.stateFormat))
			{
				stateFormat = new FourCC(customAttribute.stateFormat);
			}
			InternedString variants = default(InternedString);
			if (customAttribute != null)
			{
				variants = new InternedString(customAttribute.variants);
			}
			InputControlLayout inputControlLayout = new InputControlLayout(name, type)
			{
				m_Controls = list.ToArray(),
				m_StateFormat = stateFormat,
				m_Variants = variants,
				m_UpdateBeforeRender = ((customAttribute != null) ? customAttribute.updateBeforeRenderInternal : null),
				isGenericTypeOfDevice = (customAttribute != null && customAttribute.isGenericTypeOfDevice),
				hideInUI = (customAttribute != null && customAttribute.hideInUI),
				m_Description = ((customAttribute != null) ? customAttribute.description : null),
				m_DisplayName = ((customAttribute != null) ? customAttribute.displayName : null),
				canRunInBackground = ((customAttribute != null) ? customAttribute.canRunInBackgroundInternal : null),
				isNoisy = (customAttribute != null && customAttribute.isNoisy)
			};
			if (((customAttribute != null) ? customAttribute.commonUsages : null) != null)
			{
				inputControlLayout.m_CommonUsages = ArrayHelpers.Select<string, InternedString>(customAttribute.commonUsages, (string x) => new InternedString(x));
			}
			return inputControlLayout;
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(InputControlLayout.LayoutJson.FromLayout(this), true);
		}

		public static InputControlLayout FromJson(string json)
		{
			return JsonUtility.FromJson<InputControlLayout.LayoutJson>(json).ToLayout();
		}

		private InputControlLayout(string name, Type type)
		{
			this.m_Name = new InternedString(name);
			this.m_Type = type;
		}

		private static void AddControlItems(Type type, List<InputControlLayout.ControlItem> controlLayouts, string layoutName)
		{
			InputControlLayout.AddControlItemsFromFields(type, controlLayouts, layoutName);
			InputControlLayout.AddControlItemsFromProperties(type, controlLayouts, layoutName);
		}

		private static void AddControlItemsFromFields(Type type, List<InputControlLayout.ControlItem> controlLayouts, string layoutName)
		{
			MemberInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			InputControlLayout.AddControlItemsFromMembers(fields, controlLayouts, layoutName);
		}

		private static void AddControlItemsFromProperties(Type type, List<InputControlLayout.ControlItem> controlLayouts, string layoutName)
		{
			MemberInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			InputControlLayout.AddControlItemsFromMembers(properties, controlLayouts, layoutName);
		}

		private static void AddControlItemsFromMembers(MemberInfo[] members, List<InputControlLayout.ControlItem> controlItems, string layoutName)
		{
			foreach (MemberInfo memberInfo in members)
			{
				if (!(memberInfo.DeclaringType == typeof(InputControl)))
				{
					Type valueType = TypeHelpers.GetValueType(memberInfo);
					if (valueType != null && valueType.IsValueType && typeof(IInputStateTypeInfo).IsAssignableFrom(valueType))
					{
						int count = controlItems.Count;
						InputControlLayout.AddControlItems(valueType, controlItems, layoutName);
						if (memberInfo as FieldInfo != null)
						{
							int num = Marshal.OffsetOf(memberInfo.DeclaringType, memberInfo.Name).ToInt32();
							int count2 = controlItems.Count;
							for (int j = count; j < count2; j++)
							{
								InputControlLayout.ControlItem value = controlItems[j];
								if (controlItems[j].offset != 4294967295U)
								{
									value.offset += (uint)num;
									controlItems[j] = value;
								}
							}
						}
					}
					InputControlAttribute[] array = memberInfo.GetCustomAttributes(false).ToArray<InputControlAttribute>();
					if (array.Length != 0 || (!(valueType == null) && typeof(InputControl).IsAssignableFrom(valueType) && !(memberInfo is PropertyInfo)))
					{
						InputControlLayout.AddControlItemsFromMember(memberInfo, array, controlItems);
					}
				}
			}
		}

		private static void AddControlItemsFromMember(MemberInfo member, InputControlAttribute[] attributes, List<InputControlLayout.ControlItem> controlItems)
		{
			if (attributes.Length == 0)
			{
				InputControlLayout.ControlItem item = InputControlLayout.CreateControlItemFromMember(member, null);
				controlItems.Add(item);
				return;
			}
			foreach (InputControlAttribute attribute in attributes)
			{
				InputControlLayout.ControlItem item2 = InputControlLayout.CreateControlItemFromMember(member, attribute);
				controlItems.Add(item2);
			}
		}

		private static InputControlLayout.ControlItem CreateControlItemFromMember(MemberInfo member, InputControlAttribute attribute)
		{
			string text = (attribute != null) ? attribute.name : null;
			if (string.IsNullOrEmpty(text))
			{
				text = member.Name;
			}
			bool flag = text.IndexOf('/') != -1;
			string displayName = (attribute != null) ? attribute.displayName : null;
			string shortDisplayName = (attribute != null) ? attribute.shortDisplayName : null;
			string text2 = (attribute != null) ? attribute.layout : null;
			if (string.IsNullOrEmpty(text2) && !flag && (!(member is FieldInfo) || member.GetCustomAttribute(false) == null))
			{
				text2 = InputControlLayout.InferLayoutFromValueType(TypeHelpers.GetValueType(member));
			}
			string text3 = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.variants))
			{
				text3 = attribute.variants;
			}
			uint offset = uint.MaxValue;
			if (attribute != null && attribute.offset != 4294967295U)
			{
				offset = attribute.offset;
			}
			else if (member is FieldInfo && !flag)
			{
				offset = (uint)Marshal.OffsetOf(member.DeclaringType, member.Name).ToInt32();
			}
			uint num = uint.MaxValue;
			if (attribute != null)
			{
				num = attribute.bit;
			}
			uint sizeInBits = 0U;
			if (attribute != null)
			{
				sizeInBits = attribute.sizeInBits;
			}
			FourCC format = default(FourCC);
			if (attribute != null && !string.IsNullOrEmpty(attribute.format))
			{
				format = new FourCC(attribute.format);
			}
			else if (!flag && num == 4294967295U)
			{
				format = InputStateBlock.GetPrimitiveFormatFromType(TypeHelpers.GetValueType(member));
			}
			InternedString[] array = null;
			if (attribute != null)
			{
				string[] array2 = ArrayHelpers.Join<string>(attribute.alias, attribute.aliases);
				if (array2 != null)
				{
					array = (from x in array2
					select new InternedString(x)).ToArray<InternedString>();
				}
			}
			InternedString[] array3 = null;
			if (attribute != null)
			{
				string[] array4 = ArrayHelpers.Join<string>(attribute.usage, attribute.usages);
				if (array4 != null)
				{
					array3 = (from x in array4
					select new InternedString(x)).ToArray<InternedString>();
				}
			}
			NamedValue[] array5 = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.parameters))
			{
				array5 = NamedValue.ParseMultiple(attribute.parameters);
			}
			NameAndParameters[] array6 = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.processors))
			{
				array6 = NameAndParameters.ParseMultiple(attribute.processors).ToArray<NameAndParameters>();
			}
			string useStateFrom = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.useStateFrom))
			{
				useStateFrom = attribute.useStateFrom;
			}
			bool isNoisy = false;
			if (attribute != null)
			{
				isNoisy = attribute.noisy;
			}
			bool dontReset = false;
			if (attribute != null)
			{
				dontReset = attribute.dontReset;
			}
			bool isSynthetic = false;
			if (attribute != null)
			{
				isSynthetic = attribute.synthetic;
			}
			int arraySize = 0;
			if (attribute != null)
			{
				arraySize = attribute.arraySize;
			}
			PrimitiveValue defaultState = default(PrimitiveValue);
			if (attribute != null)
			{
				defaultState = PrimitiveValue.FromObject(attribute.defaultState);
			}
			PrimitiveValue minValue = default(PrimitiveValue);
			PrimitiveValue maxValue = default(PrimitiveValue);
			if (attribute != null)
			{
				minValue = PrimitiveValue.FromObject(attribute.minValue);
				maxValue = PrimitiveValue.FromObject(attribute.maxValue);
			}
			return new InputControlLayout.ControlItem
			{
				name = new InternedString(text),
				displayName = displayName,
				shortDisplayName = shortDisplayName,
				layout = new InternedString(text2),
				variants = new InternedString(text3),
				useStateFrom = useStateFrom,
				format = format,
				offset = offset,
				bit = num,
				sizeInBits = sizeInBits,
				parameters = new ReadOnlyArray<NamedValue>(array5),
				processors = new ReadOnlyArray<NameAndParameters>(array6),
				usages = new ReadOnlyArray<InternedString>(array3),
				aliases = new ReadOnlyArray<InternedString>(array),
				isModifyingExistingControl = flag,
				isFirstDefinedInThisLayout = true,
				isNoisy = isNoisy,
				dontReset = dontReset,
				isSynthetic = isSynthetic,
				arraySize = arraySize,
				defaultState = defaultState,
				minValue = minValue,
				maxValue = maxValue
			};
		}

		private static string InferLayoutFromValueType(Type type)
		{
			InternedString str = InputControlLayout.s_Layouts.TryFindLayoutForType(type);
			if (str.IsEmpty())
			{
				InternedString internedString = new InternedString(type.Name);
				if (InputControlLayout.s_Layouts.HasLayout(internedString))
				{
					str = internedString;
				}
				else if (type.Name.EndsWith("Control"))
				{
					internedString = new InternedString(type.Name.Substring(0, type.Name.Length - "Control".Length));
					if (InputControlLayout.s_Layouts.HasLayout(internedString))
					{
						str = internedString;
					}
				}
			}
			return str;
		}

		public void MergeLayout(InputControlLayout other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			bool? updateBeforeRender = this.m_UpdateBeforeRender;
			this.m_UpdateBeforeRender = ((updateBeforeRender != null) ? updateBeforeRender : other.m_UpdateBeforeRender);
			if (this.m_Variants.IsEmpty())
			{
				this.m_Variants = other.m_Variants;
			}
			if (this.m_Type == null)
			{
				this.m_Type = other.m_Type;
			}
			else if (this.m_Type.IsAssignableFrom(other.m_Type))
			{
				this.m_Type = other.m_Type;
			}
			bool flag = !this.m_Variants.IsEmpty();
			if (this.m_StateFormat == default(FourCC))
			{
				this.m_StateFormat = other.m_StateFormat;
			}
			this.m_CommonUsages = ArrayHelpers.Merge<InternedString>(other.m_CommonUsages, this.m_CommonUsages);
			this.m_AppliedOverrides.Merge(other.m_AppliedOverrides);
			if (string.IsNullOrEmpty(this.m_DisplayName))
			{
				this.m_DisplayName = other.m_DisplayName;
			}
			if (this.m_Controls == null)
			{
				this.m_Controls = other.m_Controls;
				return;
			}
			if (other.m_Controls != null)
			{
				InputControlLayout.ControlItem[] controls = other.m_Controls;
				List<InputControlLayout.ControlItem> list = new List<InputControlLayout.ControlItem>();
				List<string> list2 = new List<string>();
				Dictionary<string, InputControlLayout.ControlItem> dictionary = InputControlLayout.CreateLookupTableForControls(controls, list2);
				foreach (KeyValuePair<string, InputControlLayout.ControlItem> keyValuePair in InputControlLayout.CreateLookupTableForControls(this.m_Controls, null))
				{
					InputControlLayout.ControlItem other2;
					if (dictionary.TryGetValue(keyValuePair.Key, out other2))
					{
						InputControlLayout.ControlItem item = keyValuePair.Value.Merge(other2);
						list.Add(item);
						dictionary.Remove(keyValuePair.Key);
					}
					else if (keyValuePair.Value.variants.IsEmpty() || keyValuePair.Value.variants == InputControlLayout.DefaultVariant)
					{
						bool flag2 = false;
						if (flag)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (InputControlLayout.VariantsMatch(this.m_Variants.ToLower(), list2[i]))
								{
									string key = keyValuePair.Key + "@" + list2[i];
									if (dictionary.TryGetValue(key, out other2))
									{
										InputControlLayout.ControlItem item2 = keyValuePair.Value.Merge(other2);
										list.Add(item2);
										dictionary.Remove(key);
										flag2 = true;
									}
								}
							}
						}
						else
						{
							foreach (string str in list2)
							{
								string key2 = keyValuePair.Key + "@" + str;
								if (dictionary.TryGetValue(key2, out other2))
								{
									InputControlLayout.ControlItem item3 = keyValuePair.Value.Merge(other2);
									list.Add(item3);
									dictionary.Remove(key2);
									flag2 = true;
								}
							}
						}
						if (!flag2)
						{
							list.Add(keyValuePair.Value);
						}
					}
					else if (dictionary.TryGetValue(keyValuePair.Value.name.ToLower(), out other2))
					{
						InputControlLayout.ControlItem item4 = keyValuePair.Value.Merge(other2);
						list.Add(item4);
						dictionary.Remove(keyValuePair.Value.name.ToLower());
					}
					else if (InputControlLayout.VariantsMatch(this.m_Variants, keyValuePair.Value.variants))
					{
						list.Add(keyValuePair.Value);
					}
				}
				if (!flag)
				{
					int count = list.Count;
					list.AddRange(dictionary.Values);
					for (int j = count; j < list.Count; j++)
					{
						InputControlLayout.ControlItem value = list[j];
						value.isFirstDefinedInThisLayout = false;
						list[j] = value;
					}
				}
				else
				{
					int count2 = list.Count;
					list.AddRange(from x in dictionary.Values
					where InputControlLayout.VariantsMatch(this.m_Variants, x.variants)
					select x);
					for (int k = count2; k < list.Count; k++)
					{
						InputControlLayout.ControlItem value2 = list[k];
						value2.isFirstDefinedInThisLayout = false;
						list[k] = value2;
					}
				}
				this.m_Controls = list.ToArray();
			}
		}

		private static Dictionary<string, InputControlLayout.ControlItem> CreateLookupTableForControls(InputControlLayout.ControlItem[] controlItems, List<string> variants = null)
		{
			Dictionary<string, InputControlLayout.ControlItem> dictionary = new Dictionary<string, InputControlLayout.ControlItem>();
			int i = 0;
			while (i < controlItems.Length)
			{
				string text = controlItems[i].name.ToLower();
				InternedString variants2 = controlItems[i].variants;
				if (variants2.IsEmpty() || !(variants2 != InputControlLayout.DefaultVariant))
				{
					goto IL_EC;
				}
				if (variants2.ToString().IndexOf(";"[0]) != -1)
				{
					foreach (string text2 in variants2.ToLower().Split(";"[0], StringSplitOptions.None))
					{
						if (variants != null)
						{
							variants.Add(text2);
						}
						text = text + "@" + text2;
						dictionary[text] = controlItems[i];
					}
				}
				else
				{
					text = text + "@" + variants2.ToLower();
					if (variants != null)
					{
						variants.Add(variants2.ToLower());
						goto IL_EC;
					}
					goto IL_EC;
				}
				IL_FA:
				i++;
				continue;
				IL_EC:
				dictionary[text] = controlItems[i];
				goto IL_FA;
			}
			return dictionary;
		}

		internal static bool VariantsMatch(InternedString expected, InternedString actual)
		{
			return InputControlLayout.VariantsMatch(expected.ToLower(), actual.ToLower());
		}

		internal static bool VariantsMatch(string expected, string actual)
		{
			return (actual != null && StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(InputControlLayout.DefaultVariant, actual, ";"[0])) || expected == null || actual == null || StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(expected, actual, ";"[0]);
		}

		internal static void ParseHeaderFieldsFromJson(string json, out InternedString name, out InlinedArray<InternedString> baseLayouts, out InputDeviceMatcher deviceMatcher)
		{
			InputControlLayout.LayoutJsonNameAndDescriptorOnly layoutJsonNameAndDescriptorOnly = JsonUtility.FromJson<InputControlLayout.LayoutJsonNameAndDescriptorOnly>(json);
			name = new InternedString(layoutJsonNameAndDescriptorOnly.name);
			baseLayouts = default(InlinedArray<InternedString>);
			if (!string.IsNullOrEmpty(layoutJsonNameAndDescriptorOnly.extend))
			{
				baseLayouts.Append(new InternedString(layoutJsonNameAndDescriptorOnly.extend));
			}
			if (layoutJsonNameAndDescriptorOnly.extendMultiple != null)
			{
				foreach (string text in layoutJsonNameAndDescriptorOnly.extendMultiple)
				{
					baseLayouts.Append(new InternedString(text));
				}
			}
			deviceMatcher = layoutJsonNameAndDescriptorOnly.device.ToMatcher();
		}

		internal static ref InputControlLayout.Cache cache
		{
			get
			{
				return ref InputControlLayout.s_CacheInstance;
			}
		}

		internal static InputControlLayout.CacheRefInstance CacheRef()
		{
			InputControlLayout.s_CacheInstanceRef++;
			return new InputControlLayout.CacheRefInstance
			{
				valid = true
			};
		}

		private static InternedString s_DefaultVariant = new InternedString("Default");

		public const string VariantSeparator = ";";

		private InternedString m_Name;

		private Type m_Type;

		private InternedString m_Variants;

		private FourCC m_StateFormat;

		internal int m_StateSizeInBytes;

		internal bool? m_UpdateBeforeRender;

		internal InlinedArray<InternedString> m_BaseLayouts;

		private InlinedArray<InternedString> m_AppliedOverrides;

		private InternedString[] m_CommonUsages;

		internal InputControlLayout.ControlItem[] m_Controls;

		internal string m_DisplayName;

		private string m_Description;

		private InputControlLayout.Flags m_Flags;

		internal static InputControlLayout.Collection s_Layouts;

		internal static InputControlLayout.Cache s_CacheInstance;

		internal static int s_CacheInstanceRef;

		public struct ControlItem
		{
			public InternedString name { readonly get; internal set; }

			public InternedString layout { readonly get; internal set; }

			public InternedString variants { readonly get; internal set; }

			public string useStateFrom { readonly get; internal set; }

			public string displayName { readonly get; internal set; }

			public string shortDisplayName { readonly get; internal set; }

			public ReadOnlyArray<InternedString> usages { readonly get; internal set; }

			public ReadOnlyArray<InternedString> aliases { readonly get; internal set; }

			public ReadOnlyArray<NamedValue> parameters { readonly get; internal set; }

			public ReadOnlyArray<NameAndParameters> processors { readonly get; internal set; }

			public uint offset { readonly get; internal set; }

			public uint bit { readonly get; internal set; }

			public uint sizeInBits { readonly get; internal set; }

			public FourCC format { readonly get; internal set; }

			private InputControlLayout.ControlItem.Flags flags { readonly get; set; }

			public int arraySize { readonly get; internal set; }

			public PrimitiveValue defaultState { readonly get; internal set; }

			public PrimitiveValue minValue { readonly get; internal set; }

			public PrimitiveValue maxValue { readonly get; internal set; }

			public bool isModifyingExistingControl
			{
				get
				{
					return (this.flags & InputControlLayout.ControlItem.Flags.isModifyingExistingControl) == InputControlLayout.ControlItem.Flags.isModifyingExistingControl;
				}
				internal set
				{
					if (value)
					{
						this.flags |= InputControlLayout.ControlItem.Flags.isModifyingExistingControl;
						return;
					}
					this.flags &= ~InputControlLayout.ControlItem.Flags.isModifyingExistingControl;
				}
			}

			public bool isNoisy
			{
				get
				{
					return (this.flags & InputControlLayout.ControlItem.Flags.IsNoisy) == InputControlLayout.ControlItem.Flags.IsNoisy;
				}
				internal set
				{
					if (value)
					{
						this.flags |= InputControlLayout.ControlItem.Flags.IsNoisy;
						return;
					}
					this.flags &= ~InputControlLayout.ControlItem.Flags.IsNoisy;
				}
			}

			public bool isSynthetic
			{
				get
				{
					return (this.flags & InputControlLayout.ControlItem.Flags.IsSynthetic) == InputControlLayout.ControlItem.Flags.IsSynthetic;
				}
				internal set
				{
					if (value)
					{
						this.flags |= InputControlLayout.ControlItem.Flags.IsSynthetic;
						return;
					}
					this.flags &= ~InputControlLayout.ControlItem.Flags.IsSynthetic;
				}
			}

			public bool dontReset
			{
				get
				{
					return (this.flags & InputControlLayout.ControlItem.Flags.DontReset) == InputControlLayout.ControlItem.Flags.DontReset;
				}
				internal set
				{
					if (value)
					{
						this.flags |= InputControlLayout.ControlItem.Flags.DontReset;
						return;
					}
					this.flags &= ~InputControlLayout.ControlItem.Flags.DontReset;
				}
			}

			public bool isFirstDefinedInThisLayout
			{
				get
				{
					return (this.flags & InputControlLayout.ControlItem.Flags.IsFirstDefinedInThisLayout) > (InputControlLayout.ControlItem.Flags)0;
				}
				internal set
				{
					if (value)
					{
						this.flags |= InputControlLayout.ControlItem.Flags.IsFirstDefinedInThisLayout;
						return;
					}
					this.flags &= ~InputControlLayout.ControlItem.Flags.IsFirstDefinedInThisLayout;
				}
			}

			public bool isArray
			{
				get
				{
					return this.arraySize != 0;
				}
			}

			public InputControlLayout.ControlItem Merge(InputControlLayout.ControlItem other)
			{
				InputControlLayout.ControlItem result = default(InputControlLayout.ControlItem);
				result.name = this.name;
				result.isModifyingExistingControl = this.isModifyingExistingControl;
				result.displayName = (string.IsNullOrEmpty(this.displayName) ? other.displayName : this.displayName);
				result.shortDisplayName = (string.IsNullOrEmpty(this.shortDisplayName) ? other.shortDisplayName : this.shortDisplayName);
				result.layout = (this.layout.IsEmpty() ? other.layout : this.layout);
				result.variants = (this.variants.IsEmpty() ? other.variants : this.variants);
				result.useStateFrom = (this.useStateFrom ?? other.useStateFrom);
				result.arraySize = ((!this.isArray) ? other.arraySize : this.arraySize);
				result.isNoisy = (this.isNoisy || other.isNoisy);
				result.dontReset = (this.dontReset || other.dontReset);
				result.isSynthetic = (this.isSynthetic || other.isSynthetic);
				result.isFirstDefinedInThisLayout = false;
				if (this.offset != 4294967295U)
				{
					result.offset = this.offset;
				}
				else
				{
					result.offset = other.offset;
				}
				if (this.bit != 4294967295U)
				{
					result.bit = this.bit;
				}
				else
				{
					result.bit = other.bit;
				}
				if (this.format != 0)
				{
					result.format = this.format;
				}
				else
				{
					result.format = other.format;
				}
				if (this.sizeInBits != 0U)
				{
					result.sizeInBits = this.sizeInBits;
				}
				else
				{
					result.sizeInBits = other.sizeInBits;
				}
				if (this.aliases.Count > 0)
				{
					result.aliases = this.aliases;
				}
				else
				{
					result.aliases = other.aliases;
				}
				if (this.usages.Count > 0)
				{
					result.usages = this.usages;
				}
				else
				{
					result.usages = other.usages;
				}
				if (this.parameters.Count == 0)
				{
					result.parameters = other.parameters;
				}
				else
				{
					result.parameters = this.parameters;
				}
				if (this.processors.Count == 0)
				{
					result.processors = other.processors;
				}
				else
				{
					result.processors = this.processors;
				}
				if (!string.IsNullOrEmpty(this.displayName))
				{
					result.displayName = this.displayName;
				}
				else
				{
					result.displayName = other.displayName;
				}
				if (!this.defaultState.isEmpty)
				{
					result.defaultState = this.defaultState;
				}
				else
				{
					result.defaultState = other.defaultState;
				}
				if (!this.minValue.isEmpty)
				{
					result.minValue = this.minValue;
				}
				else
				{
					result.minValue = other.minValue;
				}
				if (!this.maxValue.isEmpty)
				{
					result.maxValue = this.maxValue;
				}
				else
				{
					result.maxValue = other.maxValue;
				}
				return result;
			}

			[Flags]
			private enum Flags
			{
				isModifyingExistingControl = 1,
				IsNoisy = 2,
				IsSynthetic = 4,
				IsFirstDefinedInThisLayout = 8,
				DontReset = 16
			}
		}

		public class Builder
		{
			public string name { get; set; }

			public string displayName { get; set; }

			public Type type { get; set; }

			public FourCC stateFormat { get; set; }

			public int stateSizeInBytes { get; set; }

			public string extendsLayout
			{
				get
				{
					return this.m_ExtendsLayout;
				}
				set
				{
					if (!string.IsNullOrEmpty(value))
					{
						this.m_ExtendsLayout = value;
						return;
					}
					this.m_ExtendsLayout = null;
				}
			}

			public bool? updateBeforeRender { get; set; }

			public ReadOnlyArray<InputControlLayout.ControlItem> controls
			{
				get
				{
					return new ReadOnlyArray<InputControlLayout.ControlItem>(this.m_Controls, 0, this.m_ControlCount);
				}
			}

			public InputControlLayout.Builder.ControlBuilder AddControl(string name)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException(name);
				}
				int index = ArrayHelpers.AppendWithCapacity<InputControlLayout.ControlItem>(ref this.m_Controls, ref this.m_ControlCount, new InputControlLayout.ControlItem
				{
					name = new InternedString(name),
					isModifyingExistingControl = (name.IndexOf('/') != -1),
					offset = uint.MaxValue,
					bit = uint.MaxValue
				}, 10);
				return new InputControlLayout.Builder.ControlBuilder
				{
					builder = this,
					index = index
				};
			}

			public InputControlLayout.Builder WithName(string name)
			{
				this.name = name;
				return this;
			}

			public InputControlLayout.Builder WithDisplayName(string displayName)
			{
				this.displayName = displayName;
				return this;
			}

			public InputControlLayout.Builder WithType<T>() where T : InputControl
			{
				this.type = typeof(T);
				return this;
			}

			public InputControlLayout.Builder WithFormat(FourCC format)
			{
				this.stateFormat = format;
				return this;
			}

			public InputControlLayout.Builder WithFormat(string format)
			{
				return this.WithFormat(new FourCC(format));
			}

			public InputControlLayout.Builder WithSizeInBytes(int sizeInBytes)
			{
				this.stateSizeInBytes = sizeInBytes;
				return this;
			}

			public InputControlLayout.Builder Extend(string baseLayoutName)
			{
				this.extendsLayout = baseLayoutName;
				return this;
			}

			public InputControlLayout Build()
			{
				InputControlLayout.ControlItem[] array = null;
				if (this.m_ControlCount > 0)
				{
					array = new InputControlLayout.ControlItem[this.m_ControlCount];
					Array.Copy(this.m_Controls, array, this.m_ControlCount);
				}
				return new InputControlLayout(new InternedString(this.name), (this.type == null && string.IsNullOrEmpty(this.extendsLayout)) ? typeof(InputDevice) : this.type)
				{
					m_DisplayName = this.displayName,
					m_StateFormat = this.stateFormat,
					m_StateSizeInBytes = this.stateSizeInBytes,
					m_BaseLayouts = ((!string.IsNullOrEmpty(this.extendsLayout)) ? new InlinedArray<InternedString>(new InternedString(this.extendsLayout)) : default(InlinedArray<InternedString>)),
					m_Controls = array,
					m_UpdateBeforeRender = this.updateBeforeRender
				};
			}

			private string m_ExtendsLayout;

			private int m_ControlCount;

			private InputControlLayout.ControlItem[] m_Controls;

			public struct ControlBuilder
			{
				public InputControlLayout.Builder.ControlBuilder WithDisplayName(string displayName)
				{
					this.builder.m_Controls[this.index].displayName = displayName;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithLayout(string layout)
				{
					if (string.IsNullOrEmpty(layout))
					{
						throw new ArgumentException("Layout name cannot be null or empty", "layout");
					}
					this.builder.m_Controls[this.index].layout = new InternedString(layout);
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithFormat(FourCC format)
				{
					this.builder.m_Controls[this.index].format = format;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithFormat(string format)
				{
					return this.WithFormat(new FourCC(format));
				}

				public InputControlLayout.Builder.ControlBuilder WithByteOffset(uint offset)
				{
					this.builder.m_Controls[this.index].offset = offset;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithBitOffset(uint bit)
				{
					this.builder.m_Controls[this.index].bit = bit;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder IsSynthetic(bool value)
				{
					this.builder.m_Controls[this.index].isSynthetic = value;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder IsNoisy(bool value)
				{
					this.builder.m_Controls[this.index].isNoisy = value;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder DontReset(bool value)
				{
					this.builder.m_Controls[this.index].dontReset = value;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithSizeInBits(uint sizeInBits)
				{
					this.builder.m_Controls[this.index].sizeInBits = sizeInBits;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithRange(float minValue, float maxValue)
				{
					this.builder.m_Controls[this.index].minValue = minValue;
					this.builder.m_Controls[this.index].maxValue = maxValue;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithUsages(params InternedString[] usages)
				{
					if (usages == null || usages.Length == 0)
					{
						return this;
					}
					for (int i = 0; i < usages.Length; i++)
					{
						if (usages[i].IsEmpty())
						{
							throw new ArgumentException(string.Format("Empty usage entry at index {0} for control '{1}' in layout '{2}'", i, this.builder.m_Controls[this.index].name, this.builder.name), "usages");
						}
					}
					this.builder.m_Controls[this.index].usages = new ReadOnlyArray<InternedString>(usages);
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithUsages(IEnumerable<string> usages)
				{
					InternedString[] usages2 = (from x in usages
					select new InternedString(x)).ToArray<InternedString>();
					return this.WithUsages(usages2);
				}

				public InputControlLayout.Builder.ControlBuilder WithUsages(params string[] usages)
				{
					return this.WithUsages(usages);
				}

				public InputControlLayout.Builder.ControlBuilder WithParameters(string parameters)
				{
					if (string.IsNullOrEmpty(parameters))
					{
						return this;
					}
					NamedValue[] array = NamedValue.ParseMultiple(parameters);
					this.builder.m_Controls[this.index].parameters = new ReadOnlyArray<NamedValue>(array);
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithProcessors(string processors)
				{
					if (string.IsNullOrEmpty(processors))
					{
						return this;
					}
					NameAndParameters[] array = NameAndParameters.ParseMultiple(processors).ToArray<NameAndParameters>();
					this.builder.m_Controls[this.index].processors = new ReadOnlyArray<NameAndParameters>(array);
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder WithDefaultState(PrimitiveValue value)
				{
					this.builder.m_Controls[this.index].defaultState = value;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder UsingStateFrom(string path)
				{
					if (string.IsNullOrEmpty(path))
					{
						return this;
					}
					this.builder.m_Controls[this.index].useStateFrom = path;
					return this;
				}

				public InputControlLayout.Builder.ControlBuilder AsArrayOfControlsWithSize(int arraySize)
				{
					this.builder.m_Controls[this.index].arraySize = arraySize;
					return this;
				}

				internal InputControlLayout.Builder builder;

				internal int index;
			}
		}

		[Flags]
		private enum Flags
		{
			IsGenericTypeOfDevice = 1,
			HideInUI = 2,
			IsOverride = 4,
			CanRunInBackground = 8,
			CanRunInBackgroundIsSet = 16,
			IsNoisy = 32
		}

		[Serializable]
		internal struct LayoutJsonNameAndDescriptorOnly
		{
			public string name;

			public string extend;

			public string[] extendMultiple;

			public InputDeviceMatcher.MatcherJson device;
		}

		[Serializable]
		private struct LayoutJson
		{
			public InputControlLayout ToLayout()
			{
				Type type = null;
				if (!string.IsNullOrEmpty(this.type))
				{
					type = Type.GetType(this.type, false);
					if (type == null)
					{
						Debug.Log(string.Concat(new string[]
						{
							"Cannot find type '",
							this.type,
							"' used by layout '",
							this.name,
							"'; falling back to using InputDevice"
						}));
						type = typeof(InputDevice);
					}
					else if (!typeof(InputControl).IsAssignableFrom(type))
					{
						throw new InvalidOperationException(string.Concat(new string[]
						{
							"'",
							this.type,
							"' used by layout '",
							this.name,
							"' is not an InputControl"
						}));
					}
				}
				else if (string.IsNullOrEmpty(this.extend))
				{
					type = typeof(InputDevice);
				}
				InputControlLayout inputControlLayout = new InputControlLayout(this.name, type);
				inputControlLayout.m_DisplayName = this.displayName;
				inputControlLayout.m_Description = this.description;
				inputControlLayout.isGenericTypeOfDevice = this.isGenericTypeOfDevice;
				inputControlLayout.hideInUI = this.hideInUI;
				inputControlLayout.m_Variants = new InternedString(this.variant);
				inputControlLayout.m_CommonUsages = ArrayHelpers.Select<string, InternedString>(this.commonUsages, (string x) => new InternedString(x));
				InputControlLayout inputControlLayout2 = inputControlLayout;
				if (!string.IsNullOrEmpty(this.format))
				{
					inputControlLayout2.m_StateFormat = new FourCC(this.format);
				}
				if (!string.IsNullOrEmpty(this.extend))
				{
					inputControlLayout2.m_BaseLayouts.Append(new InternedString(this.extend));
				}
				if (this.extendMultiple != null)
				{
					foreach (string text in this.extendMultiple)
					{
						inputControlLayout2.m_BaseLayouts.Append(new InternedString(text));
					}
				}
				if (!string.IsNullOrEmpty(this.beforeRender))
				{
					string a = this.beforeRender.ToLowerInvariant();
					if (a == "ignore")
					{
						inputControlLayout2.m_UpdateBeforeRender = new bool?(false);
					}
					else
					{
						if (!(a == "update"))
						{
							throw new InvalidOperationException("Invalid beforeRender setting '" + this.beforeRender + "' (should be 'ignore' or 'update')");
						}
						inputControlLayout2.m_UpdateBeforeRender = new bool?(true);
					}
				}
				if (!string.IsNullOrEmpty(this.runInBackground))
				{
					string a2 = this.runInBackground.ToLowerInvariant();
					if (a2 == "enabled")
					{
						inputControlLayout2.canRunInBackground = new bool?(true);
					}
					else
					{
						if (!(a2 == "disabled"))
						{
							throw new InvalidOperationException("Invalid runInBackground setting '" + this.beforeRender + "' (should be 'enabled' or 'disabled')");
						}
						inputControlLayout2.canRunInBackground = new bool?(false);
					}
				}
				if (this.controls != null)
				{
					List<InputControlLayout.ControlItem> list = new List<InputControlLayout.ControlItem>();
					foreach (InputControlLayout.ControlItemJson controlItemJson in this.controls)
					{
						if (string.IsNullOrEmpty(controlItemJson.name))
						{
							throw new InvalidOperationException("Control with no name in layout '" + this.name);
						}
						InputControlLayout.ControlItem item = controlItemJson.ToLayout();
						list.Add(item);
					}
					inputControlLayout2.m_Controls = list.ToArray();
				}
				return inputControlLayout2;
			}

			public static InputControlLayout.LayoutJson FromLayout(InputControlLayout layout)
			{
				InputControlLayout.LayoutJson result = default(InputControlLayout.LayoutJson);
				result.name = layout.m_Name;
				Type type = layout.type;
				result.type = ((type != null) ? type.AssemblyQualifiedName : null);
				result.variant = layout.m_Variants;
				result.displayName = layout.m_DisplayName;
				result.description = layout.m_Description;
				result.isGenericTypeOfDevice = layout.isGenericTypeOfDevice;
				result.hideInUI = layout.hideInUI;
				result.extend = ((layout.m_BaseLayouts.length == 1) ? layout.m_BaseLayouts[0].ToString() : null);
				string[] array;
				if (layout.m_BaseLayouts.length <= 1)
				{
					array = null;
				}
				else
				{
					array = layout.m_BaseLayouts.ToArray<string>((InternedString x) => x.ToString());
				}
				result.extendMultiple = array;
				result.format = layout.stateFormat.ToString();
				result.commonUsages = ArrayHelpers.Select<InternedString, string>(layout.m_CommonUsages, (InternedString x) => x.ToString());
				result.controls = InputControlLayout.ControlItemJson.FromControlItems(layout.m_Controls);
				result.beforeRender = ((layout.m_UpdateBeforeRender != null) ? (layout.m_UpdateBeforeRender.Value ? "Update" : "Ignore") : null);
				return result;
			}

			public string name;

			public string extend;

			public string[] extendMultiple;

			public string format;

			public string beforeRender;

			public string runInBackground;

			public string[] commonUsages;

			public string displayName;

			public string description;

			public string type;

			public string variant;

			public bool isGenericTypeOfDevice;

			public bool hideInUI;

			public InputControlLayout.ControlItemJson[] controls;
		}

		[Serializable]
		private class ControlItemJson
		{
			public ControlItemJson()
			{
				this.offset = uint.MaxValue;
				this.bit = uint.MaxValue;
			}

			public InputControlLayout.ControlItem ToLayout()
			{
				InputControlLayout.ControlItem result = new InputControlLayout.ControlItem
				{
					name = new InternedString(this.name),
					layout = new InternedString(this.layout),
					variants = new InternedString(this.variants),
					displayName = this.displayName,
					shortDisplayName = this.shortDisplayName,
					offset = this.offset,
					useStateFrom = this.useStateFrom,
					bit = this.bit,
					sizeInBits = this.sizeInBits,
					isModifyingExistingControl = (this.name.IndexOf('/') != -1),
					isNoisy = this.noisy,
					dontReset = this.dontReset,
					isSynthetic = this.synthetic,
					isFirstDefinedInThisLayout = true,
					arraySize = this.arraySize
				};
				if (!string.IsNullOrEmpty(this.format))
				{
					result.format = new FourCC(this.format);
				}
				if (!string.IsNullOrEmpty(this.usage) || this.usages != null)
				{
					List<string> list = new List<string>();
					if (!string.IsNullOrEmpty(this.usage))
					{
						list.Add(this.usage);
					}
					if (this.usages != null)
					{
						list.AddRange(this.usages);
					}
					result.usages = new ReadOnlyArray<InternedString>((from x in list
					select new InternedString(x)).ToArray<InternedString>());
				}
				if (!string.IsNullOrEmpty(this.alias) || this.aliases != null)
				{
					List<string> list2 = new List<string>();
					if (!string.IsNullOrEmpty(this.alias))
					{
						list2.Add(this.alias);
					}
					if (this.aliases != null)
					{
						list2.AddRange(this.aliases);
					}
					result.aliases = new ReadOnlyArray<InternedString>((from x in list2
					select new InternedString(x)).ToArray<InternedString>());
				}
				if (!string.IsNullOrEmpty(this.parameters))
				{
					result.parameters = new ReadOnlyArray<NamedValue>(NamedValue.ParseMultiple(this.parameters));
				}
				if (!string.IsNullOrEmpty(this.processors))
				{
					result.processors = new ReadOnlyArray<NameAndParameters>(NameAndParameters.ParseMultiple(this.processors).ToArray<NameAndParameters>());
				}
				if (this.defaultState != null)
				{
					result.defaultState = PrimitiveValue.FromObject(this.defaultState);
				}
				if (this.minValue != null)
				{
					result.minValue = PrimitiveValue.FromObject(this.minValue);
				}
				if (this.maxValue != null)
				{
					result.maxValue = PrimitiveValue.FromObject(this.maxValue);
				}
				return result;
			}

			public static InputControlLayout.ControlItemJson[] FromControlItems(InputControlLayout.ControlItem[] items)
			{
				if (items == null)
				{
					return null;
				}
				int num = items.Length;
				InputControlLayout.ControlItemJson[] array = new InputControlLayout.ControlItemJson[num];
				for (int i = 0; i < num; i++)
				{
					InputControlLayout.ControlItem controlItem = items[i];
					InputControlLayout.ControlItemJson[] array2 = array;
					int num2 = i;
					InputControlLayout.ControlItemJson controlItemJson = new InputControlLayout.ControlItemJson();
					controlItemJson.name = controlItem.name;
					controlItemJson.layout = controlItem.layout;
					controlItemJson.variants = controlItem.variants;
					controlItemJson.displayName = controlItem.displayName;
					controlItemJson.shortDisplayName = controlItem.shortDisplayName;
					controlItemJson.bit = controlItem.bit;
					controlItemJson.offset = controlItem.offset;
					controlItemJson.sizeInBits = controlItem.sizeInBits;
					controlItemJson.format = controlItem.format.ToString();
					controlItemJson.parameters = string.Join(",", (from x in controlItem.parameters
					select x.ToString()).ToArray<string>());
					controlItemJson.processors = string.Join(",", (from x in controlItem.processors
					select x.ToString()).ToArray<string>());
					controlItemJson.usages = (from x in controlItem.usages
					select x.ToString()).ToArray<string>();
					controlItemJson.aliases = (from x in controlItem.aliases
					select x.ToString()).ToArray<string>();
					controlItemJson.noisy = controlItem.isNoisy;
					controlItemJson.dontReset = controlItem.dontReset;
					controlItemJson.synthetic = controlItem.isSynthetic;
					controlItemJson.arraySize = controlItem.arraySize;
					controlItemJson.defaultState = controlItem.defaultState.ToString();
					controlItemJson.minValue = controlItem.minValue.ToString();
					controlItemJson.maxValue = controlItem.maxValue.ToString();
					array2[num2] = controlItemJson;
				}
				return array;
			}

			public string name;

			public string layout;

			public string variants;

			public string usage;

			public string alias;

			public string useStateFrom;

			public uint offset;

			public uint bit;

			public uint sizeInBits;

			public string format;

			public int arraySize;

			public string[] usages;

			public string[] aliases;

			public string parameters;

			public string processors;

			public string displayName;

			public string shortDisplayName;

			public bool noisy;

			public bool dontReset;

			public bool synthetic;

			public string defaultState;

			public string minValue;

			public string maxValue;
		}

		internal struct Collection
		{
			public void Allocate()
			{
				this.layoutTypes = new Dictionary<InternedString, Type>();
				this.layoutStrings = new Dictionary<InternedString, string>();
				this.layoutBuilders = new Dictionary<InternedString, Func<InputControlLayout>>();
				this.baseLayoutTable = new Dictionary<InternedString, InternedString>();
				this.layoutOverrides = new Dictionary<InternedString, InternedString[]>();
				this.layoutOverrideNames = new HashSet<InternedString>();
				this.layoutMatchers = new List<InputControlLayout.Collection.LayoutMatcher>();
				this.precompiledLayouts = new Dictionary<InternedString, InputControlLayout.Collection.PrecompiledLayout>();
			}

			public InternedString TryFindLayoutForType(Type layoutType)
			{
				foreach (KeyValuePair<InternedString, Type> keyValuePair in this.layoutTypes)
				{
					if (keyValuePair.Value == layoutType)
					{
						return keyValuePair.Key;
					}
				}
				return default(InternedString);
			}

			public InternedString TryFindMatchingLayout(InputDeviceDescription deviceDescription)
			{
				float num = 0f;
				InternedString result = default(InternedString);
				int count = this.layoutMatchers.Count;
				for (int i = 0; i < count; i++)
				{
					InputDeviceMatcher deviceMatcher = this.layoutMatchers[i].deviceMatcher;
					float num2 = deviceMatcher.MatchPercentage(deviceDescription);
					if (num2 > 0f && !this.layoutBuilders.ContainsKey(this.layoutMatchers[i].layoutName))
					{
						num2 += 1f;
					}
					if (num2 > num)
					{
						num = num2;
						result = this.layoutMatchers[i].layoutName;
					}
				}
				return result;
			}

			public bool HasLayout(InternedString name)
			{
				return this.layoutTypes.ContainsKey(name) || this.layoutStrings.ContainsKey(name) || this.layoutBuilders.ContainsKey(name);
			}

			private InputControlLayout TryLoadLayoutInternal(InternedString name)
			{
				string json;
				if (this.layoutStrings.TryGetValue(name, out json))
				{
					return InputControlLayout.FromJson(json);
				}
				Type type;
				if (this.layoutTypes.TryGetValue(name, out type))
				{
					return InputControlLayout.FromType(name, type);
				}
				Func<InputControlLayout> func;
				if (!this.layoutBuilders.TryGetValue(name, out func))
				{
					return null;
				}
				InputControlLayout inputControlLayout = func();
				if (inputControlLayout == null)
				{
					throw new InvalidOperationException(string.Format("Layout builder '{0}' returned null when invoked", name));
				}
				return inputControlLayout;
			}

			public InputControlLayout TryLoadLayout(InternedString name, Dictionary<InternedString, InputControlLayout> table = null)
			{
				InputControlLayout inputControlLayout;
				if (table != null && table.TryGetValue(name, out inputControlLayout))
				{
					return inputControlLayout;
				}
				inputControlLayout = this.TryLoadLayoutInternal(name);
				if (inputControlLayout != null)
				{
					inputControlLayout.m_Name = name;
					if (this.layoutOverrideNames.Contains(name))
					{
						inputControlLayout.isOverride = true;
					}
					InternedString internedString = default(InternedString);
					if (!inputControlLayout.isOverride && this.baseLayoutTable.TryGetValue(name, out internedString))
					{
						InputControlLayout inputControlLayout2 = this.TryLoadLayout(internedString, table);
						if (inputControlLayout2 == null)
						{
							throw new InputControlLayout.LayoutNotFoundException(string.Format("Cannot find base layout '{0}' of layout '{1}'", internedString, name));
						}
						inputControlLayout.MergeLayout(inputControlLayout2);
						if (inputControlLayout.m_BaseLayouts.length == 0)
						{
							inputControlLayout.m_BaseLayouts.Append(internedString);
						}
					}
					InternedString[] array;
					if (this.layoutOverrides.TryGetValue(name, out array))
					{
						foreach (InternedString internedString2 in array)
						{
							InputControlLayout inputControlLayout3 = this.TryLoadLayout(internedString2, null);
							inputControlLayout3.MergeLayout(inputControlLayout);
							inputControlLayout3.m_BaseLayouts.Clear();
							inputControlLayout3.isOverride = false;
							inputControlLayout3.isGenericTypeOfDevice = inputControlLayout.isGenericTypeOfDevice;
							inputControlLayout3.m_Name = inputControlLayout.name;
							inputControlLayout3.m_BaseLayouts = inputControlLayout.m_BaseLayouts;
							inputControlLayout = inputControlLayout3;
							inputControlLayout.m_AppliedOverrides.Append(internedString2);
						}
					}
					if (table != null)
					{
						table[name] = inputControlLayout;
					}
				}
				return inputControlLayout;
			}

			public InternedString GetBaseLayoutName(InternedString layoutName)
			{
				InternedString result;
				if (this.baseLayoutTable.TryGetValue(layoutName, out result))
				{
					return result;
				}
				return default(InternedString);
			}

			public InternedString GetRootLayoutName(InternedString layoutName)
			{
				InternedString internedString;
				while (this.baseLayoutTable.TryGetValue(layoutName, out internedString))
				{
					layoutName = internedString;
				}
				return layoutName;
			}

			public bool ComputeDistanceInInheritanceHierarchy(InternedString firstLayout, InternedString secondLayout, out int distance)
			{
				distance = 0;
				int num = 0;
				InternedString internedString = secondLayout;
				while (!internedString.IsEmpty() && internedString != firstLayout)
				{
					internedString = this.GetBaseLayoutName(internedString);
					num++;
				}
				if (internedString == firstLayout)
				{
					distance = num;
					return true;
				}
				int num2 = 0;
				internedString = firstLayout;
				while (!internedString.IsEmpty() && internedString != secondLayout)
				{
					internedString = this.GetBaseLayoutName(internedString);
					num2++;
				}
				if (internedString == secondLayout)
				{
					distance = num2;
					return true;
				}
				return false;
			}

			public InternedString FindLayoutThatIntroducesControl(InputControl control, InputControlLayout.Cache cache)
			{
				InputControl inputControl = control;
				while (inputControl.parent != control.device)
				{
					inputControl = inputControl.parent;
				}
				InternedString internedString = control.device.m_Layout;
				InternedString internedString2 = internedString;
				while (this.baseLayoutTable.TryGetValue(internedString2, out internedString2))
				{
					if (cache.FindOrLoadLayout(internedString2, true).FindControl(inputControl.m_Name) != null)
					{
						internedString = internedString2;
					}
				}
				return internedString;
			}

			public Type GetControlTypeForLayout(InternedString layoutName)
			{
				while (this.layoutStrings.ContainsKey(layoutName))
				{
					InternedString internedString;
					if (!this.baseLayoutTable.TryGetValue(layoutName, out internedString))
					{
						return typeof(InputDevice);
					}
					layoutName = internedString;
				}
				Type result;
				this.layoutTypes.TryGetValue(layoutName, out result);
				return result;
			}

			public bool ValueTypeIsAssignableFrom(InternedString layoutName, Type valueType)
			{
				Type controlTypeForLayout = this.GetControlTypeForLayout(layoutName);
				if (controlTypeForLayout == null)
				{
					return false;
				}
				Type genericTypeArgumentFromHierarchy = TypeHelpers.GetGenericTypeArgumentFromHierarchy(controlTypeForLayout, typeof(InputControl<>), 0);
				return !(genericTypeArgumentFromHierarchy == null) && valueType.IsAssignableFrom(genericTypeArgumentFromHierarchy);
			}

			public bool IsGeneratedLayout(InternedString layout)
			{
				return this.layoutBuilders.ContainsKey(layout);
			}

			public IEnumerable<InternedString> GetBaseLayouts(InternedString layout, bool includeSelf = true)
			{
				if (includeSelf)
				{
					yield return layout;
				}
				while (this.baseLayoutTable.TryGetValue(layout, out layout))
				{
					yield return layout;
				}
				yield break;
			}

			public bool IsBasedOn(InternedString parentLayout, InternedString childLayout)
			{
				InternedString internedString = childLayout;
				while (this.baseLayoutTable.TryGetValue(internedString, out internedString))
				{
					if (internedString == parentLayout)
					{
						return true;
					}
				}
				return false;
			}

			public void AddMatcher(InternedString layout, InputDeviceMatcher matcher)
			{
				int count = this.layoutMatchers.Count;
				for (int i = 0; i < count; i++)
				{
					if (this.layoutMatchers[i].deviceMatcher == matcher)
					{
						return;
					}
				}
				this.layoutMatchers.Add(new InputControlLayout.Collection.LayoutMatcher
				{
					layoutName = layout,
					deviceMatcher = matcher
				});
			}

			public const float kBaseScoreForNonGeneratedLayouts = 1f;

			public Dictionary<InternedString, Type> layoutTypes;

			public Dictionary<InternedString, string> layoutStrings;

			public Dictionary<InternedString, Func<InputControlLayout>> layoutBuilders;

			public Dictionary<InternedString, InternedString> baseLayoutTable;

			public Dictionary<InternedString, InternedString[]> layoutOverrides;

			public HashSet<InternedString> layoutOverrideNames;

			public Dictionary<InternedString, InputControlLayout.Collection.PrecompiledLayout> precompiledLayouts;

			public List<InputControlLayout.Collection.LayoutMatcher> layoutMatchers;

			public struct LayoutMatcher
			{
				public InternedString layoutName;

				public InputDeviceMatcher deviceMatcher;
			}

			public struct PrecompiledLayout
			{
				public Func<InputDevice> factoryMethod;

				public string metadata;
			}
		}

		public class LayoutNotFoundException : Exception
		{
			public string layout { get; }

			public LayoutNotFoundException()
			{
			}

			public LayoutNotFoundException(string name, string message) : base(message)
			{
				this.layout = name;
			}

			public LayoutNotFoundException(string name) : base("Cannot find control layout '" + name + "'")
			{
				this.layout = name;
			}

			public LayoutNotFoundException(string message, Exception innerException) : base(message, innerException)
			{
			}

			protected LayoutNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}

		internal struct Cache
		{
			public void Clear()
			{
				this.table = null;
			}

			public InputControlLayout FindOrLoadLayout(string name, bool throwIfNotFound = true)
			{
				InternedString name2 = new InternedString(name);
				if (this.table == null)
				{
					this.table = new Dictionary<InternedString, InputControlLayout>();
				}
				InputControlLayout inputControlLayout = InputControlLayout.s_Layouts.TryLoadLayout(name2, this.table);
				if (inputControlLayout != null)
				{
					return inputControlLayout;
				}
				if (throwIfNotFound)
				{
					throw new InputControlLayout.LayoutNotFoundException(name);
				}
				return null;
			}

			public Dictionary<InternedString, InputControlLayout> table;
		}

		internal struct CacheRefInstance : IDisposable
		{
			public void Dispose()
			{
				if (!this.valid)
				{
					return;
				}
				InputControlLayout.s_CacheInstanceRef--;
				if (InputControlLayout.s_CacheInstanceRef <= 0)
				{
					InputControlLayout.s_CacheInstance = default(InputControlLayout.Cache);
					InputControlLayout.s_CacheInstanceRef = 0;
				}
				this.valid = false;
			}

			public bool valid;
		}
	}
}
