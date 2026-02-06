using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

public class GREnemy : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	private void Awake()
	{
		this.damageFlash.Setup();
	}

	public void OnEntityInit()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	public void OnEntityDestroy()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	public static void HideRenderers(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	public static void HideObjects(List<GameObject> objects, bool hide)
	{
		if (objects == null)
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null)
			{
				objects[i].SetActive(!hide);
			}
		}
	}

	public void OnUpdate()
	{
		this.damageFlash.Update();
	}

	public void SetMaxHP(int maxHp)
	{
		if (this.healthMeter != null)
		{
			this.healthMeter.Setup(maxHp);
		}
	}

	public void SetHP(int newHp)
	{
		if (this.healthMeter != null)
		{
			this.healthMeter.SetHP(newHp);
		}
	}

	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	public void OnHit(GameHitData hit)
	{
		if (hit.hitAmount > 0)
		{
			this.damageFlash.Play();
		}
	}

	public GRHealthMeter healthMeter;

	public GREnemyType enemyType;

	public GameEntity gameEntity;

	public GRDamageFlash damageFlash;
}
