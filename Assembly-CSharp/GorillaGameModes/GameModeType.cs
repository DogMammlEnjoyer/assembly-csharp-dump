using System;

namespace GorillaGameModes
{
	[Serializable]
	public enum GameModeType
	{
		Casual,
		Infection,
		HuntDown,
		Paintbrawl,
		Ambush,
		FreezeTag,
		Ghost,
		Custom,
		Guardian,
		PropHunt,
		InfectionCompetitive,
		SuperInfect,
		SuperCasual,
		Count,
		None = -1
	}
}
