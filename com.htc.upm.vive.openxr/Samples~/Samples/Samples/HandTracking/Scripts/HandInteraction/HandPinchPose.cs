using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;

namespace VIVE.OpenXR.Samples.Hand
{
    public class HandPinchPose : MonoBehaviour
    {
        [SerializeField] InputActionAsset ActionAsset;
        [SerializeField] InputActionReference RightPinchR;
        [SerializeField] InputActionReference LeftPinchR;

        public GameObject RightPinchPoseGObj;
        public GameObject LeftPinchPoseGObj;
        void OnEnable()
        {
            if (ActionAsset != null)
            {
                ActionAsset.Enable();
            }
        }


        void Update()
        {
            if (IsValidPose(LeftPinchR))
            {
                if (!LeftPinchPoseGObj.transform.parent.gameObject.activeSelf)
                {
                    LeftPinchPoseGObj.transform.parent.gameObject.SetActive(true);
                }
                LeftPinchPoseGObj.transform.localPosition = Get_AimPosition(LeftPinchR);
                LeftPinchPoseGObj.transform.localRotation = Get_AimRotation(LeftPinchR);
            }
            else
            {
                if (LeftPinchPoseGObj.transform.parent.gameObject.activeSelf)
                {
                    LeftPinchPoseGObj.transform.parent.gameObject.SetActive(false);
                }
            }
            if (IsValidPose(RightPinchR))
            {
                if (!RightPinchPoseGObj.transform.parent.gameObject.activeSelf)
                {
                    RightPinchPoseGObj.transform.parent.gameObject.SetActive(true);
                }
                RightPinchPoseGObj.transform.localPosition = Get_AimPosition(RightPinchR);
                RightPinchPoseGObj.transform.localRotation = Get_AimRotation(RightPinchR);
            }
            else
            {
                if (RightPinchPoseGObj.transform.parent.gameObject.activeSelf)
                {
                    RightPinchPoseGObj.transform.parent.gameObject.SetActive(false);
                }
            }
        }
        InputTrackingState trackingStatus = InputTrackingState.None;
        bool IsValidPose(InputActionReference _ActionReference)
        {

#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
            UnityEngine.InputSystem.XR.PoseState _Pose = _ActionReference.action.ReadValue<UnityEngine.InputSystem.XR.PoseState>();

#else
            UnityEngine.XR.OpenXR.Input.Pose _Pose = _ActionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>();
#endif
            trackingStatus = _Pose.trackingState;
            bool positionTracked = ((trackingStatus & InputTrackingState.Position) != 0);
            bool rotationTracked = ((trackingStatus & InputTrackingState.Rotation) != 0);

            if (!_Pose.isTracked || !positionTracked || !rotationTracked)
            {
                return false;
            }

            return true;
        }

        Vector3 Get_AimPosition(InputActionReference _ActionReference)
        {
            Vector3 _Position = Vector3.zero;
#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
            _Position = _ActionReference.action.ReadValue<UnityEngine.InputSystem.XR.PoseState>().position;
#else
            _Position = _ActionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>().position;
#endif
            return _Position;
        }
        Quaternion Get_AimRotation(InputActionReference _ActionReference)
        {
            Quaternion _Rotation = Quaternion.identity;
#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
            _Rotation = _ActionReference.action.ReadValue<UnityEngine.InputSystem.XR.PoseState>().rotation;
#else
            _Rotation = _ActionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>().rotation;
#endif
            return _Rotation;
        }
    }
}
