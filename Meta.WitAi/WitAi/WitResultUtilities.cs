using System;
using Meta.WitAi.Data.Entities;
using Meta.WitAi.Data.Intents;
using Meta.WitAi.Json;

namespace Meta.WitAi
{
	public static class WitResultUtilities
	{
		public static int GetStatusCode(this WitResponseNode witResponse)
		{
			if (!(null != witResponse) || !(witResponse.AsObject != null) || !witResponse.AsObject.HasChild("code"))
			{
				return 200;
			}
			return witResponse["code"].AsInt;
		}

		public static string GetError(this WitResponseNode witResponse)
		{
			if (!(null != witResponse) || !(witResponse.AsObject != null) || !witResponse.AsObject.HasChild("error"))
			{
				return string.Empty;
			}
			return witResponse["error"].Value;
		}

		public static string GetTranscription(this WitResponseNode witResponse)
		{
			if (!(null != witResponse) || !(witResponse.AsObject != null) || !witResponse.AsObject.HasChild("text"))
			{
				return string.Empty;
			}
			return witResponse["text"].Value;
		}

		public static WitResponseNode SafeGet(this WitResponseNode witResponse, string key)
		{
			WitResponseClass witResponseClass = (witResponse != null) ? witResponse.AsObject : null;
			if (!(witResponseClass != null) || !witResponseClass.HasChild(key))
			{
				return null;
			}
			return witResponseClass[key];
		}

		public static string GetRequestId(this WitResponseNode witResponse)
		{
			return ((witResponse != null) ? witResponse["client_request_id"].Value : null) ?? string.Empty;
		}

		public static string GetClientUserId(this WitResponseNode witResponse)
		{
			string text = ((witResponse != null) ? witResponse["client_user_id"].Value : null) ?? string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				return "unknown";
			}
			return text;
		}

		public static string GetResponseType(this WitResponseNode witResponse)
		{
			return (witResponse != null) ? witResponse["type"] : null;
		}

		public static WitResponseClass GetResponse(this WitResponseNode witResponse)
		{
			WitResponseClass result;
			if ((result = ((witResponse != null) ? witResponse.GetFinalResponse() : null)) == null)
			{
				if (witResponse == null)
				{
					return null;
				}
				result = witResponse.GetPartialResponse();
			}
			return result;
		}

