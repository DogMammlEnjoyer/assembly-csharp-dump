using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-sf-distancegrab/")]
public class OVRPlayerController : MonoBehaviour
{
	public event Action<Transform> TransformUpdated;

	public event Action CameraUpdated;

	public event Action PreCharacterMove;

	public float InitialYRotation { get; private set; }

	private void Start()
	{
		Vector3 localPosition = this.CameraRig.transform.localPosition;
		localPosition.z = OVRManager.profile.eyeDepth;
		this.CameraRig.transform.localPosition = localPosition;
		this.moveForwardAction = new InputAction(null, InputActionType.Value, "<Keyboard>/w", null, null, null);
		this.moveForwardAction.AddBinding("<Keyboard>/upArrow", null, null, null);
		this.moveLeftAction = new InputAction(null, InputActionType.Value, "<Keyboard>/a", null, null, null);
		this.moveLeftAction.AddBinding("<Keyboard>/leftArrow", null, null, null);
		this.moveRightAction = new InputAction(null, InputActionType.Value, "<Keyboard>/d", null, null, null);
		this.moveRightAction.AddBinding("<Keyboard>/rightArrow", null, null, null);
		this.moveBackAction = new InputAction(null, InputActionType.Value, "<Keyboard>/s", null, null, null);
		this.moveBackAction.AddBinding("<Keyboard>/downArrow", null, null, null);
		this.runAction = new InputAction(null, InputActionType.Value, "<Keyboard>/leftShift", null, null, null);
		this.runAction.AddBinding("<Keyboard>/rightShift", null, null, null);
		this.moveForwardAction.Enable();
		this.moveLeftAction.Enable();
		this.moveRightAction.Enable();
		this.moveBackAction.Enable();
		this.runAction.Enable();
	}

