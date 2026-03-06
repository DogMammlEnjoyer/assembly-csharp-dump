using System;
using System.Reflection;

namespace UnityEngine.ProBuilder
{
	public static class BuiltinMaterials
	{
		private static void Init()
		{
			if (BuiltinMaterials.s_IsInitialized)
			{
				return;
			}
			BuiltinMaterials.s_IsInitialized = true;
			Shader shader = Shader.Find("Hidden/ProBuilder/LineBillboard");
			BuiltinMaterials.s_GeometryShadersSupported = (shader != null && shader.isSupported);
			BuiltinMaterials.s_SelectionPickerShader = Shader.Find("Hidden/ProBuilder/SelectionPicker");
			if ((BuiltinMaterials.s_FacePickerMaterial = Resources.Load<Material>(BuiltinMaterials.k_FacePickerMaterial)) == null)
			{
				Log.Error("FacePicker material not loaded... please re-install ProBuilder to fix this error.");
				BuiltinMaterials.s_FacePickerMaterial = new Material(Shader.Find(BuiltinMaterials.k_FacePickerShader));
			}
			if ((BuiltinMaterials.s_VertexPickerMaterial = Resources.Load<Material>(BuiltinMaterials.k_VertexPickerMaterial)) == null)
			{
				Log.Error("VertexPicker material not loaded... please re-install ProBuilder to fix this error.");
				BuiltinMaterials.s_VertexPickerMaterial = new Material(Shader.Find(BuiltinMaterials.k_VertexPickerShader));
			}
			if ((BuiltinMaterials.s_EdgePickerMaterial = Resources.Load<Material>(BuiltinMaterials.k_EdgePickerMaterial)) == null)
			{
				Log.Error("EdgePicker material not loaded... please re-install ProBuilder to fix this error.");
				BuiltinMaterials.s_EdgePickerMaterial = new Material(Shader.Find(BuiltinMaterials.k_EdgePickerShader));
			}
		}

		public static bool geometryShadersSupported
		{
			get
			{
				BuiltinMaterials.Init();
				return BuiltinMaterials.s_GeometryShadersSupported;
			}
		}

		public static Material defaultMaterial
		{
			get
			{
				BuiltinMaterials.Init();
				if (BuiltinMaterials.s_DefaultMaterial == null)
				{
					BuiltinMaterials.s_DefaultMaterial = BuiltinMaterials.GetDefaultMaterial();
				}
				return BuiltinMaterials.s_DefaultMaterial;
			}
		}

		internal static Shader selectionPickerShader
		{
			get
			{
				BuiltinMaterials.Init();
				return BuiltinMaterials.s_SelectionPickerShader;
			}
		}

		internal static Material facePickerMaterial
		{
			get
			{
				BuiltinMaterials.Init();
				return BuiltinMaterials.s_FacePickerMaterial;
			}
		}

		internal static Material vertexPickerMaterial
		{
			get
			{
				BuiltinMaterials.Init();
				return BuiltinMaterials.s_VertexPickerMaterial;
			}
		}

		internal static Material edgePickerMaterial
		{
			get
			{
				BuiltinMaterials.Init();
				return BuiltinMaterials.s_EdgePickerMaterial;
			}
		}

		internal static Material triggerMaterial
		{
			get
			{
				BuiltinMaterials.Init();
				return (Material)Resources.Load("Materials/Trigger", typeof(Material));
			}
		}

		internal static Material colliderMaterial
		{
			get
			{
				BuiltinMaterials.Init();
				return (Material)Resources.Load("Materials/Collider", typeof(Material));
			}
		}

		internal static Material GetLegacyDiffuse()
		{
			BuiltinMaterials.Init();
			if (BuiltinMaterials.s_UnityDefaultDiffuse == null)
			{
				MethodInfo method = typeof(Material).GetMethod("GetDefaultMaterial", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					BuiltinMaterials.s_UnityDefaultDiffuse = (method.Invoke(null, null) as Material);
				}
				if (BuiltinMaterials.s_UnityDefaultDiffuse == null)
				{
					GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					BuiltinMaterials.s_UnityDefaultDiffuse = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
					Object.DestroyImmediate(gameObject);
				}
			}
			return BuiltinMaterials.s_UnityDefaultDiffuse;
		}

		internal static Material GetDefaultMaterial()
		{
			Material material = (Material)Resources.Load("Materials/ProBuilderDefault", typeof(Material));
			material.shader = Shader.Find("ProBuilder6/Standard Vertex Color");
			if (material == null || !material.shader.isSupported)
			{
				material = BuiltinMaterials.GetLegacyDiffuse();
			}
			return material;
		}

		private static Material GetPreviewMaterial()
		{
			if (BuiltinMaterials.defaultMaterial == null)
			{
				return null;
			}
			Material material = new Material(BuiltinMaterials.defaultMaterial.shader);
			material.hideFlags = HideFlags.HideAndDontSave;
			if (material.HasProperty("_MainTex"))
			{
				material.mainTexture = (Texture2D)Resources.Load("Textures/GridBox_Default");
			}
			if (material.HasProperty("_Color"))
			{
				material.SetColor("_Color", BuiltinMaterials.previewColor);
			}
			return material;
		}

		internal static Material ShapePreviewMaterial
		{
			get
			{
				if (BuiltinMaterials.s_ShapePreviewMaterial == null)
				{
					BuiltinMaterials.s_ShapePreviewMaterial = BuiltinMaterials.GetPreviewMaterial();
				}
				return BuiltinMaterials.s_ShapePreviewMaterial;
			}
		}

		private static bool s_IsInitialized;

		public const string faceShader = "Hidden/ProBuilder/FaceHighlight";

		public const string lineShader = "Hidden/ProBuilder/LineBillboard";

		public const string lineShaderMetal = "Hidden/ProBuilder/LineBillboardMetal";

		public const string pointShader = "Hidden/ProBuilder/PointBillboard";

		public const string wireShader = "Hidden/ProBuilder/FaceHighlight";

		public const string dotShader = "Hidden/ProBuilder/VertexShader";

		internal static readonly Color previewColor = new Color(0.5f, 0.9f, 1f, 0.56f);

		private static Shader s_SelectionPickerShader;

		private static bool s_GeometryShadersSupported;

		private static Material s_DefaultMaterial;

		private static Material s_FacePickerMaterial;

		private static Material s_VertexPickerMaterial;

		private static Material s_EdgePickerMaterial;

		private static Material s_UnityDefaultDiffuse;

		private static Material s_ShapePreviewMaterial;

		private static string k_EdgePickerMaterial = "Materials/EdgePicker";

		private static string k_FacePickerMaterial = "Materials/FacePicker";

		private static string k_VertexPickerMaterial = "Materials/VertexPicker";

		private static string k_EdgePickerShader = "Hidden/ProBuilder/EdgePicker";

		private static string k_FacePickerShader = "Hidden/ProBuilder/FacePicker";

		private static string k_VertexPickerShader = "Hidden/ProBuilder/VertexPicker";
	}
}
