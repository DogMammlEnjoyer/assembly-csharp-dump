using System;

namespace System.Threading
{
	/// <summary>Provides lazy initialization routines.</summary>
	public static class LazyInitializer
	{
		/// <summary>Initializes a target reference type with the type's default constructor if it hasn't already been initialized.</summary>
		/// <param name="target">A reference of type T to initialize if it has not already been initialized.</param>
		/// <typeparam name="T">The type of the reference to be initialized.</typeparam>
		/// <returns>The initialized reference of type <paramref name="T" />.</returns>
		/// <exception cref="T:System.MemberAccessException">Permissions to access the constructor of type <paramref name="T" /> were missing.</exception>
		/// <exception cref="T:System.MissingMemberException">Type <paramref name="T" /> does not have a default constructor.</exception>
		public static T EnsureInitialized<T>(ref T target) where T : class
		{
			T result;
			if ((result = Volatile.Read<T>(ref target)) == null)
			{
				result = LazyInitializer.EnsureInitializedCore<T>(ref target);
			}
			return result;
		}

		private static T EnsureInitializedCore<T>(ref T target) where T : class
		{
			try
			{
				Interlocked.CompareExchange<T>(ref target, Activator.CreateInstance<T>(), default(T));
			}
			catch (MissingMethodException)
			{
				throw new MissingMemberException("The lazily-initialized type does not have a public, parameterless constructor.");
			}
			return target;
		}

		/// <summary>Initializes a target reference type by using a specified function if it hasn't already been initialized.</summary>
		/// <param name="target">The reference of type T to initialize if it hasn't already been initialized.</param>
		/// <param name="valueFactory">The function that is called to initialize the reference.</param>
		/// <typeparam name="T">The reference type of the reference to be initialized.</typeparam>
		/// <returns>The initialized value of type <paramref name="T" />.</returns>
		/// <exception cref="T:System.MissingMemberException">Type <paramref name="T" /> does not have a default constructor.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="valueFactory" /> returned null (Nothing in Visual Basic).</exception>
		public static T EnsureInitialized<T>(ref T target, Func<T> valueFactory) where T : class
		{
			T result;
			if ((result = Volatile.Read<T>(ref target)) == null)
			{
				result = LazyInitializer.EnsureInitializedCore<T>(ref target, valueFactory);
			}
			return result;
		}

		private static T EnsureInitializedCore<T>(ref T target, Func<T> valueFactory) where T : class
		{
			T t = valueFactory();
			if (t == null)
			{
				throw new InvalidOperationException("ValueFactory returned null.");
			}
			Interlocked.CompareExchange<T>(ref target, t, default(T));
			return target;
		}

		/// <summary>Initializes a target reference or value type with its default constructor if it hasn't already been initialized.</summary>
		/// <param name="target">A reference or value of type T to initialize if it hasn't already been initialized.</param>
		/// <param name="initialized">A reference to a Boolean value that determines whether the target has already been initialized.</param>
		/// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing <paramref name="target" />. If <paramref name="syncLock" /> is <see langword="null" />, a new object will be instantiated.</param>
		/// <typeparam name="T">The type of the reference to be initialized.</typeparam>
		/// <returns>The initialized value of type <paramref name="T" />.</returns>
		/// <exception cref="T:System.MemberAccessException">Permissions to access the constructor of type <paramref name="T" /> were missing.</exception>
		/// <exception cref="T:System.MissingMemberException">Type <paramref name="T" /> does not have a default constructor.</exception>
		public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock)
		{
			if (Volatile.Read(ref initialized))
			{
				return target;
			}
			return LazyInitializer.EnsureInitializedCore<T>(ref target, ref initialized, ref syncLock);
		}

		private static T EnsureInitializedCore<T>(ref T target, ref bool initialized, ref object syncLock)
		{
			object obj = LazyInitializer.EnsureLockInitialized(ref syncLock);
			lock (obj)
			{
				if (!Volatile.Read(ref initialized))
				{
					try
					{
						target = Activator.CreateInstance<T>();
					}
					catch (MissingMethodException)
					{
						throw new MissingMemberException("The lazily-initialized type does not have a public, parameterless constructor.");
					}
					Volatile.Write(ref initialized, true);
				}
			}
			return target;
		}

		/// <summary>Initializes a target reference or value type by using a specified function if it hasn't already been initialized.</summary>
		/// <param name="target">A reference or value of type T to initialize if it hasn't already been initialized.</param>
		/// <param name="initialized">A reference to a Boolean value that determines whether the target has already been initialized.</param>
		/// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing <paramref name="target" />. If <paramref name="syncLock" /> is <see langword="null" />, a new object will be instantiated.</param>
		/// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
		/// <typeparam name="T">The type of the reference to be initialized.</typeparam>
		/// <returns>The initialized value of type <paramref name="T" />.</returns>
		/// <exception cref="T:System.MemberAccessException">Permissions to access the constructor of type <paramref name="T" /> were missing.</exception>
		/// <exception cref="T:System.MissingMemberException">Type <paramref name="T" /> does not have a default constructor.</exception>
		public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock, Func<T> valueFactory)
		{
			if (Volatile.Read(ref initialized))
			{
				return target;
			}
			return LazyInitializer.EnsureInitializedCore<T>(ref target, ref initialized, ref syncLock, valueFactory);
		}

		private static T EnsureInitializedCore<T>(ref T target, ref bool initialized, ref object syncLock, Func<T> valueFactory)
		{
			object obj = LazyInitializer.EnsureLockInitialized(ref syncLock);
			lock (obj)
			{
				if (!Volatile.Read(ref initialized))
				{
					target = valueFactory();
					Volatile.Write(ref initialized, true);
				}
			}
			return target;
		}

		public static T EnsureInitialized<T>(ref T target, ref object syncLock, Func<T> valueFactory) where T : class
		{
			T result;
			if ((result = Volatile.Read<T>(ref target)) == null)
			{
				result = LazyInitializer.EnsureInitializedCore<T>(ref target, ref syncLock, valueFactory);
			}
			return result;
		}

		private static T EnsureInitializedCore<T>(ref T target, ref object syncLock, Func<T> valueFactory) where T : class
		{
			object obj = LazyInitializer.EnsureLockInitialized(ref syncLock);
			lock (obj)
			{
				if (Volatile.Read<T>(ref target) == null)
				{
					Volatile.Write<T>(ref target, valueFactory());
					if (target == null)
					{
						throw new InvalidOperationException("ValueFactory returned null.");
					}
				}
			}
			return target;
		}

		private static object EnsureLockInitialized(ref object syncLock)
		{
			object result;
			if ((result = syncLock) == null)
			{
				result = (Interlocked.CompareExchange(ref syncLock, new object(), null) ?? syncLock);
			}
			return result;
		}
	}
}
