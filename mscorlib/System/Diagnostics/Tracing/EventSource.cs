using System;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	/// <summary>Provides the ability to create events for event tracing for Windows (ETW).</summary>
	public class EventSource : IDisposable
	{
		/// <summary>Creates a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class.</summary>
		protected EventSource()
		{
			this.Name = base.GetType().Name;
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class and specifies whether to throw an exception when an error occurs in the underlying Windows code.</summary>
		/// <param name="throwOnEventWriteErrors">
		///   <see langword="true" /> to throw an exception when an error occurs in the underlying Windows code; otherwise, <see langword="false" />.</param>
		protected EventSource(bool throwOnEventWriteErrors) : this()
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class with the specified configuration settings.</summary>
		/// <param name="settings">A bitwise combination of the enumeration values that specify the configuration settings to apply to the event source.</param>
		protected EventSource(EventSourceSettings settings) : this()
		{
			this.Settings = settings;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> to be used with non-contract events that contains the specified settings and traits.</summary>
		/// <param name="settings">A bitwise combination of the enumeration values that specify the configuration settings to apply to the event source.</param>
		/// <param name="traits">The key-value pairs that specify traits for the event source.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="traits" /> is not specified in key-value pairs.</exception>
		protected EventSource(EventSourceSettings settings, params string[] traits) : this(settings)
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class with the specified name.</summary>
		/// <param name="eventSourceName">The name to apply to the event source. Must not be <see langword="null" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="eventSourceName" /> is <see langword="null" />.</exception>
		public EventSource(string eventSourceName)
		{
			this.Name = eventSourceName;
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class with the specified name and settings.</summary>
		/// <param name="eventSourceName">The name to apply to the event source. Must not be <see langword="null" />.</param>
		/// <param name="config">A bitwise combination of the enumeration values that specify the configuration settings to apply to the event source.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="eventSourceName" /> is <see langword="null" />.</exception>
		public EventSource(string eventSourceName, EventSourceSettings config) : this(eventSourceName)
		{
			this.Settings = config;
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class with the specified configuration settings.</summary>
		/// <param name="eventSourceName">The name to apply to the event source. Must not be <see langword="null" />.</param>
		/// <param name="config">A bitwise combination of the enumeration values that specify the configuration settings to apply to the event source.</param>
		/// <param name="traits">The key-value pairs that specify traits for the event source.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="eventSourceName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="traits" /> is not specified in key-value pairs.</exception>
		public EventSource(string eventSourceName, EventSourceSettings config, params string[] traits) : this(eventSourceName, config)
		{
		}

		internal EventSource(Guid eventSourceGuid, string eventSourceName) : this(eventSourceName)
		{
		}

		/// <summary>Allows the <see cref="T:System.Diagnostics.Tracing.EventSource" /> object to attempt to free resources and perform other cleanup operations before the  object is reclaimed by garbage collection.</summary>
		~EventSource()
		{
			this.Dispose(false);
		}

		/// <summary>Gets any exception that was thrown during the construction of the event source.</summary>
		/// <returns>The exception that was thrown during the construction of the event source, or <see langword="null" /> if no exception was thrown.</returns>
		public Exception ConstructionException
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the activity ID of the current thread.</summary>
		/// <returns>The activity ID of the current thread.</returns>
		public static Guid CurrentThreadActivityId
		{
			get
			{
				return Guid.Empty;
			}
		}

		/// <summary>The unique identifier for the event source.</summary>
		/// <returns>A unique identifier for the event source.</returns>
		public Guid Guid
		{
			get
			{
				return Guid.Empty;
			}
		}

		/// <summary>The friendly name of the class that is derived from the event source.</summary>
		/// <returns>The friendly name of the derived class.  The default is the simple name of the class.</returns>
		public string Name { get; private set; }

		/// <summary>Gets the settings applied to this event source.</summary>
		/// <returns>The settings applied to this event source.</returns>
		public EventSourceSettings Settings { get; private set; }

		/// <summary>Determines whether the current event source is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if the current event source is enabled; otherwise, <see langword="false" />.</returns>
		public bool IsEnabled()
		{
			return false;
		}

		/// <summary>Determines whether the current event source that has the specified level and keyword is enabled.</summary>
		/// <param name="level">The level of the event source.</param>
		/// <param name="keywords">The keyword of the event source.</param>
		/// <returns>
		///   <see langword="true" /> if the event source is enabled; otherwise, <see langword="false" />.</returns>
		public bool IsEnabled(EventLevel level, EventKeywords keywords)
		{
			return false;
		}

		/// <summary>Determines whether the current event source is enabled for events with the specified level, keywords and channel.</summary>
		/// <param name="level">The event level to check. An event source will be considered enabled when its level is greater than or equal to <paramref name="level" />.</param>
		/// <param name="keywords">The event keywords to check.</param>
		/// <param name="channel">The event channel to check.</param>
		/// <returns>
		///   <see langword="true" /> if the event source is enabled for the specified event level, keywords and channel; otherwise, <see langword="false" />.  
		/// The result of this method is only an approximation of whether a particular event is active.  Use it to avoid expensive computation for logging when logging is disabled. Event sources may have additional filtering that determines their activity.</returns>
		public bool IsEnabled(EventLevel level, EventKeywords keywords, EventChannel channel)
		{
			return false;
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Gets the trait value associated with the specified key.</summary>
		/// <param name="key">The key of the trait to get.</param>
		/// <returns>The trait value associated with the specified key. If the key is not found, returns <see langword="null" />.</returns>
		public string GetTrait(string key)
		{
			return null;
		}

		/// <summary>Writes an event without fields, but with the specified name and default options.</summary>
		/// <param name="eventName">The name of the event to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="eventName" /> is <see langword="null" />.</exception>
		public void Write(string eventName)
		{
		}

		/// <summary>Writes an event without fields, but with the specified name and options.</summary>
		/// <param name="eventName">The name of the event to write.</param>
		/// <param name="options">The options such as level, keywords and operation code for the event.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="eventName" /> is <see langword="null" />.</exception>
		public void Write(string eventName, EventSourceOptions options)
		{
		}

		/// <summary>Writes an event with the specified name and data.</summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="data">The event data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventDataAttribute" /> attribute.</param>
		/// <typeparam name="T">The type that defines the event and its associated data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventSourceAttribute" /> attribute.</typeparam>
		public void Write<T>(string eventName, T data)
		{
		}

		/// <summary>Writes an event with the specified name, event data and options.</summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="options">The event options.</param>
		/// <param name="data">The event data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventDataAttribute" /> attribute.</param>
		/// <typeparam name="T">The type that defines the event and its associated data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventSourceAttribute" /> attribute.</typeparam>
		public void Write<T>(string eventName, EventSourceOptions options, T data)
		{
		}

		/// <summary>Writes an event with the specified name, options and event data.</summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="options">The event options.</param>
		/// <param name="data">The event data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventDataAttribute" /> attribute.</param>
		/// <typeparam name="T">The type that defines the event and its associated data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventSourceAttribute" /> attribute.</typeparam>
		[CLSCompliant(false)]
		public void Write<T>(string eventName, ref EventSourceOptions options, ref T data)
		{
		}

		/// <summary>Writes an event with the specified name, options, related activity and event data.</summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="options">The event options.</param>
		/// <param name="activityId">The ID of the activity associated with the event.</param>
		/// <param name="relatedActivityId">The ID of an associated activity, or <see cref="F:System.Guid.Empty" /> if there is no associated activity.</param>
		/// <param name="data">The event data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventDataAttribute" /> attribute.</param>
		/// <typeparam name="T">The type that defines the event and its associated data. This type must be an anonymous type or marked with the <see cref="T:System.Diagnostics.Tracing.EventSourceAttribute" /> attribute.</typeparam>
		public void Write<T>(string eventName, ref EventSourceOptions options, ref Guid activityId, ref Guid relatedActivityId, ref T data)
		{
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Diagnostics.Tracing.EventSource" /> class and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Called when the current event source is updated by the controller.</summary>
		/// <param name="command">The arguments for the event.</param>
		protected virtual void OnEventCommand(EventCommandEventArgs command)
		{
		}

		internal void ReportOutOfBandMessage(string msg, bool flush)
		{
		}

		/// <summary>Writes an event by using the provided event identifier.</summary>
		/// <param name="eventId">The event identifier. This value should be between 0 and 65535.</param>
		protected void WriteEvent(int eventId)
		{
			this.WriteEvent(eventId, new object[0]);
		}

		/// <summary>Writes an event by using the provided event identifier and byte array argument.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A byte array argument.</param>
		protected void WriteEvent(int eventId, byte[] arg1)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 32-bit integer argument.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">An integer argument.</param>
		protected void WriteEvent(int eventId, int arg1)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1
			});
		}

		/// <summary>Writes an event by using the provided event identifier and string argument.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A string argument.</param>
		protected void WriteEvent(int eventId, string arg1)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 32-bit integer arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">An integer argument.</param>
		/// <param name="arg2">An integer argument.</param>
		protected void WriteEvent(int eventId, int arg1, int arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 32-bit integer arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">An integer argument.</param>
		/// <param name="arg2">An integer argument.</param>
		/// <param name="arg3">An integer argument.</param>
		protected void WriteEvent(int eventId, int arg1, int arg2, int arg3)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2,
				arg3
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 32-bit integer and string arguments.</summary>
		/// <param name="eventId">The event identifier. This value should be between 0 and 65535.</param>
		/// <param name="arg1">A 32-bit integer argument.</param>
		/// <param name="arg2">A string argument.</param>
		protected void WriteEvent(int eventId, int arg1, string arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 64-bit integer argument.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A 64 bit integer argument.</param>
		protected void WriteEvent(int eventId, long arg1)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1
			});
		}

		/// <summary>Writes the event data using the specified indentifier and 64-bit integer and byte array arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A 64-bit integer argument.</param>
		/// <param name="arg2">A byte array argument.</param>
		protected void WriteEvent(int eventId, long arg1, byte[] arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 64-bit arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A 64 bit integer argument.</param>
		/// <param name="arg2">A 64 bit integer argument.</param>
		protected void WriteEvent(int eventId, long arg1, long arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 64-bit arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A 64 bit integer argument.</param>
		/// <param name="arg2">A 64 bit integer argument.</param>
		/// <param name="arg3">A 64 bit integer argument.</param>
		protected void WriteEvent(int eventId, long arg1, long arg2, long arg3)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2,
				arg3
			});
		}

		/// <summary>Writes an event by using the provided event identifier and 64-bit integer, and string arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A 64-bit integer argument.</param>
		/// <param name="arg2">A string argument.</param>
		protected void WriteEvent(int eventId, long arg1, string arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and array of arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="args">An array of objects.</param>
		protected void WriteEvent(int eventId, params object[] args)
		{
		}

		/// <summary>Writes an event by using the provided event identifier and arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A string argument.</param>
		/// <param name="arg2">A 32 bit integer argument.</param>
		protected void WriteEvent(int eventId, string arg1, int arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A string argument.</param>
		/// <param name="arg2">A 32 bit integer argument.</param>
		/// <param name="arg3">A 32 bit integer argument.</param>
		protected void WriteEvent(int eventId, string arg1, int arg2, int arg3)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2,
				arg3
			});
		}

		/// <summary>Writes an event by using the provided event identifier and arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A string argument.</param>
		/// <param name="arg2">A 64 bit integer argument.</param>
		protected void WriteEvent(int eventId, string arg1, long arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and string arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A string argument.</param>
		/// <param name="arg2">A string argument.</param>
		protected void WriteEvent(int eventId, string arg1, string arg2)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2
			});
		}

		/// <summary>Writes an event by using the provided event identifier and string arguments.</summary>
		/// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
		/// <param name="arg1">A string argument.</param>
		/// <param name="arg2">A string argument.</param>
		/// <param name="arg3">A string argument.</param>
		protected void WriteEvent(int eventId, string arg1, string arg2, string arg3)
		{
			this.WriteEvent(eventId, new object[]
			{
				arg1,
				arg2,
				arg3
			});
		}

		/// <summary>Creates a new <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overload by using the provided event identifier and event data.</summary>
		/// <param name="eventId">The event identifier.</param>
		/// <param name="eventDataCount">The number of event data items.</param>
		/// <param name="data">The structure that contains the event data.</param>
		[CLSCompliant(false)]
		protected unsafe void WriteEventCore(int eventId, int eventDataCount, EventSource.EventData* data)
		{
		}

		/// <summary>Writes an event that indicates that the current activity is related to another activity.</summary>
		/// <param name="eventId">An identifier that uniquely identifies this event within the <see cref="T:System.Diagnostics.Tracing.EventSource" />.</param>
		/// <param name="relatedActivityId">The related activity identifier.</param>
		/// <param name="args">An array of objects that contain data about the event.</param>
		protected void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, params object[] args)
		{
		}

		/// <summary>Writes an event that indicates that the current activity is related to another activity.</summary>
		/// <param name="eventId">An identifier that uniquely identifies this event within the <see cref="T:System.Diagnostics.Tracing.EventSource" />.</param>
		/// <param name="relatedActivityId">A pointer to the GUID of the related activity ID.</param>
		/// <param name="eventDataCount">The number of items in the <paramref name="data" /> field.</param>
		/// <param name="data">A pointer to the first item in the event data field.</param>
		[CLSCompliant(false)]
		protected unsafe void WriteEventWithRelatedActivityIdCore(int eventId, Guid* relatedActivityId, int eventDataCount, EventSource.EventData* data)
		{
		}

		/// <summary>Occurs when a command comes from an event listener.</summary>
		public event EventHandler<EventCommandEventArgs> EventCommandExecuted
		{
			add
			{
				throw new NotImplementedException();
			}
			remove
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Returns a string of the XML manifest that is associated with the current event source.</summary>
		/// <param name="eventSourceType">The type of the event source.</param>
		/// <param name="assemblyPathToIncludeInManifest">The path to the assembly file (.dll) to include in the provider element of the manifest.</param>
		/// <returns>The XML data string.</returns>
		public static string GenerateManifest(Type eventSourceType, string assemblyPathToIncludeInManifest)
		{
			throw new NotImplementedException();
		}

		/// <summary>Returns a string of the XML manifest that is associated with the current event source.</summary>
		/// <param name="eventSourceType">The type of the event source.</param>
		/// <param name="assemblyPathToIncludeInManifest">The path to the assembly file (.dll) file to include in the provider element of the manifest.</param>
		/// <param name="flags">A bitwise combination of the enumeration values that specify how the manifest is generated.</param>
		/// <returns>The XML data string or <see langword="null" />.</returns>
		public static string GenerateManifest(Type eventSourceType, string assemblyPathToIncludeInManifest, EventManifestOptions flags)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets the unique identifier for this implementation of the event source.</summary>
		/// <param name="eventSourceType">The type of the event source.</param>
		/// <returns>A unique identifier for this event source type.</returns>
		public static Guid GetGuid(Type eventSourceType)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets the friendly name of the event source.</summary>
		/// <param name="eventSourceType">The type of the event source.</param>
		/// <returns>The friendly name of the event source. The default is the simple name of the class.</returns>
		public static string GetName(Type eventSourceType)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets a snapshot of all the event sources for the application domain.</summary>
		/// <returns>An enumeration of all the event sources in the application domain.</returns>
		public static IEnumerable<EventSource> GetSources()
		{
			throw new NotImplementedException();
		}

		/// <summary>Sends a command to a specified event source.</summary>
		/// <param name="eventSource">The event source to send the command to.</param>
		/// <param name="command">The event command to send.</param>
		/// <param name="commandArguments">The arguments for the event command.</param>
		public static void SendCommand(EventSource eventSource, EventCommand command, IDictionary<string, string> commandArguments)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets the activity ID on the current thread.</summary>
		/// <param name="activityId">The current thread's new activity ID, or <see cref="F:System.Guid.Empty" /> to indicate that work on the current thread is not associated with any activity.</param>
		public static void SetCurrentThreadActivityId(Guid activityId)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets the activity ID on the current thread, and returns the previous activity ID.</summary>
		/// <param name="activityId">The current thread's new activity ID, or <see cref="F:System.Guid.Empty" /> to indicate that work on the current thread is not associated with any activity.</param>
		/// <param name="oldActivityThatWillContinue">When this method returns, contains the previous activity ID on the current thread.</param>
		public static void SetCurrentThreadActivityId(Guid activityId, out Guid oldActivityThatWillContinue)
		{
			throw new NotImplementedException();
		}

		/// <summary>Provides the event data for creating fast <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overloads by using the <see cref="M:System.Diagnostics.Tracing.EventSource.WriteEventCore(System.Int32,System.Int32,System.Diagnostics.Tracing.EventSource.EventData*)" /> method.</summary>
		protected internal struct EventData
		{
			/// <summary>Gets or sets the pointer to the data for the new <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overload.</summary>
			/// <returns>The pointer to the data.</returns>
			public IntPtr DataPointer { readonly get; set; }

			/// <summary>Gets or sets the number of payload items in the new <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overload.</summary>
			/// <returns>The number of payload items in the new overload.</returns>
			public int Size { readonly get; set; }

			internal int Reserved { readonly get; set; }
		}
	}
}
