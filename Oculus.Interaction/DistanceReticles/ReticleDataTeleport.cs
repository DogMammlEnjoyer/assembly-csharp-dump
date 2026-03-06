using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleDataTeleport : MonoBehaviour, IReticleData
	{
		[Obsolete("Use HideReticle instead")]
		public ReticleDataTeleport.TeleportReticleMode ReticleMode
		{
			get
			{
				return this._reticleMode;
			}
			set
			{
				this._reticleMode = value;
			}
		}

		public bool HideReticle
		{
			get
			{
				return this._hideReticle;
			}
			set
			{
				this._hideReticle = value;
			}
		}

		public Vector3 ProcessHitPoint(Vector3 hitPoint)
		{
			if (this._snapPoint != null)
			{
				return this._snapPoint.position;
			}
			return hitPoint;
		}

		public void Highlight(bool highlight)
		{
			if (this._materialBlock != null)
			{
				this._materialBlock.MaterialPropertyBlock.SetFloat(ReticleDataTeleport._highlightShaderID, highlight ? 1f : 0f);
				this._materialBlock.UpdateMaterialPropertyBlock();
			}
		}

		public void InjectOptionalSnapPoint(Transform snapPoint)
		{
			this._snapPoint = snapPoint;
		}

		public void InjectOptionalMaterialPropertyBlockEditor(MaterialPropertyBlockEditor materialBlock)
		{
			this._materialBlock = materialBlock;
		}

		[SerializeField]
		[Optional]
		private Transform _snapPoint;

		[SerializeField]
		[Optional]
		private MaterialPropertyBlockEditor _materialBlock;

		private static readonly int _highlightShaderID = Shader.PropertyToID("_Highlight");

		[Tooltip("Determines if the teleport reticle is hidden or marked as either valid or invalid when hovering over this spot.")]
		[SerializeField]
		[Obsolete("Use _hideReticle instead")]
		[Optional(OptionalAttribute.Flag.Obsolete)]
		private ReticleDataTeleport.TeleportReticleMode _reticleMode = ReticleDataTeleport.TeleportReticleMode.ValidTarget;

		[Tooltip("Determines if the teleport reticle is hidden when hovering over this spot.")]
		[SerializeField]
		public bool _hideReticle;

		[Obsolete]
		public enum TeleportReticleMode
		{
			Hidden,
			ValidTarget,
			InvalidTarget
		}
	}
}
