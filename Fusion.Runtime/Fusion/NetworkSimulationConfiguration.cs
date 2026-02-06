using System;
using Fusion.Sockets;
using UnityEngine;

namespace Fusion
{
	[Serializable]
	public class NetworkSimulationConfiguration
	{
		public NetworkSimulationConfiguration Clone()
		{
			return (NetworkSimulationConfiguration)base.MemberwiseClone();
		}

		public NetConfigSimulation Create()
		{
			NetConfigSimulation defaults = NetConfigSimulation.Defaults;
			bool enabled = this.Enabled;
			if (enabled)
			{
				bool flag = this.DelayMin == 0.0 && this.DelayMax == 0.0;
				if (flag)
				{
					defaults.DelayOscillator.Min = 0.0;
					defaults.DelayOscillator.Max = 0.0;
				}
				else
				{
					bool flag2 = this.DelayMin > this.DelayMax;
					if (flag2)
					{
						defaults.DelayOscillator.Min = Math.Max(9.999999747378752E-05, this.DelayMax * 0.5);
						defaults.DelayOscillator.Max = Math.Max(9.999999747378752E-05, this.DelayMin * 0.5);
					}
					else
					{
						defaults.DelayOscillator.Min = Math.Max(9.999999747378752E-05, this.DelayMin * 0.5);
						defaults.DelayOscillator.Max = Math.Max(9.999999747378752E-05, this.DelayMax * 0.5);
					}
				}
				defaults.DelayOscillator.Period = this.DelayPeriod;
				defaults.DelayOscillator.Shape = this.DelayShape;
				defaults.DelayOscillator.Threshold = this.DelayThreshold;
				defaults.DelayOscillator.Additional = this.AdditionalJitter * 0.5;
				bool flag3 = this.LossChanceMin == 0.0 && this.LossChanceMax == 0.0;
				if (flag3)
				{
					defaults.LossOscillator.Min = 0.0;
					defaults.LossOscillator.Max = 0.0;
				}
				else
				{
					bool flag4 = this.LossChanceMin > this.LossChanceMax;
					if (flag4)
					{
						defaults.LossOscillator.Min = Math.Max(9.999999747378752E-05, this.LossChanceMax * 0.5);
						defaults.LossOscillator.Max = Math.Max(9.999999747378752E-05, this.LossChanceMin * 0.5);
					}
					else
					{
						defaults.LossOscillator.Min = Math.Max(9.999999747378752E-05, this.LossChanceMin * 0.5);
						defaults.LossOscillator.Max = Math.Max(9.999999747378752E-05, this.LossChanceMax * 0.5);
					}
				}
				defaults.LossOscillator.Period = this.LossChancePeriod;
				defaults.LossOscillator.Shape = this.LossChanceShape;
				defaults.LossOscillator.Threshold = this.LossChanceThreshold;
				defaults.LossOscillator.Additional = this.AdditionalLoss;
			}
			return defaults;
		}

		[InlineHelp]
		public bool Enabled;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		public NetConfigSimulationOscillator.WaveShape DelayShape = NetConfigSimulationOscillator.WaveShape.Noise;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.Seconds)]
		[RangeEx(0.0, 0.5)]
		public double DelayMin = 0.15;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.Seconds)]
		[RangeEx(0.0, 0.5)]
		public double DelayMax = 0.15;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.Seconds)]
		[RangeEx(0.0, 10.0)]
		public double DelayPeriod = 0.0;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.Seconds)]
		[RangeEx(0.0, 1.0)]
		public double DelayThreshold = 0.0;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.Seconds)]
		[RangeEx(0.0, 1.0)]
		public double AdditionalJitter = 0.05;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Space]
		public NetConfigSimulationOscillator.WaveShape LossChanceShape = NetConfigSimulationOscillator.WaveShape.Noise;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.NormalizedPercentage)]
		[RangeEx(0.0, 1.0)]
		public double LossChanceMin = 0.05;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.NormalizedPercentage)]
		[RangeEx(0.0, 1.0)]
		public double LossChanceMax = 0.05;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.NormalizedPercentage)]
		[RangeEx(0.0, 1.0)]
		public double LossChanceThreshold = 0.0;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.Seconds)]
		[RangeEx(0.0, 10.0)]
		public double LossChancePeriod = 0.0;

		[InlineHelp]
		[DrawIf("Enabled", Hide = true)]
		[Unit(Units.NormalizedPercentage)]
		[RangeEx(0.0, 1.0)]
		public double AdditionalLoss = 0.0;
	}
}
