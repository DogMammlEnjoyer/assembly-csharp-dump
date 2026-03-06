using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEventSequencer : MonoBehaviour, IGorillaSliceableSimple
{
	public void StartSequence()
	{
		this.idx = 0;
		this.startTime = Time.time;
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: Starting");
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
		if (this.startTime < 0f || this.idx == this.nodes.Length)
		{
			return;
		}
		if (Time.time >= this.startTime + (float)this.nodes[this.idx].Time)
		{
			Debug.Log(string.Concat(new string[]
			{
				"SimpleEventSequencer :: ",
				base.name,
				" :: ",
				this.nodes[this.idx].Name,
				" :: Invoke"
			}));
			UnityEvent unityEvent = this.nodes[this.idx].UnityEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.startTime = Time.time;
			this.idx++;
			if (this.disableOnComplete && this.idx == this.nodes.Length)
			{
				base.gameObject.SetActive(false);
				Debug.Log("SimpleEventSequencer :: " + base.name + " :: Disabling");
			}
		}
	}

	[SerializeField]
	private SimpleEventSequencer.SimpleEventSequencerNode[] nodes;

	[SerializeField]
	private bool startOnEnable = true;

	[SerializeField]
	private bool disableOnComplete = true;

	private float startTime = -1f;

	private int idx;

	[Serializable]
	private class SimpleEventSequencerNode
	{
		public int Time
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

		[Tooltip("This is just for legibilty. Doesn't matter what you name it.")]
		[SerializeField]
		private string name = "Untitled Node";

		[Tooltip("Seconds after the previous node's events are dispatched")]
		[SerializeField]
		private int time;

		[SerializeField]
		private UnityEvent unityEvent;
	}
}
