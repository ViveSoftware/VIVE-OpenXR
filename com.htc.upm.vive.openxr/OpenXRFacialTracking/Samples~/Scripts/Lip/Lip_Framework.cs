//========= Copyright 2019, HTC Corporation. All rights reserved. ===========
using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace VIVE
{
    namespace FacialTracking.Sample
    {
        public class Lip_Framework : MonoBehaviour
        {
            public enum FrameworkStatus { STOP, START, WORKING, ERROR }
            /// <summary>
            /// The status of the Lip engine.
            /// </summary>
            public static FrameworkStatus Status { get; protected set; }
            /// <summary>
            /// Whether to enable Lip module.
            /// </summary>
            public bool EnableLip = true;

            private static Lip_Framework Mgr = null;
            public static Lip_Framework Instance
            {
                get
                {
                    if (Mgr == null)
                    {
                        Mgr = FindObjectOfType<Lip_Framework>();
                    }
                    if (Mgr == null)
                    {
                        Debug.LogError("Lip_Framework not found");
                    }
                    return Mgr;
                }
            }

            void Start()
            {
                StartFramework();
            }

            void OnDestroy()
            {
                StopFramework();
            }
            [Obsolete("Create FacialManager object and call member function StartFramework instead")]

            private void StartFramework()
            {
                if (!EnableLip) return;
                if (Status == FrameworkStatus.WORKING) return;

                Status = FrameworkStatus.START;

                Debug.Log("Starting to Initial Lip Engine");
                XrFacialTrackerCreateInfoHTC m_expressioncreateInfo = new XrFacialTrackerCreateInfoHTC(
                    XrStructureType.XR_TYPE_FACIAL_TRACKER_CREATE_INFO_HTC,
                    IntPtr.Zero,
                    XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC);
                var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
                int res = (int)feature.xrCreateFacialTrackerHTC(m_expressioncreateInfo, out OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>().m_expressionHandle_Lip);
                if (res == (int)XrResult.XR_SUCCESS || res == (int)XrResult.XR_SESSION_LOSS_PENDING)
                {
                    Debug.Log("Initial Lip Engine :" + res);
                    Status = FrameworkStatus.WORKING;
                }
                else
                {
                    Debug.LogError("Initial Lip Engine :" + res);
                    Status = FrameworkStatus.ERROR;
                }
                
            }
            [Obsolete("Create FacialManager object and call member function StopFramework instead")]

            public void StopFramework()
            {
                if (Status != FrameworkStatus.STOP)
                {
                    var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
                    int res = feature.xrDestroyFacialTrackerHTC(OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>().m_expressionHandle_Lip);
                    if (res == (int)XrResult.XR_SUCCESS)
                    {
                        Debug.Log("Release Lip Engine : " + res);

                    }
                    else
                    {
                        Debug.LogError("Release Lip Engine : " + res);
                    }
                }
                else
                {
                    Debug.Log("Stop Lip Framework : module not on");
                }
                Status = FrameworkStatus.STOP;
            }
        }
        
    }
}