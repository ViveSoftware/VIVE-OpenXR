using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using VIVE.HandTracking;
[Obsolete("Use HandManager instead")]
public static class FrameWork
{
    private class HandData
    {
        public bool isStarted;
        public XrHandEXT hand;
        public ulong trackerHandle;
        public bool isActive;
        public int jointUpdatedFrame = -1;
        public XrHandJointLocationEXT[] joints = new XrHandJointLocationEXT[(int)XrHandJointEXT.XR_HAND_JOINT_MAX_ENUM_EXT];
        public XrHandJointVelocityEXT[] velocities = new XrHandJointVelocityEXT[(int)XrHandJointEXT.XR_HAND_JOINT_MAX_ENUM_EXT];

        public bool isCreated { get { return trackerHandle != default; } }

        public void ClearJoints()
        {
            Array.Clear(joints, 0, joints.Length);
            Array.Clear(velocities, 0, velocities.Length);
        }
    }

    private static HandTracking_OpenXR_API feature;
    private static bool isInitialized;
    private static bool isSystemSupported;
    private static ulong refSpace;
    private static HandData leftHandData = new HandData() { hand = XrHandEXT.XR_HAND_LEFT_EXT };
    private static HandData rightHandData = new HandData() { hand = XrHandEXT.XR_HAND_RIGHT_EXT };

    private static bool isRefSpaceCreated { get { return refSpace != default; } }

    private static HandData GetHandData(bool isLeft)
    {
        return isLeft ? leftHandData : rightHandData;
    }

    public static bool Initialize()
    {
        if (!isInitialized)
        {
            if (feature == null)
            {
                feature = OpenXRSettings.Instance.GetFeature<HandTracking_OpenXR_API>();

                if (feature != null)
                {
                    // FIXME: Assume featuer instance won't be destroied and created to a new one
                    feature.onSessionCreate += OnFeatureSessionCreate;
                    feature.onSessionDestroy += OnFeatureSessionDestroy;
                    feature.onSystemChange += OnFeatureSystemChange;
                }
            }

            if (feature != null && feature.IsEnabledAndInitialized)
            {
                UpdateSystemSupported();
                isInitialized = true;
            }
        }

        return isInitialized;
    }

    private static void OnFeatureSessionCreate(ulong xrSession)
    {
        TryCreateHandTracker(leftHandData);
        TryCreateHandTracker(rightHandData);
    }

    private static void OnFeatureSessionDestroy(ulong xrSession)
    {
        DestroyHandTracker(leftHandData);
        DestroyHandTracker(rightHandData);
        DestroyRefSpace();
    }

    private static void OnFeatureSystemChange(ulong systemId)
    {
        UpdateSystemSupported();
    }

    private static void UpdateSystemSupported()
    {
        if (feature == null || !feature.IsEnabledAndInitialized)
        {
            isSystemSupported = false;
        }
        else
        {
            isSystemSupported = feature.SystemSupportsHandTracking(out var result);

            if (!isSystemSupported)
            {
                if (result != XrResult.XR_SUCCESS)
                {
                    Debug.LogWarning("Fail SystemSupportsHandTracking: " + result);
                }
                else
                {
                    Debug.LogWarning("Hand tracking not supported by the system");
                }
            }
        }
    }

    private static bool TryCreateHandTracker(HandData handData)
    {
        if (!handData.isStarted) { return false; }
        if (!Initialize()) { return false; }
        if (!isSystemSupported) { return false; }
        if (!feature.IsSessionCreated) { return false; }

        if (!handData.isCreated)
        {
            if (!feature.TryCreateHandTracker(handData.hand, out handData.trackerHandle, out var result))
            {
                handData.trackerHandle = default;
                Debug.LogWarning("Fail CreateHandTracker: " + result);
            }
        }

        return handData.isCreated;
    }

    private static void DestroyHandTracker(HandData handData)
    {
        if (!handData.isCreated) { return; }
        if (!Initialize()) { return; }

        feature.TryDestroyHandTracker(handData.trackerHandle, out _);
        handData.trackerHandle = default;
        handData.ClearJoints();
    }

