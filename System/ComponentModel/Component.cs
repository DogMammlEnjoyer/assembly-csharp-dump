using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	/// <summary>Provides the base implementation for the <see cref="T:System.ComponentModel.IComponent" /> interface and enables object sharing between applications.</summary>
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[DesignerCategory("Component")]
	public class Component : MarshalByRefObject, IComponent, IDisposable
	{
		/// <summary>Releases unmanaged resources and performs other cleanup operations before the <see cref="T:System.ComponentModel.Component" /> is reclaimed by garbage collection.</summary>
		~Component()
		{
			this.Dispose(false);
		}

		/// <summary>Gets a value indicating whether the component can raise an event.</summary>
		/// <returns>
		///   <see langword="true" /> if the component can raise events; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		protected virtual bool CanRaiseEvents
		{
			get
			{
				return true;
			}
		}

		internal bool CanRaiseEventsInternal
		{
			get
			{
				return this.CanRaiseEvents;
			}
		}

		/// <summary>Occurs when the component is disposed by a call to the <see cref="M:System.ComponentModel.Component.Dispose" /> method.</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event EventHandler Disposed
		{
			add
			{
				this.Events.AddHandler(Component.EventDisposed, value);
			}
			remove
			{
				this.Events.RemoveHandler(Component.EventDisposed, value);
			}
		}

		/// <summary>Gets the list of event handlers that are attached to this <see cref="T:System.ComponentModel.Component" />.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.EventHandlerList" /> that provides the delegates for this component.</returns>
		protected EventHandlerList Events
		{
			get
			{
				if (this.events == null)
				{
					this.events = new EventHandlerList(this);
				}
				return this.events;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.ComponentModel.ISite" /> of the <see cref="T:System.ComponentModel.Component" />.</summary>
		/// <returns>The <see cref="T:System.ComponentModel.ISite" /> associated with the <see cref="T:System.ComponentModel.Component" />, or <see langword="null" /> if the <see cref="T:System.ComponentModel.Component" /> is not encapsulated in an <see cref="T:System.ComponentModel.IContainer" />, the <see cref="T:System.ComponentModel.Component" /> does not have an <see cref="T:System.ComponentModel.ISite" /> associated with it, or the <see cref="T:System.ComponentModel.Component" /> is removed from its <see cref="T:System.ComponentModel.IContainer" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual ISite Site
		{
			get
			{
				return this.site;
			}
			set
			{
				this.site = value;
			}
		}

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Component" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					if (this.site != null && this.site.Container != null)
					{
						this.site.Container.Remove(this);
					}
					if (this.events != null)
					{
						EventHandler eventHandler = (EventHandler)this.events[Component.EventDisposed];
						if (eventHandler != null)
						{
							eventHandler(this, EventArgs.Empty);
						}
					}
				}
			}
		}

		/// <summary>Gets the <see cref="T:System.ComponentModel.IContainer" /> that contains the <see cref="T:System.ComponentModel.Component" />.</summary>
		/// <returns>The <see cref="T:System.ComponentModel.IContainer" /> that contains the <see cref="T:System.ComponentModel.Component" />, if any, or <see langword="null" /> if the <see cref="T:System.ComponentModel.Component" /> is not encapsulated in an <see cref="T:System.ComponentModel.IContainer" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IContainer Container
		{
			get
			{
				ISite site = this.site;
				if (site != null)
				{
					return site.Container;
				}
				return null;
			}
		}

		/// <summary>Returns an object that represents a service provided by the <see cref="T:System.ComponentModel.Component" /> or by its <see cref="T:System.ComponentModel.Container" />.</summary>
		/// <param name="service">A service provided by the <see cref="T:System.ComponentModel.Component" />.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents a service provided by the <see cref="T:System.ComponentModel.Component" />, or <see langword="null" /> if the <see cref="T:System.ComponentModel.Component" /> does not provide the specified service.</returns>
		protected virtual object GetService(Type service)
		{
			ISite site = this.site;
			if (site != null)
			{
				return site.GetService(service);
			}
			return null;
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.ComponentModel.Component" /> is currently in design mode.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.Component" /> is in design mode; otherwise, <see langword="false" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		protected bool DesignMode
		{
			get
			{
				ISite site = this.site;
				return site != null && site.DesignMode;
			}
		}

		/// <summary>Returns a <see cref="T:System.String" /> containing the name of the <see cref="T:System.ComponentModel.Component" />, if any. This method should not be overridden.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the name of the <see cref="T:System.ComponentModel.Component" />, if any, or <see langword="null" /> if the <see cref="T:System.ComponentModel.Component" /> is unnamed.</returns>
		public override string ToString()
		{
			ISite site = this.site;
			if (site != null)
			{
				return site.Name + " [" + base.GetType().FullName + "]";
			}
			return base.GetType().FullName;
		}

		private static readonly object EventDisposed = new object();

		private ISite site;

		private EventHandlerList events;
	}
}
