using System;
using System.ComponentModel.Design;

namespace System.ComponentModel
{
	/// <summary>Implements <see cref="T:System.ComponentModel.IComponent" /> and provides the base implementation for remotable components that are marshaled by value (a copy of the serialized object is passed).</summary>
	[DesignerCategory("Component")]
	[Designer("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner))]
	[TypeConverter(typeof(ComponentConverter))]
	public class MarshalByValueComponent : IComponent, IDisposable, IServiceProvider
	{
		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~MarshalByValueComponent()
		{
			this.Dispose(false);
		}

		/// <summary>Adds an event handler to listen to the <see cref="E:System.ComponentModel.MarshalByValueComponent.Disposed" /> event on the component.</summary>
		public event EventHandler Disposed
		{
			add
			{
				this.Events.AddHandler(MarshalByValueComponent.s_eventDisposed, value);
			}
			remove
			{
				this.Events.RemoveHandler(MarshalByValueComponent.s_eventDisposed, value);
			}
		}

		/// <summary>Gets the list of event handlers that are attached to this component.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.EventHandlerList" /> that provides the delegates for this component.</returns>
		protected EventHandlerList Events
		{
			get
			{
				EventHandlerList result;
				if ((result = this._events) == null)
				{
					result = (this._events = new EventHandlerList());
				}
				return result;
			}
		}

		/// <summary>Gets or sets the site of the component.</summary>
		/// <returns>An object implementing the <see cref="T:System.ComponentModel.ISite" /> interface that represents the site of the component.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual ISite Site
		{
			get
			{
				return this._site;
			}
			set
			{
				this._site = value;
			}
		}

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.MarshalByValueComponent" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.MarshalByValueComponent" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					ISite site = this._site;
					if (site != null)
					{
						IContainer container = site.Container;
						if (container != null)
						{
							container.Remove(this);
						}
					}
					EventHandlerList events = this._events;
					EventHandler eventHandler = (EventHandler)((events != null) ? events[MarshalByValueComponent.s_eventDisposed] : null);
					if (eventHandler != null)
					{
						eventHandler(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>Gets the container for the component.</summary>
		/// <returns>An object implementing the <see cref="T:System.ComponentModel.IContainer" /> interface that represents the component's container, or <see langword="null" /> if the component does not have a site.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IContainer Container
		{
			get
			{
				ISite site = this._site;
				if (site == null)
				{
					return null;
				}
				return site.Container;
			}
		}

		/// <summary>Gets the implementer of the <see cref="T:System.IServiceProvider" />.</summary>
		/// <param name="service">A <see cref="T:System.Type" /> that represents the type of service you want.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents the implementer of the <see cref="T:System.IServiceProvider" />.</returns>
		public virtual object GetService(Type service)
		{
			ISite site = this._site;
			if (site == null)
			{
				return null;
			}
			return site.GetService(service);
		}

		/// <summary>Gets a value indicating whether the component is currently in design mode.</summary>
		/// <returns>
		///   <see langword="true" /> if the component is in design mode; otherwise, <see langword="false" />.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool DesignMode
		{
			get
			{
				ISite site = this._site;
				return site != null && site.DesignMode;
			}
		}

		/// <summary>Returns a <see cref="T:System.String" /> containing the name of the <see cref="T:System.ComponentModel.Component" />, if any. This method should not be overridden.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the name of the <see cref="T:System.ComponentModel.Component" />, if any.  
		///  <see langword="null" /> if the <see cref="T:System.ComponentModel.Component" /> is unnamed.</returns>
		public override string ToString()
		{
			ISite site = this._site;
			if (site != null)
			{
				return site.Name + " [" + base.GetType().FullName + "]";
			}
			return base.GetType().FullName;
		}

		private static readonly object s_eventDisposed = new object();

		private ISite _site;

		private EventHandlerList _events;
	}
}
