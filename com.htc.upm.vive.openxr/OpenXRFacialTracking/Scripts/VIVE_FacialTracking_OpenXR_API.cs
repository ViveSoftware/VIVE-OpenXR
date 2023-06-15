using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.XR.OpenXR.Features;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif
namespace VIVE
{
    namespace FacialTracking
    {

#if UNITY_EDITOR
        [OpenXRFeature(UiName = "Facial Tracking",
            BuildTargetGroups = new[] { BuildTargetGroup.Standalone },
            Company = "HTC",
            Desc = "Facial Tracking OpenXR Feature",
            DocumentationLink = "https://developer.vive.com/resources/openxr/openxr-pcvr/tutorials/unity/integrate-facial-tracking-your-avatar/",
            OpenxrExtensionStrings = "XR_HTC_facial_tracking", 
            Version = "0.0.1",
            FeatureId = featureId)]
#endif
        public class VIVE_FacialTracking_OpenXR_API : OpenXRFeature
        {
            /// <summary>
            /// The feature id string. This is used to give the feature a well known id for reference.
            /// </summary>
            public const string featureId = "com.htc.openxr.facialtracking.feature";
            private ulong m_XrInstance;
            private ulong m_XrSession;
            private ulong m_systemid;
            [Obsolete] XrSystemProperties systemProperties;
            [Obsolete] public ulong m_expressionHandle;
            [Obsolete] public ulong m_expressionHandle_Lip;

            public bool IsInitialized { get { return m_xrGetSystemProperties != null; } }
            public bool IsEnabledAndInitialized { get { return enabled && IsInitialized; } }
            public bool IsSessionCreated { get { return XrSession != default; } }
            public ulong XrInstance { get { return m_XrInstance; } }
            public ulong XrSession { get { return m_XrSession; } }
            public ulong SystemId { get { return m_systemid; } }

            public event Action<ulong> onSessionCreate;
            public event Action<ulong> onSessionDestroy;
            public event Action<ulong> onSystemChange;

            /// <inheritdoc />
            protected override bool OnInstanceCreate(ulong xrInstance)
            {
                UnityEngine.Debug.Log($"OnInstanceCreate({xrInstance:X})");
                m_XrInstance = xrInstance;

                return GetXrFunctionDelegates(xrInstance);
            }

            /// <inheritdoc />
            protected override void OnInstanceDestroy(ulong xrInstance)
            {
                UnityEngine.Debug.Log($"OnInstanceDestroy({xrInstance:X})");
                m_XrInstance = default;
            }
            protected override void OnSessionCreate(ulong xrSession)
            {
                UnityEngine.Debug.Log($"OnSessionCreate({xrSession:X})");
                m_XrSession = xrSession;

                try { onSessionCreate?.Invoke(xrSession); }
                catch (Exception e) { UnityEngine.Debug.LogError(e); }
            }
            protected override void OnSystemChange(ulong xrSystem)
            {
                UnityEngine.Debug.Log($"OnSystemChange({xrSystem:X})");
                m_systemid = xrSystem;

                try { onSystemChange?.Invoke(xrSystem); }
                catch (Exception e) { UnityEngine.Debug.LogError(e); }
            }

            protected override void OnSessionDestroy(ulong xrSession)
            {
                UnityEngine.Debug.Log($"OnSessionDestroy({xrSession:X})");
                m_XrSession = default;
                try { onSessionDestroy?.Invoke(xrSession); }
                catch (Exception e) { UnityEngine.Debug.LogError(e); }
            }

            private bool GetXrFunctionDelegates(ulong xrInstance)
            {
                if (xrGetInstanceProcAddr == null || xrGetInstanceProcAddr == IntPtr.Zero)
                    UnityEngine.Debug.LogError("xrGetInstanceProcAddr is null");
                // Get delegate of xrGetInstanceProcAddr.
                var xrGetProc =  Marshal.GetDelegateForFunctionPointer<xrGetInstanceProcDelegate>(xrGetInstanceProcAddr);
                // Get delegate of other OpenXR functions using xrGetInstanceProcAddr.
                if (!MarshelFunc(xrInstance, xrGetProc, "xrGetSystemProperties", ref m_xrGetSystemProperties)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrCreateFacialTrackerHTC", ref m_xrCreateFacialTrackerHTC)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrDestroyFacialTrackerHTC", ref m_xrDestroyFacialTrackerHTC)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrGetFacialExpressionsHTC", ref m_xrGetFacialExpressionsHTC)) { return false; }
                return true;
            }
            private static bool MarshelFunc<T>(ulong instance, xrGetInstanceProcDelegate instanceProc, string funcName, ref T func, bool verbose = true)
                where T : Delegate
            {
                if (instanceProc(instance, funcName, out var fp) != 0)
                {
                    if (verbose)
                    {
                        UnityEngine.Debug.LogError("Fail getting function " + funcName);
                    }
                    return false;
                }

                func = Marshal.GetDelegateForFunctionPointer<T>(fp);
                return true;
            }
            delegate int XrGetInstanceProcAddrDelegate(ulong instance, string name, out IntPtr function);
            XrGetInstanceProcAddrDelegate m_XrGetInstanceProcAddr;

