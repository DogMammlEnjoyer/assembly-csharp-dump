using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class KIDUI_AnimatedEllipsis : MonoBehaviour
{
	private void Awake()
	{
		if (this._ellipsisObjects != null)
		{
			return;
		}
		this.SetupEllipsis();
	}

	private void Start()
	{
	}

	private void OnDisable()
	{
		this.StopAnimation();
	}

	private void SetupEllipsis()
	{
		if (this._ellipsisRoot == null)
		{
			this._ellipsisRoot = base.gameObject;
		}
		this._ellipsisObjects = new ValueTuple<GameObject, float, float, float>[this._ellipsisStartingValues.Count];
		for (int i = 0; i < this._ellipsisStartingValues.Count; i++)
		{
			float num = this._ellipsisStartingValues[i];
			this._ellipsisObjects[i].Item1 = Object.Instantiate<GameObject>(this._ellipsisPrefab, this._ellipsisRoot.transform);
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			this._ellipsisObjects[i].Item2 = (this._ellipsisObjects[i].Item3 = num);
		}
	}

	private IEnumerator EllipsisAnimation()
	{
		int currIndex = 0;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				int num = i - currIndex;
				if (num < 0)
				{
					num = this._ellipsisStartingValues.Count + num;
				}
				float d = this._ellipsisStartingValues[num];
				this._ellipsisObjects[i].Item1.transform.localScale = Vector3.one * d;
			}
			int num2 = currIndex;
			currIndex = num2 + 1;
			if (currIndex >= this._ellipsisObjects.Length)
			{
				currIndex = 0;
			}
			yield return new WaitForSeconds(this._pauseBetweenScale);
		}
		yield break;
	}

	private IEnumerator EllipsisAnimation2()
	{
		float time = 0f;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				float offsetTime = this._scaleDuration / (float)(this._ellipsisObjects.Length + 1) * (float)i;
				float num = this.LerpLoop(this._startingScale, this._endScale, time, offsetTime, this._scaleDuration);
				this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			}
			time += Time.deltaTime * this._animationSpeedMultiplier;
			yield return null;
		}
		yield break;
	}

	public Task StartAnimation()
	{
		KIDUI_AnimatedEllipsis.<StartAnimation>d__24 <StartAnimation>d__;
		<StartAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAnimation>d__.<>4__this = this;
		<StartAnimation>d__.<>1__state = -1;
		<StartAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StartAnimation>d__24>(ref <StartAnimation>d__);
		return <StartAnimation>d__.<>t__builder.Task;
	}

	public Task StopAnimation()
	{
		KIDUI_AnimatedEllipsis.<StopAnimation>d__25 <StopAnimation>d__;
		<StopAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StopAnimation>d__.<>4__this = this;
		<StopAnimation>d__.<>1__state = -1;
		<StopAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StopAnimation>d__25>(ref <StopAnimation>d__);
		return <StopAnimation>d__.<>t__builder.Task;
	}

	public float LerpLoop(float start, float end, float time, float offsetTime, float duration)
	{
		float time2 = (offsetTime - time) % duration / duration;
		float t = this._ellipsisAnimationCurve.Evaluate(time2);
		return Mathf.Lerp(start, end, t);
	}

	[Header("Ellipsis Spawning")]
	[SerializeField]
	private bool _animateOnStart = true;

	[SerializeField]
	private int _ellipsisCount = 3;

	[SerializeField]
	private GameObject _ellipsisPrefab;

	[SerializeField]
	private GameObject _ellipsisRoot;

	[SerializeField]
	private List<float> _ellipsisStartingValues = new List<float>();

	[Header("Animation Settings")]
	[SerializeField]
	private bool _shouldLerp;

	[SerializeField]
	private AnimationCurve _ellipsisAnimationCurve;

	[SerializeField]
	private float _animationSpeedMultiplier = 0.25f;

	[SerializeField]
	private float _startingScale = 0.33f;

	[SerializeField]
	private float _intermediaryScale = 0.66f;

	[SerializeField]
	private float _endScale = 1f;

	[SerializeField]
	private float _scaleDuration = 0.25f;

	[SerializeField]
	private float _pauseBetweenScale = 0.25f;

	[SerializeField]
	private float _pauseBetweenCycles = 0.5f;

	private bool _runAnimation;

	private float _nextChange;

	[TupleElementNames(new string[]
	{
		"ellipsis",
		"startingScale",
		"currentScale",
		"lerpT"
	})]
	private ValueTuple<GameObject, float, float, float>[] _ellipsisObjects;

	private Coroutine _animationCoroutine;
}
