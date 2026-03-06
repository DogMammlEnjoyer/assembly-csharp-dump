using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Unity;

namespace System.Security.Policy
{
	/// <summary>Defines the set of information that constitutes input to security policy decisions. This class cannot be inherited.</summary>
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public sealed class Evidence : ICollection, IEnumerable
	{
		/// <summary>Initializes a new empty instance of the <see cref="T:System.Security.Policy.Evidence" /> class.</summary>
		public Evidence()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Evidence" /> class from a shallow copy of an existing one.</summary>
		/// <param name="evidence">The <see cref="T:System.Security.Policy.Evidence" /> instance from which to create the new instance. This instance is not deep-copied.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="evidence" /> parameter is not a valid instance of <see cref="T:System.Security.Policy.Evidence" />.</exception>
		public Evidence(Evidence evidence)
		{
			if (evidence != null)
			{
				this.Merge(evidence);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Evidence" /> class from multiple sets of host and assembly evidence.</summary>
		/// <param name="hostEvidence">The host evidence from which to create the new instance.</param>
		/// <param name="assemblyEvidence">The assembly evidence from which to create the new instance.</param>
		public Evidence(EvidenceBase[] hostEvidence, EvidenceBase[] assemblyEvidence)
		{
			if (hostEvidence != null)
			{
				this.HostEvidenceList.AddRange(hostEvidence);
			}
			if (assemblyEvidence != null)
			{
				this.AssemblyEvidenceList.AddRange(assemblyEvidence);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Policy.Evidence" /> class from multiple sets of host and assembly evidence.</summary>
		/// <param name="hostEvidence">The host evidence from which to create the new instance.</param>
		/// <param name="assemblyEvidence">The assembly evidence from which to create the new instance.</param>
		[Obsolete]
		public Evidence(object[] hostEvidence, object[] assemblyEvidence)
		{
			if (hostEvidence != null)
			{
				this.HostEvidenceList.AddRange(hostEvidence);
			}
			if (assemblyEvidence != null)
			{
				this.AssemblyEvidenceList.AddRange(assemblyEvidence);
			}
		}

		/// <summary>Gets the number of evidence objects in the evidence set.</summary>
		/// <returns>The number of evidence objects in the evidence set.</returns>
		[Obsolete]
		public int Count
		{
			get
			{
				int num = 0;
				if (this.hostEvidenceList != null)
				{
					num += this.hostEvidenceList.Count;
				}
				if (this.assemblyEvidenceList != null)
				{
					num += this.assemblyEvidenceList.Count;
				}
				return num;
			}
		}

		/// <summary>Gets a value indicating whether the evidence set is read-only.</summary>
		/// <returns>Always <see langword="false" />, because read-only evidence sets are not supported.</returns>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the evidence set is thread-safe.</summary>
		/// <returns>Always <see langword="false" /> because thread-safe evidence sets are not supported.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets or sets a value indicating whether the evidence is locked.</summary>
		/// <returns>
		///   <see langword="true" /> if the evidence is locked; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool Locked
		{
			get
			{
				return this._locked;
			}
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
			set
			{
				this._locked = value;
			}
		}

		/// <summary>Gets the synchronization root.</summary>
		/// <returns>Always <see langword="this" /> (<see langword="Me" /> in Visual Basic), because synchronization of evidence sets is not supported.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal ArrayList HostEvidenceList
		{
			get
			{
				if (this.hostEvidenceList == null)
				{
					this.hostEvidenceList = ArrayList.Synchronized(new ArrayList());
				}
				return this.hostEvidenceList;
			}
		}

		internal ArrayList AssemblyEvidenceList
		{
			get
			{
				if (this.assemblyEvidenceList == null)
				{
					this.assemblyEvidenceList = ArrayList.Synchronized(new ArrayList());
				}
				return this.assemblyEvidenceList;
			}
		}

		/// <summary>Adds the specified assembly evidence to the evidence set.</summary>
		/// <param name="id">Any evidence object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="id" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="id" /> is not serializable.</exception>
		[Obsolete]
		public void AddAssembly(object id)
		{
			this.AssemblyEvidenceList.Add(id);
		}

		/// <summary>Adds the specified evidence supplied by the host to the evidence set.</summary>
		/// <param name="id">Any evidence object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="id" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="id" /> is not serializable.</exception>
		[Obsolete]
		public void AddHost(object id)
		{
			if (this._locked && SecurityManager.SecurityEnabled)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			this.HostEvidenceList.Add(id);
		}

		/// <summary>Removes the host and assembly evidence from the evidence set.</summary>
		[ComVisible(false)]
		public void Clear()
		{
			if (this.hostEvidenceList != null)
			{
				this.hostEvidenceList.Clear();
			}
			if (this.assemblyEvidenceList != null)
			{
				this.assemblyEvidenceList.Clear();
			}
		}

		/// <summary>Returns a duplicate copy of this evidence object.</summary>
		/// <returns>A duplicate copy of this evidence object.</returns>
		[ComVisible(false)]
		public Evidence Clone()
		{
			return new Evidence(this);
		}

		/// <summary>Copies evidence objects to an <see cref="T:System.Array" />.</summary>
		/// <param name="array">The target array to which to copy evidence objects.</param>
		/// <param name="index">The zero-based position in the array to which to begin copying evidence objects.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is outside the range of the target array.</exception>
		[Obsolete]
		public void CopyTo(Array array, int index)
		{
			int num = 0;
			if (this.hostEvidenceList != null)
			{
				num = this.hostEvidenceList.Count;
				if (num > 0)
				{
					this.hostEvidenceList.CopyTo(array, index);
				}
			}
			if (this.assemblyEvidenceList != null && this.assemblyEvidenceList.Count > 0)
			{
				this.assemblyEvidenceList.CopyTo(array, index + num);
			}
		}

		/// <summary>Enumerates all evidence in the set, both that provided by the host and that provided by the assembly.</summary>
		/// <returns>An enumerator for evidence added by both the <see cref="M:System.Security.Policy.Evidence.AddHost(System.Object)" /> method and the <see cref="M:System.Security.Policy.Evidence.AddAssembly(System.Object)" /> method.</returns>
		[Obsolete]
		public IEnumerator GetEnumerator()
		{
			IEnumerator hostenum = null;
			if (this.hostEvidenceList != null)
			{
				hostenum = this.hostEvidenceList.GetEnumerator();
			}
			IEnumerator assemblyenum = null;
			if (this.assemblyEvidenceList != null)
			{
				assemblyenum = this.assemblyEvidenceList.GetEnumerator();
			}
			return new Evidence.EvidenceEnumerator(hostenum, assemblyenum);
		}

		/// <summary>Enumerates evidence provided by the assembly.</summary>
		/// <returns>An enumerator for evidence added by the <see cref="M:System.Security.Policy.Evidence.AddAssembly(System.Object)" /> method.</returns>
		public IEnumerator GetAssemblyEnumerator()
		{
			return this.AssemblyEvidenceList.GetEnumerator();
		}

		/// <summary>Enumerates evidence supplied by the host.</summary>
		/// <returns>An enumerator for evidence added by the <see cref="M:System.Security.Policy.Evidence.AddHost(System.Object)" /> method.</returns>
		public IEnumerator GetHostEnumerator()
		{
			return this.HostEvidenceList.GetEnumerator();
		}

		/// <summary>Merges the specified evidence set into the current evidence set.</summary>
		/// <param name="evidence">The evidence set to be merged into the current evidence set.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="evidence" /> parameter is not a valid instance of <see cref="T:System.Security.Policy.Evidence" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">
		///   <see cref="P:System.Security.Policy.Evidence.Locked" /> is <see langword="true" />, the code that calls this method does not have <see cref="F:System.Security.Permissions.SecurityPermissionFlag.ControlEvidence" />, and the <paramref name="evidence" /> parameter has a host list that is not empty.</exception>
		public void Merge(Evidence evidence)
		{
			if (evidence != null && evidence.Count > 0)
			{
				if (evidence.hostEvidenceList != null)
				{
					foreach (object id in evidence.hostEvidenceList)
					{
						this.AddHost(id);
					}
				}
				if (evidence.assemblyEvidenceList != null)
				{
					foreach (object id2 in evidence.assemblyEvidenceList)
					{
						this.AddAssembly(id2);
					}
				}
			}
		}

		/// <summary>Removes the evidence for a given type from the host and assembly enumerations.</summary>
		/// <param name="t">The type of the evidence to be removed.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="t" /> is null.</exception>
		[ComVisible(false)]
		public void RemoveType(Type t)
		{
			for (int i = this.hostEvidenceList.Count; i >= 0; i--)
			{
				if (this.hostEvidenceList.GetType() == t)
				{
					this.hostEvidenceList.RemoveAt(i);
				}
			}
			for (int j = this.assemblyEvidenceList.Count; j >= 0; j--)
			{
				if (this.assemblyEvidenceList.GetType() == t)
				{
					this.assemblyEvidenceList.RemoveAt(j);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAuthenticodePresent(Assembly a);

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		internal static Evidence GetDefaultHostEvidence(Assembly a)
		{
			Evidence evidence = new Evidence();
			string escapedCodeBase = a.EscapedCodeBase;
			evidence.AddHost(Zone.CreateFromUrl(escapedCodeBase));
			evidence.AddHost(new Url(escapedCodeBase));
			evidence.AddHost(new Hash(a));
			if (string.Compare("FILE://", 0, escapedCodeBase, 0, 7, true, CultureInfo.InvariantCulture) != 0)
			{
				evidence.AddHost(Site.CreateFromUrl(escapedCodeBase));
			}
			AssemblyName name = a.GetName();
			byte[] publicKey = name.GetPublicKey();
			if (publicKey != null && publicKey.Length != 0)
			{
				StrongNamePublicKeyBlob blob = new StrongNamePublicKeyBlob(publicKey);
				evidence.AddHost(new StrongName(blob, name.Name, name.Version));
			}
			if (Evidence.IsAuthenticodePresent(a))
			{
				try
				{
					X509Certificate cert = X509Certificate.CreateFromSignedFile(a.Location);
					evidence.AddHost(new Publisher(cert));
				}
				catch (CryptographicException)
				{
				}
			}
			if (a.GlobalAssemblyCache)
			{
				evidence.AddHost(new GacInstalled());
			}
			AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
			if (domainManager != null && (domainManager.HostSecurityManager.Flags & HostSecurityManagerOptions.HostAssemblyEvidence) == HostSecurityManagerOptions.HostAssemblyEvidence)
			{
				evidence = domainManager.HostSecurityManager.ProvideAssemblyEvidence(a, evidence);
			}
			return evidence;
		}

		/// <summary>Adds an evidence object of the specified type to the assembly-supplied evidence list.</summary>
		/// <param name="evidence">The assembly evidence to add.</param>
		/// <typeparam name="T">The type of the object in <paramref name="evidence" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="evidence" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">Evidence of type <paramref name="T" /> is already in the list.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="evidence" /> is not serializable.</exception>
		[ComVisible(false)]
		public void AddAssemblyEvidence<T>(T evidence)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Adds host evidence of the specified type to the host evidence collection.</summary>
		/// <param name="evidence">The host evidence to add.</param>
		/// <typeparam name="T">The type of the object in <paramref name="evidence" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="evidence" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">Evidence of type <paramref name="T" /> is already in the list.</exception>
		[ComVisible(false)]
		public void AddHostEvidence<T>(T evidence)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets assembly evidence of the specified type from the collection.</summary>
		/// <typeparam name="T">The type of the evidence to get.</typeparam>
		/// <returns>Evidence of type <paramref name="T" /> in the assembly evidence collection.</returns>
		[ComVisible(false)]
		public T GetAssemblyEvidence<T>()
		{
			ThrowStub.ThrowNotSupportedException();
			return default(T);
		}

		/// <summary>Gets host evidence of the specified type from the collection.</summary>
		/// <typeparam name="T">The type of the evidence to get.</typeparam>
		/// <returns>Evidence of type <paramref name="T" /> in the host evidence collection.</returns>
		[ComVisible(false)]
		public T GetHostEvidence<T>()
		{
			ThrowStub.ThrowNotSupportedException();
			return default(T);
		}

		private bool _locked;

		private ArrayList hostEvidenceList;

		private ArrayList assemblyEvidenceList;

		private class EvidenceEnumerator : IEnumerator
		{
			public EvidenceEnumerator(IEnumerator hostenum, IEnumerator assemblyenum)
			{
				this.hostEnum = hostenum;
				this.assemblyEnum = assemblyenum;
				this.currentEnum = this.hostEnum;
			}

			public bool MoveNext()
			{
				if (this.currentEnum == null)
				{
					return false;
				}
				bool flag = this.currentEnum.MoveNext();
				if (!flag && this.hostEnum == this.currentEnum && this.assemblyEnum != null)
				{
					this.currentEnum = this.assemblyEnum;
					flag = this.assemblyEnum.MoveNext();
				}
				return flag;
			}

			public void Reset()
			{
				if (this.hostEnum != null)
				{
					this.hostEnum.Reset();
					this.currentEnum = this.hostEnum;
				}
				else
				{
					this.currentEnum = this.assemblyEnum;
				}
				if (this.assemblyEnum != null)
				{
					this.assemblyEnum.Reset();
				}
			}

			public object Current
			{
				get
				{
					return this.currentEnum.Current;
				}
			}

			private IEnumerator currentEnum;

			private IEnumerator hostEnum;

			private IEnumerator assemblyEnum;
		}
	}
}
