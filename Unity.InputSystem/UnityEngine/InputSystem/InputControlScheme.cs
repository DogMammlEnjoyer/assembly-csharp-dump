using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public struct InputControlScheme : IEquatable<InputControlScheme>
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public string bindingGroup
		{
			get
			{
				return this.m_BindingGroup;
			}
			set
			{
				this.m_BindingGroup = value;
			}
		}

		public ReadOnlyArray<InputControlScheme.DeviceRequirement> deviceRequirements
		{
			get
			{
				return new ReadOnlyArray<InputControlScheme.DeviceRequirement>(this.m_DeviceRequirements);
			}
		}

		public InputControlScheme(string name, IEnumerable<InputControlScheme.DeviceRequirement> devices = null, string bindingGroup = null)
		{
			this = default(InputControlScheme);
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			this.SetNameAndBindingGroup(name, bindingGroup);
			this.m_DeviceRequirements = null;
			if (devices != null)
			{
				this.m_DeviceRequirements = devices.ToArray<InputControlScheme.DeviceRequirement>();
				if (this.m_DeviceRequirements.Length == 0)
				{
					this.m_DeviceRequirements = null;
				}
			}
		}

		internal void SetNameAndBindingGroup(string name, string bindingGroup = null)
		{
			this.m_Name = name;
			if (!string.IsNullOrEmpty(bindingGroup))
			{
				this.m_BindingGroup = bindingGroup;
				return;
			}
			this.m_BindingGroup = (name.Contains(';') ? name.Replace(";", "") : name);
		}

		public static InputControlScheme? FindControlSchemeForDevices<TDevices, TSchemes>(TDevices devices, TSchemes schemes, InputDevice mustIncludeDevice = null, bool allowUnsuccesfulMatch = false) where TDevices : IReadOnlyList<InputDevice> where TSchemes : IEnumerable<InputControlScheme>
		{
			if (devices == null)
			{
				throw new ArgumentNullException("devices");
			}
			if (schemes == null)
			{
				throw new ArgumentNullException("schemes");
			}
			InputControlScheme value;
			InputControlScheme.MatchResult matchResult;
			if (!InputControlScheme.FindControlSchemeForDevices<TDevices, TSchemes>(devices, schemes, out value, out matchResult, mustIncludeDevice, allowUnsuccesfulMatch))
			{
				return null;
			}
			matchResult.Dispose();
			return new InputControlScheme?(value);
		}

		public static bool FindControlSchemeForDevices<TDevices, TSchemes>(TDevices devices, TSchemes schemes, out InputControlScheme controlScheme, out InputControlScheme.MatchResult matchResult, InputDevice mustIncludeDevice = null, bool allowUnsuccessfulMatch = false) where TDevices : IReadOnlyList<InputDevice> where TSchemes : IEnumerable<InputControlScheme>
		{
			if (devices == null)
			{
				throw new ArgumentNullException("devices");
			}
			if (schemes == null)
			{
				throw new ArgumentNullException("schemes");
			}
			InputControlScheme.MatchResult? matchResult2 = null;
			InputControlScheme? inputControlScheme = null;
			foreach (InputControlScheme value in schemes)
			{
				InputControlScheme.MatchResult value2 = value.PickDevicesFrom<TDevices>(devices, mustIncludeDevice);
				if (!value2.isSuccessfulMatch && (!allowUnsuccessfulMatch || value2.score <= 0f))
				{
					value2.Dispose();
				}
				else if (mustIncludeDevice != null && !value2.devices.Contains(mustIncludeDevice))
				{
					value2.Dispose();
				}
				else if (matchResult2 != null && matchResult2.Value.score >= value2.score)
				{
					value2.Dispose();
				}
				else
				{
					if (matchResult2 != null)
					{
						matchResult2.GetValueOrDefault().Dispose();
					}
					matchResult2 = new InputControlScheme.MatchResult?(value2);
					inputControlScheme = new InputControlScheme?(value);
				}
			}
			matchResult = matchResult2.GetValueOrDefault();
			controlScheme = inputControlScheme.GetValueOrDefault();
			return matchResult2 != null;
		}

		public static InputControlScheme? FindControlSchemeForDevice<TSchemes>(InputDevice device, TSchemes schemes) where TSchemes : IEnumerable<InputControlScheme>
		{
			if (schemes == null)
			{
				throw new ArgumentNullException("schemes");
			}
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			return InputControlScheme.FindControlSchemeForDevices<OneOrMore<InputDevice, ReadOnlyArray<InputDevice>>, TSchemes>(new OneOrMore<InputDevice, ReadOnlyArray<InputDevice>>(device), schemes, null, false);
		}

		public bool SupportsDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			for (int i = 0; i < this.m_DeviceRequirements.Length; i++)
			{
				if (InputControlPath.TryFindControl(device, this.m_DeviceRequirements[i].controlPath, 0) != null)
				{
					return true;
				}
			}
			return false;
		}

		public InputControlScheme.MatchResult PickDevicesFrom<TDevices>(TDevices devices, InputDevice favorDevice = null) where TDevices : IReadOnlyList<InputDevice>
		{
			InputControlScheme.MatchResult result;
			if (this.m_DeviceRequirements == null || this.m_DeviceRequirements.Length == 0)
			{
				result = new InputControlScheme.MatchResult
				{
					m_Result = InputControlScheme.MatchResult.Result.AllSatisfied,
					m_Score = 0.5f
				};
				return result;
			}
			bool flag = true;
			bool flag2 = true;
			int num = this.m_DeviceRequirements.Length;
			float num2 = 0f;
			InputControlList<InputControl> controls = new InputControlList<InputControl>(Allocator.Persistent, num);
			try
			{
				bool flag3 = false;
				bool flag4 = false;
				for (int i = 0; i < num; i++)
				{
					bool isOR = this.m_DeviceRequirements[i].isOR;
					bool isOptional = this.m_DeviceRequirements[i].isOptional;
					if (isOR && flag3)
					{
						controls.Add(null);
					}
					else
					{
						string controlPath = this.m_DeviceRequirements[i].controlPath;
						if (string.IsNullOrEmpty(controlPath))
						{
							num2 += 1f;
							controls.Add(null);
						}
						else
						{
							InputControl inputControl = null;
							int j = 0;
							while (j < devices.Count)
							{
								InputDevice inputDevice = devices[j];
								if (favorDevice != null)
								{
									if (j == 0)
									{
										inputDevice = favorDevice;
									}
									else if (inputDevice == favorDevice)
									{
										inputDevice = devices[0];
									}
								}
								InputControl inputControl2 = InputControlPath.TryFindControl(inputDevice, controlPath, 0);
								if (inputControl2 != null && !controls.Contains(inputControl2))
								{
									inputControl = inputControl2;
									InternedString firstLayout = new InternedString(InputControlPath.TryGetDeviceLayout(controlPath));
									if (firstLayout.IsEmpty())
									{
										num2 += 1f;
										break;
									}
									InternedString layout = inputControl2.device.m_Layout;
									int value;
									if (InputControlLayout.s_Layouts.ComputeDistanceInInheritanceHierarchy(firstLayout, layout, out value))
									{
										num2 += 1f + 1f / (float)(Math.Abs(value) + 1);
										break;
									}
									num2 += 1f;
									break;
								}
								else
								{
									j++;
								}
							}
							if (i + 1 < num && this.m_DeviceRequirements[i + 1].isOR)
							{
								if (inputControl != null)
								{
									flag3 = true;
								}
								else if (!isOptional)
								{
									flag4 = true;
								}
							}
							else if (isOR && i == num - 1)
							{
								if (inputControl == null)
								{
									if (flag4)
									{
										flag = false;
									}
									else
									{
										flag2 = false;
									}
								}
							}
							else
							{
								if (inputControl == null)
								{
									if (isOptional)
									{
										flag2 = false;
									}
									else
									{
										flag = false;
									}
								}
								if (i > 0 && this.m_DeviceRequirements[i - 1].isOR)
								{
									if (!flag3)
									{
										if (flag4)
										{
											flag = false;
										}
										else
										{
											flag2 = false;
										}
									}
									flag3 = false;
								}
							}
							controls.Add(inputControl);
						}
					}
				}
			}
			catch (Exception)
			{
				controls.Dispose();
				throw;
			}
			result = new InputControlScheme.MatchResult
			{
				m_Result = ((!flag) ? InputControlScheme.MatchResult.Result.MissingRequired : ((!flag2) ? InputControlScheme.MatchResult.Result.MissingOptional : InputControlScheme.MatchResult.Result.AllSatisfied)),
				m_Controls = controls,
				m_Requirements = this.m_DeviceRequirements,
				m_Score = num2
			};
			return result;
		}

		public bool Equals(InputControlScheme other)
		{
			if (!string.Equals(this.m_Name, other.m_Name, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(this.m_BindingGroup, other.m_BindingGroup, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			if (this.m_DeviceRequirements == null || this.m_DeviceRequirements.Length == 0)
			{
				return other.m_DeviceRequirements == null || other.m_DeviceRequirements.Length == 0;
			}
			if (other.m_DeviceRequirements == null || this.m_DeviceRequirements.Length != other.m_DeviceRequirements.Length)
			{
				return false;
			}
			int num = this.m_DeviceRequirements.Length;
			for (int i = 0; i < num; i++)
			{
				InputControlScheme.DeviceRequirement right = this.m_DeviceRequirements[i];
				bool flag = false;
				for (int j = 0; j < num; j++)
				{
					if (other.m_DeviceRequirements[j] == right)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is InputControlScheme && this.Equals((InputControlScheme)obj);
		}

		public override int GetHashCode()
		{
			return (((this.m_Name != null) ? this.m_Name.GetHashCode() : 0) * 397 ^ ((this.m_BindingGroup != null) ? this.m_BindingGroup.GetHashCode() : 0)) * 397 ^ ((this.m_DeviceRequirements != null) ? this.m_DeviceRequirements.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.m_Name))
			{
				return base.ToString();
			}
			if (this.m_DeviceRequirements == null)
			{
				return this.m_Name;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Name);
			stringBuilder.Append('(');
			bool flag = true;
			foreach (InputControlScheme.DeviceRequirement deviceRequirement in this.m_DeviceRequirements)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(deviceRequirement.controlPath);
				flag = false;
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public static bool operator ==(InputControlScheme left, InputControlScheme right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputControlScheme left, InputControlScheme right)
		{
			return !left.Equals(right);
		}

		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal string m_BindingGroup;

		[SerializeField]
		internal InputControlScheme.DeviceRequirement[] m_DeviceRequirements;

		public struct MatchResult : IEnumerable<InputControlScheme.MatchResult.Match>, IEnumerable, IDisposable
		{
			public float score
			{
				get
				{
					return this.m_Score;
				}
			}

			public bool isSuccessfulMatch
			{
				get
				{
					return this.m_Result != InputControlScheme.MatchResult.Result.MissingRequired;
				}
			}

			public bool hasMissingRequiredDevices
			{
				get
				{
					return this.m_Result == InputControlScheme.MatchResult.Result.MissingRequired;
				}
			}

			public bool hasMissingOptionalDevices
			{
				get
				{
					return this.m_Result == InputControlScheme.MatchResult.Result.MissingOptional;
				}
			}

			public InputControlList<InputDevice> devices
			{
				get
				{
					if (this.m_Devices.Count == 0 && !this.hasMissingRequiredDevices)
					{
						int count = this.m_Controls.Count;
						if (count != 0)
						{
							this.m_Devices.Capacity = count;
							for (int i = 0; i < count; i++)
							{
								InputControl inputControl = this.m_Controls[i];
								if (inputControl != null)
								{
									InputDevice device = inputControl.device;
									if (!this.m_Devices.Contains(device))
									{
										this.m_Devices.Add(device);
									}
								}
							}
						}
					}
					return this.m_Devices;
				}
			}

			public InputControlScheme.MatchResult.Match this[int index]
			{
				get
				{
					if (index < 0 || this.m_Requirements == null || index >= this.m_Requirements.Length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return new InputControlScheme.MatchResult.Match
					{
						m_RequirementIndex = index,
						m_Requirements = this.m_Requirements,
						m_Controls = this.m_Controls
					};
				}
			}

			public IEnumerator<InputControlScheme.MatchResult.Match> GetEnumerator()
			{
				return new InputControlScheme.MatchResult.Enumerator
				{
					m_Index = -1,
					m_Requirements = this.m_Requirements,
					m_Controls = this.m_Controls
				};
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public void Dispose()
			{
				this.m_Controls.Dispose();
				this.m_Devices.Dispose();
			}

			internal InputControlScheme.MatchResult.Result m_Result;

			internal float m_Score;

			internal InputControlList<InputDevice> m_Devices;

			internal InputControlList<InputControl> m_Controls;

			internal InputControlScheme.DeviceRequirement[] m_Requirements;

			internal enum Result
			{
				AllSatisfied,
				MissingRequired,
				MissingOptional
			}

			public struct Match
			{
				public InputControl control
				{
					get
					{
						return this.m_Controls[this.m_RequirementIndex];
					}
				}

				public InputDevice device
				{
					get
					{
						InputControl control = this.control;
						if (control == null)
						{
							return null;
						}
						return control.device;
					}
				}

				public int requirementIndex
				{
					get
					{
						return this.m_RequirementIndex;
					}
				}

				public InputControlScheme.DeviceRequirement requirement
				{
					get
					{
						return this.m_Requirements[this.m_RequirementIndex];
					}
				}

				public bool isOptional
				{
					get
					{
						return this.requirement.isOptional;
					}
				}

				internal int m_RequirementIndex;

				internal InputControlScheme.DeviceRequirement[] m_Requirements;

				internal InputControlList<InputControl> m_Controls;
			}

			private struct Enumerator : IEnumerator<InputControlScheme.MatchResult.Match>, IEnumerator, IDisposable
			{
				public bool MoveNext()
				{
					this.m_Index++;
					return this.m_Requirements != null && this.m_Index < this.m_Requirements.Length;
				}

				public void Reset()
				{
					this.m_Index = -1;
				}

				public InputControlScheme.MatchResult.Match Current
				{
					get
					{
						if (this.m_Requirements == null || this.m_Index < 0 || this.m_Index >= this.m_Requirements.Length)
						{
							throw new InvalidOperationException("Enumerator is not valid");
						}
						return new InputControlScheme.MatchResult.Match
						{
							m_RequirementIndex = this.m_Index,
							m_Requirements = this.m_Requirements,
							m_Controls = this.m_Controls
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

				internal int m_Index;

				internal InputControlScheme.DeviceRequirement[] m_Requirements;

				internal InputControlList<InputControl> m_Controls;
			}
		}

		[Serializable]
		public struct DeviceRequirement : IEquatable<InputControlScheme.DeviceRequirement>
		{
			public string controlPath
			{
				get
				{
					return this.m_ControlPath;
				}
				set
				{
					this.m_ControlPath = value;
				}
			}

			public bool isOptional
			{
				get
				{
					return (this.m_Flags & InputControlScheme.DeviceRequirement.Flags.Optional) > InputControlScheme.DeviceRequirement.Flags.None;
				}
				set
				{
					if (value)
					{
						this.m_Flags |= InputControlScheme.DeviceRequirement.Flags.Optional;
						return;
					}
					this.m_Flags &= ~InputControlScheme.DeviceRequirement.Flags.Optional;
				}
			}

			public bool isAND
			{
				get
				{
					return !this.isOR;
				}
				set
				{
					this.isOR = !value;
				}
			}

			public bool isOR
			{
				get
				{
					return (this.m_Flags & InputControlScheme.DeviceRequirement.Flags.Or) > InputControlScheme.DeviceRequirement.Flags.None;
				}
				set
				{
					if (value)
					{
						this.m_Flags |= InputControlScheme.DeviceRequirement.Flags.Or;
						return;
					}
					this.m_Flags &= ~InputControlScheme.DeviceRequirement.Flags.Or;
				}
			}

			public override string ToString()
			{
				if (string.IsNullOrEmpty(this.controlPath))
				{
					return base.ToString();
				}
				if (this.isOptional)
				{
					return this.controlPath + " (Optional)";
				}
				return this.controlPath + " (Required)";
			}

			public bool Equals(InputControlScheme.DeviceRequirement other)
			{
				return string.Equals(this.m_ControlPath, other.m_ControlPath) && this.m_Flags == other.m_Flags && string.Equals(this.controlPath, other.controlPath) && this.isOptional == other.isOptional;
			}

			public override bool Equals(object obj)
			{
				return obj != null && obj is InputControlScheme.DeviceRequirement && this.Equals((InputControlScheme.DeviceRequirement)obj);
			}

			public override int GetHashCode()
			{
				return ((((this.m_ControlPath != null) ? this.m_ControlPath.GetHashCode() : 0) * 397 ^ this.m_Flags.GetHashCode()) * 397 ^ ((this.controlPath != null) ? this.controlPath.GetHashCode() : 0)) * 397 ^ this.isOptional.GetHashCode();
			}

			public static bool operator ==(InputControlScheme.DeviceRequirement left, InputControlScheme.DeviceRequirement right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(InputControlScheme.DeviceRequirement left, InputControlScheme.DeviceRequirement right)
			{
				return !left.Equals(right);
			}

			[SerializeField]
			internal string m_ControlPath;

			[SerializeField]
			internal InputControlScheme.DeviceRequirement.Flags m_Flags;

			[Flags]
			internal enum Flags
			{
				None = 0,
				Optional = 1,
				Or = 2
			}
		}

		[Serializable]
		internal struct SchemeJson
		{
			public InputControlScheme ToScheme()
			{
				InputControlScheme.DeviceRequirement[] array = null;
				if (this.devices != null && this.devices.Length != 0)
				{
					int num = this.devices.Length;
					array = new InputControlScheme.DeviceRequirement[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = this.devices[i].ToDeviceEntry();
					}
				}
				return new InputControlScheme
				{
					m_Name = (string.IsNullOrEmpty(this.name) ? null : this.name),
					m_BindingGroup = (string.IsNullOrEmpty(this.bindingGroup) ? null : this.bindingGroup),
					m_DeviceRequirements = array
				};
			}

			public static InputControlScheme.SchemeJson ToJson(InputControlScheme scheme)
			{
				InputControlScheme.SchemeJson.DeviceJson[] array = null;
				if (scheme.m_DeviceRequirements != null && scheme.m_DeviceRequirements.Length != 0)
				{
					int num = scheme.m_DeviceRequirements.Length;
					array = new InputControlScheme.SchemeJson.DeviceJson[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = InputControlScheme.SchemeJson.DeviceJson.From(scheme.m_DeviceRequirements[i]);
					}
				}
				return new InputControlScheme.SchemeJson
				{
					name = scheme.m_Name,
					bindingGroup = scheme.m_BindingGroup,
					devices = array
				};
			}

			public static InputControlScheme.SchemeJson[] ToJson(InputControlScheme[] schemes)
			{
				if (schemes == null || schemes.Length == 0)
				{
					return null;
				}
				int num = schemes.Length;
				InputControlScheme.SchemeJson[] array = new InputControlScheme.SchemeJson[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = InputControlScheme.SchemeJson.ToJson(schemes[i]);
				}
				return array;
			}

			public static InputControlScheme[] ToSchemes(InputControlScheme.SchemeJson[] schemes)
			{
				if (schemes == null || schemes.Length == 0)
				{
					return null;
				}
				int num = schemes.Length;
				InputControlScheme[] array = new InputControlScheme[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = schemes[i].ToScheme();
				}
				return array;
			}

			public string name;

			public string bindingGroup;

			public InputControlScheme.SchemeJson.DeviceJson[] devices;

			[Serializable]
			public struct DeviceJson
			{
				public InputControlScheme.DeviceRequirement ToDeviceEntry()
				{
					return new InputControlScheme.DeviceRequirement
					{
						controlPath = this.devicePath,
						isOptional = this.isOptional,
						isOR = this.isOR
					};
				}

				public static InputControlScheme.SchemeJson.DeviceJson From(InputControlScheme.DeviceRequirement requirement)
				{
					return new InputControlScheme.SchemeJson.DeviceJson
					{
						devicePath = requirement.controlPath,
						isOptional = requirement.isOptional,
						isOR = requirement.isOR
					};
				}

				public string devicePath;

				public bool isOptional;

				public bool isOR;
			}
		}
	}
}
