using System;
using UnityEngine;

public class XRaySkeleton : SyncToPlayerColor
{
	protected override void Awake()
	{
		base.Awake();
		this.target = this.renderer.material;
		Material[] materialsToChangeTo = this.rig.materialsToChangeTo;
		this.tagMaterials = new Material[materialsToChangeTo.Length];
		this.tagMaterials[0] = new Material(this.target);
		for (int i = 1; i < materialsToChangeTo.Length; i++)
		{
			Material material = new Material(materialsToChangeTo[i]);
			this.tagMaterials[i] = material;
		}
	}

	public void SetMaterialIndex(int index)
	{
		this.renderer.sharedMaterial = this.tagMaterials[index];
		this._lastMatIndex = index;
	}

	private void Setup()
	{
		this.colorPropertiesToSync = new ShaderHashId[]
		{
			XRaySkeleton._BaseColor,
			XRaySkeleton._EmissionColor
		};
	}

	public override void UpdateColor(Color color)
	{
		if (this._lastMatIndex != 0)
		{
			return;
		}
		Material material = this.tagMaterials[0];
		float h;
		float s;
		float value;
		Color.RGBToHSV(color, out h, out s, out value);
		Color value2 = Color.HSVToRGB(h, s, Mathf.Clamp(value, this.baseValueMinMax.x, this.baseValueMinMax.y));
		material.SetColor(XRaySkeleton._BaseColor, value2);
		float h2;
		float num;
		float num2;
		Color.RGBToHSV(color, out h2, out num, out num2);
		Color color2 = Color.HSVToRGB(h2, 0.82f, 0.9f, true);
		color2 = new Color(color2.r * 1.4f, color2.g * 1.4f, color2.b * 1.4f);
		material.SetColor(XRaySkeleton._EmissionColor, ColorUtils.ComposeHDR(new Color32(36, 191, 136, byte.MaxValue), 2f));
		this.renderer.sharedMaterial = material;
	}

	public SkinnedMeshRenderer renderer;

	public Vector2 baseValueMinMax = new Vector2(0.69f, 1f);

	public Material[] tagMaterials = new Material[0];

	private int _lastMatIndex;

	private static readonly ShaderHashId _BaseColor = "_BaseColor";

	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";
}
