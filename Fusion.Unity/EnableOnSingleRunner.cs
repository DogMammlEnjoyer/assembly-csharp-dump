using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
	[AddComponentMenu("Fusion/Enable On Single Runner")]
	public class EnableOnSingleRunner : Behaviour
	{
		internal void AddNodes(List<RunnerVisibilityLink> existingNodes)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				bool flag = false;
				using (List<RunnerVisibilityLink>.Enumerator enumerator = existingNodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Component == this.Components[i])
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					RunnerVisibilityLink runnerVisibilityLink = this.Components[i].gameObject.AddComponent<RunnerVisibilityLink>();
					runnerVisibilityLink.Guid = this._guid + i.ToString();
					runnerVisibilityLink.Component = this.Components[i];
					runnerVisibilityLink.SetupOnSingleRunnerLink(this.PreferredRunner);
					existingNodes.Add(runnerVisibilityLink);
				}
			}
		}

		[EditorButton("Find on GameObject", EditorButtonVisibility.EditMode, 0, true)]
		public void FindRecognizedTypes()
		{
			this.Components = EnableOnSingleRunner.FindRecognizedComponentsOnGameObject(base.gameObject);
		}

		[EditorButton("Find in Nested Children", EditorButtonVisibility.EditMode, 0, true)]
		public void FindNestedRecognizedTypes()
		{
			this.Components = EnableOnSingleRunner.FindRecognizedNestedComponents(base.gameObject);
		}

		private static Component[] FindRecognizedComponentsOnGameObject(GameObject go)
		{
			Component[] result;
			try
			{
				go.GetComponents<Component>(EnableOnSingleRunner.reusableComponentsList);
				EnableOnSingleRunner.reusableComponentsList2.Clear();
				foreach (Component component in EnableOnSingleRunner.reusableComponentsList)
				{
					if (component.GetType().IsRecognizedByRunnerVisibility())
					{
						EnableOnSingleRunner.reusableComponentsList2.Add(component);
					}
				}
				result = EnableOnSingleRunner.reusableComponentsList2.ToArray();
			}
			finally
			{
				EnableOnSingleRunner.reusableComponentsList.Clear();
				EnableOnSingleRunner.reusableComponentsList2.Clear();
			}
			return result;
		}

		private static Component[] FindRecognizedNestedComponents(GameObject go)
		{
			Component[] result;
			try
			{
				go.transform.GetNestedComponentsInChildren(EnableOnSingleRunner.reusableComponentsList, true);
				EnableOnSingleRunner.reusableComponentsList2.Clear();
				foreach (Component component in EnableOnSingleRunner.reusableComponentsList)
				{
					if (component.GetType().IsRecognizedByRunnerVisibility())
					{
						EnableOnSingleRunner.reusableComponentsList2.Add(component);
					}
				}
				result = EnableOnSingleRunner.reusableComponentsList2.ToArray();
			}
			finally
			{
				EnableOnSingleRunner.reusableComponentsList.Clear();
				EnableOnSingleRunner.reusableComponentsList2.Clear();
			}
			return result;
		}

		[InlineHelp]
		[SerializeField]
		public RunnerVisibilityLink.PreferredRunners PreferredRunner;

		[InlineHelp]
		public Component[] Components = new Component[0];

		[HideInInspector]
		[SerializeField]
		private string _guid = Guid.NewGuid().ToString().Substring(0, 19);

		private static readonly List<Component> reusableComponentsList = new List<Component>();

		private static readonly List<Component> reusableComponentsList2 = new List<Component>();
	}
}
