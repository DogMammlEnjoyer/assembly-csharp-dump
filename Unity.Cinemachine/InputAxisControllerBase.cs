using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[SaveDuringPlay]
	public abstract class InputAxisControllerBase<T> : MonoBehaviour, IInputAxisController where T : IInputAxisReader, new()
	{
		public List<InputAxisControllerBase<T>.Controller> Controllers
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_ControllerManager.Controllers;
			}
		}

		protected virtual void OnValidate()
		{
			this.m_ControllerManager.Validate();
		}

		protected virtual void Reset()
		{
			this.ScanRecursively = true;
			this.SuppressInputWhileBlending = true;
			this.m_ControllerManager.Reset();
			this.SynchronizeControllers();
		}

		protected virtual void OnEnable()
		{
			this.SynchronizeControllers();
		}

		protected virtual void OnDisable()
		{
			this.m_ControllerManager.OnDisable();
		}

		public void SynchronizeControllers()
		{
			this.m_ControllerManager.CreateControllers(base.gameObject, this.ScanRecursively, base.enabled, new InputAxisControllerManager<T>.DefaultInitializer(this.InitializeControllerDefaultsForAxis));
		}

		protected virtual void InitializeControllerDefaultsForAxis(in IInputAxisOwner.AxisDescriptor axis, InputAxisControllerBase<T>.Controller controller)
		{
		}

		protected void UpdateControllers()
		{
			this.UpdateControllers(this.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
		}

		protected void UpdateControllers(float deltaTime)
		{
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase;
			if (this.SuppressInputWhileBlending && base.TryGetComponent<CinemachineVirtualCameraBase>(out cinemachineVirtualCameraBase) && cinemachineVirtualCameraBase.IsParticipatingInBlend())
			{
				return;
			}
			this.m_ControllerManager.UpdateControllers(this, deltaTime);
		}

		[Tooltip("If set, a recursive search for IInputAxisOwners behaviours will be performed.  Otherwise, only behaviours attached directly to this GameObject will be considered, and child objects will be ignored")]
		public bool ScanRecursively = true;

		[HideIfNoComponent(typeof(CinemachineVirtualCameraBase))]
		[Tooltip("If set, input will not be processed while the Cinemachine Camera is participating in a blend.")]
		public bool SuppressInputWhileBlending = true;

		public bool IgnoreTimeScale;

		[Header("Driven Axes")]
		[InputAxisControllerManager]
		[SerializeField]
		[NoSaveDuringPlay]
		internal InputAxisControllerManager<T> m_ControllerManager = new InputAxisControllerManager<T>();

		[Serializable]
		public class Controller
		{
			[HideInInspector]
			public string Name;

			[HideInInspector]
			public Object Owner;

			[Tooltip("When enabled, this controller will drive the input axis")]
			public bool Enabled = true;

			[HideFoldout]
			public T Input;

			public float InputValue;

			[HideFoldout]
			public DefaultInputAxisDriver Driver;
		}
	}
}
