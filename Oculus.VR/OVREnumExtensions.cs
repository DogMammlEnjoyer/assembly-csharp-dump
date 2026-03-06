using System;

public static class OVREnumExtensions
{
	public static bool IsHand(this OVRSkeleton.SkeletonType skeletonType)
	{
		return skeletonType == OVRSkeleton.SkeletonType.HandLeft || skeletonType == OVRSkeleton.SkeletonType.HandRight || (skeletonType == OVRSkeleton.SkeletonType.XRHandLeft || skeletonType == OVRSkeleton.SkeletonType.XRHandRight);
	}

	public static bool IsOpenXRHandSkeleton(this OVRSkeleton.SkeletonType skeletonType)
	{
		return skeletonType == OVRSkeleton.SkeletonType.XRHandLeft || skeletonType == OVRSkeleton.SkeletonType.XRHandRight;
	}

	public static bool IsOVRHandSkeleton(this OVRSkeleton.SkeletonType skeletonType)
	{
		return skeletonType == OVRSkeleton.SkeletonType.HandLeft || skeletonType == OVRSkeleton.SkeletonType.HandRight;
	}

	public static bool IsLeft(this OVRSkeleton.SkeletonType type)
	{
		return type == OVRSkeleton.SkeletonType.HandLeft || type == OVRSkeleton.SkeletonType.XRHandLeft;
	}

	public static OVRHand.Hand AsHandType(this OVRSkeleton.SkeletonType skeletonType)
	{
		switch (skeletonType)
		{
		case OVRSkeleton.SkeletonType.HandLeft:
		case OVRSkeleton.SkeletonType.XRHandLeft:
			return OVRHand.Hand.HandLeft;
		case OVRSkeleton.SkeletonType.HandRight:
		case OVRSkeleton.SkeletonType.XRHandRight:
			return OVRHand.Hand.HandRight;
		}
		return OVRHand.Hand.None;
	}

	[Obsolete("Use the overload which takes an OVRHandSkeletonVersioninstead.")]
	public static OVRSkeleton.SkeletonType AsSkeletonType(this OVRHand.Hand hand)
	{
		if (hand == OVRHand.Hand.HandLeft)
		{
			return OVRSkeleton.SkeletonType.HandLeft;
		}
		if (hand != OVRHand.Hand.HandRight)
		{
			return OVRSkeleton.SkeletonType.None;
		}
		return OVRSkeleton.SkeletonType.HandRight;
	}

	public static OVRSkeleton.SkeletonType AsSkeletonType(this OVRHand.Hand hand, OVRHandSkeletonVersion version)
	{
		if (hand != OVRHand.Hand.HandLeft)
		{
			if (hand != OVRHand.Hand.HandRight)
			{
				return OVRSkeleton.SkeletonType.None;
			}
			if (version != OVRHandSkeletonVersion.OVR)
			{
				return OVRSkeleton.SkeletonType.XRHandRight;
			}
			return OVRSkeleton.SkeletonType.HandRight;
		}
		else
		{
			if (version != OVRHandSkeletonVersion.OVR)
			{
				return OVRSkeleton.SkeletonType.XRHandLeft;
			}
			return OVRSkeleton.SkeletonType.HandLeft;
		}
	}

	[Obsolete("Use the overload which takes an OVRHandSkeletonVersioninstead.")]
	public static OVRMesh.MeshType AsMeshType(this OVRHand.Hand hand)
	{
		if (hand == OVRHand.Hand.HandLeft)
		{
			return OVRMesh.MeshType.HandLeft;
		}
		if (hand != OVRHand.Hand.HandRight)
		{
			return OVRMesh.MeshType.None;
		}
		return OVRMesh.MeshType.HandRight;
	}

	public static bool IsOpenXRHandMesh(this OVRMesh.MeshType meshType)
	{
		return meshType == OVRMesh.MeshType.XRHandLeft || meshType == OVRMesh.MeshType.XRHandRight;
	}

	public static bool IsOVRHandMesh(this OVRMesh.MeshType meshType)
	{
		return meshType == OVRMesh.MeshType.HandLeft || meshType == OVRMesh.MeshType.HandRight;
	}

	public static OVRMesh.MeshType AsMeshType(this OVRHand.Hand hand, OVRHandSkeletonVersion version)
	{
		if (hand != OVRHand.Hand.HandLeft)
		{
			if (hand != OVRHand.Hand.HandRight)
			{
				return OVRMesh.MeshType.None;
			}
			if (version != OVRHandSkeletonVersion.OVR)
			{
				return OVRMesh.MeshType.XRHandRight;
			}
			return OVRMesh.MeshType.HandRight;
		}
		else
		{
			if (version != OVRHandSkeletonVersion.OVR)
			{
				return OVRMesh.MeshType.XRHandLeft;
			}
			return OVRMesh.MeshType.HandLeft;
		}
	}

	public static bool IsLeft(this OVRMesh.MeshType type)
	{
		return type == OVRMesh.MeshType.HandLeft || type == OVRMesh.MeshType.XRHandLeft;
	}

	public static bool IsHand(this OVRMesh.MeshType meshType)
	{
		return meshType == OVRMesh.MeshType.HandLeft || meshType == OVRMesh.MeshType.HandRight || (meshType == OVRMesh.MeshType.XRHandLeft || meshType == OVRMesh.MeshType.XRHandRight);
	}

	public static OVRHand.Hand AsHandType(this OVRMesh.MeshType meshType)
	{
		switch (meshType)
		{
		case OVRMesh.MeshType.HandLeft:
		case OVRMesh.MeshType.XRHandLeft:
			return OVRHand.Hand.HandLeft;
		case OVRMesh.MeshType.HandRight:
		case OVRMesh.MeshType.XRHandRight:
			return OVRHand.Hand.HandRight;
		}
		return OVRHand.Hand.None;
	}
}
