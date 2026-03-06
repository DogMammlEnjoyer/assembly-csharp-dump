using System;
using System.Collections.Generic;
using System.Linq;
using OVRSimpleJSON;
using UnityEngine;

public class OVRGLTFAnimatinonNode
{
	public OVRGLTFAnimatinonNode(OVRGLTFInputNode inputNodeType, GameObject gameObj, OVRGLTFAnimationNodeMorphTargetHandler morphTargetHandler)
	{
		this.m_intputNodeType = inputNodeType;
		this.m_gameObj = gameObj;
		this.m_morphTargetHandler = morphTargetHandler;
		this.m_translations.Add(this.CloneVector3(this.m_gameObj.transform.localPosition));
		this.m_rotations.Add(this.CloneQuaternion(this.m_gameObj.transform.localRotation));
		this.m_scales.Add(this.CloneVector3(this.m_gameObj.transform.localScale));
	}

	public void AddChannel(JSONNode channel, JSONNode samplers, OVRGLTFAccessor dataAccessor)
	{
		int asInt = channel["sampler"].AsInt;
		JSONNode jsonnode = channel["target"];
		JSONNode extras = channel["extras"];
		int asInt2 = jsonnode["node"].AsInt;
		OVRGLTFAnimatinonNode.OVRGLTFTransformType transformType = this.GetTransformType(jsonnode["path"].Value);
		this.ProcessAnimationSampler(samplers[asInt], asInt2, transformType, extras, dataAccessor);
	}

	public void UpdatePose(bool down)
	{
		if (this.m_inputNodeState.down == down)
		{
			return;
		}
		this.m_inputNodeState.down = down;
		if (this.m_translations.Count > 1)
		{
			this.m_gameObj.transform.localPosition = (down ? this.m_translations[1] : this.m_translations[0]);
		}
		if (this.m_rotations.Count > 1)
		{
			this.m_gameObj.transform.localRotation = (down ? this.m_rotations[1] : this.m_rotations[0]);
		}
		if (this.m_scales.Count > 1)
		{
			this.SetScale(down ? this.m_scales[1] : this.m_scales[0]);
		}
	}

