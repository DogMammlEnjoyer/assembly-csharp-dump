using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	/// <summary>Encapsulates zero or more components.</summary>
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class Container : IContainer, IDisposable
	{
		/// <summary>Releases unmanaged resources and performs other cleanup operations before the <see cref="T:System.ComponentModel.Container" /> is reclaimed by garbage collection.</summary>
		~Container()
		{
			this.Dispose(false);
		}

		/// <summary>Adds the specified <see cref="T:System.ComponentModel.Component" /> to the <see cref="T:System.ComponentModel.Container" />. The component is unnamed.</summary>
		/// <param name="component">The component to add.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		public virtual void Add(IComponent component)
		{
			this.Add(component, null);
		}

		/// <summary>Adds the specified <see cref="T:System.ComponentModel.Component" /> to the <see cref="T:System.ComponentModel.Container" /> and assigns it a name.</summary>
		/// <param name="component">The component to add.</param>
		/// <param name="name">The unique, case-insensitive name to assign to the component.  
		///  -or-  
		///  <see langword="null" />, which leaves the component unnamed.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is not unique.</exception>
		public virtual void Add(IComponent component, string name)
		{
			object obj = this.syncObj;
			lock (obj)
			{
				if (component != null)
				{
					ISite site = component.Site;
					if (site == null || site.Container != this)
					{
						if (this.sites == null)
						{
							this.sites = new ISite[4];
						}
						else
						{
							this.ValidateName(component, name);
							if (this.sites.Length == this.siteCount)
							{
								ISite[] destinationArray = new ISite[this.siteCount * 2];
								Array.Copy(this.sites, 0, destinationArray, 0, this.siteCount);
								this.sites = destinationArray;
							}
						}
						if (site != null)
						{
							site.Container.Remove(component);
						}
						ISite site2 = this.CreateSite(component, name);
						ISite[] array = this.sites;
						int num = this.siteCount;
						this.siteCount = num + 1;
						array[num] = site2;
						component.Site = site2;
						this.components = null;
					}
				}
			}
		}

		/// <summary>Creates a site <see cref="T:System.ComponentModel.ISite" /> for the given <see cref="T:System.ComponentModel.IComponent" /> and assigns the given name to the site.</summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.IComponent" /> to create a site for.</param>
		/// <param name="name">The name to assign to <paramref name="component" />, or <see langword="null" /> to skip the name assignment.</param>
		/// <returns>The newly created site.</returns>
		protected virtual ISite CreateSite(IComponent component, string name)
		{
			return new Container.Site(component, this, name);
		}

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Container" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Container" />, and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				object obj = this.syncObj;
				lock (obj)
				{
					while (this.siteCount > 0)
					{
						ISite[] array = this.sites;
						int num = this.siteCount - 1;
						this.siteCount = num;
						object obj2 = array[num];
						((ISite)obj2).Component.Site = null;
						((ISite)obj2).Component.Dispose();
					}
					this.sites = null;
					this.components = null;
				}
			}
		}

		/// <summary>Gets the service object of the specified type, if it is available.</summary>
		/// <param name="service">The <see cref="T:System.Type" /> of the service to retrieve.</param>
		/// <returns>An <see cref="T:System.Object" /> implementing the requested service, or <see langword="null" /> if the service cannot be resolved.</returns>
		protected virtual object GetService(Type service)
		{
			if (!(service == typeof(IContainer)))
			{
				return null;
			}
			return this;
		}

		/// <summary>Gets all the components in the <see cref="T:System.ComponentModel.Container" />.</summary>
		/// <returns>A collection that contains the components in the <see cref="T:System.ComponentModel.Container" />.</returns>
		public virtual ComponentCollection Components
		{
			get
			{
				object obj = this.syncObj;
				ComponentCollection result;
				lock (obj)
				{
					if (this.components == null)
					{
						IComponent[] array = new IComponent[this.siteCount];
						for (int i = 0; i < this.siteCount; i++)
						{
							array[i] = this.sites[i].Component;
						}
						this.components = new ComponentCollection(array);
						if (this.filter == null && this.checkedFilter)
						{
							this.checkedFilter = false;
						}
					}
					if (!this.checkedFilter)
					{
						this.filter = (this.GetService(typeof(ContainerFilterService)) as ContainerFilterService);
						this.checkedFilter = true;
					}
					if (this.filter != null)
					{
						ComponentCollection componentCollection = this.filter.FilterComponents(this.components);
						if (componentCollection != null)
						{
							this.components = componentCollection;
						}
					}
					result = this.components;
				}
				return result;
			}
		}

		/// <summary>Removes a component from the <see cref="T:System.ComponentModel.Container" />.</summary>
		/// <param name="component">The component to remove.</param>
		public virtual void Remove(IComponent component)
		{
			this.Remove(component, false);
		}

		private void Remove(IComponent component, bool preserveSite)
		{
			object obj = this.syncObj;
			lock (obj)
			{
				if (component != null)
				{
					ISite site = component.Site;
					if (site != null && site.Container == this)
					{
						if (!preserveSite)
						{
							component.Site = null;
						}
						for (int i = 0; i < this.siteCount; i++)
						{
							if (this.sites[i] == site)
							{
								this.siteCount--;
								Array.Copy(this.sites, i + 1, this.sites, i, this.siteCount - i);
								this.sites[this.siteCount] = null;
								this.components = null;
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>Removes a component from the <see cref="T:System.ComponentModel.Container" /> without setting <see cref="P:System.ComponentModel.IComponent.Site" /> to <see langword="null" />.</summary>
		/// <param name="component">The component to remove.</param>
		protected void RemoveWithoutUnsiting(IComponent component)
		{
			this.Remove(component, true);
		}

		/// <summary>Determines whether the component name is unique for this container.</summary>
		/// <param name="component">The named component.</param>
		/// <param name="name">The component name to validate.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is not unique.</exception>
		protected virtual void ValidateName(IComponent component, string name)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (name != null)
			{
				for (int i = 0; i < Math.Min(this.siteCount, this.sites.Length); i++)
				{
					ISite site = this.sites[i];
					if (site != null && site.Name != null && string.Equals(site.Name, name, StringComparison.OrdinalIgnoreCase) && site.Component != component && ((InheritanceAttribute)TypeDescriptor.GetAttributes(site.Component)[typeof(InheritanceAttribute)]).InheritanceLevel != InheritanceLevel.InheritedReadOnly)
					{
						throw new ArgumentException(SR.GetString("Duplicate component name '{0}'.  Component names must be unique and case-insensitive.", new object[]
						{
							name
						}));
					}
				}
			}
		}

		private ISite[] sites;

		private int siteCount;

		private ComponentCollection components;

		private ContainerFilterService filter;

		private bool checkedFilter;

		private object syncObj = new object();

		private class Site : ISite, IServiceProvider
		{
			internal Site(IComponent component, Container container, string name)
			{
				this.component = component;
				this.container = container;
				this.name = name;
			}

			public IComponent Component
			{
				get
				{
					return this.component;
				}
			}

			public IContainer Container
			{
				get
				{
					return this.container;
				}
			}

			public object GetService(Type service)
			{
				if (!(service == typeof(ISite)))
				{
					return this.container.GetService(service);
				}
				return this;
			}

			public bool DesignMode
			{
				get
				{
					return false;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					if (value == null || this.name == null || !value.Equals(this.name))
					{
						this.container.ValidateName(this.component, value);
						this.name = value;
					}
				}
			}

			private IComponent component;

			private Container container;

			private string name;
		}
	}
}
