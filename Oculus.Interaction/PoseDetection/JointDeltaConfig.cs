using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;

namespace Oculus.Interaction.PoseDetection
{
	public class JointDeltaConfig
	{
		public JointDeltaConfig(int instanceID, IEnumerable<HandJointId> jointIDs)
		{
			this.InstanceID = instanceID;
			this.JointIDs = jointIDs;
		}

		public readonly int InstanceID;

		public readonly IEnumerable<HandJointId> JointIDs;
	}
}
