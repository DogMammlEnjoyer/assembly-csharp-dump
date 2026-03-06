using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fusion
{
	public static class NetworkRunnerVisibilityExtensions
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetAllSimulationStatics()
		{
			NetworkRunnerVisibilityExtensions.ResetStatics();
		}

		static NetworkRunnerVisibilityExtensions()
		{
			NetworkRunnerVisibilityExtensions.DictionaryLookup = new Dictionary<NetworkRunner, NetworkRunnerVisibilityExtensions.RunnerVisibility>();
		}

		public static void RetryRefreshCommonLinks()
		{
			NetworkRunnerVisibilityExtensions._commonLinksWithMissingInputAuthNeedRefresh = false;
			NetworkRunnerVisibilityExtensions.RefreshCommonObjectVisibilities();
		}

		public static void EnableVisibilityExtension(this NetworkRunner runner)
		{
			if (runner && !NetworkRunnerVisibilityExtensions.DictionaryLookup.ContainsKey(runner))
			{
				NetworkRunnerVisibilityExtensions.DictionaryLookup.Add(runner, new NetworkRunnerVisibilityExtensions.RunnerVisibility());
			}
		}

		public static void DisableVisibilityExtension(this NetworkRunner runner)
		{
			if (runner && NetworkRunnerVisibilityExtensions.DictionaryLookup.ContainsKey(runner))
			{
				NetworkRunnerVisibilityExtensions.DictionaryLookup.Remove(runner);
			}
		}

		public static bool HasVisibilityEnabled(this NetworkRunner runner)
		{
			return NetworkRunnerVisibilityExtensions.DictionaryLookup.ContainsKey(runner);
		}

		public static bool GetVisible(this NetworkRunner runner)
		{
			NetworkRunnerVisibilityExtensions.RunnerVisibility runnerVisibility;
			return !(runner == null) && (!NetworkRunnerVisibilityExtensions.DictionaryLookup.TryGetValue(runner, out runnerVisibility) || runnerVisibility.IsVisible);
		}

		public static void SetVisible(this NetworkRunner runner, bool isVisibile)
		{
			runner.GetVisibilityInfo().IsVisible = isVisibile;
			NetworkRunnerVisibilityExtensions.RefreshRunnerVisibility(runner, true);
		}

		private static LinkedList<RunnerVisibilityLink> GetVisibilityNodes(this NetworkRunner runner)
		{
			if (!runner)
			{
				return null;
			}
			NetworkRunnerVisibilityExtensions.RunnerVisibility visibilityInfo = runner.GetVisibilityInfo();
			if (visibilityInfo == null)
			{
				return null;
			}
			return visibilityInfo.Nodes;
		}

		private static NetworkRunnerVisibilityExtensions.RunnerVisibility GetVisibilityInfo(this NetworkRunner runner)
		{
			NetworkRunnerVisibilityExtensions.RunnerVisibility result;
			if (!NetworkRunnerVisibilityExtensions.DictionaryLookup.TryGetValue(runner, out result))
			{
				return null;
			}
			return result;
		}

		public static void AddVisibilityNodes(this NetworkRunner runner, GameObject go)
		{
			runner.EnableVisibilityExtension();
			if (go.GetComponent<RunnerVisibilityLinksRoot>())
			{
				return;
			}
			go.AddComponent<RunnerVisibilityLinksRoot>();
			EnableOnSingleRunner[] componentsInChildren = go.transform.GetComponentsInChildren<EnableOnSingleRunner>(true);
			List<RunnerVisibilityLink> existingNodes = go.GetComponentsInChildren<RunnerVisibilityLink>(false).ToList<RunnerVisibilityLink>();
			EnableOnSingleRunner[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddNodes(existingNodes);
			}
			NetworkRunnerVisibilityExtensions.CollectBehavioursAndAddNodes(go, runner, existingNodes);
			NetworkRunnerVisibilityExtensions.RefreshRunnerVisibility(runner, true);
		}

		private static void CollectBehavioursAndAddNodes(GameObject go, NetworkRunner runner, List<RunnerVisibilityLink> existingNodes)
		{
			bool flag = false;
			foreach (Component component in go.transform.GetComponentsInChildren<Component>(true))
			{
				bool flag2 = false;
				if (!(component == null))
				{
					foreach (RunnerVisibilityLink runnerVisibilityLink in existingNodes)
					{
						if (runnerVisibilityLink.Component == component)
						{
							flag2 = true;
							if (runnerVisibilityLink.IsOnSingleRunner)
							{
								NetworkRunnerVisibilityExtensions.AddNodeToCommonLookup(runnerVisibilityLink);
								NetworkRunnerVisibilityExtensions.RegisterNode(runnerVisibilityLink, runner, component);
								flag = true;
								break;
							}
							break;
						}
					}
					if (!flag2 && component.GetType().IsRecognizedByRunnerVisibility())
					{
						NetworkRunnerVisibilityExtensions.RegisterNode(component.gameObject.AddComponent<RunnerVisibilityLink>(), runner, component);
					}
				}
			}
			if (flag)
			{
				NetworkRunnerVisibilityExtensions._commonLinksWithMissingInputAuthNeedRefresh = true;
				NetworkRunnerVisibilityExtensions.RefreshCommonObjectVisibilities();
			}
		}

		internal static bool IsRecognizedByRunnerVisibility(this Type type)
		{
			Type[] recognizedBehaviourTypes = NetworkRunnerVisibilityExtensions.RecognizedBehaviourTypes;
			for (int i = 0; i < recognizedBehaviourTypes.Length; i++)
			{
				if (recognizedBehaviourTypes[i].IsAssignableFrom(type))
				{
					return true;
				}
			}
			string name = type.Name;
			foreach (string value in NetworkRunnerVisibilityExtensions.RecognizedBehaviourNames)
			{
				if (name.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		private static void RegisterNode(RunnerVisibilityLink link, NetworkRunner runner, Component comp)
		{
			runner.GetVisibilityNodes().AddLast(link);
			link.Initialize(comp, runner);
		}

		public static void UnregisterNode(this RunnerVisibilityLink link)
		{
			if (link == null || link._runner == null)
			{
				return;
			}
			NetworkRunner runner = link._runner;
			bool flag = !runner;
			if (!flag && link._runner.GetVisibilityNodes() == null)
			{
				return;
			}
			if (!flag && runner.GetVisibilityNodes().Contains(link))
			{
				runner.GetVisibilityNodes().Remove(link);
			}
			List<RunnerVisibilityLink> list;
			if (link.Guid != null && NetworkRunnerVisibilityExtensions.CommonObjectLookup.TryGetValue(link.Guid, out list))
			{
				if (list.Contains(link))
				{
					list.Remove(link);
				}
				if (list.Count == 0)
				{
					NetworkRunnerVisibilityExtensions.CommonObjectLookup.Remove(link.Guid);
				}
			}
		}

		private static void AddNodeToCommonLookup(RunnerVisibilityLink link)
		{
			string guid = link.Guid;
			if (string.IsNullOrEmpty(guid))
			{
				return;
			}
			List<RunnerVisibilityLink> list;
			if (!NetworkRunnerVisibilityExtensions.CommonObjectLookup.TryGetValue(guid, out list))
			{
				list = new List<RunnerVisibilityLink>();
				NetworkRunnerVisibilityExtensions.CommonObjectLookup.Add(guid, list);
			}
			list.Add(link);
		}

		private static void RefreshRunnerVisibility(NetworkRunner runner, bool refreshCommonObjects = true)
		{
			if (runner.GetVisibilityNodes() == null)
			{
				return;
			}
			bool visible = runner.GetVisible();
			foreach (RunnerVisibilityLink runnerVisibilityLink in runner.GetVisibilityNodes())
			{
				if (!(runnerVisibilityLink == null))
				{
					runnerVisibilityLink.SetEnabled(visible);
				}
			}
			if (refreshCommonObjects)
			{
				NetworkRunnerVisibilityExtensions.RefreshCommonObjectVisibilities();
			}
		}

		internal static void RefreshCommonObjectVisibilities()
		{
			List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
			NetworkRunner networkRunner = null;
			NetworkRunner networkRunner2 = null;
			NetworkRunner networkRunner3 = null;
			bool flag = false;
			while (instancesEnumerator.MoveNext())
			{
				NetworkRunner networkRunner4 = instancesEnumerator.Current;
				if (networkRunner4.IsRunning && networkRunner4.GetVisible() && !networkRunner4.IsShutdown)
				{
					if (networkRunner4.IsServer)
					{
						networkRunner = networkRunner4;
					}
					if (!networkRunner2 && networkRunner4.GameMode != GameMode.Server)
					{
						networkRunner2 = networkRunner4;
					}
					if (!networkRunner3)
					{
						networkRunner3 = networkRunner4;
					}
				}
			}
			foreach (KeyValuePair<string, List<RunnerVisibilityLink>> keyValuePair in NetworkRunnerVisibilityExtensions.CommonObjectLookup)
			{
				List<RunnerVisibilityLink> value = keyValuePair.Value;
				if (value.Count > 0)
				{
					RunnerVisibilityLink runnerVisibilityLink = value[0];
					NetworkRunner networkRunner5;
					switch (runnerVisibilityLink.PreferredRunner)
					{
					case RunnerVisibilityLink.PreferredRunners.Auto:
						networkRunner5 = networkRunner3;
						break;
					case RunnerVisibilityLink.PreferredRunners.Server:
						networkRunner5 = networkRunner;
						break;
					case RunnerVisibilityLink.PreferredRunners.Client:
						networkRunner5 = networkRunner2;
						break;
					default:
						networkRunner5 = null;
						break;
					}
					flag = false;
					foreach (RunnerVisibilityLink runnerVisibilityLink2 in value)
					{
						if (runnerVisibilityLink2.PreferredRunner == RunnerVisibilityLink.PreferredRunners.InputAuthority)
						{
							bool flag2 = runnerVisibilityLink2.IsInputAuth();
							runnerVisibilityLink2.Enabled = (flag2 && runnerVisibilityLink2._runner.GetVisible());
							flag = (flag || flag2);
						}
						else
						{
							runnerVisibilityLink2.Enabled = (runnerVisibilityLink2._runner == networkRunner5);
						}
					}
					if (runnerVisibilityLink.PreferredRunner == RunnerVisibilityLink.PreferredRunners.InputAuthority && !flag && NetworkRunnerVisibilityExtensions._commonLinksWithMissingInputAuthNeedRefresh)
					{
						NetworkRunnerVisibilityExtensions._commonLinksWithMissingInputAuthNeedRefresh = false;
						runnerVisibilityLink.InvokeRefreshCommonObjectVisibilities(1f);
					}
				}
			}
		}

		internal static void ResetStatics()
		{
			NetworkRunnerVisibilityExtensions.CommonObjectLookup.Clear();
		}

		private static readonly string[] RecognizedBehaviourNames = new string[]
		{
			"EventSystem"
		};

		private static readonly Type[] RecognizedBehaviourTypes = new Type[]
		{
			typeof(IRunnerVisibilityRecognizedType),
			typeof(Renderer),
			typeof(AudioListener),
			typeof(Camera),
			typeof(Canvas),
			typeof(Light)
		};

		private static readonly Dictionary<NetworkRunner, NetworkRunnerVisibilityExtensions.RunnerVisibility> DictionaryLookup;

		private static bool _commonLinksWithMissingInputAuthNeedRefresh;

		private static readonly Dictionary<string, List<RunnerVisibilityLink>> CommonObjectLookup = new Dictionary<string, List<RunnerVisibilityLink>>();

		private class RunnerVisibility
		{
			public bool IsVisible { get; set; } = true;

			public LinkedList<RunnerVisibilityLink> Nodes = new LinkedList<RunnerVisibilityLink>();
		}
	}
}
