using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[DebuggerDisplay("{DebuggerDisplay(),nq}")]
	public abstract class InputControl
	{
		public string name
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
				this.RefreshConfigurationIfNeeded();
				if (this.m_DisplayName != null)
				{
					return this.m_DisplayName;
				}
				if (this.m_DisplayNameFromLayout != null)
				{
					return this.m_DisplayNameFromLayout;
				}
				return this.m_Name;
			}
			protected set
			{
				this.m_DisplayName = value;
			}
		}

		public string shortDisplayName
		{
			get
			{
				this.RefreshConfigurationIfNeeded();
				if (this.m_ShortDisplayName != null)
				{
					return this.m_ShortDisplayName;
				}
				if (this.m_ShortDisplayNameFromLayout != null)
				{
					return this.m_ShortDisplayNameFromLayout;
				}
				return null;
			}
			protected set
			{
				this.m_ShortDisplayName = value;
			}
		}

		public string path
		{
			get
			{
				if (this.m_Path == null)
				{
					this.m_Path = InputControlPath.Combine(this.m_Parent, this.m_Name);
				}
				return this.m_Path;
			}
		}

		public string layout
		{
			get
			{
				return this.m_Layout;
			}
		}

		public string variants
		{
			get
			{
				return this.m_Variants;
			}
		}

		public InputDevice device
		{
			get
			{
				return this.m_Device;
			}
		}

		public InputControl parent
		{
			get
			{
				return this.m_Parent;
			}
		}

		public ReadOnlyArray<InputControl> children
		{
			get
			{
				return new ReadOnlyArray<InputControl>(this.m_Device.m_ChildrenForEachControl, this.m_ChildStartIndex, this.m_ChildCount);
			}
		}

		public ReadOnlyArray<InternedString> usages
		{
			get
			{
				return new ReadOnlyArray<InternedString>(this.m_Device.m_UsagesForEachControl, this.m_UsageStartIndex, this.m_UsageCount);
			}
		}

		public ReadOnlyArray<InternedString> aliases
		{
			get
			{
				return new ReadOnlyArray<InternedString>(this.m_Device.m_AliasesForEachControl, this.m_AliasStartIndex, this.m_AliasCount);
			}
		}

		public InputStateBlock stateBlock
		{
			get
			{
				return this.m_StateBlock;
			}
		}

		public bool noisy
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.IsNoisy) > (InputControl.ControlFlags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.IsNoisy;
					ReadOnlyArray<InputControl> children = this.children;
					for (int i = 0; i < children.Count; i++)
					{
						if (children[i] != null)
						{
							children[i].noisy = true;
						}
					}
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.IsNoisy;
			}
		}

		public bool synthetic
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.IsSynthetic) > (InputControl.ControlFlags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.IsSynthetic;
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.IsSynthetic;
			}
		}

		public InputControl this[string path]
		{
			get
			{
				InputControl inputControl = InputControlPath.TryFindChild(this, path, 0);
				if (inputControl == null)
				{
					throw new KeyNotFoundException(string.Format("Cannot find control '{0}' as child of '{1}'", path, this));
				}
				return inputControl;
			}
		}

		public abstract Type valueType { get; }

		public abstract int valueSizeInBytes { get; }

		public float magnitude
		{
			get
			{
				return this.EvaluateMagnitude();
			}
		}

		public override string ToString()
		{
			return this.layout + ":" + this.path;
		}

		private string DebuggerDisplay()
		{
			if (!this.device.added)
			{
				return this.ToString();
			}
			string result;
			try
			{
				result = string.Format("{0}:{1}={2}", this.layout, this.path, this.ReadValueAsObject());
			}
			catch (Exception)
			{
				result = this.ToString();
			}
			return result;
		}

		public float EvaluateMagnitude()
		{
			return this.EvaluateMagnitude(this.currentStatePtr);
		}

		public unsafe virtual float EvaluateMagnitude(void* statePtr)
		{
			return -1f;
		}

		public unsafe abstract object ReadValueFromBufferAsObject(void* buffer, int bufferSize);

		public unsafe abstract object ReadValueFromStateAsObject(void* statePtr);

		public unsafe abstract void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize);

		public unsafe virtual void WriteValueFromBufferIntoState(void* bufferPtr, int bufferSize, void* statePtr)
		{
			throw new NotSupportedException(string.Format("Control '{0}' does not support writing", this));
		}

		public unsafe virtual void WriteValueFromObjectIntoState(object value, void* statePtr)
		{
			throw new NotSupportedException(string.Format("Control '{0}' does not support writing", this));
		}

		public unsafe abstract bool CompareValue(void* firstStatePtr, void* secondStatePtr);

		public InputControl TryGetChildControl(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			return InputControlPath.TryFindChild(this, path, 0);
		}

		public TControl TryGetChildControl<TControl>(string path) where TControl : InputControl
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			InputControl inputControl = this.TryGetChildControl(path);
			if (inputControl == null)
			{
				return default(TControl);
			}
			TControl tcontrol = inputControl as TControl;
			if (tcontrol == null)
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Expected control '",
					path,
					"' to be of type '",
					typeof(TControl).Name,
					"' but is of type '",
					inputControl.GetType().Name,
					"' instead!"
				}));
			}
			return tcontrol;
		}

		public InputControl GetChildControl(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			InputControl inputControl = this.TryGetChildControl(path);
			if (inputControl == null)
			{
				throw new ArgumentException("Cannot find input control '" + this.MakeChildPath(path) + "'", "path");
			}
			return inputControl;
		}

		public TControl GetChildControl<TControl>(string path) where TControl : InputControl
		{
			InputControl childControl = this.GetChildControl(path);
			TControl tcontrol = childControl as TControl;
			if (tcontrol == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Expected control '",
					path,
					"' to be of type '",
					typeof(TControl).Name,
					"' but is of type '",
					childControl.GetType().Name,
					"' instead!"
				}), "path");
			}
			return tcontrol;
		}

		protected InputControl()
		{
			this.m_StateBlock.byteOffset = 4294967294U;
		}

		protected virtual void FinishSetup()
		{
		}

		protected void RefreshConfigurationIfNeeded()
		{
			if (!this.isConfigUpToDate)
			{
				this.RefreshConfiguration();
				this.isConfigUpToDate = true;
			}
		}

		protected virtual void RefreshConfiguration()
		{
		}

		protected internal unsafe void* currentStatePtr
		{
			get
			{
				return InputStateBuffers.GetFrontBufferForDevice(this.GetDeviceIndex());
			}
		}

		protected internal unsafe void* previousFrameStatePtr
		{
			get
			{
				return InputStateBuffers.GetBackBufferForDevice(this.GetDeviceIndex());
			}
		}

		protected internal unsafe void* defaultStatePtr
		{
			get
			{
				return InputStateBuffers.s_DefaultStateBuffer;
			}
		}

		protected internal unsafe void* noiseMaskPtr
		{
			get
			{
				return InputStateBuffers.s_NoiseMaskBuffer;
			}
		}

		protected internal uint stateOffsetRelativeToDeviceRoot
		{
			get
			{
				uint byteOffset = this.device.m_StateBlock.byteOffset;
				return this.m_StateBlock.byteOffset - byteOffset;
			}
		}

		public FourCC optimizedControlDataType
		{
			get
			{
				return this.m_OptimizedControlDataType;
			}
		}

		protected virtual FourCC CalculateOptimizedControlDataType()
		{
			return 0;
		}

		public void ApplyParameterChanges()
		{
			this.SetOptimizedControlDataTypeRecursively();
			for (InputControl parent = this.parent; parent != null; parent = parent.parent)
			{
				parent.SetOptimizedControlDataType();
			}
			this.MarkAsStaleRecursively();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetOptimizedControlDataType()
		{
			this.m_OptimizedControlDataType = (InputSystem.s_Manager.optimizedControlsFeatureEnabled ? this.CalculateOptimizedControlDataType() : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void SetOptimizedControlDataTypeRecursively()
		{
			if (this.m_ChildCount > 0)
			{
				foreach (InputControl inputControl in this.children)
				{
					inputControl.SetOptimizedControlDataTypeRecursively();
				}
			}
			this.SetOptimizedControlDataType();
		}

		[Conditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void EnsureOptimizationTypeHasNotChanged()
		{
			if (!InputSystem.s_Manager.optimizedControlsFeatureEnabled)
			{
				return;
			}
			FourCC fourCC = this.CalculateOptimizedControlDataType();
			if (fourCC != this.optimizedControlDataType)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Control '",
					this.name,
					"' / '",
					this.path,
					"' suddenly changed optimization state due to either format ",
					string.Format("change or control parameters change (was '{0}' but became '{1}'), ", this.optimizedControlDataType, fourCC),
					"this hinders control hot path optimization, please call control.ApplyParameterChanges() after the changes to the control to fix this error."
				}));
				this.m_OptimizedControlDataType = fourCC;
			}
			if (this.m_ChildCount > 0)
			{
				foreach (InputControl inputControl in this.children)
				{
				}
			}
		}

		internal bool isSetupFinished
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.SetupFinished) == InputControl.ControlFlags.SetupFinished;
			}
			set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.SetupFinished;
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.SetupFinished;
			}
		}

		internal bool isButton
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.IsButton) == InputControl.ControlFlags.IsButton;
			}
			set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.IsButton;
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.IsButton;
			}
		}

		internal bool isConfigUpToDate
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.ConfigUpToDate) == InputControl.ControlFlags.ConfigUpToDate;
			}
			set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.ConfigUpToDate;
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.ConfigUpToDate;
			}
		}

		internal bool dontReset
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.DontReset) == InputControl.ControlFlags.DontReset;
			}
			set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.DontReset;
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.DontReset;
			}
		}

		internal bool usesStateFromOtherControl
		{
			get
			{
				return (this.m_ControlFlags & InputControl.ControlFlags.UsesStateFromOtherControl) == InputControl.ControlFlags.UsesStateFromOtherControl;
			}
			set
			{
				if (value)
				{
					this.m_ControlFlags |= InputControl.ControlFlags.UsesStateFromOtherControl;
					return;
				}
				this.m_ControlFlags &= ~InputControl.ControlFlags.UsesStateFromOtherControl;
			}
		}

		internal bool hasDefaultState
		{
			get
			{
				return !this.m_DefaultState.isEmpty;
			}
		}

		internal void CallFinishSetupRecursive()
		{
			ReadOnlyArray<InputControl> children = this.children;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].CallFinishSetupRecursive();
			}
			this.FinishSetup();
			this.SetOptimizedControlDataTypeRecursively();
		}

		internal string MakeChildPath(string path)
		{
			if (this is InputDevice)
			{
				return path;
			}
			return this.path + "/" + path;
		}

		internal void BakeOffsetIntoStateBlockRecursive(uint offset)
		{
			this.m_StateBlock.byteOffset = this.m_StateBlock.byteOffset + offset;
			ReadOnlyArray<InputControl> children = this.children;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].BakeOffsetIntoStateBlockRecursive(offset);
			}
		}

		internal int GetDeviceIndex()
		{
			int deviceIndex = this.m_Device.m_DeviceIndex;
			if (deviceIndex == -1)
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Cannot query value of control '",
					this.path,
					"' before '",
					this.device.name,
					"' has been added to system!"
				}));
			}
			return deviceIndex;
		}

		internal bool IsValueConsideredPressed(float value)
		{
			if (this.isButton)
			{
				return ((ButtonControl)this).IsValueConsideredPressed(value);
			}
			return value >= ButtonControl.s_GlobalDefaultButtonPressPoint;
		}

		internal virtual void AddProcessor(object first)
		{
		}

		internal void MarkAsStale()
		{
			this.m_CachedValueIsStale = true;
			this.m_UnprocessedCachedValueIsStale = true;
		}

		internal void MarkAsStaleRecursively()
		{
			this.MarkAsStale();
			foreach (InputControl inputControl in this.children)
			{
				inputControl.MarkAsStale();
				ButtonControl buttonControl = inputControl as ButtonControl;
				if (buttonControl != null)
				{
					buttonControl.UpdateWasPressed();
				}
			}
		}

		protected internal InputStateBlock m_StateBlock;

		internal InternedString m_Name;

		internal string m_Path;

		internal string m_DisplayName;

		internal string m_DisplayNameFromLayout;

		internal string m_ShortDisplayName;

		internal string m_ShortDisplayNameFromLayout;

		internal InternedString m_Layout;

		internal InternedString m_Variants;

		internal InputDevice m_Device;

		internal InputControl m_Parent;

		internal int m_UsageCount;

		internal int m_UsageStartIndex;

		internal int m_AliasCount;

		internal int m_AliasStartIndex;

		internal int m_ChildCount;

		internal int m_ChildStartIndex;

		internal InputControl.ControlFlags m_ControlFlags;

		internal bool m_CachedValueIsStale = true;

		internal bool m_UnprocessedCachedValueIsStale = true;

		internal PrimitiveValue m_DefaultState;

		internal PrimitiveValue m_MinValue;

		internal PrimitiveValue m_MaxValue;

		internal FourCC m_OptimizedControlDataType;

		[Flags]
		internal enum ControlFlags
		{
			ConfigUpToDate = 1,
			IsNoisy = 2,
			IsSynthetic = 4,
			IsButton = 8,
			DontReset = 16,
			SetupFinished = 32,
			UsesStateFromOtherControl = 64
		}
	}
}
