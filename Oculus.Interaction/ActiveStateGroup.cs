using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection.Debug;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ActiveStateGroup : MonoBehaviour, IActiveState
	{
		protected virtual void Awake()
		{
			this.ActiveStates = this._activeStates.ConvertAll<IActiveState>((Object mono) => mono as IActiveState);
		}

		protected virtual void Start()
		{
		}

		public bool Active
		{
			get
			{
				if (this.ActiveStates == null)
				{
					return false;
				}
				switch (this._logicOperator)
				{
				case ActiveStateGroup.ActiveStateGroupLogicOperator.AND:
					using (List<IActiveState>.Enumerator enumerator = this.ActiveStates.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.Active)
							{
								return false;
							}
						}
					}
					return true;
				case ActiveStateGroup.ActiveStateGroupLogicOperator.OR:
					using (List<IActiveState>.Enumerator enumerator = this.ActiveStates.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Active)
							{
								return true;
							}
						}
					}
					return false;
				case ActiveStateGroup.ActiveStateGroupLogicOperator.XOR:
				{
					bool flag = false;
					using (List<IActiveState>.Enumerator enumerator = this.ActiveStates.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Active)
							{
								if (flag)
								{
									return false;
								}
								flag = true;
							}
						}
					}
					return flag;
				}
				default:
					return false;
				}
				bool result;
				return result;
			}
		}

		public void InjectAllActiveStateGroup(List<IActiveState> activeStates)
		{
			this.InjectActiveStates(activeStates);
		}

		public void InjectActiveStates(List<IActiveState> activeStates)
		{
			this.ActiveStates = activeStates;
			this._activeStates = activeStates.ConvertAll<Object>((IActiveState activeState) => activeState as Object);
		}

		public void InjectOptionalLogicOperator(ActiveStateGroup.ActiveStateGroupLogicOperator logicOperator)
		{
			this._logicOperator = logicOperator;
		}

		[Tooltip("The logic operator will be applied to these IActiveStates.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private List<Object> _activeStates;

		private List<IActiveState> ActiveStates;

		[Tooltip("IActiveStates will have this boolean logic operator applied.")]
		[SerializeField]
		private ActiveStateGroup.ActiveStateGroupLogicOperator _logicOperator;

		public enum ActiveStateGroupLogicOperator
		{
			AND,
			OR,
			XOR
		}

		private class DebugModel : ActiveStateModel<ActiveStateGroup>
		{
			protected override Task<IEnumerable<IActiveState>> GetChildrenAsync(ActiveStateGroup instance)
			{
				return Task.FromResult<IEnumerable<IActiveState>>(instance.ActiveStates);
			}
		}
	}
}
