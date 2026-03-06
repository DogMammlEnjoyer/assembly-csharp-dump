using System;
using Oculus.Interaction.DebugTree;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class ActiveStateDebugTreeUI : DebugTreeUI<IActiveState>
	{
		protected override IActiveState Value
		{
			get
			{
				return this._activeState as IActiveState;
			}
		}

		protected override INodeUI<IActiveState> NodePrefab
		{
			get
			{
				return this._nodePrefab as INodeUI<IActiveState>;
			}
		}

		protected override DebugTree<IActiveState> CreateTree(IActiveState value)
		{
			return new ActiveStateDebugTree(value);
		}

		protected override string TitleForValue(IActiveState value)
		{
			Object @object = value as Object;
			if (!(@object != null))
			{
				return "";
			}
			return @object.name;
		}

		[Tooltip("The IActiveState to debug.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		[Tooltip("The node prefab which will be used to build the visual tree.")]
		[SerializeField]
		[Interface(typeof(INodeUI<IActiveState>), new Type[]
		{

		})]
		private Component _nodePrefab;
	}
}
