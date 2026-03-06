using System;
using System.Linq;

namespace UnityEngine.InputSystem
{
	public class InputActionReference : ScriptableObject
	{
		public InputActionAsset asset
		{
			get
			{
				return this.m_Asset;
			}
		}

		public InputAction action
		{
			get
			{
				if (this.m_Action == null)
				{
					if (this.m_Asset == null)
					{
						return null;
					}
					this.m_Action = this.m_Asset.FindAction(new Guid(this.m_ActionId));
				}
				return this.m_Action;
			}
		}

		public void Set(InputAction action)
		{
			if (action == null)
			{
				this.m_Asset = null;
				this.m_ActionId = null;
				return;
			}
			InputActionMap actionMap = action.actionMap;
			if (actionMap == null || actionMap.asset == null)
			{
				throw new InvalidOperationException(string.Format("Action '{0}' must be part of an InputActionAsset in order to be able to create an InputActionReference for it", action));
			}
			this.SetInternal(actionMap.asset, action);
		}

		public void Set(InputActionAsset asset, string mapName, string actionName)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (string.IsNullOrEmpty(mapName))
			{
				throw new ArgumentNullException("mapName");
			}
			if (string.IsNullOrEmpty(actionName))
			{
				throw new ArgumentNullException("actionName");
			}
			InputActionMap inputActionMap = asset.FindActionMap(mapName, false);
			if (inputActionMap == null)
			{
				throw new ArgumentException(string.Format("No action map '{0}' in '{1}'", mapName, asset), "mapName");
			}
			InputAction inputAction = inputActionMap.FindAction(actionName, false);
			if (inputAction == null)
			{
				throw new ArgumentException(string.Format("No action '{0}' in map '{1}' of asset '{2}'", actionName, mapName, asset), "actionName");
			}
			this.SetInternal(asset, inputAction);
		}

		private void SetInternal(InputActionAsset asset, InputAction action)
		{
			InputActionMap actionMap = action.actionMap;
			if (!asset.actionMaps.Contains(actionMap))
			{
				throw new ArgumentException(string.Format("Action '{0}' is not contained in asset '{1}'", action, asset), "action");
			}
			this.m_Asset = asset;
			this.m_ActionId = action.id.ToString();
			base.name = InputActionReference.GetDisplayName(action);
		}

		public override string ToString()
		{
			try
			{
				InputAction action = this.action;
				return string.Concat(new string[]
				{
					this.m_Asset.name,
					":",
					action.actionMap.name,
					"/",
					action.name
				});
			}
			catch
			{
				if (this.m_Asset != null)
				{
					return this.m_Asset.name + ":" + this.m_ActionId;
				}
			}
			return base.ToString();
		}

		internal static string GetDisplayName(InputAction action)
		{
			string value;
			if (action == null)
			{
				value = null;
			}
			else
			{
				InputActionMap actionMap = action.actionMap;
				value = ((actionMap != null) ? actionMap.name : null);
			}
			if (!string.IsNullOrEmpty(value))
			{
				InputActionMap actionMap2 = action.actionMap;
				return ((actionMap2 != null) ? actionMap2.name : null) + "/" + action.name;
			}
			if (action == null)
			{
				return null;
			}
			return action.name;
		}

		internal string ToDisplayName()
		{
			if (!string.IsNullOrEmpty(base.name))
			{
				return base.name;
			}
			return InputActionReference.GetDisplayName(this.action);
		}

		public static implicit operator InputAction(InputActionReference reference)
		{
			if (reference == null)
			{
				return null;
			}
			return reference.action;
		}

		public static InputActionReference Create(InputAction action)
		{
			if (action == null)
			{
				return null;
			}
			InputActionReference inputActionReference = ScriptableObject.CreateInstance<InputActionReference>();
			inputActionReference.Set(action);
			return inputActionReference;
		}

		internal static void ResetCachedAction()
		{
			Object[] array = Resources.FindObjectsOfTypeAll(typeof(InputActionReference));
			for (int i = 0; i < array.Length; i++)
			{
				((InputActionReference)array[i]).m_Action = null;
			}
		}

		public InputAction ToInputAction()
		{
			return this.action;
		}

		[SerializeField]
		internal InputActionAsset m_Asset;

		[SerializeField]
		internal string m_ActionId;

		[NonSerialized]
		private InputAction m_Action;
	}
}
