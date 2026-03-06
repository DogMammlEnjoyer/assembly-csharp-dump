using System;
using Oculus.Interaction;
using UnityEngine;

public class MoveRelativeToTarget : IMovement
{
	public Pose Pose
	{
		get
		{
			return this._current;
		}
	}

	public bool Stopped
	{
		get
		{
			return true;
		}
	}

	public void MoveTo(Pose target)
	{
		this._originalTarget = target;
		this._offset = PoseUtils.Delta(this._originalTarget, this._originalSource);
	}

	public void UpdateTarget(Pose target)
	{
		this._current = PoseUtils.Multiply(target, this._offset);
	}

	public void StopAndSetPose(Pose source)
	{
		this._originalSource = source;
		this._current = source;
	}

	public void Tick()
	{
	}

	private Pose _current = Pose.identity;

	private Pose _originalTarget;

	private Pose _originalSource;

	private Pose _offset = Pose.identity;
}
