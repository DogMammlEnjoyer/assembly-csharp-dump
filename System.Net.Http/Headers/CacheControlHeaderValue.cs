using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the value of the Cache-Control header.</summary>
	public class CacheControlHeaderValue : ICloneable
	{
		/// <summary>Cache-extension tokens, each with an optional assigned value.</summary>
		/// <returns>A collection of cache-extension tokens each with an optional assigned value.</returns>
		public ICollection<NameValueHeaderValue> Extensions
		{
			get
			{
				List<NameValueHeaderValue> result;
				if ((result = this.extensions) == null)
				{
					result = (this.extensions = new List<NameValueHeaderValue>());
				}
				return result;
			}
		}

		/// <summary>The maximum age, specified in seconds, that the HTTP client is willing to accept a response.</summary>
		/// <returns>The time in seconds.</returns>
		public TimeSpan? MaxAge { get; set; }

		/// <summary>Whether an HTTP client is willing to accept a response that has exceeded its expiration time.</summary>
		/// <returns>
		///   <see langword="true" /> if the HTTP client is willing to accept a response that has exceed the expiration time; otherwise, <see langword="false" />.</returns>
		public bool MaxStale { get; set; }

		/// <summary>The maximum time, in seconds, an HTTP client is willing to accept a response that has exceeded its expiration time.</summary>
		/// <returns>The time in seconds.</returns>
		public TimeSpan? MaxStaleLimit { get; set; }

		/// <summary>The freshness lifetime, in seconds, that an HTTP client is willing to accept a response.</summary>
		/// <returns>The time in seconds.</returns>
		public TimeSpan? MinFresh { get; set; }

		/// <summary>Whether the origin server require revalidation of a cache entry on any subsequent use when the cache entry becomes stale.</summary>
		/// <returns>
		///   <see langword="true" /> if the origin server requires revalidation of a cache entry on any subsequent use when the entry becomes stale; otherwise, <see langword="false" />.</returns>
		public bool MustRevalidate { get; set; }

		/// <summary>Whether an HTTP client is willing to accept a cached response.</summary>
		/// <returns>
		///   <see langword="true" /> if the HTTP client is willing to accept a cached response; otherwise, <see langword="false" />.</returns>
		public bool NoCache { get; set; }

		/// <summary>A collection of fieldnames in the "no-cache" directive in a cache-control header field on an HTTP response.</summary>
		/// <returns>A collection of fieldnames.</returns>
		public ICollection<string> NoCacheHeaders
		{
			get
			{
				List<string> result;
				if ((result = this.no_cache_headers) == null)
				{
					result = (this.no_cache_headers = new List<string>());
				}
				return result;
			}
		}

		/// <summary>Whether a cache must not store any part of either the HTTP request mressage or any response.</summary>
		/// <returns>
		///   <see langword="true" /> if a cache must not store any part of either the HTTP request mressage or any response; otherwise, <see langword="false" />.</returns>
		public bool NoStore { get; set; }

		/// <summary>Whether a cache or proxy must not change any aspect of the entity-body.</summary>
		/// <returns>
		///   <see langword="true" /> if a cache or proxy must not change any aspect of the entity-body; otherwise, <see langword="false" />.</returns>
		public bool NoTransform { get; set; }

		/// <summary>Whether a cache should either respond using a cached entry that is consistent with the other constraints of the HTTP request, or respond with a 504 (Gateway Timeout) status.</summary>
		/// <returns>
		///   <see langword="true" /> if a cache should either respond using a cached entry that is consistent with the other constraints of the HTTP request, or respond with a 504 (Gateway Timeout) status; otherwise, <see langword="false" />.</returns>
		public bool OnlyIfCached { get; set; }

		/// <summary>Whether all or part of the HTTP response message is intended for a single user and must not be cached by a shared cache.</summary>
		/// <returns>
		///   <see langword="true" /> if the HTTP response message is intended for a single user and must not be cached by a shared cache; otherwise, <see langword="false" />.</returns>
		public bool Private { get; set; }

		/// <summary>A collection fieldnames in the "private" directive in a cache-control header field on an HTTP response.</summary>
		/// <returns>A collection of fieldnames.</returns>
		public ICollection<string> PrivateHeaders
		{
			get
			{
				List<string> result;
				if ((result = this.private_headers) == null)
				{
					result = (this.private_headers = new List<string>());
				}
				return result;
			}
		}

		/// <summary>Whether the origin server require revalidation of a cache entry on any subsequent use when the cache entry becomes stale for shared user agent caches.</summary>
		/// <returns>
		///   <see langword="true" /> if the origin server requires revalidation of a cache entry on any subsequent use when the entry becomes stale for shared user agent caches; otherwise, <see langword="false" />.</returns>
		public bool ProxyRevalidate { get; set; }

		/// <summary>Whether an HTTP response may be cached by any cache, even if it would normally be non-cacheable or cacheable only within a non- shared cache.</summary>
		/// <returns>
		///   <see langword="true" /> if the HTTP response may be cached by any cache, even if it would normally be non-cacheable or cacheable only within a non- shared cache; otherwise, <see langword="false" />.</returns>
		public bool Public { get; set; }

		/// <summary>The shared maximum age, specified in seconds, in an HTTP response that overrides the "max-age" directive in a cache-control header or an Expires header for a shared cache.</summary>
		/// <returns>The time in seconds.</returns>
		public TimeSpan? SharedMaxAge { get; set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			CacheControlHeaderValue cacheControlHeaderValue = (CacheControlHeaderValue)base.MemberwiseClone();
			if (this.extensions != null)
			{
				cacheControlHeaderValue.extensions = new List<NameValueHeaderValue>();
				foreach (NameValueHeaderValue item in this.extensions)
				{
					cacheControlHeaderValue.extensions.Add(item);
				}
			}
			if (this.no_cache_headers != null)
			{
				cacheControlHeaderValue.no_cache_headers = new List<string>();
				foreach (string item2 in this.no_cache_headers)
				{
					cacheControlHeaderValue.no_cache_headers.Add(item2);
				}
			}
			if (this.private_headers != null)
			{
				cacheControlHeaderValue.private_headers = new List<string>();
				foreach (string item3 in this.private_headers)
				{
					cacheControlHeaderValue.private_headers.Add(item3);
				}
			}
			return cacheControlHeaderValue;
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			CacheControlHeaderValue cacheControlHeaderValue = obj as CacheControlHeaderValue;
			return cacheControlHeaderValue != null && (!(this.MaxAge != cacheControlHeaderValue.MaxAge) && this.MaxStale == cacheControlHeaderValue.MaxStale) && !(this.MaxStaleLimit != cacheControlHeaderValue.MaxStaleLimit) && (!(this.MinFresh != cacheControlHeaderValue.MinFresh) && this.MustRevalidate == cacheControlHeaderValue.MustRevalidate && this.NoCache == cacheControlHeaderValue.NoCache && this.NoStore == cacheControlHeaderValue.NoStore && this.NoTransform == cacheControlHeaderValue.NoTransform && this.OnlyIfCached == cacheControlHeaderValue.OnlyIfCached && this.Private == cacheControlHeaderValue.Private && this.ProxyRevalidate == cacheControlHeaderValue.ProxyRevalidate && this.Public == cacheControlHeaderValue.Public) && !(this.SharedMaxAge != cacheControlHeaderValue.SharedMaxAge) && (this.extensions.SequenceEqual(cacheControlHeaderValue.extensions) && this.no_cache_headers.SequenceEqual(cacheControlHeaderValue.no_cache_headers)) && this.private_headers.SequenceEqual(cacheControlHeaderValue.private_headers);
		}

		/// <summary>Serves as a hash function for a  <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (((((((((((((((29 * 29 + HashCodeCalculator.Calculate<NameValueHeaderValue>(this.extensions)) * 29 + this.MaxAge.GetHashCode()) * 29 + this.MaxStale.GetHashCode()) * 29 + this.MaxStaleLimit.GetHashCode()) * 29 + this.MinFresh.GetHashCode()) * 29 + this.MustRevalidate.GetHashCode()) * 29 + HashCodeCalculator.Calculate<string>(this.no_cache_headers)) * 29 + this.NoCache.GetHashCode()) * 29 + this.NoStore.GetHashCode()) * 29 + this.NoTransform.GetHashCode()) * 29 + this.OnlyIfCached.GetHashCode()) * 29 + this.Private.GetHashCode()) * 29 + HashCodeCalculator.Calculate<string>(this.private_headers)) * 29 + this.ProxyRevalidate.GetHashCode()) * 29 + this.Public.GetHashCode()) * 29 + this.SharedMaxAge.GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents cache-control header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid cache-control header value information.</exception>
		public static CacheControlHeaderValue Parse(string input)
		{
			CacheControlHeaderValue result;
			if (CacheControlHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out CacheControlHeaderValue parsedValue)
		{
			parsedValue = null;
			if (input == null)
			{
				return true;
			}
			CacheControlHeaderValue cacheControlHeaderValue = new CacheControlHeaderValue();
			Lexer lexer = new Lexer(input);
			Token token;
			for (;;)
			{
				token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					break;
				}
				string stringValue = lexer.GetStringValue(token);
				bool flag = false;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(stringValue);
				if (num <= 1922561311U)
				{
					TimeSpan? timeSpan;
					if (num <= 719568158U)
					{
						if (num != 129047354U)
						{
							if (num != 412259456U)
							{
								if (num != 719568158U)
								{
									goto IL_3B1;
								}
								if (!(stringValue == "no-store"))
								{
									goto IL_3B1;
								}
								cacheControlHeaderValue.NoStore = true;
								goto IL_40A;
							}
							else if (!(stringValue == "s-maxage"))
							{
								goto IL_3B1;
							}
						}
						else if (!(stringValue == "min-fresh"))
						{
							goto IL_3B1;
						}
					}
					else if (num != 962188105U)
					{
						if (num != 1657474316U)
						{
							if (num != 1922561311U)
							{
								goto IL_3B1;
							}
							if (!(stringValue == "max-age"))
							{
								goto IL_3B1;
							}
						}
						else
						{
							if (!(stringValue == "private"))
							{
								goto IL_3B1;
							}
							goto IL_2FE;
						}
					}
					else
					{
						if (!(stringValue == "max-stale"))
						{
							goto IL_3B1;
						}
						cacheControlHeaderValue.MaxStale = true;
						token = lexer.Scan(false);
						if (token != Token.Type.SeparatorEqual)
						{
							flag = true;
							goto IL_40A;
						}
						token = lexer.Scan(false);
						if (token != Token.Type.Token)
						{
							return false;
						}
						timeSpan = lexer.TryGetTimeSpanValue(token);
						if (timeSpan == null)
						{
							return false;
						}
						cacheControlHeaderValue.MaxStaleLimit = timeSpan;
						goto IL_40A;
					}
					token = lexer.Scan(false);
					if (token != Token.Type.SeparatorEqual)
					{
						return false;
					}
					token = lexer.Scan(false);
					if (token != Token.Type.Token)
					{
						return false;
					}
					timeSpan = lexer.TryGetTimeSpanValue(token);
					if (timeSpan == null)
					{
						return false;
					}
					int i = stringValue.Length;
					if (i != 7)
					{
						if (i != 8)
						{
							cacheControlHeaderValue.MinFresh = timeSpan;
						}
						else
						{
							cacheControlHeaderValue.SharedMaxAge = timeSpan;
						}
					}
					else
					{
						cacheControlHeaderValue.MaxAge = timeSpan;
					}
				}
				else if (num <= 2802093227U)
				{
					if (num != 2033558065U)
					{
						if (num != 2154495528U)
						{
							if (num != 2802093227U)
							{
								goto IL_3B1;
							}
							if (!(stringValue == "no-transform"))
							{
								goto IL_3B1;
							}
							cacheControlHeaderValue.NoTransform = true;
						}
						else
						{
							if (!(stringValue == "must-revalidate"))
							{
								goto IL_3B1;
							}
							cacheControlHeaderValue.MustRevalidate = true;
						}
					}
					else
					{
						if (!(stringValue == "proxy-revalidate"))
						{
							goto IL_3B1;
						}
						cacheControlHeaderValue.ProxyRevalidate = true;
					}
				}
				else if (num != 2866772502U)
				{
					if (num != 3432027008U)
					{
						if (num != 3443516981U)
						{
							goto IL_3B1;
						}
						if (!(stringValue == "no-cache"))
						{
							goto IL_3B1;
						}
						goto IL_2FE;
					}
					else
					{
						if (!(stringValue == "public"))
						{
							goto IL_3B1;
						}
						cacheControlHeaderValue.Public = true;
					}
				}
				else
				{
					if (!(stringValue == "only-if-cached"))
					{
						goto IL_3B1;
					}
					cacheControlHeaderValue.OnlyIfCached = true;
				}
				IL_40A:
				if (!flag)
				{
					token = lexer.Scan(false);
				}
				if (token != Token.Type.SeparatorComma)
				{
					goto Block_46;
				}
				continue;
				IL_2FE:
				if (stringValue.Length == 7)
				{
					cacheControlHeaderValue.Private = true;
				}
				else
				{
					cacheControlHeaderValue.NoCache = true;
				}
				token = lexer.Scan(false);
				if (token != Token.Type.SeparatorEqual)
				{
					flag = true;
					goto IL_40A;
				}
				token = lexer.Scan(false);
				if (token != Token.Type.QuotedString)
				{
					return false;
				}
				string[] array = lexer.GetQuotedStringValue(token).Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string item = array[i].Trim(new char[]
					{
						'\t',
						' '
					});
					if (stringValue.Length == 7)
					{
						cacheControlHeaderValue.PrivateHeaders.Add(item);
					}
					else
					{
						cacheControlHeaderValue.NoCache = true;
						cacheControlHeaderValue.NoCacheHeaders.Add(item);
					}
				}
				goto IL_40A;
				IL_3B1:
				string stringValue2 = lexer.GetStringValue(token);
				string value = null;
				token = lexer.Scan(false);
				if (token == Token.Type.SeparatorEqual)
				{
					token = lexer.Scan(false);
					Token.Type kind = token.Kind;
					if (kind - Token.Type.Token > 1)
					{
						return false;
					}
					value = lexer.GetStringValue(token);
				}
				else
				{
					flag = true;
				}
				cacheControlHeaderValue.Extensions.Add(NameValueHeaderValue.Create(stringValue2, value));
				goto IL_40A;
			}
			return false;
			Block_46:
			if (token != Token.Type.End)
			{
				return false;
			}
			parsedValue = cacheControlHeaderValue;
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.NoStore)
			{
				stringBuilder.Append("no-store");
				stringBuilder.Append(", ");
			}
			if (this.NoTransform)
			{
				stringBuilder.Append("no-transform");
				stringBuilder.Append(", ");
			}
			if (this.OnlyIfCached)
			{
				stringBuilder.Append("only-if-cached");
				stringBuilder.Append(", ");
			}
			if (this.Public)
			{
				stringBuilder.Append("public");
				stringBuilder.Append(", ");
			}
			if (this.MustRevalidate)
			{
				stringBuilder.Append("must-revalidate");
				stringBuilder.Append(", ");
			}
			if (this.ProxyRevalidate)
			{
				stringBuilder.Append("proxy-revalidate");
				stringBuilder.Append(", ");
			}
			if (this.NoCache)
			{
				stringBuilder.Append("no-cache");
				if (this.no_cache_headers != null)
				{
					stringBuilder.Append("=\"");
					this.no_cache_headers.ToStringBuilder(stringBuilder);
					stringBuilder.Append("\"");
				}
				stringBuilder.Append(", ");
			}
			if (this.MaxAge != null)
			{
				stringBuilder.Append("max-age=");
				stringBuilder.Append(this.MaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(", ");
			}
			if (this.SharedMaxAge != null)
			{
				stringBuilder.Append("s-maxage=");
				stringBuilder.Append(this.SharedMaxAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(", ");
			}
			if (this.MaxStale)
			{
				stringBuilder.Append("max-stale");
				if (this.MaxStaleLimit != null)
				{
					stringBuilder.Append("=");
					stringBuilder.Append(this.MaxStaleLimit.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				}
				stringBuilder.Append(", ");
			}
			if (this.MinFresh != null)
			{
				stringBuilder.Append("min-fresh=");
				stringBuilder.Append(this.MinFresh.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(", ");
			}
			if (this.Private)
			{
				stringBuilder.Append("private");
				if (this.private_headers != null)
				{
					stringBuilder.Append("=\"");
					this.private_headers.ToStringBuilder(stringBuilder);
					stringBuilder.Append("\"");
				}
				stringBuilder.Append(", ");
			}
			this.extensions.ToStringBuilder(stringBuilder);
			if (stringBuilder.Length > 2 && stringBuilder[stringBuilder.Length - 2] == ',' && stringBuilder[stringBuilder.Length - 1] == ' ')
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			return stringBuilder.ToString();
		}

		private List<NameValueHeaderValue> extensions;

		private List<string> no_cache_headers;

		private List<string> private_headers;
	}
}
