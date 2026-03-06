using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ListLayoutEase
	{
		public ListLayoutEase(ListLayout layout, float curveTime = 0.3f, AnimationCurve curve = null)
		{
			this._curve = curve;
			this._curveTime = curveTime;
			if (this._curve == null)
			{
				this._curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			}
			this._elementDict = new Dictionary<int, ListLayoutEase.ListElementEase>();
			this._listLayout = layout;
			ListLayout listLayout = this._listLayout;
			listLayout.WhenElementAdded = (Action<int>)Delegate.Combine(listLayout.WhenElementAdded, new Action<int>(this.HandleElementAdded));
			ListLayout listLayout2 = this._listLayout;
			listLayout2.WhenElementUpdated = (Action<int, bool>)Delegate.Combine(listLayout2.WhenElementUpdated, new Action<int, bool>(this.HandleElementUpdated));
			ListLayout listLayout3 = this._listLayout;
			listLayout3.WhenElementRemoved = (Action<int>)Delegate.Combine(listLayout3.WhenElementRemoved, new Action<int>(this.HandleElementRemoved));
		}

		private void HandleElementAdded(int id)
		{
			float elementPosition = this._listLayout.GetElementPosition(id);
			this._elementDict.Add(id, new ListLayoutEase.ListElementEase(this._curve, this._curveTime, elementPosition));
		}

		private void HandleElementUpdated(int id, bool sizeUpdate)
		{
			this._elementDict[id].SetTarget(this._listLayout.GetElementPosition(id), this._time, sizeUpdate);
		}

		private void HandleElementRemoved(int id)
		{
			this._elementDict.Remove(id);
		}

		public void UpdateTime(float time)
		{
			this._time = time;
			foreach (ListLayoutEase.ListElementEase listElementEase in this._elementDict.Values)
			{
				listElementEase.UpdateTime(this._time);
			}
		}

		public float GetPosition(int id)
		{
			return this._elementDict[id].position;
		}

		private ListLayout _listLayout;

		private Dictionary<int, ListLayoutEase.ListElementEase> _elementDict;

		private AnimationCurve _curve;

		private float _curveTime;

		private float _time;

		private class ListElementEase
		{
			public ListElementEase(AnimationCurve curve, float easeTime, float position)
			{
				this._curve = curve;
				this._curveTime = easeTime;
				this.position = position;
				this._target = position;
				this._start = position;
			}

			public void SetTarget(float target, float time, bool skipEase)
			{
				this._target = target;
				if (!skipEase)
				{
					this._start = this.position;
					this._startTime = time;
					return;
				}
				this._start = target;
				this.position = target;
			}

			public void UpdateTime(float time)
			{
				float time2 = Mathf.Clamp01((time - this._startTime) / this._curveTime);
				float num = this._curve.Evaluate(time2);
				this.position = (this._target - this._start) * num + this._start;
			}

			private AnimationCurve _curve;

			private float _curveTime;

			private float _startTime;

			private float _start;

			private float _target;

			public float position;
		}
	}
}
