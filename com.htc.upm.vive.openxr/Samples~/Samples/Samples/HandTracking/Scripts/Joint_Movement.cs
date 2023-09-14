// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using VIVE.OpenXR.Hand;

namespace VIVE.OpenXR.Samples.Hand
{
    public class Joint_Movement : MonoBehaviour
    {
        public int jointNum = 0;
        public bool isLeft = false;
        [SerializeField] List<GameObject> Childs = new List<GameObject>();

        private Vector3 jointPos = Vector3.zero;
        private Quaternion jointRot = Quaternion.identity;
        public static void GetVectorFromOpenXR(XrVector3f xrVec3, out Vector3 vec)
        {
            vec.x = xrVec3.x;
            vec.y = xrVec3.y;
            vec.z = -xrVec3.z;
        }
        public static void GetQuaternionFromOpenXR(XrQuaternionf xrQuat, out Quaternion qua)
        {
            qua.x = xrQuat.x;
            qua.y = xrQuat.y;
            qua.z = -xrQuat.z;
            qua.w = -xrQuat.w;
        }

        void Update()
        {
            if (!XR_EXT_hand_tracking.Interop.GetJointLocations(isLeft, out XrHandJointLocationEXT[] handJointLocation)) { return; }

            bool poseTracked = false;

            if (((UInt64)handJointLocation[jointNum].locationFlags & (UInt64)XrSpaceLocationFlags.XR_SPACE_LOCATION_ORIENTATION_TRACKED_BIT) != 0)
            {
                GetQuaternionFromOpenXR(handJointLocation[jointNum].pose.orientation, out jointRot);
                transform.rotation = jointRot;
                poseTracked = true;
            }
            if (((UInt64)handJointLocation[jointNum].locationFlags & (UInt64)XrSpaceLocationFlags.XR_SPACE_LOCATION_POSITION_TRACKED_BIT) != 0)
            {
                GetVectorFromOpenXR(handJointLocation[jointNum].pose.position, out jointPos);
                transform.localPosition = jointPos;
                poseTracked = true;
            }

            ActiveChilds(poseTracked);
        }

        void ActiveChilds(bool _SetActive)
        {
            for (int i = 0; i < Childs.Count; i++)
            {
                Childs[i].SetActive(_SetActive);
            }
        }
    }
}
