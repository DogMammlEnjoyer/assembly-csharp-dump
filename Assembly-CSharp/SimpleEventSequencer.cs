using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEventSequencer : MonoBehaviour, IGorillaSliceableSimple
{
	public void StartSequence()
	{
		SimpleEventSequencer.<StartSequence>d__9 <StartSequence>d__;
		<StartSequence>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartSequence>d__.<>4__this = this;
		<StartSequence>d__.<>1__state = -1;
		<StartSequence>d__.<>t__builder.Start<SimpleEventSequencer.<StartSequence>d__9>(ref <StartSequence>d__);
	}

	private void startSequenceImmediate()
	{
		this.startTime = Time.time;
		this.idx = 0;
	}

	private void Awake()
	{
		this.enabledNodes.Clear();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			if (this.nodes[i].Enabled)
			{
				this.enabledNodes.Add(this.nodes[i]);
			}
		}
	}

	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (this.startOnEnable)
		{
			this.StartSequence();
		}
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.idx < 0 || this.idx == this.enabledNodes.Count)
		{
			return;
		}
		if (Time.time >= this.startTime + this.enabledNodes[this.idx].Time)
		{
			Debug.Log(string.Concat(new string[]
			{
				"SimpleEventSequencer :: ",
				base.name,
				" :: ",
				this.enabledNodes[this.idx].Name,
				" :: Invoke"
			}));
			UnityEvent unityEvent = this.enabledNodes[this.idx].UnityEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.startTime = Time.time;
			this.idx++;
			if (this.idx == this.enabledNodes.Count)
			{
				SimpleEventSequencer.OnCompleteAction onCompleteAction = this.onComplete;
				if (onCompleteAction != SimpleEventSequencer.OnCompleteAction.Disable)
				{
					if (onCompleteAction == SimpleEventSequencer.OnCompleteAction.Repeat)
					{
						this.StartSequence();
						return;
					}
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
		}
	}

	private void onValueChanged()
	{
		float num = 0f;
		for (int i = 0; i < this.nodes.Length; i++)
		{
			if (this.nodes[i].Enabled)
			{
				num += this.nodes[i].Time;
			}
			this.nodes[i].TotalTime = num;
			this.nodes[i].onValueChanged();
		}
	}

	public void SetOnCompleteActionDisable()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.Disable;
	}

	public void SetOnCompleteActionRepeat()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.Repeat;
	}

	public void ClearOnCompleteAction()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.None;
	}

	[SerializeField]
	private SimpleEventSequencer.SimpleEventSequencerNode[] nodes;

	[SerializeField]
	private bool startOnEnable = true;

	[SerializeField]
	private SimpleEventSequencer.OnCompleteAction onComplete = SimpleEventSequencer.OnCompleteAction.Disable;

	[SerializeField]
	private ServerTimeSyncRule serverTimeSync;

	private float startTime;

	private int idx = -1;

	private List<SimpleEventSequencer.SimpleEventSequencerNode> enabledNodes = new List<SimpleEventSequencer.SimpleEventSequencerNode>();

	[Serializable]
	private class SimpleEventSequencerNode
	{
		public float Time
		{
			get
			{
				return this.time;
			}
		}

		public UnityEvent UnityEvent
		{
			get
			{
				return this.unityEvent;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public float TotalTime
		{
			set
			{
				this.totalTime = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		public void onValueChanged()
		{
			if (this.enabled)
			{
				this.fancyName = string.Format("T+{0} ({1}) : {2}", this.totalTime, this.time, this.name);
				return;
			}
			this.fancyName = string.Format("Skip ({0}) : {1}", this.time, this.name);
		}

		private Color getGUIColor()
		{
			if (this.enabled)
			{
				return Color.white;
			}
			return Color.gray3;
		}

		[Tooltip("Uncheck to skip this node")]
		[SerializeField]
		private bool enabled = true;

		[Tooltip("Seconds after the previous node's events are dispatched")]
		[SerializeField]
		private float time;

		[Tooltip("This is just for legibilty. Doesn't matter what you name it.")]
		[SerializeField]
		private string name = "New Node";

		[SerializeField]
		private UnityEvent unityEvent;

		private string fancyName = "New Node";

		private float totalTime;
	}

	private enum OnCompleteAction
	{
		None,
		Disable,
		Repeat
	}
}
