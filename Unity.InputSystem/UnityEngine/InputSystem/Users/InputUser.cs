using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Users
{
	public struct InputUser : IEquatable<InputUser>
	{
		public bool valid
		{
			get
			{
				if (this.m_Id == 0U)
				{
					return false;
				}
				for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
				{
					if (InputUser.s_GlobalState.allUsers[i].m_Id == this.m_Id)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int index
		{
			get
			{
				if (this.m_Id == 0U)
				{
					throw new InvalidOperationException("Invalid user");
				}
				int num = InputUser.TryFindUserIndex(this.m_Id);
				if (num == -1)
				{
					throw new InvalidOperationException(string.Format("User with ID {0} is no longer valid", this.m_Id));
				}
				return num;
			}
		}

		public uint id
		{
			get
			{
				return this.m_Id;
			}
		}

		public InputUserAccountHandle? platformUserAccountHandle
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].platformUserAccountHandle;
			}
		}

		public string platformUserAccountName
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].platformUserAccountName;
			}
		}

		public string platformUserAccountId
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].platformUserAccountId;
			}
		}

		public ReadOnlyArray<InputDevice> pairedDevices
		{
			get
			{
				int index = this.index;
				return new ReadOnlyArray<InputDevice>(InputUser.s_GlobalState.allPairedDevices, InputUser.s_GlobalState.allUserData[index].deviceStartIndex, InputUser.s_GlobalState.allUserData[index].deviceCount);
			}
		}

		public ReadOnlyArray<InputDevice> lostDevices
		{
			get
			{
				int index = this.index;
				return new ReadOnlyArray<InputDevice>(InputUser.s_GlobalState.allLostDevices, InputUser.s_GlobalState.allUserData[index].lostDeviceStartIndex, InputUser.s_GlobalState.allUserData[index].lostDeviceCount);
			}
		}

		public IInputActionCollection actions
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].actions;
			}
		}

		public InputControlScheme? controlScheme
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].controlScheme;
			}
		}

		public InputControlScheme.MatchResult controlSchemeMatch
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].controlSchemeMatch;
			}
		}

		public bool hasMissingRequiredDevices
		{
			get
			{
				return InputUser.s_GlobalState.allUserData[this.index].controlSchemeMatch.hasMissingRequiredDevices;
			}
		}

		public static ReadOnlyArray<InputUser> all
		{
			get
			{
				return new ReadOnlyArray<InputUser>(InputUser.s_GlobalState.allUsers, 0, InputUser.s_GlobalState.allUserCount);
			}
		}

		public static event Action<InputUser, InputUserChange, InputDevice> onChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputUser.s_GlobalState.onChange.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputUser.s_GlobalState.onChange.RemoveCallback(value);
			}
		}

		public static event Action<InputControl, InputEventPtr> onUnpairedDeviceUsed
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputUser.s_GlobalState.onUnpairedDeviceUsed.AddCallback(value);
				if (InputUser.s_GlobalState.listenForUnpairedDeviceActivity > 0)
				{
					InputUser.HookIntoEvents();
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputUser.s_GlobalState.onUnpairedDeviceUsed.RemoveCallback(value);
				if (InputUser.s_GlobalState.onUnpairedDeviceUsed.length == 0)
				{
					InputUser.UnhookFromDeviceStateChange();
				}
			}
		}

		public static event Func<InputDevice, InputEventPtr, bool> onPrefilterUnpairedDeviceActivity
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputUser.s_GlobalState.onPreFilterUnpairedDeviceUsed.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputUser.s_GlobalState.onPreFilterUnpairedDeviceUsed.RemoveCallback(value);
			}
		}

		public static int listenForUnpairedDeviceActivity
		{
			get
			{
				return InputUser.s_GlobalState.listenForUnpairedDeviceActivity;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Cannot be negative");
				}
				if (value > 0 && InputUser.s_GlobalState.onUnpairedDeviceUsed.length > 0)
				{
					InputUser.HookIntoEvents();
				}
				else if (value == 0)
				{
					InputUser.UnhookFromDeviceStateChange();
				}
				InputUser.s_GlobalState.listenForUnpairedDeviceActivity = value;
			}
		}

		public override string ToString()
		{
			if (!this.valid)
			{
				return string.Format("<Invalid> (id: {0})", this.m_Id);
			}
			string text = string.Join<InputDevice>(",", this.pairedDevices);
			return string.Format("User #{0} (id: {1}, devices: {2}, actions: {3})", new object[]
			{
				this.index,
				this.m_Id,
				text,
				this.actions
			});
		}

		public void AssociateActionsWithUser(IInputActionCollection actions)
		{
			int index = this.index;
			if (InputUser.s_GlobalState.allUserData[index].actions == actions)
			{
				return;
			}
			IInputActionCollection actions2 = InputUser.s_GlobalState.allUserData[index].actions;
			if (actions2 != null)
			{
				actions2.devices = null;
				actions2.bindingMask = null;
			}
			InputUser.s_GlobalState.allUserData[index].actions = actions;
			if (actions != null)
			{
				InputUser.HookIntoActionChange();
				actions.devices = new ReadOnlyArray<InputDevice>?(this.pairedDevices);
				if (InputUser.s_GlobalState.allUserData[index].controlScheme != null)
				{
					this.ActivateControlSchemeInternal(index, InputUser.s_GlobalState.allUserData[index].controlScheme.Value);
				}
			}
		}

		public InputUser.ControlSchemeChangeSyntax ActivateControlScheme(string schemeName)
		{
			if (!string.IsNullOrEmpty(schemeName))
			{
				InputControlScheme scheme;
				this.FindControlScheme(schemeName, out scheme);
				return this.ActivateControlScheme(scheme);
			}
			return this.ActivateControlScheme(default(InputControlScheme));
		}

		private bool TryFindControlScheme(string schemeName, out InputControlScheme scheme)
		{
			if (string.IsNullOrEmpty(schemeName))
			{
				scheme = default(InputControlScheme);
				return false;
			}
			if (InputUser.s_GlobalState.allUserData[this.index].actions == null)
			{
				throw new InvalidOperationException(string.Format("Cannot set control scheme '{0}' by name on user #{1} as not actions have been associated with the user yet (AssociateActionsWithUser)", schemeName, this.index));
			}
			ReadOnlyArray<InputControlScheme> controlSchemes = InputUser.s_GlobalState.allUserData[this.index].actions.controlSchemes;
			for (int i = 0; i < controlSchemes.Count; i++)
			{
				if (string.Compare(controlSchemes[i].name, schemeName, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					scheme = controlSchemes[i];
					return true;
				}
			}
			scheme = default(InputControlScheme);
			return false;
		}

		internal void FindControlScheme(string schemeName, out InputControlScheme scheme)
		{
			if (this.TryFindControlScheme(schemeName, out scheme))
			{
				return;
			}
			throw new ArgumentException(string.Format("Cannot find control scheme '{0}' in actions '{1}'", schemeName, InputUser.s_GlobalState.allUserData[this.index].actions));
		}

		public InputUser.ControlSchemeChangeSyntax ActivateControlScheme(InputControlScheme scheme)
		{
			int index = this.index;
			InputControlScheme? controlScheme = InputUser.s_GlobalState.allUserData[index].controlScheme;
			if (controlScheme == null || (controlScheme != null && controlScheme.GetValueOrDefault() != scheme) || (scheme == default(InputControlScheme) && InputUser.s_GlobalState.allUserData[index].controlScheme != null))
			{
				this.ActivateControlSchemeInternal(index, scheme);
				InputUser.Notify(index, InputUserChange.ControlSchemeChanged, null);
			}
			return new InputUser.ControlSchemeChangeSyntax
			{
				m_UserIndex = index
			};
		}

		private void ActivateControlSchemeInternal(int userIndex, InputControlScheme scheme)
		{
			bool flag = scheme == default(InputControlScheme);
			if (flag)
			{
				InputUser.s_GlobalState.allUserData[userIndex].controlScheme = null;
			}
			else
			{
				InputUser.s_GlobalState.allUserData[userIndex].controlScheme = new InputControlScheme?(scheme);
			}
			if (InputUser.s_GlobalState.allUserData[userIndex].actions != null)
			{
				if (flag)
				{
					InputUser.s_GlobalState.allUserData[userIndex].actions.bindingMask = null;
					InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch.Dispose();
					InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch = default(InputControlScheme.MatchResult);
					return;
				}
				InputUser.s_GlobalState.allUserData[userIndex].actions.bindingMask = new InputBinding?(new InputBinding
				{
					groups = scheme.bindingGroup
				});
				InputUser.UpdateControlSchemeMatch(userIndex, false);
				if (InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch.isSuccessfulMatch)
				{
					InputUser.RemoveLostDevicesForUser(userIndex);
				}
			}
		}

		public void UnpairDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			int index = this.index;
			if (!this.pairedDevices.ContainsReference(device))
			{
				return;
			}
			InputUser.RemoveDeviceFromUser(index, device, false);
		}

		public void UnpairDevices()
		{
			int index = this.index;
			InputUser.RemoveLostDevicesForUser(index);
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				while (InputUser.s_GlobalState.allUserData[index].deviceCount > 0)
				{
					this.UnpairDevice(InputUser.s_GlobalState.allPairedDevices[InputUser.s_GlobalState.allUserData[index].deviceStartIndex + InputUser.s_GlobalState.allUserData[index].deviceCount - 1]);
				}
			}
			if (InputUser.s_GlobalState.allUserData[index].controlScheme != null)
			{
				InputUser.UpdateControlSchemeMatch(index, false);
			}
		}

		private static void RemoveLostDevicesForUser(int userIndex)
		{
			int lostDeviceCount = InputUser.s_GlobalState.allUserData[userIndex].lostDeviceCount;
			if (lostDeviceCount > 0)
			{
				int lostDeviceStartIndex = InputUser.s_GlobalState.allUserData[userIndex].lostDeviceStartIndex;
				ArrayHelpers.EraseSliceWithCapacity<InputDevice>(ref InputUser.s_GlobalState.allLostDevices, ref InputUser.s_GlobalState.allLostDeviceCount, lostDeviceStartIndex, lostDeviceCount);
				InputUser.s_GlobalState.allUserData[userIndex].lostDeviceCount = 0;
				InputUser.s_GlobalState.allUserData[userIndex].lostDeviceStartIndex = 0;
				for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
				{
					if (InputUser.s_GlobalState.allUserData[i].lostDeviceStartIndex > lostDeviceStartIndex)
					{
						InputUser.UserData[] allUserData = InputUser.s_GlobalState.allUserData;
						int num = i;
						allUserData[num].lostDeviceStartIndex = allUserData[num].lostDeviceStartIndex - lostDeviceCount;
					}
				}
			}
		}

		public void UnpairDevicesAndRemoveUser()
		{
			this.UnpairDevices();
			InputUser.RemoveUser(this.index);
			this.m_Id = 0U;
		}

		public static InputControlList<InputDevice> GetUnpairedInputDevices()
		{
			InputControlList<InputDevice> result = new InputControlList<InputDevice>(Allocator.Temp, 0);
			InputUser.GetUnpairedInputDevices(ref result);
			return result;
		}

		public static int GetUnpairedInputDevices(ref InputControlList<InputDevice> list)
		{
			int count = list.Count;
			foreach (InputDevice inputDevice in InputSystem.devices)
			{
				if (!InputUser.s_GlobalState.allPairedDevices.ContainsReference(InputUser.s_GlobalState.allPairedDeviceCount, inputDevice))
				{
					list.Add(inputDevice);
				}
			}
			return list.Count - count;
		}

		public static InputUser? FindUserPairedToDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			int num = InputUser.TryFindUserIndex(device);
			if (num == -1)
			{
				return null;
			}
			return new InputUser?(InputUser.s_GlobalState.allUsers[num]);
		}

		public static InputUser? FindUserByAccount(InputUserAccountHandle platformUserAccountHandle)
		{
			if (platformUserAccountHandle == default(InputUserAccountHandle))
			{
				throw new ArgumentException("Empty platform user account handle", "platformUserAccountHandle");
			}
			int num = InputUser.TryFindUserIndex(platformUserAccountHandle);
			if (num == -1)
			{
				return null;
			}
			return new InputUser?(InputUser.s_GlobalState.allUsers[num]);
		}

		public static InputUser CreateUserWithoutPairedDevices()
		{
			int num = InputUser.AddUser();
			return InputUser.s_GlobalState.allUsers[num];
		}

		public static InputUser PerformPairingWithDevice(InputDevice device, InputUser user = default(InputUser), InputUserPairingOptions options = InputUserPairingOptions.None)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (user != default(InputUser) && !user.valid)
			{
				throw new ArgumentException("Invalid user", "user");
			}
			int num;
			if (user == default(InputUser))
			{
				num = InputUser.AddUser();
			}
			else
			{
				num = user.index;
				if ((options & InputUserPairingOptions.UnpairCurrentDevicesFromUser) != InputUserPairingOptions.None)
				{
					user.UnpairDevices();
				}
				if (user.pairedDevices.ContainsReference(device))
				{
					if ((options & InputUserPairingOptions.ForcePlatformUserAccountSelection) != InputUserPairingOptions.None)
					{
						InputUser.InitiateUserAccountSelection(num, device, options);
					}
					return user;
				}
			}
			if (!InputUser.InitiateUserAccountSelection(num, device, options))
			{
				InputUser.AddDeviceToUser(num, device, false, false);
			}
			return InputUser.s_GlobalState.allUsers[num];
		}

		private static bool InitiateUserAccountSelection(int userIndex, InputDevice device, InputUserPairingOptions options)
		{
			long num = ((options & InputUserPairingOptions.ForcePlatformUserAccountSelection) == InputUserPairingOptions.None) ? InputUser.UpdatePlatformUserAccount(userIndex, device) : 0L;
			if (((options & InputUserPairingOptions.ForcePlatformUserAccountSelection) != InputUserPairingOptions.None || (num != -1L && (num & 2L) == 0L && (options & InputUserPairingOptions.ForceNoPlatformUserAccountSelection) == InputUserPairingOptions.None)) && InputUser.InitiateUserAccountSelectionAtPlatformLevel(device))
			{
				InputUser.UserData[] allUserData = InputUser.s_GlobalState.allUserData;
				allUserData[userIndex].flags = (allUserData[userIndex].flags | InputUser.UserFlags.UserAccountSelectionInProgress);
				InputUser.s_GlobalState.ongoingAccountSelections.Append(new InputUser.OngoingAccountSelection
				{
					device = device,
					userId = InputUser.s_GlobalState.allUsers[userIndex].id
				});
				InputUser.HookIntoDeviceChange();
				InputUser.Notify(userIndex, InputUserChange.AccountSelectionInProgress, device);
				return true;
			}
			return false;
		}

		public bool Equals(InputUser other)
		{
			return this.m_Id == other.m_Id;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is InputUser && this.Equals((InputUser)obj);
		}

		public override int GetHashCode()
		{
			return (int)this.m_Id;
		}

		public static bool operator ==(InputUser left, InputUser right)
		{
			return left.m_Id == right.m_Id;
		}

		public static bool operator !=(InputUser left, InputUser right)
		{
			return left.m_Id != right.m_Id;
		}

		private static int AddUser()
		{
			uint num = InputUser.s_GlobalState.lastUserId + 1U;
			InputUser.s_GlobalState.lastUserId = num;
			uint id = num;
			int allUserCount = InputUser.s_GlobalState.allUserCount;
			ArrayHelpers.AppendWithCapacity<InputUser>(ref InputUser.s_GlobalState.allUsers, ref allUserCount, new InputUser
			{
				m_Id = id
			}, 10);
			int num2 = ArrayHelpers.AppendWithCapacity<InputUser.UserData>(ref InputUser.s_GlobalState.allUserData, ref InputUser.s_GlobalState.allUserCount, default(InputUser.UserData), 10);
			InputUser.Notify(num2, InputUserChange.Added, null);
			return num2;
		}

		private static void RemoveUser(int userIndex)
		{
			if (InputUser.s_GlobalState.allUserData[userIndex].controlScheme != null && InputUser.s_GlobalState.allUserData[userIndex].actions != null)
			{
				InputUser.s_GlobalState.allUserData[userIndex].actions.bindingMask = null;
			}
			InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch.Dispose();
			InputUser.RemoveLostDevicesForUser(userIndex);
			for (int i = 0; i < InputUser.s_GlobalState.ongoingAccountSelections.length; i++)
			{
				if (InputUser.s_GlobalState.ongoingAccountSelections[i].userId == InputUser.s_GlobalState.allUsers[userIndex].id)
				{
					InputUser.s_GlobalState.ongoingAccountSelections.RemoveAtByMovingTailWithCapacity(i);
					i--;
				}
			}
			InputUser.Notify(userIndex, InputUserChange.Removed, null);
			int allUserCount = InputUser.s_GlobalState.allUserCount;
			InputUser.s_GlobalState.allUsers.EraseAtWithCapacity(ref allUserCount, userIndex);
			InputUser.s_GlobalState.allUserData.EraseAtWithCapacity(ref InputUser.s_GlobalState.allUserCount, userIndex);
			if (InputUser.s_GlobalState.allUserCount == 0)
			{
				InputUser.UnhookFromDeviceChange();
				InputUser.UnhookFromActionChange();
			}
		}

		private static void Notify(int userIndex, InputUserChange change, InputDevice device)
		{
			if (InputUser.s_GlobalState.onChange.length == 0)
			{
				return;
			}
			InputUser.s_GlobalState.onChange.LockForChanges();
			for (int i = 0; i < InputUser.s_GlobalState.onChange.length; i++)
			{
				try
				{
					InputUser.s_GlobalState.onChange[i](InputUser.s_GlobalState.allUsers[userIndex], change, device);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.GetType().Name + " while executing 'InputUser.onChange' callbacks");
					Debug.LogException(ex);
				}
			}
			InputUser.s_GlobalState.onChange.UnlockForChanges();
		}

		private static int TryFindUserIndex(uint userId)
		{
			for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
			{
				if (InputUser.s_GlobalState.allUsers[i].m_Id == userId)
				{
					return i;
				}
			}
			return -1;
		}

		private static int TryFindUserIndex(InputUserAccountHandle platformHandle)
		{
			for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
			{
				InputUserAccountHandle? platformUserAccountHandle = InputUser.s_GlobalState.allUserData[i].platformUserAccountHandle;
				if (platformUserAccountHandle != null && (platformUserAccountHandle == null || platformUserAccountHandle.GetValueOrDefault() == platformHandle))
				{
					return i;
				}
			}
			return -1;
		}

		private static int TryFindUserIndex(InputDevice device)
		{
			int num = InputUser.s_GlobalState.allPairedDevices.IndexOfReference(device, InputUser.s_GlobalState.allPairedDeviceCount);
			if (num == -1)
			{
				return -1;
			}
			for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
			{
				int deviceStartIndex = InputUser.s_GlobalState.allUserData[i].deviceStartIndex;
				if (deviceStartIndex <= num && num < deviceStartIndex + InputUser.s_GlobalState.allUserData[i].deviceCount)
				{
					return i;
				}
			}
			return -1;
		}

		private static void AddDeviceToUser(int userIndex, InputDevice device, bool asLostDevice = false, bool dontUpdateControlScheme = false)
		{
			int num = asLostDevice ? InputUser.s_GlobalState.allUserData[userIndex].lostDeviceCount : InputUser.s_GlobalState.allUserData[userIndex].deviceCount;
			int num2 = asLostDevice ? InputUser.s_GlobalState.allUserData[userIndex].lostDeviceStartIndex : InputUser.s_GlobalState.allUserData[userIndex].deviceStartIndex;
			InputUser.s_GlobalState.pairingStateVersion = InputUser.s_GlobalState.pairingStateVersion + 1;
			if (num > 0)
			{
				ArrayHelpers.MoveSlice<InputDevice>(asLostDevice ? InputUser.s_GlobalState.allLostDevices : InputUser.s_GlobalState.allPairedDevices, num2, asLostDevice ? (InputUser.s_GlobalState.allLostDeviceCount - num) : (InputUser.s_GlobalState.allPairedDeviceCount - num), num);
				for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
				{
					if (i != userIndex && (asLostDevice ? InputUser.s_GlobalState.allUserData[i].lostDeviceStartIndex : InputUser.s_GlobalState.allUserData[i].deviceStartIndex) > num2)
					{
						if (asLostDevice)
						{
							InputUser.UserData[] allUserData = InputUser.s_GlobalState.allUserData;
							int num3 = i;
							allUserData[num3].lostDeviceStartIndex = allUserData[num3].lostDeviceStartIndex - num;
						}
						else
						{
							InputUser.UserData[] allUserData2 = InputUser.s_GlobalState.allUserData;
							int num4 = i;
							allUserData2[num4].deviceStartIndex = allUserData2[num4].deviceStartIndex - num;
						}
					}
				}
			}
			if (asLostDevice)
			{
				InputUser.s_GlobalState.allUserData[userIndex].lostDeviceStartIndex = InputUser.s_GlobalState.allLostDeviceCount - num;
				ArrayHelpers.AppendWithCapacity<InputDevice>(ref InputUser.s_GlobalState.allLostDevices, ref InputUser.s_GlobalState.allLostDeviceCount, device, 10);
				InputUser.UserData[] allUserData3 = InputUser.s_GlobalState.allUserData;
				allUserData3[userIndex].lostDeviceCount = allUserData3[userIndex].lostDeviceCount + 1;
			}
			else
			{
				InputUser.s_GlobalState.allUserData[userIndex].deviceStartIndex = InputUser.s_GlobalState.allPairedDeviceCount - num;
				ArrayHelpers.AppendWithCapacity<InputDevice>(ref InputUser.s_GlobalState.allPairedDevices, ref InputUser.s_GlobalState.allPairedDeviceCount, device, 10);
				InputUser.UserData[] allUserData4 = InputUser.s_GlobalState.allUserData;
				allUserData4[userIndex].deviceCount = allUserData4[userIndex].deviceCount + 1;
				IInputActionCollection actions = InputUser.s_GlobalState.allUserData[userIndex].actions;
				if (actions != null)
				{
					actions.devices = new ReadOnlyArray<InputDevice>?(InputUser.s_GlobalState.allUsers[userIndex].pairedDevices);
					if (!dontUpdateControlScheme && InputUser.s_GlobalState.allUserData[userIndex].controlScheme != null)
					{
						InputUser.UpdateControlSchemeMatch(userIndex, false);
					}
				}
			}
			InputUser.HookIntoDeviceChange();
			InputUser.Notify(userIndex, asLostDevice ? InputUserChange.DeviceLost : InputUserChange.DevicePaired, device);
		}

		private static void RemoveDeviceFromUser(int userIndex, InputDevice device, bool asLostDevice = false)
		{
			int num = asLostDevice ? InputUser.s_GlobalState.allLostDevices.IndexOfReference(device, InputUser.s_GlobalState.allLostDeviceCount) : InputUser.s_GlobalState.allPairedDevices.IndexOfReference(device, InputUser.s_GlobalState.allUserData[userIndex].deviceStartIndex, InputUser.s_GlobalState.allUserData[userIndex].deviceCount);
			if (num == -1)
			{
				return;
			}
			if (asLostDevice)
			{
				InputUser.s_GlobalState.allLostDevices.EraseAtWithCapacity(ref InputUser.s_GlobalState.allLostDeviceCount, num);
				InputUser.UserData[] allUserData = InputUser.s_GlobalState.allUserData;
				allUserData[userIndex].lostDeviceCount = allUserData[userIndex].lostDeviceCount - 1;
			}
			else
			{
				InputUser.s_GlobalState.pairingStateVersion = InputUser.s_GlobalState.pairingStateVersion + 1;
				InputUser.s_GlobalState.allPairedDevices.EraseAtWithCapacity(ref InputUser.s_GlobalState.allPairedDeviceCount, num);
				InputUser.UserData[] allUserData2 = InputUser.s_GlobalState.allUserData;
				allUserData2[userIndex].deviceCount = allUserData2[userIndex].deviceCount - 1;
			}
			for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
			{
				if ((asLostDevice ? InputUser.s_GlobalState.allUserData[i].lostDeviceStartIndex : InputUser.s_GlobalState.allUserData[i].deviceStartIndex) > num)
				{
					if (asLostDevice)
					{
						InputUser.UserData[] allUserData3 = InputUser.s_GlobalState.allUserData;
						int num2 = i;
						allUserData3[num2].lostDeviceStartIndex = allUserData3[num2].lostDeviceStartIndex - 1;
					}
					else
					{
						InputUser.UserData[] allUserData4 = InputUser.s_GlobalState.allUserData;
						int num3 = i;
						allUserData4[num3].deviceStartIndex = allUserData4[num3].deviceStartIndex - 1;
					}
				}
			}
			if (!asLostDevice)
			{
				for (int j = 0; j < InputUser.s_GlobalState.ongoingAccountSelections.length; j++)
				{
					if (InputUser.s_GlobalState.ongoingAccountSelections[j].userId == InputUser.s_GlobalState.allUsers[userIndex].id && InputUser.s_GlobalState.ongoingAccountSelections[j].device == device)
					{
						InputUser.s_GlobalState.ongoingAccountSelections.RemoveAtByMovingTailWithCapacity(j);
						j--;
					}
				}
				IInputActionCollection actions = InputUser.s_GlobalState.allUserData[userIndex].actions;
				if (actions != null)
				{
					actions.devices = new ReadOnlyArray<InputDevice>?(InputUser.s_GlobalState.allUsers[userIndex].pairedDevices);
					if (InputUser.s_GlobalState.allUsers[userIndex].controlScheme != null)
					{
						InputUser.UpdateControlSchemeMatch(userIndex, false);
					}
				}
				InputUser.Notify(userIndex, InputUserChange.DeviceUnpaired, device);
			}
		}

		private static void UpdateControlSchemeMatch(int userIndex, bool autoPairMissing = false)
		{
			if (InputUser.s_GlobalState.allUserData[userIndex].controlScheme == null)
			{
				return;
			}
			InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch.Dispose();
			InputControlScheme.MatchResult controlSchemeMatch = default(InputControlScheme.MatchResult);
			try
			{
				InputControlScheme value = InputUser.s_GlobalState.allUserData[userIndex].controlScheme.Value;
				if (value.deviceRequirements.Count > 0)
				{
					InputControlList<InputDevice> devices = new InputControlList<InputDevice>(Allocator.Temp, 0);
					try
					{
						devices.AddSlice<ReadOnlyArray<InputDevice>>(InputUser.s_GlobalState.allUsers[userIndex].pairedDevices, -1, -1, 0);
						if (autoPairMissing)
						{
							int count = devices.Count;
							int unpairedInputDevices = InputUser.GetUnpairedInputDevices(ref devices);
							if (InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountHandle != null)
							{
								devices.Sort<InputUser.CompareDevicesByUserAccount>(count, unpairedInputDevices, new InputUser.CompareDevicesByUserAccount
								{
									platformUserAccountHandle = InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountHandle.Value
								});
							}
						}
						controlSchemeMatch = value.PickDevicesFrom<InputControlList<InputDevice>>(devices, null);
						if (controlSchemeMatch.isSuccessfulMatch && autoPairMissing)
						{
							InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch = controlSchemeMatch;
							foreach (InputDevice inputDevice in controlSchemeMatch.devices)
							{
								if (!InputUser.s_GlobalState.allUsers[userIndex].pairedDevices.ContainsReference(inputDevice))
								{
									InputUser.AddDeviceToUser(userIndex, inputDevice, false, true);
								}
							}
						}
					}
					finally
					{
						devices.Dispose();
					}
				}
				InputUser.s_GlobalState.allUserData[userIndex].controlSchemeMatch = controlSchemeMatch;
			}
			catch (Exception)
			{
				controlSchemeMatch.Dispose();
				throw;
			}
		}

		private static long UpdatePlatformUserAccount(int userIndex, InputDevice device)
		{
			InputUserAccountHandle? inputUserAccountHandle;
			string text;
			string text2;
			long num = InputUser.QueryPairedPlatformUserAccount(device, out inputUserAccountHandle, out text, out text2);
			if (num == -1L)
			{
				if ((InputUser.s_GlobalState.allUserData[userIndex].flags & InputUser.UserFlags.UserAccountSelectionInProgress) != (InputUser.UserFlags)0)
				{
					InputUser.Notify(userIndex, InputUserChange.AccountSelectionCanceled, null);
				}
				InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountHandle = null;
				InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountName = null;
				InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountId = null;
				return num;
			}
			if ((InputUser.s_GlobalState.allUserData[userIndex].flags & InputUser.UserFlags.UserAccountSelectionInProgress) != (InputUser.UserFlags)0)
			{
				if ((num & 4L) == 0L)
				{
					if ((num & 16L) != 0L)
					{
						InputUser.Notify(userIndex, InputUserChange.AccountSelectionCanceled, device);
					}
					else
					{
						InputUser.UserData[] allUserData = InputUser.s_GlobalState.allUserData;
						allUserData[userIndex].flags = (allUserData[userIndex].flags & ~InputUser.UserFlags.UserAccountSelectionInProgress);
						InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountHandle = inputUserAccountHandle;
						InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountName = text;
						InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountId = text2;
						InputUser.Notify(userIndex, InputUserChange.AccountSelectionComplete, device);
					}
				}
			}
			else if (InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountHandle != inputUserAccountHandle || InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountId != text2)
			{
				InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountHandle = inputUserAccountHandle;
				InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountName = text;
				InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountId = text2;
				InputUser.Notify(userIndex, InputUserChange.AccountChanged, device);
			}
			else if (InputUser.s_GlobalState.allUserData[userIndex].platformUserAccountName != text)
			{
				InputUser.Notify(userIndex, InputUserChange.AccountNameChanged, device);
			}
			return num;
		}

		private static long QueryPairedPlatformUserAccount(InputDevice device, out InputUserAccountHandle? platformAccountHandle, out string platformAccountName, out string platformAccountId)
		{
			QueryPairedUserAccountCommand queryPairedUserAccountCommand = QueryPairedUserAccountCommand.Create();
			long num = device.ExecuteCommand<QueryPairedUserAccountCommand>(ref queryPairedUserAccountCommand);
			if (num == -1L)
			{
				platformAccountHandle = null;
				platformAccountName = null;
				platformAccountId = null;
				return -1L;
			}
			if ((num & 2L) != 0L)
			{
				platformAccountHandle = new InputUserAccountHandle?(new InputUserAccountHandle(device.description.interfaceName ?? "<Unknown>", queryPairedUserAccountCommand.handle));
				platformAccountName = queryPairedUserAccountCommand.name;
				platformAccountId = queryPairedUserAccountCommand.id;
			}
			else
			{
				platformAccountHandle = null;
				platformAccountName = null;
				platformAccountId = null;
			}
			return num;
		}

		private static bool InitiateUserAccountSelectionAtPlatformLevel(InputDevice device)
		{
			InitiateUserAccountPairingCommand initiateUserAccountPairingCommand = InitiateUserAccountPairingCommand.Create();
			long num = device.ExecuteCommand<InitiateUserAccountPairingCommand>(ref initiateUserAccountPairingCommand);
			if (num == -2L)
			{
				throw new InvalidOperationException("User pairing already in progress");
			}
			return num == 1L;
		}

		private static void OnActionChange(object obj, InputActionChange change)
		{
			if (change == InputActionChange.BoundControlsChanged)
			{
				for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
				{
					if (InputUser.s_GlobalState.allUsers[i].actions == obj)
					{
						InputUser.Notify(i, InputUserChange.ControlsChanged, null);
					}
				}
			}
		}

		private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (change == InputDeviceChange.Added)
			{
				for (int num = InputUser.FindLostDevice(device, 0); num != -1; num = InputUser.FindLostDevice(device, num))
				{
					int userIndex = -1;
					for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
					{
						int lostDeviceStartIndex = InputUser.s_GlobalState.allUserData[i].lostDeviceStartIndex;
						if (lostDeviceStartIndex <= num && num < lostDeviceStartIndex + InputUser.s_GlobalState.allUserData[i].lostDeviceCount)
						{
							userIndex = i;
							break;
						}
					}
					InputUser.RemoveDeviceFromUser(userIndex, InputUser.s_GlobalState.allLostDevices[num], true);
					InputUser.Notify(userIndex, InputUserChange.DeviceRegained, device);
					InputUser.AddDeviceToUser(userIndex, device, false, false);
				}
				return;
			}
			if (change == InputDeviceChange.Removed)
			{
				for (int num2 = InputUser.s_GlobalState.allPairedDevices.IndexOfReference(device, InputUser.s_GlobalState.allPairedDeviceCount); num2 != -1; num2 = InputUser.s_GlobalState.allPairedDevices.IndexOfReference(device, InputUser.s_GlobalState.allPairedDeviceCount))
				{
					int userIndex2 = -1;
					for (int j = 0; j < InputUser.s_GlobalState.allUserCount; j++)
					{
						int deviceStartIndex = InputUser.s_GlobalState.allUserData[j].deviceStartIndex;
						if (deviceStartIndex <= num2 && num2 < deviceStartIndex + InputUser.s_GlobalState.allUserData[j].deviceCount)
						{
							userIndex2 = j;
							break;
						}
					}
					InputUser.AddDeviceToUser(userIndex2, device, true, false);
					InputUser.RemoveDeviceFromUser(userIndex2, device, false);
				}
				return;
			}
			if (change != InputDeviceChange.ConfigurationChanged)
			{
				return;
			}
			bool flag = false;
			for (int k = 0; k < InputUser.s_GlobalState.ongoingAccountSelections.length; k++)
			{
				if (InputUser.s_GlobalState.ongoingAccountSelections[k].device == device)
				{
					InputUser inputUser = default(InputUser);
					inputUser.m_Id = InputUser.s_GlobalState.ongoingAccountSelections[k].userId;
					int index = inputUser.index;
					if ((InputUser.UpdatePlatformUserAccount(index, device) & 4L) == 0L)
					{
						flag = true;
						InputUser.s_GlobalState.ongoingAccountSelections.RemoveAtByMovingTailWithCapacity(k);
						k--;
						if (!InputUser.s_GlobalState.allUsers[index].pairedDevices.ContainsReference(device))
						{
							InputUser.AddDeviceToUser(index, device, false, false);
						}
					}
				}
			}
			if (!flag)
			{
				int num5;
				for (int num3 = InputUser.s_GlobalState.allPairedDevices.IndexOfReference(device, InputUser.s_GlobalState.allPairedDeviceCount); num3 != -1; num3 = InputUser.s_GlobalState.allPairedDevices.IndexOfReference(device, num5, InputUser.s_GlobalState.allPairedDeviceCount - num5))
				{
					int num4 = -1;
					for (int l = 0; l < InputUser.s_GlobalState.allUserCount; l++)
					{
						int deviceStartIndex2 = InputUser.s_GlobalState.allUserData[l].deviceStartIndex;
						if (deviceStartIndex2 <= num3 && num3 < deviceStartIndex2 + InputUser.s_GlobalState.allUserData[l].deviceCount)
						{
							num4 = l;
							break;
						}
					}
					InputUser.UpdatePlatformUserAccount(num4, device);
					num5 = num3 + Math.Max(1, InputUser.s_GlobalState.allUserData[num4].deviceCount);
				}
			}
		}

		private static int FindLostDevice(InputDevice device, int startIndex = 0)
		{
			int deviceId = device.deviceId;
			for (int i = startIndex; i < InputUser.s_GlobalState.allLostDeviceCount; i++)
			{
				InputDevice inputDevice = InputUser.s_GlobalState.allLostDevices[i];
				if (device == inputDevice || inputDevice.deviceId == deviceId)
				{
					return i;
				}
			}
			return -1;
		}

		private static void OnEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (InputUser.s_GlobalState.listenForUnpairedDeviceActivity == 0)
			{
				return;
			}
			FourCC type = eventPtr.type;
			if (type != 1398030676 && type != 1145852993)
			{
				return;
			}
			if (!device.enabled)
			{
				return;
			}
			if (InputUser.s_GlobalState.allPairedDevices.ContainsReference(InputUser.s_GlobalState.allPairedDeviceCount, device))
			{
				return;
			}
			if (!DelegateHelpers.InvokeCallbacksSafe_AnyCallbackReturnsTrue<InputDevice, InputEventPtr>(ref InputUser.s_GlobalState.onPreFilterUnpairedDeviceUsed, device, eventPtr, "InputUser.onPreFilterUnpairedDeviceActivity", null))
			{
				return;
			}
			foreach (InputControl arg in eventPtr.EnumerateChangedControls(device, 0.0001f))
			{
				bool flag = false;
				InputUser.s_GlobalState.onUnpairedDeviceUsed.LockForChanges();
				for (int i = 0; i < InputUser.s_GlobalState.onUnpairedDeviceUsed.length; i++)
				{
					int pairingStateVersion = InputUser.s_GlobalState.pairingStateVersion;
					try
					{
						InputUser.s_GlobalState.onUnpairedDeviceUsed[i](arg, eventPtr);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.GetType().Name + " while executing 'InputUser.onUnpairedDeviceUsed' callbacks");
						Debug.LogException(ex);
					}
					if (pairingStateVersion != InputUser.s_GlobalState.pairingStateVersion && InputUser.FindUserPairedToDevice(device) != null)
					{
						flag = true;
						break;
					}
				}
				InputUser.s_GlobalState.onUnpairedDeviceUsed.UnlockForChanges();
				if (flag)
				{
					break;
				}
			}
		}

		internal static ISavedState SaveAndResetState()
		{
			ISavedState result = new SavedStructState<InputUser.GlobalState>(ref InputUser.s_GlobalState, delegate(ref InputUser.GlobalState state)
			{
				InputUser.s_GlobalState = state;
			}, delegate()
			{
				InputUser.DisposeAndResetGlobalState();
			});
			InputUser.s_GlobalState = default(InputUser.GlobalState);
			return result;
		}

		private static void HookIntoActionChange()
		{
			if (InputUser.s_GlobalState.onActionChangeHooked)
			{
				return;
			}
			if (InputUser.s_GlobalState.actionChangeDelegate == null)
			{
				InputUser.s_GlobalState.actionChangeDelegate = new Action<object, InputActionChange>(InputUser.OnActionChange);
			}
			InputSystem.onActionChange += InputUser.OnActionChange;
			InputUser.s_GlobalState.onActionChangeHooked = true;
		}

		private static void UnhookFromActionChange()
		{
			if (!InputUser.s_GlobalState.onActionChangeHooked)
			{
				return;
			}
			InputSystem.onActionChange -= InputUser.OnActionChange;
			InputUser.s_GlobalState.onActionChangeHooked = false;
		}

		private static void HookIntoDeviceChange()
		{
			if (InputUser.s_GlobalState.onDeviceChangeHooked)
			{
				return;
			}
			if (InputUser.s_GlobalState.onDeviceChangeDelegate == null)
			{
				InputUser.s_GlobalState.onDeviceChangeDelegate = new Action<InputDevice, InputDeviceChange>(InputUser.OnDeviceChange);
			}
			InputSystem.onDeviceChange += InputUser.s_GlobalState.onDeviceChangeDelegate;
			InputUser.s_GlobalState.onDeviceChangeHooked = true;
		}

		private static void UnhookFromDeviceChange()
		{
			if (!InputUser.s_GlobalState.onDeviceChangeHooked)
			{
				return;
			}
			InputSystem.onDeviceChange -= InputUser.s_GlobalState.onDeviceChangeDelegate;
			InputUser.s_GlobalState.onDeviceChangeHooked = false;
		}

		private static void HookIntoEvents()
		{
			if (InputUser.s_GlobalState.onEventHooked)
			{
				return;
			}
			if (InputUser.s_GlobalState.onEventDelegate == null)
			{
				InputUser.s_GlobalState.onEventDelegate = new Action<InputEventPtr, InputDevice>(InputUser.OnEvent);
			}
			InputSystem.onEvent += InputUser.s_GlobalState.onEventDelegate;
			InputUser.s_GlobalState.onEventHooked = true;
		}

		private static void UnhookFromDeviceStateChange()
		{
			if (!InputUser.s_GlobalState.onEventHooked)
			{
				return;
			}
			InputSystem.onEvent -= InputUser.s_GlobalState.onEventDelegate;
			InputUser.s_GlobalState.onEventHooked = false;
		}

		private static void DisposeAndResetGlobalState()
		{
			for (int i = 0; i < InputUser.s_GlobalState.allUserCount; i++)
			{
				InputUser.s_GlobalState.allUserData[i].controlSchemeMatch.Dispose();
			}
			uint lastUserId = InputUser.s_GlobalState.lastUserId;
			InputUser.s_GlobalState = default(InputUser.GlobalState);
			InputUser.s_GlobalState.lastUserId = lastUserId;
		}

		internal static void ResetGlobals()
		{
			InputUser.UnhookFromActionChange();
			InputUser.UnhookFromDeviceChange();
			InputUser.UnhookFromDeviceStateChange();
			InputUser.DisposeAndResetGlobalState();
		}

		public const uint InvalidId = 0U;

		private static readonly ProfilerMarker k_InputUserOnChangeMarker = new ProfilerMarker("InputUser.onChange");

		private static readonly ProfilerMarker k_InputCheckForUnpairMarker = new ProfilerMarker("InputCheckForUnpairedDeviceActivity");

		private uint m_Id;

		private static InputUser.GlobalState s_GlobalState;

		public struct ControlSchemeChangeSyntax
		{
			public InputUser.ControlSchemeChangeSyntax AndPairRemainingDevices()
			{
				InputUser.UpdateControlSchemeMatch(this.m_UserIndex, true);
				return this;
			}

			internal int m_UserIndex;
		}

		[Flags]
		internal enum UserFlags
		{
			BindToAllDevices = 1,
			UserAccountSelectionInProgress = 2
		}

		private struct UserData
		{
			public InputUserAccountHandle? platformUserAccountHandle;

			public string platformUserAccountName;

			public string platformUserAccountId;

			public int deviceCount;

			public int deviceStartIndex;

			public IInputActionCollection actions;

			public InputControlScheme? controlScheme;

			public InputControlScheme.MatchResult controlSchemeMatch;

			public int lostDeviceCount;

			public int lostDeviceStartIndex;

			public InputUser.UserFlags flags;
		}

		private struct CompareDevicesByUserAccount : IComparer<InputDevice>
		{
			public int Compare(InputDevice x, InputDevice y)
			{
				InputUserAccountHandle? userAccountHandleForDevice = InputUser.CompareDevicesByUserAccount.GetUserAccountHandleForDevice(x);
				InputUserAccountHandle? userAccountHandleForDevice2 = InputUser.CompareDevicesByUserAccount.GetUserAccountHandleForDevice(x);
				InputUserAccountHandle? inputUserAccountHandle = userAccountHandleForDevice;
				InputUserAccountHandle right = this.platformUserAccountHandle;
				if (inputUserAccountHandle != null && (inputUserAccountHandle == null || inputUserAccountHandle.GetValueOrDefault() == right))
				{
					inputUserAccountHandle = userAccountHandleForDevice2;
					right = this.platformUserAccountHandle;
					if (inputUserAccountHandle != null && (inputUserAccountHandle == null || inputUserAccountHandle.GetValueOrDefault() == right))
					{
						return 0;
					}
				}
				inputUserAccountHandle = userAccountHandleForDevice;
				right = this.platformUserAccountHandle;
				if (inputUserAccountHandle != null && (inputUserAccountHandle == null || inputUserAccountHandle.GetValueOrDefault() == right))
				{
					return -1;
				}
				inputUserAccountHandle = userAccountHandleForDevice2;
				right = this.platformUserAccountHandle;
				if (inputUserAccountHandle != null && (inputUserAccountHandle == null || inputUserAccountHandle.GetValueOrDefault() == right))
				{
					return 1;
				}
				return 0;
			}

			private static InputUserAccountHandle? GetUserAccountHandleForDevice(InputDevice device)
			{
				return null;
			}

			public InputUserAccountHandle platformUserAccountHandle;
		}

		private struct OngoingAccountSelection
		{
			public InputDevice device;

			public uint userId;
		}

		private struct GlobalState
		{
			internal int pairingStateVersion;

			internal uint lastUserId;

			internal int allUserCount;

			internal int allPairedDeviceCount;

			internal int allLostDeviceCount;

			internal InputUser[] allUsers;

			internal InputUser.UserData[] allUserData;

			internal InputDevice[] allPairedDevices;

			internal InputDevice[] allLostDevices;

			internal InlinedArray<InputUser.OngoingAccountSelection> ongoingAccountSelections;

			internal CallbackArray<Action<InputUser, InputUserChange, InputDevice>> onChange;

			internal CallbackArray<Action<InputControl, InputEventPtr>> onUnpairedDeviceUsed;

			internal CallbackArray<Func<InputDevice, InputEventPtr, bool>> onPreFilterUnpairedDeviceUsed;

			internal Action<object, InputActionChange> actionChangeDelegate;

			internal Action<InputDevice, InputDeviceChange> onDeviceChangeDelegate;

			internal Action<InputEventPtr, InputDevice> onEventDelegate;

			internal bool onActionChangeHooked;

			internal bool onDeviceChangeHooked;

			internal bool onEventHooked;

			internal int listenForUnpairedDeviceActivity;
		}
	}
}
