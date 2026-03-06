using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Defines the smallest unit of a code access security permission that is set for a <see cref="T:System.Diagnostics.PerformanceCounter" />.</summary>
	[Serializable]
	public class PerformanceCounterPermissionEntry
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterPermissionEntry" /> class.</summary>
		/// <param name="permissionAccess">A bitwise combination of the <see cref="T:System.Diagnostics.PerformanceCounterPermissionAccess" /> values. The <see cref="P:System.Diagnostics.PerformanceCounterPermissionEntry.PermissionAccess" /> property is set to this value.</param>
		/// <param name="machineName">The server on which the category of the performance counter resides.</param>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which this performance counter is associated.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="categoryName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="machineName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="permissionAccess" /> is not a valid <see cref="T:System.Diagnostics.PerformanceCounterPermissionAccess" /> value.  
		/// -or-  
		/// <paramref name="machineName" /> is not a valid computer name.</exception>
		public PerformanceCounterPermissionEntry(PerformanceCounterPermissionAccess permissionAccess, string machineName, string categoryName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if ((permissionAccess | PerformanceCounterPermissionAccess.Administer) != PerformanceCounterPermissionAccess.Administer)
			{
				throw new ArgumentException("permissionAccess");
			}
			ResourcePermissionBase.ValidateMachineName(machineName);
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			this.permissionAccess = permissionAccess;
			this.machineName = machineName;
			this.categoryName = categoryName;
		}

		/// <summary>Gets the name of the performance counter category (performance object).</summary>
		/// <returns>The name of the performance counter category (performance object).</returns>
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
		}

		/// <summary>Gets the name of the server on which the category of the performance counter resides.</summary>
		/// <returns>The name of the server on which the category resides.</returns>
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		/// <summary>Gets the permission access level of the entry.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Diagnostics.PerformanceCounterPermissionAccess" /> values.</returns>
		public PerformanceCounterPermissionAccess PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
		}

		internal ResourcePermissionBaseEntry CreateResourcePermissionBaseEntry()
		{
			return new ResourcePermissionBaseEntry((int)this.permissionAccess, new string[]
			{
				this.machineName,
				this.categoryName
			});
		}

		private const PerformanceCounterPermissionAccess All = PerformanceCounterPermissionAccess.Administer;

		private PerformanceCounterPermissionAccess permissionAccess;

		private string machineName;

		private string categoryName;
	}
}
