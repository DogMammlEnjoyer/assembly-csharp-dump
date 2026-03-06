using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Meta.XR.Util;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	[Feature(Feature.TrackedKeyboard)]
	public sealed class MRUKTrackable : MRUKAnchor
	{
		public OVRAnchor.TrackableType TrackableType { get; private set; }

		public bool IsTracked { get; internal set; }

		public string MarkerPayloadString { get; private set; }

		public byte[] MarkerPayloadBytes { get; private set; }

		internal void OnFetch()
		{
			this.TrackableType = base.Anchor.GetTrackableType();
			List<OVRPlugin.SpaceComponentType> list;
			using (new OVRObjectPool.ListScope<OVRPlugin.SpaceComponentType>(ref list))
			{
				if (base.Anchor.GetSupportedComponents(list))
				{
					foreach (OVRPlugin.SpaceComponentType spaceComponentType in list)
					{
						bool flag;
						bool flag2;
						if (OVRPlugin.GetSpaceComponentStatus(base.Anchor.Handle, spaceComponentType, out flag, out flag2) && flag)
						{
							if (spaceComponentType != OVRPlugin.SpaceComponentType.Bounded2D)
							{
								if (spaceComponentType == OVRPlugin.SpaceComponentType.Bounded3D)
								{
									base.VolumeBounds = new Bounds?(base.Anchor.GetComponent<OVRBounded3D>().BoundingBox);
									continue;
								}
								if (spaceComponentType != OVRPlugin.SpaceComponentType.MarkerPayload)
								{
									continue;
								}
								OVRMarkerPayload component = base.Anchor.GetComponent<OVRMarkerPayload>();
								using (NativeArray<byte> nativeArray = new NativeArray<byte>(component.ByteCount, Allocator.Temp, NativeArrayOptions.ClearMemory))
								{
									Span<byte> span = new Span<byte>(nativeArray.GetUnsafePtr<byte>(), nativeArray.Length);
									Span<byte> span2 = span;
									Span<byte> span3 = span2.Slice(0, component.GetBytes(span));
									if (!span3.SequenceEqual(this.MarkerPayloadBytes))
									{
										this.MarkerPayloadBytes = span3.ToArray();
										this.MarkerPayloadString = ((component.PayloadType == OVRMarkerPayloadType.StringQRCode) ? Encoding.UTF8.GetString(this.MarkerPayloadBytes) : null);
									}
									continue;
								}
							}
							OVRBounded2D component2 = base.Anchor.GetComponent<OVRBounded2D>();
							base.PlaneRect = new Rect?(component2.BoundingBox);
							base.PlaneBoundary2D = MRUKTrackable.<OnFetch>g__GetUpdatedBoundary|16_0(component2, base.PlaneBoundary2D);
						}
					}
				}
			}
		}

		internal void OnInstantiate(OVRAnchor anchor)
		{
			base.Anchor = anchor;
			this.IsTracked = true;
			this.OnFetch();
		}

		[CompilerGenerated]
		internal static List<Vector2> <OnFetch>g__GetUpdatedBoundary|16_0(OVRBounded2D component, List<Vector2> currentBoundary)
		{
			int length;
			if (component.TryGetBoundaryPointsCount(out length))
			{
				using (NativeArray<Vector2> positions = new NativeArray<Vector2>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
				{
					if (component.TryGetBoundaryPoints(positions))
					{
						if (currentBoundary == null)
						{
							currentBoundary = new List<Vector2>(positions.Length);
						}
						currentBoundary.Clear();
						foreach (Vector2 item in positions)
						{
							currentBoundary.Add(item);
						}
						return currentBoundary;
					}
				}
			}
			return null;
		}
	}
}