	public void UpdatePose(float t, bool applyDeadZone = true)
	{
		if (applyDeadZone && Math.Abs(this.m_inputNodeState.t - t) < 0.05f)
		{
			return;
		}
		this.m_inputNodeState.t = t;
		if (this.m_translations.Count > 1)
		{
			this.m_gameObj.transform.localPosition = Vector3.Lerp(this.m_translations[0], this.m_translations[1], t);
		}
		if (this.m_rotations.Count > 1)
		{
			this.m_gameObj.transform.localRotation = Quaternion.Lerp(this.m_rotations[0], this.m_rotations[1], t);
		}
		if (this.m_scales.Count > 1)
		{
			this.SetScale(Vector3.Lerp(this.m_scales[0], this.m_scales[1], t));
		}
		if (this.m_morphTargetHandler != null && this.m_weights.Count > 0)
		{
			int num = this.m_morphTargetHandler.Weights.Length;
			if (this.m_additiveWeightIndex == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_morphTargetHandler.Weights[i] = Mathf.Lerp(this.m_weights[i], this.m_weights[i + num], t);
				}
			}
			else
			{
				this.m_morphTargetHandler.Weights[this.m_additiveWeightIndex] += Mathf.Lerp(this.m_weights[this.m_additiveWeightIndex], this.m_weights[this.m_additiveWeightIndex + num], t);
			}
			this.m_morphTargetHandler.MarkModified();
		}
	}

	public void UpdatePose(Vector2 joystick)
	{
		if (Math.Abs((this.m_inputNodeState.vecT - joystick).magnitude) < 0.05f)
		{
			return;
		}
		this.m_inputNodeState.vecT.x = joystick.x;
		this.m_inputNodeState.vecT.y = joystick.y;
		if (this.m_rotations.Count != 9)
		{
			Debug.LogError("Wrong joystick animation data.");
			return;
		}
		Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection> cardinalThumbsticks = this.GetCardinalThumbsticks(joystick);
		Vector2 cardinalWeights = this.GetCardinalWeights(joystick, cardinalThumbsticks);
		Quaternion quaternion = this.CloneQuaternion(this.m_rotations[0]);
		for (int i = 0; i < 2; i++)
		{
			float num = cardinalWeights[i];
			if (num != 0f)
			{
				int num2 = ((i == 0) ? cardinalThumbsticks.Item1 : cardinalThumbsticks.Item2) - OVRGLTFAnimatinonNode.ThumbstickDirection.North;
				Quaternion b = this.m_rotations[num2 + 1];
				quaternion = Quaternion.Slerp(quaternion, b, num);
			}
		}
		this.m_gameObj.transform.localRotation = quaternion;
		if (this.m_translations.Count > 1 || this.m_scales.Count > 1)
		{
			Debug.LogWarning("Unsupported pose.");
		}
	}

	private Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection> GetCardinalThumbsticks(Vector2 joystick)
	{
		if (joystick.magnitude < 0.005f)
		{
			return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.None, OVRGLTFAnimatinonNode.ThumbstickDirection.None);
		}
		if (joystick.x >= 0f)
		{
			if (joystick.y >= 0f)
			{
				if (joystick.y > joystick.x)
				{
					return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.North, OVRGLTFAnimatinonNode.ThumbstickDirection.NorthEast);
				}
				return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.NorthEast, OVRGLTFAnimatinonNode.ThumbstickDirection.East);
			}
			else
			{
				if (joystick.x > -joystick.y)
				{
					return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.East, OVRGLTFAnimatinonNode.ThumbstickDirection.SouthEast);
				}
				return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.SouthEast, OVRGLTFAnimatinonNode.ThumbstickDirection.South);
			}
		}
		else if (joystick.y < 0f)
		{
			if (joystick.x > joystick.y)
			{
				return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.South, OVRGLTFAnimatinonNode.ThumbstickDirection.SouthWest);
			}
			return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.SouthWest, OVRGLTFAnimatinonNode.ThumbstickDirection.West);
		}
		else
		{
			if (-joystick.x > joystick.y)
			{
				return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.West, OVRGLTFAnimatinonNode.ThumbstickDirection.NorthWest);
			}
			return new Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection>(OVRGLTFAnimatinonNode.ThumbstickDirection.NorthWest, OVRGLTFAnimatinonNode.ThumbstickDirection.North);
		}
	}

	private Vector2 GetCardinalWeights(Vector2 joystick, Tuple<OVRGLTFAnimatinonNode.ThumbstickDirection, OVRGLTFAnimatinonNode.ThumbstickDirection> cardinals)
	{
		if (cardinals.Item1 == OVRGLTFAnimatinonNode.ThumbstickDirection.None || cardinals.Item2 == OVRGLTFAnimatinonNode.ThumbstickDirection.None)
		{
			return new Vector2(0f, 0f);
		}
		Vector2 vector = OVRGLTFAnimatinonNode.CardDirections[(int)cardinals.Item1];
		Vector2 vector2 = OVRGLTFAnimatinonNode.CardDirections[(int)cardinals.Item2];
		float num = Vector2.Dot(vector, vector);
		float num2 = Vector2.Dot(vector, vector2);
		float num3 = Vector2.Dot(vector, joystick);
		float num4 = Vector2.Dot(vector2, vector2);
		float num5 = Vector2.Dot(vector2, joystick);
		float num6 = 1f / (num * num4 - num2 * num2);
		float x = (num4 * num3 - num2 * num5) * num6;
		float y = (num * num5 - num2 * num3) * num6;
		return new Vector2(x, y);
	}

	private void ProcessAnimationSampler(JSONNode samplerNode, int nodeId, OVRGLTFAnimatinonNode.OVRGLTFTransformType transformType, JSONNode extras, OVRGLTFAccessor _dataAccessor)
	{
		int asInt = samplerNode["output"].AsInt;
		if (this.ToOVRInterpolationType(samplerNode["interpolation"].Value) == OVRGLTFAnimatinonNode.OVRInterpolationType.None)
		{
			Debug.LogError("Unsupported interpolation type: " + samplerNode["interpolation"].Value);
			return;
		}
		int asInt2 = samplerNode["input"].AsInt;
		_dataAccessor.Seek(asInt2, false);
		float[] array = _dataAccessor.ReadFloat();
		if (array.Length > 2 && this.m_intputNodeType == OVRGLTFInputNode.None)
		{
			Debug.LogWarning("Unsupported keyframe count");
		}
		_dataAccessor.Seek(asInt, false);
		switch (transformType)
		{
		case OVRGLTFAnimatinonNode.OVRGLTFTransformType.Translation:
			this.CopyData<Vector3>(ref this.m_translations, _dataAccessor.ReadVector3(OVRGLTFLoader.GLTFToUnitySpace));
			return;
		case OVRGLTFAnimatinonNode.OVRGLTFTransformType.Rotation:
			this.CopyData<Quaternion>(ref this.m_rotations, _dataAccessor.ReadQuaterion(OVRGLTFLoader.GLTFToUnitySpace_Rotation));
			return;
		case OVRGLTFAnimatinonNode.OVRGLTFTransformType.Scale:
			this.CopyData<Vector3>(ref this.m_scales, _dataAccessor.ReadVector3(Vector3.one));
			return;
		case OVRGLTFAnimatinonNode.OVRGLTFTransformType.Weights:
			this.CopyData<float>(ref this.m_weights, _dataAccessor.ReadFloat());
			if (extras != null && extras["additiveWeightIndex"] != null)
			{
				this.m_additiveWeightIndex = extras["additiveWeightIndex"].AsInt;
			}
			if (this.m_morphTargetHandler != null)
			{
				this.m_morphTargetHandler.Weights = new float[this.m_weights.Count / array.Length];
				return;
			}
			break;
		default:
			Debug.LogError("Unsupported transform type: " + transformType.ToString());
			break;
		}
	}

	private OVRGLTFAnimatinonNode.OVRGLTFTransformType GetTransformType(string transform)
	{
		if (transform == "translation")
		{
			return OVRGLTFAnimatinonNode.OVRGLTFTransformType.Translation;
		}
		if (transform == "rotation")
		{
			return OVRGLTFAnimatinonNode.OVRGLTFTransformType.Rotation;
		}
		if (transform == "scale")
		{
			return OVRGLTFAnimatinonNode.OVRGLTFTransformType.Scale;
		}
		if (transform == "weights")
		{
			return OVRGLTFAnimatinonNode.OVRGLTFTransformType.Weights;
		}
		if (!(transform == "none"))
		{
			Debug.LogError("Unsupported transform type: " + transform);
			return OVRGLTFAnimatinonNode.OVRGLTFTransformType.None;
		}
		return OVRGLTFAnimatinonNode.OVRGLTFTransformType.None;
	}

	private OVRGLTFAnimatinonNode.OVRInterpolationType ToOVRInterpolationType(string interpolationType)
	{
		if (interpolationType == "LINEAR")
		{
			return OVRGLTFAnimatinonNode.OVRInterpolationType.LINEAR;
		}
		if (interpolationType == "STEP")
		{
			Debug.LogError("Unsupported interpolationType type." + interpolationType);
			return OVRGLTFAnimatinonNode.OVRInterpolationType.STEP;
		}
		if (!(interpolationType == "CUBICSPLINE"))
		{
			Debug.LogError("Unsupported interpolationType type." + interpolationType);
			return OVRGLTFAnimatinonNode.OVRInterpolationType.None;
		}
		Debug.LogError("Unsupported interpolationType type." + interpolationType);
		return OVRGLTFAnimatinonNode.OVRInterpolationType.CUBICSPLINE;
	}

	private void CopyData<T>(ref List<T> dest, T[] src)
	{
		if (this.m_intputNodeType == OVRGLTFInputNode.None)
		{
			dest = src.ToList<T>();
			return;
		}
		if (this.m_intputNodeType == OVRGLTFInputNode.ThumbStick)
		{
			using (List<int>.Enumerator enumerator = OVRGLTFAnimatinonNode.ThumbStickKeyFrames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num = enumerator.Current;
					if (num < src.Length)
					{
						dest.Add(src[num]);
					}
				}
				return;
			}
		}
		int num2 = OVRGLTFAnimatinonNode.InputNodeKeyFrames[this.m_intputNodeType];
		if (num2 < src.Length)
		{
			dest.Add(src[num2]);
		}
	}

	private Vector3 CloneVector3(Vector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	private Quaternion CloneQuaternion(Quaternion q)
	{
		return new Quaternion(q.x, q.y, q.z, q.w);
	}

	private void SetScale(Vector3 scale)
	{
		this.m_gameObj.transform.localScale = scale;
		this.m_gameObj.SetActive(this.m_gameObj.transform.localScale != Vector3.zero);
	}

	private OVRGLTFInputNode m_intputNodeType;

	private JSONNode m_jsonData;

	private GameObject m_gameObj;

	private OVRGLTFAnimatinonNode.InputNodeState m_inputNodeState;

	private OVRGLTFAnimationNodeMorphTargetHandler m_morphTargetHandler;

	private List<Vector3> m_translations = new List<Vector3>();

	private List<Quaternion> m_rotations = new List<Quaternion>();

	private List<Vector3> m_scales = new List<Vector3>();

	private List<float> m_weights = new List<float>();

	private int m_additiveWeightIndex = -1;

	private static Dictionary<OVRGLTFInputNode, int> InputNodeKeyFrames = new Dictionary<OVRGLTFInputNode, int>
	{
		{
			OVRGLTFInputNode.Button_A_X,
			5
		},
		{
			OVRGLTFInputNode.Button_B_Y,
			8
		},
		{
			OVRGLTFInputNode.Button_Oculus_Menu,
			24
		},
		{
			OVRGLTFInputNode.Trigger_Grip,
			21
		},
		{
			OVRGLTFInputNode.Trigger_Front,
			16
		},
		{
			OVRGLTFInputNode.ThumbStick,
			0
		}
	};

	private static List<int> ThumbStickKeyFrames = new List<int>
	{
		29,
		39,
		34,
		40,
		31,
		36,
		32,
		37
	};

	private static Vector2[] CardDirections = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f),
		new Vector2(1f, 0f),
		new Vector2(1f, -1f),
		new Vector2(0f, -1f),
		new Vector2(-1f, -1f),
		new Vector2(-1f, 0f),
		new Vector2(-1f, 1f)
	};

	private enum ThumbstickDirection
	{
		None,
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest
	}

	private enum OVRGLTFTransformType
	{
		None,
		Translation,
		Rotation,
		Scale,
		Weights
	}

	private enum OVRInterpolationType
	{
		None,
		LINEAR,
		STEP,
		CUBICSPLINE
	}

	private struct InputNodeState
	{
		public bool down;

		public float t;

		public Vector2 vecT;
	}
}
