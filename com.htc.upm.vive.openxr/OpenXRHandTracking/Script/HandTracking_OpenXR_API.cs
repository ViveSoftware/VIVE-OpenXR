using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.XR.OpenXR.Features;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace VIVE
{
    namespace HandTracking
    {

#if UNITY_EDITOR
        [OpenXRFeature(UiName = "Hand Tracking",
            BuildTargetGroups = new[] { BuildTargetGroup.Standalone },
            Company = "HTC",
            Desc = "Hand Tracking OpenXR Feature",
            DocumentationLink = "https://developer.vive.com/resources/openxr/openxr-pcvr/tutorials/unity/how-integrate-hand-tracking-data-your-hand-model/",
            OpenxrExtensionStrings = "XR_EXT_hand_tracking",
            Version = "0.0.1",
            FeatureId = featureId)]
#endif
        public class HandTracking_OpenXR_API : OpenXRFeature
        {
            /// <summary>
            /// The feature id string. This is used to give the feature a well known id for reference.
            /// </summary>
            public const string featureId = "com.company.openxr.handtracking.feature";
            private IntPtr oldxrGetInstanceProcAddr;
            private IntPtr oldWaitFrame;
            private ulong m_xrInstance;
            private ulong m_xrSession;
            private ulong m_systemId;
            private XrFrameWaitInfo m_frameWaitInfo;
            private XrFrameState m_frameState;

            [Obsolete] public ulong m_space;
            [Obsolete] public XrSystemProperties systemProperties;
            [Obsolete] public ulong m_leftHandle;
            [Obsolete] public ulong m_rightHandle;

            public bool IsInitialized { get { return m_xrGetSystemProperties != null; } }
            public bool IsEnabledAndInitialized { get { return enabled && IsInitialized; } }
            public bool IsSessionCreated { get { return XrSession != default; } }
            public ulong XrInstance { get { return m_xrInstance; } }
            public ulong XrSession { get { return m_xrSession; } }
            public ulong SystemId { get { return m_systemId; } }

            public event Action<ulong> onSessionCreate;
            public event Action<ulong> onSessionDestroy;
            public event Action<ulong> onSystemChange;
            protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
            {
                UnityEngine.Debug.Log("EXT: registering our own xrGetInstanceProcAddr");
                oldxrGetInstanceProcAddr = func;
                m_intercept_xrWaitFrame_xrGetInstanceProcAddr = intercept_xrWaitFrame_xrGetInstanceProcAddr;
                return Marshal.GetFunctionPointerForDelegate(m_intercept_xrWaitFrame_xrGetInstanceProcAddr);
            }


            private int intercept_xrWaitFrame_xrGetInstanceProcAddr(ulong instance, string name, out IntPtr function)
            {
                if (oldxrGetInstanceProcAddr == null || oldxrGetInstanceProcAddr == IntPtr.Zero)
                {
                    UnityEngine.Debug.LogError("oldxrGetInstanceProcAddr is null");
                    function = IntPtr.Zero;
                    return -1;
                }

                // Get delegate of old xrGetInstanceProcAddr.
                var xrGetProc = Marshal.GetDelegateForFunctionPointer<xrGetInstanceProcDelegate>(oldxrGetInstanceProcAddr);

                int result = xrGetProc(instance, name,out function);
                if(name == "xrWaitFrame")
                {
                    oldWaitFrame = function;
                    m_intercept_xrWaitFrame = intercepted_xrWaitFrame;
                    function = Marshal.GetFunctionPointerForDelegate(m_intercept_xrWaitFrame); ;
                    UnityEngine.Debug.Log("Getting xrWaitFrame func");
                }
                return result;

            }

            private int intercepted_xrWaitFrame(ulong session,ref XrFrameWaitInfo frameWaitInfo, ref XrFrameState frameState)
            {
                // Get delegate of old xrWaitFrame.
                var xrWaitFrame = Marshal.GetDelegateForFunctionPointer<xrWaitFrameDelegate>(oldWaitFrame);
                int res = xrWaitFrame(session, ref frameWaitInfo, ref  frameState);
                m_frameWaitInfo = frameWaitInfo;
                m_frameState = frameState;
                return res;
            }

            /// <inheritdoc />
            protected override bool OnInstanceCreate(ulong xrInstance)
            {
                UnityEngine.Debug.Log($"OnInstanceCreate({xrInstance:X})");
                m_xrInstance = xrInstance;
                return GetXrFunctionDelegates(xrInstance);
            }

            /// <inheritdoc />
            protected override void OnInstanceDestroy(ulong xrInstance)
            {
                UnityEngine.Debug.Log($"OnInstanceDestroy({xrInstance:X})");
                m_xrInstance = default;
                //ClearXrFunctionDelegates();
            }

            /// <inheritdoc />
            protected override void OnSessionCreate(ulong xrSession)
            {
                UnityEngine.Debug.Log($"OnSessionCreate({xrSession:X})");
                m_xrSession = xrSession;

                try { onSessionCreate?.Invoke(xrSession); }
                catch (Exception e) { UnityEngine.Debug.LogError(e); }
            }

            /// <inheritdoc />
            protected override void OnSystemChange(ulong xrSystem)
            {
                UnityEngine.Debug.Log($"OnSystemChange({xrSystem:X})");
                m_systemId = xrSystem;

                try { onSystemChange?.Invoke(xrSystem); }
                catch (Exception e) { UnityEngine.Debug.LogError(e); }
            }

            /// <inheritdoc />
            protected override void OnSessionDestroy(ulong xrSession)
            {
                UnityEngine.Debug.Log($"OnSessionDestroy({xrSession:X})");
                m_xrSession = default;

                try { onSessionDestroy?.Invoke(xrSession); }
                catch (Exception e) { UnityEngine.Debug.LogError(e); }
            }

            /// <summary>Return true if the result equals zero. </summary>
            private bool GetXrFunctionDelegates(ulong xrInstance)
            {
                if (xrGetInstanceProcAddr == null || xrGetInstanceProcAddr == IntPtr.Zero)
                {
                    UnityEngine.Debug.LogError("xrGetInstanceProcAddr is null");
                    return false;
                }

                // Get delegate of xrGetInstanceProcAddr.
                var xrGetProc = Marshal.GetDelegateForFunctionPointer<xrGetInstanceProcDelegate>(xrGetInstanceProcAddr);

                if (!MarshelFunc(xrInstance, xrGetProc, "xrGetSystemProperties", ref m_xrGetSystemProperties)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrEnumerateReferenceSpaces", ref m_xrEnumerateReferenceSpaces)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrEnumerateReferenceSpaces", ref m_xrEnumerateReferenceSpaces2)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrCreateReferenceSpace", ref m_xrCreateReferenceSpace)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrDestroySpace", ref m_xrDestroySpace)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrCreateHandTrackerEXT", ref m_xrCreateHandTrackerEXT)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrDestroyHandTrackerEXT", ref m_xrDestroyHandTrackerEXT)) { return false; }
                if (!MarshelFunc(xrInstance, xrGetProc, "xrLocateHandJointsEXT", ref m_xrLocateHandJointsEXT)) { return false; }
                return true;
            }

            private void ClearXrFunctionDelegates()
            {
                m_xrGetSystemProperties = null;
                m_xrEnumerateReferenceSpaces = null;
                m_xrEnumerateReferenceSpaces2 = null;
                m_xrCreateReferenceSpace = null;
                m_xrDestroySpace = null;
                m_xrCreateHandTrackerEXT = null;
                m_xrDestroyHandTrackerEXT = null;
                m_xrLocateHandJointsEXT = null;
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

            private xrGetInstanceProcDelegate m_intercept_xrWaitFrame_xrGetInstanceProcAddr;

            private delegate int xrGetSystemPropertiesDelegate(ulong instance, ulong systemId, ref XrSystemProperties properties);
            private xrGetSystemPropertiesDelegate m_xrGetSystemProperties;
            public int xrGetSystemProperties(ref XrSystemProperties properties) =>
                m_xrGetSystemProperties(XrInstance, SystemId, ref properties);
            public int xrGetSystemProperties(ulong instance, ulong systemId, ref XrSystemProperties properties) =>
                m_xrGetSystemProperties(instance, systemId, ref properties);

            private delegate int xrWaitFrameDelegate(ulong session,ref XrFrameWaitInfo frameWaitInfo, ref XrFrameState frameState);
            private xrWaitFrameDelegate m_intercept_xrWaitFrame;
            #region space
            private delegate int xrEnumerateReferenceSpacesDelegate(ulong session, int spaceCapacityInput, out int spaceCountOutput, out XrReferenceSpaceType spaces);
            private xrEnumerateReferenceSpacesDelegate m_xrEnumerateReferenceSpaces;
            public int xrEnumerateReferenceSpaces(int spaceCapacityInput, out int spaceCountOutput, out XrReferenceSpaceType spaces) =>
                m_xrEnumerateReferenceSpaces(XrSession, spaceCapacityInput, out spaceCountOutput, out spaces);

            private delegate int xrEnumerateReferenceSpacesDelegate2(ulong session, int spaceCapacityInput, out int spaceCountOutput, IntPtr spaces);
            private xrEnumerateReferenceSpacesDelegate2 m_xrEnumerateReferenceSpaces2;
            public int xrEnumerateReferenceSpaces(ulong session, int spaceCapacityInput, out int spaceCountOutput, IntPtr spaces) =>
                m_xrEnumerateReferenceSpaces2(session, spaceCapacityInput, out spaceCountOutput, spaces);

            private delegate int xrCreateReferenceSpaceDelegate(ulong session, ref XrReferenceSpaceCreateInfo createInfo, out ulong space);
            private xrCreateReferenceSpaceDelegate m_xrCreateReferenceSpace;
            public int xrCreateReferenceSpace(ref XrReferenceSpaceCreateInfo createInfo, out ulong space) =>
                m_xrCreateReferenceSpace(XrSession, ref createInfo, out space);
            public int xrCreateReferenceSpace(ulong session, ref XrReferenceSpaceCreateInfo createInfo, out ulong space) =>
                m_xrCreateReferenceSpace(session, ref createInfo, out space);

            private delegate int xrDestroySpaceDelegate(ulong space);
            private xrDestroySpaceDelegate m_xrDestroySpace;
            public int xrDestroySpace(ulong space) =>
                m_xrDestroySpace(space);
            #endregion

            private delegate int xrCreateHandTrackerEXTDelegate(ulong session, XrHandTrackerCreateInfoEXT createInfo, out ulong handTracker);
            private xrCreateHandTrackerEXTDelegate m_xrCreateHandTrackerEXT;
            public int xrCreateHandTrackerEXT(XrHandTrackerCreateInfoEXT createInfo, out ulong handle) =>
                m_xrCreateHandTrackerEXT(XrSession, createInfo, out handle);
            public int xrCreateHandTrackerEXT(ulong session, XrHandTrackerCreateInfoEXT createInfo, out ulong handle) =>
                m_xrCreateHandTrackerEXT(session, createInfo, out handle);

            private delegate int xrDestroyHandTrackerEXTDelegate(ulong handTracker);
            private xrDestroyHandTrackerEXTDelegate m_xrDestroyHandTrackerEXT;
            public int xrDestroyHandTrackerEXT(ulong handle) =>
                m_xrDestroyHandTrackerEXT(handle);

            private delegate int xrLocateHandJointsEXTDelegate(ulong handTracker,ref XrHandJointsLocateInfoEXT locateInfo, ref XrHandJointLocationsEXT locations);
            private xrLocateHandJointsEXTDelegate m_xrLocateHandJointsEXT;
            public int xrLocateHandJointsEXT(ulong handTracker,ref XrHandJointsLocateInfoEXT locateInfo, ref XrHandJointLocationsEXT locations) =>
                m_xrLocateHandJointsEXT(handTracker,ref locateInfo, ref locations);

            public unsafe bool SystemSupportsHandTracking(out XrResult result)
            {
                var handTrackingSystemProp = new XrSystemHandTrackingPropertiesEXT()
                {
                    type = XrStructureType.XR_TYPE_SYSTEM_HAND_TRACKING_PROPERTIES_EXT,
                    next = IntPtr.Zero,
                };

                var systemProp = new XrSystemProperties()
                {
                    type = XrStructureType.XR_TYPE_SYSTEM_PROPERTIES,
                    next = (IntPtr)(&handTrackingSystemProp),
                };

                result = (XrResult)xrGetSystemProperties(XrInstance, SystemId, ref systemProp);
                return result == XrResult.XR_SUCCESS && handTrackingSystemProp.supportsHandTracking != 0u;
            }

            public bool TryGetSupportedReferenceSpaceTypeCount(out int count, out XrResult result)
            {
                result = (XrResult)m_xrEnumerateReferenceSpaces2(XrSession, 0, out count, IntPtr.Zero);
                return result == XrResult.XR_SUCCESS;
            }

            public bool TryGetSupportedReferenceSpaceTypes(XrReferenceSpaceType[] spaces, out XrResult result)
            {
                if (spaces == null || spaces.Length == 0)
                {
                    result = default;
                    return true;
                }

                result = (XrResult)m_xrEnumerateReferenceSpaces2(XrSession, spaces.Length, out _, ArrayPtr(spaces));
                return result == XrResult.XR_SUCCESS;
            }

            public bool TryGetSupportedReferenceSpaceType(XrReferenceSpaceType preferType, out XrReferenceSpaceType supportedType, out XrResult result)
            {
                supportedType = default;
                if (!TryGetSupportedReferenceSpaceTypeCount(out var count, out result)) { return false; }
                if (count == 0) { return false; } // FIXME: error code?

                var spaces = new XrReferenceSpaceType[count];
                if (!TryGetSupportedReferenceSpaceTypes(spaces, out result)) { return false; }

                for (int i = 0; i < count; ++i)
                {
                    supportedType = spaces[i];
                    if (supportedType == preferType) { break; }
                }

                return true;
            }

            public bool TryCreateReferenceSpace(XrReferenceSpaceType refSpaceType, XrVector3f position, XrQuaternionf orientation, out ulong handle, out XrResult result)
            {
                var createInfo = new XrReferenceSpaceCreateInfo()
                {
                    type = XrStructureType.XR_TYPE_REFERENCE_SPACE_CREATE_INFO,
                    next = IntPtr.Zero,
                    referencespacetype = refSpaceType,
                    poseInReferenceSpace = new XrPosef()
                    {
                        position = position,
                        orientation = orientation,
                    },
                };

                result = (XrResult)xrCreateReferenceSpace(XrSession, ref createInfo, out handle);
                return result == XrResult.XR_SUCCESS;
            }

            public bool TryDestroyReferenceSpace(ulong handle, out XrResult result)
            {
                result = (XrResult)xrDestroySpace(handle);
                return result == XrResult.XR_SUCCESS;
            }

            public bool TryCreateHandTracker(XrHandEXT hand, out ulong handle, out XrResult result, XrHandJointSetEXT jointSet = XrHandJointSetEXT.XR_HAND_JOINT_SET_DEFAULT_EXT)
            {
                var createInfo = new XrHandTrackerCreateInfoEXT()
                {
                    type = XrStructureType.XR_TYPE_HAND_TRACKER_CREATE_INFO_EXT,
                    next = IntPtr.Zero,
                    hand = hand,
                    handJointSet = XrHandJointSetEXT.XR_HAND_JOINT_SET_DEFAULT_EXT,
                };

                result = (XrResult)xrCreateHandTrackerEXT(XrSession, createInfo, out handle);
                return result == XrResult.XR_SUCCESS;
            }

            public bool TryDestroyHandTracker(ulong handle, out XrResult result)
            {
                result = (XrResult)xrDestroyHandTrackerEXT(handle);
                return result == XrResult.XR_SUCCESS;
            }

            public bool TryLocateHandJoints(ulong handle, ulong space, out bool isActive, XrHandJointLocationEXT[] locationArray, out XrResult result)
            {
                return TryLocateHandJoints(handle, space, out isActive, locationArray, null, out result);
            }

            public bool TryLocateHandJoints(ulong handle, ulong space, out bool isActive, XrHandJointLocationEXT[] locationArray, XrHandJointVelocityEXT[] velocityArray, out XrResult result)
            {
                return TryLocateHandJoints(handle, space, IntPtr.Zero, out isActive, locationArray, velocityArray, out result);
            }

            public bool TryLocateHandJoints(ulong handle, ulong space, ref XrHandJointsMotionRangeEXT motionRange, out bool isActive, XrHandJointLocationEXT[] locationArray, out XrResult result)
            {
                return TryLocateHandJoints(handle, space, ref motionRange, out isActive, locationArray, null, out result);
            }

            public unsafe bool TryLocateHandJoints(ulong handle, ulong space, ref XrHandJointsMotionRangeEXT motionRange, out bool isActive, XrHandJointLocationEXT[] locationArray, XrHandJointVelocityEXT[] velocityArray, out XrResult result)
            {
                var motionRangeInfo = new XrHandJointsMotionRangeInfoEXT()
                {
                    type = XrStructureType.XR_TYPE_HAND_JOINTS_MOTION_RANGE_INFO_EXT,
                    next = IntPtr.Zero,
                    handJointsMotionRange = motionRange,
                };
                
                bool res = TryLocateHandJoints(handle, space, (IntPtr)(&motionRangeInfo), out isActive, locationArray, velocityArray, out result);
                motionRange = motionRangeInfo.handJointsMotionRange;
                return res;
            }

            private unsafe bool TryLocateHandJoints(ulong handle, ulong space, IntPtr motionRangeInfo, out bool isActive, XrHandJointLocationEXT[] locationArray, XrHandJointVelocityEXT[] velocityArray, out XrResult result)
            {
                var locateInfo = new XrHandJointsLocateInfoEXT()
                {
                    type = XrStructureType.XR_TYPE_HAND_JOINTS_LOCATE_INFO_EXT,
                    next = motionRangeInfo,
                    baseSpace = space,
                    time = m_frameState.predictedDisplayTime, //An arbitrary number greater than 0
                };

                var locVelocitiesPtr = IntPtr.Zero;
                if (velocityArray != null)
                {
                    var locVelocities = new XrHandJointVelocitiesEXT()
                    {
                        type = XrStructureType.XR_TYPE_HAND_JOINT_VELOCITIES_EXT,
                        next = IntPtr.Zero,
                        jointCount = (uint)velocityArray.Length,
                        jointVelocities = ArrayPtr(velocityArray)
                    };

                    locVelocitiesPtr = (IntPtr)(&locVelocities);
                }

                var locations = new XrHandJointLocationsEXT()
                {
                    type = XrStructureType.XR_TYPE_HAND_JOINT_LOCATIONS_EXT,
                    next = locVelocitiesPtr,
                    jointCount = locationArray == null ? 0 : locationArray.Length,
                    jointLocations = ArrayPtr(locationArray),
                };

                result = (XrResult)xrLocateHandJointsEXT(handle,ref locateInfo, ref locations);
                isActive = locations.isActive != 0u;
                return result == XrResult.XR_SUCCESS;
            }

            private unsafe static IntPtr ArrayPtr(XrReferenceSpaceType[] array)
            {
                if (array == null) { return IntPtr.Zero; }
                fixed (XrReferenceSpaceType* p = array) { return (IntPtr)p; }
            }

            private unsafe static IntPtr ArrayPtr(XrHandJointLocationEXT[] array)
            {
                if (array == null) { return IntPtr.Zero; }
                fixed (XrHandJointLocationEXT* p = array) { return (IntPtr)p; }
            }

            private unsafe static IntPtr ArrayPtr(XrHandJointVelocityEXT[] array)
            {
                if (array == null) { return IntPtr.Zero; }
                fixed (XrHandJointVelocityEXT* p = array) { return (IntPtr)p; }
            }

        }
    }
}