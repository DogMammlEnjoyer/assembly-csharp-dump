using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Meta.WitAi;

namespace Meta.Conduit
{
	internal class ConduitDispatcher : IConduitDispatcher
	{
		public Manifest Manifest { get; private set; }

		public ConduitDispatcher(IManifestLoader manifestLoader, IInstanceResolver instanceResolver)
		{
			this._manifestLoader = manifestLoader;
			this._instanceResolver = instanceResolver;
		}

		public Task Initialize(string manifestFilePath)
		{
			ConduitDispatcher.<Initialize>d__11 <Initialize>d__;
			<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Initialize>d__.<>4__this = this;
			<Initialize>d__.manifestFilePath = manifestFilePath;
			<Initialize>d__.<>1__state = -1;
			<Initialize>d__.<>t__builder.Start<ConduitDispatcher.<Initialize>d__11>(ref <Initialize>d__);
			return <Initialize>d__.<>t__builder.Task;
		}

		public bool InvokeAction(IParameterProvider parameterProvider, string actionId, bool relaxed, float confidence = 1f, bool partial = false)
		{
			if (!this._isInitialized)
			{
				VLog.W("Conduit Manifest is not yet initialized", null);
				return false;
			}
			if (!this.Manifest.ContainsAction(actionId))
			{
				bool flag = Manifest.WitResponseMatcherIntents.Contains(actionId);
				if (!this._ignoredActionIds.Contains(actionId) && !flag)
				{
					this._ignoredActionIds.Add(actionId);
					this.InvokeError(actionId, new Exception("Conduit did not find intent '" + actionId + "' in manifest."));
					VLog.W("Conduit did not find intent '" + actionId + "' in manifest.", null);
				}
				return false;
			}
			parameterProvider.PopulateRoles(this._parameterToRoleMap);
			ConduitDispatcher.InvocationContextFilter invocationContextFilter = new ConduitDispatcher.InvocationContextFilter(parameterProvider, this.Manifest.GetInvocationContexts(actionId), relaxed);
			List<InvocationContext> list = invocationContextFilter.ResolveInvocationContexts(actionId, confidence, partial);
			if (list.Count < 1)
			{
				if (!partial && invocationContextFilter.ResolveInvocationContexts(actionId, confidence, true).Count < 1)
				{
					VLog.W(string.Concat(new string[]
					{
						"Failed to resolve ",
						partial ? "partial" : "final",
						" method for ",
						actionId,
						" with supplied context"
					}), null);
					this.InvokeError(actionId, new Exception(string.Concat(new string[]
					{
						"Failed to resolve ",
						partial ? "partial" : "final",
						" method for ",
						actionId,
						" with supplied context"
					})));
				}
				return false;
			}
			bool result = true;
			foreach (InvocationContext invocationContext in list)
			{
				try
				{
					if (!this.InvokeMethod(invocationContext, parameterProvider, relaxed))
					{
						result = false;
					}
				}
				catch (Exception ex)
				{
					VLog.W(string.Format("Failed to invoke {0}. {1}", invocationContext.MethodInfo.Name, ex), null);
					result = false;
					this.InvokeError(invocationContext.MethodInfo.Name, ex);
				}
			}
			return result;
		}

		public bool InvokeError(string actionId, Exception exception)
		{
			if (!this._isInitialized)
			{
				VLog.E(string.Format("Attempting to invoke error {0} ({1}) with no initialized manifest.", actionId, exception), null);
				return false;
			}
			foreach (InvocationContext invocationContext in this.Manifest.GetErrorHandlerContexts())
			{
				ParameterProvider parameterProvider = new ParameterProvider();
				parameterProvider.AddParameter("intent", actionId);
				parameterProvider.AddParameter("error", exception);
				this.InvokeMethod(invocationContext, parameterProvider, true);
			}
			return true;
		}

		private bool InvokeMethod(InvocationContext invocationContext, IParameterProvider parameterProvider, bool relaxed)
		{
			MethodInfo methodInfo = invocationContext.MethodInfo;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				StringBuilder arg = new StringBuilder();
				array[i] = parameterProvider.GetParameterValue(parameters[i], invocationContext.ParameterMap, relaxed);
				if (array[i] == null && !parameters[i].ParameterType.IsNullableType())
				{
					this.InvokeError(invocationContext.MethodInfo.Name, new Exception(string.Format("Failed to find method param while invoking\nType: {0}\nMethod: {1}\nParameter Issues\n{2}", invocationContext.Type.FullName, invocationContext.MethodInfo.Name, arg)));
					VLog.W(string.Format("Failed to find method param while invoking\nType: {0}\nMethod: {1}\nParameter Issues\n{2}", invocationContext.Type.FullName, invocationContext.MethodInfo.Name, arg), null);
					return false;
				}
			}
			if (methodInfo.IsStatic)
			{
				try
				{
					methodInfo.Invoke(null, array.ToArray<object>());
				}
				catch (Exception ex)
				{
					VLog.W(string.Format("Failed to invoke static method {0}. {1}", methodInfo.Name, ex), null);
					this.InvokeError(invocationContext.MethodInfo.Name, ex);
					return false;
				}
				return true;
			}
			bool result = true;
			foreach (object obj in this._instanceResolver.GetObjectsOfType(invocationContext.Type))
			{
				try
				{
					methodInfo.Invoke(obj, array.ToArray<object>());
				}
				catch (Exception ex2)
				{
					VLog.W(string.Format("Failed to invoke method {0}. {1} on {2}", methodInfo.Name, ex2, obj), null);
					result = false;
					this.InvokeError(invocationContext.MethodInfo.Name, ex2);
				}
			}
			return result;
		}

