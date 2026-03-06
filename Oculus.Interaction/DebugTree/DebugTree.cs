using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Oculus.Interaction.DebugTree
{
	public abstract class DebugTree<TLeaf> where TLeaf : class
	{
		public DebugTree(TLeaf root)
		{
			this.Root = root;
		}

		public ITreeNode<TLeaf> GetRootNode()
		{
			return this._rootNode;
		}

		[Obsolete("Use async method instead.", true)]
		public void Rebuild()
		{
			throw new NotImplementedException();
		}

		public Task RebuildAsync()
		{
			DebugTree<TLeaf>.<RebuildAsync>d__7 <RebuildAsync>d__;
			<RebuildAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RebuildAsync>d__.<>4__this = this;
			<RebuildAsync>d__.<>1__state = -1;
			<RebuildAsync>d__.<>t__builder.Start<DebugTree<TLeaf>.<RebuildAsync>d__7>(ref <RebuildAsync>d__);
			return <RebuildAsync>d__.<>t__builder.Task;
		}

		private Task<DebugTree<TLeaf>.Node> BuildTreeAsync(TLeaf root)
		{
			DebugTree<TLeaf>.<BuildTreeAsync>d__8 <BuildTreeAsync>d__;
			<BuildTreeAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DebugTree<TLeaf>.Node>.Create();
			<BuildTreeAsync>d__.<>4__this = this;
			<BuildTreeAsync>d__.root = root;
			<BuildTreeAsync>d__.<>1__state = -1;
			<BuildTreeAsync>d__.<>t__builder.Start<DebugTree<TLeaf>.<BuildTreeAsync>d__8>(ref <BuildTreeAsync>d__);
			return <BuildTreeAsync>d__.<>t__builder.Task;
		}

		private Task<DebugTree<TLeaf>.Node> BuildTreeRecursiveAsync(TLeaf value)
		{
			DebugTree<TLeaf>.<BuildTreeRecursiveAsync>d__9 <BuildTreeRecursiveAsync>d__;
			<BuildTreeRecursiveAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DebugTree<TLeaf>.Node>.Create();
			<BuildTreeRecursiveAsync>d__.<>4__this = this;
			<BuildTreeRecursiveAsync>d__.value = value;
			<BuildTreeRecursiveAsync>d__.<>1__state = -1;
			<BuildTreeRecursiveAsync>d__.<>t__builder.Start<DebugTree<TLeaf>.<BuildTreeRecursiveAsync>d__9>(ref <BuildTreeRecursiveAsync>d__);
			return <BuildTreeRecursiveAsync>d__.<>t__builder.Task;
		}

		[Obsolete("Use async method instead.", true)]
		protected virtual bool TryGetChildren(TLeaf node, out IEnumerable<TLeaf> children)
		{
			throw new NotImplementedException();
		}

		protected abstract Task<IEnumerable<TLeaf>> TryGetChildrenAsync(TLeaf node);

		private Dictionary<TLeaf, DebugTree<TLeaf>.Node> _existingNodes = new Dictionary<TLeaf, DebugTree<TLeaf>.Node>();

		private readonly TLeaf Root;

		private DebugTree<TLeaf>.Node _rootNode;

		private class Node : ITreeNode<TLeaf>
		{
			TLeaf ITreeNode<!0>.Value
			{
				get
				{
					return this.Value;
				}
			}

			IEnumerable<ITreeNode<TLeaf>> ITreeNode<!0>.Children
			{
				get
				{
					return this.Children;
				}
			}

			public TLeaf Value { get; set; }

			public List<DebugTree<TLeaf>.Node> Children { get; set; }
		}
	}
}