		public static WitResponseClass GetFinalResponse(this WitResponseNode witResponse)
		{
			if (witResponse == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = witResponse.SafeGet("response");
			if (witResponseNode == null)
			{
				return null;
			}
			return witResponseNode.AsObject;
		}

		public static WitResponseClass GetPartialResponse(this WitResponseNode witResponse)
		{
			if (witResponse == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = witResponse.SafeGet("partial_response");
			if (witResponseNode == null)
			{
				return null;
			}
			return witResponseNode.AsObject;
		}

		public static bool GetIsFinal(this WitResponseNode witResponse)
		{
			bool? flag;
			if (witResponse == null)
			{
				flag = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse.SafeGet("is_final");
				flag = ((witResponseNode != null) ? new bool?(witResponseNode.AsBool) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}

		public static bool GetIsNlpPartial(this WitResponseNode witResponse)
		{
			return string.Equals((witResponse != null) ? witResponse.GetResponseType() : null, "PARTIAL_UNDERSTANDING");
		}

		public static bool GetIsNlpFinal(this WitResponseNode witResponse)
		{
			return string.Equals((witResponse != null) ? witResponse.GetResponseType() : null, "FINAL_UNDERSTANDING");
		}

		public static bool GetIsTranscriptionPartial(this WitResponseNode witResponse)
		{
			return string.Equals((witResponse != null) ? witResponse.GetResponseType() : null, "PARTIAL_TRANSCRIPTION") && !string.IsNullOrEmpty(witResponse["text"]);
		}

		public static bool GetIsTranscriptionFinal(this WitResponseNode witResponse)
		{
			return string.Equals((witResponse != null) ? witResponse.GetResponseType() : null, "FINAL_TRANSCRIPTION") && !string.IsNullOrEmpty(witResponse["text"]);
		}

		public static bool GetHasTranscription(this WitResponseNode witResponse)
		{
			string a = (witResponse != null) ? witResponse.GetResponseType() : null;
			return (string.Equals(a, "PARTIAL_TRANSCRIPTION") || string.Equals(a, "FINAL_TRANSCRIPTION")) && !string.IsNullOrEmpty(witResponse["text"]);
		}

		private static WitResponseArray GetArray(WitResponseNode witResponse, string key)
		{
			if (witResponse == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = witResponse.SafeGet(key);
			if (witResponseNode == null)
			{
				return null;
			}
			return witResponseNode.AsArray;
		}

		public static WitEntityData AsWitEntity(this WitResponseNode witResponse)
		{
			return new WitEntityData(witResponse);
		}

		public static WitEntityFloatData AsWitFloatEntity(this WitResponseNode witResponse)
		{
			return new WitEntityFloatData(witResponse);
		}

		public static WitEntityIntData AsWitIntEntity(this WitResponseNode witResponse)
		{
			return new WitEntityIntData(witResponse);
		}

		public static string GetFirstEntityValue(this WitResponseNode witResponse, string name)
		{
			if (witResponse == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = witResponse["entities"];
			if (witResponseNode == null)
			{
				return null;
			}
			WitResponseNode witResponseNode2 = witResponseNode[name];
			if (witResponseNode2 == null)
			{
				return null;
			}
			WitResponseNode witResponseNode3 = witResponseNode2[0];
			if (witResponseNode3 == null)
			{
				return null;
			}
			WitResponseNode witResponseNode4 = witResponseNode3["value"];
			if (witResponseNode4 == null)
			{
				return null;
			}
			return witResponseNode4.Value;
		}

		public static string[] GetAllEntityValues(this WitResponseNode witResponse, string name)
		{
			int? num;
			if (witResponse == null)
			{
				num = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				if (witResponseNode == null)
				{
					num = null;
				}
				else
				{
					WitResponseNode witResponseNode2 = witResponseNode[name];
					num = ((witResponseNode2 != null) ? new int?(witResponseNode2.Count) : null);
				}
			}
			int? num2 = num;
			string[] array = new string[num2.GetValueOrDefault()];
			int num3 = 0;
			for (;;)
			{
				int num4 = num3;
				int? num5;
				if (witResponse == null)
				{
					num5 = null;
				}
				else
				{
					WitResponseNode witResponseNode3 = witResponse["entities"];
					if (witResponseNode3 == null)
					{
						num5 = null;
					}
					else
					{
						WitResponseNode witResponseNode4 = witResponseNode3[name];
						num5 = ((witResponseNode4 != null) ? new int?(witResponseNode4.Count) : null);
					}
				}
				num2 = num5;
				if (!(num4 < num2.GetValueOrDefault() & num2 != null))
				{
					break;
				}
				string[] array2 = array;
				int num6 = num3;
				object obj;
				if (witResponse == null)
				{
					obj = null;
				}
				else
				{
					WitResponseNode witResponseNode5 = witResponse["entities"];
					if (witResponseNode5 == null)
					{
						obj = null;
					}
					else
					{
						WitResponseNode witResponseNode6 = witResponseNode5[name];
						if (witResponseNode6 == null)
						{
							obj = null;
						}
						else
						{
							WitResponseNode witResponseNode7 = witResponseNode6[num3];
							if (witResponseNode7 == null)
							{
								obj = null;
							}
							else
							{
								WitResponseNode witResponseNode8 = witResponseNode7["value"];
								obj = ((witResponseNode8 != null) ? witResponseNode8.Value : null);
							}
						}
					}
				}
				array2[num6] = obj;
				num3++;
			}
			return array;
		}

		public static WitResponseNode GetFirstEntity(this WitResponseNode witResponse, string name)
		{
			if (witResponse == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = witResponse["entities"];
			if (witResponseNode == null)
			{
				return null;
			}
			return witResponseNode[name][0];
		}

		public static WitEntityData GetFirstWitEntity(this WitResponseNode witResponse, string name)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			if (witResponseArray2 == null || witResponseArray2.Count <= 0)
			{
				return null;
			}
			return witResponseArray2[0].AsWitEntity();
		}

		public static WitEntityIntData GetFirstWitIntEntity(this WitResponseNode witResponse, string name)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			if (witResponseArray2 == null || witResponseArray2.Count <= 0)
			{
				return null;
			}
			return witResponseArray2[0].AsWitIntEntity();
		}

		public static int GetFirstWitIntValue(this WitResponseNode witResponse, string name, int defaultValue)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			if (null == witResponseArray2 || witResponseArray2.Count == 0)
			{
				return defaultValue;
			}
			return witResponseArray2[0].AsWitIntEntity().value;
		}

		public static WitEntityFloatData GetFirstWitFloatEntity(this WitResponseNode witResponse, string name)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			if (witResponseArray2 == null || witResponseArray2.Count <= 0)
			{
				return null;
			}
			return witResponseArray2[0].AsWitFloatEntity();
		}

		public static float GetFirstWitFloatValue(this WitResponseNode witResponse, string name, float defaultValue)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			if (null == witResponseArray2 || witResponseArray2.Count == 0)
			{
				return defaultValue;
			}
			return witResponseArray2[0].AsWitFloatEntity().value;
		}

		public static WitEntityData[] GetEntities(this WitResponseNode witResponse, string name)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			WitEntityData[] array = new WitEntityData[(witResponseArray2 != null) ? witResponseArray2.Count : 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = witResponseArray2[i].AsWitEntity();
			}
			return array;
		}

		public static int EntityCount(this WitResponseNode response)
		{
			int? num;
			if (response == null)
			{
				num = null;
			}
			else
			{
				WitResponseNode witResponseNode = response["entities"];
				if (witResponseNode == null)
				{
					num = null;
				}
				else
				{
					WitResponseArray asArray = witResponseNode.AsArray;
					num = ((asArray != null) ? new int?(asArray.Count) : null);
				}
			}
			int? num2 = num;
			return num2.GetValueOrDefault();
		}

		public static WitEntityFloatData[] GetFloatEntities(this WitResponseNode witResponse, string name)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			WitEntityFloatData[] array = new WitEntityFloatData[(witResponseArray2 != null) ? witResponseArray2.Count : 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = witResponseArray2[i].AsWitFloatEntity();
			}
			return array;
		}

