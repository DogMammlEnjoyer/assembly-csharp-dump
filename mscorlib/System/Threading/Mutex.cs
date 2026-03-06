using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace System.Threading
{
	/// <summary>A synchronization primitive that can also be used for interprocess synchronization.</summary>
	[ComVisible(true)]
	public sealed class Mutex : WaitHandle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr CreateMutex_icall(bool initiallyOwned, char* name, int name_length, out bool created);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr OpenMutex_icall(char* name, int name_length, MutexRights rights, out MonoIOError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ReleaseMutex_internal(IntPtr handle);

		private unsafe static IntPtr CreateMutex_internal(bool initiallyOwned, string name, out bool created)
		{
			char* ptr = name;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Mutex.CreateMutex_icall(initiallyOwned, ptr, (name != null) ? name.Length : 0, out created);
		}

		private unsafe static IntPtr OpenMutex_internal(string name, MutexRights rights, out MonoIOError error)
		{
			char* ptr = name;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Mutex.OpenMutex_icall(ptr, (name != null) ? name.Length : 0, rights, out error);
		}

		private Mutex(IntPtr handle)
		{
			this.Handle = handle;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Mutex" /> class with default properties.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public Mutex()
		{
			bool flag;
			this.Handle = Mutex.CreateMutex_internal(false, null, out flag);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Mutex" /> class with a Boolean value that indicates whether the calling thread should have initial ownership of the mutex.</summary>
		/// <param name="initiallyOwned">
		///   <see langword="true" /> to give the calling thread initial ownership of the mutex; otherwise, <see langword="false" />.</param>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public Mutex(bool initiallyOwned)
		{
			bool flag;
			this.Handle = Mutex.CreateMutex_internal(initiallyOwned, null, out flag);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Mutex" /> class with a Boolean value that indicates whether the calling thread should have initial ownership of the mutex, and a string that is the name of the mutex.</summary>
		/// <param name="initiallyOwned">
		///   <see langword="true" /> to give the calling thread initial ownership of the named system mutex if the named system mutex is created as a result of this call; otherwise, <see langword="false" />.</param>
		/// <param name="name">The name of the <see cref="T:System.Threading.Mutex" />. If the value is <see langword="null" />, the <see cref="T:System.Threading.Mutex" /> is unnamed.</param>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists and has access control security, but the user does not have <see cref="F:System.Security.AccessControl.MutexRights.FullControl" />.</exception>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.Threading.WaitHandleCannotBeOpenedException">The named mutex cannot be created, perhaps because a wait handle of a different type has the same name.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is longer than 260 characters.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public Mutex(bool initiallyOwned, string name)
		{
			bool flag;
			this.Handle = Mutex.CreateMutex_internal(initiallyOwned, name, out flag);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Mutex" /> class with a Boolean value that indicates whether the calling thread should have initial ownership of the mutex, a string that is the name of the mutex, and a Boolean value that, when the method returns, indicates whether the calling thread was granted initial ownership of the mutex.</summary>
		/// <param name="initiallyOwned">
		///   <see langword="true" /> to give the calling thread initial ownership of the named system mutex if the named system mutex is created as a result of this call; otherwise, <see langword="false" />.</param>
		/// <param name="name">The name of the <see cref="T:System.Threading.Mutex" />. If the value is <see langword="null" />, the <see cref="T:System.Threading.Mutex" /> is unnamed.</param>
		/// <param name="createdNew">When this method returns, contains a Boolean that is <see langword="true" /> if a local mutex was created (that is, if <paramref name="name" /> is <see langword="null" /> or an empty string) or if the specified named system mutex was created; <see langword="false" /> if the specified named system mutex already existed. This parameter is passed uninitialized.</param>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists and has access control security, but the user does not have <see cref="F:System.Security.AccessControl.MutexRights.FullControl" />.</exception>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.Threading.WaitHandleCannotBeOpenedException">The named mutex cannot be created, perhaps because a wait handle of a different type has the same name.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is longer than 260 characters.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public Mutex(bool initiallyOwned, string name, out bool createdNew)
		{
			this.Handle = Mutex.CreateMutex_internal(initiallyOwned, name, out createdNew);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Mutex" /> class with a Boolean value that indicates whether the calling thread should have initial ownership of the mutex, a string that is the name of the mutex, a Boolean variable that, when the method returns, indicates whether the calling thread was granted initial ownership of the mutex, and the access control security to be applied to the named mutex.</summary>
		/// <param name="initiallyOwned">
		///   <see langword="true" /> to give the calling thread initial ownership of the named system mutex if the named system mutex is created as a result of this call; otherwise, <see langword="false" />.</param>
		/// <param name="name">The name of the system mutex. If the value is <see langword="null" />, the <see cref="T:System.Threading.Mutex" /> is unnamed.</param>
		/// <param name="createdNew">When this method returns, contains a Boolean that is <see langword="true" /> if a local mutex was created (that is, if <paramref name="name" /> is <see langword="null" /> or an empty string) or if the specified named system mutex was created; <see langword="false" /> if the specified named system mutex already existed. This parameter is passed uninitialized.</param>
		/// <param name="mutexSecurity">A <see cref="T:System.Security.AccessControl.MutexSecurity" /> object that represents the access control security to be applied to the named system mutex.</param>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists and has access control security, but the user does not have <see cref="F:System.Security.AccessControl.MutexRights.FullControl" />.</exception>
		/// <exception cref="T:System.Threading.WaitHandleCannotBeOpenedException">The named mutex cannot be created, perhaps because a wait handle of a different type has the same name.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is longer than 260 characters.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO("Use MutexSecurity in CreateMutex_internal")]
		public Mutex(bool initiallyOwned, string name, out bool createdNew, MutexSecurity mutexSecurity)
		{
			this.Handle = Mutex.CreateMutex_internal(initiallyOwned, name, out createdNew);
		}

		/// <summary>Gets a <see cref="T:System.Security.AccessControl.MutexSecurity" /> object that represents the access control security for the named mutex.</summary>
		/// <returns>A <see cref="T:System.Security.AccessControl.MutexSecurity" /> object that represents the access control security for the named mutex.</returns>
		/// <exception cref="T:System.UnauthorizedAccessException">The current <see cref="T:System.Threading.Mutex" /> object represents a named system mutex, but the user does not have <see cref="F:System.Security.AccessControl.MutexRights.ReadPermissions" />.  
		///  -or-  
		///  The current <see cref="T:System.Threading.Mutex" /> object represents a named system mutex, and was not opened with <see cref="F:System.Security.AccessControl.MutexRights.ReadPermissions" />.</exception>
		/// <exception cref="T:System.NotSupportedException">Not supported for Windows 98 or Windows Millennium Edition.</exception>
		public MutexSecurity GetAccessControl()
		{
			return new MutexSecurity(base.SafeWaitHandle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		/// <summary>Opens the specified named mutex, if it already exists.</summary>
		/// <param name="name">The name of the system mutex to open.</param>
		/// <returns>An object that represents the named system mutex.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string.  
		/// -or-  
		/// <paramref name="name" /> is longer than 260 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Threading.WaitHandleCannotBeOpenedException">The named mutex does not exist.</exception>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists, but the user does not have the security access required to use it.</exception>
		public static Mutex OpenExisting(string name)
		{
			return Mutex.OpenExisting(name, MutexRights.Modify | MutexRights.Synchronize);
		}

		/// <summary>Opens the specified named mutex, if it already exists, with the desired security access.</summary>
		/// <param name="name">The name of the system mutex to open.</param>
		/// <param name="rights">A bitwise combination of the enumeration values that represent the desired security access.</param>
		/// <returns>An object that represents the named system mutex.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string.  
		/// -or-  
		/// <paramref name="name" /> is longer than 260 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Threading.WaitHandleCannotBeOpenedException">The named mutex does not exist.</exception>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists, but the user does not have the desired security access.</exception>
		public static Mutex OpenExisting(string name, MutexRights rights)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0 || name.Length > 260)
			{
				throw new ArgumentException("name", Locale.GetText("Invalid length [1-260]."));
			}
			MonoIOError monoIOError;
			IntPtr intPtr = Mutex.OpenMutex_internal(name, rights, out monoIOError);
			if (!(intPtr == (IntPtr)null))
			{
				return new Mutex(intPtr);
			}
			if (monoIOError == MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				throw new WaitHandleCannotBeOpenedException(Locale.GetText("Named Mutex handle does not exist: ") + name);
			}
			if (monoIOError == MonoIOError.ERROR_ACCESS_DENIED)
			{
				throw new UnauthorizedAccessException();
			}
			throw new IOException(Locale.GetText("Win32 IO error: ") + monoIOError.ToString());
		}

		/// <summary>Opens the specified named mutex, if it already exists, and returns a value that indicates whether the operation succeeded.</summary>
		/// <param name="name">The name of the system mutex to open.</param>
		/// <param name="result">When this method returns, contains a <see cref="T:System.Threading.Mutex" /> object that represents the named mutex if the call succeeded, or <see langword="null" /> if the call failed. This parameter is treated as uninitialized.</param>
		/// <returns>
		///   <see langword="true" /> if the named mutex was opened successfully; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string.  
		/// -or-  
		/// <paramref name="name" /> is longer than 260 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists, but the user does not have the security access required to use it.</exception>
		public static bool TryOpenExisting(string name, out Mutex result)
		{
			return Mutex.TryOpenExisting(name, MutexRights.Modify | MutexRights.Synchronize, out result);
		}

		/// <summary>Opens the specified named mutex, if it already exists, with the desired security access, and returns a value that indicates whether the operation succeeded.</summary>
		/// <param name="name">The name of the system mutex to open.</param>
		/// <param name="rights">A bitwise combination of the enumeration values that represent the desired security access.</param>
		/// <param name="result">When this method returns, contains a <see cref="T:System.Threading.Mutex" /> object that represents the named mutex if the call succeeded, or <see langword="null" /> if the call failed. This parameter is treated as uninitialized.</param>
		/// <returns>
		///   <see langword="true" /> if the named mutex was opened successfully; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string.  
		/// -or-  
		/// <paramref name="name" /> is longer than 260 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.IOException">A Win32 error occurred.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The named mutex exists, but the user does not have the security access required to use it.</exception>
		public static bool TryOpenExisting(string name, MutexRights rights, out Mutex result)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0 || name.Length > 260)
			{
				throw new ArgumentException("name", Locale.GetText("Invalid length [1-260]."));
			}
			MonoIOError monoIOError;
			IntPtr intPtr = Mutex.OpenMutex_internal(name, rights, out monoIOError);
			if (intPtr == (IntPtr)null)
			{
				result = null;
				return false;
			}
			result = new Mutex(intPtr);
			return true;
		}

		/// <summary>Releases the <see cref="T:System.Threading.Mutex" /> once.</summary>
		/// <exception cref="T:System.ApplicationException">The calling thread does not own the mutex.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has already been disposed.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void ReleaseMutex()
		{
			if (!Mutex.ReleaseMutex_internal(this.Handle))
			{
				throw new ApplicationException("Mutex is not owned");
			}
		}

		/// <summary>Sets the access control security for a named system mutex.</summary>
		/// <param name="mutexSecurity">A <see cref="T:System.Security.AccessControl.MutexSecurity" /> object that represents the access control security to be applied to the named system mutex.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="mutexSecurity" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The user does not have <see cref="F:System.Security.AccessControl.MutexRights.ChangePermissions" />.  
		///  -or-  
		///  The mutex was not opened with <see cref="F:System.Security.AccessControl.MutexRights.ChangePermissions" />.</exception>
		/// <exception cref="T:System.SystemException">The current <see cref="T:System.Threading.Mutex" /> object does not represent a named system mutex.</exception>
		public void SetAccessControl(MutexSecurity mutexSecurity)
		{
			if (mutexSecurity == null)
			{
				throw new ArgumentNullException("mutexSecurity");
			}
			mutexSecurity.PersistModifications(base.SafeWaitHandle);
		}
	}
}
