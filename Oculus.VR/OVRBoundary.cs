using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-ovrboundary/")]
public class OVRBoundary
{
	public bool GetConfigured()
	{
		return OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && OVRPlugin.GetBoundaryConfigured();
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public OVRBoundary.BoundaryTestResult TestNode(OVRBoundary.Node node, OVRBoundary.BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult boundaryTestResult = OVRPlugin.TestBoundaryNode((OVRPlugin.Node)node, (OVRPlugin.BoundaryType)boundaryType);
		return new OVRBoundary.BoundaryTestResult
		{
			IsTriggering = (boundaryTestResult.IsTriggering == OVRPlugin.Bool.True),
			ClosestDistance = boundaryTestResult.ClosestDistance,
			ClosestPoint = boundaryTestResult.ClosestPoint.FromFlippedZVector3f(),
			ClosestPointNormal = boundaryTestResult.ClosestPointNormal.FromFlippedZVector3f()
		};
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public OVRBoundary.BoundaryTestResult TestPoint(Vector3 point, OVRBoundary.BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult boundaryTestResult = OVRPlugin.TestBoundaryPoint(point.ToFlippedZVector3f(), (OVRPlugin.BoundaryType)boundaryType);
		return new OVRBoundary.BoundaryTestResult
		{
			IsTriggering = (boundaryTestResult.IsTriggering == OVRPlugin.Bool.True),
			ClosestDistance = boundaryTestResult.ClosestDistance,
			ClosestPoint = boundaryTestResult.ClosestPoint.FromFlippedZVector3f(),
			ClosestPointNormal = boundaryTestResult.ClosestPointNormal.FromFlippedZVector3f()
		};
	}

	public Vector3[] GetGeometry(OVRBoundary.BoundaryType boundaryType)
	{
		if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
		{
			Debug.LogError("This functionality is not supported in your current version of Unity.");
			return null;
		}
		int num = 0;
		if (OVRPlugin.GetBoundaryGeometry2((OVRPlugin.BoundaryType)boundaryType, IntPtr.Zero, ref num) && num > 0)
		{
			int num2 = num * OVRBoundary.cachedVector3fSize;
			if (OVRBoundary.cachedGeometryNativeBuffer.GetCapacity() < num2)
			{
				OVRBoundary.cachedGeometryNativeBuffer.Reset(num2);
			}
			int num3 = num * 3;
			if (OVRBoundary.cachedGeometryManagedBuffer.Length < num3)
			{
				OVRBoundary.cachedGeometryManagedBuffer = new float[num3];
			}
			if (OVRPlugin.GetBoundaryGeometry2((OVRPlugin.BoundaryType)boundaryType, OVRBoundary.cachedGeometryNativeBuffer.GetPointer(0), ref num))
			{
				Marshal.Copy(OVRBoundary.cachedGeometryNativeBuffer.GetPointer(0), OVRBoundary.cachedGeometryManagedBuffer, 0, num3);
				Vector3[] array = new Vector3[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = new OVRPlugin.Vector3f
					{
						x = OVRBoundary.cachedGeometryManagedBuffer[3 * i],
						y = OVRBoundary.cachedGeometryManagedBuffer[3 * i + 1],
						z = OVRBoundary.cachedGeometryManagedBuffer[3 * i + 2]
					}.FromFlippedZVector3f();
				}
				return array;
			}
		}
		return new Vector3[0];
	}

	public Vector3 GetDimensions(OVRBoundary.BoundaryType boundaryType)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			return OVRPlugin.GetBoundaryDimensions((OVRPlugin.BoundaryType)boundaryType).FromVector3f();
		}
		return Vector3.zero;
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public bool GetVisible()
	{
		return OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && OVRPlugin.GetBoundaryVisible();
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public void SetVisible(bool value)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			OVRPlugin.SetBoundaryVisible(value);
		}
	}

	private static int cachedVector3fSize = Marshal.SizeOf(typeof(OVRPlugin.Vector3f));

	private static OVRNativeBuffer cachedGeometryNativeBuffer = new OVRNativeBuffer(0);

	private static float[] cachedGeometryManagedBuffer = new float[0];

	private List<Vector3> cachedGeometryList = new List<Vector3>();

	public enum Node
	{
		HandLeft = 3,
		HandRight,
		Head = 9
	}

	public enum BoundaryType
	{
		[Obsolete("Deprecated. This enum value will not be supported in OpenXR", false)]
		OuterBoundary = 1,
		PlayArea = 256
	}

	[Obsolete("Deprecated. This struct will not be supported in OpenXR", false)]
	public struct BoundaryTestResult
	{
		public bool IsTriggering;

		public float ClosestDistance;

		public Vector3 ClosestPoint;

		public Vector3 ClosestPointNormal;
	}
}
