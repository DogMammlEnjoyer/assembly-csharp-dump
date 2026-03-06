using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Provides a set of methods and properties that enable applications to trace the execution of code and associate trace messages with their source.</summary>
	public class TraceSource
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.TraceSource" /> class, using the specified name for the source.</summary>
		/// <param name="name">The name of the source (typically, the name of the application).</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string ("").</exception>
		public TraceSource(string name) : this(name, SourceLevels.Off)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.TraceSource" /> class, using the specified name for the source and the default source level at which tracing is to occur.</summary>
		/// <param name="name">The name of the source, typically the name of the application.</param>
		/// <param name="defaultLevel">A bitwise combination of the enumeration values that specifies the default source level at which to trace.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string ("").</exception>
		public TraceSource(string name, SourceLevels defaultLevel)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("name");
			}
			this.sourceName = name;
			this.switchLevel = defaultLevel;
			List<WeakReference> obj = TraceSource.tracesources;
			lock (obj)
			{
				TraceSource._pruneCachedTraceSources();
				TraceSource.tracesources.Add(new WeakReference(this));
			}
		}

		private static void _pruneCachedTraceSources()
		{
			List<WeakReference> obj = TraceSource.tracesources;
			lock (obj)
			{
				if (TraceSource.s_LastCollectionCount != GC.CollectionCount(2))
				{
					List<WeakReference> list = new List<WeakReference>(TraceSource.tracesources.Count);
					for (int i = 0; i < TraceSource.tracesources.Count; i++)
					{
						if ((TraceSource)TraceSource.tracesources[i].Target != null)
						{
							list.Add(TraceSource.tracesources[i]);
						}
					}
					if (list.Count < TraceSource.tracesources.Count)
					{
						TraceSource.tracesources.Clear();
						TraceSource.tracesources.AddRange(list);
						TraceSource.tracesources.TrimExcess();
					}
					TraceSource.s_LastCollectionCount = GC.CollectionCount(2);
				}
			}
		}

		private void Initialize()
		{
			if (!this._initCalled)
			{
				lock (this)
				{
					if (!this._initCalled)
					{
						SourceElementsCollection sources = DiagnosticsConfiguration.Sources;
						if (sources != null)
						{
							SourceElement sourceElement = sources[this.sourceName];
							if (sourceElement != null)
							{
								if (!string.IsNullOrEmpty(sourceElement.SwitchName))
								{
									this.CreateSwitch(sourceElement.SwitchType, sourceElement.SwitchName);
								}
								else
								{
									this.CreateSwitch(sourceElement.SwitchType, this.sourceName);
									if (!string.IsNullOrEmpty(sourceElement.SwitchValue))
									{
										this.internalSwitch.Level = (SourceLevels)Enum.Parse(typeof(SourceLevels), sourceElement.SwitchValue);
									}
								}
								this.listeners = sourceElement.Listeners.GetRuntimeObject();
								this.attributes = new StringDictionary();
								TraceUtils.VerifyAttributes(sourceElement.Attributes, this.GetSupportedAttributes(), this);
								this.attributes.ReplaceHashtable(sourceElement.Attributes);
							}
							else
							{
								this.NoConfigInit();
							}
						}
						else
						{
							this.NoConfigInit();
						}
						this._initCalled = true;
					}
				}
			}
		}

		private void NoConfigInit()
		{
			this.internalSwitch = new SourceSwitch(this.sourceName, this.switchLevel.ToString());
			this.listeners = new TraceListenerCollection();
			this.listeners.Add(new DefaultTraceListener());
			this.attributes = null;
		}

		/// <summary>Closes all the trace listeners in the trace listener collection.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Close()
		{
			if (this.listeners != null)
			{
				object critSec = TraceInternal.critSec;
				lock (critSec)
				{
					foreach (object obj in this.listeners)
					{
						((TraceListener)obj).Close();
					}
				}
			}
		}

		/// <summary>Flushes all the trace listeners in the trace listener collection.</summary>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		public void Flush()
		{
			if (this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						using (IEnumerator enumerator = this.listeners.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								((TraceListener)obj).Flush();
							}
							return;
						}
					}
				}
				foreach (object obj2 in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj2;
					if (!traceListener.IsThreadSafe)
					{
						TraceListener obj3 = traceListener;
						lock (obj3)
						{
							traceListener.Flush();
							continue;
						}
					}
					traceListener.Flush();
				}
			}
		}

		/// <summary>Gets the custom attributes supported by the trace source.</summary>
		/// <returns>A string array naming the custom attributes supported by the trace source, or <see langword="null" /> if there are no custom attributes.</returns>
		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		internal static void RefreshAll()
		{
			List<WeakReference> obj = TraceSource.tracesources;
			lock (obj)
			{
				TraceSource._pruneCachedTraceSources();
				for (int i = 0; i < TraceSource.tracesources.Count; i++)
				{
					TraceSource traceSource = (TraceSource)TraceSource.tracesources[i].Target;
					if (traceSource != null)
					{
						traceSource.Refresh();
					}
				}
			}
		}

		internal void Refresh()
		{
			if (!this._initCalled)
			{
				this.Initialize();
				return;
			}
			SourceElementsCollection sources = DiagnosticsConfiguration.Sources;
			if (sources != null)
			{
				SourceElement sourceElement = sources[this.Name];
				if (sourceElement != null)
				{
					if ((string.IsNullOrEmpty(sourceElement.SwitchType) && this.internalSwitch.GetType() != typeof(SourceSwitch)) || sourceElement.SwitchType != this.internalSwitch.GetType().AssemblyQualifiedName)
					{
						if (!string.IsNullOrEmpty(sourceElement.SwitchName))
						{
							this.CreateSwitch(sourceElement.SwitchType, sourceElement.SwitchName);
						}
						else
						{
							this.CreateSwitch(sourceElement.SwitchType, this.Name);
							if (!string.IsNullOrEmpty(sourceElement.SwitchValue))
							{
								this.internalSwitch.Level = (SourceLevels)Enum.Parse(typeof(SourceLevels), sourceElement.SwitchValue);
							}
						}
					}
					else if (!string.IsNullOrEmpty(sourceElement.SwitchName))
					{
						if (sourceElement.SwitchName != this.internalSwitch.DisplayName)
						{
							this.CreateSwitch(sourceElement.SwitchType, sourceElement.SwitchName);
						}
						else
						{
							this.internalSwitch.Refresh();
						}
					}
					else if (!string.IsNullOrEmpty(sourceElement.SwitchValue))
					{
						this.internalSwitch.Level = (SourceLevels)Enum.Parse(typeof(SourceLevels), sourceElement.SwitchValue);
					}
					else
					{
						this.internalSwitch.Level = SourceLevels.Off;
					}
					TraceListenerCollection traceListenerCollection = new TraceListenerCollection();
					foreach (object obj in sourceElement.Listeners)
					{
						ListenerElement listenerElement = (ListenerElement)obj;
						TraceListener traceListener = this.listeners[listenerElement.Name];
						if (traceListener != null)
						{
							traceListenerCollection.Add(listenerElement.RefreshRuntimeObject(traceListener));
						}
						else
						{
							traceListenerCollection.Add(listenerElement.GetRuntimeObject());
						}
					}
					TraceUtils.VerifyAttributes(sourceElement.Attributes, this.GetSupportedAttributes(), this);
					this.attributes = new StringDictionary();
					this.attributes.ReplaceHashtable(sourceElement.Attributes);
					this.listeners = traceListenerCollection;
					return;
				}
				this.internalSwitch.Level = this.switchLevel;
				this.listeners.Clear();
				this.attributes = null;
			}
		}

		/// <summary>Writes a trace event message to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified event type and event identifier.</summary>
		/// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id)
		{
			this.Initialize();
			TraceEventCache eventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceEvent(eventCache, this.Name, eventType, id);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener obj = traceListener2;
						lock (obj)
						{
							traceListener2.TraceEvent(eventCache, this.Name, eventType, id);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_10F;
						}
						goto IL_F1;
					}
					goto IL_F1;
					IL_10F:
					j++;
					continue;
					IL_F1:
					traceListener2.TraceEvent(eventCache, this.Name, eventType, id);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_10F;
					}
					goto IL_10F;
				}
			}
		}

		/// <summary>Writes a trace event message to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified event type, event identifier, and message.</summary>
		/// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string message)
		{
			this.Initialize();
			TraceEventCache eventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceEvent(eventCache, this.Name, eventType, id, message);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener obj = traceListener2;
						lock (obj)
						{
							traceListener2.TraceEvent(eventCache, this.Name, eventType, id, message);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_112;
						}
						goto IL_F3;
					}
					goto IL_F3;
					IL_112:
					j++;
					continue;
					IL_F3:
					traceListener2.TraceEvent(eventCache, this.Name, eventType, id, message);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_112;
					}
					goto IL_112;
				}
			}
		}

		/// <summary>Writes a trace event to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified event type, event identifier, and argument array and format.</summary>
		/// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the <paramref name="args" /> array.</param>
		/// <param name="args">An <see langword="object" /> array containing zero or more objects to format.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is invalid.  
		/// -or-  
		/// The number that indicates an argument to format is less than zero, or greater than or equal to the number of specified objects to format.</exception>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			this.Initialize();
			TraceEventCache eventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceEvent(eventCache, this.Name, eventType, id, format, args);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener obj = traceListener2;
						lock (obj)
						{
							traceListener2.TraceEvent(eventCache, this.Name, eventType, id, format, args);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_118;
						}
						goto IL_F7;
					}
					goto IL_F7;
					IL_118:
					j++;
					continue;
					IL_F7:
					traceListener2.TraceEvent(eventCache, this.Name, eventType, id, format, args);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_118;
					}
					goto IL_118;
				}
			}
		}

		/// <summary>Writes trace data to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified event type, event identifier, and trace data.</summary>
		/// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">The trace data.</param>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, object data)
		{
			this.Initialize();
			TraceEventCache eventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceData(eventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener obj = traceListener2;
						lock (obj)
						{
							traceListener2.TraceData(eventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_112;
						}
						goto IL_F3;
					}
					goto IL_F3;
					IL_112:
					j++;
					continue;
					IL_F3:
					traceListener2.TraceData(eventCache, this.Name, eventType, id, data);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_112;
					}
					goto IL_112;
				}
			}
		}

		/// <summary>Writes trace data to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified event type, event identifier, and trace data array.</summary>
		/// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">An object array containing the trace data.</param>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			this.Initialize();
			TraceEventCache eventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceData(eventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener obj = traceListener2;
						lock (obj)
						{
							traceListener2.TraceData(eventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_112;
						}
						goto IL_F3;
					}
					goto IL_F3;
					IL_112:
					j++;
					continue;
					IL_F3:
					traceListener2.TraceData(eventCache, this.Name, eventType, id, data);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_112;
					}
					goto IL_112;
				}
			}
		}

		/// <summary>Writes an informational message to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified message.</summary>
		/// <param name="message">The informative message to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceInformation(string message)
		{
		}

		/// <summary>Writes an informational message to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified object array and formatting information.</summary>
		/// <param name="format">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the <paramref name="args" /> array.</param>
		/// <param name="args">An array containing zero or more objects to format.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is invalid.  
		/// -or-  
		/// The number that indicates an argument to format is less than zero, or greater than or equal to the number of specified objects to format.</exception>
		/// <exception cref="T:System.ObjectDisposedException">An attempt was made to trace an event during finalization.</exception>
		[Conditional("TRACE")]
		public void TraceInformation(string format, params object[] args)
		{
		}

		/// <summary>Writes a trace transfer message to the trace listeners in the <see cref="P:System.Diagnostics.TraceSource.Listeners" /> collection using the specified numeric identifier, message, and related activity identifier.</summary>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write.</param>
		/// <param name="relatedActivityId">A structure that identifies the related activity.</param>
		[Conditional("TRACE")]
		public void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			this.Initialize();
			TraceEventCache eventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(TraceEventType.Transfer) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceTransfer(eventCache, this.Name, id, message, relatedActivityId);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener obj = traceListener2;
						lock (obj)
						{
							traceListener2.TraceTransfer(eventCache, this.Name, id, message, relatedActivityId);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_116;
						}
						goto IL_F7;
					}
					goto IL_F7;
					IL_116:
					j++;
					continue;
					IL_F7:
					traceListener2.TraceTransfer(eventCache, this.Name, id, message, relatedActivityId);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_116;
					}
					goto IL_116;
				}
			}
		}

		private void CreateSwitch(string typename, string name)
		{
			if (!string.IsNullOrEmpty(typename))
			{
				this.internalSwitch = (SourceSwitch)TraceUtils.GetRuntimeObject(typename, typeof(SourceSwitch), name);
				return;
			}
			this.internalSwitch = new SourceSwitch(name, this.switchLevel.ToString());
		}

		/// <summary>Gets the custom switch attributes defined in the application configuration file.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.StringDictionary" /> containing the custom attributes for the trace switch.</returns>
		public StringDictionary Attributes
		{
			get
			{
				this.Initialize();
				if (this.attributes == null)
				{
					this.attributes = new StringDictionary();
				}
				return this.attributes;
			}
		}

		/// <summary>Gets the name of the trace source.</summary>
		/// <returns>The name of the trace source.</returns>
		public string Name
		{
			get
			{
				return this.sourceName;
			}
		}

		/// <summary>Gets the collection of trace listeners for the trace source.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.TraceListenerCollection" /> that contains the active trace listeners associated with the source.</returns>
		public TraceListenerCollection Listeners
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				this.Initialize();
				return this.listeners;
			}
		}

		/// <summary>Gets or sets the source switch value.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.SourceSwitch" /> object representing the source switch value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Diagnostics.TraceSource.Switch" /> is set to <see langword="null" />.</exception>
		public SourceSwitch Switch
		{
			get
			{
				this.Initialize();
				return this.internalSwitch;
			}
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Switch");
				}
				this.Initialize();
				this.internalSwitch = value;
			}
		}

		private static List<WeakReference> tracesources = new List<WeakReference>();

		private static int s_LastCollectionCount;

		private volatile SourceSwitch internalSwitch;

		private volatile TraceListenerCollection listeners;

		private StringDictionary attributes;

		private SourceLevels switchLevel;

		private volatile string sourceName;

		internal volatile bool _initCalled;
	}
}
