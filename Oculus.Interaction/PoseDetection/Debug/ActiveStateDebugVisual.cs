using System;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class ActiveStateDebugVisual : MonoBehaviour
	{
		private IActiveState ActiveState { get; set; }

		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
			this._material = this._target.material;
			this.SetMaterialColor(this._lastActiveValue ? this._activeColor : this._normalColor);
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		protected virtual void Update()
		{
			bool active = this.ActiveState.Active;
			if (this._lastActiveValue != active)
			{
				this.SetMaterialColor(active ? this._activeColor : this._normalColor);
				this._lastActiveValue = active;
			}
		}

		private void SetMaterialColor(Color activeColor)
		{
			this._material.color = activeColor;
			this._target.enabled = (this._material.color.a > 0f);
		}

		[Tooltip("The IActiveState to debug.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		[Tooltip("The renderer used for the color change.")]
		[SerializeField]
		private Renderer _target;

		[Tooltip("The renderer will be set to this color when ActiveState is inactive.")]
		[SerializeField]
		private Color _normalColor = Color.red;

		[Tooltip("The renderer will be set to this color when ActiveState is active.")]
		[SerializeField]
		private Color _activeColor = Color.green;

		private Material _material;

		private bool _lastActiveValue;
	}
}
