using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics
{
	/// <summary>Represents a Windows NT performance counter component.</summary>
	[InstallerType(typeof(PerformanceCounterInstaller))]
	public sealed class PerformanceCounter : Component, ISupportInitialize
	{
		/// <summary>Initializes a new, read-only instance of the <see cref="T:System.Diagnostics.PerformanceCounter" /> class, without associating the instance with any system or custom performance counter.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		public PerformanceCounter()
		{
			this.categoryName = (this.counterName = (this.instanceName = ""));
			this.machineName = ".";
		}

		/// <summary>Initializes a new, read-only instance of the <see cref="T:System.Diagnostics.PerformanceCounter" /> class and associates it with the specified system or custom performance counter on the local computer. This constructor requires that the category have a single instance.</summary>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which this performance counter is associated.</param>
		/// <param name="counterName">The name of the performance counter.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="categoryName" /> is an empty string ("").  
		/// -or-  
		/// <paramref name="counterName" /> is an empty string ("").  
		/// -or-  
		/// The category specified does not exist.  
		/// -or-  
		/// The category specified is marked as multi-instance and requires the performance counter to be created with an instance name.  
		/// -or-  
		/// <paramref name="categoryName" /> and <paramref name="counterName" /> have been localized into different languages.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> or <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter(string categoryName, string counterName) : this(categoryName, counterName, false)
		{
		}

		/// <summary>Initializes a new, read-only or read/write instance of the <see cref="T:System.Diagnostics.PerformanceCounter" /> class and associates it with the specified system or custom performance counter on the local computer. This constructor requires that the category contain a single instance.</summary>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which this performance counter is associated.</param>
		/// <param name="counterName">The name of the performance counter.</param>
		/// <param name="readOnly">
		///   <see langword="true" /> to access the counter in read-only mode (although the counter itself could be read/write); <see langword="false" /> to access the counter in read/write mode.</param>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="categoryName" /> is an empty string ("").  
		///  -or-  
		///  The <paramref name="counterName" /> is an empty string ("").  
		///  -or-  
		///  The category specified does not exist. (if <paramref name="readOnly" /> is <see langword="true" />).  
		///  -or-  
		///  The category specified is not a .NET Framework custom category (if <paramref name="readOnly" /> is <see langword="false" />).  
		///  -or-  
		///  The category specified is marked as multi-instance and requires the performance counter to be created with an instance name.  
		///  -or-  
		///  <paramref name="categoryName" /> and <paramref name="counterName" /> have been localized into different languages.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> or <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter(string categoryName, string counterName, bool readOnly) : this(categoryName, counterName, "", readOnly)
		{
		}

		/// <summary>Initializes a new, read-only instance of the <see cref="T:System.Diagnostics.PerformanceCounter" /> class and associates it with the specified system or custom performance counter and category instance on the local computer.</summary>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which this performance counter is associated.</param>
		/// <param name="counterName">The name of the performance counter.</param>
		/// <param name="instanceName">The name of the performance counter category instance, or an empty string (""), if the category contains a single instance.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="categoryName" /> is an empty string ("").  
		/// -or-  
		/// <paramref name="counterName" /> is an empty string ("").  
		/// -or-  
		/// The category specified is not valid.  
		/// -or-  
		/// The category specified is marked as multi-instance and requires the performance counter to be created with an instance name.  
		/// -or-  
		/// <paramref name="instanceName" /> is longer than 127 characters.  
		/// -or-  
		/// <paramref name="categoryName" /> and <paramref name="counterName" /> have been localized into different languages.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> or <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter(string categoryName, string counterName, string instanceName) : this(categoryName, counterName, instanceName, false)
		{
		}

		/// <summary>Initializes a new, read-only or read/write instance of the <see cref="T:System.Diagnostics.PerformanceCounter" /> class and associates it with the specified system or custom performance counter and category instance on the local computer.</summary>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which this performance counter is associated.</param>
		/// <param name="counterName">The name of the performance counter.</param>
		/// <param name="instanceName">The name of the performance counter category instance, or an empty string (""), if the category contains a single instance.</param>
		/// <param name="readOnly">
		///   <see langword="true" /> to access a counter in read-only mode; <see langword="false" /> to access a counter in read/write mode.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="categoryName" /> is an empty string ("").  
		/// -or-  
		/// <paramref name="counterName" /> is an empty string ("").  
		/// -or-  
		/// The read/write permission setting requested is invalid for this counter.  
		/// -or-  
		/// The category specified does not exist (if <paramref name="readOnly" /> is <see langword="true" />).  
		/// -or-  
		/// The category specified is not a .NET Framework custom category (if <paramref name="readOnly" /> is <see langword="false" />).  
		/// -or-  
		/// The category specified is marked as multi-instance and requires the performance counter to be created with an instance name.  
		/// -or-  
		/// <paramref name="instanceName" /> is longer than 127 characters.  
		/// -or-  
		/// <paramref name="categoryName" /> and <paramref name="counterName" /> have been localized into different languages.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> or <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter(string categoryName, string counterName, string instanceName, bool readOnly)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			if (counterName == null)
			{
				throw new ArgumentNullException("counterName");
			}
			if (instanceName == null)
			{
				throw new ArgumentNullException("instanceName");
			}
			this.CategoryName = categoryName;
			this.CounterName = counterName;
			if (categoryName == "" || counterName == "")
			{
				throw new InvalidOperationException();
			}
			this.InstanceName = instanceName;
			this.instanceName = instanceName;
			this.machineName = ".";
			this.readOnly = readOnly;
			this.changed = true;
		}

		/// <summary>Initializes a new, read-only instance of the <see cref="T:System.Diagnostics.PerformanceCounter" /> class and associates it with the specified system or custom performance counter and category instance, on the specified computer.</summary>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which this performance counter is associated.</param>
		/// <param name="counterName">The name of the performance counter.</param>
		/// <param name="instanceName">The name of the performance counter category instance, or an empty string (""), if the category contains a single instance.</param>
		/// <param name="machineName">The computer on which the performance counter and its associated category exist.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="categoryName" /> is an empty string ("").  
		/// -or-  
		/// <paramref name="counterName" /> is an empty string ("").  
		/// -or-  
		/// The read/write permission setting requested is invalid for this counter.  
		/// -or-  
		/// The counter does not exist on the specified computer.  
		/// -or-  
		/// The category specified is marked as multi-instance and requires the performance counter to be created with an instance name.  
		/// -or-  
		/// <paramref name="instanceName" /> is longer than 127 characters.  
		/// -or-  
		/// <paramref name="categoryName" /> and <paramref name="counterName" /> have been localized into different languages.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="machineName" /> parameter is not valid.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> or <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter(string categoryName, string counterName, string instanceName, string machineName) : this(categoryName, counterName, instanceName, false)
		{
			this.machineName = machineName;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr GetImpl_icall(char* category, int category_length, char* counter, int counter_length, char* instance, int instance_length, out PerformanceCounterType ctype, out bool custom);

		private unsafe static IntPtr GetImpl(string category, string counter, string instance, out PerformanceCounterType ctype, out bool custom)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = counter;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr3 = instance;
			if (ptr3 != null)
			{
				ptr3 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounter.GetImpl_icall(ptr, (category != null) ? category.Length : 0, ptr2, (counter != null) ? counter.Length : 0, ptr3, (instance != null) ? instance.Length : 0, out ctype, out custom);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetSample(IntPtr impl, bool only_value, out CounterSample sample);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long UpdateValue(IntPtr impl, bool do_incr, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeData(IntPtr impl);

		private static bool IsValidMachine(string machine)
		{
			return machine == ".";
		}

		private void UpdateInfo()
		{
			if (this.impl != IntPtr.Zero)
			{
				this.Close();
			}
			if (PerformanceCounter.IsValidMachine(this.machineName))
			{
				this.impl = PerformanceCounter.GetImpl(this.categoryName, this.counterName, this.instanceName, out this.type, out this.is_custom);
			}
			if (!this.is_custom)
			{
				this.readOnly = true;
			}
			this.changed = false;
		}

		/// <summary>Gets or sets the name of the performance counter category for this performance counter.</summary>
		/// <returns>The name of the performance counter category (performance object) with which this performance counter is associated.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Diagnostics.PerformanceCounter.CategoryName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		[DefaultValue("")]
		[ReadOnly(true)]
		[SettingsBindable(true)]
		[TypeConverter("System.Diagnostics.Design.CategoryValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SRDescription("The category name for this performance counter.")]
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("categoryName");
				}
				this.categoryName = value;
				this.changed = true;
			}
		}

		/// <summary>Gets the description for this performance counter.</summary>
		/// <returns>A description of the item or quantity that this performance counter measures.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Diagnostics.PerformanceCounter" /> instance is not associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		[MonitoringDescription("A description describing the counter.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ReadOnly(true)]
		[MonoTODO]
		public string CounterHelp
		{
			get
			{
				return "";
			}
		}

		/// <summary>Gets or sets the name of the performance counter that is associated with this <see cref="T:System.Diagnostics.PerformanceCounter" /> instance.</summary>
		/// <returns>The name of the counter, which generally describes the quantity being counted. This name is displayed in the list of counters of the Performance Counter Manager MMC snap in's Add Counters dialog box.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Diagnostics.PerformanceCounter.CounterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		[SRDescription("The name of this performance counter.")]
		[DefaultValue("")]
		[ReadOnly(true)]
		[SettingsBindable(true)]
		[TypeConverter("System.Diagnostics.Design.CounterNameConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string CounterName
		{
			get
			{
				return this.counterName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("counterName");
				}
				this.counterName = value;
				this.changed = true;
			}
		}

		/// <summary>Gets the counter type of the associated performance counter.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterType" /> that describes both how the counter interacts with a monitoring application and the nature of the values it contains (for example, calculated or uncalculated).</returns>
		/// <exception cref="T:System.InvalidOperationException">The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The type of the counter.")]
		public PerformanceCounterType CounterType
		{
			get
			{
				if (this.changed)
				{
					this.UpdateInfo();
				}
				return this.type;
			}
		}

		/// <summary>Gets or sets the lifetime of a process.</summary>
		/// <returns>One of the <see cref="T:System.Diagnostics.PerformanceCounterInstanceLifetime" /> values. The default is <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Global" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value set is not a member of the <see cref="T:System.Diagnostics.PerformanceCounterInstanceLifetime" /> enumeration.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> is set after the <see cref="T:System.Diagnostics.PerformanceCounter" /> has been initialized.</exception>
		[MonoTODO]
		[DefaultValue(PerformanceCounterInstanceLifetime.Global)]
		public PerformanceCounterInstanceLifetime InstanceLifetime
		{
			get
			{
				return this.lifetime;
			}
			set
			{
				this.lifetime = value;
			}
		}

		/// <summary>Gets or sets an instance name for this performance counter.</summary>
		/// <returns>The name of the performance counter category instance, or an empty string (""), if the counter is a single-instance counter.</returns>
		[SRDescription("The instance name for this performance counter.")]
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.InstanceNameConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[ReadOnly(true)]
		[SettingsBindable(true)]
		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.instanceName = value;
				this.changed = true;
			}
		}

		/// <summary>Gets or sets the computer name for this performance counter</summary>
		/// <returns>The server on which the performance counter and its associated category reside.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Diagnostics.PerformanceCounter.MachineName" /> format is invalid.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		[DefaultValue(".")]
		[Browsable(false)]
		[SettingsBindable(true)]
		[SRDescription("The machine where this performance counter resides.")]
		[MonoTODO("What's the machine name format?")]
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == "" || value == ".")
				{
					this.machineName = ".";
					this.changed = true;
					return;
				}
				throw new PlatformNotSupportedException();
			}
		}

		/// <summary>Gets or sets the raw, or uncalculated, value of this counter.</summary>
		/// <returns>The raw value of the counter.</returns>
		/// <exception cref="T:System.InvalidOperationException">You are trying to set the counter's raw value, but the counter is read-only.  
		///  -or-  
		///  The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The raw value of the counter.")]
		public long RawValue
		{
			get
			{
				if (this.changed)
				{
					this.UpdateInfo();
				}
				CounterSample counterSample;
				PerformanceCounter.GetSample(this.impl, true, out counterSample);
				return counterSample.RawValue;
			}
			set
			{
				if (this.changed)
				{
					this.UpdateInfo();
				}
				if (this.readOnly)
				{
					throw new InvalidOperationException();
				}
				PerformanceCounter.UpdateValue(this.impl, false, value);
			}
		}

		/// <summary>Gets or sets a value indicating whether this <see cref="T:System.Diagnostics.PerformanceCounter" /> instance is in read-only mode.</summary>
		/// <returns>
		///   <see langword="true" />, if the <see cref="T:System.Diagnostics.PerformanceCounter" /> instance is in read-only mode (even if the counter itself is a custom .NET Framework counter); <see langword="false" /> if it is in read/write mode. The default is the value set by the constructor.</returns>
		[DefaultValue(true)]
		[MonitoringDescription("The accessability level of the counter.")]
		[Browsable(false)]
		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		/// <summary>Begins the initialization of a <see cref="T:System.Diagnostics.PerformanceCounter" /> instance used on a form or by another component. The initialization occurs at runtime.</summary>
		public void BeginInit()
		{
		}

		/// <summary>Ends the initialization of a <see cref="T:System.Diagnostics.PerformanceCounter" /> instance that is used on a form or by another component. The initialization occurs at runtime.</summary>
		public void EndInit()
		{
		}

		/// <summary>Closes the performance counter and frees all the resources allocated by this performance counter instance.</summary>
		public void Close()
		{
			IntPtr value = this.impl;
			this.impl = IntPtr.Zero;
			if (value != IntPtr.Zero)
			{
				PerformanceCounter.FreeData(value);
			}
		}

		/// <summary>Frees the performance counter library shared state allocated by the counters.</summary>
		public static void CloseSharedResources()
		{
		}

		/// <summary>Decrements the associated performance counter by one through an efficient atomic operation.</summary>
		/// <returns>The decremented counter value.</returns>
		/// <exception cref="T:System.InvalidOperationException">The counter is read-only, so the application cannot decrement it.  
		///  -or-  
		///  The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		public long Decrement()
		{
			return this.IncrementBy(-1L);
		}

		protected override void Dispose(bool disposing)
		{
			this.Close();
		}

		/// <summary>Increments the associated performance counter by one through an efficient atomic operation.</summary>
		/// <returns>The incremented counter value.</returns>
		/// <exception cref="T:System.InvalidOperationException">The counter is read-only, so the application cannot increment it.  
		///  -or-  
		///  The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		public long Increment()
		{
			return this.IncrementBy(1L);
		}

		/// <summary>Increments or decrements the value of the associated performance counter by a specified amount through an efficient atomic operation.</summary>
		/// <param name="value">The value to increment by. (A negative value decrements the counter.)</param>
		/// <returns>The new counter value.</returns>
		/// <exception cref="T:System.InvalidOperationException">The counter is read-only, so the application cannot increment it.  
		///  -or-  
		///  The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public long IncrementBy(long value)
		{
			if (this.changed)
			{
				this.UpdateInfo();
			}
			if (this.readOnly)
			{
				return 0L;
			}
			return PerformanceCounter.UpdateValue(this.impl, true, value);
		}

		/// <summary>Obtains a counter sample, and returns the raw, or uncalculated, value for it.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.CounterSample" /> that represents the next raw value that the system obtains for this counter.</returns>
		/// <exception cref="T:System.InvalidOperationException">The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public CounterSample NextSample()
		{
			if (this.changed)
			{
				this.UpdateInfo();
			}
			CounterSample result;
			PerformanceCounter.GetSample(this.impl, false, out result);
			this.valid_old = true;
			this.old_sample = result;
			return result;
		}

		/// <summary>Obtains a counter sample and returns the calculated value for it.</summary>
		/// <returns>The next calculated value that the system obtains for this counter.</returns>
		/// <exception cref="T:System.InvalidOperationException">The instance is not correctly associated with a performance counter.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public float NextValue()
		{
			if (this.changed)
			{
				this.UpdateInfo();
			}
			CounterSample newSample;
			PerformanceCounter.GetSample(this.impl, false, out newSample);
			float result;
			if (this.valid_old)
			{
				result = CounterSampleCalculator.ComputeCounterValue(this.old_sample, newSample);
			}
			else
			{
				result = CounterSampleCalculator.ComputeCounterValue(newSample);
			}
			this.valid_old = true;
			this.old_sample = newSample;
			return result;
		}

		/// <summary>Deletes the category instance specified by the <see cref="T:System.Diagnostics.PerformanceCounter" /> object <see cref="P:System.Diagnostics.PerformanceCounter.InstanceName" /> property.</summary>
		/// <exception cref="T:System.InvalidOperationException">This counter is read-only, so any instance that is associated with the category cannot be removed.  
		///  -or-  
		///  The instance is not correctly associated with a performance counter.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.PerformanceCounter.InstanceLifetime" /> property is set to <see cref="F:System.Diagnostics.PerformanceCounterInstanceLifetime.Process" /> when using global shared memory.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when accessing a system API.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Me), which does not support performance counters.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO]
		public void RemoveInstance()
		{
			throw new NotImplementedException();
		}

		private string categoryName;

		private string counterName;

		private string instanceName;

		private string machineName;

		private IntPtr impl;

		private PerformanceCounterType type;

		private CounterSample old_sample;

		private bool readOnly;

		private bool valid_old;

		private bool changed;

		private bool is_custom;

		private PerformanceCounterInstanceLifetime lifetime;

		/// <summary>Specifies the size, in bytes, of the global memory shared by performance counters. The default size is 524,288 bytes.</summary>
		[Obsolete]
		public static int DefaultFileMappingSize = 524288;
	}
}
