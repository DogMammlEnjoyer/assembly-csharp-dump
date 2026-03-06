using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public enum HandJointId
	{
		Invalid = -1,
		HandStart,
		[Tooltip("Palm")]
		HandPalm = 0,
		[Tooltip("Wrist Joint")]
		HandWristRoot,
		[Tooltip("Thumb Metacarpal Joint")]
		HandThumb1,
		[Tooltip("Thumb Proximal Joint")]
		HandThumb2,
		[Tooltip("Thumb Distal Joint")]
		HandThumb3,
		[Tooltip("Thumb Tip")]
		HandThumbTip,
		[Tooltip("Index Finger Metacarpal Joint")]
		HandIndex0,
		[Tooltip("Index Finger Proximal Joint")]
		HandIndex1,
		[Tooltip("Index Finger Intermediate Joint")]
		HandIndex2,
		[Tooltip("Index Finger Distal Joint")]
		HandIndex3,
		[Tooltip("Index Finger Tip")]
		HandIndexTip,
		[Tooltip("Middle Finger Metacarpal Joint")]
		HandMiddle0,
		[Tooltip("Middle Finger Proximal Joint")]
		HandMiddle1,
		[Tooltip("Middle Finger Intermediate Joint")]
		HandMiddle2,
		[Tooltip("Middle Finger Distal Joint")]
		HandMiddle3,
		[Tooltip("Middle Finger Tip")]
		HandMiddleTip,
		[Tooltip("Ring Finger Metacarpal Joint")]
		HandRing0,
		[Tooltip("Ring Finger Proximal Joint")]
		HandRing1,
		[Tooltip("Ring Finger Intermediate Joint")]
		HandRing2,
		[Tooltip("Ring Finger Distal Joint")]
		HandRing3,
		[Tooltip("Ring Finger Tip")]
		HandRingTip,
		[Tooltip("Pinky Finger Metacarpal Joint")]
		HandPinky0,
		[Tooltip("Pinky Finger Proximal Joint")]
		HandPinky1,
		[Tooltip("Pinky Finger Intermediate Joint")]
		HandPinky2,
		[Tooltip("Pinky Finger Distal Joint")]
		HandPinky3,
		[Tooltip("Pinky Finger Tip")]
		HandPinkyTip,
		HandEnd,
		HandMaxSkinnable = 26
	}
}
