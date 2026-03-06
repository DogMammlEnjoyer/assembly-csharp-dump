using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public readonly struct OVRSemanticLabels : IOVRAnchorComponent<OVRSemanticLabels>, IEquatable<OVRSemanticLabels>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRSemanticLabels>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRSemanticLabels>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRSemanticLabels IOVRAnchorComponent<OVRSemanticLabels>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRSemanticLabels(anchor);
	}

	public bool IsNull
	{
		get
		{
			return this.Handle == 0UL;
		}
	}

	public bool IsEnabled
	{
		get
		{
			bool flag;
			bool flag2;
			return !this.IsNull && OVRPlugin.GetSpaceComponentStatus(this.Handle, this.Type, out flag, out flag2) && flag && !flag2;
		}
	}

	OVRTask<bool> IOVRAnchorComponent<OVRSemanticLabels>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The SemanticLabels component cannot be enabled or disabled.");
	}

	public bool Equals(OVRSemanticLabels other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRSemanticLabels lhs, OVRSemanticLabels rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRSemanticLabels lhs, OVRSemanticLabels rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRSemanticLabels)
		{
			OVRSemanticLabels other = (OVRSemanticLabels)obj;
			return this.Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return this.Handle.GetHashCode() * 486187739 + ((int)this.Type).GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}.SemanticLabels", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.SemanticLabels;
		}
	}

	internal ulong Handle { get; }

	private OVRSemanticLabels(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
	public string Labels
	{
		get
		{
			string labels;
			if (!OVRPlugin.GetSpaceSemanticLabels(this.Handle, out labels))
			{
				throw new Exception("Could not Get Semantic Labels");
			}
			return OVRSemanticClassification.ValidateAndUpgradeLabels(labels);
		}
	}

	public void GetClassifications(ICollection<OVRSemanticLabels.Classification> classifications)
	{
		int length;
		if (!OVRPlugin.GetSpaceSemanticLabelsNonAlloc(this.Handle, ref OVRSemanticLabels._semanticLabelsBuffer, out length))
		{
			throw new Exception("Could not Get Semantic Labels");
		}
		classifications.Clear();
		OVRSemanticLabels.FromApiString(new ReadOnlySpan<char>(OVRSemanticLabels._semanticLabelsBuffer, 0, length), classifications);
		bool flag = classifications.Contains(OVRSemanticLabels.Classification.InvisibleWallFace);
		bool flag2 = classifications.Contains(OVRSemanticLabels.Classification.WallFace);
		if (flag && !flag2)
		{
			classifications.Add(OVRSemanticLabels.Classification.WallFace);
		}
	}

	internal static OVRSemanticLabels.Classification FromApiLabel(ReadOnlySpan<char> singleLabel)
	{
		if (singleLabel.SequenceEqual("FLOOR"))
		{
			return OVRSemanticLabels.Classification.Floor;
		}
		if (singleLabel.SequenceEqual("CEILING"))
		{
			return OVRSemanticLabels.Classification.Ceiling;
		}
		if (singleLabel.SequenceEqual("WALL_FACE"))
		{
			return OVRSemanticLabels.Classification.WallFace;
		}
		if (singleLabel.SequenceEqual("COUCH"))
		{
			return OVRSemanticLabels.Classification.Couch;
		}
		if (singleLabel.SequenceEqual("DOOR_FRAME"))
		{
			return OVRSemanticLabels.Classification.DoorFrame;
		}
		if (singleLabel.SequenceEqual("WINDOW_FRAME"))
		{
			return OVRSemanticLabels.Classification.WindowFrame;
		}
		if (singleLabel.SequenceEqual("OTHER"))
		{
			return OVRSemanticLabels.Classification.Other;
		}
		if (singleLabel.SequenceEqual("STORAGE"))
		{
			return OVRSemanticLabels.Classification.Storage;
		}
		if (singleLabel.SequenceEqual("BED"))
		{
			return OVRSemanticLabels.Classification.Bed;
		}
		if (singleLabel.SequenceEqual("SCREEN"))
		{
			return OVRSemanticLabels.Classification.Screen;
		}
		if (singleLabel.SequenceEqual("LAMP"))
		{
			return OVRSemanticLabels.Classification.Lamp;
		}
		if (singleLabel.SequenceEqual("PLANT"))
		{
			return OVRSemanticLabels.Classification.Plant;
		}
		if (singleLabel.SequenceEqual("TABLE"))
		{
			return OVRSemanticLabels.Classification.Table;
		}
		if (singleLabel.SequenceEqual("WALL_ART"))
		{
			return OVRSemanticLabels.Classification.WallArt;
		}
		if (singleLabel.SequenceEqual("INVISIBLE_WALL_FACE"))
		{
			return OVRSemanticLabels.Classification.InvisibleWallFace;
		}
		if (singleLabel.SequenceEqual("GLOBAL_MESH"))
		{
			return OVRSemanticLabels.Classification.SceneMesh;
		}
		Debug.LogWarning("Unknown classification: " + singleLabel.ToString());
		return OVRSemanticLabels.Classification.Other;
	}

	internal static void FromApiString(ReadOnlySpan<char> apiLabels, ICollection<OVRSemanticLabels.Classification> classifications)
	{
		int num = 0;
		int num2;
		while ((num2 = OVRSemanticLabels.<FromApiString>g__IndexOf|30_1(apiLabels, ',', num)) != -1)
		{
			OVRSemanticLabels.<FromApiString>g__AddLabel|30_0(apiLabels.Slice(num, num2 - num), classifications);
			num = num2 + 1;
		}
		if (num < apiLabels.Length)
		{
			OVRSemanticLabels.<FromApiString>g__AddLabel|30_0(apiLabels.Slice(num), classifications);
		}
	}

	internal static string ToApiLabel(OVRSemanticLabels.Classification classification)
	{
		string result;
		switch (classification)
		{
		case OVRSemanticLabels.Classification.Floor:
			result = "FLOOR";
			break;
		case OVRSemanticLabels.Classification.Ceiling:
			result = "CEILING";
			break;
		case OVRSemanticLabels.Classification.WallFace:
			result = "WALL_FACE";
			break;
		case OVRSemanticLabels.Classification.Table:
			result = "TABLE";
			break;
		case OVRSemanticLabels.Classification.Couch:
			result = "COUCH";
			break;
		case OVRSemanticLabels.Classification.DoorFrame:
			result = "DOOR_FRAME";
			break;
		case OVRSemanticLabels.Classification.WindowFrame:
			result = "WINDOW_FRAME";
			break;
		case OVRSemanticLabels.Classification.Other:
			result = "OTHER";
			break;
		case OVRSemanticLabels.Classification.Storage:
			result = "STORAGE";
			break;
		case OVRSemanticLabels.Classification.Bed:
			result = "BED";
			break;
		case OVRSemanticLabels.Classification.Screen:
			result = "SCREEN";
			break;
		case OVRSemanticLabels.Classification.Lamp:
			result = "LAMP";
			break;
		case OVRSemanticLabels.Classification.Plant:
			result = "PLANT";
			break;
		case OVRSemanticLabels.Classification.WallArt:
			result = "WALL_ART";
			break;
		case OVRSemanticLabels.Classification.SceneMesh:
			result = "GLOBAL_MESH";
			break;
		case OVRSemanticLabels.Classification.InvisibleWallFace:
			result = "INVISIBLE_WALL_FACE";
			break;
		default:
			result = "OTHER";
			break;
		}
		return result;
	}

	internal static string ToApiString(IReadOnlyList<OVRSemanticLabels.Classification> classifications)
	{
		if (classifications == null)
		{
			return string.Empty;
		}
		List<string> list;
		string result;
		using (new OVRObjectPool.ListScope<string>(ref list))
		{
			foreach (OVRSemanticLabels.Classification classification in classifications)
			{
				list.Add(OVRSemanticLabels.ToApiLabel(classification));
			}
			result = string.Join<string>(',', list);
		}
		return result;
	}

	[CompilerGenerated]
	internal static void <FromApiString>g__AddLabel|30_0(ReadOnlySpan<char> label, ICollection<OVRSemanticLabels.Classification> labels)
	{
		if (!label.SequenceEqual("DESK"))
		{
			labels.Add(OVRSemanticLabels.FromApiLabel(label));
		}
	}

	[CompilerGenerated]
	internal unsafe static int <FromApiString>g__IndexOf|30_1(ReadOnlySpan<char> s, char c, int start)
	{
		for (int i = start; i < s.Length; i++)
		{
			if (*s[i] == (ushort)c)
			{
				return i;
			}
		}
		return -1;
	}

	public static readonly OVRSemanticLabels Null;

	private static char[] _semanticLabelsBuffer;

	internal const string DeprecationMessage = "String-based labels are deprecated (v65). Please use the equivalent enum-based methods.";

	public enum Classification
	{
		Floor,
		Ceiling,
		WallFace,
		Table,
		Couch,
		DoorFrame,
		WindowFrame,
		Other,
		Storage,
		Bed,
		Screen,
		Lamp,
		Plant,
		WallArt,
		SceneMesh,
		InvisibleWallFace
	}
}