    private static bool InitializeRefSpace()
    {
        if (!Initialize()) { return false; }
        if (!isSystemSupported) { return false; }
        if (!feature.IsSessionCreated) { return false; }

        if (!isRefSpaceCreated)
        {
            const XrReferenceSpaceType preferSpaceType = XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_STAGE;
            XrReferenceSpaceType spaceType;
            if (!feature.TryGetSupportedReferenceSpaceType(preferSpaceType, out spaceType, out var result))
            {
                Debug.LogWarning("Fail GetSupportedReferenceSpaceType: " + result);
            }
            else
            {
                if (!feature.TryCreateReferenceSpace(
                     spaceType,
                     new XrVector3f(0f, 0f, 0f),
                     new XrQuaternionf(0f, 0f, 0f, 1f),
                     out refSpace,
                     out result))
                {
                    refSpace = default;
                    Debug.LogWarning("Fail CreateReferenceSpace: " + result);
                }
            }
        }

        return isRefSpaceCreated;
    }

    private static void DestroyRefSpace()
    {
        if (!isRefSpaceCreated) { return; }

        if (Initialize())
        {
            if (!feature.TryDestroyReferenceSpace(refSpace, out var result))
            {
                Debug.LogWarning("Fail DestroyReferenceSpace: " + result);
            }
        }

        refSpace = default;
    }

    public static void StartFrameWork(bool isLeft)
    {
        var handData = GetHandData(isLeft);
        handData.isStarted = true;

        TryCreateHandTracker(handData);
    }

    public static void StopFrameWork(bool isLeft)
    {
        var handData = GetHandData(isLeft);
        handData.isStarted = false;

        DestroyHandTracker(handData);
    }

    public static bool GetJointLocation(bool isleft, out XrHandJointLocationEXT[] joints, bool forceUpdate = false)
    {
        var handData = GetHandData(isleft);
        if (handData.isCreated)
        {
            if (forceUpdate || handData.jointUpdatedFrame != Time.frameCount)
            {
                handData.jointUpdatedFrame = Time.frameCount;

                if (InitializeRefSpace())
                {
                    if (!feature.TryLocateHandJoints(
                        handData.trackerHandle,
                        refSpace,
                        out handData.isActive,
                        handData.joints,
                        out var result))
                    {
                        handData.isActive = false;
                        Debug.LogWarning("Fail LocateHandJoints: " + result);
                    }
                }
            }
        }

        joints = handData.joints;
        return handData.isActive;
    }

    public static bool GetJointLocation(bool isleft, out XrHandJointLocationEXT[] joints,ref XrHandJointsMotionRangeEXT type, bool forceUpdate = false)
    {
        var handData = GetHandData(isleft);
        if (handData.isCreated)
        {
            if (forceUpdate || handData.jointUpdatedFrame != Time.frameCount)
            {
                handData.jointUpdatedFrame = Time.frameCount;

                if (InitializeRefSpace())
                {
                    if (!feature.TryLocateHandJoints(
                        handData.trackerHandle,
                        refSpace,
                        ref type,
                        out handData.isActive,
                        handData.joints,
                        out var result))
                    {
                        handData.isActive = false;
                        Debug.LogWarning("Fail LocateHandJoints: " + result);
                    }
                }
            }
        }
        joints = handData.joints;
        return handData.isActive;
    }

    public static bool GetJointLocation(bool isleft, out XrHandJointLocationEXT[] joints, out XrHandJointVelocityEXT[] velocities, bool forceUpdate = false)
    {
        var handData = GetHandData(isleft);
        if (handData.isCreated)
        {
            if (forceUpdate || handData.jointUpdatedFrame != Time.frameCount)
            {
                handData.jointUpdatedFrame = Time.frameCount;

                if (InitializeRefSpace())
                {
                    if (!feature.TryLocateHandJoints(
                        handData.trackerHandle,
                        refSpace,
                        out handData.isActive,
                        handData.joints,
                        handData.velocities,
                        out var result))
                    {
                        handData.isActive = false;
                        Debug.LogWarning("Fail LocateHandJoints: " + result);
                    }
                }
            }
        }

        joints = handData.joints;
        velocities = handData.velocities;
        return handData.isActive;
    }
}
