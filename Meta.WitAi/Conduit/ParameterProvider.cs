using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Meta.WitAi;
using Meta.WitAi.Json;

namespace Meta.Conduit
{
	internal class ParameterProvider : IParameterProvider
	{
		public List<string> AllParameterNames
		{
			get
			{
				return this.ActualParameters.Keys.ToList<string>();
			}
		}

		public void AddCustomType(string name, Type type)
		{
			this._customTypes[name] = type;
		}

		public void AddParameter(string parameterName, object value)
		{
			this.ActualParameters[parameterName] = value;
		}

		public void PopulateParametersFromNode(WitResponseNode responseNode)
		{
			this._parametersOfType.Clear();
			Dictionary<string, ConduitParameterValue> dictionary = new Dictionary<string, ConduitParameterValue>();
			foreach (WitResponseNode witResponseNode in responseNode.AsObject["entities"].Childs)
			{
				string value = witResponseNode[0]["role"].Value;
				string value2 = witResponseNode[0]["value"].Value;
				List<Type> list = this.GetParameterTypes(witResponseNode[0]["name"].Value, value2).ToList<Type>();
				foreach (Type key in list)
				{
					if (!this._parametersOfType.ContainsKey(key))
					{
						this._parametersOfType.Add(key, new List<string>());
					}
					this._parametersOfType[key].Add(value);
				}
				dictionary.Add(value, new ConduitParameterValue(value2, list.First<Type>()));
			}
			dictionary.Add("@WitResponseNode", new ConduitParameterValue(responseNode, typeof(WitResponseNode)));
			this.PopulateParameters(dictionary);
		}

		public void SetSpecializedParameter(string reservedParameterName, Type parameterType)
		{
			this._specializedParameters[parameterType] = reservedParameterName.ToLower();
		}

		public void PopulateParameters(Dictionary<string, ConduitParameterValue> actualParameters)
		{
			this.ActualParameters.Clear();
			foreach (KeyValuePair<string, ConduitParameterValue> keyValuePair in actualParameters)
			{
				this.ActualParameters[keyValuePair.Key] = keyValuePair.Value.Value;
			}
		}

		public void PopulateRoles(Dictionary<string, string> parameterToRoleMap)
		{
			this._parameterToRoleMap.Clear();
			foreach (KeyValuePair<string, string> keyValuePair in parameterToRoleMap)
			{
				this._parameterToRoleMap[keyValuePair.Key.ToLower()] = keyValuePair.Value;
			}
		}

		public bool ContainsParameter(ParameterInfo parameter, StringBuilder log)
		{
			if (this.SupportedSpecializedParameter(parameter))
			{
				return true;
			}
			if (!this.ActualParameters.ContainsKey(parameter.Name))
			{
				log.AppendLine("\tParameter '" + parameter.Name + "' not sent in invoke");
				return false;
			}
			if (!this._parameterToRoleMap.ContainsKey(parameter.Name))
			{
				log.AppendLine("\tParameter '" + parameter.Name + "' not found in role map");
				return false;
			}
			return true;
		}

		public object GetRawParameterValue(string parameterName)
		{
			object obj;
			if (!this.ActualParameters.TryGetValue(parameterName, out obj) || obj == null)
			{
				return null;
			}
			return obj;
		}

		public object GetParameterValue(ParameterInfo formalParameter, Dictionary<string, string> parameterMap = null, bool relaxed = false)
		{
			if (parameterMap == null)
			{
				parameterMap = new Dictionary<string, string>();
			}
			if (this.SupportedSpecializedParameter(formalParameter))
			{
				return this.GetSpecializedParameter(formalParameter);
			}
			string actualParameterName = this.GetActualParameterName(formalParameter, parameterMap, relaxed);
			if (string.IsNullOrEmpty(actualParameterName))
			{
				return null;
			}
			object obj;
			if (!this.ActualParameters.TryGetValue(actualParameterName, out obj) || obj == null)
			{
				return null;
			}
			return ConduitUtilities.GetTypedParameterValue(formalParameter, obj);
		}

		public T GetParameterValue<T>(string parameterName, Dictionary<string, string> parameterMap = null, bool relaxed = false)
		{
			object obj;
			if (!this.ActualParameters.TryGetValue(parameterName, out obj) || obj == null)
			{
				return default(T);
			}
			return (T)((object)ConduitUtilities.GetTypedParameterValue(typeof(T), obj));
		}

		private static object ToNullable(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			Type type = obj.GetType();
			if (type.IsNullableType())
			{
				return obj;
			}
			Type conversionType = typeof(Nullable<>).MakeGenericType(new Type[]
			{
				type
			});
			return Convert.ChangeType(obj, conversionType);
		}

