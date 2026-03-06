using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WebSocketSharp.Net
{
	[Serializable]
	public class CookieCollection : ICollection<Cookie>, IEnumerable<Cookie>, IEnumerable
	{
		public CookieCollection()
		{
			this._list = new List<Cookie>();
			this._sync = ((ICollection)this._list).SyncRoot;
		}

		internal IList<Cookie> List
		{
			get
			{
				return this._list;
			}
		}

		internal IEnumerable<Cookie> Sorted
		{
			get
			{
				List<Cookie> list = new List<Cookie>(this._list);
				bool flag = list.Count > 1;
				if (flag)
				{
					list.Sort(new Comparison<Cookie>(CookieCollection.compareForSorted));
				}
				return list;
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._readOnly;
			}
			internal set
			{
				this._readOnly = value;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public Cookie this[int index]
		{
			get
			{
				bool flag = index < 0 || index >= this._list.Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._list[index];
			}
		}

		public Cookie this[string name]
		{
			get
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
				foreach (Cookie cookie in this.Sorted)
				{
					bool flag2 = cookie.Name.Equals(name, comparisonType);
					if (flag2)
					{
						return cookie;
					}
				}
				return null;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._sync;
			}
		}

		private void add(Cookie cookie)
		{
			int num = this.search(cookie);
			bool flag = num == -1;
			if (flag)
			{
				this._list.Add(cookie);
			}
			else
			{
				this._list[num] = cookie;
			}
		}

		private static int compareForSort(Cookie x, Cookie y)
		{
			return x.Name.Length + x.Value.Length - (y.Name.Length + y.Value.Length);
		}

		private static int compareForSorted(Cookie x, Cookie y)
		{
			int num = x.Version - y.Version;
			return (num != 0) ? num : (((num = x.Name.CompareTo(y.Name)) != 0) ? num : (y.Path.Length - x.Path.Length));
		}

		private static CookieCollection parseRequest(string value)
		{
			CookieCollection cookieCollection = new CookieCollection();
			Cookie cookie = null;
			int num = 0;
			StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
			List<string> list = value.SplitHeaderValue(new char[]
			{
				',',
				';'
			}).ToList<string>();
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i].Trim();
				bool flag = text.Length == 0;
				if (!flag)
				{
					int num2 = text.IndexOf('=');
					bool flag2 = num2 == -1;
					if (flag2)
					{
						bool flag3 = cookie == null;
						if (!flag3)
						{
							bool flag4 = text.Equals("$port", comparisonType);
							if (flag4)
							{
								cookie.Port = "\"\"";
							}
						}
					}
					else
					{
						bool flag5 = num2 == 0;
						if (flag5)
						{
							bool flag6 = cookie != null;
							if (flag6)
							{
								cookieCollection.add(cookie);
								cookie = null;
							}
						}
						else
						{
							string text2 = text.Substring(0, num2).TrimEnd(new char[]
							{
								' '
							});
							string text3 = (num2 < text.Length - 1) ? text.Substring(num2 + 1).TrimStart(new char[]
							{
								' '
							}) : string.Empty;
							bool flag7 = text2.Equals("$version", comparisonType);
							if (flag7)
							{
								bool flag8 = text3.Length == 0;
								if (!flag8)
								{
									int num3;
									bool flag9 = !int.TryParse(text3.Unquote(), out num3);
									if (!flag9)
									{
										num = num3;
									}
								}
							}
							else
							{
								bool flag10 = text2.Equals("$path", comparisonType);
								if (flag10)
								{
									bool flag11 = cookie == null;
									if (!flag11)
									{
										bool flag12 = text3.Length == 0;
										if (!flag12)
										{
											cookie.Path = text3;
										}
									}
								}
								else
								{
									bool flag13 = text2.Equals("$domain", comparisonType);
									if (flag13)
									{
										bool flag14 = cookie == null;
										if (!flag14)
										{
											bool flag15 = text3.Length == 0;
											if (!flag15)
											{
												cookie.Domain = text3;
											}
										}
									}
									else
									{
										bool flag16 = text2.Equals("$port", comparisonType);
										if (flag16)
										{
											bool flag17 = cookie == null;
											if (!flag17)
											{
												bool flag18 = text3.Length == 0;
												if (!flag18)
												{
													cookie.Port = text3;
												}
											}
										}
										else
										{
											bool flag19 = cookie != null;
											if (flag19)
											{
												cookieCollection.add(cookie);
											}
											bool flag20 = !Cookie.TryCreate(text2, text3, out cookie);
											if (!flag20)
											{
												bool flag21 = num != 0;
												if (flag21)
												{
													cookie.Version = num;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag22 = cookie != null;
			if (flag22)
			{
				cookieCollection.add(cookie);
			}
			return cookieCollection;
		}

		private static CookieCollection parseResponse(string value)
		{
			CookieCollection cookieCollection = new CookieCollection();
			Cookie cookie = null;
			StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
			List<string> list = value.SplitHeaderValue(new char[]
			{
				',',
				';'
			}).ToList<string>();
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i].Trim();
				bool flag = text.Length == 0;
				if (!flag)
				{
					int num = text.IndexOf('=');
					bool flag2 = num == -1;
					if (flag2)
					{
						bool flag3 = cookie == null;
						if (!flag3)
						{
							bool flag4 = text.Equals("port", comparisonType);
							if (flag4)
							{
								cookie.Port = "\"\"";
							}
							else
							{
								bool flag5 = text.Equals("discard", comparisonType);
								if (flag5)
								{
									cookie.Discard = true;
								}
								else
								{
									bool flag6 = text.Equals("secure", comparisonType);
									if (flag6)
									{
										cookie.Secure = true;
									}
									else
									{
										bool flag7 = text.Equals("httponly", comparisonType);
										if (flag7)
										{
											cookie.HttpOnly = true;
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag8 = num == 0;
						if (flag8)
						{
							bool flag9 = cookie != null;
							if (flag9)
							{
								cookieCollection.add(cookie);
								cookie = null;
							}
						}
						else
						{
							string text2 = text.Substring(0, num).TrimEnd(new char[]
							{
								' '
							});
							string text3 = (num < text.Length - 1) ? text.Substring(num + 1).TrimStart(new char[]
							{
								' '
							}) : string.Empty;
							bool flag10 = text2.Equals("version", comparisonType);
							if (flag10)
							{
								bool flag11 = cookie == null;
								if (!flag11)
								{
									bool flag12 = text3.Length == 0;
									if (!flag12)
									{
										int version;
										bool flag13 = !int.TryParse(text3.Unquote(), out version);
										if (!flag13)
										{
											cookie.Version = version;
										}
									}
								}
							}
							else
							{
								bool flag14 = text2.Equals("expires", comparisonType);
								if (flag14)
								{
									bool flag15 = text3.Length == 0;
									if (!flag15)
									{
										bool flag16 = i == list.Count - 1;
										if (flag16)
										{
											break;
										}
										i++;
										bool flag17 = cookie == null;
										if (!flag17)
										{
											bool flag18 = cookie.Expires != DateTime.MinValue;
											if (!flag18)
											{
												StringBuilder stringBuilder = new StringBuilder(text3, 32);
												stringBuilder.AppendFormat(", {0}", list[i].Trim());
												DateTime dateTime;
												bool flag19 = !DateTime.TryParseExact(stringBuilder.ToString(), new string[]
												{
													"ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'",
													"r"
												}, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime);
												if (!flag19)
												{
													cookie.Expires = dateTime.ToLocalTime();
												}
											}
										}
									}
								}
								else
								{
									bool flag20 = text2.Equals("max-age", comparisonType);
									if (flag20)
									{
										bool flag21 = cookie == null;
										if (!flag21)
										{
											bool flag22 = text3.Length == 0;
											if (!flag22)
											{
												int maxAge;
												bool flag23 = !int.TryParse(text3.Unquote(), out maxAge);
												if (!flag23)
												{
													cookie.MaxAge = maxAge;
												}
											}
										}
									}
									else
									{
										bool flag24 = text2.Equals("path", comparisonType);
										if (flag24)
										{
											bool flag25 = cookie == null;
											if (!flag25)
											{
												bool flag26 = text3.Length == 0;
												if (!flag26)
												{
													cookie.Path = text3;
												}
											}
										}
										else
										{
											bool flag27 = text2.Equals("domain", comparisonType);
											if (flag27)
											{
												bool flag28 = cookie == null;
												if (!flag28)
												{
													bool flag29 = text3.Length == 0;
													if (!flag29)
													{
														cookie.Domain = text3;
													}
												}
											}
											else
											{
												bool flag30 = text2.Equals("port", comparisonType);
												if (flag30)
												{
													bool flag31 = cookie == null;
													if (!flag31)
													{
														bool flag32 = text3.Length == 0;
														if (!flag32)
														{
															cookie.Port = text3;
														}
													}
												}
												else
												{
													bool flag33 = text2.Equals("comment", comparisonType);
													if (flag33)
													{
														bool flag34 = cookie == null;
														if (!flag34)
														{
															bool flag35 = text3.Length == 0;
															if (!flag35)
															{
																cookie.Comment = CookieCollection.urlDecode(text3, Encoding.UTF8);
															}
														}
													}
													else
													{
														bool flag36 = text2.Equals("commenturl", comparisonType);
														if (flag36)
														{
															bool flag37 = cookie == null;
															if (!flag37)
															{
																bool flag38 = text3.Length == 0;
																if (!flag38)
																{
																	cookie.CommentUri = text3.Unquote().ToUri();
																}
															}
														}
														else
														{
															bool flag39 = text2.Equals("samesite", comparisonType);
															if (flag39)
															{
																bool flag40 = cookie == null;
																if (!flag40)
																{
																	bool flag41 = text3.Length == 0;
																	if (!flag41)
																	{
																		cookie.SameSite = text3.Unquote();
																	}
																}
															}
															else
															{
																bool flag42 = cookie != null;
																if (flag42)
																{
																	cookieCollection.add(cookie);
																}
																Cookie.TryCreate(text2, text3, out cookie);
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag43 = cookie != null;
			if (flag43)
			{
				cookieCollection.add(cookie);
			}
			return cookieCollection;
		}

		private int search(Cookie cookie)
		{
			for (int i = this._list.Count - 1; i >= 0; i--)
			{
				bool flag = this._list[i].EqualsWithoutValue(cookie);
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		private static string urlDecode(string s, Encoding encoding)
		{
			bool flag = s.IndexOfAny(new char[]
			{
				'%',
				'+'
			}) == -1;
			string result;
			if (flag)
			{
				result = s;
			}
			else
			{
				try
				{
					result = HttpUtility.UrlDecode(s, encoding);
				}
				catch
				{
					result = null;
				}
			}
			return result;
		}

		internal static CookieCollection Parse(string value, bool response)
		{
			CookieCollection result;
			try
			{
				result = (response ? CookieCollection.parseResponse(value) : CookieCollection.parseRequest(value));
			}
			catch (Exception innerException)
			{
				throw new CookieException("It could not be parsed.", innerException);
			}
			return result;
		}

		internal void SetOrRemove(Cookie cookie)
		{
			int num = this.search(cookie);
			bool flag = num == -1;
			if (flag)
			{
				bool expired = cookie.Expired;
				if (!expired)
				{
					this._list.Add(cookie);
				}
			}
			else
			{
				bool expired2 = cookie.Expired;
				if (expired2)
				{
					this._list.RemoveAt(num);
				}
				else
				{
					this._list[num] = cookie;
				}
			}
		}

		internal void SetOrRemove(CookieCollection cookies)
		{
			foreach (Cookie orRemove in cookies._list)
			{
				this.SetOrRemove(orRemove);
			}
		}

		internal void Sort()
		{
			bool flag = this._list.Count > 1;
			if (flag)
			{
				this._list.Sort(new Comparison<Cookie>(CookieCollection.compareForSort));
			}
		}

		public void Add(Cookie cookie)
		{
			bool readOnly = this._readOnly;
			if (readOnly)
			{
				string message = "The collection is read-only.";
				throw new InvalidOperationException(message);
			}
			bool flag = cookie == null;
			if (flag)
			{
				throw new ArgumentNullException("cookie");
			}
			this.add(cookie);
		}

		public void Add(CookieCollection cookies)
		{
			bool readOnly = this._readOnly;
			if (readOnly)
			{
				string message = "The collection is read-only.";
				throw new InvalidOperationException(message);
			}
			bool flag = cookies == null;
			if (flag)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (Cookie cookie in cookies._list)
			{
				this.add(cookie);
			}
		}

		public void Clear()
		{
			bool readOnly = this._readOnly;
			if (readOnly)
			{
				string message = "The collection is read-only.";
				throw new InvalidOperationException(message);
			}
			this._list.Clear();
		}

		public bool Contains(Cookie cookie)
		{
			bool flag = cookie == null;
			if (flag)
			{
				throw new ArgumentNullException("cookie");
			}
			return this.search(cookie) > -1;
		}

		public void CopyTo(Cookie[] array, int index)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			bool flag2 = index < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", "Less than zero.");
			}
			bool flag3 = array.Length - index < this._list.Count;
			if (flag3)
			{
				string message = "The available space of the array is not enough to copy to.";
				throw new ArgumentException(message);
			}
			this._list.CopyTo(array, index);
		}

		public IEnumerator<Cookie> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		public bool Remove(Cookie cookie)
		{
			bool readOnly = this._readOnly;
			if (readOnly)
			{
				string message = "The collection is read-only.";
				throw new InvalidOperationException(message);
			}
			bool flag = cookie == null;
			if (flag)
			{
				throw new ArgumentNullException("cookie");
			}
			int num = this.search(cookie);
			bool flag2 = num == -1;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				this._list.RemoveAt(num);
				result = true;
			}
			return result;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		private List<Cookie> _list;

		private bool _readOnly;

		private object _sync;
	}
}
