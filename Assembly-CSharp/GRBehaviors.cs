using System;
using System.Collections.Generic;

public class GRBehaviors<T> : GRBehaviorsBase where T : Enum
{
	public void AddBehavior(T behavior, GRAbilityBase ability)
	{
		GRBehaviors<T>.BehaviorData item = new GRBehaviors<T>.BehaviorData
		{
			behavior = behavior,
			ability = ability
		};
		this.behaviorData.Add(item);
	}

	public List<GRBehaviors<T>.BehaviorData> behaviorData;

	public class BehaviorData
	{
		public T behavior;

		public GRAbilityBase ability;
	}
}
