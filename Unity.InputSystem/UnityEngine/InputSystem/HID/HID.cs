using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Pool;

namespace UnityEngine.InputSystem.HID
{
	public class HID : InputDevice
	{
		public static FourCC QueryHIDReportDescriptorDeviceCommandType
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'D');
			}
		}

		public static FourCC QueryHIDReportDescriptorSizeDeviceCommandType
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'S');
			}
		}

		public static FourCC QueryHIDParsedReportDescriptorDeviceCommandType
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'P');
			}
		}

		public HID.HIDDeviceDescriptor hidDescriptor
		{
			get
			{
				if (!this.m_HaveParsedHIDDescriptor)
				{
					if (!string.IsNullOrEmpty(base.description.capabilities))
					{
						this.m_HIDDescriptor = JsonUtility.FromJson<HID.HIDDeviceDescriptor>(base.description.capabilities);
					}
					this.m_HaveParsedHIDDescriptor = true;
				}
				return this.m_HIDDescriptor;
			}
		}

		internal static string OnFindLayoutForDevice(ref InputDeviceDescription description, string matchedLayout, InputDeviceExecuteCommandDelegate executeDeviceCommand)
		{
			if (!string.IsNullOrEmpty(matchedLayout))
			{
				return null;
			}
			if (description.interfaceName != "HID")
			{
				return null;
			}
			HID.HIDDeviceDescriptor hiddeviceDescriptor = HID.ReadHIDDeviceDescriptor(ref description, executeDeviceCommand);
			if (!HIDSupport.supportedHIDUsages.Contains(new HIDSupport.HIDPageUsage(hiddeviceDescriptor.usagePage, hiddeviceDescriptor.usage)))
			{
				return null;
			}
			bool flag = false;
			if (hiddeviceDescriptor.elements != null)
			{
				foreach (HID.HIDElementDescriptor hidelementDescriptor in hiddeviceDescriptor.elements)
				{
					if (hidelementDescriptor.IsUsableElement())
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return null;
			}
			Type typeFromHandle = typeof(HID);
			string text = "HID";
			if (hiddeviceDescriptor.usagePage == HID.UsagePage.GenericDesktop && (hiddeviceDescriptor.usage == 4 || hiddeviceDescriptor.usage == 5))
			{
				text = "Joystick";
				typeFromHandle = typeof(Joystick);
			}
			string text2 = "";
			if (text != "Joystick")
			{
				text2 = ((hiddeviceDescriptor.usagePage == HID.UsagePage.GenericDesktop) ? string.Format(" {0}", (HID.GenericDesktop)hiddeviceDescriptor.usage) : string.Format(" {0}-{1}", hiddeviceDescriptor.usagePage, hiddeviceDescriptor.usage));
			}
			InputDeviceMatcher value = InputDeviceMatcher.FromDeviceDescription(description);
			string text3;
			if (!string.IsNullOrEmpty(description.product) && !string.IsNullOrEmpty(description.manufacturer))
			{
				text3 = string.Concat(new string[]
				{
					"HID::",
					description.manufacturer,
					" ",
					description.product,
					text2
				});
			}
			else if (!string.IsNullOrEmpty(description.product))
			{
				text3 = "HID::" + description.product + text2;
			}
			else
			{
				if (hiddeviceDescriptor.vendorId == 0)
				{
					return null;
				}
				text3 = string.Format("{0}::{1:X}-{2:X}{3}", new object[]
				{
					"HID",
					hiddeviceDescriptor.vendorId,
					hiddeviceDescriptor.productId,
					text2
				});
				value = value.WithCapability<int>("productId", hiddeviceDescriptor.productId).WithCapability<int>("vendorId", hiddeviceDescriptor.vendorId);
			}
			value = value.WithCapability<int>("usage", hiddeviceDescriptor.usage).WithCapability<HID.UsagePage>("usagePage", hiddeviceDescriptor.usagePage);
			HID.HIDLayoutBuilder layout = new HID.HIDLayoutBuilder
			{
				displayName = description.product,
				hidDescriptor = hiddeviceDescriptor,
				parentLayout = text,
				deviceType = (typeFromHandle ?? typeof(HID))
			};
			InputSystem.RegisterLayoutBuilder(() => layout.Build(), text3, text, new InputDeviceMatcher?(value));
			return text3;
		}

		internal unsafe static HID.HIDDeviceDescriptor ReadHIDDeviceDescriptor(ref InputDeviceDescription deviceDescription, InputDeviceExecuteCommandDelegate executeCommandDelegate)
		{
			if (deviceDescription.interfaceName != "HID")
			{
				throw new ArgumentException(string.Format("Device '{0}' is not a HID", deviceDescription));
			}
			bool flag = true;
			HID.HIDDeviceDescriptor hiddeviceDescriptor = default(HID.HIDDeviceDescriptor);
			if (!string.IsNullOrEmpty(deviceDescription.capabilities))
			{
				try
				{
					hiddeviceDescriptor = HID.HIDDeviceDescriptor.FromJson(deviceDescription.capabilities);
					if (hiddeviceDescriptor.elements != null && hiddeviceDescriptor.elements.Length != 0)
					{
						flag = false;
					}
				}
				catch (Exception exception)
				{
					Debug.LogError(string.Format("Could not parse HID descriptor of device '{0}'", deviceDescription));
					Debug.LogException(exception);
				}
			}
			if (flag)
			{
				InputDeviceCommand inputDeviceCommand = new InputDeviceCommand(HID.QueryHIDReportDescriptorSizeDeviceCommandType, 8);
				long num = executeCommandDelegate(ref inputDeviceCommand);
				if (num > 0L)
				{
					using (NativeArray<byte> nativeArray = InputDeviceCommand.AllocateNative(HID.QueryHIDReportDescriptorDeviceCommandType, (int)num))
					{
						InputDeviceCommand* unsafePtr = (InputDeviceCommand*)nativeArray.GetUnsafePtr<byte>();
						if (executeCommandDelegate(ref *unsafePtr) != num)
						{
							HID.HIDDeviceDescriptor result = default(HID.HIDDeviceDescriptor);
							return result;
						}
						if (!HIDParser.ParseReportDescriptor((byte*)unsafePtr->payloadPtr, (int)num, ref hiddeviceDescriptor))
						{
							return default(HID.HIDDeviceDescriptor);
						}
					}
					deviceDescription.capabilities = hiddeviceDescriptor.ToJson();
					return hiddeviceDescriptor;
				}
				using (NativeArray<byte> nativeArray2 = InputDeviceCommand.AllocateNative(HID.QueryHIDParsedReportDescriptorDeviceCommandType, 2097152))
				{
					InputDeviceCommand* unsafePtr2 = (InputDeviceCommand*)nativeArray2.GetUnsafePtr<byte>();
					long num2 = executeCommandDelegate(ref *unsafePtr2);
					if (num2 < 0L)
					{
						return default(HID.HIDDeviceDescriptor);
					}
					byte[] array = new byte[num2];
					try
					{
						byte[] array2;
						byte* destination;
						if ((array2 = array) == null || array2.Length == 0)
						{
							destination = null;
						}
						else
						{
							destination = &array2[0];
						}
						UnsafeUtility.MemCpy((void*)destination, unsafePtr2->payloadPtr, num2);
					}
					finally
					{
						byte[] array2 = null;
					}
					string @string = Encoding.UTF8.GetString(array, 0, (int)num2);
					try
					{
						hiddeviceDescriptor = HID.HIDDeviceDescriptor.FromJson(@string);
					}
					catch (Exception exception2)
					{
						Debug.LogError(string.Format("Could not parse HID descriptor of device '{0}'", deviceDescription));
						Debug.LogException(exception2);
						return default(HID.HIDDeviceDescriptor);
					}
					deviceDescription.capabilities = @string;
				}
				return hiddeviceDescriptor;
			}
			return hiddeviceDescriptor;
		}

		public static string UsagePageToString(HID.UsagePage usagePage)
		{
			if (usagePage < HID.UsagePage.VendorDefined)
			{
				return usagePage.ToString();
			}
			return "Vendor-Defined";
		}

		public static string UsageToString(HID.UsagePage usagePage, int usage)
		{
			if (usagePage == HID.UsagePage.GenericDesktop)
			{
				HID.GenericDesktop genericDesktop = (HID.GenericDesktop)usage;
				return genericDesktop.ToString();
			}
			if (usagePage != HID.UsagePage.Simulation)
			{
				return null;
			}
			HID.Simulation simulation = (HID.Simulation)usage;
			return simulation.ToString();
		}

		internal const string kHIDInterface = "HID";

		internal const string kHIDNamespace = "HID";

		private bool m_HaveParsedHIDDescriptor;

		private HID.HIDDeviceDescriptor m_HIDDescriptor;

		private static readonly ProfilerMarker k_HIDParseDescriptorFallback = new ProfilerMarker("HIDParseDescriptorFallback");

		[Serializable]
		private class HIDLayoutBuilder
		{
			public InputControlLayout Build()
			{
				InputControlLayout.Builder builder = new InputControlLayout.Builder
				{
					displayName = this.displayName,
					type = this.deviceType,
					extendsLayout = this.parentLayout,
					stateFormat = new FourCC('H', 'I', 'D', ' ')
				};
				HID.HIDElementDescriptor hidelementDescriptor = Array.Find<HID.HIDElementDescriptor>(this.hidDescriptor.elements, (HID.HIDElementDescriptor element) => element.usagePage == HID.UsagePage.GenericDesktop && element.usage == 48);
				HID.HIDElementDescriptor hidelementDescriptor2 = Array.Find<HID.HIDElementDescriptor>(this.hidDescriptor.elements, (HID.HIDElementDescriptor element) => element.usagePage == HID.UsagePage.GenericDesktop && element.usage == 49);
				bool flag = hidelementDescriptor.usage == 48 && hidelementDescriptor2.usage == 49;
				if (flag)
				{
					int bit;
					int num;
					int sizeInBits;
					if (hidelementDescriptor.reportOffsetInBits <= hidelementDescriptor2.reportOffsetInBits)
					{
						bit = hidelementDescriptor.reportOffsetInBits % 8;
						num = hidelementDescriptor.reportOffsetInBits / 8;
						sizeInBits = hidelementDescriptor2.reportOffsetInBits + hidelementDescriptor2.reportSizeInBits - hidelementDescriptor.reportOffsetInBits;
					}
					else
					{
						bit = hidelementDescriptor2.reportOffsetInBits % 8;
						num = hidelementDescriptor2.reportOffsetInBits / 8;
						sizeInBits = hidelementDescriptor.reportOffsetInBits + hidelementDescriptor.reportSizeInBits - hidelementDescriptor2.reportSizeInBits;
					}
					InputControlLayout.Builder.ControlBuilder controlBuilder = builder.AddControl("stick");
					controlBuilder = controlBuilder.WithDisplayName("Stick");
					controlBuilder = controlBuilder.WithLayout("Stick");
					controlBuilder = controlBuilder.WithBitOffset((uint)bit);
					controlBuilder = controlBuilder.WithByteOffset((uint)num);
					controlBuilder = controlBuilder.WithSizeInBits((uint)sizeInBits);
					controlBuilder.WithUsages(new InternedString[]
					{
						CommonUsages.Primary2DMotion
					});
					string text = hidelementDescriptor.DetermineParameters();
					string text2 = hidelementDescriptor2.DetermineParameters();
					controlBuilder = builder.AddControl("stick/x");
					controlBuilder = controlBuilder.WithFormat(hidelementDescriptor.isSigned ? InputStateBlock.FormatSBit : InputStateBlock.FormatBit);
					controlBuilder = controlBuilder.WithByteOffset((uint)(hidelementDescriptor.reportOffsetInBits / 8 - num));
					controlBuilder = controlBuilder.WithBitOffset((uint)(hidelementDescriptor.reportOffsetInBits % 8));
					controlBuilder = controlBuilder.WithSizeInBits((uint)hidelementDescriptor.reportSizeInBits);
					controlBuilder = controlBuilder.WithParameters(text);
					controlBuilder = controlBuilder.WithDefaultState(hidelementDescriptor.DetermineDefaultState());
					controlBuilder.WithProcessors(hidelementDescriptor.DetermineProcessors());
					controlBuilder = builder.AddControl("stick/y");
					controlBuilder = controlBuilder.WithFormat(hidelementDescriptor2.isSigned ? InputStateBlock.FormatSBit : InputStateBlock.FormatBit);
					controlBuilder = controlBuilder.WithByteOffset((uint)(hidelementDescriptor2.reportOffsetInBits / 8 - num));
					controlBuilder = controlBuilder.WithBitOffset((uint)(hidelementDescriptor2.reportOffsetInBits % 8));
					controlBuilder = controlBuilder.WithSizeInBits((uint)hidelementDescriptor2.reportSizeInBits);
					controlBuilder = controlBuilder.WithParameters(text2);
					controlBuilder = controlBuilder.WithDefaultState(hidelementDescriptor2.DetermineDefaultState());
					controlBuilder.WithProcessors(hidelementDescriptor2.DetermineProcessors());
					controlBuilder = builder.AddControl("stick/up");
					controlBuilder.WithParameters(StringHelpers.Join<string>(",", new string[]
					{
						text2,
						"clamp=2,clampMin=-1,clampMax=0,invert=true"
					}));
					controlBuilder = builder.AddControl("stick/down");
					controlBuilder.WithParameters(StringHelpers.Join<string>(",", new string[]
					{
						text2,
						"clamp=2,clampMin=0,clampMax=1,invert=false"
					}));
					controlBuilder = builder.AddControl("stick/left");
					controlBuilder.WithParameters(StringHelpers.Join<string>(",", new string[]
					{
						text,
						"clamp=2,clampMin=-1,clampMax=0,invert"
					}));
					controlBuilder = builder.AddControl("stick/right");
					controlBuilder.WithParameters(StringHelpers.Join<string>(",", new string[]
					{
						text,
						"clamp=2,clampMin=0,clampMax=1"
					}));
				}
				HID.HIDElementDescriptor[] elements = this.hidDescriptor.elements;
				int num2 = elements.Length;
				for (int i = 0; i < num2; i++)
				{
					ref HID.HIDElementDescriptor ptr = ref elements[i];
					if (ptr.reportType == HID.HIDReportType.Input && (!flag || (!ptr.Is(HID.UsagePage.GenericDesktop, 48) && !ptr.Is(HID.UsagePage.GenericDesktop, 49))))
					{
						string text3 = ptr.DetermineLayout();
						if (text3 != null)
						{
							string text4 = ptr.DetermineName();
							text4 = StringHelpers.MakeUniqueName<InputControlLayout.ControlItem>(text4, builder.controls, (InputControlLayout.ControlItem x) => x.name);
							InputControlLayout.Builder.ControlBuilder controlBuilder = builder.AddControl(text4);
							controlBuilder = controlBuilder.WithDisplayName(ptr.DetermineDisplayName());
							controlBuilder = controlBuilder.WithLayout(text3);
							controlBuilder = controlBuilder.WithByteOffset((uint)(ptr.reportOffsetInBits / 8));
							controlBuilder = controlBuilder.WithBitOffset((uint)(ptr.reportOffsetInBits % 8));
							controlBuilder = controlBuilder.WithSizeInBits((uint)ptr.reportSizeInBits);
							controlBuilder = controlBuilder.WithFormat(ptr.DetermineFormat());
							controlBuilder = controlBuilder.WithDefaultState(ptr.DetermineDefaultState());
							InputControlLayout.Builder.ControlBuilder controlBuilder2 = controlBuilder.WithProcessors(ptr.DetermineProcessors());
							string text5 = ptr.DetermineParameters();
							if (!string.IsNullOrEmpty(text5))
							{
								controlBuilder2.WithParameters(text5);
							}
							InternedString[] array = ptr.DetermineUsages();
							if (array != null)
							{
								controlBuilder2.WithUsages(array);
							}
							ptr.AddChildControls(ref ptr, text4, ref builder);
						}
					}
				}
				return builder.Build();
			}

			public string displayName;

			public HID.HIDDeviceDescriptor hidDescriptor;

			public string parentLayout;

			public Type deviceType;
		}

		public enum HIDReportType
		{
			Unknown,
			Input,
			Output,
			Feature
		}

		public enum HIDCollectionType
		{
			Physical,
			Application,
			Logical,
			Report,
			NamedArray,
			UsageSwitch,
			UsageModifier
		}

		[Flags]
		public enum HIDElementFlags
		{
			Constant = 1,
			Variable = 2,
			Relative = 4,
			Wrap = 8,
			NonLinear = 16,
			NoPreferred = 32,
			NullState = 64,
			Volatile = 128,
			BufferedBytes = 256
		}

		[Serializable]
		public struct HIDElementDescriptor
		{
			public bool hasNullState
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.NullState) == HID.HIDElementFlags.NullState;
				}
			}

			public bool hasPreferredState
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.NoPreferred) != HID.HIDElementFlags.NoPreferred;
				}
			}

			public bool isArray
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.Variable) != HID.HIDElementFlags.Variable;
				}
			}

			public bool isNonLinear
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.NonLinear) == HID.HIDElementFlags.NonLinear;
				}
			}

			public bool isRelative
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.Relative) == HID.HIDElementFlags.Relative;
				}
			}

			public bool isConstant
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.Constant) == HID.HIDElementFlags.Constant;
				}
			}

			public bool isWrapping
			{
				get
				{
					return (this.flags & HID.HIDElementFlags.Wrap) == HID.HIDElementFlags.Wrap;
				}
			}

			internal bool isSigned
			{
				get
				{
					return this.logicalMin < 0;
				}
			}

			internal float minFloatValue
			{
				get
				{
					if (this.isSigned)
					{
						int minValue = (int)(-(int)((int)1L << this.reportSizeInBits - 1));
						int maxValue = (int)((1L << this.reportSizeInBits - 1) - 1L);
						return NumberHelpers.IntToNormalizedFloat(this.logicalMin, minValue, maxValue) * 2f - 1f;
					}
					uint maxValue2 = (uint)((1L << this.reportSizeInBits) - 1L);
					return NumberHelpers.UIntToNormalizedFloat((uint)this.logicalMin, 0U, maxValue2);
				}
			}

			internal float maxFloatValue
			{
				get
				{
					if (this.isSigned)
					{
						int minValue = (int)(-(int)((int)1L << this.reportSizeInBits - 1));
						int maxValue = (int)((1L << this.reportSizeInBits - 1) - 1L);
						return NumberHelpers.IntToNormalizedFloat(this.logicalMax, minValue, maxValue) * 2f - 1f;
					}
					uint maxValue2 = (uint)((1L << this.reportSizeInBits) - 1L);
					return NumberHelpers.UIntToNormalizedFloat((uint)this.logicalMax, 0U, maxValue2);
				}
			}

			public bool Is(HID.UsagePage usagePage, int usage)
			{
				return usagePage == this.usagePage && usage == this.usage;
			}

			internal string DetermineName()
			{
				HID.UsagePage usagePage = this.usagePage;
				if (usagePage != HID.UsagePage.GenericDesktop)
				{
					if (usagePage != HID.UsagePage.Button)
					{
						return string.Format("UsagePage({0:X}) Usage({1:X})", this.usagePage, this.usage);
					}
					if (this.usage == 1)
					{
						return "trigger";
					}
					return string.Format("button{0}", this.usage);
				}
				else
				{
					if (this.usage == 57)
					{
						return "hat";
					}
					HID.GenericDesktop genericDesktop = (HID.GenericDesktop)this.usage;
					string text = genericDesktop.ToString();
					return char.ToLowerInvariant(text[0]).ToString() + text.Substring(1);
				}
			}

			internal string DetermineDisplayName()
			{
				HID.UsagePage usagePage = this.usagePage;
				if (usagePage == HID.UsagePage.GenericDesktop)
				{
					HID.GenericDesktop genericDesktop = (HID.GenericDesktop)this.usage;
					return genericDesktop.ToString();
				}
				if (usagePage != HID.UsagePage.Button)
				{
					return null;
				}
				if (this.usage == 1)
				{
					return "Trigger";
				}
				return string.Format("Button {0}", this.usage);
			}

			internal bool IsUsableElement()
			{
				int num = this.usage;
				if (num - 48 <= 1)
				{
					return this.usagePage == HID.UsagePage.GenericDesktop;
				}
				return this.DetermineLayout() != null;
			}

			internal string DetermineLayout()
			{
				if (this.reportType != HID.HIDReportType.Input)
				{
					return null;
				}
				HID.UsagePage usagePage = this.usagePage;
				if (usagePage == HID.UsagePage.GenericDesktop)
				{
					int num = this.usage;
					switch (num)
					{
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 64:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
						return "Axis";
					case 57:
						if (this.logicalMax - this.logicalMin + 1 == 8)
						{
							return "Dpad";
						}
						goto IL_BC;
					case 58:
					case 59:
					case 60:
					case 63:
						goto IL_BC;
					case 61:
					case 62:
						break;
					default:
						if (num - 144 > 3)
						{
							goto IL_BC;
						}
						break;
					}
					return "Button";
				}
				if (usagePage == HID.UsagePage.Button)
				{
					return "Button";
				}
				IL_BC:
				return null;
			}

			internal FourCC DetermineFormat()
			{
				int num = this.reportSizeInBits;
				if (num != 8)
				{
					if (num != 16)
					{
						if (num != 32)
						{
							return InputStateBlock.FormatBit;
						}
						if (!this.isSigned)
						{
							return InputStateBlock.FormatUInt;
						}
						return InputStateBlock.FormatInt;
					}
					else
					{
						if (!this.isSigned)
						{
							return InputStateBlock.FormatUShort;
						}
						return InputStateBlock.FormatShort;
					}
				}
				else
				{
					if (!this.isSigned)
					{
						return InputStateBlock.FormatByte;
					}
					return InputStateBlock.FormatSByte;
				}
			}

			internal InternedString[] DetermineUsages()
			{
				if (this.usagePage == HID.UsagePage.Button && this.usage == 1)
				{
					return new InternedString[]
					{
						CommonUsages.PrimaryTrigger,
						CommonUsages.PrimaryAction
					};
				}
				if (this.usagePage == HID.UsagePage.Button && this.usage == 2)
				{
					return new InternedString[]
					{
						CommonUsages.SecondaryTrigger,
						CommonUsages.SecondaryAction
					};
				}
				if (this.usagePage == HID.UsagePage.GenericDesktop && this.usage == 53)
				{
					return new InternedString[]
					{
						CommonUsages.Twist
					};
				}
				return null;
			}

			internal string DetermineParameters()
			{
				if (this.usagePage == HID.UsagePage.GenericDesktop)
				{
					switch (this.usage)
					{
					case 48:
					case 50:
					case 51:
					case 53:
					case 54:
					case 55:
					case 56:
					case 64:
					case 66:
					case 67:
					case 69:
						return this.DetermineAxisNormalizationParameters();
					case 49:
					case 52:
					case 65:
					case 68:
						return StringHelpers.Join<string>(",", new string[]
						{
							"invert",
							this.DetermineAxisNormalizationParameters()
						});
					}
				}
				return null;
			}

			private string DetermineAxisNormalizationParameters()
			{
				if (this.logicalMin == 0 && this.logicalMax == 0)
				{
					return "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5";
				}
				float minFloatValue = this.minFloatValue;
				float maxFloatValue = this.maxFloatValue;
				if (Mathf.Approximately(0f, minFloatValue) && Mathf.Approximately(0f, maxFloatValue))
				{
					return null;
				}
				float num = minFloatValue + (maxFloatValue - minFloatValue) / 2f;
				return string.Format(CultureInfo.InvariantCulture, "normalize,normalizeMin={0},normalizeMax={1},normalizeZero={2}", minFloatValue, maxFloatValue, num);
			}

			internal string DetermineProcessors()
			{
				if (this.usagePage == HID.UsagePage.GenericDesktop)
				{
					int num = this.usage;
					if (num - 48 <= 8 || num - 64 <= 5)
					{
						return "axisDeadzone";
					}
				}
				return null;
			}

			internal PrimitiveValue DetermineDefaultState()
			{
				if (this.usagePage == HID.UsagePage.GenericDesktop)
				{
					switch (this.usage)
					{
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 64:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
						if (!this.isSigned)
						{
							int num = this.logicalMin + (this.logicalMax - this.logicalMin) / 2;
							if (num != 0)
							{
								return new PrimitiveValue(num);
							}
						}
						break;
					case 57:
						if (this.hasNullState)
						{
							if (this.logicalMin >= 1)
							{
								return new PrimitiveValue(this.logicalMin - 1);
							}
							ulong num2 = (1UL << this.reportSizeInBits) - 1UL;
							if ((long)this.logicalMax < (long)num2)
							{
								return new PrimitiveValue(this.logicalMax + 1);
							}
						}
						break;
					}
				}
				return default(PrimitiveValue);
			}

			internal void AddChildControls(ref HID.HIDElementDescriptor element, string controlName, ref InputControlLayout.Builder builder)
			{
				if (this.usagePage == HID.UsagePage.GenericDesktop && this.usage == 57)
				{
					PrimitiveValue primitiveValue = this.DetermineDefaultState();
					if (primitiveValue.isEmpty)
					{
						return;
					}
					builder.AddControl(controlName + "/up").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton").WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1},nullValue={2},wrapAtValue={3}", new object[]
					{
						this.logicalMax,
						this.logicalMin + 1,
						primitiveValue.ToString(),
						this.logicalMax
					})).WithBitOffset((uint)(element.reportOffsetInBits % 8)).WithSizeInBits((uint)this.reportSizeInBits);
					builder.AddControl(controlName + "/right").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton").WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1}", this.logicalMin + 1, this.logicalMin + 3)).WithBitOffset((uint)(element.reportOffsetInBits % 8)).WithSizeInBits((uint)this.reportSizeInBits);
					builder.AddControl(controlName + "/down").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton").WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1}", this.logicalMin + 3, this.logicalMin + 5)).WithBitOffset((uint)(element.reportOffsetInBits % 8)).WithSizeInBits((uint)this.reportSizeInBits);
					builder.AddControl(controlName + "/left").WithFormat(InputStateBlock.FormatBit).WithLayout("DiscreteButton").WithParameters(string.Format(CultureInfo.InvariantCulture, "minValue={0},maxValue={1}", this.logicalMin + 5, this.logicalMin + 7)).WithBitOffset((uint)(element.reportOffsetInBits % 8)).WithSizeInBits((uint)this.reportSizeInBits);
				}
			}

			public int usage;

			public HID.UsagePage usagePage;

			public int unit;

			public int unitExponent;

			public int logicalMin;

			public int logicalMax;

			public int physicalMin;

			public int physicalMax;

			public HID.HIDReportType reportType;

			public int collectionIndex;

			public int reportId;

			public int reportSizeInBits;

			public int reportOffsetInBits;

			public HID.HIDElementFlags flags;

			public int? usageMin;

			public int? usageMax;
		}

		[Serializable]
		public struct HIDCollectionDescriptor
		{
			public HID.HIDCollectionType type;

			public int usage;

			public HID.UsagePage usagePage;

			public int parent;

			public int childCount;

			public int firstChild;
		}

		[Serializable]
		public struct HIDDeviceDescriptor
		{
			public string ToJson()
			{
				return JsonUtility.ToJson(this, true);
			}

			public static HID.HIDDeviceDescriptor FromJson(string json)
			{
				HID.HIDDeviceDescriptor result;
				try
				{
					HID.HIDDeviceDescriptor hiddeviceDescriptor = default(HID.HIDDeviceDescriptor);
					ReadOnlySpan<char> readOnlySpan = json.AsSpan();
					PredictiveParser predictiveParser = default(PredictiveParser);
					predictiveParser.ExpectSingleChar(readOnlySpan, '{');
					ReadOnlySpan<char> readOnlySpan2;
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.vendorId = predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.productId = predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.usage = predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.usagePage = (HID.UsagePage)predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.inputReportSize = predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.outputReportSize = predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan2);
					predictiveParser.ExpectSingleChar(readOnlySpan, ':');
					hiddeviceDescriptor.featureReportSize = predictiveParser.ExpectInt(readOnlySpan);
					predictiveParser.AcceptSingleChar(readOnlySpan, ',');
					ReadOnlySpan<char> readOnlySpan3;
					predictiveParser.AcceptString(readOnlySpan, out readOnlySpan3);
					if (readOnlySpan3.ToString() != "elements")
					{
						result = hiddeviceDescriptor;
					}
					else
					{
						predictiveParser.ExpectSingleChar(readOnlySpan, ':');
						predictiveParser.ExpectSingleChar(readOnlySpan, '[');
						List<HID.HIDElementDescriptor> list;
						using (CollectionPool<List<HID.HIDElementDescriptor>, HID.HIDElementDescriptor>.Get(out list))
						{
							while (!predictiveParser.AcceptSingleChar(readOnlySpan, ']'))
							{
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectSingleChar(readOnlySpan, '{');
								HID.HIDElementDescriptor item = default(HID.HIDElementDescriptor);
								predictiveParser.AcceptSingleChar(readOnlySpan, '}');
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.usage = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.usagePage = (HID.UsagePage)predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.unit = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.unitExponent = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.logicalMin = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.logicalMax = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.physicalMin = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.physicalMax = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.collectionIndex = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.reportType = (HID.HIDReportType)predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.reportId = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								predictiveParser.AcceptInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.reportSizeInBits = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.reportOffsetInBits = predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.AcceptSingleChar(readOnlySpan, ',');
								predictiveParser.ExpectString(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, ':');
								item.flags = (HID.HIDElementFlags)predictiveParser.ExpectInt(readOnlySpan);
								predictiveParser.ExpectSingleChar(readOnlySpan, '}');
								list.Add(item);
							}
							hiddeviceDescriptor.elements = list.ToArray();
							result = hiddeviceDescriptor;
						}
					}
				}
				catch (Exception)
				{
					result = JsonUtility.FromJson<HID.HIDDeviceDescriptor>(json);
				}
				return result;
			}

			public int vendorId;

			public int productId;

			public int usage;

			public HID.UsagePage usagePage;

			public int inputReportSize;

			public int outputReportSize;

			public int featureReportSize;

			public HID.HIDElementDescriptor[] elements;

			public HID.HIDCollectionDescriptor[] collections;
		}

		public struct HIDDeviceDescriptorBuilder
		{
			public HIDDeviceDescriptorBuilder(HID.UsagePage usagePage, int usage)
			{
				this = default(HID.HIDDeviceDescriptorBuilder);
				this.usagePage = usagePage;
				this.usage = usage;
			}

			public HIDDeviceDescriptorBuilder(HID.GenericDesktop usage)
			{
				this = new HID.HIDDeviceDescriptorBuilder(HID.UsagePage.GenericDesktop, (int)usage);
			}

			public HID.HIDDeviceDescriptorBuilder StartReport(HID.HIDReportType reportType, int reportId = 1)
			{
				this.m_CurrentReportId = reportId;
				this.m_CurrentReportType = reportType;
				this.m_CurrentReportOffsetInBits = 8;
				return this;
			}

			public HID.HIDDeviceDescriptorBuilder AddElement(HID.UsagePage usagePage, int usage, int sizeInBits)
			{
				if (this.m_Elements == null)
				{
					this.m_Elements = new List<HID.HIDElementDescriptor>();
				}
				else
				{
					foreach (HID.HIDElementDescriptor hidelementDescriptor in this.m_Elements)
					{
						if (hidelementDescriptor.reportId == this.m_CurrentReportId && hidelementDescriptor.reportType == this.m_CurrentReportType && hidelementDescriptor.usagePage == usagePage && hidelementDescriptor.usage == usage)
						{
							throw new InvalidOperationException(string.Format("Cannot add two elements with the same usage page '{0}' and usage '0x{1:X} the to same device", usagePage, usage));
						}
					}
				}
				this.m_Elements.Add(new HID.HIDElementDescriptor
				{
					usage = usage,
					usagePage = usagePage,
					reportOffsetInBits = this.m_CurrentReportOffsetInBits,
					reportSizeInBits = sizeInBits,
					reportType = this.m_CurrentReportType,
					reportId = this.m_CurrentReportId
				});
				this.m_CurrentReportOffsetInBits += sizeInBits;
				return this;
			}

			public HID.HIDDeviceDescriptorBuilder AddElement(HID.GenericDesktop usage, int sizeInBits)
			{
				return this.AddElement(HID.UsagePage.GenericDesktop, (int)usage, sizeInBits);
			}

			public HID.HIDDeviceDescriptorBuilder WithPhysicalMinMax(int min, int max)
			{
				int num = this.m_Elements.Count - 1;
				if (num < 0)
				{
					throw new InvalidOperationException("No element has been added to the descriptor yet");
				}
				HID.HIDElementDescriptor value = this.m_Elements[num];
				value.physicalMin = min;
				value.physicalMax = max;
				this.m_Elements[num] = value;
				return this;
			}

			public HID.HIDDeviceDescriptorBuilder WithLogicalMinMax(int min, int max)
			{
				int num = this.m_Elements.Count - 1;
				if (num < 0)
				{
					throw new InvalidOperationException("No element has been added to the descriptor yet");
				}
				HID.HIDElementDescriptor value = this.m_Elements[num];
				value.logicalMin = min;
				value.logicalMax = max;
				this.m_Elements[num] = value;
				return this;
			}

			public HID.HIDDeviceDescriptor Finish()
			{
				HID.HIDDeviceDescriptor result = default(HID.HIDDeviceDescriptor);
				result.usage = this.usage;
				result.usagePage = this.usagePage;
				List<HID.HIDElementDescriptor> elements = this.m_Elements;
				result.elements = ((elements != null) ? elements.ToArray() : null);
				List<HID.HIDCollectionDescriptor> collections = this.m_Collections;
				result.collections = ((collections != null) ? collections.ToArray() : null);
				return result;
			}

			public HID.UsagePage usagePage;

			public int usage;

			private int m_CurrentReportId;

			private HID.HIDReportType m_CurrentReportType;

			private int m_CurrentReportOffsetInBits;

			private List<HID.HIDElementDescriptor> m_Elements;

			private List<HID.HIDCollectionDescriptor> m_Collections;

			private int m_InputReportSize;

			private int m_OutputReportSize;

			private int m_FeatureReportSize;
		}

		public enum UsagePage
		{
			Undefined,
			GenericDesktop,
			Simulation,
			VRControls,
			SportControls,
			GameControls,
			GenericDeviceControls,
			Keyboard,
			LEDs,
			Button,
			Ordinal,
			Telephony,
			Consumer,
			Digitizer,
			PID = 15,
			Unicode,
			AlphanumericDisplay = 20,
			MedicalInstruments = 64,
			Monitor = 128,
			Power = 132,
			BarCodeScanner = 140,
			MagneticStripeReader = 142,
			Camera = 144,
			Arcade,
			VendorDefined = 65280
		}

		public enum GenericDesktop
		{
			Undefined,
			Pointer,
			Mouse,
			Joystick = 4,
			Gamepad,
			Keyboard,
			Keypad,
			MultiAxisController,
			TabletPCControls,
			AssistiveControl,
			X = 48,
			Y,
			Z,
			Rx,
			Ry,
			Rz,
			Slider,
			Dial,
			Wheel,
			HatSwitch,
			CountedBuffer,
			ByteCount,
			MotionWakeup,
			Start,
			Select,
			Vx = 64,
			Vy,
			Vz,
			Vbrx,
			Vbry,
			Vbrz,
			Vno,
			FeatureNotification,
			ResolutionMultiplier,
			SystemControl = 128,
			SystemPowerDown,
			SystemSleep,
			SystemWakeUp,
			SystemContextMenu,
			SystemMainMenu,
			SystemAppMenu,
			SystemMenuHelp,
			SystemMenuExit,
			SystemMenuSelect,
			SystemMenuRight,
			SystemMenuLeft,
			SystemMenuUp,
			SystemMenuDown,
			SystemColdRestart,
			SystemWarmRestart,
			DpadUp,
			DpadDown,
			DpadRight,
			DpadLeft,
			SystemDock = 160,
			SystemUndock,
			SystemSetup,
			SystemBreak,
			SystemDebuggerBreak,
			ApplicationBreak,
			ApplicationDebuggerBreak,
			SystemSpeakerMute,
			SystemHibernate,
			SystemDisplayInvert = 176,
			SystemDisplayInternal,
			SystemDisplayExternal,
			SystemDisplayBoth,
			SystemDisplayDual,
			SystemDisplayToggleIntExt,
			SystemDisplaySwapPrimarySecondary,
			SystemDisplayLCDAutoScale
		}

		public enum Simulation
		{
			Undefined,
			FlightSimulationDevice,
			AutomobileSimulationDevice,
			TankSimulationDevice,
			SpaceshipSimulationDevice,
			SubmarineSimulationDevice,
			SailingSimulationDevice,
			MotorcycleSimulationDevice,
			SportsSimulationDevice,
			AirplaneSimulationDevice,
			HelicopterSimulationDevice,
			MagicCarpetSimulationDevice,
			BicylcleSimulationDevice,
			FlightControlStick = 32,
			FlightStick,
			CyclicControl,
			CyclicTrim,
			FlightYoke,
			TrackControl,
			Aileron = 176,
			AileronTrim,
			AntiTorqueControl,
			AutopilotEnable,
			ChaffRelease,
			CollectiveControl,
			DiveBreak,
			ElectronicCountermeasures,
			Elevator,
			ElevatorTrim,
			Rudder,
			Throttle,
			FlightCommunications,
			FlareRelease,
			LandingGear,
			ToeBreak,
			Trigger,
			WeaponsArm,
			WeaponsSelect,
			WingFlaps,
			Accelerator,
			Brake,
			Clutch,
			Shifter,
			Steering,
			TurretDirection,
			BarrelElevation,
			DivePlane,
			Ballast,
			BicycleCrank,
			HandleBars,
			FrontBrake,
			RearBrake
		}

		public enum Button
		{
			Undefined,
			Primary,
			Secondary,
			Tertiary
		}
	}
}
