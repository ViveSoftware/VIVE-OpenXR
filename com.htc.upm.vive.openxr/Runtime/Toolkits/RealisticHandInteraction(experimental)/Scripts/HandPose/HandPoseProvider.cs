
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	public enum HandPoseType : UInt32
	{
		UNKNOWN = 0x7FFFFFFF,

		HAND_LEFT = 100,
		HAND_RIGHT = 101,

		MESH_LEFT = 200,
		MESH_RIGHT = 201,
	}

	public static class HandPoseProvider
	{
		private static Dictionary<HandPoseType, HandPose> m_HandPoseMap = new Dictionary<HandPoseType, HandPose>();
		public static Dictionary<HandPoseType, HandPose> HandPoseMap
		{
			get
			{
				if (m_HandPoseMap == null) { m_HandPoseMap = new Dictionary<HandPoseType, HandPose>(); }
				return m_HandPoseMap;
			}
			private set { m_HandPoseMap = value; }
		}

		public static bool RegisterHandPose(in HandPoseType poseType, in HandPose handPose)
		{
			if (!HandPoseMap.ContainsKey(poseType))
			{
				HandPoseMap.Add(poseType, handPose);
				return true;
			}
			return false;
		}

		public static bool UnregisterHandPose(in HandPoseType poseType)
		{
			if (HandPoseMap.ContainsKey(poseType))
			{
				HandPoseMap.Remove(poseType);
				return true;
			}
			return false;
		}

		public static HandPose GetHandPose(in HandPoseType poseType)
		{
			if (HandPoseMap.ContainsKey(poseType))
			{
				return HandPoseMap[poseType];
			}
			if (poseType == HandPoseType.HAND_LEFT || poseType == HandPoseType.MESH_LEFT)
			{
				return GetDefaultHandPose("LeftHandPose", HandPoseType.HAND_LEFT);
			}
			else if (poseType == HandPoseType.HAND_RIGHT || poseType == HandPoseType.MESH_RIGHT)
			{
				return GetDefaultHandPose("RightHandPose", HandPoseType.HAND_RIGHT);
			}
			return null;
		}

		public static string Name(this HandPoseType poseType)
		{
			string name = "";
			switch (poseType)
			{
				case HandPoseType.HAND_LEFT: name = "HAND_LEFT"; break;
				case HandPoseType.HAND_RIGHT: name = "HAND_LEFT"; break;
				case HandPoseType.MESH_LEFT: name = "MESH_LEFT"; break;
				case HandPoseType.MESH_RIGHT: name = "MESH_RIGHT"; break;
			}
			return name;
		}

		private static HandPose GetDefaultHandPose(string poseName, HandPoseType poseType)
		{
			if (!HandPoseMap.ContainsKey(poseType))
			{
				GameObject handPoseObject = new GameObject(poseName);
				RealHandPose realHandPose = handPoseObject.AddComponent<RealHandPose>();
				realHandPose.SetType(poseType);
				RegisterHandPose(poseType, realHandPose);
				return realHandPose;
			}
			return HandPoseMap[poseType];
		}
	}
}
