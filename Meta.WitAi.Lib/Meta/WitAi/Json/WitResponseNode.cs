using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Meta.WitAi.Json
{
	public class WitResponseNode
	{
		public virtual void Add(string aKey, WitResponseNode aItem)
		{
		}

		public virtual WitResponseNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual WitResponseNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public virtual string[] ChildNodeNames
		{
			get
			{
				return Array.Empty<string>();
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual void Add(WitResponseNode aItem)
		{
			this.Add("", aItem);
		}

		public virtual WitResponseNode Remove(string aKey)
		{
			return null;
		}

		public virtual WitResponseNode Remove(int aIndex)
		{
			return null;
		}

		public virtual WitResponseNode Remove(WitResponseNode aNode)
		{
			return aNode;
		}

		public virtual IEnumerable<WitResponseNode> Childs
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<WitResponseNode> DeepChilds
		{
			get
			{
				foreach (WitResponseNode witResponseNode in this.Childs)
				{
					foreach (WitResponseNode witResponseNode2 in witResponseNode.DeepChilds)
					{
						yield return witResponseNode2;
					}
					IEnumerator<WitResponseNode> enumerator2 = null;
				}
				IEnumerator<WitResponseNode> enumerator = null;
				yield break;
				yield break;
			}
		}

		public override string ToString()
		{
			return "JSONNode";
		}

		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		public virtual int AsInt
		{
			get
			{
				int result;
				if (int.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual float AsFloat
		{
			get
			{
				float result;
				if (float.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0f;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual double AsDouble
		{
			get
			{
				double result;
				if (double.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result;
				if (bool.TryParse(this.Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = (value ? "true" : "false");
			}
		}

		public virtual WitResponseArray AsArray
		{
			get
			{
				return this as WitResponseArray;
			}
		}

		public virtual string[] AsStringArray
		{
			get
			{
				string[] array = new string[0];
				WitResponseArray asArray = this.AsArray;
				if (null != asArray)
				{
					array = new string[asArray.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = asArray[i].Value;
					}
				}
				return array;
			}
		}

		public virtual WitResponseClass AsObject
		{
			get
			{
				return this as WitResponseClass;
			}
		}

		public virtual T Cast<T>(T defaultValue = default(T))
		{
			object obj = defaultValue;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(string))
			{
				obj = this.Value;
			}
			else if (typeFromHandle == typeof(int))
			{
				obj = this.AsInt;
			}
			else if (typeFromHandle == typeof(float))
			{
				obj = this.AsFloat;
			}
			else if (typeFromHandle == typeof(double))
			{
				obj = this.AsDouble;
			}
			else if (typeFromHandle == typeof(bool))
			{
				obj = this.AsBool;
			}
			else if (typeFromHandle == typeof(string[]))
			{
				obj = this.AsStringArray;
			}
			else if (typeFromHandle == typeof(WitResponseNode))
			{
				obj = this;
			}
			else if (typeFromHandle == typeof(WitResponseArray))
			{
				obj = this.AsArray;
			}
			else if (typeFromHandle == typeof(WitResponseClass))
			{
				obj = this.AsObject;
			}
			else if (typeFromHandle == typeof(WitResponseData))
			{
				obj = (this as WitResponseData);
			}
			else
			{
				VLog.W("WitResponseNode", string.Concat(new string[]
				{
					"Cast ",
					base.GetType().Name,
					" to ",
					typeFromHandle.Name,
					" not supported"
				}), null);
			}
			return (T)((object)obj);
		}

		public static implicit operator WitResponseNode(string s)
		{
			return new WitResponseData(s);
		}

		public static implicit operator string(WitResponseNode d)
		{
			if (d == null)
			{
				return null;
			}
			return d.Value;
		}

		public static bool operator ==(WitResponseNode a, object b)
		{
			return (b == null && a is WitResponseLazyCreator) || a == b;
		}

		public static bool operator !=(WitResponseNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			WitResponseNode witResponseNode = obj as WitResponseNode;
			if (witResponseNode != null)
			{
				return WitResponseNode.Equals(this, witResponseNode);
			}
			return this == obj;
		}

		public static bool Equals(WitResponseNode oldNode, WitResponseNode newNode)
		{
			if (oldNode == null && newNode == null)
			{
				return true;
			}
			if (oldNode == null || newNode == null || oldNode.GetType() != newNode.GetType())
			{
				return false;
			}
			if (newNode is WitResponseData)
			{
				return string.Equals(oldNode.Value, newNode.Value);
			}
			if (newNode is WitResponseArray)
			{
				WitResponseArray asArray = oldNode.AsArray;
				WitResponseArray asArray2 = newNode.AsArray;
				if (asArray.Count != asArray2.Count)
				{
					return false;
				}
				for (int i = 0; i < asArray2.Count; i++)
				{
					if (!WitResponseNode.Equals(asArray[i], asArray2[i]))
					{
						return false;
					}
				}
			}
			else if (newNode is WitResponseClass)
			{
				WitResponseClass asObject = oldNode.AsObject;
				WitResponseClass asObject2 = newNode.AsObject;
				if (asObject.ChildNodeNames.Length != asObject2.ChildNodeNames.Length)
				{
					return false;
				}
				foreach (string aKey in asObject2.ChildNodeNames)
				{
					if (!WitResponseNode.Equals(asObject[aKey], asObject2[aKey]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			if (string.IsNullOrEmpty(aText))
			{
				return aText;
			}
			string text = "";
			int i = 0;
			while (i < aText.Length)
			{
				char c = aText[i];
				switch (c)
				{
				case '\b':
					text += "\\b";
					break;
				case '\t':
					text += "\\t";
					break;
				case '\n':
					text += "\\n";
					break;
				case '\v':
					goto IL_AD;
				case '\f':
					text += "\\f";
					break;
				case '\r':
					text += "\\r";
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_AD;
						}
						text += "\\\\";
					}
					else
					{
						text += "\\\"";
					}
					break;
				}
				IL_BB:
				i++;
				continue;
				IL_AD:
				text += c.ToString();
				goto IL_BB;
			}
			return text;
		}

		public static WitResponseNode Parse(string aJSON)
		{
			Stack<WitResponseNode> stack = new Stack<WitResponseNode>();
			WitResponseNode witResponseNode = null;
			int i = 0;
			string text = "";
			string text2 = "";
			bool flag = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				if (c <= ',')
				{
					if (c <= ' ')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
						case '\r':
							goto IL_429;
						case '\v':
						case '\f':
							goto IL_412;
						default:
							if (c != ' ')
							{
								goto IL_412;
							}
							break;
						}
						if (flag)
						{
							text += aJSON[i].ToString();
						}
					}
					else if (c != '"')
					{
						if (c != ',')
						{
							goto IL_412;
						}
						if (flag)
						{
							text += aJSON[i].ToString();
						}
						else
						{
							if (text != "")
							{
								if (witResponseNode is WitResponseArray)
								{
									witResponseNode.Add(text);
								}
								else if (text2 != "")
								{
									witResponseNode.Add(text2, text);
								}
							}
							text2 = "";
							text = "";
						}
					}
					else
					{
						flag = !flag;
					}
				}
				else
				{
					if (c <= ']')
					{
						if (c != ':')
						{
							switch (c)
							{
							case '[':
								if (flag)
								{
									text += aJSON[i].ToString();
									goto IL_429;
								}
								stack.Push(new WitResponseArray());
								if (witResponseNode != null)
								{
									text2 = text2.Trim();
									if (witResponseNode is WitResponseArray)
									{
										witResponseNode.Add(stack.Peek());
									}
									else if (text2 != "")
									{
										witResponseNode.Add(text2, stack.Peek());
									}
								}
								text2 = "";
								text = "";
								witResponseNode = stack.Peek();
								goto IL_429;
							case '\\':
								i++;
								if (flag)
								{
									char c2 = aJSON[i];
									if (c2 <= 'f')
									{
										if (c2 == 'b')
										{
											text += "\b";
											goto IL_429;
										}
										if (c2 == 'f')
										{
											text += "\f";
											goto IL_429;
										}
									}
									else
									{
										if (c2 == 'n')
										{
											text += "\n";
											goto IL_429;
										}
										switch (c2)
										{
										case 'r':
											text += "\r";
											goto IL_429;
										case 't':
											text += "\t";
											goto IL_429;
										case 'u':
										{
											string s = aJSON.Substring(i + 1, 4);
											text += ((char)int.Parse(s, NumberStyles.AllowHexSpecifier)).ToString();
											i += 4;
											goto IL_429;
										}
										}
									}
									text += c2.ToString();
									goto IL_429;
								}
								goto IL_429;
							case ']':
								break;
							default:
								goto IL_412;
							}
						}
						else
						{
							if (flag)
							{
								text += aJSON[i].ToString();
								goto IL_429;
							}
							text2 = text;
							text = "";
							goto IL_429;
						}
					}
					else if (c != '{')
					{
						if (c != '}')
						{
							goto IL_412;
						}
					}
					else
					{
						if (flag)
						{
							text += aJSON[i].ToString();
							goto IL_429;
						}
						stack.Push(new WitResponseClass());
						if (witResponseNode != null)
						{
							text2 = text2.Trim();
							if (witResponseNode is WitResponseArray)
							{
								witResponseNode.Add(stack.Peek());
							}
							else if (text2 != "")
							{
								witResponseNode.Add(text2, stack.Peek());
							}
						}
						text2 = "";
						text = "";
						witResponseNode = stack.Peek();
						goto IL_429;
					}
					if (flag)
					{
						text += aJSON[i].ToString();
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new JSONParseException("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (text != "")
						{
							text2 = text2.Trim();
							if (witResponseNode is WitResponseArray)
							{
								witResponseNode.Add(text);
							}
							else if (text2 != "")
							{
								witResponseNode.Add(text2, text);
							}
						}
						text2 = "";
						text = "";
						if (stack.Count > 0)
						{
							witResponseNode = stack.Peek();
						}
					}
				}
				IL_429:
				i++;
				continue;
				IL_412:
				text += aJSON[i].ToString();
				goto IL_429;
			}
			if (flag)
			{
				throw new JSONParseException("JSON Parse: Quotation marks seems to be messed up.");
			}
			return witResponseNode;
		}

		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		public string SaveToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		public static WitResponseNode Deserialize(BinaryReader aReader)
		{
			JSONBinaryTag jsonbinaryTag = (JSONBinaryTag)aReader.ReadByte();
			switch (jsonbinaryTag)
			{
			case JSONBinaryTag.Array:
			{
				int num = aReader.ReadInt32();
				WitResponseArray witResponseArray = new WitResponseArray();
				for (int i = 0; i < num; i++)
				{
					witResponseArray.Add(WitResponseNode.Deserialize(aReader));
				}
				return witResponseArray;
			}
			case JSONBinaryTag.Class:
			{
				int num2 = aReader.ReadInt32();
				WitResponseClass witResponseClass = new WitResponseClass();
				for (int j = 0; j < num2; j++)
				{
					string aKey = aReader.ReadString();
					WitResponseNode aItem = WitResponseNode.Deserialize(aReader);
					witResponseClass.Add(aKey, aItem);
				}
				return witResponseClass;
			}
			case JSONBinaryTag.Value:
				return new WitResponseData(aReader.ReadString());
			case JSONBinaryTag.IntValue:
				return new WitResponseData(aReader.ReadInt32());
			case JSONBinaryTag.DoubleValue:
				return new WitResponseData(aReader.ReadDouble());
			case JSONBinaryTag.BoolValue:
				return new WitResponseData(aReader.ReadBoolean());
			case JSONBinaryTag.FloatValue:
				return new WitResponseData(aReader.ReadSingle());
			default:
				throw new JSONParseException("Error deserializing JSON. Unknown tag: " + jsonbinaryTag.ToString());
			}
		}

		public static WitResponseNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static WitResponseNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static WitResponseNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static WitResponseNode LoadFromStream(Stream aData)
		{
			WitResponseNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = WitResponseNode.Deserialize(binaryReader);
			}
			return result;
		}

		public static WitResponseNode LoadFromFile(string aFileName)
		{
			WitResponseNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = WitResponseNode.LoadFromStream(fileStream);
			}
			return result;
		}

		public static WitResponseNode LoadFromBase64(string aBase64)
		{
			return WitResponseNode.LoadFromStream(new MemoryStream(Convert.FromBase64String(aBase64))
			{
				Position = 0L
			});
		}
	}
}