		private readonly IManifestLoader _manifestLoader;

		private readonly IInstanceResolver _instanceResolver;

		private bool _isInitializing;

		private bool _isInitialized;

		private readonly Dictionary<string, string> _parameterToRoleMap = new Dictionary<string, string>();

		private readonly HashSet<string> _ignoredActionIds = new HashSet<string>();

		internal class InvocationContextFilter
		{
			public InvocationContextFilter(IParameterProvider parameterProvider, List<InvocationContext> actionContexts, bool relaxed = false)
			{
				this._parameterProvider = parameterProvider;
				this._actionContexts = actionContexts;
				this._relaxed = relaxed;
			}

			public List<InvocationContext> ResolveInvocationContexts(string actionId, float confidence, bool partial)
			{
				if (this._actionContexts == null)
				{
					return new List<InvocationContext>();
				}
				return (from context in this._actionContexts
				where this.CompatibleInvocationContext(context, confidence, partial)
				select context).ToList<InvocationContext>();
			}

			private bool CompatibleInvocationContext(InvocationContext invocationContext, float confidence, bool partial)
			{
				Dictionary<string, string> parameterMap = new Dictionary<string, string>();
				ParameterInfo[] parameters = invocationContext.MethodInfo.GetParameters();
				if (invocationContext.ValidatePartial != partial)
				{
					return false;
				}
				if (invocationContext.MinConfidence > confidence || confidence > invocationContext.MaxConfidence)
				{
					return false;
				}
				HashSet<string> hashSet = new HashSet<string>();
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (ParameterInfo parameterInfo in parameters)
				{
					if (this._parameterProvider.ContainsParameter(parameterInfo, stringBuilder))
					{
						hashSet.Add(parameterInfo.Name);
					}
					else if (!parameterInfo.ParameterType.IsNullableType())
					{
						VLog.D((!this._relaxed) ? ("Could not find value for parameter: " + parameterInfo.Name) : ("Could not find exact value for parameter: " + parameterInfo.Name + ". Will attempt resolving by type."));
						flag = false;
					}
				}
				if (flag)
				{
					return true;
				}
				if (!this._relaxed)
				{
					VLog.D(string.Format("Failed to resolve parameters. \nType: {0}\nMethod: {1}\n{2}", invocationContext.Type.FullName, invocationContext.MethodInfo.Name, stringBuilder));
					return false;
				}
				HashSet<Type> actualTypes = new HashSet<Type>();
				return this.ResolveByType(invocationContext, parameters, hashSet, actualTypes, parameterMap);
			}

			private bool ResolveByType(InvocationContext invocationContext, ParameterInfo[] parameters, ICollection<string> exactMatches, ISet<Type> actualTypes, Dictionary<string, string> parameterMap)
			{
				Func<string, bool> <>9__0;
				foreach (ParameterInfo parameterInfo in parameters)
				{
					if (!exactMatches.Contains(parameterInfo.Name))
					{
						if (actualTypes.Contains(parameterInfo.ParameterType))
						{
							VLog.D(string.Format("Failed to resolve parameters by type. More than one value of type {0} were provided.", parameterInfo.ParameterType));
							return false;
						}
						actualTypes.Add(parameterInfo.ParameterType);
						IEnumerable<string> parameterNamesOfType = this._parameterProvider.GetParameterNamesOfType(parameterInfo.ParameterType);
						Func<string, bool> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((string parameterName) => !exactMatches.Contains(parameterName)));
						}
						List<string> list = parameterNamesOfType.Where(predicate).ToList<string>();
						if (list.Count != 1)
						{
							VLog.D("Failed to find compatible value for " + parameterInfo.Name);
							return false;
						}
						parameterMap[parameterInfo.Name] = list[0];
					}
				}
				invocationContext.ParameterMap = parameterMap;
				return true;
			}

			private readonly List<InvocationContext> _actionContexts;

			private readonly IParameterProvider _parameterProvider;

			private readonly bool _relaxed;
		}
	}
}
