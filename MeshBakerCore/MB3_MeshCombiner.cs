using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public abstract class MB3_MeshCombiner : MB_IMeshBakerSettings, IDisposable
	{
		public static bool EVAL_VERSION
		{
			get
			{
				return false;
			}
		}

		public virtual MB3_MeshCombiner.MeshCombiningStatus bakeStatus
		{
			get
			{
				return this._bakeStatus;
			}
		}

		public virtual MB2_ValidationLevel validationLevel
		{
			get
			{
				return this._validationLevel;
			}
			set
			{
				this._validationLevel = value;
			}
		}

		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public virtual MB2_TextureBakeResults textureBakeResults
		{
			get
			{
				return this._textureBakeResults;
			}
			set
			{
				this._textureBakeResults = value;
			}
		}

		public virtual GameObject resultSceneObject
		{
			get
			{
				return this._resultSceneObject;
			}
			set
			{
				this._resultSceneObject = value;
			}
		}

		public virtual Renderer targetRenderer
		{
			get
			{
				return this._targetRenderer;
			}
			set
			{
				if (this._targetRenderer != null && this._targetRenderer != value)
				{
					Debug.LogWarning("Previous targetRenderer was not null. Combined mesh may be shared by more than one Renderer");
				}
				this._targetRenderer = value;
				if (value != null && MB_Utility.IsSceneInstance(value.gameObject) && value.transform.parent != null)
				{
					this._resultSceneObject = value.transform.parent.gameObject;
				}
			}
		}

		public virtual MB2_LogLevel LOG_LEVEL
		{
			get
			{
				return this._LOG_LEVEL;
			}
			set
			{
				this._LOG_LEVEL = value;
			}
		}

		public MB_IMeshBakerSettings settings
		{
			get
			{
				if (this._settingsHolder != null)
				{
					return this.settingsHolder.GetMeshBakerSettings();
				}
				return this;
			}
		}

		public virtual MB_IMeshBakerSettingsHolder settingsHolder
		{
			get
			{
				if (this._settingsHolder != null)
				{
					if (this._settingsHolder is MB_IMeshBakerSettingsHolder)
					{
						return (MB_IMeshBakerSettingsHolder)this._settingsHolder;
					}
					this._settingsHolder = null;
				}
				return null;
			}
			set
			{
				if (value is Object)
				{
					this._settingsHolder = (Object)value;
					return;
				}
				Debug.LogError("The settings holder must be a UnityEngine.Object");
			}
		}

		public virtual MB2_OutputOptions outputOption
		{
			get
			{
				return this._outputOption;
			}
			set
			{
				this._outputOption = value;
			}
		}

		public virtual MB_RenderType renderType
		{
			get
			{
				return this._renderType;
			}
			set
			{
				this._renderType = value;
			}
		}

		public virtual MB2_LightmapOptions lightmapOption
		{
			get
			{
				return this._lightmapOption;
			}
			set
			{
				this._lightmapOption = value;
			}
		}

		public virtual bool doNorm
		{
			get
			{
				return this._doNorm;
			}
			set
			{
				this._doNorm = value;
			}
		}

		public virtual bool doTan
		{
			get
			{
				return this._doTan;
			}
			set
			{
				this._doTan = value;
			}
		}

		public virtual bool doCol
		{
			get
			{
				return this._doCol;
			}
			set
			{
				this._doCol = value;
			}
		}

		public virtual bool doUV
		{
			get
			{
				return this._doUV;
			}
			set
			{
				this._doUV = value;
			}
		}

		public virtual bool doUV1
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual bool doUV2()
		{
			return this.settings.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged || this.settings.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || this.settings.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects;
		}

		public virtual bool doUV3
		{
			get
			{
				return this._doUV3;
			}
			set
			{
				this._doUV3 = value;
			}
		}

		public virtual bool doUV4
		{
			get
			{
				return this._doUV4;
			}
			set
			{
				this._doUV4 = value;
			}
		}

		public virtual bool doUV5
		{
			get
			{
				return this._doUV5;
			}
			set
			{
				this._doUV5 = value;
			}
		}

		public virtual bool doUV6
		{
			get
			{
				return this._doUV6;
			}
			set
			{
				this._doUV6 = value;
			}
		}

		public virtual bool doUV7
		{
			get
			{
				return this._doUV7;
			}
			set
			{
				this._doUV7 = value;
			}
		}

		public virtual bool doUV8
		{
			get
			{
				return this._doUV8;
			}
			set
			{
				this._doUV8 = value;
			}
		}

		public virtual bool doBlendShapes
		{
			get
			{
				return this._doBlendShapes;
			}
			set
			{
				this._doBlendShapes = value;
			}
		}

		public virtual MB_MeshPivotLocation pivotLocationType
		{
			get
			{
				return this._pivotLocationType;
			}
			set
			{
				this._pivotLocationType = value;
			}
		}

		public virtual Vector3 pivotLocation
		{
			get
			{
				return this._pivotLocation;
			}
			set
			{
				this._pivotLocation = value;
			}
		}

		public virtual bool clearBuffersAfterBake
		{
			get
			{
				return this._clearBuffersAfterBake;
			}
			set
			{
				this._clearBuffersAfterBake = value;
			}
		}

		public bool optimizeAfterBake
		{
			get
			{
				return this._optimizeAfterBake;
			}
			set
			{
				this._optimizeAfterBake = value;
			}
		}

		public float uv2UnwrappingParamsHardAngle
		{
			get
			{
				return this._uv2UnwrappingParamsHardAngle;
			}
			set
			{
				this._uv2UnwrappingParamsHardAngle = value;
			}
		}

		public float uv2UnwrappingParamsPackMargin
		{
			get
			{
				return this._uv2UnwrappingParamsPackMargin;
			}
			set
			{
				this._uv2UnwrappingParamsPackMargin = value;
			}
		}

		public bool smrNoExtraBonesWhenCombiningMeshRenderers
		{
			get
			{
				return this._smrNoExtraBonesWhenCombiningMeshRenderers;
			}
			set
			{
				this._smrNoExtraBonesWhenCombiningMeshRenderers = value;
			}
		}

		public bool smrMergeBlendShapesWithSameNames
		{
			get
			{
				return this._smrMergeBlendShapesWithSameNames;
			}
			set
			{
				this._smrMergeBlendShapesWithSameNames = value;
			}
		}

		public IAssignToMeshCustomizer assignToMeshCustomizer
		{
			get
			{
				if (this._assignToMeshCustomizer is IAssignToMeshCustomizer)
				{
					return (IAssignToMeshCustomizer)this._assignToMeshCustomizer;
				}
				this._assignToMeshCustomizer = null;
				return null;
			}
			set
			{
				this._assignToMeshCustomizer = (Object)value;
			}
		}

		public MB_MeshCombineAPIType meshAPI
		{
			get
			{
				return this._meshAPItoUse;
			}
			set
			{
				this._meshAPItoUse = value;
			}
		}

		public virtual void DisposeRuntimeCreated()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public bool IsDisposed()
		{
			return this._disposed;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			this._DisposeRuntimeCreated();
			this._disposed = true;
		}

		public abstract int GetLightmapIndex();

		public abstract void ClearBuffers();

		public abstract void ClearMesh();

		public abstract void ClearMesh(MB2_EditorMethodsInterface editorMethods);

		internal abstract void _DisposeRuntimeCreated();

		public abstract void DestroyMesh();

		public abstract void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods);

		public abstract List<GameObject> GetObjectsInCombined();

		public abstract int GetNumObjectsInCombined();

		public virtual bool Apply()
		{
			return this.Apply(null);
		}

		public abstract bool Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod);

		public abstract bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool uv5, bool uv6, bool uv7, bool uv8, bool colors, bool bones = false, bool blendShapeFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null);

		public abstract bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapeFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null);

		public virtual bool UpdateGameObjects(GameObject[] gos)
		{
			return this.UpdateGameObjects(gos, true, true, true, true, true, false, false, false, false, false, false, false, false, false);
		}

		public virtual bool UpdateGameObjects(GameObject[] gos, bool updateBounds)
		{
			return this.UpdateGameObjects(gos, updateBounds, true, true, true, true, false, false, false, false, false, false, false, false, false);
		}

		public abstract bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo);

		public abstract bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo);

		public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);

		public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource);

		public abstract bool CombinedMeshContains(GameObject go);

		public abstract void UpdateSkinnedMeshApproximateBounds();

		public abstract void UpdateSkinnedMeshApproximateBoundsFromBones();

		public abstract void CheckIntegrity();

		public abstract void UpdateSkinnedMeshApproximateBoundsFromBounds();

		public static void UpdateSkinnedMeshApproximateBoundsFromBonesStatic(Transform[] bs, SkinnedMeshRenderer smr)
		{
			Vector3 position = bs[0].position;
			Vector3 position2 = bs[0].position;
			for (int i = 1; i < bs.Length; i++)
			{
				Vector3 position3 = bs[i].position;
				if (position3.x < position2.x)
				{
					position2.x = position3.x;
				}
				if (position3.y < position2.y)
				{
					position2.y = position3.y;
				}
				if (position3.z < position2.z)
				{
					position2.z = position3.z;
				}
				if (position3.x > position.x)
				{
					position.x = position3.x;
				}
				if (position3.y > position.y)
				{
					position.y = position3.y;
				}
				if (position3.z > position.z)
				{
					position.z = position3.z;
				}
			}
			Vector3 v = (position + position2) / 2f;
			Vector3 v2 = position - position2;
			Matrix4x4 worldToLocalMatrix = smr.worldToLocalMatrix;
			Bounds localBounds = new Bounds(worldToLocalMatrix * v, worldToLocalMatrix * v2);
			smr.localBounds = localBounds;
		}

		public static void UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(List<GameObject> objectsInCombined, SkinnedMeshRenderer smr)
		{
			Bounds bounds = default(Bounds);
			Bounds localBounds = default(Bounds);
			if (MB_Utility.GetBounds(objectsInCombined[0], out bounds))
			{
				localBounds = bounds;
				for (int i = 1; i < objectsInCombined.Count; i++)
				{
					if (!MB_Utility.GetBounds(objectsInCombined[i], out bounds))
					{
						Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
						return;
					}
					localBounds.Encapsulate(bounds);
				}
				smr.localBounds = localBounds;
				return;
			}
			Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
		}

		protected virtual bool _CreateTemporaryTextrueBakeResult(GameObject[] gos, List<Material> matsOnTargetRenderer)
		{
			if (this.GetNumObjectsInCombined() > 0)
			{
				Debug.LogError("Can't add objects if there are already objects in combined mesh when 'Texture Bake Result' is not set. Perhaps enable 'Clear Buffers After Bake'");
				return false;
			}
			this._usingTemporaryTextureBakeResult = true;
			this._textureBakeResults = MB2_TextureBakeResults.CreateForMaterialsOnRenderer(gos, matsOnTargetRenderer);
			return true;
		}

		public abstract List<Material> GetMaterialsOnTargetRenderer();

		[SerializeField]
		protected MB3_MeshCombiner.MeshCombiningStatus _bakeStatus;

		[SerializeField]
		protected MB2_ValidationLevel _validationLevel = MB2_ValidationLevel.robust;

		[SerializeField]
		protected string _name;

		[SerializeField]
		protected MB2_TextureBakeResults _textureBakeResults;

		[SerializeField]
		protected GameObject _resultSceneObject;

		[SerializeField]
		protected Renderer _targetRenderer;

		[SerializeField]
		protected MB2_LogLevel _LOG_LEVEL = MB2_LogLevel.info;

		[SerializeField]
		protected Object _settingsHolder;

		[SerializeField]
		protected MB2_OutputOptions _outputOption;

		[SerializeField]
		protected MB_RenderType _renderType;

		[SerializeField]
		protected MB2_LightmapOptions _lightmapOption = MB2_LightmapOptions.ignore_UV2;

		[SerializeField]
		protected bool _doNorm = true;

		[SerializeField]
		protected bool _doTan = true;

		[SerializeField]
		protected bool _doCol;

		[SerializeField]
		protected bool _doUV = true;

		[SerializeField]
		protected bool _doUV3;

		[SerializeField]
		protected bool _doUV4;

		[SerializeField]
		protected bool _doUV5;

		[SerializeField]
		protected bool _doUV6;

		[SerializeField]
		protected bool _doUV7;

		[SerializeField]
		protected bool _doUV8;

		[SerializeField]
		protected bool _doBlendShapes;

		[FormerlySerializedAs("_recenterVertsToBoundsCenter")]
		[SerializeField]
		protected MB_MeshPivotLocation _pivotLocationType;

		[SerializeField]
		protected Vector3 _pivotLocation;

		[SerializeField]
		protected bool _clearBuffersAfterBake;

		[SerializeField]
		public bool _optimizeAfterBake = true;

		[SerializeField]
		[FormerlySerializedAs("uv2UnwrappingParamsHardAngle")]
		protected float _uv2UnwrappingParamsHardAngle = 60f;

		[SerializeField]
		[FormerlySerializedAs("uv2UnwrappingParamsPackMargin")]
		protected float _uv2UnwrappingParamsPackMargin = 0.005f;

		[SerializeField]
		protected bool _smrNoExtraBonesWhenCombiningMeshRenderers;

		[SerializeField]
		protected bool _smrMergeBlendShapesWithSameNames;

		[SerializeField]
		protected Object _assignToMeshCustomizer;

		[SerializeField]
		protected MB_MeshCombineAPIType _meshAPItoUse = MB_MeshCombineAPIType.betaNativeArrayAPI;

		protected bool _usingTemporaryTextureBakeResult;

		private bool _disposed;

		public enum MeshCombiningStatus
		{
			preAddDeleteOrUpdate,
			readyForApply
		}

		public delegate void GenerateUV2Delegate(Mesh m, float hardAngle, float packMargin);

		public class MBBlendShapeKey
		{
			public MBBlendShapeKey(GameObject srcSkinnedMeshRenderGameObject, int blendShapeIndexInSource)
			{
				this.gameObject = srcSkinnedMeshRenderGameObject;
				this.blendShapeIndexInSrc = blendShapeIndexInSource;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is MB3_MeshCombiner.MBBlendShapeKey) || obj == null)
				{
					return false;
				}
				MB3_MeshCombiner.MBBlendShapeKey mbblendShapeKey = (MB3_MeshCombiner.MBBlendShapeKey)obj;
				return this.gameObject == mbblendShapeKey.gameObject && this.blendShapeIndexInSrc == mbblendShapeKey.blendShapeIndexInSrc;
			}

			public override int GetHashCode()
			{
				return (23 * 31 + this.gameObject.GetInstanceID()) * 31 + this.blendShapeIndexInSrc;
			}

			public GameObject gameObject;

			public int blendShapeIndexInSrc;
		}

		public class MBBlendShapeValue
		{
			public GameObject combinedMeshGameObject;

			public int blendShapeIndex;
		}
	}
}
