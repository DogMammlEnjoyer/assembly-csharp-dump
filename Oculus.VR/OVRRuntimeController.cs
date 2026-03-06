using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-runtime-controller/")]
public class OVRRuntimeController : MonoBehaviour
{
	private void Start()
	{
		if (this.m_controller == OVRInput.Controller.LTouch)
		{
			this.m_controllerModelPath = OVRRuntimeController.leftControllerModelPath;
		}
		else if (this.m_controller == OVRInput.Controller.RTouch)
		{
			this.m_controllerModelPath = OVRRuntimeController.rightControllerModelPath;
		}
		this.m_modelSupported = this.IsModelSupported(this.m_controllerModelPath);
		if (this.m_modelSupported)
		{
			base.StartCoroutine(this.UpdateControllerModel());
		}
		OVRManager.InputFocusAcquired += this.InputFocusAquired;
		OVRManager.InputFocusLost += this.InputFocusLost;
	}

	private void Update()
	{
		bool flag = OVRInput.IsControllerConnected(this.m_controller);
		if (this.m_hasInputFocus != this.m_hasInputFocusPrev || flag != this.m_controllerConnectedPrev)
		{
			if (this.m_controllerObject != null)
			{
				this.m_controllerObject.SetActive(flag && this.m_hasInputFocus);
			}
			this.m_hasInputFocusPrev = this.m_hasInputFocus;
			this.m_controllerConnectedPrev = flag;
		}
		if (flag)
		{
			this.UpdateControllerAnimation();
		}
	}

	private bool IsModelSupported(string modelPath)
	{
		string[] renderModelPaths = OVRPlugin.GetRenderModelPaths();
		if (renderModelPaths.Length == 0)
		{
			Debug.LogError("Failed to enumerate model paths from the runtime. Check that the render model feature is enabled in OVRManager.");
			return false;
		}
		for (int i = 0; i < renderModelPaths.Length; i++)
		{
			if (renderModelPaths[i].Equals(modelPath))
			{
				return true;
			}
		}
		Debug.LogError("Render model path " + modelPath + " not supported by this device.");
		return false;
	}

	private bool LoadControllerModel(string modelPath)
	{
		OVRPlugin.RenderModelProperties renderModelProperties = default(OVRPlugin.RenderModelProperties);
		if (OVRPlugin.GetRenderModelProperties(modelPath, ref renderModelProperties))
		{
			if (renderModelProperties.ModelKey != 0UL)
			{
				byte[] array = OVRPlugin.LoadRenderModel(renderModelProperties.ModelKey);
				if (array != null)
				{
					OVRGLTFLoader ovrgltfloader = new OVRGLTFLoader(array);
					ovrgltfloader.SetModelShader(this.m_controllerModelShader);
					OVRGLTFScene ovrgltfscene = ovrgltfloader.LoadGLB(this.m_supportAnimation, true);
					this.m_controllerObject = ovrgltfscene.root;
					this.m_animationNodes = ovrgltfscene.animationNodes;
					if (this.m_controllerObject != null)
					{
						this.m_controllerObject.transform.SetParent(base.transform, false);
						this.m_controllerObject.transform.parent.localPosition = new Vector3(0f, -0.03f, -0.04f);
						this.m_controllerObject.transform.parent.localRotation = Quaternion.AngleAxis(-60f, new Vector3(1f, 0f, 0f));
						return true;
					}
				}
			}
			Debug.LogError("Retrived a null model key of " + modelPath);
		}
		Debug.LogError("Failed to load controller model of " + modelPath);
		return false;
	}

	private IEnumerator UpdateControllerModel()
	{
		for (;;)
		{
			bool flag = OVRInput.IsControllerConnected(this.m_controller);
			if (this.m_controllerObject == null && flag)
			{
				this.LoadControllerModel(this.m_controllerModelPath);
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	private void UpdateControllerAnimation()
	{
		if (this.m_animationNodes == null)
		{
			return;
		}
		if (this.m_animationNodes.ContainsKey(OVRGLTFInputNode.Button_A_X))
		{
			this.m_animationNodes[OVRGLTFInputNode.Button_A_X].UpdatePose(OVRInput.Get((this.m_controller == OVRInput.Controller.LTouch) ? OVRInput.RawButton.X : OVRInput.RawButton.A, OVRInput.Controller.Active));
		}
		if (this.m_animationNodes.ContainsKey(OVRGLTFInputNode.Button_B_Y))
		{
			this.m_animationNodes[OVRGLTFInputNode.Button_B_Y].UpdatePose(OVRInput.Get((this.m_controller == OVRInput.Controller.LTouch) ? OVRInput.RawButton.Y : OVRInput.RawButton.B, OVRInput.Controller.Active));
		}
		if (this.m_animationNodes.ContainsKey(OVRGLTFInputNode.Button_Oculus_Menu))
		{
			this.m_animationNodes[OVRGLTFInputNode.Button_Oculus_Menu].UpdatePose(OVRInput.Get(OVRInput.RawButton.Start, OVRInput.Controller.Active));
		}
		if (this.m_animationNodes.ContainsKey(OVRGLTFInputNode.Trigger_Grip))
		{
			this.m_animationNodes[OVRGLTFInputNode.Trigger_Grip].UpdatePose(OVRInput.Get((this.m_controller == OVRInput.Controller.LTouch) ? OVRInput.RawAxis1D.LHandTrigger : OVRInput.RawAxis1D.RHandTrigger, OVRInput.Controller.Active), true);
		}
		if (this.m_animationNodes.ContainsKey(OVRGLTFInputNode.Trigger_Front))
		{
			this.m_animationNodes[OVRGLTFInputNode.Trigger_Front].UpdatePose(OVRInput.Get((this.m_controller == OVRInput.Controller.LTouch) ? OVRInput.RawAxis1D.LIndexTrigger : OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.Active), true);
		}
		if (this.m_animationNodes.ContainsKey(OVRGLTFInputNode.ThumbStick))
		{
			this.m_animationNodes[OVRGLTFInputNode.ThumbStick].UpdatePose(OVRInput.Get((this.m_controller == OVRInput.Controller.LTouch) ? OVRInput.RawAxis2D.LThumbstick : OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active));
		}
	}

	public void InputFocusAquired()
	{
		this.m_hasInputFocus = true;
	}

	public void InputFocusLost()
	{
		this.m_hasInputFocus = false;
	}

	public OVRInput.Controller m_controller;

	public Shader m_controllerModelShader;

	public bool m_supportAnimation = true;

	private GameObject m_controllerObject;

	private static string leftControllerModelPath = "/model_fb/controller/left";

	private static string rightControllerModelPath = "/model_fb/controller/right";

	private string m_controllerModelPath;

	private bool m_modelSupported;

	private bool m_hasInputFocus = true;

	private bool m_hasInputFocusPrev;

	private bool m_controllerConnectedPrev;

	private Dictionary<OVRGLTFInputNode, OVRGLTFAnimatinonNode> m_animationNodes;
}
