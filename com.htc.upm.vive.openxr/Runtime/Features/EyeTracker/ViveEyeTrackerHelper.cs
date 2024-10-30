// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Runtime.InteropServices;

namespace VIVE.OpenXR.EyeTracker
{
    /// <summary>
    /// The XrEyeTrackerHTC handle represents the resources for eye tracking.
    /// </summary>
    public struct XrEyeTrackerHTC : IEquatable<UInt64>
    {
        private readonly UInt64 value;

        public XrEyeTrackerHTC(UInt64 u)
        {
            value = u;
        }

        public static implicit operator UInt64(XrEyeTrackerHTC equatable)
        {
            return equatable.value;
        }
        public static implicit operator XrEyeTrackerHTC(UInt64 u)
        {
            return new XrEyeTrackerHTC(u);
        }

        public bool Equals(XrEyeTrackerHTC other)
        {
            return value == other.value;
        }
        public bool Equals(UInt64 other)
        {
            return value == other;
        }
        public override bool Equals(object obj)
        {
            return obj is XrEyeTrackerHTC && Equals((XrEyeTrackerHTC)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static bool operator ==(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.Equals(b); }
        public static bool operator !=(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return !a.Equals(b); }
        public static bool operator >=(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value >= b.value; }
        public static bool operator <=(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value <= b.value; }
        public static bool operator >(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value > b.value; }
        public static bool operator <(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value < b.value; }
        public static XrEyeTrackerHTC operator +(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value + b.value; }
        public static XrEyeTrackerHTC operator -(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value - b.value; }
        public static XrEyeTrackerHTC operator *(XrEyeTrackerHTC a, XrEyeTrackerHTC b) { return a.value * b.value; }
        public static XrEyeTrackerHTC operator /(XrEyeTrackerHTC a, XrEyeTrackerHTC b)
        {
            if (b.value == 0)
            {
                throw new DivideByZeroException();
            }
            return a.value / b.value;
        }
    }
    /// <summary>
    /// The XrEyePositionHTC describes which eye is under tracking for the data retrieved from <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see>, <see cref="XrEyePupilDataHTC">XrEyePupilDataHTC</see> or <see cref="XrEyeGeometricDataHTC">XrEyeGeometricDataHTC</see>.
    /// </summary>
    public enum XrEyePositionHTC
    {
        /// <summary>
        /// Specifies the position of the left eye.
        /// </summary>
        XR_EYE_POSITION_LEFT_HTC = 0,
        /// <summary>
        /// Specifies the position of the right eye.
        /// </summary>
        XR_EYE_POSITION_RIGHT_HTC = 1,
        XR_EYE_POSITION_COUNT_HTC = 2
    };

    /// <summary>
    /// An application can inspect whether the system is capable of eye tracking input by extending the  <see cref="XrSystemProperties">XrSystemProperties</see> with  <see cref="XrSystemEyeTrackingPropertiesHTC">XrSystemEyeTrackingPropertiesHTC</see> structure when calling <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrGetSystemProperties">xrGetSystemProperties</see>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSystemEyeTrackingPropertiesHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// Indicating if the current system is capable of receiving eye tracking input.
        /// </summary>
        public XrBool32 supportsEyeTracking;
    };

    /// <summary>
    /// The XrEyeTrackerCreateInfoHTC structure describes the information to create an <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> handle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyeTrackerCreateInfoHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;

        /// <param name="in_type">The <see cref="XrStructureType">XrStructureType</see> of this structure.</param>
        /// <param name="in_next">NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.</param>
        public XrEyeTrackerCreateInfoHTC(XrStructureType in_type, IntPtr in_next)
        {
            type = in_type;
            next = in_next;
        }
    };

    /// <summary>
    /// The XrEyeGazeDataInfoHTC structure describes the information to get eye gaze directions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyeGazeDataInfoHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// An <see cref="XrSpace">XrSpace</see> within which the returned eye poses will be represented.
        /// </summary>
        public XrSpace baseSpace;
        /// <summary>
        /// An <see cref="XrTime">XrTime</see> at which the eye gaze information is requested.
        /// </summary>
        public XrTime time;

