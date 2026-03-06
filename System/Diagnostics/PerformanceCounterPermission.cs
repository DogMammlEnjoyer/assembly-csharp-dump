using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Allows control of code access permissions for <see cref="T:System.Diagnostics.PerformanceCounter" />.</summary>
	[Serializable]
	public sealed class PerformanceCounterPermission : ResourcePermissionBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterPermission" /> class.</summary>
		public PerformanceCounterPermission()
		{
			this.SetUp();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterPermission" /> class with the specified permission access level entries.</summary>
		/// <param name="permissionAccessEntries">An array of <see cref="T:System.Diagnostics.PerformanceCounterPermissionEntry" /> objects. The <see cref="P:System.Diagnostics.PerformanceCounterPermission.PermissionEntries" /> property is set to this value.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="permissionAccessEntries" /> is <see langword="null" />.</exception>
		public PerformanceCounterPermission(PerformanceCounterPermissionEntry[] permissionAccessEntries)
		{
			if (permissionAccessEntries == null)
			{
				throw new ArgumentNullException("permissionAccessEntries");
			}
			this.SetUp();
			this.innerCollection = new PerformanceCounterPermissionEntryCollection(this);
			this.innerCollection.AddRange(permissionAccessEntries);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterPermission" /> class with the specified permission state.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="state" /> parameter is not a valid value of <see cref="T:System.Security.Permissions.PermissionState" />.</exception>
		public PerformanceCounterPermission(PermissionState state) : base(state)
		{
			this.SetUp();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterPermission" /> class with the specified access levels, the name of the computer to use, and the category associated with the performance counter.</summary>
		/// <param name="permissionAccess">One of the <see cref="T:System.Diagnostics.PerformanceCounterPermissionAccess" /> values.</param>
		/// <param name="machineName">The server on which the performance counter and its associate category reside.</param>
		/// <param name="categoryName">The name of the performance counter category (performance object) with which the performance counter is associated.</param>
		public PerformanceCounterPermission(PerformanceCounterPermissionAccess permissionAccess, string machineName, string categoryName)
		{
			this.SetUp();
			this.innerCollection = new PerformanceCounterPermissionEntryCollection(this);
			this.innerCollection.Add(new PerformanceCounterPermissionEntry(permissionAccess, machineName, categoryName));
		}

		/// <summary>Gets the collection of permission entries for this permissions request.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterPermissionEntryCollection" /> that contains the permission entries for this permissions request.</returns>
		public PerformanceCounterPermissionEntryCollection PermissionEntries
		{
			get
			{
				if (this.innerCollection == null)
				{
					this.innerCollection = new PerformanceCounterPermissionEntryCollection(this);
				}
				return this.innerCollection;
			}
		}

		private void SetUp()
		{
			base.TagNames = new string[]
			{
				"Machine",
				"Category"
			};
			base.PermissionAccessType = typeof(PerformanceCounterPermissionAccess);
		}

		internal ResourcePermissionBaseEntry[] GetEntries()
		{
			return base.GetPermissionEntries();
		}

		internal void ClearEntries()
		{
			base.Clear();
		}

		internal void Add(object obj)
		{
			PerformanceCounterPermissionEntry performanceCounterPermissionEntry = obj as PerformanceCounterPermissionEntry;
			base.AddPermissionAccess(performanceCounterPermissionEntry.CreateResourcePermissionBaseEntry());
		}

		internal void Remove(object obj)
		{
			PerformanceCounterPermissionEntry performanceCounterPermissionEntry = obj as PerformanceCounterPermissionEntry;
			base.RemovePermissionAccess(performanceCounterPermissionEntry.CreateResourcePermissionBaseEntry());
		}

		private PerformanceCounterPermissionEntryCollection innerCollection;
	}
}
