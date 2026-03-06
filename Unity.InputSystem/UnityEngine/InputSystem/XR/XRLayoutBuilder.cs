using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	internal class XRLayoutBuilder
	{
		private static uint GetSizeOfFeature(XRFeatureDescriptor featureDescriptor)
		{
			switch (featureDescriptor.featureType)
			{
			case FeatureType.Custom:
				return featureDescriptor.customSize;
			case FeatureType.Binary:
				return 1U;
			case FeatureType.DiscreteStates:
				return 4U;
			case FeatureType.Axis1D:
				return 4U;
			case FeatureType.Axis2D:
				return 8U;
			case FeatureType.Axis3D:
				return 12U;
			case FeatureType.Rotation:
				return 16U;
			case FeatureType.Hand:
				return 104U;
			case FeatureType.Bone:
				return 32U;
			case FeatureType.Eyes:
				return 76U;
			default:
				return 0U;
			}
		}

		private static string SanitizeString(string original, bool allowPaths = false)
		{
			int length = original.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				char c = original[i];
				if (char.IsUpper(c) || char.IsLower(c) || char.IsDigit(c) || c == '_' || (allowPaths && c == '/'))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string OnFindLayoutForDevice(ref InputDeviceDescription description, string matchedLayout, InputDeviceExecuteCommandDelegate executeCommandDelegate)
		{
			if (description.interfaceName != "XRInputV1" && description.interfaceName != "XRInput")
			{
				return null;
			}
			if (string.IsNullOrEmpty(description.capabilities))
			{
				return null;
			}
			XRDeviceDescriptor xrdeviceDescriptor;
			try
			{
				xrdeviceDescriptor = XRDeviceDescriptor.FromJson(description.capabilities);
			}
			catch (Exception)
			{
				return null;
			}
			if (xrdeviceDescriptor == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(matchedLayout))
			{
				if ((xrdeviceDescriptor.characteristics & InputDeviceCharacteristics.HeadMounted) != InputDeviceCharacteristics.None)
				{
					matchedLayout = "XRHMD";
				}
				else if ((xrdeviceDescriptor.characteristics & (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller)) == (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller))
				{
					matchedLayout = "XRController";
				}
			}
			string text;
			if (string.IsNullOrEmpty(description.manufacturer))
			{
				text = XRLayoutBuilder.SanitizeString(description.interfaceName, false) + "::" + XRLayoutBuilder.SanitizeString(description.product, false);
			}
			else
			{
				text = string.Concat(new string[]
				{
					XRLayoutBuilder.SanitizeString(description.interfaceName, false),
					"::",
					XRLayoutBuilder.SanitizeString(description.manufacturer, false),
					"::",
					XRLayoutBuilder.SanitizeString(description.product, false)
				});
			}
			XRLayoutBuilder layout = new XRLayoutBuilder
			{
				descriptor = xrdeviceDescriptor,
				parentLayout = matchedLayout,
				interfaceName = description.interfaceName
			};
			InputSystem.RegisterLayoutBuilder(() => layout.Build(), text, matchedLayout, null);
			return text;
		}

		private static string ConvertPotentialAliasToName(InputControlLayout layout, string nameOrAlias)
		{
			InternedString b = new InternedString(nameOrAlias);
			ReadOnlyArray<InputControlLayout.ControlItem> controls = layout.controls;
			for (int i = 0; i < controls.Count; i++)
			{
				InputControlLayout.ControlItem controlItem = controls[i];
				if (controlItem.name == b)
				{
					return nameOrAlias;
				}
				ReadOnlyArray<InternedString> aliases = controlItem.aliases;
				for (int j = 0; j < aliases.Count; j++)
				{
					if (aliases[j] == nameOrAlias)
					{
						return controlItem.name.ToString();
					}
				}
			}
			return nameOrAlias;
		}

		private bool IsSubControl(string name)
		{
			return name.Contains('/');
		}

		private string GetParentControlName(string name)
		{
			int length = name.IndexOf('/');
			return name.Substring(0, length);
		}

		private bool IsPoseControl(List<XRFeatureDescriptor> features, int startIndex)
		{
			for (int i = 0; i < 6; i++)
			{
				if (!features[startIndex + i].name.EndsWith(XRLayoutBuilder.poseSubControlNames[i]) || features[startIndex + i].featureType != XRLayoutBuilder.poseSubControlTypes[i])
				{
					return false;
				}
			}
			return true;
		}

		private InputControlLayout Build()
		{
			InputControlLayout.Builder builder = new InputControlLayout.Builder
			{
				stateFormat = new FourCC('X', 'R', 'S', '0'),
				extendsLayout = this.parentLayout,
				updateBeforeRender = new bool?(true)
			};
			InputControlLayout inputControlLayout = (!string.IsNullOrEmpty(this.parentLayout)) ? InputSystem.LoadLayout(this.parentLayout) : null;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			uint num = 0U;
			for (int i = 0; i < this.descriptor.inputFeatures.Count; i++)
			{
				XRFeatureDescriptor xrfeatureDescriptor = this.descriptor.inputFeatures[i];
				list2.Clear();
				if (xrfeatureDescriptor.usageHints != null)
				{
					foreach (UsageHint usageHint in xrfeatureDescriptor.usageHints)
					{
						if (!string.IsNullOrEmpty(usageHint.content))
						{
							list2.Add(usageHint.content);
						}
					}
				}
				string text = xrfeatureDescriptor.name;
				text = XRLayoutBuilder.SanitizeString(text, true);
				if (inputControlLayout != null)
				{
					text = XRLayoutBuilder.ConvertPotentialAliasToName(inputControlLayout, text);
				}
				text = text.ToLowerInvariant();
				if (this.IsSubControl(text))
				{
					string parentControlName = this.GetParentControlName(text);
					if (!list.Contains(parentControlName) && this.IsPoseControl(this.descriptor.inputFeatures, i))
					{
						builder.AddControl(parentControlName).WithLayout("Pose").WithByteOffset(0U);
						list.Add(parentControlName);
					}
				}
				uint sizeOfFeature = XRLayoutBuilder.GetSizeOfFeature(xrfeatureDescriptor);
				if (!(this.interfaceName == "XRInput") && sizeOfFeature >= 4U && num % 4U != 0U)
				{
					num += 4U - num % 4U;
				}
				switch (xrfeatureDescriptor.featureType)
				{
				case FeatureType.Binary:
					builder.AddControl(text).WithLayout("Button").WithByteOffset(num).WithFormat(InputStateBlock.FormatBit).WithUsages(list2);
					break;
				case FeatureType.DiscreteStates:
					builder.AddControl(text).WithLayout("Integer").WithByteOffset(num).WithFormat(InputStateBlock.FormatInt).WithUsages(list2);
					break;
				case FeatureType.Axis1D:
					builder.AddControl(text).WithLayout("Analog").WithRange(-1f, 1f).WithByteOffset(num).WithFormat(InputStateBlock.FormatFloat).WithUsages(list2);
					break;
				case FeatureType.Axis2D:
					builder.AddControl(text).WithLayout("Stick").WithByteOffset(num).WithFormat(InputStateBlock.FormatVector2).WithUsages(list2);
					builder.AddControl(text + "/x").WithLayout("Analog").WithRange(-1f, 1f);
					builder.AddControl(text + "/y").WithLayout("Analog").WithRange(-1f, 1f);
					break;
				case FeatureType.Axis3D:
					builder.AddControl(text).WithLayout("Vector3").WithByteOffset(num).WithFormat(InputStateBlock.FormatVector3).WithUsages(list2);
					break;
				case FeatureType.Rotation:
					builder.AddControl(text).WithLayout("Quaternion").WithByteOffset(num).WithFormat(InputStateBlock.FormatQuaternion).WithUsages(list2);
					break;
				case FeatureType.Bone:
					builder.AddControl(text).WithLayout("Bone").WithByteOffset(num).WithUsages(list2);
					break;
				case FeatureType.Eyes:
					builder.AddControl(text).WithLayout("Eyes").WithByteOffset(num).WithUsages(list2);
					break;
				}
				num += sizeOfFeature;
			}
			return builder.Build();
		}

		private string parentLayout;

		private string interfaceName;

		private XRDeviceDescriptor descriptor;

		private static readonly string[] poseSubControlNames = new string[]
		{
			"/isTracked",
			"/trackingState",
			"/position",
			"/rotation",
			"/velocity",
			"/angularVelocity"
		};

		private static readonly FeatureType[] poseSubControlTypes = new FeatureType[]
		{
			FeatureType.Binary,
			FeatureType.DiscreteStates,
			FeatureType.Axis3D,
			FeatureType.Rotation,
			FeatureType.Axis3D,
			FeatureType.Axis3D
		};
	}
}
