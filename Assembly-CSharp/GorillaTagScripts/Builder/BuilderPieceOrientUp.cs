using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		public void OnPieceDestroy()
		{
		}

		public void OnPiecePlacementDeserialized()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		public void OnPieceActivate()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		public void OnPieceDeactivate()
		{
		}

		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
