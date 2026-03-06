using System;
using System.Collections;

namespace System.ComponentModel.Design
{
	/// <summary>Provides a service for registering event handlers for component events.</summary>
	public interface IEventBindingService
	{
		/// <summary>Creates a unique name for an event-handler method for the specified component and event.</summary>
		/// <param name="component">The component instance the event is connected to.</param>
		/// <param name="e">The event to create a name for.</param>
		/// <returns>The recommended name for the event-handler method for this event.</returns>
		string CreateUniqueMethodName(IComponent component, EventDescriptor e);

		/// <summary>Gets a collection of event-handler methods that have a method signature compatible with the specified event.</summary>
		/// <param name="e">The event to get the compatible event-handler methods for.</param>
		/// <returns>A collection of strings.</returns>
		ICollection GetCompatibleMethods(EventDescriptor e);

		/// <summary>Gets an <see cref="T:System.ComponentModel.EventDescriptor" /> for the event that the specified property descriptor represents, if it represents an event.</summary>
		/// <param name="property">The property that represents an event.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> for the event that the property represents, or <see langword="null" /> if the property does not represent an event.</returns>
		EventDescriptor GetEvent(PropertyDescriptor property);

		/// <summary>Converts a set of event descriptors to a set of property descriptors.</summary>
		/// <param name="events">The events to convert to properties.</param>
		/// <returns>An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects that describe the event set.</returns>
		PropertyDescriptorCollection GetEventProperties(EventDescriptorCollection events);

		/// <summary>Converts a single event descriptor to a property descriptor.</summary>
		/// <param name="e">The event to convert.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that describes the event.</returns>
		PropertyDescriptor GetEventProperty(EventDescriptor e);

		/// <summary>Displays the user code for the designer.</summary>
		/// <returns>
		///   <see langword="true" /> if the code is displayed; otherwise, <see langword="false" />.</returns>
		bool ShowCode();

		/// <summary>Displays the user code for the designer at the specified line.</summary>
		/// <param name="lineNumber">The line number to place the caret on.</param>
		/// <returns>
		///   <see langword="true" /> if the code is displayed; otherwise, <see langword="false" />.</returns>
		bool ShowCode(int lineNumber);

		/// <summary>Displays the user code for the specified event.</summary>
		/// <param name="component">The component that the event is connected to.</param>
		/// <param name="e">The event to display.</param>
		/// <returns>
		///   <see langword="true" /> if the code is displayed; otherwise, <see langword="false" />.</returns>
		bool ShowCode(IComponent component, EventDescriptor e);
	}
}
