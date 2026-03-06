using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Resolvers
{
	/// <summary>Represents a class that is used to prepopulate the cache with DTDs or XML streams.</summary>
	public class XmlPreloadedResolver : XmlResolver
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> class.</summary>
		public XmlPreloadedResolver() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> class with the specified preloaded well-known DTDs.</summary>
		/// <param name="preloadedDtds">The well-known DTDs that should be prepopulated into the cache.</param>
		public XmlPreloadedResolver(XmlKnownDtds preloadedDtds) : this(null, preloadedDtds, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> class with the specified fallback resolver.</summary>
		/// <param name="fallbackResolver">The <see langword="XmlResolver" />, <see langword="XmlXapResolver" />, or your own resolver.</param>
		public XmlPreloadedResolver(XmlResolver fallbackResolver) : this(fallbackResolver, XmlKnownDtds.All, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> class with the specified fallback resolver and preloaded well-known DTDs.</summary>
		/// <param name="fallbackResolver">The <see langword="XmlResolver" />, <see langword="XmlXapResolver" />, or your own resolver.</param>
		/// <param name="preloadedDtds">The well-known DTDs that should be prepopulated into the cache.</param>
		public XmlPreloadedResolver(XmlResolver fallbackResolver, XmlKnownDtds preloadedDtds) : this(fallbackResolver, preloadedDtds, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> class with the specified fallback resolver, preloaded well-known DTDs, and URI equality comparer.</summary>
		/// <param name="fallbackResolver">The <see langword="XmlResolver" />, <see langword="XmlXapResolver" />, or your own resolver.</param>
		/// <param name="preloadedDtds">The well-known DTDs that should be prepopulated into cache.</param>
		/// <param name="uriComparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> generic interface to use when you compare URIs.</param>
		public XmlPreloadedResolver(XmlResolver fallbackResolver, XmlKnownDtds preloadedDtds, IEqualityComparer<Uri> uriComparer)
		{
			this._fallbackResolver = fallbackResolver;
			this._mappings = new Dictionary<Uri, XmlPreloadedResolver.PreloadedData>(16, uriComparer);
			this._preloadedDtds = preloadedDtds;
			if (preloadedDtds != XmlKnownDtds.None)
			{
				if ((preloadedDtds & XmlKnownDtds.Xhtml10) != XmlKnownDtds.None)
				{
					this.AddKnownDtd(XmlPreloadedResolver.s_xhtml10_Dtd);
				}
				if ((preloadedDtds & XmlKnownDtds.Rss091) != XmlKnownDtds.None)
				{
					this.AddKnownDtd(XmlPreloadedResolver.s_rss091_Dtd);
				}
			}
		}

		/// <summary>Resolves the absolute URI from the base and relative URIs.</summary>
		/// <param name="baseUri">The base URI used to resolve the relative URI.</param>
		/// <param name="relativeUri">The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the <paramref name="baseUri" /> value. If relative, it combines with the <paramref name="baseUri" /> to make an absolute URI.</param>
		/// <returns>The <see cref="T:System.Uri" /> representing the absolute URI or <see langword="null" /> if the relative URI cannot be resolved.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> is <see langword="null" />.</exception>
		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			if (relativeUri != null && relativeUri.StartsWith("-//", StringComparison.CurrentCulture))
			{
				if ((this._preloadedDtds & XmlKnownDtds.Xhtml10) != XmlKnownDtds.None && relativeUri.StartsWith("-//W3C//", StringComparison.CurrentCulture))
				{
					for (int i = 0; i < XmlPreloadedResolver.s_xhtml10_Dtd.Length; i++)
					{
						if (relativeUri == XmlPreloadedResolver.s_xhtml10_Dtd[i].publicId)
						{
							return new Uri(relativeUri, UriKind.Relative);
						}
					}
				}
				if ((this._preloadedDtds & XmlKnownDtds.Rss091) != XmlKnownDtds.None && relativeUri == XmlPreloadedResolver.s_rss091_Dtd[0].publicId)
				{
					return new Uri(relativeUri, UriKind.Relative);
				}
			}
			return base.ResolveUri(baseUri, relativeUri);
		}

		/// <summary>Maps a URI to an object that contains the actual resource.</summary>
		/// <param name="absoluteUri">The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)" />.</param>
		/// <param name="role">The current version of the .NET Framework for Silverlight does not use this parameter when resolving URIs. This parameter is provided for future extensibility purposes. For example, this parameter can be mapped to the xlink:role and used as an implementation-specific argument in other scenarios.</param>
		/// <param name="ofObjectToReturn">The type of object to return. The <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> supports <see cref="T:System.IO.Stream" /> objects and <see cref="T:System.IO.TextReader" /> objects for URIs that were added as <see langword="String" />. If the requested type is not supported by the resolver, an exception will be thrown. Use the <see cref="M:System.Xml.Resolvers.XmlPreloadedResolver.SupportsType(System.Uri,System.Type)" /> method to determine whether a certain <see langword="Type" /> is supported by this resolver.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> or <see cref="T:System.IO.TextReader" /> object that corresponds to the actual source.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="absoluteUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Xml.XmlException">Cannot resolve URI passed in <paramref name="absoluteUri" />.-or-
		///         <paramref name="ofObjectToReturn" /> is not of a supported type.</exception>
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (!this._mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				if (this._fallbackResolver != null)
				{
					return this._fallbackResolver.GetEntity(absoluteUri, role, ofObjectToReturn);
				}
				throw new XmlException(SR.Format("Cannot resolve '{0}'.", absoluteUri.ToString()));
			}
			else
			{
				if (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream) || ofObjectToReturn == typeof(object))
				{
					return preloadedData.AsStream();
				}
				if (ofObjectToReturn == typeof(TextReader))
				{
					return preloadedData.AsTextReader();
				}
				throw new XmlException("Object type is not supported.");
			}
		}

		/// <summary>Sets the credentials that are used to authenticate the underlying <see cref="T:System.Net.WebRequest" />.</summary>
		/// <returns>The credentials that are used to authenticate the underlying web request.</returns>
		public override ICredentials Credentials
		{
			set
			{
				if (this._fallbackResolver != null)
				{
					this._fallbackResolver.Credentials = value;
				}
			}
		}

		/// <summary>Determines whether the resolver supports other <see cref="T:System.Type" />s than just <see cref="T:System.IO.Stream" />.</summary>
		/// <param name="absoluteUri">The absolute URI to check.</param>
		/// <param name="type">The <see cref="T:System.Type" /> to return.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Type" /> is supported; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> is <see langword="null" />.</exception>
		public override bool SupportsType(Uri absoluteUri, Type type)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (this._mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				return preloadedData.SupportsType(type);
			}
			if (this._fallbackResolver != null)
			{
				return this._fallbackResolver.SupportsType(absoluteUri, type);
			}
			return base.SupportsType(absoluteUri, type);
		}

		/// <summary>Adds a byte array to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store and maps it to a URI. If the store already contains a mapping for the same URI, the existing mapping is overridden.</summary>
		/// <param name="uri">The URI of the data that is being added to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store.</param>
		/// <param name="value">A byte array with the data that corresponds to the provided URI.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> or <paramref name="value" /> is <see langword="null" />.</exception>
		public void Add(Uri uri, byte[] value)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(value, 0, value.Length));
		}

		/// <summary>Adds a byte array to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store and maps it to a URI. If the store already contains a mapping for the same URI, the existing mapping is overridden.</summary>
		/// <param name="uri">The URI of the data that is being added to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store.</param>
		/// <param name="value">A byte array with the data that corresponds to the provided URI.</param>
		/// <param name="offset">The offset in the provided byte array where the data starts.</param>
		/// <param name="count">The number of bytes to read from the byte array, starting at the provided offset.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> or <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="offset" /> or <paramref name="count" /> is less than 0.-or-The length of the <paramref name="value" /> minus <paramref name="offset" /> is less than <paramref name="count." /></exception>
		public void Add(Uri uri, byte[] value, int offset, int count)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (value.Length - offset < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(value, offset, count));
		}

		/// <summary>Adds a <see cref="T:System.IO.Stream" /> to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store and maps it to a URI. If the store already contains a mapping for the same URI, the existing mapping is overridden.</summary>
		/// <param name="uri">The URI of the data that is being added to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store.</param>
		/// <param name="value">A <see cref="T:System.IO.Stream" /> with the data that corresponds to the provided URI.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> or <paramref name="value" /> is <see langword="null" />.</exception>
		public void Add(Uri uri, Stream value)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			checked
			{
				if (value.CanSeek)
				{
					int num = (int)value.Length;
					byte[] array = new byte[num];
					value.Read(array, 0, num);
					this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(array));
					return;
				}
				MemoryStream memoryStream = new MemoryStream();
				byte[] array2 = new byte[4096];
				int count;
				while ((count = value.Read(array2, 0, array2.Length)) > 0)
				{
					memoryStream.Write(array2, 0, count);
				}
				int num2 = (int)memoryStream.Position;
				byte[] array3 = new byte[num2];
				Array.Copy(memoryStream.ToArray(), array3, num2);
				this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(array3));
			}
		}

		/// <summary>Adds a string with preloaded data to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store and maps it to a URI. If the store already contains a mapping for the same URI, the existing mapping is overridden.</summary>
		/// <param name="uri">The URI of the data that is being added to the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store.</param>
		/// <param name="value">A <see langword="String" /> with the data that corresponds to the provided URI.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> or <paramref name="value" /> is <see langword="null" />.</exception>
		public void Add(Uri uri, string value)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Add(uri, new XmlPreloadedResolver.StringData(value));
		}

		/// <summary>Gets a collection of preloaded URIs.</summary>
		/// <returns>The collection of preloaded URIs.</returns>
		public IEnumerable<Uri> PreloadedUris
		{
			get
			{
				return this._mappings.Keys;
			}
		}

		/// <summary>Removes the data that corresponds to the URI from the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" />.</summary>
		/// <param name="uri">The URI of the data that should be removed from the <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> store.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="uri" /> is <see langword="null" />.</exception>
		public void Remove(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this._mappings.Remove(uri);
		}

		private void Add(Uri uri, XmlPreloadedResolver.PreloadedData data)
		{
			if (this._mappings.ContainsKey(uri))
			{
				this._mappings[uri] = data;
				return;
			}
			this._mappings.Add(uri, data);
		}

		private void AddKnownDtd(XmlPreloadedResolver.XmlKnownDtdData[] dtdSet)
		{
			foreach (XmlPreloadedResolver.XmlKnownDtdData xmlKnownDtdData in dtdSet)
			{
				this._mappings.Add(new Uri(xmlKnownDtdData.publicId, UriKind.RelativeOrAbsolute), xmlKnownDtdData);
				this._mappings.Add(new Uri(xmlKnownDtdData.systemId, UriKind.RelativeOrAbsolute), xmlKnownDtdData);
			}
		}

		/// <summary>Asynchronously maps a URI to an object that contains the actual resource.</summary>
		/// <param name="absoluteUri">The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)" />.</param>
		/// <param name="role">The current version of the .NET Framework for Silverlight does not use this parameter when resolving URIs. This parameter is provided for future extensibility purposes. For example, this parameter can be mapped to the xlink:role and used as an implementation-specific argument in other scenarios.</param>
		/// <param name="ofObjectToReturn">The type of object to return. The <see cref="T:System.Xml.Resolvers.XmlPreloadedResolver" /> supports <see cref="T:System.IO.Stream" /> objects and <see cref="T:System.IO.TextReader" /> objects for URIs that were added as <see langword="String" />. If the requested type is not supported by the resolver, an exception will be thrown. Use the <see cref="M:System.Xml.Resolvers.XmlPreloadedResolver.SupportsType(System.Uri,System.Type)" /> method to determine whether a certain <see langword="Type" /> is supported by this resolver.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> or <see cref="T:System.IO.TextReader" /> object that corresponds to the actual source.</returns>
		public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (!this._mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				if (this._fallbackResolver != null)
				{
					return this._fallbackResolver.GetEntityAsync(absoluteUri, role, ofObjectToReturn);
				}
				throw new XmlException(SR.Format("Cannot resolve '{0}'.", absoluteUri.ToString()));
			}
			else
			{
				if (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream) || ofObjectToReturn == typeof(object))
				{
					return Task.FromResult<object>(preloadedData.AsStream());
				}
				if (ofObjectToReturn == typeof(TextReader))
				{
					return Task.FromResult<object>(preloadedData.AsTextReader());
				}
				throw new XmlException("Object type is not supported.");
			}
		}

		private XmlResolver _fallbackResolver;

		private Dictionary<Uri, XmlPreloadedResolver.PreloadedData> _mappings;

		private XmlKnownDtds _preloadedDtds;

		private static XmlPreloadedResolver.XmlKnownDtdData[] s_xhtml10_Dtd = new XmlPreloadedResolver.XmlKnownDtdData[]
		{
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Strict//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd", "xhtml1-strict.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Transitional//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd", "xhtml1-transitional.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Frameset//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd", "xhtml1-frameset.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Latin 1 for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent", "xhtml-lat1.ent"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Symbols for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent", "xhtml-symbol.ent"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Special for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent", "xhtml-special.ent")
		};

		private static XmlPreloadedResolver.XmlKnownDtdData[] s_rss091_Dtd = new XmlPreloadedResolver.XmlKnownDtdData[]
		{
			new XmlPreloadedResolver.XmlKnownDtdData("-//Netscape Communications//DTD RSS 0.91//EN", "http://my.netscape.com/publish/formats/rss-0.91.dtd", "rss-0.91.dtd")
		};

		private abstract class PreloadedData
		{
			internal abstract Stream AsStream();

			internal virtual TextReader AsTextReader()
			{
				throw new XmlException("Object type is not supported.");
			}

			internal virtual bool SupportsType(Type type)
			{
				return type == null || type == typeof(Stream);
			}
		}

		private class XmlKnownDtdData : XmlPreloadedResolver.PreloadedData
		{
			internal XmlKnownDtdData(string publicId, string systemId, string resourceName)
			{
				this.publicId = publicId;
				this.systemId = systemId;
				this._resourceName = resourceName;
			}

			internal override Stream AsStream()
			{
				return base.GetType().Assembly.GetManifestResourceStream(this._resourceName);
			}

			internal string publicId;

			internal string systemId;

			private string _resourceName;
		}

		private class ByteArrayChunk : XmlPreloadedResolver.PreloadedData
		{
			internal ByteArrayChunk(byte[] array) : this(array, 0, array.Length)
			{
			}

			internal ByteArrayChunk(byte[] array, int offset, int length)
			{
				this._array = array;
				this._offset = offset;
				this._length = length;
			}

			internal override Stream AsStream()
			{
				return new MemoryStream(this._array, this._offset, this._length);
			}

			private byte[] _array;

			private int _offset;

			private int _length;
		}

		private class StringData : XmlPreloadedResolver.PreloadedData
		{
			internal StringData(string str)
			{
				this._str = str;
			}

			internal override Stream AsStream()
			{
				return new MemoryStream(Encoding.Unicode.GetBytes(this._str));
			}

			internal override TextReader AsTextReader()
			{
				return new StringReader(this._str);
			}

			internal override bool SupportsType(Type type)
			{
				return type == typeof(TextReader) || base.SupportsType(type);
			}

			private string _str;
		}
	}
}
