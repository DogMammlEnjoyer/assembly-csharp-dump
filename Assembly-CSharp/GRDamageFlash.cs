using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GRDamageFlash
{
	public void Setup()
	{
		this.flashRendererDefaultMaterial = new List<Material>(this.flashRenderers.Count);
		this.stateMachine = new SimpleStateMachine<GRDamageFlash.State>();
		for (int i = 0; i < this.flashRenderers.Count; i++)
		{
			this.flashRendererDefaultMaterial.Add(this.flashRenderers[i].sharedMaterial);
		}
		this.stateMachine.Setup(GRDamageFlash.State.Idle, new Action<GRDamageFlash.State>(this.OnStateStart), new Action<GRDamageFlash.State>(this.OnStateEnd), new Action<GRDamageFlash.State>(this.OnStateUpdate));
	}

	public void Play()
	{
		if (this.stateMachine.GetState() == GRDamageFlash.State.Idle)
		{
			this.stateMachine.SetState(GRDamageFlash.State.Playing, false);
		}
	}

	public void OnStateStart(GRDamageFlash.State state)
	{
		if (state == GRDamageFlash.State.Playing)
		{
			for (int i = 0; i < this.flashRenderers.Count; i++)
			{
				this.flashRenderers[i].material = this.flashMaterial;
			}
		}
	}

	public void OnStateEnd(GRDamageFlash.State state)
	{
		if (state == GRDamageFlash.State.Playing)
		{
			for (int i = 0; i < this.flashRenderers.Count; i++)
			{
				this.flashRenderers[i].material = this.flashRendererDefaultMaterial[i];
			}
		}
	}

	public void OnStateUpdate(GRDamageFlash.State state)
	{
		if (state != GRDamageFlash.State.Playing)
		{
			if (state != GRDamageFlash.State.Cooldown)
			{
				return;
			}
			if (this.stateMachine.IsStateFinished(Time.timeAsDouble, this.flashCooldown))
			{
				this.stateMachine.SetState(GRDamageFlash.State.Idle, false);
			}
		}
		else if (this.stateMachine.IsStateFinished(Time.timeAsDouble, this.flashDuration))
		{
			this.stateMachine.SetState((this.flashCooldown > 0f) ? GRDamageFlash.State.Cooldown : GRDamageFlash.State.Idle, false);
			return;
		}
	}

	public void Stop()
	{
		this.stateMachine.SetState(GRDamageFlash.State.Idle, false);
	}

	public void Update()
	{
		this.stateMachine.Update();
	}

	public Material flashMaterial;

	public float flashDuration = 0.1f;

	public float flashCooldown = 0.1f;

	public List<Renderer> flashRenderers;

	private SimpleStateMachine<GRDamageFlash.State> stateMachine;

	private List<Material> flashRendererDefaultMaterial;

	public enum State
	{
		Idle,
		Playing,
		Cooldown
	}
}