        /// <param name="in_type">The <see cref="XrStructureType">XrStructureType</see> of this structure.</param>
        /// <param name="in_next">NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.</param>
        /// <param name="in_baseSpace">An <see cref="XrSpace">XrSpace</see> within which the returned eye poses will be represented.</param>
        /// <param name="in_time">An <see cref="XrTime">XrTime</see> at which the eye gaze information is requested.</param>
        public XrEyeGazeDataInfoHTC(XrStructureType in_type, IntPtr in_next, XrSpace in_baseSpace, XrTime in_time)
        {
            type = in_type;
            next = in_next;
            baseSpace = in_baseSpace;
            time = in_time;
        }
    };

    /// <summary>
    /// The XrSingleEyeGazeDataHTC structure describes the validity and direction of a eye gaze observation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSingleEyeGazeDataHTC
    {
        /// <summary>
        /// An <see cref="XrBool32">XrBool32</see> indicating if the returned gazePose is valid. Callers should check the validity of pose prior to use.
        /// </summary>
        public XrBool32 isValid;
        /// <summary>
        /// An <see cref="XrPosef">XrPosef</see> describing the position and orientation of the user's eye. The pose is represented in the coordinate system provided by <see cref="XrEyeGazeDataInfoHTC">XrEyeGazeDataInfoHTC</see>::<see cref="XrEyeGazeDataInfoHTC.baseSpace">baseSpace</see>.
        /// </summary>
        public XrPosef gazePose;

        /// <param name="in_isValid">An <see cref="XrBool32">XrBool32</see> indicating if the returned gazePose is valid. Callers should check the validity of pose prior to use.</param>
        /// <param name="in_gazePose">An <see cref="XrPosef">XrPosef</see> describing the position and orientation of the user's eye. The pose is represented in the coordinate system provided by <see cref="XrEyeGazeDataInfoHTC">XrEyeGazeDataInfoHTC</see>::<see cref="XrEyeGazeDataInfoHTC.baseSpace">baseSpace</see>.</param>
        public XrSingleEyeGazeDataHTC(XrBool32 in_isValid, XrPosef in_gazePose)
        {
            isValid = in_isValid;
            gazePose = in_gazePose;
        }
    };

    /// <summary>
    /// The XrEyeGazeDataHTC structure returns the state of the eye gaze directions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyeGazeDataHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// An <see cref="XrTime">XrTime</see> at which the eye gaze information is requested.
        /// </summary>
        public XrTime time;
        /// <summary>
        /// An array of <see cref="XrSingleEyeGazeDataHTC">XrSingleEyeGazeDataHTC</see> receiving the returned eye gaze directions.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public XrSingleEyeGazeDataHTC[] gaze;
    };

    /// <summary>
    /// The XrEyePupilDataInfoHTC structure describes the information to get pupil data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyePupilDataInfoHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <param name="in_type">The <see cref="XrStructureType">XrStructureType</see> of this structure.</param>
        /// <param name="in_next">NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.</param>
        public XrEyePupilDataInfoHTC(XrStructureType in_type, IntPtr in_next)
        {
            type = in_type;
            next = in_next;
        }
    };

    /// <summary>
    /// The XrSingleEyePupilDataHTC structure describes the validity, diameter and position of a pupil observation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSingleEyePupilDataHTC
    {
        /// <summary>
        /// An <see cref="XrBool32">XrBool32</see> indicating if the returned pupilDiameter is valid. Callers should check the validity of diameter prior to use.
        /// </summary>
        public XrBool32 isDiameterValid;
        /// <summary>
        /// An <see cref="XrBool32">XrBool32</see> indicating if the returned pupilPosition is valid. Callers should check the validity of position prior to use.
        /// </summary>
        public XrBool32 isPositionValid;
        /// <summary>
        /// The diameter of pupil in millimeters.
        /// </summary>
        public float pupilDiameter;
        /// <summary>
        /// The position of pupil in sensor area which x and y are normalized in [0,1] with +Y up and +X to the right.
        /// </summary>
        public XrVector2f pupilPosition;

        /// <param name="in_isDiameterValid">An <see cref="XrBool32">XrBool32</see> indicating if the returned gazePose is valid. Callers should check the validity of pose prior to use.</param>
        /// <param name="in_isPositionValid">An <see cref="XrBool32">XrBool32</see> indicating if the returned pupilPosition is valid. Callers should check the validity of position prior to use.</param>
        /// <param name="in_pupilDiameter">The diameter of pupil in millimeters.</param>
        /// <param name="in_pupilPosition">The position of pupil in sensor area which x and y are normalized in [0,1]with +Y up and +X to the right.</param>
        public XrSingleEyePupilDataHTC(XrBool32 in_isDiameterValid, XrBool32 in_isPositionValid, float in_pupilDiameter, XrVector2f in_pupilPosition)
        {
            isDiameterValid = in_isDiameterValid;
            isPositionValid = in_isPositionValid;
            pupilDiameter  = in_pupilDiameter;
            pupilPosition  = in_pupilPosition;
        }
    };

    /// <summary>
    /// The XrEyePupilDataHTC structure returns the pupil data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyePupilDataHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// An <see cref="XrTime">XrTime</see> at which the pupil data was captured.
        /// </summary>
        public XrTime time;
        /// <summary>
        /// An array of <see cref="XrSingleEyePupilDataHTC">XrSingleEyePupilDataHTC</see> receiving the returned pupil data.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public XrSingleEyePupilDataHTC[] pupilData;
    };

    /// <summary>
    /// The XrEyeGeometricDataInfoHTC structure describes the information to get geometric related data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyeGeometricDataInfoHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <param name="in_type">The <see cref="XrStructureType">XrStructureType</see> of this structure.</param>
        /// <param name="in_next">NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.</param>
        public XrEyeGeometricDataInfoHTC(XrStructureType in_type, IntPtr in_next)
        {
            type = in_type;
            next = in_next;
        }
    };

    /// <summary>
    /// The XrSingleEyeGeometricDataHTC structure describes the geometric related data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSingleEyeGeometricDataHTC
    {
        /// <summary>
        /// A flag that indicates if the geometric data is valid. Callers should check the validity of the geometric data prior to use.
        /// </summary>
        public XrBool32 isValid;
        /// <summary>
        /// A value in range [0,1] representing the openness of the user's eye. When this value is zero, the eye closes normally. When this value is one, the eye opens normally. When this value goes higher, the eye approaches open.
        /// </summary>
        public float eyeOpenness;
        /// <summary>
        /// A value in range [0,1] representing how the user's eye open widely. When this value is zero, the eye opens normally. When this value goes higher, the eye opens wider.
        public float eyeWide;
        /// <summary>
        /// A value in range [0,1] representing how the user's eye is closed. When this value is zero, the eye closes normally. When this value goes higher, the eye closes tighter.
        /// </summary>
        public float eyeSqueeze;

        /// <param name="in_isValid">A flag that indicates if the geometric data is valid. Callers should check the validity of the geometric data prior to use.</param>
        /// <param name="in_eyeOpenness">A value in range [0,1] representing the openness of the user's eye. When this value is zero, the eye closes normally. When this value is one, the eye opens normally. When this value goes higher, the eye approaches open.</param>
        /// <param name="in_eyeWide">A value in range [0,1] representing how the user's eye open widely. When this value is zero, the eye opens normally. When this value goes higher, the eye opens wider.</param>
        /// <param name="in_eyeSqueeze">A value in range [0,1] representing how the user's eye is closed. When this value is zero, the eye closes normally. When this value goes higher, the eye closes tighter.</param>
        public XrSingleEyeGeometricDataHTC(XrBool32 in_isValid, float in_eyeOpenness, float in_eyeWide, float in_eyeSqueeze)
        {
            isValid = in_isValid;
            eyeOpenness = in_eyeOpenness;
            eyeWide = in_eyeWide;
            eyeSqueeze = in_eyeSqueeze;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct XrEyeGeometricDataHTC
    {
        /// <summary>
        /// The <see cref="XrStructureType">XrStructureType</see> of this structure.
        /// </summary>
        public XrStructureType type;
        /// <summary>
        /// NULL or a pointer to the next structure in a structure chain. No such structures are defined in core OpenXR or this extension.
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// An <see cref="XrTime">XrTime</see> at which the returned eye data is tracked.
        /// </summary>
        public XrTime time;
        /// <summary>
        /// An array of <see cref="XrSingleEyeGeometricDataHTC">XrSingleEyeGeometricDataHTC</see> receiving the returned eye geometric data.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public XrSingleEyeGeometricDataHTC[] eyeGeometricData;
    };


    public static class ViveEyeTrackerHelper
    {
        /// <param name="session">An XrSession in which the eye tracker will be active.</param>
        /// <param name="createInfo">The <see cref="XrEyeTrackerCreateInfoHTC">XrEyeTrackerCreateInfoHTC</see> used to specify the eye tracker.</param>
        /// <param name="eyeTracker">The returned <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> handle.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public delegate XrResult xrCreateEyeTrackerHTCDelegate(
            XrSession session,
            ref XrEyeTrackerCreateInfoHTC createInfo,
            out XrEyeTrackerHTC eyeTracker);

        /// <param name="eyeTracker">An XrEyeTrackerHTC previously created by xrCreateEyeTrackerHTC.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public delegate XrResult xrDestroyEyeTrackerHTCDelegate(
            XrEyeTrackerHTC eyeTracker);

        /// <summary>
        /// Retrieves the <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see> data of a <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see>.
        /// </summary>
        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="gazeInfo">The information to get eye gaze.</param>
        /// <param name="eyeGazes">A pointer to <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see> receiving the returned eye poses.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public delegate XrResult xrGetEyeGazeDataHTCDelegate(
            XrEyeTrackerHTC eyeTracker,
            ref XrEyeGazeDataInfoHTC gazeInfo,
            ref XrEyeGazeDataHTC eyeGazes);

        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="pupilDataInfo">The information to get pupil data.</param>
        /// <param name="pupilData">A pointer to <see cref="XrEyePupilDataHTC">XrEyePupilDataHTC</see> returned by the runtime.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public delegate XrResult xrGetEyePupilDataHTCDelegate(
            XrEyeTrackerHTC eyeTracker,
            ref XrEyePupilDataInfoHTC pupilDataInfo,
            ref XrEyePupilDataHTC pupilData);

        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="info">A pointer to <see cref="XrEyeGeometricDataInfoHTC">XrEyeGeometricDataInfoHTC</see> structure.</param>
        /// <param name="eyeGeometricData">A pointer to <see cref="XrEyeGeometricDataHTC">XrEyeGeometricDataHTC</see> returned by the runtime.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public delegate XrResult xrGetEyeGeometricDataHTC(
            XrEyeTrackerHTC eyeTracker,
            ref XrEyeGeometricDataInfoHTC info,
            ref XrEyeGeometricDataHTC eyeGeometricData);
    }
}
