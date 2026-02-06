using System;
using GorillaTagScripts.GhostReactor;

[Serializable]
public struct GREnemyCount
{
	public GREnemyType GetEnemyType()
	{
		if (this.EnemyType == GREnemyType.MoonBoss_Phase1 || this.EnemyType == GREnemyType.MoonBoss_Phase2)
		{
			return GREnemyType.MoonBoss;
		}
		return this.EnemyType;
	}

	public string GetEnemyName()
	{
		if (this.GetEnemyType() == GREnemyType.MoonBoss)
		{
			return "Meteor Monster";
		}
		return this.GetEnemyType().ToString();
	}

	public GREnemyType EnemyType;

	public int Count;
}
