using System;
using System.Diagnostics;
using System.Threading;

namespace System
{
	/// <summary>Provides support for lazy initialization.</summary>
	/// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
	[DebuggerTypeProxy(typeof(LazyDebugView<>))]
	[DebuggerDisplay("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
	[Serializable]
	public class Lazy<T>
	{
		private static T CreateViaDefaultConstructor()
		{
			return (T)((object)LazyHelper.CreateViaDefaultConstructor(typeof(T)));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Lazy`1" /> class. When lazy initialization occurs, the default constructor of the target type is used.</summary>
		public Lazy() : this(null, LazyThreadSafetyMode.ExecutionAndPublication, true)
		{
		}

		public Lazy(T value)
		{
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Lazy`1" /> class. When lazy initialization occurs, the specified initialization function is used.</summary>
		/// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="valueFactory" /> is <see langword="null" />.</exception>
		public Lazy(Func<T> valueFactory) : this(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Lazy`1" /> class. When lazy initialization occurs, the default constructor of the target type and the specified initialization mode are used.</summary>
		/// <param name="isThreadSafe">
		///   <see langword="true" /> to make this instance usable concurrently by multiple threads; <see langword="false" /> to make the instance usable by only one thread at a time.</param>
		public Lazy(bool isThreadSafe) : this(null, LazyHelper.GetModeFromIsThreadSafe(isThreadSafe), true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Lazy`1" /> class that uses the default constructor of <paramref name="T" /> and the specified thread-safety mode.</summary>
		/// <param name="mode">One of the enumeration values that specifies the thread safety mode.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="mode" /> contains an invalid value.</exception>
		public Lazy(LazyThreadSafetyMode mode) : this(null, mode, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Lazy`1" /> class. When lazy initialization occurs, the specified initialization function and initialization mode are used.</summary>
		/// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
		/// <param name="isThreadSafe">
		///   <see langword="true" /> to make this instance usable concurrently by multiple threads; <see langword="false" /> to make this instance usable by only one thread at a time.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="valueFactory" /> is <see langword="null" />.</exception>
		public Lazy(Func<T> valueFactory, bool isThreadSafe) : this(valueFactory, LazyHelper.GetModeFromIsThreadSafe(isThreadSafe), false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Lazy`1" /> class that uses the specified initialization function and thread-safety mode.</summary>
		/// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
		/// <param name="mode">One of the enumeration values that specifies the thread safety mode.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="mode" /> contains an invalid value.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="valueFactory" /> is <see langword="null" />.</exception>
		public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode) : this(valueFactory, mode, false)
		{
		}

		private Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode, bool useDefaultConstructor)
		{
			if (valueFactory == null && !useDefaultConstructor)
			{
				throw new ArgumentNullException("valueFactory");
			}
			this._factory = valueFactory;
			this._state = LazyHelper.Create(mode, useDefaultConstructor);
		}

		private void ViaConstructor()
		{
			this._value = Lazy<T>.CreateViaDefaultConstructor();
			this._state = null;
		}

		private void ViaFactory(LazyThreadSafetyMode mode)
		{
			try
			{
				Func<T> factory = this._factory;
				if (factory == null)
				{
					throw new InvalidOperationException("ValueFactory attempted to access the Value property of this instance.");
				}
				this._factory = null;
				this._value = factory();
				this._state = null;
			}
			catch (Exception exception)
			{
				this._state = new LazyHelper(mode, exception);
				throw;
			}
		}

		private void ExecutionAndPublication(LazyHelper executionAndPublication, bool useDefaultConstructor)
		{
			lock (executionAndPublication)
			{
				if (this._state == executionAndPublication)
				{
					if (useDefaultConstructor)
					{
						this.ViaConstructor();
					}
					else
					{
						this.ViaFactory(LazyThreadSafetyMode.ExecutionAndPublication);
					}
				}
			}
		}

		private void PublicationOnly(LazyHelper publicationOnly, T possibleValue)
		{
			if (Interlocked.CompareExchange<LazyHelper>(ref this._state, LazyHelper.PublicationOnlyWaitForOtherThreadToPublish, publicationOnly) == publicationOnly)
			{
				this._factory = null;
				this._value = possibleValue;
				this._state = null;
			}
		}

		private void PublicationOnlyViaConstructor(LazyHelper initializer)
		{
			this.PublicationOnly(initializer, Lazy<T>.CreateViaDefaultConstructor());
		}

		private void PublicationOnlyViaFactory(LazyHelper initializer)
		{
			Func<T> factory = this._factory;
			if (factory == null)
			{
				this.PublicationOnlyWaitForOtherThreadToPublish();
				return;
			}
			this.PublicationOnly(initializer, factory());
		}

		private void PublicationOnlyWaitForOtherThreadToPublish()
		{
			SpinWait spinWait = default(SpinWait);
			while (this._state != null)
			{
				spinWait.SpinOnce();
			}
		}

		private T CreateValue()
		{
			LazyHelper state = this._state;
			if (state != null)
			{
				switch (state.State)
				{
				case LazyState.NoneViaConstructor:
					this.ViaConstructor();
					goto IL_84;
				case LazyState.NoneViaFactory:
					this.ViaFactory(LazyThreadSafetyMode.None);
					goto IL_84;
				case LazyState.PublicationOnlyViaConstructor:
					this.PublicationOnlyViaConstructor(state);
					goto IL_84;
				case LazyState.PublicationOnlyViaFactory:
					this.PublicationOnlyViaFactory(state);
					goto IL_84;
				case LazyState.PublicationOnlyWait:
					this.PublicationOnlyWaitForOtherThreadToPublish();
					goto IL_84;
				case LazyState.ExecutionAndPublicationViaConstructor:
					this.ExecutionAndPublication(state, true);
					goto IL_84;
				case LazyState.ExecutionAndPublicationViaFactory:
					this.ExecutionAndPublication(state, false);
					goto IL_84;
				}
				state.ThrowException();
			}
			IL_84:
			return this.Value;
		}

		/// <summary>Creates and returns a string representation of the <see cref="P:System.Lazy`1.Value" /> property for this instance.</summary>
		/// <returns>The result of calling the <see cref="M:System.Object.ToString" /> method on the <see cref="P:System.Lazy`1.Value" /> property for this instance, if the value has been created (that is, if the <see cref="P:System.Lazy`1.IsValueCreated" /> property returns <see langword="true" />). Otherwise, a string indicating that the value has not been created.</returns>
		/// <exception cref="T:System.NullReferenceException">The <see cref="P:System.Lazy`1.Value" /> property is <see langword="null" />.</exception>
		public override string ToString()
		{
			if (!this.IsValueCreated)
			{
				return "Value is not created.";
			}
			T value = this.Value;
			return value.ToString();
		}

		internal T ValueForDebugDisplay
		{
			get
			{
				if (!this.IsValueCreated)
				{
					return default(T);
				}
				return this._value;
			}
		}

		internal LazyThreadSafetyMode? Mode
		{
			get
			{
				return LazyHelper.GetMode(this._state);
			}
		}

		internal bool IsValueFaulted
		{
			get
			{
				return LazyHelper.GetIsValueFaulted(this._state);
			}
		}

		/// <summary>Gets a value that indicates whether a value has been created for this <see cref="T:System.Lazy`1" /> instance.</summary>
		/// <returns>
		///   <see langword="true" /> if a value has been created for this <see cref="T:System.Lazy`1" /> instance; otherwise, <see langword="false" />.</returns>
		public bool IsValueCreated
		{
			get
			{
				return this._state == null;
			}
		}

		/// <summary>Gets the lazily initialized value of the current <see cref="T:System.Lazy`1" /> instance.</summary>
		/// <returns>The lazily initialized value of the current <see cref="T:System.Lazy`1" /> instance.</returns>
		/// <exception cref="T:System.MemberAccessException">The <see cref="T:System.Lazy`1" /> instance is initialized to use the default constructor of the type that is being lazily initialized, and permissions to access the constructor are missing.</exception>
		/// <exception cref="T:System.MissingMemberException">The <see cref="T:System.Lazy`1" /> instance is initialized to use the default constructor of the type that is being lazily initialized, and that type does not have a public, parameterless constructor.</exception>
		/// <exception cref="T:System.InvalidOperationException">The initialization function tries to access <see cref="P:System.Lazy`1.Value" /> on this instance.</exception>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Value
		{
			get
			{
				if (this._state != null)
				{
					return this.CreateValue();
				}
				return this._value;
			}
		}

		private volatile LazyHelper _state;

		private Func<T> _factory;

		private T _value;
	}
}
