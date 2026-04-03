using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

public class TitleDataDateRefActivation : MonoBehaviour, IGorillaSliceableSimple
{
	private void Initialize()
	{
		TitleDataDateRefActivation.<Initialize>d__6 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<TitleDataDateRefActivation.<Initialize>d__6>(ref <Initialize>d__);
	}

	private void onTD(string s)
	{
		try
		{
			this.nodeList.Clear();
			DateTime refTime = DateTime.Parse(s);
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Initialize(refTime);
				this.nodeList.Add(this.nodes[i]);
			}
			this.nodeList.Sort();
			this.readyState = TitleDataDateRefActivation.ReadyState.Ready;
		}
		catch (Exception ex)
		{
			Debug.Log("TitleDataDateRefActivation :: onTD :: " + ex.Message + " :: " + ex.StackTrace);
			this.readyState = TitleDataDateRefActivation.ReadyState.Crashed;
		}
	}

	private void onTDError(PlayFabError error)
	{
		Debug.Log(string.Format("TitleDataDateRefActivation :: onTDError :: {0}", error));
		this.readyState = TitleDataDateRefActivation.ReadyState.Crashed;
	}

	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.readyState != TitleDataDateRefActivation.ReadyState.Ready)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return;
		}
		if (this.nodeList.Count > 0 && this.nodeList[0].ActivationTime <= serverTime)
		{
			this.nodeList[0].Activate(serverTime);
			this.nodeList.RemoveAt(0);
		}
	}

	public void ManuallyTriggerNode(int i)
	{
		this.nodes[i].Activate();
	}

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private TitleDataDateRefActivation.TitleDataDateRefActivationTarget[] nodes;

	private TitleDataDateRefActivation.ReadyState readyState;

	private List<TitleDataDateRefActivation.TitleDataDateRefActivationTarget> nodeList = new List<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>();

	private enum ReadyState
	{
		None,
		Initializing,
		Ready,
		Crashed
	}

	[Serializable]
	private class TitleDataDateRefActivationTarget : IComparable<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>
	{
		private string gameObjectName
		{
			get
			{
				if (!(this.gameObject == null))
				{
					return (this.activationState ? "Activate " : "Deactivate ") + this.gameObject.name;
				}
				return "No Game Object";
			}
		}

		private string eventsLabel
		{
			get
			{
				return string.Format("Events [{0}]", this.payload.GetPersistentEventCount()) + (this.lateDispatch ? " *" : "");
			}
		}

		public DateTime ActivationTime
		{
			get
			{
				return this.dateTime;
			}
		}

		public void Initialize(DateTime refTime)
		{
			this.dateTime = refTime.AddHours((double)this.hrs).AddMinutes((double)this.min).AddSeconds((double)this.sec);
			this.gameObject.SetActive(this.initialState);
		}

		public void Activate(DateTime now)
		{
			float num = (float)(now - this.dateTime).TotalSeconds;
			PersistLog.Log(string.Format("TitleDataDateRefActivationTarget :: Activate :: Time:{0}", num));
			this.Activate(num);
		}

		public void Activate()
		{
			this.Activate(0f);
		}

		private void Activate(float late)
		{
			if (this.gameObject != null && (this.persistent || late < 1f))
			{
				this.gameObject.SetActive(this.activationState);
				if (this.activationState)
				{
					Animator[] componentsInChildren = this.gameObject.GetComponentsInChildren<Animator>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						int fullPathHash = componentsInChildren[i].GetCurrentAnimatorStateInfo(0).fullPathHash;
						componentsInChildren[i].PlayInFixedTime(fullPathHash, 0, late);
					}
				}
			}
			if (this.lateDispatch || late < 1f)
			{
				UnityEvent unityEvent = this.payload;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
			}
		}

		int IComparable<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>.CompareTo(TitleDataDateRefActivation.TitleDataDateRefActivationTarget other)
		{
			return this.dateTime.CompareTo(other.dateTime);
		}

		[SerializeField]
		private int hrs;

		[SerializeField]
		private int min;

		[SerializeField]
		private int sec;

		[SerializeField]
		private GameObject gameObject;

		[SerializeField]
		private bool activationState;

		[SerializeField]
		private bool initialState;

		[SerializeField]
		private bool persistent = true;

		[SerializeField]
		private UnityEvent payload;

		[SerializeField]
		private bool lateDispatch;

		private DateTime dateTime = DateTime.MaxValue;
	}
}
