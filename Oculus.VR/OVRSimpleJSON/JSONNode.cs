using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace OVRSimpleJSON
{
	public abstract class JSONNode
	{
		public abstract JSONNodeType Tag { get; }

		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[string aKey]
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

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		public virtual bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public virtual void Clear()
		{
		}

		public virtual JSONNode Clone()
		{
			return null;
		}

		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode jsonnode in this.Children)
				{
					foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
					{
						yield return jsonnode2;
					}
					IEnumerator<JSONNode> enumerator2 = null;
				}
				IEnumerator<JSONNode> enumerator = null;
				yield break;
				yield break;
			}
		}

		public virtual bool HasKey(string aKey)
		{
			return false;
		}

		public virtual JSONNode GetValueOrDefault(string aKey, JSONNode aDefault)
		{
			return aDefault;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		public abstract JSONNode.Enumerator GetEnumerator();

		public IEnumerable<KeyValuePair<string, JSONNode>> Linq
		{
			get
			{
				return new JSONNode.LinqEnumerator(this);
			}
		}

		public JSONNode.KeyEnumerator Keys
		{
			get
			{
				return new JSONNode.KeyEnumerator(this.GetEnumerator());
			}
		}

		public JSONNode.ValueEnumerator Values
		{
			get
			{
				return new JSONNode.ValueEnumerator(this.GetEnumerator());
			}
		}

		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(this.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public virtual int AsInt
		{
			get
			{
				return (int)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		public virtual float AsFloat
		{
			get
			{
				return (float)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result = false;
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

		public virtual long AsLong
		{
			get
			{
				long result = 0L;
				if (long.TryParse(this.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0L;
			}
			set
			{
				this.Value = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public virtual ulong AsULong
		{
			get
			{
				ulong result = 0UL;
				if (ulong.TryParse(this.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0UL;
			}
			set
			{
				this.Value = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		public static implicit operator JSONNode(string s)
		{
			if (s != null)
			{
				return new JSONString(s);
			}
			return JSONNull.CreateOrGet();
		}

		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber((double)n);
		}

		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber((double)n);
		}

		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		public static implicit operator JSONNode(long n)
		{
			if (JSONNode.longAsString)
			{
				return new JSONString(n.ToString(CultureInfo.InvariantCulture));
			}
			return new JSONNumber((double)n);
		}

		public static implicit operator long(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsLong;
			}
			return 0L;
		}

		public static implicit operator JSONNode(ulong n)
		{
			if (JSONNode.longAsString)
			{
				return new JSONString(n.ToString(CultureInfo.InvariantCulture));
			}
			return new JSONNumber(n);
		}

		public static implicit operator ulong(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsULong;
			}
			return 0UL;
		}

		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
		{
			return aKeyValue.Value;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			if (a == b)
			{
				return true;
			}
			bool flag = a is JSONNull || a == null || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
			return (flag && flag2) || (!flag && a.Equals(b));
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static StringBuilder EscapeBuilder
		{
			get
			{
				if (JSONNode.m_EscapeBuilder == null)
				{
					JSONNode.m_EscapeBuilder = new StringBuilder();
				}
				return JSONNode.m_EscapeBuilder;
			}
		}

		internal static string Escape(string aText)
		{
			StringBuilder escapeBuilder = JSONNode.EscapeBuilder;
			escapeBuilder.Length = 0;
			if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				escapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			int i = 0;
			while (i < aText.Length)
			{
				char c = aText[i];
				switch (c)
				{
				case '\b':
					escapeBuilder.Append("\\b");
					break;
				case '\t':
					escapeBuilder.Append("\\t");
					break;
				case '\n':
					escapeBuilder.Append("\\n");
					break;
				case '\v':
					goto IL_E2;
				case '\f':
					escapeBuilder.Append("\\f");
					break;
				case '\r':
					escapeBuilder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_E2;
						}
						escapeBuilder.Append("\\\\");
					}
					else
					{
						escapeBuilder.Append("\\\"");
					}
					break;
				}
				IL_121:
				i++;
				continue;
				IL_E2:
				if (c < ' ' || (JSONNode.forceASCII && c > '\u007f'))
				{
					ushort num = (ushort)c;
					escapeBuilder.Append("\\u").Append(num.ToString("X4"));
					goto IL_121;
				}
				escapeBuilder.Append(c);
				goto IL_121;
			}
			string result = escapeBuilder.ToString();
			escapeBuilder.Length = 0;
			return result;
		}

		private static JSONNode ParseElement(string token, bool quoted)
		{
			if (quoted)
			{
				return token;
			}
			if (token.Length <= 5)
			{
				string a = token.ToLower();
				if (a == "false" || a == "true")
				{
					return a == "true";
				}
				if (a == "null")
				{
					return JSONNull.CreateOrGet();
				}
			}
			double n;
			if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out n))
			{
				return n;
			}
			return token;
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string aKey = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				if (c <= '/')
				{
					if (c <= ' ')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
						case '\r':
							flag3 = true;
							goto IL_3EC;
						case '\v':
						case '\f':
							goto IL_3DE;
						default:
							if (c != ' ')
							{
								goto IL_3DE;
							}
							break;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
					}
					else if (c != '"')
					{
						if (c != ',')
						{
							if (c != '/')
							{
								goto IL_3DE;
							}
							if (JSONNode.allowLineComments && !flag && i + 1 < aJSON.Length && aJSON[i + 1] == '/')
							{
								while (++i < aJSON.Length && aJSON[i] != '\n')
								{
									if (aJSON[i] == '\r')
									{
										break;
									}
								}
							}
							else
							{
								stringBuilder.Append(aJSON[i]);
							}
						}
						else if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
						else
						{
							if (stringBuilder.Length > 0 || flag2)
							{
								jsonnode.Add(aKey, JSONNode.ParseElement(stringBuilder.ToString(), flag2));
							}
							aKey = "";
							stringBuilder.Length = 0;
							flag2 = false;
						}
					}
					else
					{
						flag = !flag;
						flag2 = (flag2 || flag);
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
									stringBuilder.Append(aJSON[i]);
									goto IL_3EC;
								}
								stack.Push(new JSONArray());
								if (jsonnode != null)
								{
									jsonnode.Add(aKey, stack.Peek());
								}
								aKey = "";
								stringBuilder.Length = 0;
								jsonnode = stack.Peek();
								flag3 = false;
								goto IL_3EC;
							case '\\':
								i++;
								if (flag)
								{
									char c2 = aJSON[i];
									if (c2 <= 'f')
									{
										if (c2 == 'b')
										{
											stringBuilder.Append('\b');
											goto IL_3EC;
										}
										if (c2 == 'f')
										{
											stringBuilder.Append('\f');
											goto IL_3EC;
										}
									}
									else
									{
										if (c2 == 'n')
										{
											stringBuilder.Append('\n');
											goto IL_3EC;
										}
										switch (c2)
										{
										case 'r':
											stringBuilder.Append('\r');
											goto IL_3EC;
										case 't':
											stringBuilder.Append('\t');
											goto IL_3EC;
										case 'u':
										{
											string s = aJSON.Substring(i + 1, 4);
											stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
											i += 4;
											goto IL_3EC;
										}
										}
									}
									stringBuilder.Append(c2);
									goto IL_3EC;
								}
								goto IL_3EC;
							case ']':
								break;
							default:
								goto IL_3DE;
							}
						}
						else
						{
							if (flag)
							{
								stringBuilder.Append(aJSON[i]);
								goto IL_3EC;
							}
							aKey = stringBuilder.ToString();
							stringBuilder.Length = 0;
							flag2 = false;
							goto IL_3EC;
						}
					}
					else if (c != '{')
					{
						if (c != '}')
						{
							if (c != '﻿')
							{
								goto IL_3DE;
							}
							goto IL_3EC;
						}
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
							goto IL_3EC;
						}
						stack.Push(new JSONObject());
						if (jsonnode != null)
						{
							jsonnode.Add(aKey, stack.Peek());
						}
						aKey = "";
						stringBuilder.Length = 0;
						jsonnode = stack.Peek();
						flag3 = false;
						goto IL_3EC;
					}
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (stringBuilder.Length > 0 || flag2)
						{
							jsonnode.Add(aKey, JSONNode.ParseElement(stringBuilder.ToString(), flag2));
						}
						if (jsonnode != null)
						{
							jsonnode.Inline = !flag3;
						}
						flag2 = false;
						aKey = "";
						stringBuilder.Length = 0;
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
				}
				IL_3EC:
				i++;
				continue;
				IL_3DE:
				stringBuilder.Append(aJSON[i]);
				goto IL_3EC;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			if (jsonnode == null)
			{
				return JSONNode.ParseElement(stringBuilder.ToString(), flag2);
			}
			return jsonnode;
		}

		private static JSONNode GetContainer(JSONContainerType aType)
		{
			if (aType == JSONContainerType.Array)
			{
				return new JSONArray();
			}
			return new JSONObject();
		}

		public static implicit operator JSONNode(Vector2 aVec)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.VectorContainerType);
			container.WriteVector2(aVec, "x", "y");
			return container;
		}

		public static implicit operator JSONNode(Vector3 aVec)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.VectorContainerType);
			container.WriteVector3(aVec, "x", "y", "z");
			return container;
		}

		public static implicit operator JSONNode(Vector4 aVec)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.VectorContainerType);
			container.WriteVector4(aVec);
			return container;
		}

		public static implicit operator JSONNode(Color aCol)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.ColorContainerType);
			container.WriteColor(aCol);
			return container;
		}

		public static implicit operator JSONNode(Quaternion aRot)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.QuaternionContainerType);
			container.WriteQuaternion(aRot);
			return container;
		}

		public static implicit operator JSONNode(Rect aRect)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.RectContainerType);
			container.WriteRect(aRect);
			return container;
		}

		public static implicit operator JSONNode(RectOffset aRect)
		{
			JSONNode container = JSONNode.GetContainer(JSONNode.RectContainerType);
			container.WriteRectOffset(aRect);
			return container;
		}

		public static implicit operator Vector2(JSONNode aNode)
		{
			return aNode.ReadVector2();
		}

		public static implicit operator Vector3(JSONNode aNode)
		{
			return aNode.ReadVector3();
		}

		public static implicit operator Vector4(JSONNode aNode)
		{
			return aNode.ReadVector4();
		}

		public static implicit operator Color(JSONNode aNode)
		{
			return aNode.ReadColor();
		}

		public static implicit operator Quaternion(JSONNode aNode)
		{
			return aNode.ReadQuaternion();
		}

		public static implicit operator Rect(JSONNode aNode)
		{
			return aNode.ReadRect();
		}

		public static implicit operator RectOffset(JSONNode aNode)
		{
			return aNode.ReadRectOffset();
		}

		public Vector2 ReadVector2(Vector2 aDefault)
		{
			if (this.IsObject)
			{
				return new Vector2(this["x"].AsFloat, this["y"].AsFloat);
			}
			if (this.IsArray)
			{
				return new Vector2(this[0].AsFloat, this[1].AsFloat);
			}
			return aDefault;
		}

		public Vector2 ReadVector2(string aXName, string aYName)
		{
			if (this.IsObject)
			{
				return new Vector2(this[aXName].AsFloat, this[aYName].AsFloat);
			}
			return Vector2.zero;
		}

		public Vector2 ReadVector2()
		{
			return this.ReadVector2(Vector2.zero);
		}

		public JSONNode WriteVector2(Vector2 aVec, string aXName = "x", string aYName = "y")
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this[aXName].AsFloat = aVec.x;
				this[aYName].AsFloat = aVec.y;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsFloat = aVec.x;
				this[1].AsFloat = aVec.y;
			}
			return this;
		}

		public Vector3 ReadVector3(Vector3 aDefault)
		{
			if (this.IsObject)
			{
				return new Vector3(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat);
			}
			if (this.IsArray)
			{
				return new Vector3(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat);
			}
			return aDefault;
		}

		public Vector3 ReadVector3(string aXName, string aYName, string aZName)
		{
			if (this.IsObject)
			{
				return new Vector3(this[aXName].AsFloat, this[aYName].AsFloat, this[aZName].AsFloat);
			}
			return Vector3.zero;
		}

		public Vector3 ReadVector3()
		{
			return this.ReadVector3(Vector3.zero);
		}

		public JSONNode WriteVector3(Vector3 aVec, string aXName = "x", string aYName = "y", string aZName = "z")
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this[aXName].AsFloat = aVec.x;
				this[aYName].AsFloat = aVec.y;
				this[aZName].AsFloat = aVec.z;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsFloat = aVec.x;
				this[1].AsFloat = aVec.y;
				this[2].AsFloat = aVec.z;
			}
			return this;
		}

		public Vector4 ReadVector4(Vector4 aDefault)
		{
			if (this.IsObject)
			{
				return new Vector4(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat, this["w"].AsFloat);
			}
			if (this.IsArray)
			{
				return new Vector4(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, this[3].AsFloat);
			}
			return aDefault;
		}

		public Vector4 ReadVector4()
		{
			return this.ReadVector4(Vector4.zero);
		}

		public JSONNode WriteVector4(Vector4 aVec)
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this["x"].AsFloat = aVec.x;
				this["y"].AsFloat = aVec.y;
				this["z"].AsFloat = aVec.z;
				this["w"].AsFloat = aVec.w;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsFloat = aVec.x;
				this[1].AsFloat = aVec.y;
				this[2].AsFloat = aVec.z;
				this[3].AsFloat = aVec.w;
			}
			return this;
		}

		public Color ReadColor(Color aDefault)
		{
			if (this.IsObject)
			{
				return new Color(this["r"].AsFloat, this["g"].AsFloat, this["b"].AsFloat, this.HasKey("a") ? this["a"].AsFloat : JSONNode.ColorDefaultAlpha);
			}
			if (this.IsArray)
			{
				return new Color(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, (this.Count > 3) ? this[3].AsFloat : JSONNode.ColorDefaultAlpha);
			}
			return aDefault;
		}

		public Color ReadColor()
		{
			return this.ReadColor(Color.clear);
		}

		public JSONNode WriteColor(Color aCol)
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this["r"].AsFloat = aCol.r;
				this["g"].AsFloat = aCol.g;
				this["b"].AsFloat = aCol.b;
				this["a"].AsFloat = aCol.a;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsFloat = aCol.r;
				this[1].AsFloat = aCol.g;
				this[2].AsFloat = aCol.b;
				this[3].AsFloat = aCol.a;
			}
			return this;
		}

		public Quaternion ReadQuaternion(Quaternion aDefault)
		{
			if (this.IsObject)
			{
				return new Quaternion(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat, this["w"].AsFloat);
			}
			if (this.IsArray)
			{
				return new Quaternion(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, this[3].AsFloat);
			}
			return aDefault;
		}

		public Quaternion ReadQuaternion()
		{
			return this.ReadQuaternion(Quaternion.identity);
		}

		public JSONNode WriteQuaternion(Quaternion aRot)
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this["x"].AsFloat = aRot.x;
				this["y"].AsFloat = aRot.y;
				this["z"].AsFloat = aRot.z;
				this["w"].AsFloat = aRot.w;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsFloat = aRot.x;
				this[1].AsFloat = aRot.y;
				this[2].AsFloat = aRot.z;
				this[3].AsFloat = aRot.w;
			}
			return this;
		}

		public Rect ReadRect(Rect aDefault)
		{
			if (this.IsObject)
			{
				return new Rect(this["x"].AsFloat, this["y"].AsFloat, this["width"].AsFloat, this["height"].AsFloat);
			}
			if (this.IsArray)
			{
				return new Rect(this[0].AsFloat, this[1].AsFloat, this[2].AsFloat, this[3].AsFloat);
			}
			return aDefault;
		}

		public Rect ReadRect()
		{
			return this.ReadRect(default(Rect));
		}

		public JSONNode WriteRect(Rect aRect)
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this["x"].AsFloat = aRect.x;
				this["y"].AsFloat = aRect.y;
				this["width"].AsFloat = aRect.width;
				this["height"].AsFloat = aRect.height;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsFloat = aRect.x;
				this[1].AsFloat = aRect.y;
				this[2].AsFloat = aRect.width;
				this[3].AsFloat = aRect.height;
			}
			return this;
		}

		public RectOffset ReadRectOffset(RectOffset aDefault)
		{
			if (this is JSONObject)
			{
				return new RectOffset(this["left"].AsInt, this["right"].AsInt, this["top"].AsInt, this["bottom"].AsInt);
			}
			if (this is JSONArray)
			{
				return new RectOffset(this[0].AsInt, this[1].AsInt, this[2].AsInt, this[3].AsInt);
			}
			return aDefault;
		}

		public RectOffset ReadRectOffset()
		{
			return this.ReadRectOffset(new RectOffset());
		}

		public JSONNode WriteRectOffset(RectOffset aRect)
		{
			if (this.IsObject)
			{
				this.Inline = true;
				this["left"].AsInt = aRect.left;
				this["right"].AsInt = aRect.right;
				this["top"].AsInt = aRect.top;
				this["bottom"].AsInt = aRect.bottom;
			}
			else if (this.IsArray)
			{
				this.Inline = true;
				this[0].AsInt = aRect.left;
				this[1].AsInt = aRect.right;
				this[2].AsInt = aRect.top;
				this[3].AsInt = aRect.bottom;
			}
			return this;
		}

		public Matrix4x4 ReadMatrix()
		{
			Matrix4x4 identity = Matrix4x4.identity;
			if (this.IsArray)
			{
				for (int i = 0; i < 16; i++)
				{
					identity[i] = this[i].AsFloat;
				}
			}
			return identity;
		}

		public JSONNode WriteMatrix(Matrix4x4 aMatrix)
		{
			if (this.IsArray)
			{
				this.Inline = true;
				for (int i = 0; i < 16; i++)
				{
					this[i].AsFloat = aMatrix[i];
				}
			}
			return this;
		}

		public static bool forceASCII = false;

		public static bool longAsString = false;

		public static bool allowLineComments = true;

		[ThreadStatic]
		private static StringBuilder m_EscapeBuilder;

		public static float ColorDefaultAlpha = 1f;

		public static JSONContainerType VectorContainerType = JSONContainerType.Array;

		public static JSONContainerType QuaternionContainerType = JSONContainerType.Array;

		public static JSONContainerType RectContainerType = JSONContainerType.Array;

		public static JSONContainerType ColorContainerType = JSONContainerType.Array;

		public struct Enumerator
		{
			public bool IsValid
			{
				get
				{
					return this.type > JSONNode.Enumerator.Type.None;
				}
			}

			public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this.type = JSONNode.Enumerator.Type.Array;
				this.m_Object = default(Dictionary<string, JSONNode>.Enumerator);
				this.m_Array = aArrayEnum;
			}

			public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this.type = JSONNode.Enumerator.Type.Object;
				this.m_Object = aDictEnum;
				this.m_Array = default(List<JSONNode>.Enumerator);
			}

			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					if (this.type == JSONNode.Enumerator.Type.Array)
					{
						return new KeyValuePair<string, JSONNode>(string.Empty, this.m_Array.Current);
					}
					if (this.type == JSONNode.Enumerator.Type.Object)
					{
						return this.m_Object.Current;
					}
					return new KeyValuePair<string, JSONNode>(string.Empty, null);
				}
			}

			public bool MoveNext()
			{
				if (this.type == JSONNode.Enumerator.Type.Array)
				{
					return this.m_Array.MoveNext();
				}
				return this.type == JSONNode.Enumerator.Type.Object && this.m_Object.MoveNext();
			}

			private JSONNode.Enumerator.Type type;

			private Dictionary<string, JSONNode>.Enumerator m_Object;

			private List<JSONNode>.Enumerator m_Array;

			private enum Type
			{
				None,
				Array,
				Object
			}
		}

		public struct ValueEnumerator
		{
			public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aArrayEnum));
			}

			public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aDictEnum));
			}

			public ValueEnumerator(JSONNode.Enumerator aEnumerator)
			{
				this.m_Enumerator = aEnumerator;
			}

			public JSONNode Current
			{
				get
				{
					KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
					return keyValuePair.Value;
				}
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public JSONNode.ValueEnumerator GetEnumerator()
			{
				return this;
			}

			private JSONNode.Enumerator m_Enumerator;
		}

		public struct KeyEnumerator
		{
			public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aArrayEnum));
			}

			public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aDictEnum));
			}

			public KeyEnumerator(JSONNode.Enumerator aEnumerator)
			{
				this.m_Enumerator = aEnumerator;
			}

			public string Current
			{
				get
				{
					KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
					return keyValuePair.Key;
				}
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public JSONNode.KeyEnumerator GetEnumerator()
			{
				return this;
			}

			private JSONNode.Enumerator m_Enumerator;
		}

		public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerator, IDisposable, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
		{
			internal LinqEnumerator(JSONNode aNode)
			{
				this.m_Node = aNode;
				if (this.m_Node != null)
				{
					this.m_Enumerator = this.m_Node.GetEnumerator();
				}
			}

			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public void Dispose()
			{
				this.m_Node = null;
				this.m_Enumerator = default(JSONNode.Enumerator);
			}

			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
			{
				return new JSONNode.LinqEnumerator(this.m_Node);
			}

			public void Reset()
			{
				if (this.m_Node != null)
				{
					this.m_Enumerator = this.m_Node.GetEnumerator();
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new JSONNode.LinqEnumerator(this.m_Node);
			}

			private JSONNode m_Node;

			private JSONNode.Enumerator m_Enumerator;
		}
	}
}
