using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Represents a performance object, which defines a category of performance counters.</summary>
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class PerformanceCounterCategory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool CategoryDelete_icall(char* name, int name_length);

		private unsafe static bool CategoryDelete(string name)
		{
			char* ptr = name;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.CategoryDelete_icall(ptr, (name != null) ? name.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern string CategoryHelp_icall(char* category, int category_length);

		private unsafe static string CategoryHelpInternal(string category)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.CategoryHelp_icall(ptr, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool CounterCategoryExists_icall(char* counter, int counter_length, char* category, int category_length);

		private unsafe static bool CounterCategoryExists(string counter, string category)
		{
			char* ptr = counter;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = category;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.CounterCategoryExists_icall(ptr, (counter != null) ? counter.Length : 0, ptr2, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool Create_icall(char* categoryName, int categoryName_length, char* categoryHelp, int categoryHelp_length, PerformanceCounterCategoryType categoryType, CounterCreationData[] items);

		private unsafe static bool Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationData[] items)
		{
			char* ptr = categoryName;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = categoryHelp;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.Create_icall(ptr, (categoryName != null) ? categoryName.Length : 0, ptr2, (categoryHelp != null) ? categoryHelp.Length : 0, categoryType, items);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool InstanceExistsInternal_icall(char* instance, int instance_length, char* category, int category_length);

		private unsafe static bool InstanceExistsInternal(string instance, string category)
		{
			char* ptr = instance;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = category;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.InstanceExistsInternal_icall(ptr, (instance != null) ? instance.Length : 0, ptr2, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetCategoryNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern string[] GetCounterNames_icall(char* category, int category_length);

		private unsafe static string[] GetCounterNames(string category)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.GetCounterNames_icall(ptr, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern string[] GetInstanceNames_icall(char* category, int category_length);

		private unsafe static string[] GetInstanceNames(string category)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.GetInstanceNames_icall(ptr, (category != null) ? category.Length : 0);
		}

		private static void CheckCategory(string categoryName)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			if (categoryName == "")
			{
				throw new ArgumentException("categoryName");
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> class, leaves the <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property empty, and sets the <see cref="P:System.Diagnostics.PerformanceCounterCategory.MachineName" /> property to the local computer.</summary>
		public PerformanceCounterCategory() : this("", ".")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> class, sets the <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property to the specified value, and sets the <see cref="P:System.Diagnostics.PerformanceCounterCategory.MachineName" /> property to the local computer.</summary>
		/// <param name="categoryName">The name of the performance counter category, or performance object, with which to associate this <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> instance.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> is <see langword="null" />.</exception>
		public PerformanceCounterCategory(string categoryName) : this(categoryName, ".")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> class and sets the <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> and <see cref="P:System.Diagnostics.PerformanceCounterCategory.MachineName" /> properties to the specified values.</summary>
		/// <param name="categoryName">The name of the performance counter category, or performance object, with which to associate this <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> instance.</param>
		/// <param name="machineName">The computer on which the performance counter category and its associated counters exist.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> is an empty string ("").  
		///  -or-  
		///  The <paramref name="machineName" /> syntax is invalid.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> is <see langword="null" />.</exception>
		public PerformanceCounterCategory(string categoryName, string machineName)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			this.categoryName = categoryName;
			this.machineName = machineName;
		}

		private static bool IsValidMachine(string machine)
		{
			return machine == ".";
		}

		/// <summary>Gets the category's help text.</summary>
		/// <returns>A description of the performance object that this category measures.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property is <see langword="null" />. The category name must be set before getting the category help.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		public string CategoryHelp
		{
			get
			{
				string text = null;
				if (PerformanceCounterCategory.IsValidMachine(this.machineName))
				{
					text = PerformanceCounterCategory.CategoryHelpInternal(this.categoryName);
				}
				if (text != null)
				{
					return text;
				}
				throw new InvalidOperationException();
			}
		}

		/// <summary>Gets or sets the name of the performance object that defines this category.</summary>
		/// <returns>The name of the performance counter category, or performance object, with which to associate this <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> instance.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> is <see langword="null" />.</exception>
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
					throw new ArgumentNullException("value");
				}
				if (value == "")
				{
					throw new ArgumentException("value");
				}
				this.categoryName = value;
			}
		}

		/// <summary>Gets or sets the name of the computer on which this category exists.</summary>
		/// <returns>The name of the computer on which the performance counter category and its associated counters exist.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.MachineName" /> syntax is invalid.</exception>
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
				if (value == "")
				{
					throw new ArgumentException("value");
				}
				this.machineName = value;
			}
		}

		/// <summary>Gets the performance counter category type.</summary>
		/// <returns>One of the <see cref="T:System.Diagnostics.PerformanceCounterCategoryType" /> values.</returns>
		public PerformanceCounterCategoryType CategoryType
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>Determines whether the specified counter is registered to this category, which is indicated by the <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> and <see cref="P:System.Diagnostics.PerformanceCounterCategory.MachineName" /> properties.</summary>
		/// <param name="counterName">The name of the performance counter to look for.</param>
		/// <returns>
		///   <see langword="true" /> if the counter is registered to the category that is specified by the <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> and <see cref="P:System.Diagnostics.PerformanceCounterCategory.MachineName" /> properties; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property has not been set.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public bool CounterExists(string counterName)
		{
			return PerformanceCounterCategory.CounterExists(counterName, this.categoryName, this.machineName);
		}

		/// <summary>Determines whether the specified counter is registered to the specified category on the local computer.</summary>
		/// <param name="counterName">The name of the performance counter to look for.</param>
		/// <param name="categoryName">The name of the performance counter category, or performance object, with which the specified performance counter is associated.</param>
		/// <returns>
		///   <see langword="true" />, if the counter is registered to the specified category on the local computer; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> is an empty string ("").</exception>
		/// <exception cref="T:System.InvalidOperationException">The category name does not exist.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static bool CounterExists(string counterName, string categoryName)
		{
			return PerformanceCounterCategory.CounterExists(counterName, categoryName, ".");
		}

		/// <summary>Determines whether the specified counter is registered to the specified category on a remote computer.</summary>
		/// <param name="counterName">The name of the performance counter to look for.</param>
		/// <param name="categoryName">The name of the performance counter category, or performance object, with which the specified performance counter is associated.</param>
		/// <param name="machineName">The name of the computer on which the performance counter category and its associated counters exist.</param>
		/// <returns>
		///   <see langword="true" />, if the counter is registered to the specified category on the specified computer; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="counterName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> is an empty string ("").  
		///  -or-  
		///  The <paramref name="machineName" /> is invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category name does not exist.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static bool CounterExists(string counterName, string categoryName, string machineName)
		{
			if (counterName == null)
			{
				throw new ArgumentNullException("counterName");
			}
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			return PerformanceCounterCategory.IsValidMachine(machineName) && PerformanceCounterCategory.CounterCategoryExists(counterName, categoryName);
		}

		/// <summary>Registers the custom performance counter category containing the specified counters on the local computer.</summary>
		/// <param name="categoryName">The name of the custom performance counter category to create and register with the system.</param>
		/// <param name="categoryHelp">A description of the custom category.</param>
		/// <param name="counterData">A <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> that specifies the counters to create as part of the new category.</param>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> that is associated with the new custom category, or performance object.</returns>
		/// <exception cref="T:System.ArgumentException">A counter name that is specified within the <paramref name="counterData" /> collection is <see langword="null" /> or an empty string ("").  
		///  -or-  
		///  A counter that is specified within the <paramref name="counterData" /> collection already exists.  
		///  -or-  
		///  The <paramref name="counterName" /> parameter has invalid syntax. It might contain backslash characters ("\") or have length greater than 80 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category already exists on the local computer.  
		///  -or-  
		///  The layout of the <paramref name="counterData" /> collection is incorrect for base counters. A counter of type <see langword="AverageCount64" />, <see langword="AverageTimer32" />, <see langword="CounterMultiTimer" />, <see langword="CounterMultiTimerInverse" />, <see langword="CounterMultiTimer100Ns" />, <see langword="CounterMultiTimer100NsInverse" />, <see langword="RawFraction" />, <see langword="SampleFraction" /> or <see langword="SampleCounter" /> has to be immediately followed by one of the base counter types (<see langword="AverageBase" />, <see langword="MultiBase" />, <see langword="RawBase" />, or <see langword="SampleBase" />).</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		[Obsolete("Use another overload that uses PerformanceCounterCategoryType instead")]
		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, CounterCreationDataCollection counterData)
		{
			return PerformanceCounterCategory.Create(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown, counterData);
		}

		/// <summary>Registers a custom performance counter category containing a single counter of type <see langword="NumberOfItems32" /> on the local computer.</summary>
		/// <param name="categoryName">The name of the custom performance counter category to create and register with the system.</param>
		/// <param name="categoryHelp">A description of the custom category.</param>
		/// <param name="counterName">The name of a new counter, of type <see langword="NumberOfItems32" />, to create as part of the new category.</param>
		/// <param name="counterHelp">A description of the counter that is associated with the new custom category.</param>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> that is associated with the new system category, or performance object.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="counterName" /> is <see langword="null" /> or is an empty string ("").  
		/// -or-  
		/// The counter that is specified by <paramref name="counterName" /> already exists.  
		/// -or-  
		/// <paramref name="counterName" /> has invalid syntax. It might contain backslash characters ("\") or have length greater than 80 characters.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category already exists on the local computer.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="counterHelp" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		[Obsolete("Use another overload that uses PerformanceCounterCategoryType instead")]
		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, string counterName, string counterHelp)
		{
			return PerformanceCounterCategory.Create(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown, counterName, counterHelp);
		}

		/// <summary>Registers the custom performance counter category containing the specified counters on the local computer.</summary>
		/// <param name="categoryName">The name of the custom performance counter category to create and register with the system.</param>
		/// <param name="categoryHelp">A description of the custom category.</param>
		/// <param name="categoryType">One of the <see cref="T:System.Diagnostics.PerformanceCounterCategoryType" /> values.</param>
		/// <param name="counterData">A <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> that specifies the counters to create as part of the new category.</param>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> that is associated with the new custom category, or performance object.</returns>
		/// <exception cref="T:System.ArgumentException">A counter name that is specified within the <paramref name="counterData" /> collection is <see langword="null" /> or an empty string ("").  
		///  -or-  
		///  A counter that is specified within the <paramref name="counterData" /> collection already exists.  
		///  -or-  
		///  <paramref name="counterName" /> has invalid syntax. It might contain backslash characters ("\") or have length greater than 80 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="counterData" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="categoryType" /> value is outside of the range of the following values: <see langword="MultiInstance" />, <see langword="SingleInstance" />, or <see langword="Unknown" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category already exists on the local computer.  
		///  -or-  
		///  The layout of the <paramref name="counterData" /> collection is incorrect for base counters. A counter of type <see langword="AverageCount64" />, <see langword="AverageTimer32" />, <see langword="CounterMultiTimer" />, <see langword="CounterMultiTimerInverse" />, <see langword="CounterMultiTimer100Ns" />, <see langword="CounterMultiTimer100NsInverse" />, <see langword="RawFraction" />, <see langword="SampleFraction" />, or <see langword="SampleCounter" /> must be immediately followed by one of the base counter types (<see langword="AverageBase" />, <see langword="MultiBase" />, <see langword="RawBase" />, or <see langword="SampleBase" />).</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationDataCollection counterData)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (counterData == null)
			{
				throw new ArgumentNullException("counterData");
			}
			if (counterData.Count == 0)
			{
				throw new ArgumentException("counterData");
			}
			CounterCreationData[] array = new CounterCreationData[counterData.Count];
			counterData.CopyTo(array, 0);
			if (!PerformanceCounterCategory.Create(categoryName, categoryHelp, categoryType, array))
			{
				throw new InvalidOperationException();
			}
			return new PerformanceCounterCategory(categoryName, categoryHelp);
		}

		/// <summary>Registers the custom performance counter category containing a single counter of type <see cref="F:System.Diagnostics.PerformanceCounterType.NumberOfItems32" /> on the local computer.</summary>
		/// <param name="categoryName">The name of the custom performance counter category to create and register with the system.</param>
		/// <param name="categoryHelp">A description of the custom category.</param>
		/// <param name="categoryType">One of the <see cref="T:System.Diagnostics.PerformanceCounterCategoryType" /> values specifying whether the category is <see cref="F:System.Diagnostics.PerformanceCounterCategoryType.MultiInstance" />, <see cref="F:System.Diagnostics.PerformanceCounterCategoryType.SingleInstance" />, or <see cref="F:System.Diagnostics.PerformanceCounterCategoryType.Unknown" />.</param>
		/// <param name="counterName">The name of a new counter to create as part of the new category.</param>
		/// <param name="counterHelp">A description of the counter that is associated with the new custom category.</param>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> that is associated with the new system category, or performance object.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="counterName" /> is <see langword="null" /> or is an empty string ("").  
		/// -or-  
		/// The counter that is specified by <paramref name="counterName" /> already exists.  
		/// -or-  
		/// <paramref name="counterName" /> has invalid syntax. It might contain backslash characters ("\") or have length greater than 80 characters.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category already exists on the local computer.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="counterHelp" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, string counterName, string counterHelp)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (!PerformanceCounterCategory.Create(categoryName, categoryHelp, categoryType, new CounterCreationData[]
			{
				new CounterCreationData(counterName, counterHelp, PerformanceCounterType.NumberOfItems32)
			}))
			{
				throw new InvalidOperationException();
			}
			return new PerformanceCounterCategory(categoryName, categoryHelp);
		}

		/// <summary>Removes the category and its associated counters from the local computer.</summary>
		/// <param name="categoryName">The name of the custom performance counter category to delete.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> parameter has invalid syntax. It might contain backslash characters ("\") or have length greater than 80 characters.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category cannot be deleted because it is not a custom category.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static void Delete(string categoryName)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (!PerformanceCounterCategory.CategoryDelete(categoryName))
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>Determines whether the category is registered on the local computer.</summary>
		/// <param name="categoryName">The name of the performance counter category to look for.</param>
		/// <returns>
		///   <see langword="true" /> if the category is registered; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> parameter is an empty string ("").</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static bool Exists(string categoryName)
		{
			return PerformanceCounterCategory.Exists(categoryName, ".");
		}

		/// <summary>Determines whether the category is registered on the specified computer.</summary>
		/// <param name="categoryName">The name of the performance counter category to look for.</param>
		/// <param name="machineName">The name of the computer to examine for the category.</param>
		/// <returns>
		///   <see langword="true" /> if the category is registered; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="categoryName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> parameter is an empty string ("").  
		///  -or-  
		///  The <paramref name="machineName" /> parameter is invalid.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.IO.IOException">The network path cannot be found.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.  
		///  -or-  
		///  Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static bool Exists(string categoryName, string machineName)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			return PerformanceCounterCategory.IsValidMachine(machineName) && PerformanceCounterCategory.CounterCategoryExists(null, categoryName);
		}

		/// <summary>Retrieves a list of the performance counter categories that are registered on the local computer.</summary>
		/// <returns>An array of <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> objects indicating the categories that are registered on the local computer.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static PerformanceCounterCategory[] GetCategories()
		{
			return PerformanceCounterCategory.GetCategories(".");
		}

		/// <summary>Retrieves a list of the performance counter categories that are registered on the specified computer.</summary>
		/// <param name="machineName">The computer to look on.</param>
		/// <returns>An array of <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> objects indicating the categories that are registered on the specified computer.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="machineName" /> parameter is invalid.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static PerformanceCounterCategory[] GetCategories(string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!PerformanceCounterCategory.IsValidMachine(machineName))
			{
				return Array.Empty<PerformanceCounterCategory>();
			}
			string[] categoryNames = PerformanceCounterCategory.GetCategoryNames();
			PerformanceCounterCategory[] array = new PerformanceCounterCategory[categoryNames.Length];
			for (int i = 0; i < categoryNames.Length; i++)
			{
				array[i] = new PerformanceCounterCategory(categoryNames[i], machineName);
			}
			return array;
		}

		/// <summary>Retrieves a list of the counters in a performance counter category that contains exactly one instance.</summary>
		/// <returns>An array of <see cref="T:System.Diagnostics.PerformanceCounter" /> objects indicating the counters that are associated with this single-instance performance counter category.</returns>
		/// <exception cref="T:System.ArgumentException">The category is not a single instance.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The category does not have an associated instance.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter[] GetCounters()
		{
			return this.GetCounters("");
		}

		/// <summary>Retrieves a list of the counters in a performance counter category that contains one or more instances.</summary>
		/// <param name="instanceName">The performance object instance for which to retrieve the list of associated counters.</param>
		/// <returns>An array of <see cref="T:System.Diagnostics.PerformanceCounter" /> objects indicating the counters that are associated with the specified object instance of this performance counter category.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="instanceName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property for this <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> instance has not been set.  
		///  -or-  
		///  The category does not contain the instance that is specified by the <paramref name="instanceName" /> parameter.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public PerformanceCounter[] GetCounters(string instanceName)
		{
			if (!PerformanceCounterCategory.IsValidMachine(this.machineName))
			{
				return Array.Empty<PerformanceCounter>();
			}
			string[] counterNames = PerformanceCounterCategory.GetCounterNames(this.categoryName);
			PerformanceCounter[] array = new PerformanceCounter[counterNames.Length];
			for (int i = 0; i < counterNames.Length; i++)
			{
				array[i] = new PerformanceCounter(this.categoryName, counterNames[i], instanceName, this.machineName);
			}
			return array;
		}

		/// <summary>Retrieves the list of performance object instances that are associated with this category.</summary>
		/// <returns>An array of strings representing the performance object instance names that are associated with this category or, if the category contains only one performance object instance, a single-entry array that contains an empty string ("").</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property is <see langword="null" />. The property might not have been set.  
		///  -or-  
		///  The category does not have an associated instance.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public string[] GetInstanceNames()
		{
			if (!PerformanceCounterCategory.IsValidMachine(this.machineName))
			{
				return Array.Empty<string>();
			}
			return PerformanceCounterCategory.GetInstanceNames(this.categoryName);
		}

		/// <summary>Determines whether the specified performance object instance exists in the category that is identified by this <see cref="T:System.Diagnostics.PerformanceCounterCategory" /> object's <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property.</summary>
		/// <param name="instanceName">The performance object instance in this performance counter category to search for.</param>
		/// <returns>
		///   <see langword="true" /> if the category contains the specified performance object instance; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property is <see langword="null" />. The property might not have been set.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="instanceName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public bool InstanceExists(string instanceName)
		{
			return PerformanceCounterCategory.InstanceExists(instanceName, this.categoryName, this.machineName);
		}

		/// <summary>Determines whether a specified category on the local computer contains the specified performance object instance.</summary>
		/// <param name="instanceName">The performance object instance to search for.</param>
		/// <param name="categoryName">The performance counter category to search.</param>
		/// <returns>
		///   <see langword="true" /> if the category contains the specified performance object instance; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="instanceName" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="categoryName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> parameter is an empty string ("").</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static bool InstanceExists(string instanceName, string categoryName)
		{
			return PerformanceCounterCategory.InstanceExists(instanceName, categoryName, ".");
		}

		/// <summary>Determines whether a specified category on a specified computer contains the specified performance object instance.</summary>
		/// <param name="instanceName">The performance object instance to search for.</param>
		/// <param name="categoryName">The performance counter category to search.</param>
		/// <param name="machineName">The name of the computer on which to look for the category instance pair.</param>
		/// <returns>
		///   <see langword="true" /> if the category contains the specified performance object instance; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="instanceName" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="categoryName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="categoryName" /> parameter is an empty string ("").  
		///  -or-  
		///  The <paramref name="machineName" /> parameter is invalid.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		public static bool InstanceExists(string instanceName, string categoryName, string machineName)
		{
			if (instanceName == null)
			{
				throw new ArgumentNullException("instanceName");
			}
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			return PerformanceCounterCategory.InstanceExistsInternal(instanceName, categoryName);
		}

		/// <summary>Reads all the counter and performance object instance data that is associated with this performance counter category.</summary>
		/// <returns>An <see cref="T:System.Diagnostics.InstanceDataCollectionCollection" /> that contains the counter and performance object instance data for the category.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.PerformanceCounterCategory.CategoryName" /> property is <see langword="null" />. The property might not have been set.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A call to an underlying system API failed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">Code that is executing without administrative privileges attempted to read a performance counter.</exception>
		[MonoTODO]
		public InstanceDataCollectionCollection ReadCategory()
		{
			throw new NotImplementedException();
		}

		private string categoryName;

		private string machineName;

		private PerformanceCounterCategoryType type = PerformanceCounterCategoryType.Unknown;
	}
}