		public List<string> GetParameterNamesOfType(Type targetType)
		{
			if (this._parametersOfType.ContainsKey(targetType))
			{
				return this._parametersOfType[targetType];
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> keyValuePair in this.ActualParameters)
			{
				Type type = keyValuePair.Value.GetType();
				if (type == targetType)
				{
					list.Add(keyValuePair.Key);
				}
				else if (targetType.IsNullableType())
				{
					Type underlyingType = Nullable.GetUnderlyingType(targetType);
					if (underlyingType == null)
					{
						VLog.E(string.Format("Got a null underlying type from nullable type {0}", targetType), null);
					}
					else if (type == underlyingType)
					{
						list.Add(keyValuePair.Key);
					}
					else if (underlyingType.IsEnum)
					{
						string text = keyValuePair.Value as string;
						object obj;
						if (text != null && Enum.TryParse(underlyingType, text, out obj))
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
			}
			return this._parametersOfType[targetType] = list;
		}

		protected virtual bool SupportedSpecializedParameter(ParameterInfo formalParameter)
		{
			return this._specializedParameters.ContainsKey(formalParameter.ParameterType);
		}

		protected virtual object GetSpecializedParameter(ParameterInfo formalParameter)
		{
			if (this._specializedParameters.ContainsKey(formalParameter.ParameterType))
			{
				string key = this._specializedParameters[formalParameter.ParameterType];
				if (this.ActualParameters.ContainsKey(key))
				{
					return this.ActualParameters[key];
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Specialized parameter not found");
			stringBuilder.AppendLine(string.Format("Parameter Type: {0}", formalParameter.ParameterType));
			stringBuilder.AppendLine("Parameter Name: " + formalParameter.Name);
			stringBuilder.AppendLine(string.Format("Actual Parameters: {0}", this.ActualParameters.Keys.Count));
			foreach (string text in this.ActualParameters.Keys)
			{
				string str = (this.ActualParameters[text] == null) ? "NULL" : this.ActualParameters[text].GetType().ToString();
				stringBuilder.AppendLine("\t" + text + ": " + str);
			}
			VLog.W(stringBuilder.ToString(), null);
			return null;
		}

		private IEnumerable<Type> GetParameterTypes(string typeString, string value)
		{
			if (this._customTypes.ContainsKey(typeString))
			{
				return new List<Type>
				{
					this._customTypes[typeString]
				};
			}
			if (!ParameterProvider.BuiltInTypes.ContainsKey(typeString) || ParameterProvider.BuiltInTypes[typeString].Count == 0)
			{
				return new List<Type>
				{
					typeof(string)
				};
			}
			return (from type in ParameterProvider.BuiltInTypes[typeString]
			where this.PerfectTypeMatch(type, value)
			select type).ToList<Type>();
		}

		private bool PerfectTypeMatch(Type targetType, string value)
		{
			bool result;
			try
			{
				object obj = Convert.ChangeType(value, targetType);
				if (obj == null)
				{
					result = false;
				}
				else if (!targetType.IsPrimitive)
				{
					result = true;
				}
				else
				{
					result = value.Equals(obj.ToString());
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private string GetActualParameterName(ParameterInfo formalParameter, Dictionary<string, string> parameterMap, bool relaxed)
		{
			string name = formalParameter.Name;
			string text;
			if (parameterMap.ContainsKey(name))
			{
				text = parameterMap[name];
			}
			else
			{
				text = name;
			}
			if (this.ActualParameters.ContainsKey(text))
			{
				return text;
			}
			if (this._parameterToRoleMap.ContainsKey(text))
			{
				string text2 = this._parameterToRoleMap[text];
				if (!string.IsNullOrEmpty(text2) && this.ActualParameters.ContainsKey(text2))
				{
					return text2;
				}
			}
			if (!relaxed)
			{
				if (formalParameter.ParameterType.IsNullableType())
				{
					return null;
				}
				VLog.E("Parameter '" + name + "' is missing", null);
				return null;
			}
			else
			{
				Type parameterType = formalParameter.ParameterType;
				List<string> parameterNamesOfType = this.GetParameterNamesOfType(parameterType);
				if (parameterNamesOfType.Count > 1)
				{
					VLog.E(string.Format("Got multiple parameters of type {0} but none with the correct name", parameterType), null);
					return null;
				}
				if (parameterNamesOfType.Count == 0)
				{
					if (!formalParameter.ParameterType.IsNullableType())
					{
						VLog.E(string.Format("Got zero parameters of type {0}.", parameterType), null);
					}
					return null;
				}
				return parameterNamesOfType[0];
			}
		}

		public override string ToString()
		{
			return string.Join("',", this.AllParameterNames);
		}

		public const string WitResponseNodeReservedName = "@WitResponseNode";

		public const string VoiceSessionReservedName = "@VoiceSession";

		protected readonly Dictionary<string, object> ActualParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, string> _parameterToRoleMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<Type, List<string>> _parametersOfType = new Dictionary<Type, List<string>>();

		private readonly Dictionary<Type, string> _specializedParameters = new Dictionary<Type, string>();

		private static readonly Dictionary<string, List<Type>> BuiltInTypes = new Dictionary<string, List<Type>>
		{
			{
				"wit$age_of_person",
				new List<Type>
				{
					typeof(int),
					typeof(short),
					typeof(long),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				"wit$amount_of_money",
				new List<Type>
				{
					typeof(decimal),
					typeof(float),
					typeof(double),
					typeof(int)
				}
			},
			{
				"wit$datetime",
				new List<Type>
				{
					typeof(DateTime)
				}
			},
			{
				"wit$distance",
				new List<Type>
				{
					typeof(decimal),
					typeof(float),
					typeof(double),
					typeof(int)
				}
			},
			{
				"wit$duration",
				new List<Type>
				{
					typeof(TimeSpan),
					typeof(float),
					typeof(double),
					typeof(int),
					typeof(decimal)
				}
			},
			{
				"wit$number",
				new List<Type>
				{
					typeof(int),
					typeof(long),
					typeof(short),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				"wit$ordinal",
				new List<Type>
				{
					typeof(int),
					typeof(long),
					typeof(short)
				}
			},
			{
				"wit$quantity",
				new List<Type>
				{
					typeof(int),
					typeof(long),
					typeof(short),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				"wit$temperature",
				new List<Type>
				{
					typeof(decimal),
					typeof(float),
					typeof(double),
					typeof(int),
					typeof(short),
					typeof(long)
				}
			},
			{
				"wit$volume",
				new List<Type>
				{
					typeof(int),
					typeof(long),
					typeof(short),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			}
		};

		private readonly Dictionary<string, Type> _customTypes = new Dictionary<string, Type>();
	}
}
