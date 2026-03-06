using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Oculus.Interaction.DebugTree;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class ActiveStateDebugTree : DebugTree<IActiveState>
	{
		public ActiveStateDebugTree(IActiveState root) : base(root)
		{
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void RegisterModel<TType>(IActiveStateModel stateModel) where TType : class, IActiveState
		{
			ActiveStateDebugTree._models[typeof(TType)] = stateModel;
		}

		protected override Task<IEnumerable<IActiveState>> TryGetChildrenAsync(IActiveState node)
		{
			ActiveStateDebugTree.<TryGetChildrenAsync>d__3 <TryGetChildrenAsync>d__;
			<TryGetChildrenAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IEnumerable<IActiveState>>.Create();
			<TryGetChildrenAsync>d__.node = node;
			<TryGetChildrenAsync>d__.<>1__state = -1;
			<TryGetChildrenAsync>d__.<>t__builder.Start<ActiveStateDebugTree.<TryGetChildrenAsync>d__3>(ref <TryGetChildrenAsync>d__);
			return <TryGetChildrenAsync>d__.<>t__builder.Task;
		}

		private static Dictionary<Type, IActiveStateModel> _models = new Dictionary<Type, IActiveStateModel>();
	}
}
