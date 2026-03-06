using System;
using UnityEngine;

namespace Fusion
{
	[AddComponentMenu("")]
	public sealed class RunnerVisibilityLink : MonoBehaviour
	{
		public bool IsOnSingleRunner { get; private set; }

		public bool DefaultState
		{
			get
			{
				return this._originalState;
			}
			set
			{
				this._originalState = value;
			}
		}

		internal bool Enabled
		{
			get
			{
				if (this._componentType != RunnerVisibilityLink.ComponentType.Renderer)
				{
					return (this.Component as Behaviour).enabled;
				}
				return (this.Component as Renderer).enabled;
			}
			set
			{
				if (this.Component == null)
				{
					return;
				}
				if (this._componentType == RunnerVisibilityLink.ComponentType.Renderer)
				{
					(this.Component as Renderer).enabled = value;
					return;
				}
				(this.Component as Behaviour).enabled = value;
			}
		}

		private void Reset()
		{
			this._showAtRuntime = true;
			this.Guid = System.Guid.NewGuid().ToString();
		}

		private bool AssociateComponent(Component component)
		{
			this.Component = component;
			component.GetType();
			if (component as Renderer != null)
			{
				this._componentType = RunnerVisibilityLink.ComponentType.Renderer;
				return true;
			}
			if (component as Behaviour != null)
			{
				this._componentType = RunnerVisibilityLink.ComponentType.Behaviour;
				return true;
			}
			return false;
		}

		private void OnValidate()
		{
			if (this.Component != null)
			{
				if (this.Component.transform != base.transform)
				{
					Debug.LogWarning("RunnerVisibilityLink can only be associated with components on the same GameObject.");
					this.Component = null;
					return;
				}
				if (this.AssociateComponent(this.Component))
				{
					return;
				}
				Debug.LogWarning("RunnerVisibilityLink can only be associated with Components that can be enabled/disabled.");
				this.Component = null;
			}
		}

		private void Awake()
		{
			if (!this._showAtRuntime)
			{
				base.hideFlags = HideFlags.HideInInspector;
			}
		}

		private void OnDestroy()
		{
			this.UnregisterNode();
		}

		internal void Initialize(Component comp, NetworkRunner runner)
		{
			this._runner = runner;
			this._networkObject = base.GetComponentInChildren<NetworkObject>();
			if (!this._networkObject)
			{
				this._networkObject = base.GetComponentInParent<NetworkObject>();
			}
			if (!this._networkObject && this.PreferredRunner == RunnerVisibilityLink.PreferredRunners.InputAuthority)
			{
				Log.Warn("No NetworkObject found for RunnerVisibilityLink on " + base.gameObject.name + " with preferred runner as Input Authority. EnableOnSingleRunner will always disable it.");
			}
			Renderer renderer = comp as Renderer;
			if (renderer != null)
			{
				this._componentType = RunnerVisibilityLink.ComponentType.Renderer;
				this._originalState = renderer.enabled;
				renderer.enabled = (runner.GetVisible() && this._originalState);
				this.Component = comp;
				return;
			}
			Behaviour behaviour = comp as Behaviour;
			if (behaviour != null)
			{
				this._componentType = RunnerVisibilityLink.ComponentType.Behaviour;
				this._originalState = behaviour.enabled;
				behaviour.enabled = (runner.GetVisible() && this._originalState);
				this.Component = comp;
			}
		}

		public void SetEnabled(bool enabled)
		{
			if (enabled)
			{
				if (!this._originalState)
				{
					if (!this.Enabled)
					{
						return;
					}
					this._originalState = true;
				}
				this.Enabled = true;
				return;
			}
			this.Enabled = false;
		}

		internal bool IsInputAuth()
		{
			return this._networkObject && this._networkObject.IsValid && this._networkObject.HasInputAuthority;
		}

		internal void SetupOnSingleRunnerLink(RunnerVisibilityLink.PreferredRunners preferredRunner)
		{
			this.PreferredRunner = preferredRunner;
			this.IsOnSingleRunner = true;
		}

		internal void InvokeRefreshCommonObjectVisibilities(float time)
		{
			base.StopAllCoroutines();
			base.Invoke("RetryRefreshCommonLinks", time);
		}

		private void RetryRefreshCommonLinks()
		{
			NetworkRunnerVisibilityExtensions.RetryRefreshCommonLinks();
		}

		[SerializeField]
		public RunnerVisibilityLink.PreferredRunners PreferredRunner;

		public Component Component;

		[SerializeField]
		[ReadOnly]
		internal string Guid;

		[SerializeField]
		[HideInInspector]
		internal bool _showAtRuntime;

		internal NetworkRunner _runner;

		private RunnerVisibilityLink.ComponentType _componentType;

		private NetworkObject _networkObject;

		private bool _originalState;

		public enum PreferredRunners
		{
			Auto,
			Server,
			Client,
			InputAuthority
		}

		private enum ComponentType
		{
			None,
			Renderer,
			Behaviour
		}
	}
}