		public static WitEntityIntData[] GetIntEntities(this WitResponseNode witResponse, string name)
		{
			WitResponseArray witResponseArray;
			if (witResponse == null)
			{
				witResponseArray = null;
			}
			else
			{
				WitResponseNode witResponseNode = witResponse["entities"];
				witResponseArray = ((witResponseNode != null) ? witResponseNode[name].AsArray : null);
			}
			WitResponseArray witResponseArray2 = witResponseArray;
			WitEntityIntData[] array = new WitEntityIntData[(witResponseArray2 != null) ? witResponseArray2.Count : 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = witResponseArray2[i].AsWitIntEntity();
			}
			return array;
		}

		public static WitIntentData AsWitIntent(this WitResponseNode witResponse)
		{
			return new WitIntentData(witResponse);
		}

		public static string GetIntentName(this WitResponseNode witResponse)
		{
			WitResponseNode firstIntent = witResponse.GetFirstIntent();
			if (firstIntent == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = firstIntent["name"];
			if (witResponseNode == null)
			{
				return null;
			}
			return witResponseNode.Value;
		}

		public static WitResponseNode GetFirstIntent(this WitResponseNode witResponse)
		{
			WitResponseArray array = WitResultUtilities.GetArray(witResponse, "intents");
			if (!(array == null) && array.Count != 0)
			{
				return array[0];
			}
			return null;
		}

		public static WitIntentData GetFirstIntentData(this WitResponseNode witResponse)
		{
			WitResponseNode firstIntent = witResponse.GetFirstIntent();
			if (firstIntent == null)
			{
				return null;
			}
			return firstIntent.AsWitIntent();
		}

		public static WitIntentData[] GetIntents(this WitResponseNode witResponse)
		{
			WitResponseArray array = WitResultUtilities.GetArray(witResponse, "intents");
			WitIntentData[] array2 = new WitIntentData[(array != null) ? array.Count : 0];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i].AsWitIntent();
			}
			return array2;
		}

		public static string GetPathValue(this WitResponseNode response, string path)
		{
			string[] array = path.Trim('.').Split('.', StringSplitOptions.None);
			WitResponseNode witResponseNode = response;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = WitResultUtilities.SplitArrays(array2[i]);
				witResponseNode = witResponseNode[array3[0]];
				for (int j = 1; j < array3.Length; j++)
				{
					witResponseNode = witResponseNode[int.Parse(array3[j])];
				}
			}
			return witResponseNode.Value;
		}

		public static void SetString(this WitResponseNode response, string path, string value)
		{
			string[] array = path.Trim('.').Split('.', StringSplitOptions.None);
			WitResponseNode witResponseNode = response;
			int i;
			for (i = 0; i < array.Length - 1; i++)
			{
				string[] array2 = WitResultUtilities.SplitArrays(array[i]);
				witResponseNode = witResponseNode[array2[0]];
				for (int j = 1; j < array2.Length; j++)
				{
					witResponseNode = witResponseNode[int.Parse(array2[j])];
				}
			}
			witResponseNode[array[i]] = value;
		}

		public static void RemovePath(this WitResponseNode response, string path)
		{
			string[] array = path.Trim('.').Split('.', StringSplitOptions.None);
			WitResponseNode witResponseNode = response;
			WitResponseNode witResponseNode2 = null;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = WitResultUtilities.SplitArrays(array2[i]);
				witResponseNode2 = witResponseNode;
				witResponseNode = witResponseNode[array3[0]];
				for (int j = 1; j < array3.Length; j++)
				{
					witResponseNode = witResponseNode[int.Parse(array3[j])];
				}
			}
			if (null != witResponseNode2)
			{
				witResponseNode2.Remove(witResponseNode);
			}
		}

		public static WitResponseReference GetWitResponseReference(string path)
		{
			string[] array = path.Trim('.').Split('.', StringSplitOptions.None);
			WitResponseReference witResponseReference = new WitResponseReference
			{
				path = path
			};
			WitResponseReference witResponseReference2 = witResponseReference;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = WitResultUtilities.SplitArrays(array2[i]);
				ObjectNodeReference objectNodeReference = new ObjectNodeReference
				{
					path = path
				};
				objectNodeReference.key = array3[0];
				witResponseReference2.child = objectNodeReference;
				witResponseReference2 = objectNodeReference;
				for (int j = 1; j < array3.Length; j++)
				{
					ArrayNodeReference arrayNodeReference = new ArrayNodeReference
					{
						path = path
					};
					arrayNodeReference.index = int.Parse(array3[j]);
					witResponseReference2.child = arrayNodeReference;
					witResponseReference2 = arrayNodeReference;
				}
			}
			return witResponseReference;
		}

		public static string GetCodeFromPath(string path)
		{
			string[] array = path.Trim('.').Split('.', StringSplitOptions.None);
			string str = "witResponse";
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = WitResultUtilities.SplitArrays(array2[i]);
				str = str + "[\"" + array3[0] + "\"]";
				for (int j = 1; j < array3.Length; j++)
				{
					str = str + "[" + array3[j] + "]";
				}
			}
			return str + ".Value";
		}

		private static string[] SplitArrays(string nodeName)
		{
			string[] array = nodeName.Split('[', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim(']');
			}
			return array;
		}

		public static string GetTraitValue(this WitResponseNode witResponse, string name)
		{
			if (witResponse == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = witResponse["traits"];
			if (witResponseNode == null)
			{
				return null;
			}
			WitResponseNode witResponseNode2 = witResponseNode[name];
			if (witResponseNode2 == null)
			{
				return null;
			}
			WitResponseNode witResponseNode3 = witResponseNode2[0];
			if (witResponseNode3 == null)
			{
				return null;
			}
			WitResponseNode witResponseNode4 = witResponseNode3["value"];
			if (witResponseNode4 == null)
			{
				return null;
			}
			return witResponseNode4.Value;
		}

		public const string WIT_KEY_TRANSCRIPTION = "text";

		public const string WIT_KEY_INTENTS = "intents";

		public const string WIT_KEY_ENTITIES = "entities";

		public const string WIT_KEY_TRAITS = "traits";

		public const string WIT_KEY_FINAL = "is_final";

		public const string WIT_PARTIAL_RESPONSE = "partial_response";

		public const string WIT_RESPONSE = "response";

		public const string WIT_STATUS_CODE = "code";

		public const string WIT_ERROR = "error";
	}
}