            delegate int xrGetSystemPropertiesDelegate(ulong instance, ulong systemId, ref XrSystemProperties properties);
            xrGetSystemPropertiesDelegate m_xrGetSystemProperties;
            public int xrGetSystemProperties(ref XrSystemProperties properties) =>
                m_xrGetSystemProperties(m_XrInstance, m_systemid, ref properties);
            public int xrGetSystemProperties(ulong instance, ulong systemId, ref XrSystemProperties properties) =>
                m_xrGetSystemProperties(instance, systemId, ref properties);

            delegate int xrCreateFacialTrackerHTCDelegate(ulong session, XrFacialTrackerCreateInfoHTC createInfo, out ulong expression);
            xrCreateFacialTrackerHTCDelegate m_xrCreateFacialTrackerHTC;
            public int xrCreateFacialTrackerHTC(XrFacialTrackerCreateInfoHTC createInfo ,out ulong handle) =>
                m_xrCreateFacialTrackerHTC(m_XrSession, createInfo, out handle);
            public int xrCreateFacialTrackerHTC(ulong session, XrFacialTrackerCreateInfoHTC createInfo, out ulong handle) =>
                m_xrCreateFacialTrackerHTC(session, createInfo, out handle);

            delegate int xrDestroyFacialTrackerHTCDelegate(ulong facialTracker);
            xrDestroyFacialTrackerHTCDelegate m_xrDestroyFacialTrackerHTC;
            public int xrDestroyFacialTrackerHTC(ulong handle) =>
                m_xrDestroyFacialTrackerHTC(handle);

            delegate int xrGetFacialExpressionsHTCDelegate(ulong facialTracker, ref XrFacialExpressionsHTC eyeExpressionData);
            xrGetFacialExpressionsHTCDelegate m_xrGetFacialExpressionsHTC;
            public int xrGetFacialExpressionsHTC(ulong handle,ref XrFacialExpressionsHTC eyeExpressionData) =>
                m_xrGetFacialExpressionsHTC(handle, ref eyeExpressionData);

            public unsafe void SystemSupportsFacialTracking(out XrResult result,out bool isSupportEye,out bool isSupportLip)
            {
                XrSystemFacialTrackingPropertiesHTC expressionProperties = new XrSystemFacialTrackingPropertiesHTC
                {
                    type = XrStructureType.XR_TYPE_SYSTEM_FACIAL_TRACKING_PROPERTIES_HTC,
                    next = IntPtr.Zero,
                };

                var systemProp = new XrSystemProperties()
                {
                    type = XrStructureType.XR_TYPE_SYSTEM_PROPERTIES,
                    next = (IntPtr)(&expressionProperties),
                };
                result = (XrResult)xrGetSystemProperties(XrInstance, SystemId ,ref systemProp);
                isSupportEye = expressionProperties.supportEyeFacialTracking != 0;
                isSupportLip = expressionProperties.supportLipFacialTracking != 0;                
            }

            public bool TryCreateFacialTracker(XrFacialTrackingTypeHTC type,out ulong handle, out XrResult result)
            {
                XrFacialTrackerCreateInfoHTC createInfo = new XrFacialTrackerCreateInfoHTC(
                    XrStructureType.XR_TYPE_FACIAL_TRACKER_CREATE_INFO_HTC,
                    IntPtr.Zero,
                    type);
                result = (XrResult)xrCreateFacialTrackerHTC(XrSession,createInfo, out handle);
                return result == XrResult.XR_SUCCESS;
            }
            public bool TryDestroFacialTracker(ulong handle, out XrResult result)
            {
                result= (XrResult)xrDestroyFacialTrackerHTC(handle);
                return result == XrResult.XR_SUCCESS;
            }
            public bool TryGetFacialData(ulong handle,out bool isActive,int maxExpressioncount,out XrResult result,float[] blendshapes)
            {
                XrFacialExpressionsHTC facialExpression = new XrFacialExpressionsHTC()
                {
                    type = XrStructureType.XR_TYPE_FACIAL_EXPRESSIONS_HTC,
                    expressionCount = maxExpressioncount,
                    sampleTime = 10L, //An arbitrary number greater than 0
                    blendShapeWeightings = Marshal.AllocCoTaskMem(sizeof(float) * maxExpressioncount),
                };
                result = (XrResult)xrGetFacialExpressionsHTC(handle, ref facialExpression);

                Marshal.Copy(facialExpression.blendShapeWeightings, blendshapes, 0, maxExpressioncount);
                isActive = facialExpression.isActive != 0u;

                return result == XrResult.XR_SUCCESS;
            }
        }
    }
}