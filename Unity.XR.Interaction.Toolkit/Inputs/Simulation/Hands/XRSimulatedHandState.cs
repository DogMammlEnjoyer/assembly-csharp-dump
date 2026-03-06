using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands
{
	internal struct XRSimulatedHandState
	{
		public Vector3 position { readonly get; set; }

		public Quaternion rotation { readonly get; set; }

		public Vector3 euler { readonly get; set; }

		public bool isTracked { readonly get; set; }

		public HandExpressionName expressionName { readonly get; set; }

		public void Reset()
		{
			this.position = default(Vector3);
			this.rotation = Quaternion.identity;
			this.euler = default(Vector3);
			this.isTracked = false;
			this.expressionName = HandExpressionName.Default;
		}
	}
}
