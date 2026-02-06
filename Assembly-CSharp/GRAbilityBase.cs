using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class GRAbilityBase
{
	protected virtual void OnStart()
	{
	}

	protected virtual void OnStop()
	{
	}

	protected virtual void OnThink(float dt)
	{
	}

	protected virtual void OnUpdateShared(float dt)
	{
	}

	protected virtual void OnUpdateRemote(float dt)
	{
	}

	protected virtual void OnUpdateAuthority(float dt)
	{
	}

	public virtual bool IsCoolDownOver()
	{
		return true;
	}

	public virtual void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.root = root;
		this.anim = anim;
		if (anim == null)
		{
			this.animator = null;
		}
		this.agent = agent;
		this.head = head;
		this.audioSource = audioSource;
		this.lineOfSight = lineOfSight;
		this.rb = agent.GetComponent<Rigidbody>();
		this.entity = agent.GetComponent<GameEntity>();
		this.attributes = agent.GetComponent<GRAttributes>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	public void Start()
	{
		this.startTime = Time.timeAsDouble;
		this.OnStart();
	}

	public void Stop()
	{
		this.stopTime = Time.timeAsDouble;
		this.OnStop();
	}

	public float GetAbilityTime(double currTime)
	{
		return (float)(currTime - this.startTime);
	}

	public virtual bool IsDone()
	{
		return false;
	}

	public void Think(float dt)
	{
		this.OnThink(dt);
	}

	public void UpdateAuthority(float dt)
	{
		this.OnUpdateShared(dt);
		this.OnUpdateAuthority(dt);
	}

	public void UpdateRemote(float dt)
	{
		this.OnUpdateShared(dt);
		this.OnUpdateRemote(dt);
	}

	protected virtual void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			if (this.anim.GetClip(animName) == null)
			{
				Debug.LogErrorFormat("Anim Clip {0} does not exist in (1)", new object[]
				{
					animName,
					this.anim
				});
				return;
			}
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	public bool IsCoolDownOver(float coolDown)
	{
		return (float)(Time.timeAsDouble - this.stopTime) > coolDown;
	}

	public virtual float GetRange()
	{
		return 0f;
	}

	protected GameAgent agent;

	protected GameEntity entity;

	protected Animation anim;

	protected Animator animator;

	protected Transform root;

	protected Transform head;

	protected AudioSource audioSource;

	protected GRSenseLineOfSight lineOfSight;

	protected Rigidbody rb;

	protected GRAttributes attributes;

	[ReadOnly]
	public double startTime;

	[ReadOnly]
	public double stopTime;

	protected int walkableArea = -1;
}
