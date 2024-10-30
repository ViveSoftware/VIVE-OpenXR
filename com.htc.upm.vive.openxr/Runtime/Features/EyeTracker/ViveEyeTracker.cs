// Copyright HTC Corporation All Rights Reserved.

using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using AOT;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace VIVE.OpenXR.EyeTracker
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "VIVE XR Eye Tracker",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone },
        Company = "HTC",
        Desc = "Support the eye tracker extension.",
        DocumentationLink = "..\\Documentation",
        OpenxrExtensionStrings = kOpenxrExtensionString,
        Version = "1.0.0",
        FeatureId = featureId)]
#endif
    public class ViveEyeTracker : OpenXRFeature
    {
        const string LOG_TAG = "VIVE.OpenXR.Eye.ViveEyeTracker";
        void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
        void WARNING(string msg) { Debug.LogWarning(LOG_TAG + " " + msg); }
        void ERROR(string msg) { Debug.LogError(LOG_TAG + " " + msg); }

        public const string kOpenxrExtensionString = "XR_HTC_eye_tracker";

        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "vive.openxr.feature.eye.tracker";

        #region OpenXR Life Cycle
        private bool m_XrInstanceCreated = false;
        private XrInstance m_XrInstance = 0;
        private static IntPtr xrGetInstanceProcAddr_prev;
        private static IntPtr WaitFrame_prev;
        private static XrFrameWaitInfo m_frameWaitInfo;
        private static XrFrameState m_frameState;
        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            UnityEngine.Debug.Log("EXT: registering our own xrGetInstanceProcAddr");
            xrGetInstanceProcAddr_prev = func;
            return Marshal.GetFunctionPointerForDelegate(m_intercept_xrWaitFrame_xrGetInstanceProcAddr);
        }
        [MonoPInvokeCallback(typeof(OpenXRHelper.xrGetInstanceProcAddrDelegate))]
        private static XrResult intercept_xrWaitFrame_xrGetInstanceProcAddr(XrInstance instance, string name, out IntPtr function)
        {
            if (xrGetInstanceProcAddr_prev == null || xrGetInstanceProcAddr_prev == IntPtr.Zero)
            {
                UnityEngine.Debug.LogError("xrGetInstanceProcAddr_prev is null");
                function = IntPtr.Zero;
                return XrResult.XR_ERROR_VALIDATION_FAILURE;
            }

            // Get delegate of old xrGetInstanceProcAddr.
            var xrGetProc = Marshal.GetDelegateForFunctionPointer<OpenXRHelper.xrGetInstanceProcAddrDelegate>(xrGetInstanceProcAddr_prev);
            XrResult result = xrGetProc(instance, name, out function);
            if (name == "xrWaitFrame")
            {
                WaitFrame_prev = function;
                m_intercept_xrWaitFrame = intercepted_xrWaitFrame;
                function = Marshal.GetFunctionPointerForDelegate(m_intercept_xrWaitFrame); ;
                UnityEngine.Debug.Log("Getting xrWaitFrame func");
            }

            return result;

        }
        [MonoPInvokeCallback(typeof(OpenXRHelper.xrWaitFrameDelegate))]
        private static int intercepted_xrWaitFrame(ulong session, ref XrFrameWaitInfo frameWaitInfo, ref XrFrameState frameState)
        {
            // Get delegate of prev xrWaitFrame.
            var xrWaitFrame = Marshal.GetDelegateForFunctionPointer<OpenXRHelper.xrWaitFrameDelegate>(WaitFrame_prev);
            int res = xrWaitFrame(session, ref frameWaitInfo, ref frameState);
            m_frameWaitInfo = frameWaitInfo;
            m_frameState = frameState;
            return res;
        }
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreateInstance">xrCreateInstance</see> is done.
        /// </summary>
        /// <param name="xrInstance">The created instance.</param>
        /// <returns>True for valid <see cref="XrInstance">XrInstance</see></returns>
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled(kOpenxrExtensionString))
            {
                WARNING("OnInstanceCreate() " + kOpenxrExtensionString + " is NOT enabled.");
                return false;
            }

            m_XrInstanceCreated = true;
            m_XrInstance = xrInstance;
            DEBUG("OnInstanceCreate() " + m_XrInstance);
            return GetXrFunctionDelegates(m_XrInstance);
        }
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroyInstance">xrDestroyInstance</see> is done.
        /// </summary>
        /// <param name="xrInstance">The instance to destroy.</param>
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            m_XrInstanceCreated = false;
            m_XrInstance = 0;
            DEBUG("OnInstanceDestroy() " + xrInstance);
        }

        private XrSystemId m_XrSystemId = 0;
        /// <summary>
        /// Called when the <see cref="XrSystemId">XrSystemId</see> retrieved by <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrGetSystem">xrGetSystem</see> is changed.
        /// </summary>
        /// <param name="xrSystem">The system id.</param>
        protected override void OnSystemChange(ulong xrSystem)
        {
            m_XrSystemId = xrSystem;
            DEBUG("OnSystemChange() " + m_XrSystemId);
        }

        private bool m_XrSessionCreated = false;
        private XrSession m_XrSession = 0;
        private bool hasEyeTracker = false;
        private XrEyeTrackerHTC m_EyeTracker = 0;
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreateSession">xrCreateSession</see> is done.
        /// </summary>
        /// <param name="xrSession">The created session ID.</param>
        protected override void OnSessionCreate(ulong xrSession)
        {
            m_XrSession = xrSession;
            m_XrSessionCreated = true;
            DEBUG("OnSessionCreate() " + m_XrSession);

            if (CreateEyeTracker()) { DEBUG("OnSessionCreate() m_EyeTracker " + m_EyeTracker); }
        }
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroySession">xrDestroySession</see> is done.
        /// </summary>
        /// <param name="xrSession">The session ID to destroy.</param>
        protected override void OnSessionDestroy(ulong xrSession)
        {
            DEBUG("OnSessionDestroy() " + xrSession);

            // Eye Tracking is binding with xrSession so we destroy the trackers when xrSession is destroyed.
            DestroyEyeTracker();

            m_XrSession = 0;
            m_XrSessionCreated = false;
        }
        #endregion

        #region OpenXR function delegates
        private static readonly OpenXRHelper.xrGetInstanceProcAddrDelegate m_intercept_xrWaitFrame_xrGetInstanceProcAddr
            = new OpenXRHelper.xrGetInstanceProcAddrDelegate(intercept_xrWaitFrame_xrGetInstanceProcAddr);
        private static OpenXRHelper.xrWaitFrameDelegate m_intercept_xrWaitFrame;
        /// xrGetInstanceProcAddr
        OpenXRHelper.xrGetInstanceProcAddrDelegate XrGetInstanceProcAddr;

        /// xrGetSystemProperties
        OpenXRHelper.xrGetSystemPropertiesDelegate xrGetSystemProperties;
        private XrResult GetSystemProperties(ref XrSystemProperties properties)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("GetSystemProperties() XR_ERROR_SESSION_LOST.");
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("GetSystemProperties() XR_ERROR_INSTANCE_LOST.");
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }

            return xrGetSystemProperties(m_XrInstance, m_XrSystemId, ref properties);
        }

        /// xrDestroySpace
        OpenXRHelper.xrDestroySpaceDelegate xrDestroySpace;
        private XrResult DestroySpace(XrSpace space)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("DestroySpace() XR_ERROR_SESSION_LOST.");
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("DestroySpace() XR_ERROR_INSTANCE_LOST.");
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }

            return xrDestroySpace(space);
        }

        ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate xrCreateEyeTrackerHTC;
        private XrResult CreateEyeTrackerHTC(ref XrEyeTrackerCreateInfoHTC createInfo, out XrEyeTrackerHTC eyeTracker)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("CreateEyeTrackerHTC() XR_ERROR_SESSION_LOST.");
                eyeTracker = 0;
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("CreateEyeTrackerHTC() XR_ERROR_INSTANCE_LOST.");
                eyeTracker = 0;
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }

            return xrCreateEyeTrackerHTC(m_XrSession, ref createInfo, out eyeTracker);
        }

        ViveEyeTrackerHelper.xrDestroyEyeTrackerHTCDelegate xrDestroyEyeTrackerHTC;
        private XrResult DestroyEyeTrackerHTC(XrEyeTrackerHTC eyeTracker)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("DestroyEyeTrackerHTC() XR_ERROR_SESSION_LOST.");
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("DestroyEyeTrackerHTC() XR_ERROR_INSTANCE_LOST.");
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }

            return xrDestroyEyeTrackerHTC(eyeTracker);
        }

        ViveEyeTrackerHelper.xrGetEyeGazeDataHTCDelegate xrGetEyeGazeDataHTC;
        private XrResult GetEyeGazeDataHTC(XrEyeTrackerHTC eyeTracker,ref XrEyeGazeDataInfoHTC gazeInfo, ref XrEyeGazeDataHTC eyeGazes)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("GetEyeGazeDataHTC() XR_ERROR_SESSION_LOST.");
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("GetEyeGazeDataHTC() XR_ERROR_INSTANCE_LOST.");
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }

            XrResult res =  xrGetEyeGazeDataHTC(eyeTracker,ref gazeInfo,ref eyeGazes);
            return res;
        }
        ViveEyeTrackerHelper.xrGetEyePupilDataHTCDelegate xrGetEyePupilDataHTC;

        private XrResult GetEyePupilDataHTC(XrEyeTrackerHTC eyeTracker,ref XrEyePupilDataInfoHTC pupilDataInfo,ref XrEyePupilDataHTC pupilData)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("GetEyePupilData() XR_ERROR_SESSION_LOST.");
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("GetEyePupilData() XR_ERROR_INSTANCE_LOST.");
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }
            return xrGetEyePupilDataHTC(eyeTracker,ref pupilDataInfo, ref pupilData);
        }
        ViveEyeTrackerHelper.xrGetEyeGeometricDataHTC xrGetEyeGeometricDataHTC;
        private XrResult GetEyeGeometricDataHTC(XrEyeTrackerHTC eyeTracker,
            ref XrEyeGeometricDataInfoHTC info,
            ref XrEyeGeometricDataHTC eyeGeometricData)
        {
            if (!m_XrSessionCreated)
            {
                ERROR("GetEyeGeometricData() XR_ERROR_SESSION_LOST.");
                return XrResult.XR_ERROR_SESSION_LOST;
            }
            if (!m_XrInstanceCreated)
            {
                ERROR("GetEyeGeometricData() XR_ERROR_INSTANCE_LOST.");
                return XrResult.XR_ERROR_INSTANCE_LOST;
            }
            return xrGetEyeGeometricDataHTC(eyeTracker,ref info, ref eyeGeometricData);
        }

        private bool GetXrFunctionDelegates(XrInstance xrInstance)
        {
            /// xrGetInstanceProcAddr
            if (xrGetInstanceProcAddr != null && xrGetInstanceProcAddr != IntPtr.Zero)
            {
                DEBUG("Get function pointer of xrGetInstanceProcAddr.");
                XrGetInstanceProcAddr = Marshal.GetDelegateForFunctionPointer(
                    xrGetInstanceProcAddr,
                    typeof(OpenXRHelper.xrGetInstanceProcAddrDelegate)) as OpenXRHelper.xrGetInstanceProcAddrDelegate;
            }
            else
            {
                ERROR("xrGetInstanceProcAddr is null");
                return false;
            }

            IntPtr funcPtr = IntPtr.Zero;
            /// xrGetSystemProperties
            if (XrGetInstanceProcAddr(xrInstance, "xrGetSystemProperties", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrGetSystemProperties.");
                    xrGetSystemProperties = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(OpenXRHelper.xrGetSystemPropertiesDelegate)) as OpenXRHelper.xrGetSystemPropertiesDelegate;
                }
            }
            else
            {
                ERROR("xrGetSystemProperties");
                return false;
            }
            /// xrDestroySpace
            if (XrGetInstanceProcAddr(xrInstance, "xrDestroySpace", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrDestroySpace.");
                    xrDestroySpace = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(OpenXRHelper.xrDestroySpaceDelegate)) as OpenXRHelper.xrDestroySpaceDelegate;
                }
            }
            else
            {
                ERROR("xrDestroySpace");
                return false;
            }

            /// xrCreateEyeTrackerHTC
            if (XrGetInstanceProcAddr(xrInstance, "xrCreateEyeTrackerHTC", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrCreateEyeTrackerHTC.");
                    xrCreateEyeTrackerHTC = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate)) as ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate;
                }
            }
            else
            {
                ERROR("xrCreateEyeTrackerHTC");
                return false;
            }
            /// xrDestroyEyeTrackerHTC
            if (XrGetInstanceProcAddr(xrInstance, "xrDestroyEyeTrackerHTC", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrDestroyEyeTrackerHTC.");
                    xrDestroyEyeTrackerHTC = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(ViveEyeTrackerHelper.xrDestroyEyeTrackerHTCDelegate)) as ViveEyeTrackerHelper.xrDestroyEyeTrackerHTCDelegate;
                }
            }
            else
            {
                ERROR("xrDestroyEyeTrackerHTC");
                return false;
            }
            /// xrGetEyeGazeDataHTC
            if (XrGetInstanceProcAddr(xrInstance, "xrGetEyeGazeDataHTC", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrGetEyeGazeDataHTC.");
                    xrGetEyeGazeDataHTC = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(ViveEyeTrackerHelper.xrGetEyeGazeDataHTCDelegate)) as ViveEyeTrackerHelper.xrGetEyeGazeDataHTCDelegate;
                }
            }
            else
            {
                ERROR("xrGetEyeGazeDataHTC");
                return false;
            }
            /// xrGetEyePupilDataHTC
            if (XrGetInstanceProcAddr(xrInstance, "xrGetEyePupilDataHTC", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrGetEyePupilDataHTC.");
                    xrGetEyePupilDataHTC = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(ViveEyeTrackerHelper.xrGetEyePupilDataHTCDelegate)) as ViveEyeTrackerHelper.xrGetEyePupilDataHTCDelegate;
                }
            }
            else
            {
                ERROR("xrGetEyePupilDataHTC");
                return false;
            }
            /// xrGetEyeGeometricDataHTC
            if (XrGetInstanceProcAddr(xrInstance, "xrGetEyeGeometricDataHTC", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrGetEyeGeometricDataHTC.");
                    xrGetEyeGeometricDataHTC = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(ViveEyeTrackerHelper.xrGetEyeGeometricDataHTC)) as ViveEyeTrackerHelper.xrGetEyeGeometricDataHTC;
                }
            }
            else
            {
                ERROR("xrGetEyeGeometricDataHTC");
                return false;
            }

            return true;
        }
        #endregion

        XrSystemEyeTrackingPropertiesHTC eyeTrackingSystemProperties;
        XrSystemProperties systemProperties;
        private bool IsEyeTrackingSupported()
        {
            if (!m_XrSessionCreated)
            {
                ERROR("IsEyeTrackingSupported() session is not created.");
                return false;
            }

            eyeTrackingSystemProperties.type = XrStructureType.XR_TYPE_SYSTEM_EYE_TRACKING_PROPERTIES_HTC;
            systemProperties.type = XrStructureType.XR_TYPE_SYSTEM_PROPERTIES;
            systemProperties.next = Marshal.AllocHGlobal(Marshal.SizeOf(eyeTrackingSystemProperties));

            long offset = 0;
            if (IntPtr.Size == 4)
                offset = systemProperties.next.ToInt32();
            else
                offset = systemProperties.next.ToInt64();

            IntPtr sys_eye_tracking_prop_ptr = new IntPtr(offset);
            Marshal.StructureToPtr(eyeTrackingSystemProperties, sys_eye_tracking_prop_ptr, false);

            if (GetSystemProperties(ref systemProperties) == XrResult.XR_SUCCESS)
            {
                if (IntPtr.Size == 4)
                    offset = systemProperties.next.ToInt32();
                else
                    offset = systemProperties.next.ToInt64();

                sys_eye_tracking_prop_ptr = new IntPtr(offset);
                eyeTrackingSystemProperties = (XrSystemEyeTrackingPropertiesHTC)Marshal.PtrToStructure(sys_eye_tracking_prop_ptr, typeof(XrSystemEyeTrackingPropertiesHTC));

                DEBUG("IsEyeTrackingSupported() XrSystemEyeTrackingPropertiesHTC.supportsEyeTracking: "
                    + eyeTrackingSystemProperties.supportsEyeTracking);

                return (eyeTrackingSystemProperties.supportsEyeTracking > 0);
            }
            else
            {
                ERROR("IsEyeTrackingSupported() GetSystemProperties failed.");
            }

            return false;
        }

        /// <summary>
        /// An application can create an <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> handle using CreateEyeTracker.
        /// </summary>
        /// <param name="createInfo">The <see cref="XrEyeTrackerCreateInfoHTC">XrEyeTrackerCreateInfoHTC</see> used to specify the eye tracker.</param>
        /// <param name="eyeTracker">The returned XrEyeTrackerHTC handle.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public XrResult CreateEyeTracker(XrEyeTrackerCreateInfoHTC createInfo, out XrEyeTrackerHTC eyeTracker)
        {
            if (hasEyeTracker)
            {
                eyeTracker = m_EyeTracker;
                DEBUG("CreateEyeTracker() m_EyeTracker: " + eyeTracker + " already created before.");
                return XrResult.XR_SUCCESS;
            }

            if (!IsEyeTrackingSupported())
            {
                ERROR("CreateEyeTracker() is NOT supported.");
                eyeTracker = 0;
                return XrResult.XR_ERROR_VALIDATION_FAILURE;
            }

            var result = CreateEyeTrackerHTC(ref createInfo, out eyeTracker);
            DEBUG("CreateEyeTracker() " + result + ", eyeTracker: " + eyeTracker);

            if (result == XrResult.XR_SUCCESS)
            {
                hasEyeTracker = true;
                m_EyeTracker = eyeTracker;
                DEBUG("CreateEyeTracker() m_EyeTracker " + m_EyeTracker);
            }
            return result;
        }
        /// <summary>
        /// An application can create an <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> handle using CreateEyeTracker.
        /// </summary>
        /// <returns>True for success.</returns>
        public bool CreateEyeTracker()
        {
            XrEyeTrackerCreateInfoHTC createInfo = new XrEyeTrackerCreateInfoHTC(
                in_type: XrStructureType.XR_TYPE_EYE_TRACKER_CREATE_INFO_HTC,
                in_next: IntPtr.Zero);

            var result = CreateEyeTracker(createInfo, out XrEyeTrackerHTC value);
            DEBUG("CreateEyeTracker() " +  " tracker: " + value);
            return result == XrResult.XR_SUCCESS;
        }

        /// <summary>
        /// Releases the eye tracker and the underlying resources when the eye tracking experience is over.
        /// </summary>
        /// <param name="eyeTracker">An XrEyeTrackerHTC previously created by xrCreateEyeTrackerHTC.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public XrResult DestroyEyeTracker(XrEyeTrackerHTC eyeTracker)
        {
            XrResult result = DestroyEyeTrackerHTC(eyeTracker);
            DEBUG("DestroyEyeTracker() " + eyeTracker + ", result: " + result);

            return result;
        }
        /// <summary>
        /// Releases the eye tracker and the underlying resources when the eye tracking experience is over.
        /// </summary>
        /// <returns>True for success.</returns>
        public bool DestroyEyeTracker()
        {
            if (!hasEyeTracker)
            {
                DEBUG("DestroyEyeTracker() no " + "tracker.");
                return true;
            }

            XrResult ret = XrResult.XR_ERROR_VALIDATION_FAILURE;

            ret = DestroyEyeTracker(m_EyeTracker);
            hasEyeTracker = false;
            m_EyeTracker = 0;

            return ret == XrResult.XR_SUCCESS;
        }

        private XrEyeGazeDataHTC m_gazes = new XrEyeGazeDataHTC();// = new XrEyeGazeDataHTC(XrStructureType.XR_TYPE_EYE_GAZE_DATA_HTC, IntPtr.Zero, 0);
        /// <summary>
        /// Retrieves an array of <see cref="XrSingleEyeGazeDataHTC">XrSingleEyeGazeDataHTC</see> containing the returned eye gaze directions.
        /// </summary>
        /// <param name="out_gazes">Output parameter to retrieve an array of <see cref="XrSingleEyeGazeDataHTC">XrSingleEyeGazeDataHTC</see>.</param>
        /// <returns>True for success.</returns>
        public bool GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes)
        {
            m_gazes.type = XrStructureType.XR_TYPE_EYE_GAZE_DATA_HTC;
            m_gazes.next = IntPtr.Zero;
            m_gazes.time =  m_frameState.predictedDisplayTime;

            out_gazes = m_gazes.gaze;
            XrEyeGazeDataInfoHTC gazeInfo = new XrEyeGazeDataInfoHTC(
                in_type: XrStructureType.XR_TYPE_EYE_GAZE_DATA_INFO_HTC,
                in_next: IntPtr.Zero,
                in_baseSpace: GetCurrentAppSpace(),
                in_time: m_frameState.predictedDisplayTime);
            if (GetEyeGazeData(m_EyeTracker, gazeInfo, out m_gazes) == XrResult.XR_SUCCESS)
            {
                out_gazes = m_gazes.gaze;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Retrieves the <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see> data of a <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see>.
        /// </summary>
        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="gazeInfo">The information to get eye gaze.</param>
        /// <param name="eyeGazes">Output parameter to retrieve a pointer to <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see> receiving the returned eye poses.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public XrResult GetEyeGazeData(XrEyeTrackerHTC eyeTracker, XrEyeGazeDataInfoHTC gazeInfo, out XrEyeGazeDataHTC eyeGazes)
        {
            m_gazes.type = XrStructureType.XR_TYPE_EYE_GAZE_DATA_HTC;
            m_gazes.next = IntPtr.Zero;
            m_gazes.time = m_frameState.predictedDisplayTime;
            eyeGazes = m_gazes;
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;
            result = GetEyeGazeDataHTC(eyeTracker,ref gazeInfo,ref m_gazes);
            if (result == XrResult.XR_SUCCESS) { eyeGazes = m_gazes; }
            
            return result;
        }

        private XrEyePupilDataHTC m_eyePupilData = new XrEyePupilDataHTC();

        /// <summary>
        /// Retrieves an array of <see cref="XrSingleEyePupilDataHTC">XrSingleEyePupilDataHTC</see> containing the returned data for user's pupils.
        /// </summary>
        /// <param name="pupilData">Output parameter to retrieve an array of <see cref="XrSingleEyePupilDataHTC">XrSingleEyePupilDataHTC</see>.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public bool GetEyePupilData(out XrSingleEyePupilDataHTC[] pupilData)
        {
            m_eyePupilData.type = XrStructureType.XR_TYPE_EYE_PUPIL_DATA_HTC;
            m_eyePupilData.next = IntPtr.Zero;
            m_eyePupilData.time = m_frameState.predictedDisplayTime;
            pupilData = m_eyePupilData.pupilData;
            XrEyePupilDataInfoHTC pupilDataInfo = new XrEyePupilDataInfoHTC(
                in_type: XrStructureType.XR_TYPE_EYE_PUPIL_DATA_INFO_HTC,
                in_next: IntPtr.Zero);
            if (GetEyePupilData(m_EyeTracker, pupilDataInfo, out m_eyePupilData) == XrResult.XR_SUCCESS)
            {
                pupilData = m_eyePupilData.pupilData;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves the <see cref="XrEyePupilDataHTC">XrEyePupilDataHTC</see> data of a <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see>.
        /// </summary>
        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="pupilDataInfo">The information to get pupil data.</param>
        /// <param name="pupilData">A pointer to <see cref="XrEyePupilDataHTC">XrEyePupilDataHTC</see> returned by the runtime.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public XrResult GetEyePupilData(XrEyeTrackerHTC eyeTracker, XrEyePupilDataInfoHTC pupilDataInfo, out XrEyePupilDataHTC pupilData)
        {
            m_eyePupilData.type = XrStructureType.XR_TYPE_EYE_PUPIL_DATA_HTC;
            m_eyePupilData.next = IntPtr.Zero;
            m_eyePupilData.time = m_frameState.predictedDisplayTime;
            pupilData = m_eyePupilData;
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;
            result = GetEyePupilDataHTC(eyeTracker,ref pupilDataInfo, ref m_eyePupilData);
            if (result == XrResult.XR_SUCCESS) { pupilData = m_eyePupilData; }
            return result;
        }

        private XrEyeGeometricDataHTC m_eyeGeometricData = new XrEyeGeometricDataHTC();//XrStructureType.XR_TYPE_EYE_GEOMETRIC_DATA_HTC, IntPtr.Zero, 0);
        /// <param name="geometricData">Output parameter to retrieve an array of <see cref="XrSingleEyeGeometricDataHTC">XrSingleEyeGeometricDataHTC</see>.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public bool GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] geometricData)
        {
            m_eyeGeometricData.type = XrStructureType.XR_TYPE_EYE_GEOMETRIC_DATA_HTC;
            m_eyeGeometricData.next = IntPtr.Zero;
            m_eyeGeometricData.time = m_frameState.predictedDisplayTime;
            geometricData = m_eyeGeometricData.eyeGeometricData;
            XrEyeGeometricDataInfoHTC eyeGeometricDataInfo = new XrEyeGeometricDataInfoHTC(
                in_type: XrStructureType.XR_TYPE_EYE_GEOMETRIC_DATA_INFO_HTC,
                in_next: IntPtr.Zero);
            if (GetEyeGeometricData(m_EyeTracker, eyeGeometricDataInfo, out m_eyeGeometricData) == XrResult.XR_SUCCESS)
            {
                geometricData = m_eyeGeometricData.eyeGeometricData;
                return true;
            }
            return false;
        }

        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="eyeGeometricDataInfo">A pointer to <see cref="XrEyeGeometricDataInfoHTC">XrEyeGeometricDataInfoHTC</see> structure.</param>
        /// <param name="eyeGeometricData">A pointer to <see cref="XrEyeGeometricDataHTC">XrEyeGeometricDataHTC</see> returned by the runtime.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public XrResult GetEyeGeometricData(XrEyeTrackerHTC eyeTracker, XrEyeGeometricDataInfoHTC eyeGeometricDataInfo, out XrEyeGeometricDataHTC eyeGeometricData)
        {
            m_eyeGeometricData.type = XrStructureType.XR_TYPE_EYE_GEOMETRIC_DATA_HTC;
            m_eyeGeometricData.next = IntPtr.Zero;
            m_eyeGeometricData.time = m_frameState.predictedDisplayTime;
            eyeGeometricData = m_eyeGeometricData;
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;
            result = GetEyeGeometricDataHTC(eyeTracker,ref eyeGeometricDataInfo, ref m_eyeGeometricData);
            if (result == XrResult.XR_SUCCESS) { eyeGeometricData = m_eyeGeometricData; }
            return result;
        }
    }
}
