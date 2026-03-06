using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.AddressableAssets
{
	public class InvalidKeyException : Exception
	{
		public object Key { get; private set; }

		public Type Type { get; private set; }

		public Addressables.MergeMode? MergeMode { get; }

		public InvalidKeyException(object key) : this(key, typeof(object))
		{
		}

		public InvalidKeyException(object key, Type type)
		{
			this.Key = key;
			this.Type = type;
		}

		internal InvalidKeyException(object key, Type type, AddressablesImpl addr)
		{
			this.Key = key;
			this.Type = type;
			this.m_Addressables = addr;
		}

		public InvalidKeyException(object key, Type type, Addressables.MergeMode mergeMode)
		{
			this.Key = key;
			this.Type = type;
			this.MergeMode = new Addressables.MergeMode?(mergeMode);
		}

		internal InvalidKeyException(object key, Type type, Addressables.MergeMode mergeMode, AddressablesImpl addr)
		{
			this.Key = key;
			this.Type = type;
			this.MergeMode = new Addressables.MergeMode?(mergeMode);
			this.m_Addressables = addr;
		}

		public InvalidKeyException()
		{
		}

		public InvalidKeyException(string message) : base(message)
		{
		}

		public InvalidKeyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidKeyException(SerializationInfo message, StreamingContext context) : base(message, context)
		{
		}

		internal string FormatMessage(InvalidKeyException.Format format, string foundWithTypeString = null)
		{
			switch (format)
			{
			case InvalidKeyException.Format.StandardMessage:
				return string.Format("{0}, Key={1}, Type={2}", base.Message, this.Key.ToString(), this.Type.FullName);
			case InvalidKeyException.Format.MultipleTypesRequested:
			{
				IEnumerable enumerable = this.Key as IEnumerable;
				string text = null;
				foreach (object obj in enumerable)
				{
					if (text == null)
					{
						text = obj.ToString();
					}
					else
					{
						text = text + ", " + obj.ToString();
					}
				}
				return string.Format("{0} Enumerable key contains multiple Types. {1}, all Keys are expected to be strings", base.Message, text);
			}
			case InvalidKeyException.Format.NoLocation:
				return string.Format("{0} No Location found for Key={1}", base.Message, this.Key.ToString());
			case InvalidKeyException.Format.TypeMismatch:
				return string.Format("{0} No Asset found for Key={1} with Type={2}. Key exists as Type={3}, which is not assignable from the requested Type={2}", new object[]
				{
					base.Message,
					this.Key.ToString(),
					this.Type.FullName,
					foundWithTypeString
				});
			case InvalidKeyException.Format.MultipleTypeMismatch:
				return string.Format("{0} No Asset found for Key={1} with Type={2}. Key exists as multiple Types={3}, which is not assignable from the requested Type={2}", new object[]
				{
					base.Message,
					this.Key.ToString(),
					this.Type.FullName,
					foundWithTypeString
				});
			}
			throw new ArgumentOutOfRangeException("format", format, null);
		}

		internal string FormatMergeModeMessage(InvalidKeyException.Format format, string keysAvailable = null, string keysUnavailable = null, string typeString = null)
		{
			switch (format)
			{
			case InvalidKeyException.Format.NoMergeMode:
				return string.Format("{0} No MergeMode is set to merge the multiple keys requested. {1}, Type={2}", base.Message, this.GetKeyString(), this.Type.FullName);
			case InvalidKeyException.Format.NoLocation:
				return string.Format("\nNo Location found for Key={0}", (keysUnavailable == null) ? this.GetKeyString() : keysUnavailable);
			case InvalidKeyException.Format.MergeModeBase:
				return string.Format("{0} No {1} of Assets between {2} with Type={3}", new object[]
				{
					base.Message,
					(this.MergeMode != null) ? this.MergeMode.Value : Addressables.MergeMode.None,
					this.GetKeyString(),
					this.Type.FullName
				});
			case InvalidKeyException.Format.UnionAvailableForKeys:
				return string.Format("\nUnion of Type={0} found with {1}", typeString, keysAvailable);
			case InvalidKeyException.Format.UnionAvailableForKeysWithoutOther:
				return string.Format("\nUnion of Type={0} found with {1}. Without {2}", typeString, keysAvailable, keysUnavailable);
			case InvalidKeyException.Format.IntersectionAvailable:
				return string.Format("\nAn Intersection exists for Type={0}", typeString);
			case InvalidKeyException.Format.KeyAvailableAsType:
				return string.Format("\nType={0} exists for {1}", typeString, keysAvailable);
			}
			throw new ArgumentOutOfRangeException("format", format, null);
		}

		public override string Message
		{
			get
			{
				string text = this.Key as string;
				if (!string.IsNullOrEmpty(text))
				{
					if (this.m_Addressables == null)
					{
						return this.FormatMessage(InvalidKeyException.Format.StandardMessage, null);
					}
					return this.GetMessageForSingleKey(text);
				}
				else
				{
					IEnumerable enumerable = this.Key as IEnumerable;
					if (enumerable == null)
					{
						return this.FormatMessage(InvalidKeyException.Format.StandardMessage, null);
					}
					int num = 0;
					List<string> list = new List<string>();
					HashSet<string> hashSet = new HashSet<string>();
					foreach (object obj in enumerable)
					{
						num++;
						hashSet.Add(obj.GetType().ToString());
						if (obj is string)
						{
							list.Add(obj as string);
						}
					}
					if (this.MergeMode == null)
					{
						string csvstring = InvalidKeyException.GetCSVString(list, "Key=", "Keys=");
						this.FormatMergeModeMessage(InvalidKeyException.Format.NoMergeMode, null, null, null);
						return string.Format("{0} No MergeMode is set to merge the multiple keys requested. {1}, Type={2}", base.Message, csvstring, this.Type);
					}
					if (num != list.Count)
					{
						string csvstring2 = InvalidKeyException.GetCSVString(hashSet, "Type=", "Types=");
						return this.FormatMessage(InvalidKeyException.Format.MultipleTypesRequested, csvstring2);
					}
					if (num == 1)
					{
						return this.GetMessageForSingleKey(list[0]);
					}
					return this.GetMessageforMergeKeys(list);
				}
			}
		}

		private string GetMessageForSingleKey(string keyString)
		{
			HashSet<Type> typesForKey = this.GetTypesForKey(keyString);
			if (typesForKey.Count == 0)
			{
				return this.FormatNotFoundMessage(keyString);
			}
			if (typesForKey.Count == 1)
			{
				return this.FormatTypeNotAssignableMessage(keyString, typesForKey);
			}
			return this.FormatMultipleAssignableTypesMessage(keyString, typesForKey);
		}

		private string FormatNotFoundMessage(string keyString)
		{
			return this.FormatMessage(InvalidKeyException.Format.NoLocation, null);
		}

		private string FormatTypeNotAssignableMessage(string keyString, HashSet<Type> typesAvailableForKey)
		{
			Type type = null;
			using (HashSet<Type>.Enumerator enumerator = typesAvailableForKey.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					type = enumerator.Current;
				}
			}
			if (type == null)
			{
				return this.FormatMessage(InvalidKeyException.Format.StandardMessage, null);
			}
			return this.FormatMessage(InvalidKeyException.Format.TypeMismatch, type.ToString());
		}

		private string FormatMultipleAssignableTypesMessage(string keyString, HashSet<Type> typesAvailableForKey)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			int num = 0;
			foreach (Type type in typesAvailableForKey)
			{
				num++;
				stringBuilder.Append((num > 1) ? string.Format(", {0}", type) : type.ToString());
			}
			return this.FormatMessage(InvalidKeyException.Format.MultipleTypeMismatch, stringBuilder.ToString());
		}

		private string GetMessageforMergeKeys(List<string> keys)
		{
			StringBuilder stringBuilder = new StringBuilder(this.FormatMergeModeMessage(InvalidKeyException.Format.MergeModeBase, null, null, null));
			Addressables.MergeMode? mergeMode = this.MergeMode;
			if (mergeMode != null)
			{
				switch (mergeMode.GetValueOrDefault())
				{
				case Addressables.MergeMode.None:
					goto IL_242;
				case Addressables.MergeMode.Union:
				{
					Dictionary<Type, List<string>> dictionary = new Dictionary<Type, List<string>>();
					foreach (string text in keys)
					{
						if (!this.GetTypeToKeys(text, dictionary))
						{
							stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.NoLocation, null, text, null));
						}
					}
					using (Dictionary<Type, List<string>>.Enumerator enumerator2 = dictionary.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<Type, List<string>> keyValuePair = enumerator2.Current;
							string csvstring = InvalidKeyException.GetCSVString(keyValuePair.Value, "Key=", "Keys=");
							List<string> list = new List<string>();
							foreach (string item in keys)
							{
								if (!keyValuePair.Value.Contains(item))
								{
									list.Add(item);
								}
							}
							if (list.Count == 0)
							{
								stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.UnionAvailableForKeys, csvstring, null, keyValuePair.Key.ToString()));
							}
							else
							{
								string csvstring2 = InvalidKeyException.GetCSVString(list, "Key=", "Keys=");
								stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.UnionAvailableForKeysWithoutOther, csvstring, csvstring2, keyValuePair.Key.ToString()));
							}
						}
						goto IL_310;
					}
					break;
				}
				case Addressables.MergeMode.Intersection:
					break;
				default:
					goto IL_310;
				}
				bool flag = false;
				Dictionary<Type, List<string>> dictionary2 = new Dictionary<Type, List<string>>();
				foreach (string text2 in keys)
				{
					if (!this.GetTypeToKeys(text2, dictionary2))
					{
						flag = true;
						stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.NoLocation, null, text2, null));
					}
				}
				if (flag)
				{
					goto IL_310;
				}
				using (Dictionary<Type, List<string>>.Enumerator enumerator2 = dictionary2.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<Type, List<string>> keyValuePair2 = enumerator2.Current;
						if (keyValuePair2.Value.Count == keys.Count)
						{
							stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.IntersectionAvailable, null, null, keyValuePair2.Key.ToString()));
						}
					}
					goto IL_310;
				}
				IL_242:
				Dictionary<Type, List<string>> dictionary3 = new Dictionary<Type, List<string>>();
				foreach (string text3 in keys)
				{
					if (!this.GetTypeToKeys(text3, dictionary3))
					{
						stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.NoLocation, null, text3, null));
					}
				}
				foreach (KeyValuePair<Type, List<string>> keyValuePair3 in dictionary3)
				{
					foreach (string keysAvailable in keyValuePair3.Value)
					{
						stringBuilder.Append(this.FormatMergeModeMessage(InvalidKeyException.Format.KeyAvailableAsType, keysAvailable, null, keyValuePair3.Key.ToString()));
					}
				}
			}
			IL_310:
			return stringBuilder.ToString();
		}

		private HashSet<Type> GetTypesForKey(string keyString)
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			using (IEnumerator<IResourceLocator> enumerator = this.m_Addressables.ResourceLocators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IList<IResourceLocation> list;
					if (enumerator.Current.Locate(keyString, null, out list))
					{
						foreach (IResourceLocation resourceLocation in list)
						{
							hashSet.Add(resourceLocation.ResourceType);
						}
					}
				}
			}
			return hashSet;
		}

		private bool GetTypeToKeys(string key, Dictionary<Type, List<string>> typeToKeys)
		{
			HashSet<Type> typesForKey = this.GetTypesForKey(key);
			if (typesForKey.Count == 0)
			{
				return false;
			}
			foreach (Type key2 in typesForKey)
			{
				List<string> list;
				if (!typeToKeys.TryGetValue(key2, out list))
				{
					typeToKeys.Add(key2, new List<string>
					{
						key
					});
				}
				else
				{
					list.Add(key);
				}
			}
			return true;
		}

		internal string GetKeyString()
		{
			if (this.Key is string)
			{
				string str = "Key=";
				object key = this.Key;
				return str + ((key != null) ? key.ToString() : null);
			}
			IEnumerable enumerable = this.Key as IEnumerable;
			if (enumerable != null)
			{
				return InvalidKeyException.GetCSVString(enumerable, "Key=", "Keys=");
			}
			return this.Key.ToString();
		}

		internal static string GetCSVString(IEnumerable enumerator, string prefixSingle, string prefixPlural)
		{
			StringBuilder stringBuilder = new StringBuilder(prefixPlural);
			int num = 0;
			foreach (object obj in enumerator)
			{
				num++;
				stringBuilder.Append((num > 1) ? string.Format(", {0}", obj) : obj);
			}
			if (num == 1 && !string.IsNullOrEmpty(prefixPlural) && !string.IsNullOrEmpty(prefixSingle))
			{
				stringBuilder.Replace(prefixPlural, prefixSingle);
			}
			return stringBuilder.ToString();
		}

		private AddressablesImpl m_Addressables;

		internal const string BaseInvalidKeyMessageFormat = "{0}, Key={1}, Type={2}";

		internal const string NoLocationMessageFormat = "{0} No Location found for Key={1}";

		internal const string MultipleTypeMismatchMessageFormat = "{0} No Asset found for Key={1} with Type={2}. Key exists as multiple Types={3}, which is not assignable from the requested Type={2}";

		internal const string TypeMismatchMessageFormat = "{0} No Asset found for Key={1} with Type={2}. Key exists as Type={3}, which is not assignable from the requested Type={2}";

		internal const string MultipleTypesMessageFormat = "{0} Enumerable key contains multiple Types. {1}, all Keys are expected to be strings";

		internal const string MergeModeNoLocationMessageFormat = "\nNo Location found for Key={0}";

		internal const string NoMergeModeMessageFormat = "{0} No MergeMode is set to merge the multiple keys requested. {1}, Type={2}";

		internal const string MergeModeBaseMessageFormat = "{0} No {1} of Assets between {2} with Type={3}";

		internal const string UnionAvailableForKeysMessageFormat = "\nUnion of Type={0} found with {1}";

		internal const string UnionAvailableForKeysWithoutOtherMessageFormat = "\nUnion of Type={0} found with {1}. Without {2}";

		internal const string IntersectionAvailableMessageFormat = "\nAn Intersection exists for Type={0}";

		internal const string KeyAvailableAsTypeMessageFormat = "\nType={0} exists for {1}";

		internal enum Format
		{
			StandardMessage,
			NoMergeMode,
			MultipleTypesRequested,
			NoLocation,
			TypeMismatch,
			MultipleTypeMismatch,
			MergeModeBase,
			UnionAvailableForKeys,
			UnionAvailableForKeysWithoutOther,
			IntersectionAvailable,
			KeyAvailableAsType
		}
	}
}
