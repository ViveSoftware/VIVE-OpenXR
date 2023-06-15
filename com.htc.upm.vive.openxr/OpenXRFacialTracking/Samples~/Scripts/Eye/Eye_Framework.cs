//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
namespace VIVE
{
    namespace FacialTracking.Sample
    {
        public class Eye_Framework : MonoBehaviour
        {
            public enum FrameworkStatus { STOP, START, WORKING, ERROR, NOT_SUPPORT }
            /// <summary>
            /// The status of the Eye engine.
            /// </summary>
            public static FrameworkStatus Status { get; protected set; }

            /// <summary>
            /// Whether to enable Eye module.
            /// </summary>
            public bool EnableEye = true;

            private static Eye_Framework Mgr = null;
            public static Eye_Framework Instance
            {
                get
                {
                    if (Mgr == null)
                    {
                        Mgr = FindObjectOfType<Eye_Framework>();
                    }
                    if (Mgr == null)
                    {
                        Debug.LogError("Eye_Framework not found");
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
            public void StartFramework()
            {
                if (!EnableEye) return;
                if (Status == FrameworkStatus.WORKING || Status == FrameworkStatus.NOT_SUPPORT) return;
                XrFacialTrackerCreateInfoHTC m_expressioncreateInfo = new XrFacialTrackerCreateInfoHTC(
                    XrStructureType.XR_TYPE_FACIAL_TRACKER_CREATE_INFO_HTC,
                    IntPtr.Zero,
                    XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC);
                var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
                int res = feature.xrCreateFacialTrackerHTC(m_expressioncreateInfo, out OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>().m_expressionHandle);
                if (res == (int)XrResult.XR_SUCCESS || res == (int)XrResult.XR_SESSION_LOSS_PENDING)
                {
                    Debug.Log("Initial Eye  success : " + res);
                    Status = FrameworkStatus.WORKING;
                }
                else
                {
                    Debug.LogError("Initial Eye fail : " + res);
                    Status = FrameworkStatus.ERROR;
                }
            }
            [Obsolete("Create FacialManager object and call member function StopFramework instead")]
            public void StopFramework()
            {
                if (Status != FrameworkStatus.NOT_SUPPORT)
                {
                    if (Status != FrameworkStatus.STOP)
                    {
                        var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
                        int res = feature.xrDestroyFacialTrackerHTC(OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>().m_expressionHandle);
                        if (res == (int)XrResult.XR_SUCCESS)
                        {
                            Debug.Log("Release Eye engine  success : " + res);

                        }
                        else
                        {
                            Debug.LogError("Release Eye engine fail : " + res);
                        }
                        
                    }
                    else
                    {
                        Debug.Log("Stop Eye Framework : module not on");
                    }
                }
                Status = FrameworkStatus.STOP;
            }
        }
        
    }
}