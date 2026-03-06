using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	internal static class XRSimulatorUtility
	{
		internal static SimulatedDeviceLifecycleManager FindCreateSimulatedDeviceLifecycleManager(GameObject simulator)
		{
			SimulatedDeviceLifecycleManager result;
			if (ComponentLocatorUtility<SimulatedDeviceLifecycleManager>.TryFindComponent(out result))
			{
				return result;
			}
			result = simulator.AddComponent<SimulatedDeviceLifecycleManager>();
			return result;
		}

		internal static SimulatedHandExpressionManager FindCreateSimulatedHandExpressionManager(GameObject simulator)
		{
			SimulatedHandExpressionManager result;
			if (ComponentLocatorUtility<SimulatedHandExpressionManager>.TryFindComponent(out result))
			{
				return result;
			}
			result = simulator.AddComponent<SimulatedHandExpressionManager>();
			return result;
		}

		internal static void Subscribe(InputActionReference reference, Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
		{
			InputAction inputAction = XRSimulatorUtility.GetInputAction(reference);
			if (inputAction != null)
			{
				if (performed != null)
				{
					inputAction.performed += performed;
				}
				if (canceled != null)
				{
					inputAction.canceled += canceled;
				}
			}
		}

		internal static void Unsubscribe(InputActionReference reference, Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
		{
			InputAction inputAction = XRSimulatorUtility.GetInputAction(reference);
			if (inputAction != null)
			{
				if (performed != null)
				{
					inputAction.performed -= performed;
				}
				if (canceled != null)
				{
					inputAction.canceled -= canceled;
				}
			}
		}

		private static InputAction GetInputAction(InputActionReference actionReference)
		{
			if (!(actionReference != null))
			{
				return null;
			}
			return actionReference.action;
		}

		internal static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedControllerState state, in Quaternion inverseCameraParentRotation)
		{
			return XRSimulatorUtility.GetDeltaRotation(translateSpace, state.deviceRotation, inverseCameraParentRotation);
		}

		internal static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedHandState state, in Quaternion inverseCameraParentRotation)
		{
			return XRSimulatorUtility.GetDeltaRotation(translateSpace, state.rotation, inverseCameraParentRotation);
		}

		internal static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedHMDState state, in Quaternion inverseCameraParentRotation)
		{
			return XRSimulatorUtility.GetDeltaRotation(translateSpace, state.centerEyeRotation, inverseCameraParentRotation);
		}

		internal static void GetAxes(Space translateSpace, Transform cameraTransform, out Vector3 right, out Vector3 up, out Vector3 forward)
		{
			if (cameraTransform == null)
			{
				throw new ArgumentNullException("cameraTransform");
			}
			switch (translateSpace)
			{
			case Space.Local:
			{
				Transform parent = cameraTransform.parent;
				if (parent != null)
				{
					right = parent.TransformDirection(Vector3.right);
					up = parent.TransformDirection(Vector3.up);
					forward = parent.TransformDirection(Vector3.forward);
					return;
				}
				right = Vector3.right;
				up = Vector3.up;
				forward = Vector3.forward;
				return;
			}
			case Space.Parent:
				right = Vector3.right;
				up = Vector3.up;
				forward = Vector3.forward;
				return;
			case Space.Screen:
				right = cameraTransform.TransformDirection(Vector3.right);
				up = cameraTransform.TransformDirection(Vector3.up);
				forward = cameraTransform.TransformDirection(Vector3.forward);
				return;
			default:
				right = Vector3.right;
				up = Vector3.up;
				forward = Vector3.forward;
				return;
			}
		}

		internal static Quaternion GetDeltaRotation(Space translateSpace, Quaternion rotation, in Quaternion inverseCameraParentRotation)
		{
			switch (translateSpace)
			{
			case Space.Local:
				return rotation * inverseCameraParentRotation;
			case Space.Parent:
				return Quaternion.identity;
			case Space.Screen:
				return inverseCameraParentRotation;
			default:
				return Quaternion.identity;
			}
		}

		internal static bool FindCameraTransform([TupleElementNames(new string[]
		{
			"transform",
			"camera"
		})] ref ValueTuple<Transform, Camera> cachedCamera, ref Transform cameraTransform)
		{
			if (cachedCamera.Item1 != cameraTransform)
			{
				cachedCamera = new ValueTuple<Transform, Camera>(cameraTransform, (cameraTransform != null) ? cameraTransform.GetComponent<Camera>() : null);
			}
			if (cachedCamera.Item1 == null || (cachedCamera.Item2 != null && !cachedCamera.Item2.isActiveAndEnabled))
			{
				Camera main = Camera.main;
				if (main == null)
				{
					return false;
				}
				cameraTransform = main.transform;
				cachedCamera = new ValueTuple<Transform, Camera>(cameraTransform, cameraTransform.GetComponent<Camera>());
			}
			return true;
		}

		internal unsafe static bool TryExecuteCommand(InputDeviceCommand* commandPtr, out long result)
		{
			FourCC type = commandPtr->type;
			if (type == RequestSyncCommand.Type)
			{
				result = 1L;
				return true;
			}
			if (type == QueryCanRunInBackground.Type)
			{
				((QueryCanRunInBackground*)commandPtr)->canRunInBackground = true;
				result = 1L;
				return true;
			}
			result = 0L;
			return false;
		}

		internal static Vector3 GetTranslationInDeviceSpace(float xTranslateInput, float yTranslateInput, float zTranslateInput, Transform cameraTransform, Quaternion cameraParentRotation, Quaternion inverseCameraParentRotation)
		{
			Vector3 translationInWorldSpace = XRSimulatorUtility.GetTranslationInWorldSpace(xTranslateInput, yTranslateInput, zTranslateInput, cameraTransform, cameraParentRotation);
			return inverseCameraParentRotation * translationInWorldSpace;
		}

		internal static Vector3 GetTranslationInWorldSpace(float xTranslateInput, float yTranslateInput, float zTranslateInput, Transform cameraTransform, Quaternion cameraParentRotation)
		{
			Vector3 point = new Vector3(xTranslateInput, yTranslateInput, zTranslateInput);
			Vector3 vector = cameraTransform.forward;
			Vector3 vector2 = cameraParentRotation * Vector3.up;
			if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(vector, vector2)), 1f))
			{
				vector = -cameraTransform.up;
			}
			return Quaternion.LookRotation(Vector3.ProjectOnPlane(vector, vector2), vector2) * point;
		}

		internal static readonly float cameraMaxXAngle = 80f;

		internal static readonly Vector3 leftDeviceDefaultInitialPosition = new Vector3(-0.1f, -0.05f, 0.3f);

		internal static readonly Vector3 rightDeviceDefaultInitialPosition = new Vector3(0.1f, -0.05f, 0.3f);
	}
}
