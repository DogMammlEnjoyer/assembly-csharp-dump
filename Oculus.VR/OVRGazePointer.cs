using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-ovroverlay/#using-cylinder-overlays")]
public class OVRGazePointer : OVRCursor
{
	public bool hidden { get; private set; }

	public float currentScale { get; private set; }

	public static OVRGazePointer instance
	{
		get
		{
			if (OVRGazePointer._instance == null)
			{
				Debug.Log(string.Format("Instanciating GazePointer", 0));
				OVRGazePointer._instance = Object.Instantiate<OVRGazePointer>((OVRGazePointer)Resources.Load("Prefabs/GazePointerRing", typeof(OVRGazePointer)));
			}
			return OVRGazePointer._instance;
		}
	}

	public float visibilityStrength
	{
		get
		{
			float a;
			if (this.hideByDefault)
			{
				a = Mathf.Clamp01(1f - (Time.time - this.lastShowRequestTime) / this.showTimeoutPeriod);
			}
			else
			{
				a = 1f;
			}
			float b = (this.lastHideRequestTime + this.hideTimeoutPeriod > Time.time) ? (this.dimOnHideRequest ? 0.1f : 0f) : 1f;
			return Mathf.Min(a, b);
		}
	}

	public float SelectionProgress
	{
		get
		{
			if (!this.progressIndicator)
			{
				return 0f;
			}
			return this.progressIndicator.currentProgress;
		}
		set
		{
			if (this.progressIndicator)
			{
				this.progressIndicator.currentProgress = value;
			}
		}
	}

	public void Awake()
	{
		this.currentScale = 1f;
		if (OVRGazePointer._instance != null && OVRGazePointer._instance != this)
		{
			base.enabled = false;
			Object.DestroyImmediate(this);
			return;
		}
		OVRGazePointer._instance = this;
		this.gazeIcon = base.transform.Find("GazeIcon");
		this.progressIndicator = base.transform.GetComponent<OVRProgressIndicator>();
	}

	private void Update()
	{
		if (this.rayTransform == null && Camera.main != null)
		{
			this.rayTransform = Camera.main.transform;
		}
		base.transform.position = this.rayTransform.position + this.rayTransform.forward * this.depth;
		if (this.visibilityStrength == 0f && !this.hidden)
		{
			this.Hide();
			return;
		}
		if (this.visibilityStrength > 0f && this.hidden)
		{
			this.Show();
		}
	}

	public override void SetCursorStartDest(Vector3 _, Vector3 pos, Vector3 normal)
	{
		base.transform.position = pos;
		if (!this.matchNormalOnPhysicsColliders)
		{
			normal = this.rayTransform.forward;
		}
		Quaternion rotation = base.transform.rotation;
		rotation.SetLookRotation(normal, this.rayTransform.up);
		base.transform.rotation = rotation;
		this.depth = (this.rayTransform.position - pos).magnitude;
		this.currentScale = this.depth * this.depthScaleMultiplier;
		base.transform.localScale = new Vector3(this.currentScale, this.currentScale, this.currentScale);
		this.positionSetsThisFrame++;
		this.RequestShow();
	}

	public override void SetCursorRay(Transform ray)
	{
	}

	private void LateUpdate()
	{
		if (this.positionSetsThisFrame == 0)
		{
			Quaternion rotation = base.transform.rotation;
			rotation.SetLookRotation(this.rayTransform.forward, this.rayTransform.up);
			base.transform.rotation = rotation;
		}
		Quaternion rotation2 = this.gazeIcon.rotation;
		rotation2.SetLookRotation(base.transform.rotation * new Vector3(0f, 0f, 1f));
		this.gazeIcon.rotation = rotation2;
		this.positionSetsThisFrame = 0;
	}

	public void RequestHide()
	{
		if (!this.dimOnHideRequest)
		{
			this.Hide();
		}
		this.lastHideRequestTime = Time.time;
	}

	public void RequestShow()
	{
		this.Show();
		this.lastShowRequestTime = Time.time;
	}

	private void Hide()
	{
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		if (base.GetComponent<Renderer>())
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		this.hidden = true;
	}

	private void Show()
	{
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
		if (base.GetComponent<Renderer>())
		{
			base.GetComponent<Renderer>().enabled = true;
		}
		this.hidden = false;
	}

	private Transform gazeIcon;

	[Tooltip("Should the pointer be hidden when not over interactive objects.")]
	public bool hideByDefault = true;

	[Tooltip("Time after leaving interactive object before pointer fades.")]
	public float showTimeoutPeriod = 1f;

	[Tooltip("Time after mouse pointer becoming inactive before pointer unfades.")]
	public float hideTimeoutPeriod = 0.1f;

	[Tooltip("Keep a faint version of the pointer visible while using a mouse")]
	public bool dimOnHideRequest = true;

	[Tooltip("Angular scale of pointer")]
	public float depthScaleMultiplier = 0.03f;

	public bool matchNormalOnPhysicsColliders;

	public Transform rayTransform;

	private float depth;

	private float hideUntilTime;

	private int positionSetsThisFrame;

	private float lastShowRequestTime;

	private float lastHideRequestTime;

	private OVRProgressIndicator progressIndicator;

	private static OVRGazePointer _instance;
}
