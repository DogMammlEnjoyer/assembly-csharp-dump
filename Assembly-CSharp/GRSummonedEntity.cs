using System;
using UnityEngine;

public class GRSummonedEntity : MonoBehaviour, IGameEntityComponent
{
	private void Awake()
	{
		this.entity = base.GetComponent<GameEntity>();
	}

	public void OnEntityInit()
	{
		this.summonerEntityId = this.entity.createdByEntityId;
		if (this.summonerEntityId.IsValid())
		{
			this.summoner = this.FindSummoner();
			if (this.summoner != null)
			{
				this.summoner.OnSummonedEntityInit(this.entity);
			}
		}
	}

	public GameEntityId GetSummonerID()
	{
		return this.summonerEntityId;
	}

	public void OnEntityDestroy()
	{
		if (this.summoner != null)
		{
			this.summoner.OnSummonedEntityDestroy(this.entity);
		}
	}

	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	private IGRSummoningEntity FindSummoner()
	{
		if (this.summonerEntityId.IsValid())
		{
			GameEntity gameEntity = GhostReactorManager.Get(this.entity).gameEntityManager.GetGameEntity(this.summonerEntityId);
			if (gameEntity != null)
			{
				return gameEntity.GetComponent<IGRSummoningEntity>();
			}
		}
		return null;
	}

	private GameEntityId summonerEntityId = GameEntityId.Invalid;

	private GameEntity entity;

	private IGRSummoningEntity summoner;
}
