using System;

namespace Oculus.Interaction.PoseDetection
{
	public class FeatureStateDescription
	{
		public FeatureStateDescription(string id, string name)
		{
			this.Id = id;
			this.Name = name;
		}

		public string Id { get; }

		public string Name { get; }
	}
}
