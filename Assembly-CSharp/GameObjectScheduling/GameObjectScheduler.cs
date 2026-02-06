using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	public class GameObjectScheduler : MonoBehaviour, IGorillaSliceableSimple
	{
		private void Start()
		{
			GameObjectScheduler.<Start>d__8 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<GameObjectScheduler.<Start>d__8>(ref <Start>d__);
		}

		private void SetInitialState()
		{
			double num;
			this.getActiveState(out this.previousState, out num);
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(this.previousState);
				if (num > 0.0)
				{
					Animator[] componentsInChildren = this.scheduledGameObject[i].GetComponentsInChildren<Animator>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						int fullPathHash = componentsInChildren[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
						componentsInChildren[j].PlayInFixedTime(fullPathHash, 0, (float)num);
					}
				}
			}
			this.lastMinuteCheck = this.getServerTime().Minute;
		}

		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (this.ready)
			{
				this.SetInitialState();
			}
		}

		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		private void getActiveState(out bool state, out double totalSeconds)
		{
			DateTime serverTime = this.getServerTime();
			DateTime dateTime;
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(serverTime, out dateTime);
			if (this.currentNodeIndex == -1)
			{
				state = this.schedule.InitialState;
				totalSeconds = 0.0;
				return;
			}
			if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				state = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
				totalSeconds = (serverTime - this.schedule.Nodes[this.currentNodeIndex].DateTime).TotalSeconds;
				return;
			}
			state = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			totalSeconds = (serverTime - this.schedule.Nodes[this.schedule.Nodes.Length - 1].DateTime).TotalSeconds;
		}

		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		public void SliceUpdate()
		{
			if (!this.ready || (!this.useSecondsFidelity && this.getServerTime().Minute == this.lastMinuteCheck))
			{
				return;
			}
			bool flag;
			double num;
			this.getActiveState(out flag, out num);
			if (this.previousState != flag)
			{
				this.changeActiveState(flag);
				this.previousState = flag;
			}
			this.lastMinuteCheck = this.getServerTime().Minute;
		}

		[SerializeField]
		private GameObjectSchedule schedule;

		private GameObject[] scheduledGameObject;

		private GameObjectSchedulerEventDispatcher dispatcher;

		private int currentNodeIndex = -1;

		private bool ready;

		private bool previousState;

		private int lastMinuteCheck = -1;

		public bool useSecondsFidelity;

		public bool debugTime;
	}
}