	private void Awake()
	{
		this.Controller = base.gameObject.GetComponent<CharacterController>();
		if (this.Controller == null)
		{
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");
		}
		OVRCameraRig[] componentsInChildren = base.gameObject.GetComponentsInChildren<OVRCameraRig>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
		}
		else if (componentsInChildren.Length > 1)
		{
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
		}
		else
		{
			this.CameraRig = componentsInChildren[0];
		}
		this.InitialYRotation = base.transform.rotation.eulerAngles.y;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		if (this.playerControllerEnabled)
		{
			OVRManager.display.RecenteredPose -= this.ResetOrientation;
			if (this.CameraRig != null)
			{
				this.CameraRig.UpdatedAnchors -= this.UpdateTransform;
			}
			this.playerControllerEnabled = false;
		}
		this.moveForwardAction.Disable();
		this.moveLeftAction.Disable();
		this.moveRightAction.Disable();
		this.moveBackAction.Disable();
		this.runAction.Disable();
	}

	private void Update()
	{
		if (this.playerControllerEnabled)
		{
			return;
		}
		if (OVRManager.OVRManagerinitialized)
		{
			OVRManager.display.RecenteredPose += this.ResetOrientation;
			if (this.CameraRig != null)
			{
				this.CameraRig.UpdatedAnchors += this.UpdateTransform;
			}
			this.playerControllerEnabled = true;
			return;
		}
	}

	protected virtual void UpdateController()
	{
		if (this.useProfileData)
		{
			if (this.InitialPose == null)
			{
				this.InitialPose = new OVRPose?(new OVRPose
				{
					position = this.CameraRig.transform.localPosition,
					orientation = this.CameraRig.transform.localRotation
				});
			}
			Vector3 localPosition = this.CameraRig.transform.localPosition;
			if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
			{
				localPosition.y = OVRManager.profile.eyeHeight - 0.5f * this.Controller.height + this.Controller.center.y;
			}
			else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
			{
				localPosition.y = -(0.5f * this.Controller.height) + this.Controller.center.y;
			}
			this.CameraRig.transform.localPosition = localPosition;
		}
		else if (this.InitialPose != null)
		{
			this.CameraRig.transform.localPosition = this.InitialPose.Value.position;
			this.CameraRig.transform.localRotation = this.InitialPose.Value.orientation;
			this.InitialPose = null;
		}
		this.CameraHeight = this.CameraRig.centerEyeAnchor.localPosition.y;
		if (this.CameraUpdated != null)
		{
			this.CameraUpdated();
		}
		this.UpdateMovement();
		Vector3 vector = Vector3.zero;
		float num = 1f + this.Damping * this.SimulationRate * Time.deltaTime;
		this.MoveThrottle.x = this.MoveThrottle.x / num;
		this.MoveThrottle.y = ((this.MoveThrottle.y > 0f) ? (this.MoveThrottle.y / num) : this.MoveThrottle.y);
		this.MoveThrottle.z = this.MoveThrottle.z / num;
		vector += this.MoveThrottle * this.SimulationRate * Time.deltaTime;
		if (this.Controller.isGrounded && this.FallSpeed <= 0f)
		{
			this.FallSpeed = Physics.gravity.y * (this.GravityModifier * 0.002f);
		}
		else
		{
			this.FallSpeed += Physics.gravity.y * (this.GravityModifier * 0.002f) * this.SimulationRate * Time.deltaTime;
		}
		vector.y += this.FallSpeed * this.SimulationRate * Time.deltaTime;
		if (this.Controller.isGrounded && this.MoveThrottle.y <= base.transform.lossyScale.y * 0.001f)
		{
			float d = Mathf.Max(this.Controller.stepOffset, new Vector3(vector.x, 0f, vector.z).magnitude);
			vector -= d * Vector3.up;
		}
		if (this.PreCharacterMove != null)
		{
			this.PreCharacterMove();
			this.Teleported = false;
		}
		Vector3 vector2 = Vector3.Scale(this.Controller.transform.localPosition + vector, new Vector3(1f, 0f, 1f));
		this.Controller.Move(vector);
		Vector3 vector3 = Vector3.Scale(this.Controller.transform.localPosition, new Vector3(1f, 0f, 1f));
		if (vector2 != vector3)
		{
			this.MoveThrottle += (vector3 - vector2) / (this.SimulationRate * Time.deltaTime);
		}
	}

	public virtual void UpdateMovement()
	{
		if (this.HaltUpdateMovement)
		{
			return;
		}
		if (this.EnableLinearMovement)
		{
			bool flag = this.moveForwardAction.phase == InputActionPhase.Started;
			bool flag2 = this.moveLeftAction.phase == InputActionPhase.Started;
			bool flag3 = this.moveRightAction.phase == InputActionPhase.Started;
			bool flag4 = this.moveBackAction.phase == InputActionPhase.Started;
			bool flag5 = false;
			if (OVRInput.Get(OVRInput.Button.DpadUp, OVRInput.Controller.Active))
			{
				flag = true;
				flag5 = true;
			}
			if (OVRInput.Get(OVRInput.Button.DpadDown, OVRInput.Controller.Active))
			{
				flag4 = true;
				flag5 = true;
			}
			this.MoveScale = 1f;
			if ((flag && flag2) || (flag && flag3) || (flag4 && flag2) || (flag4 && flag3))
			{
				this.MoveScale = 0.70710677f;
			}
			if (!this.Controller.isGrounded)
			{
				this.MoveScale = 0f;
			}
			this.MoveScale *= this.SimulationRate * Time.deltaTime;
			float num = this.Acceleration * 0.1f * this.MoveScale * this.MoveScaleMultiplier;
			if (flag5 || this.runAction.phase == InputActionPhase.Started)
			{
				num *= 2f;
			}
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z = (eulerAngles.x = 0f);
			Quaternion rotation = Quaternion.Euler(eulerAngles);
			if (flag)
			{
				this.MoveThrottle += rotation * (base.transform.lossyScale.z * num * Vector3.forward);
			}
			if (flag4)
			{
				this.MoveThrottle += rotation * (base.transform.lossyScale.z * num * this.BackAndSideDampen * Vector3.back);
			}
			if (flag2)
			{
				this.MoveThrottle += rotation * (base.transform.lossyScale.x * num * this.BackAndSideDampen * Vector3.left);
			}
			if (flag3)
			{
				this.MoveThrottle += rotation * (base.transform.lossyScale.x * num * this.BackAndSideDampen * Vector3.right);
			}
			num = this.Acceleration * 0.1f * this.MoveScale * this.MoveScaleMultiplier;
			num *= 1f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Active);
			Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active);
			if (this.FixedSpeedSteps > 0)
			{
				vector.y = Mathf.Round(vector.y * (float)this.FixedSpeedSteps) / (float)this.FixedSpeedSteps;
				vector.x = Mathf.Round(vector.x * (float)this.FixedSpeedSteps) / (float)this.FixedSpeedSteps;
			}
			if (vector.y > 0f)
			{
				this.MoveThrottle += rotation * (vector.y * base.transform.lossyScale.z * num * Vector3.forward);
			}
			if (vector.y < 0f)
			{
				this.MoveThrottle += rotation * (Mathf.Abs(vector.y) * base.transform.lossyScale.z * num * this.BackAndSideDampen * Vector3.back);
			}
			if (vector.x < 0f)
			{
				this.MoveThrottle += rotation * (Mathf.Abs(vector.x) * base.transform.lossyScale.x * num * this.BackAndSideDampen * Vector3.left);
			}
			if (vector.x > 0f)
			{
				this.MoveThrottle += rotation * (vector.x * base.transform.lossyScale.x * num * this.BackAndSideDampen * Vector3.right);
			}
		}
		if (this.EnableRotation)
		{
			Vector3 vector2 = this.RotateAroundGuardianCenter ? base.transform.rotation.eulerAngles : Vector3.zero;
			float num2 = this.SimulationRate * Time.deltaTime * this.RotationAmount * this.RotationScaleMultiplier;
			bool flag6 = OVRInput.Get(OVRInput.Button.PrimaryShoulder, OVRInput.Controller.Active);
			if (flag6 && !this.prevHatLeft)
			{
				vector2.y -= this.RotationRatchet;
			}
			this.prevHatLeft = flag6;
			bool flag7 = OVRInput.Get(OVRInput.Button.SecondaryShoulder, OVRInput.Controller.Active);
			if (flag7 && !this.prevHatRight)
			{
				vector2.y += this.RotationRatchet;
			}
			this.prevHatRight = flag7;
			vector2.y += this.buttonRotation;
			this.buttonRotation = 0f;
			if (!this.SkipMouseRotation)
			{
				vector2.y += Input.GetAxis("Mouse X") * num2 * 3.25f;
			}
			if (this.SnapRotation)
			{
				if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.Active)))
				{
					if (this.ReadyToSnapTurn)
					{
						vector2.y -= this.RotationRatchet;
						this.ReadyToSnapTurn = false;
					}
				}
				else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.Active)))
				{
					if (this.ReadyToSnapTurn)
					{
						vector2.y += this.RotationRatchet;
						this.ReadyToSnapTurn = false;
					}
				}
				else
				{
					this.ReadyToSnapTurn = true;
				}
			}
			else
			{
				Vector2 vector3 = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Active);
				if (this.RotationEitherThumbstick)
				{
					Vector2 vector4 = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active);
					if (vector3.sqrMagnitude < vector4.sqrMagnitude)
					{
						vector3 = vector4;
					}
				}
				vector2.y += vector3.x * num2;
			}
			if (this.RotateAroundGuardianCenter)
			{
				base.transform.rotation = Quaternion.Euler(vector2);
				return;
			}
			base.transform.RotateAround(base.transform.position, Vector3.up, vector2.y);
		}
	}

	public void UpdateTransform(OVRCameraRig rig)
	{
		Transform trackingSpace = this.CameraRig.trackingSpace;
		Transform centerEyeAnchor = this.CameraRig.centerEyeAnchor;
		if (this.HmdRotatesY && !this.Teleported)
		{
			Vector3 position = trackingSpace.position;
			Quaternion rotation = trackingSpace.rotation;
			base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
			trackingSpace.position = position;
			trackingSpace.rotation = rotation;
		}
		this.UpdateController();
		if (this.TransformUpdated != null)
		{
			this.TransformUpdated(trackingSpace);
		}
	}

	public bool Jump()
	{
		if (!this.Controller.isGrounded)
		{
			return false;
		}
		this.MoveThrottle += new Vector3(0f, base.transform.lossyScale.y * this.JumpForce, 0f);
		return true;
	}

	public void Stop()
	{
		this.Controller.Move(Vector3.zero);
		this.MoveThrottle = Vector3.zero;
		this.FallSpeed = 0f;
	}

	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = this.MoveScaleMultiplier;
	}

	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		this.MoveScaleMultiplier = moveScaleMultiplier;
	}

	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = this.RotationScaleMultiplier;
	}

	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		this.RotationScaleMultiplier = rotationScaleMultiplier;
	}

	public void GetSkipMouseRotation(ref bool skipMouseRotation)
	{
		skipMouseRotation = this.SkipMouseRotation;
	}

	public void SetSkipMouseRotation(bool skipMouseRotation)
	{
		this.SkipMouseRotation = skipMouseRotation;
	}

	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = this.HaltUpdateMovement;
	}

	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		this.HaltUpdateMovement = haltUpdateMovement;
	}

	public void ResetOrientation()
	{
		if (this.HmdResetsY && !this.HmdRotatesY)
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.y = this.InitialYRotation;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
		}
	}

	public float Acceleration = 0.1f;

	public float Damping = 0.3f;

	public float BackAndSideDampen = 0.5f;

	public float JumpForce = 0.3f;

	public float RotationAmount = 1.5f;

	public float RotationRatchet = 45f;

	[Tooltip("The player will rotate in fixed steps if Snap Rotation is enabled.")]
	public bool SnapRotation = true;

	[Obsolete]
	[Tooltip("[Deprecated] When enabled, snap rotation will happen about the center of the guardian rather than the center of the player/camera viewpoint. This (legacy) option should be left off except for edge cases that require extreme behavioral backwards compatibility.")]
	public bool RotateAroundGuardianCenter;

	[Tooltip("Sets the number of discrete speeds that will be used in continuous motion. If 0, motion speed is not discretized.")]
	public int FixedSpeedSteps;

	public bool HmdResetsY = true;

	public bool HmdRotatesY = true;

	public float GravityModifier = 0.379f;

	public bool useProfileData = true;

	[NonSerialized]
	public float CameraHeight;

	[NonSerialized]
	public bool Teleported;

	public bool EnableLinearMovement = true;

	public bool EnableRotation = true;

	public bool RotationEitherThumbstick;

	protected CharacterController Controller;

	protected OVRCameraRig CameraRig;

	private float MoveScale = 1f;

	private Vector3 MoveThrottle = Vector3.zero;

	private float FallSpeed;

	private OVRPose? InitialPose;

	private float MoveScaleMultiplier = 1f;

	private float RotationScaleMultiplier = 1f;

	private bool SkipMouseRotation = true;

	private bool HaltUpdateMovement;

	private bool prevHatLeft;

	private bool prevHatRight;

	private float SimulationRate = 60f;

	private float buttonRotation;

	private bool ReadyToSnapTurn;

	private bool playerControllerEnabled;

	private InputAction moveForwardAction;

	private InputAction moveLeftAction;

	private InputAction moveRightAction;

	private InputAction moveBackAction;

	private InputAction runAction;
}
