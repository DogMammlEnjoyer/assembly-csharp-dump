using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public abstract class ActiveStateModel<TActiveState> : IActiveStateModel where TActiveState : class, IActiveState
	{
		public Task<IEnumerable<IActiveState>> GetChildrenAsync(IActiveState activeState)
		{
			ActiveStateModel<TActiveState>.<GetChildrenAsync>d__0 <GetChildrenAsync>d__;
			<GetChildrenAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IEnumerable<IActiveState>>.Create();
			<GetChildrenAsync>d__.<>4__this = this;
			<GetChildrenAsync>d__.activeState = activeState;
			<GetChildrenAsync>d__.<>1__state = -1;
			<GetChildrenAsync>d__.<>t__builder.Start<ActiveStateModel<TActiveState>.<GetChildrenAsync>d__0>(ref <GetChildrenAsync>d__);
			return <GetChildrenAsync>d__.<>t__builder.Task;
		}

		protected abstract Task<IEnumerable<IActiveState>> GetChildrenAsync(TActiveState instance);

		[Obsolete("Use async version of this method", true)]
		public IEnumerable<IActiveState> GetChildren(IActiveState activeState)
		{
			throw new NotImplementedException();
		}

		[Obsolete("Use async version of this method", true)]
		protected virtual IEnumerable<IActiveState> GetChildren(TActiveState activeState)
		{
			throw new NotImplementedException();
		}
	}
}
