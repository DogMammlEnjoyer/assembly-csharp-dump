using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	public class CountDrivenEvents : MonoBehaviour
	{
		public int CurrentCount
		{
			get
			{
				return this.currentCount;
			}
		}

		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = (this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += this.OnCountChanged_SharedEvent;
				this._events.Deactivate += this.OnCountReached_SharedEvent;
			}
			if (this.evaluateOnEnable)
			{
				this.CheckTriggers(this.currentCount, this.currentCount);
			}
		}

		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnCountChanged_SharedEvent;
				this._events.Deactivate -= this.OnCountReached_SharedEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		private void OnValidate()
		{
			if (this.triggers == null)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount < 0)
				{
					this.triggers[i].triggerCount = 0;
				}
			}
		}

		public void Increment()
		{
			this.SetCount(this.currentCount + 1);
		}

		public void Decrement()
		{
			this.SetCount(this.currentCount - 1);
		}

		public void SetCount(int newCount)
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			int num = this.currentCount;
			if (this.wrapCount)
			{
				int highestTriggerCount = this.GetHighestTriggerCount();
				if (highestTriggerCount > 0)
				{
					int num2 = highestTriggerCount + 1;
					newCount = (newCount % num2 + num2) % num2;
				}
				else if (newCount < 0)
				{
					newCount = 0;
				}
			}
			else if (newCount < 0)
			{
				newCount = 0;
			}
			if (newCount == num)
			{
				return;
			}
			bool flag = false;
			this.currentCount = newCount;
			UnityEvent<int> unityEvent = this.onCountChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentCount);
			}
			UnityEvent<int> unityEvent2 = this.onCountChangedShared;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(this.currentCount);
			}
			if (this.currentCount > num)
			{
				UnityEvent<int> unityEvent3 = this.onCountIncreased;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(this.currentCount);
				}
				UnityEvent<int> unityEvent4 = this.onCountIncreasedShared;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke(this.currentCount);
				}
				flag = true;
			}
			else if (this.currentCount < num)
			{
				UnityEvent<int> unityEvent5 = this.onCountDecreased;
				if (unityEvent5 != null)
				{
					unityEvent5.Invoke(this.currentCount);
				}
				UnityEvent<int> unityEvent6 = this.onCountDecreasedShared;
				if (unityEvent6 != null)
				{
					unityEvent6.Invoke(this.currentCount);
				}
			}
			this.CheckTriggers(num, this.currentCount);
			if (this.currentCount == 0)
			{
				UnityEvent unityEvent7 = this.onCountResetToZero;
				if (unityEvent7 != null)
				{
					unityEvent7.Invoke();
				}
				UnityEvent unityEvent8 = this.onCountResetToZeroShared;
				if (unityEvent8 != null)
				{
					unityEvent8.Invoke();
				}
			}
			int highestTriggerCount2 = this.GetHighestTriggerCount();
			if (highestTriggerCount2 > 0 && this.currentCount == highestTriggerCount2)
			{
				UnityEvent unityEvent9 = this.onReachedMaxTrigger;
				if (unityEvent9 != null)
				{
					unityEvent9.Invoke();
				}
				UnityEvent unityEvent10 = this.onReachedMaxTriggerShared;
				if (unityEvent10 != null)
				{
					unityEvent10.Invoke();
				}
			}
			if (this.syncAllEvents && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				object[] args = new object[]
				{
					flag,
					this.currentCount
				};
				this._events.Activate.RaiseOthers(args);
			}
		}

		[Tooltip("Resets all 'triggerOnce' flags, allowing one-time triggers to fire again.\n\nUse this when restarting a sequence, resetting an object,\nor testing trigger behavior multiple times in play mode.")]
		public void ResetTriggers()
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				this.triggers[i].hasTriggered = false;
			}
		}

		private int GetHighestTriggerCount()
		{
			int num = 0;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount > num)
				{
					num = this.triggers[i].triggerCount;
				}
			}
			return num;
		}

		private void CheckTriggers(int oldCount, int newCount)
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				CountDrivenEvents.CountTrigger countTrigger = this.triggers[i];
				if (!countTrigger.triggerOnce || !countTrigger.hasTriggered)
				{
					bool flag = false;
					if (this.wrapCount)
					{
						if (newCount == countTrigger.triggerCount)
						{
							flag = true;
						}
					}
					else if (oldCount < countTrigger.triggerCount && newCount >= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount > countTrigger.triggerCount && newCount <= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount == newCount && newCount == countTrigger.triggerCount)
					{
						flag = true;
					}
					if (flag)
					{
						UnityEvent onCountReached = countTrigger.onCountReached;
						if (onCountReached != null)
						{
							onCountReached.Invoke();
						}
						UnityEvent onCountReachedShared = countTrigger.onCountReachedShared;
						if (onCountReachedShared != null)
						{
							onCountReachedShared.Invoke();
						}
						if (this.syncAllEvents && PhotonNetwork.InRoom && this._events != null && this._events.Deactivate != null)
						{
							object[] args = new object[]
							{
								i
							};
							this._events.Deactivate.RaiseOthers(args);
						}
						if (countTrigger.triggerOnce)
						{
							countTrigger.hasTriggered = true;
						}
					}
				}
			}
		}

		private void OnCountChanged_SharedEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnCountChanged_SharedEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			obj = args[1];
			if (!(obj is int))
			{
				return;
			}
			int num = (int)obj;
			UnityEvent<int> unityEvent = this.onCountChangedShared;
			if (unityEvent != null)
			{
				unityEvent.Invoke(num);
			}
			if (flag)
			{
				UnityEvent<int> unityEvent2 = this.onCountIncreasedShared;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(num);
				}
			}
			else
			{
				UnityEvent<int> unityEvent3 = this.onCountDecreasedShared;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(num);
				}
			}
			int highestTriggerCount = this.GetHighestTriggerCount();
			if (num != 0)
			{
				if (highestTriggerCount > 0 && num == highestTriggerCount)
				{
					UnityEvent unityEvent4 = this.onReachedMaxTriggerShared;
					if (unityEvent4 == null)
					{
						return;
					}
					unityEvent4.Invoke();
				}
				return;
			}
			UnityEvent unityEvent5 = this.onCountResetToZeroShared;
			if (unityEvent5 == null)
			{
				return;
			}
			unityEvent5.Invoke();
		}

		private void OnCountReached_SharedEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnCountReached_SharedEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is int))
			{
				return;
			}
			int num = (int)obj;
			if (num < 0 || num >= this.triggers.Count)
			{
				return;
			}
			UnityEvent onCountReachedShared = this.triggers[num].onCountReachedShared;
			if (onCountReachedShared == null)
			{
				return;
			}
			onCountReachedShared.Invoke();
		}

		[Header("Network")]
		[SerializeField]
		private bool syncAllEvents;

		[Header("General Settings")]
		[Tooltip("If true, triggers will be evaluated once on enable using the initial count.")]
		[SerializeField]
		private bool evaluateOnEnable;

		[Tooltip("If enabled, the counter value will loop between 0 and the highest triggerCount.")]
		[SerializeField]
		private bool wrapCount;

		[Header("Count Triggers")]
		[SerializeField]
		private List<CountDrivenEvents.CountTrigger> triggers = new List<CountDrivenEvents.CountTrigger>();

		[Header("Local and Networked Events")]
		public UnityEvent<int> onCountChanged;

		public UnityEvent<int> onCountChangedShared;

		public UnityEvent<int> onCountIncreased;

		public UnityEvent<int> onCountIncreasedShared;

		public UnityEvent<int> onCountDecreased;

		public UnityEvent<int> onCountDecreasedShared;

		public UnityEvent onCountResetToZero;

		public UnityEvent onCountResetToZeroShared;

		public UnityEvent onReachedMaxTrigger;

		public UnityEvent onReachedMaxTriggerShared;

		[Header("Debug - Counter Settings")]
		[SerializeField]
		private int currentCount;

		private RubberDuckEvents _events;

		private VRRig myRig;

		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		[Serializable]
		public class CountTrigger
		{
			[Tooltip("The count value that triggers this event")]
			public int triggerCount;

			[Tooltip("Events to invoke when count reaches this value")]
			public UnityEvent onCountReached;

			public UnityEvent onCountReachedShared;

			[Tooltip("Should this trigger fire every time the count passes through this value, or only once?")]
			public bool triggerOnce;

			[NonSerialized]
			public bool hasTriggered;
		}
	}
}
