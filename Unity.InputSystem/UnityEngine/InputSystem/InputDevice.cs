using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public class InputDevice : InputControl
	{
		public InputDeviceDescription description
		{
			get
			{
				return this.m_Description;
			}
		}

		public bool enabled
		{
			get
			{
				return (this.m_DeviceFlags & (InputDevice.DeviceFlags.DisabledInFrontend | InputDevice.DeviceFlags.DisabledWhileInBackground)) == (InputDevice.DeviceFlags)0 && this.QueryEnabledStateFromRuntime();
			}
		}

		public bool canRunInBackground
		{
			get
			{
				return this.canDeviceRunInBackground;
			}
		}

		internal bool canDeviceRunInBackground
		{
			get
			{
				if ((this.m_DeviceFlags & InputDevice.DeviceFlags.CanRunInBackgroundHasBeenQueried) != (InputDevice.DeviceFlags)0)
				{
					return (this.m_DeviceFlags & InputDevice.DeviceFlags.CanRunInBackground) > (InputDevice.DeviceFlags)0;
				}
				QueryCanRunInBackground queryCanRunInBackground = QueryCanRunInBackground.Create();
				this.m_DeviceFlags |= InputDevice.DeviceFlags.CanRunInBackgroundHasBeenQueried;
				if (this.ExecuteCommand<QueryCanRunInBackground>(ref queryCanRunInBackground) >= 0L && queryCanRunInBackground.canRunInBackground)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.CanRunInBackground;
					return true;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.CanRunInBackground;
				return false;
			}
		}

		public bool added
		{
			get
			{
				return this.m_DeviceIndex != -1;
			}
		}

		public bool remote
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.Remote) == InputDevice.DeviceFlags.Remote;
			}
		}

		public bool native
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.Native) == InputDevice.DeviceFlags.Native;
			}
		}

		public bool updateBeforeRender
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.UpdateBeforeRender) == InputDevice.DeviceFlags.UpdateBeforeRender;
			}
		}

		public int deviceId
		{
			get
			{
				return this.m_DeviceId;
			}
		}

		public double lastUpdateTime
		{
			get
			{
				return this.m_LastUpdateTimeInternal - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public bool wasUpdatedThisFrame
		{
			get
			{
				return this.m_CurrentUpdateStepCount == InputUpdate.s_UpdateStepCount;
			}
		}

		public ReadOnlyArray<InputControl> allControls
		{
			get
			{
				return new ReadOnlyArray<InputControl>(this.m_ChildrenForEachControl);
			}
		}

		public override Type valueType
		{
			get
			{
				return typeof(byte[]);
			}
		}

		public override int valueSizeInBytes
		{
			get
			{
				return (int)this.m_StateBlock.alignedSizeInBytes;
			}
		}

		[Obsolete("Use 'InputSystem.devices' instead. (UnityUpgradable) -> InputSystem.devices", false)]
		public static ReadOnlyArray<InputDevice> all
		{
			get
			{
				return InputSystem.devices;
			}
		}

		public InputDevice()
		{
			this.m_DeviceId = 0;
			this.m_ParticipantId = 0;
			this.m_DeviceIndex = -1;
		}

		public unsafe override object ReadValueFromBufferAsObject(void* buffer, int bufferSize)
		{
			throw new NotImplementedException();
		}

		public unsafe override object ReadValueFromStateAsObject(void* statePtr)
		{
			if (this.m_DeviceIndex == -1)
			{
				return null;
			}
			uint alignedSizeInBytes = base.stateBlock.alignedSizeInBytes;
			byte[] array2;
			byte[] array = array2 = new byte[alignedSizeInBytes];
			byte* destination;
			if (array == null || array2.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array2[0];
			}
			byte* source = (byte*)statePtr + this.m_StateBlock.byteOffset;
			UnsafeUtility.MemCpy((void*)destination, (void*)source, (long)((ulong)alignedSizeInBytes));
			array2 = null;
			return array;
		}

		public unsafe override void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			if (bufferPtr == null)
			{
				throw new ArgumentNullException("bufferPtr");
			}
			if (bufferSize < this.valueSizeInBytes)
			{
				throw new ArgumentException(string.Format("Buffer too small (expected: {0}, actual: {1}", this.valueSizeInBytes, bufferSize));
			}
			byte* source = (byte*)statePtr + this.m_StateBlock.byteOffset;
			UnsafeUtility.MemCpy(bufferPtr, (void*)source, (long)((ulong)this.m_StateBlock.alignedSizeInBytes));
		}

		public unsafe override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
		{
			if (firstStatePtr == null)
			{
				throw new ArgumentNullException("firstStatePtr");
			}
			if (secondStatePtr == null)
			{
				throw new ArgumentNullException("secondStatePtr");
			}
			void* ptr = (void*)((byte*)firstStatePtr + this.m_StateBlock.byteOffset);
			byte* ptr2 = (byte*)firstStatePtr + this.m_StateBlock.byteOffset;
			return UnsafeUtility.MemCmp(ptr, (void*)ptr2, (long)((ulong)this.m_StateBlock.alignedSizeInBytes)) == 0;
		}

		internal void NotifyConfigurationChanged()
		{
			base.isConfigUpToDate = false;
			for (int i = 0; i < this.m_ChildrenForEachControl.Length; i++)
			{
				this.m_ChildrenForEachControl[i].isConfigUpToDate = false;
			}
			this.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledStateHasBeenQueriedFromRuntime;
			this.OnConfigurationChanged();
		}

		public virtual void MakeCurrent()
		{
		}

		protected virtual void OnAdded()
		{
		}

		protected virtual void OnRemoved()
		{
		}

		protected virtual void OnConfigurationChanged()
		{
		}

		public unsafe long ExecuteCommand<TCommand>(ref TCommand command) where TCommand : struct, IInputDeviceCommandInfo
		{
			InputDeviceCommand* command2 = (InputDeviceCommand*)UnsafeUtility.AddressOf<TCommand>(ref command);
			InputManager s_Manager = InputSystem.s_Manager;
			s_Manager.m_DeviceCommandCallbacks.LockForChanges();
			for (int i = 0; i < s_Manager.m_DeviceCommandCallbacks.length; i++)
			{
				try
				{
					long? num = s_Manager.m_DeviceCommandCallbacks[i](this, command2);
					if (num != null)
					{
						return num.Value;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.GetType().Name + " while executing 'InputSystem.onDeviceCommand' callbacks");
					Debug.LogException(ex);
				}
			}
			s_Manager.m_DeviceCommandCallbacks.UnlockForChanges();
			return this.ExecuteCommand((InputDeviceCommand*)UnsafeUtility.AddressOf<TCommand>(ref command));
		}

		protected unsafe virtual long ExecuteCommand(InputDeviceCommand* commandPtr)
		{
			return InputRuntime.s_Instance.DeviceCommand(this.deviceId, commandPtr);
		}

		internal bool QueryEnabledStateFromRuntime()
		{
			if ((this.m_DeviceFlags & InputDevice.DeviceFlags.DisabledStateHasBeenQueriedFromRuntime) == (InputDevice.DeviceFlags)0)
			{
				QueryEnabledStateCommand queryEnabledStateCommand = QueryEnabledStateCommand.Create();
				if (this.ExecuteCommand<QueryEnabledStateCommand>(ref queryEnabledStateCommand) >= 0L)
				{
					if (queryEnabledStateCommand.isEnabled)
					{
						this.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledInRuntime;
					}
					else
					{
						this.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledInRuntime;
					}
				}
				else
				{
					this.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledInRuntime;
				}
				this.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledStateHasBeenQueriedFromRuntime;
			}
			return (this.m_DeviceFlags & InputDevice.DeviceFlags.DisabledInRuntime) == (InputDevice.DeviceFlags)0;
		}

		internal bool disabledInFrontend
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.DisabledInFrontend) > (InputDevice.DeviceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledInFrontend;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledInFrontend;
			}
		}

		internal bool disabledInRuntime
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.DisabledInRuntime) > (InputDevice.DeviceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledInRuntime;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledInRuntime;
			}
		}

		internal bool disabledWhileInBackground
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.DisabledWhileInBackground) > (InputDevice.DeviceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledWhileInBackground;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledWhileInBackground;
			}
		}

		internal static uint EncodeStateOffsetToControlMapEntry(uint controlIndex, uint stateOffsetInBits, uint stateSizeInBits)
		{
			return stateOffsetInBits << 19 | stateSizeInBits << 10 | controlIndex;
		}

		internal static void DecodeStateOffsetToControlMapEntry(uint entry, out uint controlIndex, out uint stateOffset, out uint stateSize)
		{
			controlIndex = (entry & 1023U);
			stateOffset = entry >> 19;
			stateSize = (entry >> 10 & 511U);
		}

		internal bool hasControlsWithDefaultState
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.HasControlsWithDefaultState) == InputDevice.DeviceFlags.HasControlsWithDefaultState;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.HasControlsWithDefaultState;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.HasControlsWithDefaultState;
			}
		}

		internal bool hasDontResetControls
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.HasDontResetControls) == InputDevice.DeviceFlags.HasDontResetControls;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.HasDontResetControls;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.HasDontResetControls;
			}
		}

		internal bool hasStateCallbacks
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.HasStateCallbacks) == InputDevice.DeviceFlags.HasStateCallbacks;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.HasStateCallbacks;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.HasStateCallbacks;
			}
		}

		internal bool hasEventMerger
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.HasEventMerger) == InputDevice.DeviceFlags.HasEventMerger;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.HasEventMerger;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.HasEventMerger;
			}
		}

		internal bool hasEventPreProcessor
		{
			get
			{
				return (this.m_DeviceFlags & InputDevice.DeviceFlags.HasEventPreProcessor) == InputDevice.DeviceFlags.HasEventPreProcessor;
			}
			set
			{
				if (value)
				{
					this.m_DeviceFlags |= InputDevice.DeviceFlags.HasEventPreProcessor;
					return;
				}
				this.m_DeviceFlags &= ~InputDevice.DeviceFlags.HasEventPreProcessor;
			}
		}

		internal void AddDeviceUsage(InternedString usage)
		{
			int usageStartIndex = this.m_UsageToControl.LengthSafe<InputControl>() + this.m_UsageCount;
			if (this.m_UsageCount == 0)
			{
				this.m_UsageStartIndex = usageStartIndex;
			}
			ArrayHelpers.AppendWithCapacity<InternedString>(ref this.m_UsagesForEachControl, ref usageStartIndex, usage, 10);
			this.m_UsageCount++;
		}

		internal void RemoveDeviceUsage(InternedString usage)
		{
			int count = this.m_UsageToControl.LengthSafe<InputControl>() + this.m_UsageCount;
			int num = this.m_UsagesForEachControl.IndexOfValue(usage, this.m_UsageStartIndex, count);
			if (num == -1)
			{
				return;
			}
			this.m_UsagesForEachControl.EraseAtWithCapacity(ref count, num);
			this.m_UsageCount--;
			if (this.m_UsageCount == 0)
			{
				this.m_UsageStartIndex = 0;
			}
		}

		internal void ClearDeviceUsages()
		{
			for (int i = this.m_UsageStartIndex; i < this.m_UsageCount; i++)
			{
				this.m_UsagesForEachControl[i] = default(InternedString);
			}
			this.m_UsageCount = 0;
		}

		internal bool RequestSync()
		{
			base.SetOptimizedControlDataTypeRecursively();
			RequestSyncCommand requestSyncCommand = RequestSyncCommand.Create();
			return base.device.ExecuteCommand<RequestSyncCommand>(ref requestSyncCommand) >= 0L;
		}

		internal bool RequestReset()
		{
			base.SetOptimizedControlDataTypeRecursively();
			RequestResetCommand requestResetCommand = RequestResetCommand.Create();
			return base.device.ExecuteCommand<RequestResetCommand>(ref requestResetCommand) >= 0L;
		}

		internal bool ExecuteEnableCommand()
		{
			base.SetOptimizedControlDataTypeRecursively();
			EnableDeviceCommand enableDeviceCommand = EnableDeviceCommand.Create();
			return base.device.ExecuteCommand<EnableDeviceCommand>(ref enableDeviceCommand) >= 0L;
		}

		internal bool ExecuteDisableCommand()
		{
			DisableDeviceCommand disableDeviceCommand = DisableDeviceCommand.Create();
			return base.device.ExecuteCommand<DisableDeviceCommand>(ref disableDeviceCommand) >= 0L;
		}

		internal void NotifyAdded()
		{
			this.OnAdded();
		}

		internal void NotifyRemoved()
		{
			this.OnRemoved();
		}

		internal static TDevice Build<TDevice>(string layoutName = null, string layoutVariants = null, InputDeviceDescription deviceDescription = default(InputDeviceDescription), bool noPrecompiledLayouts = false) where TDevice : InputDevice
		{
			InternedString internedString = new InternedString(layoutName);
			if (internedString.IsEmpty())
			{
				internedString = InputControlLayout.s_Layouts.TryFindLayoutForType(typeof(TDevice));
				if (internedString.IsEmpty())
				{
					internedString = new InternedString(typeof(TDevice).Name);
				}
			}
			InputControlLayout.Collection.PrecompiledLayout precompiledLayout;
			if (!noPrecompiledLayouts && string.IsNullOrEmpty(layoutVariants) && InputControlLayout.s_Layouts.precompiledLayouts.TryGetValue(internedString, out precompiledLayout))
			{
				return (TDevice)((object)precompiledLayout.factoryMethod());
			}
			TDevice result;
			using (InputDeviceBuilder.Ref())
			{
				InputDeviceBuilder.instance.Setup(internedString, new InternedString(layoutVariants), deviceDescription);
				InputDevice inputDevice = InputDeviceBuilder.instance.Finish();
				TDevice tdevice = inputDevice as TDevice;
				if (tdevice == null)
				{
					throw new ArgumentException(string.Concat(new string[]
					{
						"Expected device of type '",
						typeof(TDevice).Name,
						"' but got device of type '",
						inputDevice.GetType().Name,
						"' instead"
					}), "TDevice");
				}
				result = tdevice;
			}
			return result;
		}

		internal unsafe void WriteChangedControlStates(byte* deviceStateBuffer, void* statePtr, uint stateSizeInBytes, uint stateOffsetInDevice)
		{
			if (this.m_ControlTreeNodes.Length == 0)
			{
				return;
			}
			this.m_UpdatedButtons.Clear();
			if (this.m_StateBlock.sizeInBits != stateSizeInBytes * 8U)
			{
				if (this.m_ControlTreeNodes[0].leftChildIndex != -1)
				{
					this.WritePartialChangedControlStatesInternal(stateSizeInBytes * 8U, stateOffsetInDevice * 8U, this.m_ControlTreeNodes[0], 0U);
					return;
				}
			}
			else if (this.m_ControlTreeNodes[0].leftChildIndex != -1)
			{
				this.WriteChangedControlStatesInternal(statePtr, deviceStateBuffer, this.m_ControlTreeNodes[0], 0U);
			}
		}

		private void WritePartialChangedControlStatesInternal(uint stateSizeInBits, uint stateOffsetInDeviceInBits, InputDevice.ControlBitRangeNode parentNode, uint startOffset)
		{
			InputDevice.ControlBitRangeNode controlBitRangeNode = this.m_ControlTreeNodes[(int)parentNode.leftChildIndex];
			if (Math.Max(stateOffsetInDeviceInBits, startOffset) <= Math.Min(stateOffsetInDeviceInBits + stateSizeInBits, (uint)controlBitRangeNode.endBitOffset))
			{
				int num = (int)(controlBitRangeNode.controlStartIndex + (ushort)controlBitRangeNode.controlCount);
				for (int i = (int)controlBitRangeNode.controlStartIndex; i < num; i++)
				{
					ushort num2 = this.m_ControlTreeIndices[i];
					InputControl inputControl = this.m_ChildrenForEachControl[(int)num2];
					inputControl.MarkAsStale();
					if (inputControl.isButton && ((ButtonControl)inputControl).needsToCheckFramePress)
					{
						this.m_UpdatedButtons.Add((int)num2);
					}
				}
				if (controlBitRangeNode.leftChildIndex != -1)
				{
					this.WritePartialChangedControlStatesInternal(stateSizeInBits, stateOffsetInDeviceInBits, controlBitRangeNode, startOffset);
				}
			}
			InputDevice.ControlBitRangeNode controlBitRangeNode2 = this.m_ControlTreeNodes[(int)(parentNode.leftChildIndex + 1)];
			if (Math.Max(stateOffsetInDeviceInBits, (uint)controlBitRangeNode.endBitOffset) <= Math.Min(stateOffsetInDeviceInBits + stateSizeInBits, (uint)controlBitRangeNode2.endBitOffset))
			{
				int num3 = (int)(controlBitRangeNode2.controlStartIndex + (ushort)controlBitRangeNode2.controlCount);
				for (int j = (int)controlBitRangeNode2.controlStartIndex; j < num3; j++)
				{
					ushort num4 = this.m_ControlTreeIndices[j];
					InputControl inputControl2 = this.m_ChildrenForEachControl[(int)num4];
					inputControl2.MarkAsStale();
					if (inputControl2.isButton && ((ButtonControl)inputControl2).needsToCheckFramePress)
					{
						this.m_UpdatedButtons.Add((int)num4);
					}
				}
				if (controlBitRangeNode2.leftChildIndex != -1)
				{
					this.WritePartialChangedControlStatesInternal(stateSizeInBits, stateOffsetInDeviceInBits, controlBitRangeNode2, (uint)controlBitRangeNode.endBitOffset);
				}
			}
		}

		private void DumpControlBitRangeNode(int nodeIndex, InputDevice.ControlBitRangeNode node, uint startOffset, uint sizeInBits, List<string> output)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < (int)node.controlCount; i++)
			{
				ushort num = this.m_ControlTreeIndices[(int)node.controlStartIndex + i];
				InputControl inputControl = this.m_ChildrenForEachControl[(int)num];
				list.Add(inputControl.path);
			}
			string text = string.Join(", ", list);
			string text2 = (node.leftChildIndex != -1) ? string.Format(" <{0}, {1}>", node.leftChildIndex, (int)(node.leftChildIndex + 1)) : "";
			output.Add(string.Format("{0} [{1}, {2}]{3}->{4}", new object[]
			{
				nodeIndex,
				startOffset,
				startOffset + sizeInBits,
				text2,
				text
			}));
		}

		private void DumpControlTree(InputDevice.ControlBitRangeNode parentNode, uint startOffset, List<string> output)
		{
			InputDevice.ControlBitRangeNode controlBitRangeNode = this.m_ControlTreeNodes[(int)parentNode.leftChildIndex];
			InputDevice.ControlBitRangeNode controlBitRangeNode2 = this.m_ControlTreeNodes[(int)(parentNode.leftChildIndex + 1)];
			this.DumpControlBitRangeNode((int)parentNode.leftChildIndex, controlBitRangeNode, startOffset, (uint)controlBitRangeNode.endBitOffset - startOffset, output);
			this.DumpControlBitRangeNode((int)(parentNode.leftChildIndex + 1), controlBitRangeNode2, (uint)controlBitRangeNode.endBitOffset, (uint)(controlBitRangeNode2.endBitOffset - controlBitRangeNode.endBitOffset), output);
			if (controlBitRangeNode.leftChildIndex != -1)
			{
				this.DumpControlTree(controlBitRangeNode, startOffset, output);
			}
			if (controlBitRangeNode2.leftChildIndex != -1)
			{
				this.DumpControlTree(controlBitRangeNode2, (uint)controlBitRangeNode.endBitOffset, output);
			}
		}

		internal string DumpControlTree()
		{
			List<string> list = new List<string>();
			this.DumpControlTree(this.m_ControlTreeNodes[0], 0U, list);
			return string.Join("\n", list);
		}

		private unsafe void WriteChangedControlStatesInternal(void* statePtr, byte* deviceStatePtr, InputDevice.ControlBitRangeNode parentNode, uint startOffset)
		{
			InputDevice.ControlBitRangeNode controlBitRangeNode = this.m_ControlTreeNodes[(int)parentNode.leftChildIndex];
			if (InputDevice.HasDataChangedInRange(deviceStatePtr, statePtr, startOffset, (uint)controlBitRangeNode.endBitOffset - startOffset + 1U))
			{
				int num = (int)(controlBitRangeNode.controlStartIndex + (ushort)controlBitRangeNode.controlCount);
				for (int i = (int)controlBitRangeNode.controlStartIndex; i < num; i++)
				{
					ushort num2 = this.m_ControlTreeIndices[i];
					InputControl inputControl = this.m_ChildrenForEachControl[(int)num2];
					if (!inputControl.CompareState((void*)(deviceStatePtr - this.m_StateBlock.byteOffset), (void*)((byte*)statePtr - this.m_StateBlock.byteOffset), null))
					{
						inputControl.MarkAsStale();
						if (inputControl.isButton && ((ButtonControl)inputControl).needsToCheckFramePress)
						{
							this.m_UpdatedButtons.Add((int)num2);
						}
					}
				}
				if (controlBitRangeNode.leftChildIndex != -1)
				{
					this.WriteChangedControlStatesInternal(statePtr, deviceStatePtr, controlBitRangeNode, startOffset);
				}
			}
			InputDevice.ControlBitRangeNode controlBitRangeNode2 = this.m_ControlTreeNodes[(int)(parentNode.leftChildIndex + 1)];
			if (!InputDevice.HasDataChangedInRange(deviceStatePtr, statePtr, (uint)controlBitRangeNode.endBitOffset, (uint)(controlBitRangeNode2.endBitOffset - controlBitRangeNode.endBitOffset + 1)))
			{
				return;
			}
			int num3 = (int)(controlBitRangeNode2.controlStartIndex + (ushort)controlBitRangeNode2.controlCount);
			for (int j = (int)controlBitRangeNode2.controlStartIndex; j < num3; j++)
			{
				ushort num4 = this.m_ControlTreeIndices[j];
				InputControl inputControl2 = this.m_ChildrenForEachControl[(int)num4];
				if (!inputControl2.CompareState((void*)(deviceStatePtr - this.m_StateBlock.byteOffset), (void*)((byte*)statePtr - this.m_StateBlock.byteOffset), null))
				{
					inputControl2.MarkAsStale();
					if (inputControl2.isButton && ((ButtonControl)inputControl2).needsToCheckFramePress)
					{
						this.m_UpdatedButtons.Add((int)num4);
					}
				}
			}
			if (controlBitRangeNode2.leftChildIndex != -1)
			{
				this.WriteChangedControlStatesInternal(statePtr, deviceStatePtr, controlBitRangeNode2, (uint)controlBitRangeNode.endBitOffset);
			}
		}

		private unsafe static bool HasDataChangedInRange(byte* deviceStatePtr, void* statePtr, uint startOffset, uint sizeInBits)
		{
			if (sizeInBits == 1U)
			{
				return MemoryHelpers.ReadSingleBit((void*)deviceStatePtr, startOffset) != MemoryHelpers.ReadSingleBit(statePtr, startOffset);
			}
			return !MemoryHelpers.MemCmpBitRegion((void*)deviceStatePtr, statePtr, startOffset, sizeInBits, null);
		}

		public const int InvalidDeviceId = 0;

		internal const int kLocalParticipantId = 0;

		internal const int kInvalidDeviceIndex = -1;

		internal InputDevice.DeviceFlags m_DeviceFlags;

		internal int m_DeviceId;

		internal int m_ParticipantId;

		internal int m_DeviceIndex;

		internal uint m_CurrentProcessedEventBytesOnUpdate;

		internal InputDeviceDescription m_Description;

		internal double m_LastUpdateTimeInternal;

		internal uint m_CurrentUpdateStepCount;

		internal InternedString[] m_AliasesForEachControl;

		internal InternedString[] m_UsagesForEachControl;

		internal InputControl[] m_UsageToControl;

		internal InputControl[] m_ChildrenForEachControl;

		internal HashSet<int> m_UpdatedButtons;

		internal List<ButtonControl> m_ButtonControlsCheckingPressState;

		internal bool m_UseCachePathForButtonPresses;

		internal uint[] m_StateOffsetToControlMap;

		internal InputDevice.ControlBitRangeNode[] m_ControlTreeNodes;

		internal ushort[] m_ControlTreeIndices;

		internal const int kControlIndexBits = 10;

		internal const int kStateOffsetBits = 13;

		internal const int kStateSizeBits = 9;

		[Flags]
		[Serializable]
		internal enum DeviceFlags
		{
			UpdateBeforeRender = 1,
			HasStateCallbacks = 2,
			HasControlsWithDefaultState = 4,
			HasDontResetControls = 1024,
			HasEventMerger = 8192,
			HasEventPreProcessor = 16384,
			Remote = 8,
			Native = 16,
			DisabledInFrontend = 32,
			DisabledInRuntime = 128,
			DisabledWhileInBackground = 256,
			DisabledStateHasBeenQueriedFromRuntime = 64,
			CanRunInBackground = 2048,
			CanRunInBackgroundHasBeenQueried = 4096
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct ControlBitRangeNode
		{
			public ControlBitRangeNode(ushort endOffset)
			{
				this.controlStartIndex = 0;
				this.controlCount = 0;
				this.endBitOffset = endOffset;
				this.leftChildIndex = -1;
			}

			public ushort endBitOffset;

			public short leftChildIndex;

			public ushort controlStartIndex;

			public byte controlCount;
		}
	}
}
