using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Meta.WitAi;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.Conduit
{
	internal class Manifest
	{
		[Preserve]
		public Manifest()
		{
		}

		[Preserve]
		public string ID { get; set; }

		[Preserve]
		public string Version { get; set; }

		[Preserve]
		public string Domain { get; set; }

		[Preserve]
		public List<ManifestEntity> Entities { get; set; } = new List<ManifestEntity>();

		[Preserve]
		public List<ManifestAction> Actions { get; set; } = new List<ManifestAction>();

		[JsonIgnore]
		public Dictionary<string, Type> CustomEntityTypes { get; } = new Dictionary<string, Type>();

		public bool ResolveEntities()
		{
			bool result = true;
			foreach (ManifestEntity manifestEntity in this.Entities)
			{
				string text = (string.IsNullOrEmpty(manifestEntity.Namespace) ? manifestEntity.ID : (manifestEntity.Namespace + "." + manifestEntity.ID)) + "," + manifestEntity.Assembly;
				Type type = Type.GetType(text);
				if (type == null)
				{
					VLog.E(base.GetType().Name, "Failed to resolve type: " + text, null);
					result = false;
				}
				this.CustomEntityTypes[manifestEntity.Name] = type;
			}
			return result;
		}

		public Tuple<MethodInfo, Type> GetMethodInfo(IManifestMethod action)
		{
			if (action == null)
			{
				VLog.E(base.GetType().Name, "Cannot get MethodInfo without provided action", null);
				return null;
			}
			string id = action.ID;
			int num = string.IsNullOrEmpty(id) ? -1 : id.LastIndexOf('.');
			if (num <= 0)
			{
				VLog.E(base.GetType().Name, "Invalid Action ID: " + id, null);
				return null;
			}
			string text = id.Substring(0, num);
			string text2 = text;
			text2 += ((action.Assembly != null) ? ("," + action.Assembly) : "");
			string text3 = id.Substring(num + 1);
			Type type = Type.GetType(text2);
			if (type == null)
			{
				VLog.E(base.GetType().Name, "Failed to resolve type: " + text2, null);
				return null;
			}
			int num2 = (action.Parameters == null) ? 0 : action.Parameters.Count;
			Type[] array = new Type[num2];
			for (int i = 0; i < num2; i++)
			{
				ManifestParameter manifestParameter = action.Parameters[i];
				string text4 = manifestParameter.QualifiedTypeName + "," + manifestParameter.TypeAssembly;
				array[i] = Type.GetType(text4);
				if (array[i] == null)
				{
					VLog.E(base.GetType().Name, "Failed to resolve type: " + text4, null);
				}
			}
			MethodInfo bestMethodMatch = this.GetBestMethodMatch(type, text3, array);
			if (bestMethodMatch == null)
			{
				VLog.E(base.GetType().Name, string.Concat(new string[]
				{
					"Failed to resolve method ",
					text,
					".",
					text3,
					"."
				}), null);
				return null;
			}
			return Tuple.Create<MethodInfo, Type>(bestMethodMatch, type);
		}

		private bool ResolveAllActions()
		{
			bool result = true;
			foreach (ManifestAction manifestAction in this.Actions)
			{
				Tuple<MethodInfo, Type> methodInfo = this.GetMethodInfo(manifestAction);
				if (methodInfo == null)
				{
					return false;
				}
				MethodInfo item = methodInfo.Item1;
				Type item2 = methodInfo.Item2;
				if (item == null)
				{
					VLog.E(base.GetType().Name, "Invalid Action ID: " + manifestAction.ID, null);
					result = false;
				}
				else
				{
					object[] customAttributes = item.GetCustomAttributes(typeof(ConduitActionAttribute), false);
					if (customAttributes.Length == 0)
					{
						VLog.E(base.GetType().Name, string.Format("{0} - Did not have expected Conduit attribute", item), null);
						result = false;
					}
					else
					{
						ConduitActionAttribute conduitActionAttribute = customAttributes.First<object>() as ConduitActionAttribute;
						InvocationContext item3 = new InvocationContext
						{
							Type = item2,
							MethodInfo = item,
							MinConfidence = conduitActionAttribute.MinConfidence,
							MaxConfidence = conduitActionAttribute.MaxConfidence,
							ValidatePartial = conduitActionAttribute.ValidatePartial
						};
						if (!this._methodLookup.ContainsKey(manifestAction.Name))
						{
							this._methodLookup.Add(manifestAction.Name, new List<InvocationContext>());
						}
						this._methodLookup[manifestAction.Name].Add(item3);
					}
				}
			}
			using (IEnumerator<List<InvocationContext>> enumerator2 = (from invocationContext in this._methodLookup.Values
			where invocationContext.Count > 1
			select invocationContext).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.Sort((InvocationContext one, InvocationContext two) => two.MethodInfo.GetParameters().Length - one.MethodInfo.GetParameters().Length);
				}
			}
			return result;
		}

		private bool ResolveErrorHandlers()
		{
			if (this.ErrorHandlers == null)
			{
				return true;
			}
			bool result = true;
			foreach (ManifestErrorHandler manifestErrorHandler in this.ErrorHandlers)
			{
				Tuple<MethodInfo, Type> methodInfo = this.GetMethodInfo(manifestErrorHandler);
				MethodInfo item = methodInfo.Item1;
				Type item2 = methodInfo.Item2;
				if (item == null)
				{
					VLog.E(base.GetType().Name, "Invalid Action ID: " + manifestErrorHandler.ID, null);
					result = false;
				}
				else
				{
					object[] customAttributes = item.GetCustomAttributes(typeof(HandleEntityResolutionFailure), false);
					if (customAttributes.Length == 0)
					{
						VLog.E(base.GetType().Name, string.Format("{0} - Did not have expected Conduit attribute", item), null);
						result = false;
					}
					else if (!(customAttributes.First<object>() is HandleEntityResolutionFailure))
					{
						VLog.E(base.GetType().Name, "Found null attribute when one was expected", null);
					}
					else
					{
						InvocationContext item3 = new InvocationContext
						{
							Type = item2,
							MethodInfo = item,
							CustomAttributeType = typeof(HandleEntityResolutionFailure)
						};
						if (!this._methodLookup.ContainsKey(manifestErrorHandler.Name))
						{
							this._methodLookup.Add(manifestErrorHandler.Name, new List<InvocationContext>());
						}
						this._methodLookup[manifestErrorHandler.Name].Add(item3);
					}
				}
			}
			using (IEnumerator<List<InvocationContext>> enumerator2 = (from invocationContext in this._methodLookup.Values
			where invocationContext.Count > 1
			select invocationContext).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.Sort((InvocationContext one, InvocationContext two) => two.MethodInfo.GetParameters().Length - one.MethodInfo.GetParameters().Length);
				}
			}
			return result;
		}

		public bool ResolveActions()
		{
			return this.ResolveAllActions() && this.ResolveErrorHandlers();
		}

		private MethodInfo GetBestMethodMatch(Type targetType, string method, Type[] parameterTypes)
		{
			return targetType.GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, parameterTypes, null);
		}

		public bool ContainsAction(string actionId)
		{
			if (string.IsNullOrEmpty(actionId))
			{
				VLog.E(base.GetType().Name, "Null or empty action ID supplied", null);
				return false;
			}
			return this._methodLookup.ContainsKey(actionId);
		}

		public List<InvocationContext> GetInvocationContexts(string actionId)
		{
			if (!this._methodLookup.ContainsKey(actionId))
			{
				return null;
			}
			return this._methodLookup[actionId];
		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject<Manifest>(this, null, false);
		}

		public List<InvocationContext> GetErrorHandlerContexts()
		{
			List<InvocationContext> list = new List<InvocationContext>();
			foreach (List<InvocationContext> list2 in this._methodLookup.Values)
			{
				foreach (InvocationContext invocationContext in list2)
				{
					if (invocationContext.CustomAttributeType == typeof(HandleEntityResolutionFailure))
					{
						list.Add(invocationContext);
					}
				}
			}
			return list;
		}

		[Preserve]
		public List<ManifestErrorHandler> ErrorHandlers = new List<ManifestErrorHandler>();

		private readonly Dictionary<string, List<InvocationContext>> _methodLookup = new Dictionary<string, List<InvocationContext>>(StringComparer.OrdinalIgnoreCase);

		[JsonIgnore]
		public static List<string> WitResponseMatcherIntents = new List<string>();
	}
}
