using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEventSequencer : MonoBehaviour, IGorillaSliceableSimple
{
	private void StartSequence()
	{
		this.StartSequenceDelayed(0f);
	}

	public void StartSequenceDelayed(float delay)
	{
		SimpleEventSequencer.<StartSequenceDelayed>d__11 <StartSequenceDelayed>d__;
		<StartSequenceDelayed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartSequenceDelayed>d__.<>4__this = this;
		<StartSequenceDelayed>d__.delay = delay;
		<StartSequenceDelayed>d__.<>1__state = -1;
		<StartSequenceDelayed>d__.<>t__builder.Start<SimpleEventSequencer.<StartSequenceDelayed>d__11>(ref <StartSequenceDelayed>d__);
	}

	private void startSequenceImmediate()
	{
		this.startTime = Time.time;
		this.idx = 0;
	}

	private void startSequenceFrom(int i)
	{
		this.startTime = Time.time;
		this.idx = i;
	}

	private void stop(int i)
	{
		this.idx = -1;
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
						this.StartSequenceDelayed(this.enabledNodes[this.idx - 1].Time);
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

	public void TempAudio(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: TempAudio :: " + text);
	}

	public void TempVFX(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: TempVFX :: " + text);
	}

	public void Temp(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: Temp :: " + text);
	}

	public void DebugLog(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: DEBUG :: " + text);
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

	private SimpleEventSequencer.SimpleEventSequencerNode activeNode;

	[Serializable]
	private class SimpleEventSequencerNode
	{
		private string nameTrim
		{
			get
			{
				if (this.name.Length <= 33)
				{
					return this.name;
				}
				return this.name.Substring(0, 30) + "...";
			}
		}

		private string notesTrim
		{
			get
			{
				if (this.notes.Length <= 50)
				{
					return this.notes;
				}
				return this.notes.Substring(0, 47) + "...";
			}
		}

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
				this.fancyName = string.Format("T+{0} ({1}) : {2}", this.totalTime, this.time, this.nameTrim);
				return;
			}
			this.fancyName = string.Format("Skip ({0}) : {1}", this.time, this.nameTrim);
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

		[SerializeField]
		[TextArea(5, 10)]
		private string notes = "Notes";

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
