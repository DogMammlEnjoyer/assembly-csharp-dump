using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_MeshCombinerSettingsData : MB_IMeshBakerSettings
	{
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

		public bool clearBuffersAfterBake
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

		[SerializeField]
		protected MB_RenderType _renderType;

		[SerializeField]
		protected MB2_OutputOptions _outputOption;

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
		protected float _uv2UnwrappingParamsHardAngle = 60f;

		[SerializeField]
		protected float _uv2UnwrappingParamsPackMargin = 0.005f;

		[SerializeField]
		protected bool _smrNoExtraBonesWhenCombiningMeshRenderers;

		[SerializeField]
		protected bool _smrMergeBlendShapesWithSameNames;

		[SerializeField]
		protected Object _assignToMeshCustomizer;

		[SerializeField]
		protected MB_MeshCombineAPIType _meshAPItoUse = MB_MeshCombineAPIType.betaNativeArrayAPI;
	}
}
