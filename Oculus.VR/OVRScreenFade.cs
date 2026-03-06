using System;
using System.Collections;
using UnityEngine;

[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_screen_fade")]
public class OVRScreenFade : MonoBehaviour
{
	public static OVRScreenFade instance { get; private set; }

	public float currentAlpha
	{
		get
		{
			return Mathf.Max(new float[]
			{
				this.explicitFadeAlpha,
				this.animatedFadeAlpha,
				this.uiFadeAlpha
			});
		}
	}

	private void Start()
	{
		if (base.gameObject.name.StartsWith("OculusMRC_"))
		{
			Object.Destroy(this);
			return;
		}
		this.fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
		this.fadeMesh = base.gameObject.AddComponent<MeshFilter>();
		this.fadeRenderer = base.gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		this.fadeMesh.mesh = mesh;
		Vector3[] array = new Vector3[4];
		float num = 2f;
		float num2 = 2f;
		float z = 1f;
		array[0] = new Vector3(-num, -num2, z);
		array[1] = new Vector3(num, -num2, z);
		array[2] = new Vector3(-num, num2, z);
		array[3] = new Vector3(num, num2, z);
		mesh.vertices = array;
		mesh.triangles = new int[]
		{
			0,
			2,
			1,
			2,
			3,
			1
		};
		mesh.normals = new Vector3[]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		this.explicitFadeAlpha = 0f;
		this.animatedFadeAlpha = 0f;
		this.uiFadeAlpha = 0f;
		if (this.fadeOnStart)
		{
			this.FadeIn();
		}
		OVRScreenFade.instance = this;
	}

	public void FadeIn()
	{
		base.StartCoroutine(this.Fade(1f, 0f));
	}

	public void FadeOut()
	{
		base.StartCoroutine(this.Fade(0f, 1f));
	}

	private void OnLevelFinishedLoading(int level)
	{
		this.FadeIn();
	}

	private void OnEnable()
	{
		if (!this.fadeOnStart)
		{
			this.explicitFadeAlpha = 0f;
			this.animatedFadeAlpha = 0f;
			this.uiFadeAlpha = 0f;
		}
	}

	private void OnDestroy()
	{
		if (OVRScreenFade.instance == this)
		{
			OVRScreenFade.instance = null;
		}
		if (this.fadeRenderer != null)
		{
			Object.Destroy(this.fadeRenderer);
		}
		if (this.fadeMaterial != null)
		{
			Object.Destroy(this.fadeMaterial);
		}
		if (this.fadeMesh != null)
		{
			Object.Destroy(this.fadeMesh);
		}
	}

	public void SetUIFade(float level)
	{
		this.uiFadeAlpha = Mathf.Clamp01(level);
		this.SetMaterialAlpha();
	}

	public void SetExplicitFade(float level)
	{
		this.explicitFadeAlpha = level;
		this.SetMaterialAlpha();
	}

	private IEnumerator Fade(float startAlpha, float endAlpha)
	{
		float elapsedTime = 0f;
		while (elapsedTime < this.fadeTime)
		{
			elapsedTime += Time.deltaTime;
			this.animatedFadeAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / this.fadeTime));
			this.SetMaterialAlpha();
			yield return new WaitForEndOfFrame();
		}
		this.animatedFadeAlpha = endAlpha;
		this.SetMaterialAlpha();
		yield break;
	}

	private void SetMaterialAlpha()
	{
		Color color = this.fadeColor;
		color.a = this.currentAlpha;
		this.isFading = (color.a > 0f);
		if (this.fadeMaterial != null)
		{
			this.fadeMaterial.color = color;
			this.fadeMaterial.renderQueue = this.renderQueue;
			this.fadeRenderer.material = this.fadeMaterial;
			this.fadeRenderer.enabled = this.isFading;
		}
	}

	[Tooltip("Fade duration")]
	public float fadeTime = 2f;

	[Tooltip("Screen color at maximum fade")]
	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1f);

	public bool fadeOnStart = true;

	public int renderQueue = 5000;

	private float explicitFadeAlpha;

	private float animatedFadeAlpha;

	private float uiFadeAlpha;

	private MeshRenderer fadeRenderer;

	private MeshFilter fadeMesh;

	private Material fadeMaterial;

	private bool isFading;
}
