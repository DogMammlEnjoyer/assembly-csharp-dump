using System;
using UnityEngine;

public class SimpleStateMachine<State> where State : Enum
{
	public void Setup(State initialState, Action<State> onStateStart, Action<State> onStateEnd, Action<State> onStateUpdate)
	{
		this.onStateStart = onStateStart;
		this.onStateEnd = onStateEnd;
		this.onStateUpdate = onStateUpdate;
		this.stateStartTime = Time.timeAsDouble;
		this.currState = initialState;
		if (onStateStart != null)
		{
			onStateStart(this.currState);
		}
	}

	public void Update()
	{
		Action<State> action = this.onStateUpdate;
		if (action == null)
		{
			return;
		}
		action(this.currState);
	}

	public void SetState(State state, bool force = false)
	{
		if (!force && state.Equals(this.currState))
		{
			return;
		}
		Action<State> action = this.onStateEnd;
		if (action != null)
		{
			action(this.currState);
		}
		this.currState = state;
		this.stateStartTime = Time.timeAsDouble;
		Action<State> action2 = this.onStateStart;
		if (action2 == null)
		{
			return;
		}
		action2(this.currState);
	}

	public State GetState()
	{
		return this.currState;
	}

	public double GetStateStartTime()
	{
		return this.stateStartTime;
	}

	public bool IsStateFinished(double currTime, float stateDuration)
	{
		return currTime >= this.stateStartTime + (double)stateDuration;
	}

	private State currState;

	private double stateStartTime;

	private Action<State> onStateStart;

	private Action<State> onStateEnd;

	private Action<State> onStateUpdate;
}
