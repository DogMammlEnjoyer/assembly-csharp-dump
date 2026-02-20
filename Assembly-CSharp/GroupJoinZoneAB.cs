using System;

[Serializable]
public struct GroupJoinZoneAB
{
	public static GroupJoinZoneAB operator &(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a & two.a),
			b = (one.b & two.b)
		};
	}

	public static GroupJoinZoneAB operator |(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a | two.a),
			b = (one.b | two.b)
		};
	}

	public static GroupJoinZoneAB operator ~(GroupJoinZoneAB z)
	{
		return new GroupJoinZoneAB
		{
			a = ~z.a,
			b = ~z.b
		};
	}

	public static bool operator ==(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a == two.a && one.b == two.b;
	}

	public static bool operator !=(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a != two.a || one.b != two.b;
	}

	public bool HasAnyFlag(GroupJoinZoneAB other)
	{
		return (this.a & other.a) != ~(GroupJoinZoneA.Basement | GroupJoinZoneA.Beach | GroupJoinZoneA.Cave | GroupJoinZoneA.Canyon | GroupJoinZoneA.City | GroupJoinZoneA.Clouds | GroupJoinZoneA.Forest | GroupJoinZoneA.Mountain | GroupJoinZoneA.Rotating | GroupJoinZoneA.Mines | GroupJoinZoneA.Arena | GroupJoinZoneA.ArenaTunnel | GroupJoinZoneA.Hoverboard | GroupJoinZoneA.TreeRoom | GroupJoinZoneA.MountainTunnel | GroupJoinZoneA.BasementTunnel | GroupJoinZoneA.RotatingTunnel | GroupJoinZoneA.BeachTunnel | GroupJoinZoneA.CloudsElevator | GroupJoinZoneA.MinesTunnel | GroupJoinZoneA.CavesComputer | GroupJoinZoneA.Metropolis | GroupJoinZoneA.MetropolisTunnel | GroupJoinZoneA.Attic | GroupJoinZoneA.Arcade | GroupJoinZoneA.ArcadeTunnel | GroupJoinZoneA.Bayou | GroupJoinZoneA.BayouTunnel | GroupJoinZoneA.CustomMaps | GroupJoinZoneA.MallConnector | GroupJoinZoneA.MonkeBlocks | GroupJoinZoneA.GTFC) || (this.b & other.b) > (GroupJoinZoneB)0;
	}

	public override bool Equals(object other)
	{
		return this == (GroupJoinZoneAB)other;
	}

	public override int GetHashCode()
	{
		return this.a.GetHashCode() ^ this.b.GetHashCode();
	}

	public static implicit operator GroupJoinZoneAB(int d)
	{
		return new GroupJoinZoneAB
		{
			a = (GroupJoinZoneA)d
		};
	}

	public override string ToString()
	{
		if (this.b == (GroupJoinZoneB)0)
		{
			return this.a.ToString();
		}
		if (this.a != ~(GroupJoinZoneA.Basement | GroupJoinZoneA.Beach | GroupJoinZoneA.Cave | GroupJoinZoneA.Canyon | GroupJoinZoneA.City | GroupJoinZoneA.Clouds | GroupJoinZoneA.Forest | GroupJoinZoneA.Mountain | GroupJoinZoneA.Rotating | GroupJoinZoneA.Mines | GroupJoinZoneA.Arena | GroupJoinZoneA.ArenaTunnel | GroupJoinZoneA.Hoverboard | GroupJoinZoneA.TreeRoom | GroupJoinZoneA.MountainTunnel | GroupJoinZoneA.BasementTunnel | GroupJoinZoneA.RotatingTunnel | GroupJoinZoneA.BeachTunnel | GroupJoinZoneA.CloudsElevator | GroupJoinZoneA.MinesTunnel | GroupJoinZoneA.CavesComputer | GroupJoinZoneA.Metropolis | GroupJoinZoneA.MetropolisTunnel | GroupJoinZoneA.Attic | GroupJoinZoneA.Arcade | GroupJoinZoneA.ArcadeTunnel | GroupJoinZoneA.Bayou | GroupJoinZoneA.BayouTunnel | GroupJoinZoneA.CustomMaps | GroupJoinZoneA.MallConnector | GroupJoinZoneA.MonkeBlocks | GroupJoinZoneA.GTFC))
		{
			return this.a.ToString() + "," + this.b.ToString();
		}
		return this.b.ToString();
	}

	public GroupJoinZoneA a;

	public GroupJoinZoneB b;
}
