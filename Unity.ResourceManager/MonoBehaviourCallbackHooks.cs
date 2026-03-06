using System;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

internal class MonoBehaviourCallbackHooks : ComponentSingleton<MonoBehaviourCallbackHooks>
{
	public event Action<float> OnUpdateDelegate
	{
		add
		{
			this.m_OnUpdateDelegate = (Action<float>)Delegate.Combine(this.m_OnUpdateDelegate, value);
		}
		remove
		{
			this.m_OnUpdateDelegate = (Action<float>)Delegate.Remove(this.m_OnUpdateDelegate, value);
		}
	}

	internal event Action<float> OnLateUpdateDelegate
	{
		add
		{
			this.m_OnLateUpdateDelegate = (Action<float>)Delegate.Combine(this.m_OnLateUpdateDelegate, value);
		}
		remove
		{
			this.m_OnLateUpdateDelegate = (Action<float>)Delegate.Remove(this.m_OnLateUpdateDelegate, value);
		}
	}

	protected override string GetGameObjectName()
	{
		return "ResourceManagerCallbacks";
	}

	internal void Update()
	{
		Action<float> onUpdateDelegate = this.m_OnUpdateDelegate;
		if (onUpdateDelegate == null)
		{
			return;
		}
		onUpdateDelegate(Time.unscaledDeltaTime);
	}

	internal void LateUpdate()
	{
		Action<float> onLateUpdateDelegate = this.m_OnLateUpdateDelegate;
		if (onLateUpdateDelegate == null)
		{
			return;
		}
		onLateUpdateDelegate(Time.unscaledDeltaTime);
	}

	internal Action<float> m_OnUpdateDelegate;

	internal Action<float> m_OnLateUpdateDelegate;
}
