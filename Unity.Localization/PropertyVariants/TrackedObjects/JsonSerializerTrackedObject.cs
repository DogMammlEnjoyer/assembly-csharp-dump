using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.Localization.Pseudo;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[Serializable]
	public abstract class JsonSerializerTrackedObject : TrackedObject
	{
		public JsonSerializerTrackedObject.ApplyChangesMethod UpdateType
		{
			get
			{
				return this.m_UpdateType;
			}
			set
			{
				this.m_UpdateType = value;
			}
		}

		public override void AddTrackedProperty(ITrackedProperty trackedProperty)
		{
			base.AddTrackedProperty(trackedProperty);
			if (trackedProperty.PropertyPath.Contains(".Array.data[") || trackedProperty.PropertyPath.EndsWith(".Array.size"))
			{
				this.UpdateType = JsonSerializerTrackedObject.ApplyChangesMethod.Full;
			}
		}

		public override AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale)
		{
			if (base.Target == null)
			{
				return default(AsyncOperationHandle);
			}
			JObject jsonObject;
			if (this.UpdateType == JsonSerializerTrackedObject.ApplyChangesMethod.Full)
			{
				string json = JsonUtility.ToJson(base.Target);
				jsonObject = JObject.Parse(json);
			}
			else
			{
				jsonObject = new JObject();
			}
			List<AsyncOperationHandle> asyncOperations = CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Get();
			List<ArraySizeTrackedProperty> arraySizes = CollectionPool<List<ArraySizeTrackedProperty>, ArraySizeTrackedProperty>.Get();
			bool flag = false;
			LocaleIdentifier defaultLocaleIdentifier = (defaultLocale != null) ? defaultLocale.Identifier : default(LocaleIdentifier);
			foreach (ITrackedProperty trackedProperty in base.TrackedProperties)
			{
				if (trackedProperty != null)
				{
					ArraySizeTrackedProperty arraySizeTrackedProperty = trackedProperty as ArraySizeTrackedProperty;
					if (arraySizeTrackedProperty == null)
					{
						IStringProperty stringProperty = trackedProperty as IStringProperty;
						if (stringProperty == null)
						{
							ITrackedPropertyValue<Object> trackedPropertyValue = trackedProperty as ITrackedPropertyValue<Object>;
							if (trackedPropertyValue == null)
							{
								LocalizedStringProperty localizedStringProperty = trackedProperty as LocalizedStringProperty;
								if (localizedStringProperty == null)
								{
									LocalizedAssetProperty localizedAssetProperty = trackedProperty as LocalizedAssetProperty;
									if (localizedAssetProperty != null)
									{
										if (!localizedAssetProperty.LocalizedObject.IsEmpty)
										{
											localizedAssetProperty.LocalizedObject.LocaleOverride = variantLocale;
											AsyncOperationHandle<Object> obj = localizedAssetProperty.LocalizedObject.LoadAssetAsObjectAsync();
											JValue jvalue = (JValue)JsonSerializerTrackedObject.GetPropertyFromPath(trackedProperty.PropertyPath + ".instanceID", jsonObject);
											if (obj.IsDone)
											{
												Object result = obj.Result;
												jvalue.Value = ((result != null) ? result.GetInstanceID() : 0);
												AddressablesInterface.Release(obj);
											}
											else if (localizedAssetProperty.LocalizedObject.ForceSynchronous)
											{
												Object @object = obj.WaitForCompletion();
												jvalue.Value = ((@object != null) ? @object.GetInstanceID() : 0);
												AddressablesInterface.Release(obj);
											}
											else
											{
												JsonSerializerTrackedObject.DeferredJsonObjectOperation deferredJsonObjectOperation = JsonSerializerTrackedObject.DeferredJsonObjectOperation.Pool.Get();
												deferredJsonObjectOperation.jsonValue = jvalue;
												obj.Completed += deferredJsonObjectOperation.callback;
												asyncOperations.Add(obj);
											}
											flag = true;
										}
									}
								}
								else if (!localizedStringProperty.LocalizedString.IsEmpty)
								{
									localizedStringProperty.LocalizedString.LocaleOverride = variantLocale;
									AsyncOperationHandle<string> localizedStringAsync = localizedStringProperty.LocalizedString.GetLocalizedStringAsync();
									JValue jvalue2 = (JValue)JsonSerializerTrackedObject.GetPropertyFromPath(trackedProperty.PropertyPath, jsonObject);
									if (localizedStringAsync.IsDone)
									{
										jvalue2.Value = localizedStringAsync.Result;
										AddressablesInterface.Release(localizedStringAsync);
									}
									else if (localizedStringProperty.LocalizedString.ForceSynchronous)
									{
										jvalue2.Value = localizedStringAsync.WaitForCompletion();
										AddressablesInterface.Release(localizedStringAsync);
									}
									else
									{
										JsonSerializerTrackedObject.DeferredJsonStringOperation deferredJsonStringOperation = JsonSerializerTrackedObject.DeferredJsonStringOperation.Pool.Get();
										deferredJsonStringOperation.jsonValue = jvalue2;
										localizedStringAsync.Completed += deferredJsonStringOperation.callback;
										asyncOperations.Add(localizedStringAsync);
									}
									flag = true;
								}
							}
							else
							{
								Object object2;
								trackedPropertyValue.GetValue(variantLocale.Identifier, defaultLocaleIdentifier, out object2);
								((JValue)JsonSerializerTrackedObject.GetPropertyFromPath(trackedProperty.PropertyPath + ".instanceID", jsonObject)).Value = ((object2 != null) ? object2.GetInstanceID() : 0);
								flag = true;
							}
						}
						else
						{
							string valueAsString = stringProperty.GetValueAsString(variantLocale.Identifier, defaultLocaleIdentifier);
							if (valueAsString != null)
							{
								JValue jvalue3 = (JValue)JsonSerializerTrackedObject.GetPropertyFromPath(trackedProperty.PropertyPath, jsonObject);
								PseudoLocale pseudoLocale = variantLocale as PseudoLocale;
								jvalue3.Value = ((pseudoLocale != null) ? pseudoLocale.GetPseudoString(valueAsString) : valueAsString);
								flag = true;
							}
						}
					}
					else
					{
						arraySizes.Add(arraySizeTrackedProperty);
						flag = true;
					}
				}
			}
			if (asyncOperations.Count > 0)
			{
				AsyncOperationHandle<IList<AsyncOperationHandle>> obj2 = AddressablesInterface.CreateGroupOperation(asyncOperations);
				obj2.Completed += delegate(AsyncOperationHandle<IList<AsyncOperationHandle>> res)
				{
					this.ApplyArraySizes(arraySizes, jsonObject, variantLocale.Identifier, defaultLocaleIdentifier);
					this.ApplyJson(jsonObject);
					foreach (AsyncOperationHandle handle in res.Result)
					{
						AddressablesInterface.Release(handle);
					}
					CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Release(asyncOperations);
					CollectionPool<List<ArraySizeTrackedProperty>, ArraySizeTrackedProperty>.Release(arraySizes);
					AddressablesInterface.Release(res);
				};
				return obj2;
			}
			if (flag)
			{
				this.ApplyArraySizes(arraySizes, jsonObject, variantLocale.Identifier, defaultLocaleIdentifier);
				this.ApplyJson(jsonObject);
			}
			CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Release(asyncOperations);
			CollectionPool<List<ArraySizeTrackedProperty>, ArraySizeTrackedProperty>.Release(arraySizes);
			return default(AsyncOperationHandle);
		}

		private void ApplyArraySizes(IEnumerable<ArraySizeTrackedProperty> arraySizes, JObject jsonObject, LocaleIdentifier variantLocale, LocaleIdentifier defaultLocale)
		{
			foreach (ArraySizeTrackedProperty arraySizeTrackedProperty in arraySizes)
			{
				JArray jarray = (JArray)JsonSerializerTrackedObject.GetPropertyFromPath(arraySizeTrackedProperty.PropertyPath, jsonObject);
				uint num;
				if (arraySizeTrackedProperty.GetValue(variantLocale, defaultLocale, out num))
				{
					if ((long)jarray.Count > (long)((ulong)num))
					{
						while ((long)jarray.Count > (long)((ulong)num))
						{
							jarray.RemoveAt(jarray.Count - 1);
						}
					}
					else if ((long)jarray.Count < (long)((ulong)num))
					{
						while ((long)jarray.Count < (long)((ulong)num))
						{
							jarray.Add(new JObject());
						}
					}
				}
			}
		}

		private void ApplyJson(JObject jsonObject)
		{
			JsonUtility.FromJsonOverwrite(jsonObject.ToString(), base.Target);
			this.PostApplyTrackedProperties();
		}

		internal static JsonSerializerTrackedObject.ArrayResult GetNextArrayItem(string path, int startIndex)
		{
			if (path.Length < startIndex + ".Array.".Length)
			{
				return new JsonSerializerTrackedObject.ArrayResult(null, -1, -1, -1);
			}
			int num = path.IndexOf(".Array.", startIndex, StringComparison.Ordinal);
			if (num != -1)
			{
				if (path.Length > num + ".Array.".Length + "data[".Length)
				{
					int num2 = path.IndexOf("data[", num + ".Array.".Length, StringComparison.Ordinal);
					if (num2 != -1)
					{
						num2 += "data[".Length;
						int num3 = path.IndexOf(']', num2);
						if (num3 != -1)
						{
							return new JsonSerializerTrackedObject.ArrayResult(path, num + 1, num2, num3);
						}
					}
				}
				if (path.Length == num + "size".Length + ".Array.".Length && path.EndsWith("size"))
				{
					return new JsonSerializerTrackedObject.ArrayResult(path, num + 1, -1, -1);
				}
			}
			return new JsonSerializerTrackedObject.ArrayResult(null, -1, -1, -1);
		}

		internal static JToken GetPropertyFromPath(string path, JContainer obj)
		{
			int num = 0;
			JsonSerializerTrackedObject.ArrayResult nextArrayItem = JsonSerializerTrackedObject.GetNextArrayItem(path, 0);
			JContainer jcontainer = obj;
			while (num != -1 && num < path.Length)
			{
				if (num == nextArrayItem.arrayStartIndex)
				{
					JArray jarray = jcontainer as JArray;
					if (jarray == null)
					{
						jarray = new JArray();
						jcontainer.Add(jarray);
					}
					if (nextArrayItem.IsArraySize)
					{
						return jarray;
					}
					int dataIndex = nextArrayItem.GetDataIndex();
					if (dataIndex == -1)
					{
						return null;
					}
					while (jarray.Count <= dataIndex)
					{
						jarray.Add(new JObject());
					}
					if (nextArrayItem.IsArrayElement)
					{
						JValue jvalue = jarray[dataIndex] as JValue;
						if (jvalue == null)
						{
							jvalue = new JValue(string.Empty);
							jarray[dataIndex] = jvalue;
						}
						return jvalue;
					}
					jcontainer = (jarray[dataIndex] as JObject);
					if (jcontainer == null)
					{
						jcontainer = new JObject();
						jarray[dataIndex] = jcontainer;
					}
					num = nextArrayItem.arrayDataIndexEnd + 2;
					nextArrayItem = JsonSerializerTrackedObject.GetNextArrayItem(path, num);
				}
				else
				{
					int num2 = path.IndexOf('.', num);
					string text = (num2 == -1) ? path.Substring(num) : path.Substring(num, num2 - num);
					if (num2 == -1)
					{
						JToken jtoken = jcontainer[text];
						JProperty jproperty = (JProperty)((jtoken != null) ? jtoken.Parent : null);
						JValue jvalue2;
						if (jproperty == null)
						{
							jvalue2 = new JValue(string.Empty);
							jproperty = new JProperty(text, jvalue2);
							jcontainer.Add(jproperty);
						}
						else
						{
							jvalue2 = (jproperty.Value as JValue);
							if (jvalue2 == null)
							{
								jvalue2 = new JValue(string.Empty);
								jproperty.Value = jvalue2;
							}
						}
						return jvalue2;
					}
					JContainer jcontainer2 = (JContainer)jcontainer[text];
					if (jcontainer2 == null)
					{
						jcontainer2 = new JObject();
						jcontainer[text] = jcontainer2;
					}
					jcontainer = jcontainer2;
					num = num2 + 1;
				}
			}
			return null;
		}

		[Tooltip("Determines the type of property update that will be performed.- Full update reads the entire object into JSON, patches the properties, then reapplies the new JSON.\n- Partial update generates a partial patch and applies the changes for the tracked properties only.\nPartial update provides better performance however is not supported when modifying collections or properties that contain a serialized version such as Rect.\nThis value is automatically set based on the properties tracked.")]
		[SerializeField]
		private JsonSerializerTrackedObject.ApplyChangesMethod m_UpdateType;

		public enum ApplyChangesMethod
		{
			Partial,
			Full
		}

		private class DeferredJsonStringOperation
		{
			public DeferredJsonStringOperation()
			{
				this.callback = new Action<AsyncOperationHandle<string>>(this.OnStringLoaded);
			}

			private void OnStringLoaded(AsyncOperationHandle<string> asyncOperationHandle)
			{
				this.jsonValue.Value = asyncOperationHandle.Result;
				this.jsonValue = null;
				JsonSerializerTrackedObject.DeferredJsonStringOperation.Pool.Release(this);
			}

			public JValue jsonValue;

			public readonly Action<AsyncOperationHandle<string>> callback;

			public static readonly ObjectPool<JsonSerializerTrackedObject.DeferredJsonStringOperation> Pool = new ObjectPool<JsonSerializerTrackedObject.DeferredJsonStringOperation>(() => new JsonSerializerTrackedObject.DeferredJsonStringOperation(), null, null, null, false, 10, 10000);
		}

		private class DeferredJsonObjectOperation
		{
			public DeferredJsonObjectOperation()
			{
				this.callback = new Action<AsyncOperationHandle<Object>>(this.OnAssetLoaded);
			}

			private void OnAssetLoaded(AsyncOperationHandle<Object> asyncOperationHandle)
			{
				this.jsonValue.Value = ((asyncOperationHandle.Result != null) ? asyncOperationHandle.Result.GetInstanceID() : 0);
				this.jsonValue = null;
				JsonSerializerTrackedObject.DeferredJsonObjectOperation.Pool.Release(this);
			}

			public JValue jsonValue;

			public readonly Action<AsyncOperationHandle<Object>> callback;

			public static readonly ObjectPool<JsonSerializerTrackedObject.DeferredJsonObjectOperation> Pool = new ObjectPool<JsonSerializerTrackedObject.DeferredJsonObjectOperation>(() => new JsonSerializerTrackedObject.DeferredJsonObjectOperation(), null, null, null, false, 10, 10000);
		}

		internal struct ArrayResult
		{
			public bool IsArraySize
			{
				get
				{
					return this.arrayStartIndex != -1 && this.arrayDataIndexStart == -1;
				}
			}

			public bool IsArrayElement
			{
				get
				{
					string text = this.path;
					int? num = (text != null) ? new int?(text.Length) : null;
					int num2 = this.arrayDataIndexEnd + 1;
					return num.GetValueOrDefault() == num2 & num != null;
				}
			}

			public int GetDataIndex()
			{
				if (this.arrayDataIndexStart == -1)
				{
					return -1;
				}
				string text = this.path.Substring(this.arrayDataIndexStart, this.arrayDataIndexEnd - this.arrayDataIndexStart);
				uint result;
				if (uint.TryParse(text, out result))
				{
					return (int)result;
				}
				Debug.LogError(string.Concat(new string[]
				{
					"Failed to parse Array index `",
					text,
					"` from property path `",
					this.path,
					"`"
				}));
				return -1;
			}

			public ArrayResult(string p, int start, int bracketStart, int bracketEnd)
			{
				this.path = p;
				this.arrayStartIndex = start;
				this.arrayDataIndexStart = bracketStart;
				this.arrayDataIndexEnd = bracketEnd;
			}

			public string path;

			public int arrayStartIndex;

			public int arrayDataIndexStart;

			public int arrayDataIndexEnd;
		}
	}
}
