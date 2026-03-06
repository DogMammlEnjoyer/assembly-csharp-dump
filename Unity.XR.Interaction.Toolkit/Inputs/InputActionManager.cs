using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
	[AddComponentMenu("Input/Input Action Manager")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager.html")]
	public class InputActionManager : MonoBehaviour
	{
		public List<InputActionAsset> actionAssets
		{
			get
			{
				return this.m_ActionAssets;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_ActionAssets = value;
			}
		}

		protected void OnEnable()
		{
			this.EnableInput();
		}

		protected void OnDisable()
		{
			this.DisableInput();
		}

		public void EnableInput()
		{
			if (this.m_ActionAssets == null)
			{
				return;
			}
			foreach (InputActionAsset inputActionAsset in this.m_ActionAssets)
			{
				if (inputActionAsset != null)
				{
					inputActionAsset.Enable();
				}
			}
		}

		public void DisableInput()
		{
			if (this.m_ActionAssets == null)
			{
				return;
			}
			foreach (InputActionAsset inputActionAsset in this.m_ActionAssets)
			{
				if (inputActionAsset != null)
				{
					inputActionAsset.Disable();
				}
			}
		}

		[SerializeField]
		[Tooltip("Input action assets to affect when inputs are enabled or disabled.")]
		private List<InputActionAsset> m_ActionAssets;
	}
}
