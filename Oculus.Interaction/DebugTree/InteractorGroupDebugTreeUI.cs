using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.DebugTree
{
	public class InteractorGroupDebugTreeUI : DebugTreeUI<IInteractor>
	{
		protected override IInteractor Value
		{
			get
			{
				return this._root as IInteractor;
			}
		}

		protected override INodeUI<IInteractor> NodePrefab
		{
			get
			{
				return this._nodePrefab as INodeUI<IInteractor>;
			}
		}

		protected override DebugTree<IInteractor> CreateTree(IInteractor value)
		{
			return new InteractorGroupDebugTreeUI.InteractorGroupDebugTree(value);
		}

		protected override string TitleForValue(IInteractor value)
		{
			Object @object = value as Object;
			if (!(@object != null))
			{
				return "";
			}
			return @object.name;
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		private Object _root;

		[Tooltip("The node prefab which will be used to build the visual tree.")]
		[SerializeField]
		[Interface(typeof(INodeUI<IInteractor>), new Type[]
		{

		})]
		private Component _nodePrefab;

		private class InteractorGroupDebugTree : DebugTree<IInteractor>
		{
			public InteractorGroupDebugTree(IInteractor root) : base(root)
			{
			}

			protected override Task<IEnumerable<IInteractor>> TryGetChildrenAsync(IInteractor node)
			{
				if (node is InteractorGroup)
				{
					return Task.FromResult<IEnumerable<IInteractor>>((node as InteractorGroup).Interactors);
				}
				return Task.FromResult<IEnumerable<IInteractor>>(Enumerable.Empty<IInteractor>());
			}
		}
	}
}
