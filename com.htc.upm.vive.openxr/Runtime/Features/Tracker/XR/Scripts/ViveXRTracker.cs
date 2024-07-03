// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Input;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
#endif

#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
using PoseControl = UnityEngine.InputSystem.XR.PoseControl;
#else
using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;
#endif

namespace VIVE.OpenXR.Tracker
{
    /// <summary>
    /// This <see cref="OpenXRInteractionFeature"/> enables the use of tracker interaction profiles in OpenXR. It enables XR_HTC_vive_xr_tracker_interaction in the underyling runtime.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "VIVE XR Tracker (Beta)",
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        Company = "HTC",
        Desc = "Support for enabling the vive xr tracker interaction profile. Will register the controller map for xr tracker if enabled.",
        DocumentationLink = "..\\Documentation",
        Version = "1.0.6",
        OpenxrExtensionStrings = kOpenxrExtensionString,
        Category = FeatureCategory.Interaction,
        FeatureId = featureId)]
#endif
    public class ViveXRTracker : OpenXRInteractionFeature
    {
        const string LOG_TAG = "VIVE.OpenXR.Tracker.ViveXRTracker ";
        static StringBuilder m_sb = null;
        static StringBuilder sb {
            get {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
        static void DEBUG(StringBuilder msg) { Debug.Log(msg); }
        static void WARNING(StringBuilder msg) { Debug.LogWarning(msg); }
        static void ERROR(StringBuilder msg) { Debug.LogError(msg); }

        private static ViveXRTracker m_Instance = null;

        /// <summary>
        /// OpenXR specification.
        /// </summary>
        public const string kOpenxrExtensionString = "XR_HTC_vive_xr_tracker_interaction";

        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "vive.wave.openxr.feature.xrtracker";

        #region Tracker Product Name
        public const string kProductUltimateTracker = "VIVE Ultimate Tracker";
        public const string kProductUltimateTracker0 = "VIVE Ultimate Tracker 0";
        public const string kProductUltimateTracker1 = "VIVE Ultimate Tracker 1";
        public const string kProductUltimateTracker2 = "VIVE Ultimate Tracker 2";
        public const string kProductUltimateTracker3 = "VIVE Ultimate Tracker 3";
        public const string kProductUltimateTracker4 = "VIVE Ultimate Tracker 4";
#if UNITY_STANDALONE
        public const string kProductUltimateTracker5 = "VIVE Ultimate Tracker 5";
        public const string kProductUltimateTracker6 = "VIVE Ultimate Tracker 6";
        public const string kProductUltimateTracker7 = "VIVE Ultimate Tracker 7";
        public const string kProductUltimateTracker8 = "VIVE Ultimate Tracker 8";
        public const string kProductUltimateTracker9 = "VIVE Ultimate Tracker 9";
        public const string kProductUltimateTracker10 = "VIVE Ultimate Tracker 10";
        public const string kProductUltimateTracker11 = "VIVE Ultimate Tracker 11";
        public const string kProductUltimateTracker12 = "VIVE Ultimate Tracker 12";
        public const string kProductUltimateTracker13 = "VIVE Ultimate Tracker 13";
        public const string kProductUltimateTracker14 = "VIVE Ultimate Tracker 14";
        public const string kProductUltimateTracker15 = "VIVE Ultimate Tracker 15";
        public const string kProductUltimateTracker16 = "VIVE Ultimate Tracker 16";
        public const string kProductUltimateTracker17 = "VIVE Ultimate Tracker 17";
        public const string kProductUltimateTracker18 = "VIVE Ultimate Tracker 18";
        public const string kProductUltimateTracker19 = "VIVE Ultimate Tracker 19";
#endif
        const string kProductTrackingTag = "VIVE Tracking Tag";
        private const string kProducts = "^(" + kProductUltimateTracker
            + ")|^(" + kProductUltimateTracker0 + ")|^(" + kProductUltimateTracker1 + ")|^(" + kProductUltimateTracker2 + ")|^(" + kProductUltimateTracker3 + ")|^(" + kProductUltimateTracker4
#if UNITY_STANDALONE
            + ")|^(" + kProductUltimateTracker5 + ")|^(" + kProductUltimateTracker6 + ")|^(" + kProductUltimateTracker7 + ")|^(" + kProductUltimateTracker8 + ")|^(" + kProductUltimateTracker9
            + ")|^(" + kProductUltimateTracker10 + ")|^(" + kProductUltimateTracker11 + ")|^(" + kProductUltimateTracker12 + ")|^(" + kProductUltimateTracker13 + ")|^(" + kProductUltimateTracker14
            + ")|^(" + kProductUltimateTracker15 + ")|^(" + kProductUltimateTracker16 + ")|^(" + kProductUltimateTracker17 + ")|^(" + kProductUltimateTracker18 + ")|^(" + kProductUltimateTracker19
#endif
            + ")|^(" + kProductTrackingTag + ")";
        private readonly string[] s_UltimateTrackerProduct = { kProductUltimateTracker0, kProductUltimateTracker1, kProductUltimateTracker2, kProductUltimateTracker3, kProductUltimateTracker4
#if UNITY_STANDALONE
               ,kProductUltimateTracker5, kProductUltimateTracker6, kProductUltimateTracker7, kProductUltimateTracker8, kProductUltimateTracker9,
                kProductUltimateTracker10, kProductUltimateTracker11, kProductUltimateTracker12, kProductUltimateTracker13, kProductUltimateTracker14,
                kProductUltimateTracker15, kProductUltimateTracker16, kProductUltimateTracker17, kProductUltimateTracker18, kProductUltimateTracker19
#endif
        };
        private bool IsUltimateTracker(string product)
        {
            for (int i = 0; i < s_UltimateTrackerProduct.Length; i++)
            {
                if (s_UltimateTrackerProduct[i].Equals(product))
                    return true;
            }
            return false;
        }
        #endregion

        #region Tracker Action Map Name
        const string kUltimateTrackerActionMap0 = "viveultimatetracker0";
        const string kUltimateTrackerActionMap1 = "viveultimatetracker1";
        const string kUltimateTrackerActionMap2 = "viveultimatetracker2";
        const string kUltimateTrackerActionMap3 = "viveultimatetracker3";
        const string kUltimateTrackerActionMap4 = "viveultimatetracker4";
#if UNITY_STANDALONE
        const string kUltimateTrackerActionMap5 = "viveultimatetracker5";
        const string kUltimateTrackerActionMap6 = "viveultimatetracker6";
        const string kUltimateTrackerActionMap7 = "viveultimatetracker7";
        const string kUltimateTrackerActionMap8 = "viveultimatetracker8";
        const string kUltimateTrackerActionMap9 = "viveultimatetracker9";
        const string kUltimateTrackerActionMap10 = "viveultimatetracker10";
        const string kUltimateTrackerActionMap11 = "viveultimatetracker11";
        const string kUltimateTrackerActionMap12 = "viveultimatetracker12";
        const string kUltimateTrackerActionMap13 = "viveultimatetracker13";
        const string kUltimateTrackerActionMap14 = "viveultimatetracker14";
        const string kUltimateTrackerActionMap15 = "viveultimatetracker15";
        const string kUltimateTrackerActionMap16 = "viveultimatetracker16";
        const string kUltimateTrackerActionMap17 = "viveultimatetracker17";
        const string kUltimateTrackerActionMap18 = "viveultimatetracker18";
        const string kUltimateTrackerActionMap19 = "viveultimatetracker19";
#endif 
        #endregion

        #region Tracker Usage
        const string kUltimateTrackerUsage0 = "Ultimate Tracker 0";
        const string kUltimateTrackerUsage1 = "Ultimate Tracker 1";
        const string kUltimateTrackerUsage2 = "Ultimate Tracker 2";
        const string kUltimateTrackerUsage3 = "Ultimate Tracker 3";
        const string kUltimateTrackerUsage4 = "Ultimate Tracker 4";
#if UNITY_STANDALONE
        const string kUltimateTrackerUsage5 = "Ultimate Tracker 5";
        const string kUltimateTrackerUsage6 = "Ultimate Tracker 6";
        const string kUltimateTrackerUsage7 = "Ultimate Tracker 7";
        const string kUltimateTrackerUsage8 = "Ultimate Tracker 8";
        const string kUltimateTrackerUsage9 = "Ultimate Tracker 9";
        const string kUltimateTrackerUsage10 = "Ultimate Tracker 10";
        const string kUltimateTrackerUsage11 = "Ultimate Tracker 11";
        const string kUltimateTrackerUsage12 = "Ultimate Tracker 12";
        const string kUltimateTrackerUsage13 = "Ultimate Tracker 13";
        const string kUltimateTrackerUsage14 = "Ultimate Tracker 14";
        const string kUltimateTrackerUsage15 = "Ultimate Tracker 15";
        const string kUltimateTrackerUsage16 = "Ultimate Tracker 16";
        const string kUltimateTrackerUsage17 = "Ultimate Tracker 17";
        const string kUltimateTrackerUsage18 = "Ultimate Tracker 18";
        const string kUltimateTrackerUsage19 = "Ultimate Tracker 19";
#endif
        #endregion

        #region Tracker User Path
        public const string kUltimateTrackerPath0 = "/user/xr_tracker_htc/vive_ultimate_tracker_0";
        public const string kUltimateTrackerPath1 = "/user/xr_tracker_htc/vive_ultimate_tracker_1";
        public const string kUltimateTrackerPath2 = "/user/xr_tracker_htc/vive_ultimate_tracker_2";
        public const string kUltimateTrackerPath3 = "/user/xr_tracker_htc/vive_ultimate_tracker_3";
        public const string kUltimateTrackerPath4 = "/user/xr_tracker_htc/vive_ultimate_tracker_4";
#if UNITY_STANDALONE
        public const string kUltimateTrackerPath5 = "/user/xr_tracker_htc/vive_ultimate_tracker_5";
        public const string kUltimateTrackerPath6 = "/user/xr_tracker_htc/vive_ultimate_tracker_6";
        public const string kUltimateTrackerPath7 = "/user/xr_tracker_htc/vive_ultimate_tracker_7";
        public const string kUltimateTrackerPath8 = "/user/xr_tracker_htc/vive_ultimate_tracker_8";
        public const string kUltimateTrackerPath9 = "/user/xr_tracker_htc/vive_ultimate_tracker_9";
        public const string kUltimateTrackerPath10 = "/user/xr_tracker_htc/vive_ultimate_tracker_10";
        public const string kUltimateTrackerPath11 = "/user/xr_tracker_htc/vive_ultimate_tracker_11";
        public const string kUltimateTrackerPath12 = "/user/xr_tracker_htc/vive_ultimate_tracker_12";
        public const string kUltimateTrackerPath13 = "/user/xr_tracker_htc/vive_ultimate_tracker_13";
        public const string kUltimateTrackerPath14 = "/user/xr_tracker_htc/vive_ultimate_tracker_14";
        public const string kUltimateTrackerPath15 = "/user/xr_tracker_htc/vive_ultimate_tracker_15";
        public const string kUltimateTrackerPath16 = "/user/xr_tracker_htc/vive_ultimate_tracker_16";
        public const string kUltimateTrackerPath17 = "/user/xr_tracker_htc/vive_ultimate_tracker_17";
        public const string kUltimateTrackerPath18 = "/user/xr_tracker_htc/vive_ultimate_tracker_18";
        public const string kUltimateTrackerPath19 = "/user/xr_tracker_htc/vive_ultimate_tracker_19";
#endif
        #endregion

        #region Tracker Serial Number
        public const string kUltimateTrackerSN0 = "VIVE_Ultimate_Tracker_0";
        public const string kUltimateTrackerSN1 = "VIVE_Ultimate_Tracker_1";
        public const string kUltimateTrackerSN2 = "VIVE_Ultimate_Tracker_2";
        public const string kUltimateTrackerSN3 = "VIVE_Ultimate_Tracker_3";
        public const string kUltimateTrackerSN4 = "VIVE_Ultimate_Tracker_4";
#if UNITY_STANDALONE
        public const string kUltimateTrackerSN5 = "VIVE_Ultimate_Tracker_5";
        public const string kUltimateTrackerSN6 = "VIVE_Ultimate_Tracker_6";
        public const string kUltimateTrackerSN7 = "VIVE_Ultimate_Tracker_7";
        public const string kUltimateTrackerSN8 = "VIVE_Ultimate_Tracker_8";
        public const string kUltimateTrackerSN9 = "VIVE_Ultimate_Tracker_9";
        public const string kUltimateTrackerSN10 = "VIVE_Ultimate_Tracker_10";
        public const string kUltimateTrackerSN11 = "VIVE_Ultimate_Tracker_11";
        public const string kUltimateTrackerSN12 = "VIVE_Ultimate_Tracker_12";
        public const string kUltimateTrackerSN13 = "VIVE_Ultimate_Tracker_13";
        public const string kUltimateTrackerSN14 = "VIVE_Ultimate_Tracker_14";
        public const string kUltimateTrackerSN15 = "VIVE_Ultimate_Tracker_15";
        public const string kUltimateTrackerSN16 = "VIVE_Ultimate_Tracker_16";
        public const string kUltimateTrackerSN17 = "VIVE_Ultimate_Tracker_17";
        public const string kUltimateTrackerSN18 = "VIVE_Ultimate_Tracker_18";
        public const string kUltimateTrackerSN19 = "VIVE_Ultimate_Tracker_19";
#endif
        const string kTrackingTagSN0 = "VIVE_Tracking_Tag_0";
        const string kTrackingTagSN1 = "VIVE_Tracking_Tag_1";
        const string kTrackingTagSN2 = "VIVE_Tracking_Tag_2";
        const string kTrackingTagSN3 = "VIVE_Tracking_Tag_3";
        const string kTrackingTagSN4 = "VIVE_Tracking_Tag_4";
        const string k6DoFTrackerSN0 = "VIVE_6DoF_Tracker_a_0";
        const string k6DoFTrackerSN1 = "VIVE_6DoF_Tracker_a_1";
        #endregion

        #region Tracker Product Maps
        private Dictionary<string, string> m_UltimateTrackerActionMap = new Dictionary<string, string>() {
            { kProductUltimateTracker0, kUltimateTrackerActionMap0 },
            { kProductUltimateTracker1, kUltimateTrackerActionMap1 },
            { kProductUltimateTracker2, kUltimateTrackerActionMap2 },
            { kProductUltimateTracker3, kUltimateTrackerActionMap3 },
            { kProductUltimateTracker4, kUltimateTrackerActionMap4 },
#if UNITY_STANDALONE
            { kProductUltimateTracker5, kUltimateTrackerActionMap5 },
            { kProductUltimateTracker6, kUltimateTrackerActionMap6 },
            { kProductUltimateTracker7, kUltimateTrackerActionMap7 },
            { kProductUltimateTracker8, kUltimateTrackerActionMap8 },
            { kProductUltimateTracker9, kUltimateTrackerActionMap9 },
            { kProductUltimateTracker10, kUltimateTrackerActionMap10 },
            { kProductUltimateTracker11, kUltimateTrackerActionMap11 },
            { kProductUltimateTracker12, kUltimateTrackerActionMap12 },
            { kProductUltimateTracker13, kUltimateTrackerActionMap13 },
            { kProductUltimateTracker14, kUltimateTrackerActionMap14 },
            { kProductUltimateTracker15, kUltimateTrackerActionMap15 },
            { kProductUltimateTracker16, kUltimateTrackerActionMap16 },
            { kProductUltimateTracker17, kUltimateTrackerActionMap17 },
            { kProductUltimateTracker18, kUltimateTrackerActionMap18 },
            { kProductUltimateTracker19, kUltimateTrackerActionMap19 },
#endif
        };
        /// <summary> Mapping from product to tracker usage. </summary>
        private static Dictionary<string, string> m_UltimateTrackerUsageMap = new Dictionary<string, string>() {
            { kProductUltimateTracker0, kUltimateTrackerUsage0 },
            { kProductUltimateTracker1, kUltimateTrackerUsage1 },
            { kProductUltimateTracker2, kUltimateTrackerUsage2 },
            { kProductUltimateTracker3, kUltimateTrackerUsage3 },
            { kProductUltimateTracker4, kUltimateTrackerUsage4 },
#if UNITY_STANDALONE
            { kProductUltimateTracker5, kUltimateTrackerUsage5 },
            { kProductUltimateTracker6, kUltimateTrackerUsage6 },
            { kProductUltimateTracker7, kUltimateTrackerUsage7 },
            { kProductUltimateTracker8, kUltimateTrackerUsage8 },
            { kProductUltimateTracker9, kUltimateTrackerUsage9 },
            { kProductUltimateTracker10, kUltimateTrackerUsage10 },
            { kProductUltimateTracker11, kUltimateTrackerUsage11 },
            { kProductUltimateTracker12, kUltimateTrackerUsage12 },
            { kProductUltimateTracker13, kUltimateTrackerUsage13 },
            { kProductUltimateTracker14, kUltimateTrackerUsage14 },
            { kProductUltimateTracker15, kUltimateTrackerUsage15 },
            { kProductUltimateTracker16, kUltimateTrackerUsage16 },
            { kProductUltimateTracker17, kUltimateTrackerUsage17 },
            { kProductUltimateTracker18, kUltimateTrackerUsage18 },
            { kProductUltimateTracker19, kUltimateTrackerUsage19 },
#endif
        };
        /// <summary> Mapping from product to user path. </summary>
        private Dictionary<string, string> m_UltimateTrackerPathMap = new Dictionary<string, string>() {
            { kProductUltimateTracker0, kUltimateTrackerPath0 },
            { kProductUltimateTracker1, kUltimateTrackerPath1 },
            { kProductUltimateTracker2, kUltimateTrackerPath2 },
            { kProductUltimateTracker3, kUltimateTrackerPath3 },
            { kProductUltimateTracker4, kUltimateTrackerPath4 },
#if UNITY_STANDALONE
            { kProductUltimateTracker5, kUltimateTrackerPath5 },
            { kProductUltimateTracker6, kUltimateTrackerPath6 },
            { kProductUltimateTracker7, kUltimateTrackerPath7 },
            { kProductUltimateTracker8, kUltimateTrackerPath8 },
            { kProductUltimateTracker9, kUltimateTrackerPath9 },
            { kProductUltimateTracker10, kUltimateTrackerPath10 },
            { kProductUltimateTracker11, kUltimateTrackerPath11 },
            { kProductUltimateTracker12, kUltimateTrackerPath12 },
            { kProductUltimateTracker13, kUltimateTrackerPath13 },
            { kProductUltimateTracker14, kUltimateTrackerPath14 },
            { kProductUltimateTracker15, kUltimateTrackerPath15 },
            { kProductUltimateTracker16, kUltimateTrackerPath16 },
            { kProductUltimateTracker17, kUltimateTrackerPath17 },
            { kProductUltimateTracker18, kUltimateTrackerPath18 },
            { kProductUltimateTracker19, kUltimateTrackerPath19 },
#endif
        };
        /// <summary> Mapping from product to serial number. </summary>
        private Dictionary<string, string> m_UltimateTrackerSerialMap = new Dictionary<string, string>() {
            { kProductUltimateTracker0, kUltimateTrackerSN0 },
            { kProductUltimateTracker1, kUltimateTrackerSN1 },
            { kProductUltimateTracker2, kUltimateTrackerSN2 },
            { kProductUltimateTracker3, kUltimateTrackerSN3 },
            { kProductUltimateTracker4, kUltimateTrackerSN4 },
#if UNITY_STANDALONE
            { kProductUltimateTracker5, kUltimateTrackerSN5 },
            { kProductUltimateTracker6, kUltimateTrackerSN6 },
            { kProductUltimateTracker7, kUltimateTrackerSN7 },
            { kProductUltimateTracker8, kUltimateTrackerSN8 },
            { kProductUltimateTracker9, kUltimateTrackerSN9 },
            { kProductUltimateTracker10, kUltimateTrackerSN10 },
            { kProductUltimateTracker11, kUltimateTrackerSN11 },
            { kProductUltimateTracker12, kUltimateTrackerSN12 },
            { kProductUltimateTracker13, kUltimateTrackerSN13 },
            { kProductUltimateTracker14, kUltimateTrackerSN14 },
            { kProductUltimateTracker15, kUltimateTrackerSN15 },
            { kProductUltimateTracker16, kUltimateTrackerSN16 },
            { kProductUltimateTracker17, kUltimateTrackerSN17 },
            { kProductUltimateTracker18, kUltimateTrackerSN18 },
            { kProductUltimateTracker19, kUltimateTrackerSN19 },
#endif
        };
        #endregion

        [Preserve, InputControlLayout(displayName = "VIVE XR Tracker (OpenXR)", commonUsages = new[] {
            kUltimateTrackerUsage0, kUltimateTrackerUsage1, kUltimateTrackerUsage2, kUltimateTrackerUsage3, kUltimateTrackerUsage4,
#if UNITY_STANDALONE
            kUltimateTrackerUsage5, kUltimateTrackerUsage6, kUltimateTrackerUsage7, kUltimateTrackerUsage8, kUltimateTrackerUsage9,
            kUltimateTrackerUsage10, kUltimateTrackerUsage11, kUltimateTrackerUsage12, kUltimateTrackerUsage13, kUltimateTrackerUsage14,
            kUltimateTrackerUsage15, kUltimateTrackerUsage16, kUltimateTrackerUsage17, kUltimateTrackerUsage18, kUltimateTrackerUsage19,
#endif
        }, isGenericTypeOfDevice = true)]
        public class XrTrackerDevice : OpenXRDevice//, IInputUpdateCallbackReceiver
        {
            #region Log
            const string LOG_TAG = "VIVE.OpenXR.Tracker.ViveXRTracker.XrTrackerDevice ";
            StringBuilder m_sb = null;
            StringBuilder sb {
                get {
                    if (m_sb == null) { m_sb = new StringBuilder(); }
                    return m_sb;
                }
            }
            void DEBUG(StringBuilder msg) { Debug.Log(msg); }
            #endregion

            #region Interactions
            /// <summary>
            /// A <see cref="PoseControl"/> that represents the <see cref="entityPose"/> OpenXR binding. The entity pose represents the location of the tracker.
            /// </summary>
            [Preserve, InputControl(offset = 0, aliases = new[] { "device", "entityPose" }, usage = "Device")]
            public PoseControl devicePose { get; private set; }

            /// <summary>
            /// A <see href="https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Controls.ButtonControl.html">ButtonControl</see> required for backwards compatibility with the XRSDK layouts. This represents the overall tracking state of the device. This value is equivalent to mapping devicePose/isTracked.
            /// </summary>
            [Preserve, InputControl(offset = 0, usage = "IsTracked")]
            public ButtonControl isTracked { get; private set; }

            /// <summary>
            /// A <see href="https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Controls.IntegerControl.html">IntegerControl</see> required for backwards compatibility with the XRSDK layouts. This represents the bit flag set to indicate what data is valid. This value is equivalent to mapping devicePose/trackingState.
            /// </summary>
            [Preserve, InputControl(offset = 4, usage = "TrackingState")]
            public IntegerControl trackingState { get; private set; }

            /// <summary>
            /// A <see href="https://docs.unity3d.com/Packages/com.unity.inputsystem@0.1/api/UnityEngine.Experimental.Input.Controls.Vector3Control.html">Vector3Control</see> required for backwards compatibility with the XRSDK layouts. This is the device position. For the VIVE XR device, this is both the device and the pointer position. This value is equivalent to mapping devicePose/position.
            /// </summary>
            [Preserve, InputControl(offset = 8, alias = "gripPosition")]
            public Vector3Control devicePosition { get; private set; }

            /// <summary>
            /// A <see href="https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Controls.QuaternionControl.html">QuaternionControl</see> required for backwards compatibility with the XRSDK layouts. This is the device orientation. For the VIVE XR device, this is both the device and the pointer rotation. This value is equivalent to mapping devicePose/rotation.
            /// </summary>
            [Preserve, InputControl(offset = 20, alias = "gripOrientation")]
            public QuaternionControl deviceRotation { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="ViveXRTracker.system"/> OpenXR bindings, depending on handedness.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "systemButton" }, usage = "SystemButton")]
            public ButtonControl system { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="ViveXRTracker.menu"/> OpenXR bindings, depending on handedness.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "menuButton" }, usage = "MenuButton")]
            public ButtonControl menu { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="ViveXRTracker.gripPress"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripButton", "squeezeClick" }, usage = "GripButton")]
            public ButtonControl gripPress { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="ViveXRTracker.triggerClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "triggerButton", "triggerClick" }, usage = "TriggerButton")]
            public ButtonControl triggerPress { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="ViveXRTracker.trackpadClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "TrackpadClick", "trackpadClick" }, usage = "TrackpadButton")]
            public ButtonControl trackpadPress { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents the <see cref="ViveXRTracker.trackpadTouch"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "TrackpadTouch" }, usage = "TrackpadTouch")]
            public ButtonControl trackpadTouch { get; private set; }

            /// <summary>
            /// A <see cref="HapticControl"/> that represents the <see cref="ViveXRTracker.haptic"/> binding.
            /// </summary>
            [Preserve, InputControl(usage = "Haptic")]
            public HapticControl haptic { get; private set; }
            #endregion

#if DEBUG_CODE
            // Unity action binding path: <ViveXRTracker>{Tracker 0}/isTracked
            private InputAction inputActionIsTracked = null;
#endif

            /// <summary>
            /// Internal call used to assign controls to the the correct element.
            /// </summary>
            protected override void FinishSetup()
            {
                base.FinishSetup();

                devicePose = GetChildControl<PoseControl>("devicePose");
                isTracked = GetChildControl<ButtonControl>("isTracked");
                trackingState = GetChildControl<IntegerControl>("trackingState");
                devicePosition = GetChildControl<Vector3Control>("devicePosition");
                deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");

                system = GetChildControl<ButtonControl>("system");
                menu = GetChildControl<ButtonControl>("menu");
                gripPress = GetChildControl<ButtonControl>("gripPress");
                triggerPress = GetChildControl<ButtonControl>("triggerPress");
                trackpadPress = GetChildControl<ButtonControl>("trackpadPress");
                trackpadTouch = GetChildControl<ButtonControl>("trackpadTouch");
                haptic = GetChildControl<HapticControl>("haptic");

                sb.Clear().Append(LOG_TAG)
                    .Append("FinishSetup() device interfaceName: ").Append(description.interfaceName)
                    .Append(", deviceClass: ").Append(description.deviceClass)
                    .Append(", product: ").Append(description.product)
                    .Append(", serial: ").Append(description.serial)
                    .Append(", current profile: ").Append(profile);
                DEBUG(sb);

                if (m_UltimateTrackerUsageMap.ContainsKey(description.product))
                {
                    /// After RegisterActionMapsWithRuntime finished, each XrTrackerDevice will have a product name (e.g. kProductUltimateTracker0)
                    /// We have to assign the XrTrackerDeivce to a commonUsage (e.g. kUltimateTrackerUsage0)
                    /// 
                    /// Since we already established the m_UltimateTrackerUsageMap (kProductUltimateTracker0, kUltimateTrackerUsage0),
                    /// we can simply call SetDeviceUsage(m_UltimateTrackerUsageMap[description.product])
                    /// to set assign the XrTrackerDevice with product name kProductUltimateTracker0 to the commonUsage kUltimateTrackerUsage0.
                    InputSystem.SetDeviceUsage(this, m_UltimateTrackerUsageMap[description.product]);
                    sb.Clear().Append(LOG_TAG).Append("FinishSetup() usage: ").Append(m_UltimateTrackerUsageMap[description.product]); DEBUG(sb);
#if DEBUG_CODE
                    /// We cannot update the ActionMap outside the RegisterActionMapsWithRuntime method so ignore this code.
                    if (inputActionIsTracked == null)
                    {
                        //string actionBindingIsTracked = "<XRController>{LeftHand}/isTracked";
                        string actionBindingIsTracked = "<" + kLayoutName + ">{" + m_UltimateTrackerUsageMap[description.product] + "}/isTracked";
                        sb.Clear().Append(LOG_TAG).Append("FinishSetup() ").Append(m_UltimateTrackerUsageMap[description.product]).Append(", action binding of IsTracked: ").Append(actionBindingIsTracked);
                        DEBUG(sb);

                        inputActionIsTracked = new InputAction(
                            type: InputActionType.Value,
                            binding: actionBindingIsTracked);

                        inputActionIsTracked.Enable();
                    }
#endif
                }
            }

#if DEBUG_CODE
            /// We cannot update the ActionMap outside the RegisterActionMapsWithRuntime method so ignore this code.
            private bool UpdateInputDeviceInRuntime = true;
            private bool bRoleUpdated = false;
            public void OnUpdate()
            {
                if (!UpdateInputDeviceInRuntime) { return; }
                if (m_Instance == null) { return; }
                if (inputActionIsTracked == null) { return; }

                /// Updates the Usage (tracker role) when IsTracked becomes true.
                if (inputActionIsTracked.ReadValue<float>() > 0 && !bRoleUpdated)
                {
                    sb.Clear().Append(LOG_TAG).Append("OnUpdate() Update the InputDevice with product: ").Append(description.product); DEBUG(sb);

                    if (m_UltimateTrackerUsageMap.ContainsKey(description.product)) { m_Instance.UpdateInputDeviceUltimateTracker(description.product); }

                    bRoleUpdated = true;
                }
            }
#endif
        }

        /// <summary>The interaction profile string used to reference the wrist tracker interaction input device.</summary>
        private const string profile = "/interaction_profiles/htc/vive_xr_tracker";

        #region OpenXR Life Cycle
#pragma warning disable
        private bool m_XrInstanceCreated = false;
#pragma warning restore
        private XrInstance m_XrInstance = 0;
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreateInstance">xrCreateInstance</see> is done.
        /// </summary>
        /// <param name="xrInstance">The created instance.</param>
        /// <returns>True for valid <see cref="XrInstance">XrInstance</see></returns>
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            m_XrInstanceCreated = true;
            m_XrInstance = xrInstance;
            m_Instance = this;
            sb.Clear().Append(LOG_TAG).Append("OnInstanceCreate() ").Append(m_XrInstance); DEBUG(sb);

            GetXrFunctionDelegates(m_XrInstance);
            return true;
        }
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroyInstance">xrDestroyInstance</see> is done.
        /// </summary>
        /// <param name="xrInstance">The instance to destroy.</param>
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            if (m_XrInstance == xrInstance)
            {
                m_XrInstanceCreated = false;
                m_XrInstance = 0;
            }
            sb.Clear().Append(LOG_TAG).Append("OnInstanceDestroy() ").Append(xrInstance); DEBUG(sb);
        }

        private static bool m_XrSessionCreated = false;
        private static XrSession m_XrSession = 0;
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreateSession">xrCreateSession</see> is done.
        /// </summary>
        /// <param name="xrSession">The created session ID.</param>
        protected override void OnSessionCreate(ulong xrSession)
        {
            m_XrSession = xrSession;
            m_XrSessionCreated = true;
            sb.Clear().Append(LOG_TAG).Append("OnSessionCreate() ").Append(m_XrSession); DEBUG(sb);
        }
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroySession">xrDestroySession</see> is done.
        /// </summary>
        /// <param name="xrSession">The session ID to destroy.</param>
        protected override void OnSessionDestroy(ulong xrSession)
        {
            sb.Clear().Append(LOG_TAG).Append("OnSessionDestroy() ").Append(xrSession); DEBUG(sb);
            if (m_XrSession == xrSession)
            {
                m_XrSession = 0;
                m_XrSessionCreated = false;
            }
        }

        // "<" + kLayoutName + ">{" + m_UltimateTrackerUsageMap[description.product] + "}/isTracked"
        private const string kLayoutName = "ViveXRTracker";

        /// <summary>
        /// Registers the <see cref="XrTrackerDevice"/> layout with product name to the Input System.
        /// </summary>
        protected override void RegisterDeviceLayout()
        {
            sb.Clear().Append(LOG_TAG).Append("RegisterDeviceLayout() ").Append(kLayoutName).Append(", product: ").Append(@kProducts); DEBUG(sb);
            InputSystem.RegisterLayout(typeof(XrTrackerDevice),
                        kLayoutName,
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(@kProducts));
        }

        /// <summary>
        /// Removes the <see cref="XrTrackerDevice"/> layout from the Input System.
        /// </summary>
        protected override void UnregisterDeviceLayout()
        {
            sb.Clear().Append(LOG_TAG).Append("UnregisterDeviceLayout() ").Append(kLayoutName); DEBUG(sb);
            InputSystem.RemoveLayout(kLayoutName);
        }
        #endregion

        #region OpenXR function delegates
        /// xrGetInstanceProcAddr
        OpenXRHelper.xrGetInstanceProcAddrDelegate XrGetInstanceProcAddr;
        /// xrGetInputSourceLocalizedName
        static OpenXRHelper.xrGetInputSourceLocalizedNameDelegate xrGetInputSourceLocalizedName = null;
        /// xrEnumerateInstanceExtensionProperties
        OpenXRHelper.xrEnumerateInstanceExtensionPropertiesDelegate xrEnumerateInstanceExtensionProperties = null;
        private bool GetXrFunctionDelegates(XrInstance xrInstance)
        {
            /// xrGetInstanceProcAddr
            if (xrGetInstanceProcAddr != null && xrGetInstanceProcAddr != IntPtr.Zero)
            {
                sb.Clear().Append(LOG_TAG).Append("Get function pointer of xrGetInstanceProcAddr."); DEBUG(sb);
                XrGetInstanceProcAddr = Marshal.GetDelegateForFunctionPointer(
                    xrGetInstanceProcAddr,
                    typeof(OpenXRHelper.xrGetInstanceProcAddrDelegate)) as OpenXRHelper.xrGetInstanceProcAddrDelegate;
            }
            else
            {
                sb.Clear().Append(LOG_TAG).Append("No function pointer of xrGetInstanceProcAddr"); ERROR(sb);
                return false;
            }

            IntPtr funcPtr = IntPtr.Zero;

            /// xrEnumerateInstanceExtensionProperties
            if (XrGetInstanceProcAddr(xrInstance, "xrEnumerateInstanceExtensionProperties", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    sb.Clear().Append(LOG_TAG).Append("Get function pointer of xrEnumerateInstanceExtensionProperties."); DEBUG(sb);
                    xrEnumerateInstanceExtensionProperties = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(OpenXRHelper.xrEnumerateInstanceExtensionPropertiesDelegate)) as OpenXRHelper.xrEnumerateInstanceExtensionPropertiesDelegate;
                }
                else
                {
                    sb.Clear().Append(LOG_TAG).Append("No function pointer of xrEnumerateInstanceExtensionProperties.");
                    ERROR(sb);
                }
            }
            else
            {
                sb.Clear().Append(LOG_TAG).Append("No function pointer of xrEnumerateInstanceExtensionProperties");
                ERROR(sb);
            }

            /// xrGetInputSourceLocalizedName
            if (XrGetInstanceProcAddr(xrInstance, "xrGetInputSourceLocalizedName", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    sb.Clear().Append(LOG_TAG).Append("Get function pointer of xrGetInputSourceLocalizedName."); DEBUG(sb);
                    xrGetInputSourceLocalizedName = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(OpenXRHelper.xrGetInputSourceLocalizedNameDelegate)) as OpenXRHelper.xrGetInputSourceLocalizedNameDelegate;
                }
                else
                {
                    sb.Clear().Append(LOG_TAG).Append("No function pointer of xrGetInputSourceLocalizedName.");
                    ERROR(sb);
                }
            }
            else
            {
                sb.Clear().Append(LOG_TAG).Append("No function pointer of xrGetInputSourceLocalizedName");
                ERROR(sb);
            }

            return true;
        }
        #endregion

        // Available Bindings
        /// <summary>
        /// Constant for a pose interaction binding '.../input/entity_htc/pose' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string entityPose = "/input/entity_htc/pose";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/system/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string system = "/input/system/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/menu/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string menu = "/input/menu/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/squeeze/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string gripPress = "/input/squeeze/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trigger/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string triggerClick = "/input/trigger/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadClick = "/input/trackpad/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/touch' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadTouch = "/input/trackpad/touch";
        /// <summary>
        /// Constant for a haptic interaction binding '.../output/haptic' OpenXR Input Binding. Used by input subsystem to bind actions to physical outputs.
        /// </summary>
        public const string haptic = "/output/haptic";

        private Dictionary<string, List<ActionConfig>> m_UltimateTrackerActionConfig = new Dictionary<string, List<ActionConfig>>()
        {
            { kProductUltimateTracker0, new List<ActionConfig>()
                {
                    // Device Pose
                    new ActionConfig()
                    {
                        name = "devicePose",
                        localizedName = "Grip Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = entityPose,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // System
                    new ActionConfig()
                    {
                        name = "system",
                        localizedName = "System",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "SystemButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = system,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Menu
			        new ActionConfig()
                    {
                        name = "menu",
                        localizedName = "Menu",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "MenuButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = menu,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Grip Press
			        new ActionConfig()
                    {
                        name = "gripPress",
                        localizedName = "Grip Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "GripButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = gripPress,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trigger Press
			        new ActionConfig()
                    {
                        name = "triggerPress",
                        localizedName = "Trigger Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TriggerButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = triggerClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Press
			        new ActionConfig()
                    {
                        name = "trackpadPress",
                        localizedName = "Trackpad Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Touch
			        new ActionConfig()
                    {
                        name = "trackpadTouch",
                        localizedName = "Trackpad Touch",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadTouch"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Haptics
                    new ActionConfig()
                    {
                        name = "haptic",
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>() { "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    }
                } },
            { kProductUltimateTracker1, new List<ActionConfig>()
                {
                    // Device Pose
                    new ActionConfig()
                    {
                        name = "devicePose",
                        localizedName = "Grip Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = entityPose,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // System
                    new ActionConfig()
                    {
                        name = "system",
                        localizedName = "System",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "SystemButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = system,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Menu
			        new ActionConfig()
                    {
                        name = "menu",
                        localizedName = "Menu",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "MenuButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = menu,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Grip Press
			        new ActionConfig()
                    {
                        name = "gripPress",
                        localizedName = "Grip Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "GripButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = gripPress,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trigger Press
			        new ActionConfig()
                    {
                        name = "triggerPress",
                        localizedName = "Trigger Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TriggerButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = triggerClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Press
			        new ActionConfig()
                    {
                        name = "trackpadPress",
                        localizedName = "Trackpad Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Touch
			        new ActionConfig()
                    {
                        name = "trackpadTouch",
                        localizedName = "Trackpad Touch",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadTouch"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Haptics
                    new ActionConfig()
                    {
                        name = "haptic",
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>() { "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    }
                } },
            { kProductUltimateTracker2, new List<ActionConfig>()
                {
                    // Device Pose
                    new ActionConfig()
                    {
                        name = "devicePose",
                        localizedName = "Grip Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = entityPose,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // System
                    new ActionConfig()
                    {
                        name = "system",
                        localizedName = "System",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "SystemButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = system,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Menu
			        new ActionConfig()
                    {
                        name = "menu",
                        localizedName = "Menu",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "MenuButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = menu,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Grip Press
			        new ActionConfig()
                    {
                        name = "gripPress",
                        localizedName = "Grip Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "GripButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = gripPress,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trigger Press
			        new ActionConfig()
                    {
                        name = "triggerPress",
                        localizedName = "Trigger Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TriggerButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = triggerClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Press
			        new ActionConfig()
                    {
                        name = "trackpadPress",
                        localizedName = "Trackpad Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Touch
			        new ActionConfig()
                    {
                        name = "trackpadTouch",
                        localizedName = "Trackpad Touch",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadTouch"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Haptics
                    new ActionConfig()
                    {
                        name = "haptic",
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>() { "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    }
                } },
            { kProductUltimateTracker3, new List<ActionConfig>()
                {
                    // Device Pose
                    new ActionConfig()
                    {
                        name = "devicePose",
                        localizedName = "Grip Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = entityPose,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // System
                    new ActionConfig()
                    {
                        name = "system",
                        localizedName = "System",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "SystemButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = system,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Menu
			        new ActionConfig()
                    {
                        name = "menu",
                        localizedName = "Menu",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "MenuButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = menu,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Grip Press
			        new ActionConfig()
                    {
                        name = "gripPress",
                        localizedName = "Grip Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "GripButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = gripPress,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trigger Press
			        new ActionConfig()
                    {
                        name = "triggerPress",
                        localizedName = "Trigger Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TriggerButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = triggerClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Press
			        new ActionConfig()
                    {
                        name = "trackpadPress",
                        localizedName = "Trackpad Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Touch
			        new ActionConfig()
                    {
                        name = "trackpadTouch",
                        localizedName = "Trackpad Touch",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadTouch"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Haptics
                    new ActionConfig()
                    {
                        name = "haptic",
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>() { "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    }
                } },
            { kProductUltimateTracker4, new List<ActionConfig>()
                {
                    // Device Pose
                    new ActionConfig()
                    {
                        name = "devicePose",
                        localizedName = "Grip Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = entityPose,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // System
                    new ActionConfig()
                    {
                        name = "system",
                        localizedName = "System",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "SystemButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = system,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Menu
			        new ActionConfig()
                    {
                        name = "menu",
                        localizedName = "Menu",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "MenuButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = menu,
                                interactionProfileName = profile,
                            },
                        }
                    },
			        // Grip Press
			        new ActionConfig()
                    {
                        name = "gripPress",
                        localizedName = "Grip Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "GripButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = gripPress,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trigger Press
			        new ActionConfig()
                    {
                        name = "triggerPress",
                        localizedName = "Trigger Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TriggerButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = triggerClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Press
			        new ActionConfig()
                    {
                        name = "trackpadPress",
                        localizedName = "Trackpad Press",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadClick,
                                interactionProfileName = profile,
                            }
                        }
                    },
			        // Trackpad Touch
			        new ActionConfig()
                    {
                        name = "trackpadTouch",
                        localizedName = "Trackpad Touch",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TrackpadTouch"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trackpadTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    // Haptics
                    new ActionConfig()
                    {
                        name = "haptic",
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>() { "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    }
                } },
        };
        private void UpdateInputActionPath(List<ActionConfig> in_actions, string in_name, string in_path)
		{
            if (in_actions == null || in_actions.Count <= 0) { return; }

            string func = "UpdateInputActionPath() ";
            for (int i = 0; i < in_actions.Count; i++)
            {
                if (in_actions[i].bindings == null || in_actions[i].bindings.Count <= 0) { continue; }
                if (in_actions[i].name.Equals(in_name))
                {
                    for (int j = 0; j < in_actions[i].bindings.Count; j++)
					{
                        sb.Clear().Append(LOG_TAG).Append(func).Append("Replace action path from ").Append(in_actions[i].bindings[j].interactionPath).Append(" to ").Append(in_path); DEBUG(sb);
                        in_actions[i].bindings[j].interactionPath = in_path;
					}
                }
            }
        }

        private void RegisterActionMap(string in_name, string in_product, string in_sn, string in_path, List<ActionConfig> in_action)
        {
            sb.Clear().Append(LOG_TAG).Append("RegisterActionMap() Added ActionMapConfig of ").Append(in_path)
                .Append(", localizedName = ").Append(in_product)
                .Append(" { name = ").Append(in_name)
                .Append(", desiredInteractionProfile = ").Append(profile)
                .Append(", serialNumber = ").Append(in_sn);
            DEBUG(sb);

            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = in_name,
                localizedName = in_product,
                desiredInteractionProfile = profile,
                manufacturer = "HTC",
                serialNumber = in_sn,
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = InputDeviceCharacteristics.TrackedDevice,
                        userPath = in_path
                    }
                },
                actions = in_action
            };

            AddActionMap(actionMap);
        }
        /// <summary>
        /// Registers action maps to Unity XR.
        /// </summary>
        protected override void RegisterActionMapsWithRuntime()
        {
            if (OpenXRHelper.IsExtensionSupported(xrEnumerateInstanceExtensionProperties, kOpenxrExtensionString) != XrResult.XR_SUCCESS)
            {
                sb.Clear().Append(LOG_TAG).Append("RegisterActionMapsWithRuntime() ").Append(kOpenxrExtensionString).Append(" is NOT supported."); ERROR(sb);
                return;
            }

            /// Updates m_UltimateTrackerPathMap.
            if (!EnumeratePath())
            {
                sb.Clear().Append(LOG_TAG).Append("RegisterActionMapsWithRuntime() EnumeratePath failed.");
                ERROR(sb);
            }

            for (int userIndex = 0; userIndex < s_UltimateTrackerProduct.Length; userIndex++)
            {
                string product = s_UltimateTrackerProduct[userIndex];
                RegisterActionMap(
                    in_product: product,
                    in_name: m_UltimateTrackerActionMap[product],
                    in_sn: m_UltimateTrackerSerialMap[product],
                    in_path: m_UltimateTrackerPathMap[product],
                    in_action: m_UltimateTrackerActionConfig[product]
                );
            }
        }

        private bool EnumeratePath()
        {
            string func = "EnumeratePath() ";

            // 1. Get user path
            if (XR_HTC_path_enumeration.Interop.GetUserPaths(profile, out XrPath[] trackerPaths))
            {
                int ultimate_tracker_index = 0;
                for (int i = 0; i < trackerPaths.Length; i++)
                {
                    string userPath = PathToString(trackerPaths[i]);
                    sb.Clear().Append(LOG_TAG).Append(func).Append("trackerPaths[").Append(i).Append("] ").Append(userPath); DEBUG(sb);

                    // Ultimate Tracker
                    if (userPath.Contains("ultimate", StringComparison.OrdinalIgnoreCase) && ultimate_tracker_index < s_UltimateTrackerProduct.Length)
                    {
                        string product = s_UltimateTrackerProduct[ultimate_tracker_index];

                        m_UltimateTrackerPathMap[product] = userPath;
                        sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" path to ").Append(m_UltimateTrackerPathMap[product]); DEBUG(sb);

                        m_UltimateTrackerSerialMap[product] = ConvertUserPathToSerialNumber(userPath);
                        sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" serial number to ").Append(m_UltimateTrackerSerialMap[product]); DEBUG(sb);

                        ultimate_tracker_index++;

                        // 2. Get action path
                        if (XR_HTC_path_enumeration.Interop.GetInputPathsWithUserPath(profile, trackerPaths[i], out XrPath[] inputPaths))
                        {
                            for (int p = 0; p < inputPaths.Length; p++)
                            {
                                string fullPath = PathToString(inputPaths[p]);
                                string actionPath = fullPath.Replace(userPath, "");
                                sb.Clear().Append(LOG_TAG).Append(func).Append(userPath).Append(" inputPaths[").Append(p).Append("] ").Append(actionPath); DEBUG(sb);

                                // devicePose: "/input/entity_htc/pose"
                                if (actionPath.Contains("pose"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of devicePose to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "devicePose", actionPath);
                                }
                                // system: "/input/system/click"
                                if (actionPath.Contains("system") && actionPath.Contains("click"))
								{
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of system to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "system", actionPath);
								}
                                // menu: "/input/menu/click"
                                if (actionPath.Contains("menu") && actionPath.Contains("click"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of menu to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "menu", actionPath);
                                }
                                // gripPress: "/input/squeeze/click"
                                if (actionPath.Contains("squeeze") && actionPath.Contains("click"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of gripPress to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "gripPress", actionPath);
                                }
                                // triggerPress: "/input/trigger/click"
                                if (actionPath.Contains("trigger") && actionPath.Contains("click"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of triggerPress to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "triggerPress", actionPath);
                                }
                                // trackpadPress: "/input/trackpad/click"
                                if (actionPath.Contains("trackpad") && actionPath.Contains("click"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of trackpadPress to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "trackpadPress", actionPath);
                                }
                                // trackpadTouch: "/input/trackpad/touch"
                                if (actionPath.Contains("trackpad") && actionPath.Contains("touch"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of trackpadTouch to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "trackpadTouch", actionPath);
                                }
                                // haptic: "/output/haptic"
                                if (actionPath.Contains("haptic"))
                                {
                                    sb.Clear().Append(LOG_TAG).Append(func).Append("Updates ").Append(product).Append(" action path of haptic to ").Append(actionPath); DEBUG(sb);
                                    UpdateInputActionPath(m_UltimateTrackerActionConfig[product], "haptic", actionPath);
                                }
                            }
                        }
                    }
                }
			}
			else
			{
                sb.Clear().Append(LOG_TAG).Append(func).Append("GetUserPaths(").Append(profile).Append(") failed."); ERROR(sb);
                return false;
			}


            return true;
        }
        /// <summary>
        /// For example, user path "/user/xr_tracker_htc/vive_ultimate_tracker_0" will become serial number "VIVE_Ultimate_Tracker_0".
        /// </summary>
        /// <param name="userPath">The user path from <see cref="EnumeratePath"> EnumeratePath </see>.</param>
        /// <returns>Serial Number in string.</returns>
        private string ConvertUserPathToSerialNumber(string userPath)
        {
            string result = "";

            int lastSlashIndex = userPath.LastIndexOf('/');
            if (lastSlashIndex >= 0)
            {
                string[] parts = userPath.Substring(lastSlashIndex + 1).Split('_');
                parts[0] = parts[0].ToUpper();
                for (int i = 1; i < parts.Length - 1; i++)
                {
                    parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
                }
                result = string.Join("_", parts);
            }

            return result;
        }

        [Obsolete("This enumeration is deprecated. Please use XrInputSourceLocalizedNameFlags instead.")]
        public enum InputSourceType : UInt64
        {
            SerialNumber = XrInputSourceLocalizedNameFlags.XR_INPUT_SOURCE_LOCALIZED_NAME_SERIAL_NUMBER_BIT_HTC,
            TrackerRole = XrInputSourceLocalizedNameFlags.XR_INPUT_SOURCE_LOCALIZED_NAME_USER_PATH_BIT,
        }
        [Obsolete("This function is deprecated. Please use OpenXRHelper.GetInputSourceName instead.")]
        public static XrResult GetInputSourceName(XrPath path, InputSourceType sourceType, out string sourceName)
        {
            string func = "GetInputSourceName() ";

            sourceName = "";
            if (!m_XrSessionCreated || xrGetInputSourceLocalizedName == null) { return XrResult.XR_ERROR_VALIDATION_FAILURE; }

            string userPath = PathToString(path);
            sb.Clear().Append(LOG_TAG).Append(func)
                .Append("userPath: ").Append(userPath).Append(", flag: ").Append((UInt64)sourceType); DEBUG(sb);

            XrInputSourceLocalizedNameGetInfo nameInfo = new XrInputSourceLocalizedNameGetInfo(
                XrStructureType.XR_TYPE_INPUT_SOURCE_LOCALIZED_NAME_GET_INFO,
                IntPtr.Zero, path, (XrInputSourceLocalizedNameFlags)sourceType);

            UInt32 nameSizeIn = 0;
            UInt32 nameSizeOut = 0;
            char[] buffer = new char[0];

            XrResult result = xrGetInputSourceLocalizedName(m_XrSession, ref nameInfo, nameSizeIn, ref nameSizeOut, buffer);
            sb.Clear().Append(LOG_TAG).Append(func)
                .Append("1.xrGetInputSourceLocalizedName(").Append(userPath).Append(") result: ").Append(result)
                .Append(", flag: ").Append((UInt64)nameInfo.whichComponents)
                .Append(", bufferCapacityInput: ").Append(nameSizeIn)
                .Append(", bufferCountOutput: ").Append(nameSizeOut);
            DEBUG(sb);
            if (result == XrResult.XR_SUCCESS)
            {
                if (nameSizeOut < 1)
                {
                    sb.Clear().Append(LOG_TAG).Append(func)
                        .Append("xrGetInputSourceLocalizedName(").Append(userPath).Append(")")
                        .Append(", flag: ").Append((UInt64)sourceType)
                        .Append("bufferCountOutput size is invalid!");
                    ERROR(sb);
                    return XrResult.XR_ERROR_VALIDATION_FAILURE;
                }

                nameSizeIn = nameSizeOut;
                buffer = new char[nameSizeIn];

                result = xrGetInputSourceLocalizedName(m_XrSession, ref nameInfo, nameSizeIn, ref nameSizeOut, buffer);
                sb.Clear().Append(LOG_TAG).Append(func)
                    .Append("2.xrGetInputSourceLocalizedName(").Append(userPath).Append(") result: ").Append(result)
                    .Append(", flag: ").Append((UInt64)nameInfo.whichComponents)
                    .Append(", bufferCapacityInput: ").Append(nameSizeIn)
                    .Append(", bufferCountOutput: ").Append(nameSizeOut);
                DEBUG(sb);
                if (result == XrResult.XR_SUCCESS)
                {
                    sourceName = new string(buffer).TrimEnd('\0');
                }
            }

            return result;
        }

#if DEBUG_CODE
        XrInputSourceLocalizedNameGetInfo nameInfo;
        private bool updateSerialNumber = true, updateUsage = false;
        private bool UpdateTrackerMaps(string product, XrPath path, ref Dictionary<string, string> serialMap, ref Dictionary<string, string> roleMap)
        {
            string func = "UpdateTrackerMaps() ";
            string s_path = PathToString(path);
            sb.Clear().Append(LOG_TAG).Append(func).Append("product: ").Append(product).Append(", path: ").Append(s_path); DEBUG(sb);

            // -------------------- Tracker Serial Number --------------------
            if (updateSerialNumber)
            {
                nameInfo.type = XrStructureType.XR_TYPE_INPUT_SOURCE_LOCALIZED_NAME_GET_INFO;
                nameInfo.next = IntPtr.Zero;
                nameInfo.sourcePath = path;
                nameInfo.whichComponents = XrInputSourceLocalizedNameFlags.XR_INPUT_SOURCE_LOCALIZED_NAME_SERIAL_NUMBER_BIT_HTC;

                XrResult result = OpenXRHelper.GetInputSourceName(
                    xrGetInputSourceLocalizedName,
                    m_XrSession,
                    ref nameInfo,
                    out string sn);

                sb.Clear().Append(LOG_TAG).Append(func).Append("GetInputSourceName(").Append(s_path).Append(")")
                    .Append(", sourceType: ").Append(nameInfo.whichComponents)
                    .Append(", serial number: ").Append(sn)
                    .Append(", result: ").Append(result);
                DEBUG(sb);

                /// For sample:
                /// A user path (e.g. "/user/tracker_htc/index0") has the "sourceName" kUltimateTrackerSN0 ("VIVE_Ultimate_Tracker_0") which means
                /// the corresponding product (e.g. kProductUltimateTracker0 = "VIVE Ultimate Tracker 0") has the "sourceName" kUltimateTrackerSN0.
                /// So we have to set the serial of the product name kProductUltimateTracker0 to kUltimateTrackerSN0.
                if (result == XrResult.XR_SUCCESS)
                {
                    if (!serialMap.ContainsKey(product))
                        serialMap.Add(product, sn);
                    else
                        serialMap[product] = sn;

                    sb.Clear().Append(LOG_TAG).Append(func).Append("Sets product ").Append(product).Append(" with serial number ").Append(serialMap[product]); DEBUG(sb);
                }
            }
            // -------------------- Tracker Role --------------------
            if (updateUsage)
            {
                nameInfo.type = XrStructureType.XR_TYPE_INPUT_SOURCE_LOCALIZED_NAME_GET_INFO;
                nameInfo.next = IntPtr.Zero;
                nameInfo.sourcePath = path;
                nameInfo.whichComponents = XrInputSourceLocalizedNameFlags.XR_INPUT_SOURCE_LOCALIZED_NAME_USER_PATH_BIT;

                XrResult result = OpenXRHelper.GetInputSourceName(
                    xrGetInputSourceLocalizedName,
                    m_XrSession,
                    ref nameInfo,
                    out string role);

                sb.Clear().Append(LOG_TAG).Append(func).Append("GetInputSourceName(").Append(s_path).Append(")")
                    .Append(", sourceType: ").Append(nameInfo.whichComponents)
                    .Append(", role: ").Append(role)
                    .Append(", result: ").Append(result);
                DEBUG(sb);

                if (result == XrResult.XR_SUCCESS)
                {
                    /// For sample:
                    /// A user path (e.g. "/user/tracker_htc/index0") has the "sourceName" kTrackerRoleLeftWrist ("Left Wrist") which means
                    /// the corresponding product (e.g. kProductUltimateTracker0 = "VIVE Ultimate Tracker 0") has the "sourceName" kTrackerRoleLeftWrist.
                    /// So we have to set the usage of the product name kProductUltimateTracker0 to kTrackerRoleLeftWrist.
                    if (!roleMap.ContainsKey(product))
                        roleMap.Add(product, role);
                    else
                        roleMap[product] = role;

                    sb.Clear().Append(LOG_TAG).Append(func).Append("Sets product ").Append(product).Append(" with usage ").Append(roleMap[product]); DEBUG(sb);
                }
            }

            return true;
        }

        /// <summary>
        /// Due to "ActionMap must be added from within the RegisterActionMapsWithRuntime method", this function is unusable.
        /// </summary>
        /// <param name="product">A tracker's product name.</param>
        private void UpdateInputDeviceUltimateTracker(string product)
        {
            if (!IsUltimateTracker(product)) { return; }
            string func = "UpdateInputDeviceUltimateTracker() ";

            sb.Clear().Append(LOG_TAG).Append(func)
                .Append("product: ").Append(product)
                .Append(", serial number: ").Append(m_UltimateTrackerSerialMap[product])
                .Append(", user path: ").Append(m_UltimateTrackerPathMap[product])
                .Append(", usage: ").Append(m_UltimateTrackerUsageMap[product]);
            DEBUG(sb);

            XrPath path = StringToPath(m_UltimateTrackerPathMap[product]);
            /// Updates tracker serial number (m_UltimateTrackerSerialMap) and role (m_UltimateTrackerUsageMap)
            if (UpdateTrackerMaps(product, path, ref m_UltimateTrackerSerialMap, ref m_UltimateTrackerUsageMap))
            {
                sb.Clear().Append(LOG_TAG).Append(func)
                    .Append("Maps of ").Append(product)
                    .Append(" with user path ").Append(m_UltimateTrackerPathMap[product]).Append(" are updated.");
                DEBUG(sb);

                bool foundProduct = false;
                for (int i = 0; i < InputSystem.devices.Count; i++)
                {
                    if (InputSystem.devices[i] is XrTrackerDevice &&
                        InputSystem.devices[i].description.product.Equals(product))
                    {
                        sb.Clear().Append(LOG_TAG).Append(func)
                            .Append("Removes the XrTrackerDevice product ").Append(product);
                        DEBUG(sb);

                        InputSystem.RemoveDevice(InputSystem.devices[i]);
                        foundProduct = true;
                        break;
                    }
                }
                if (foundProduct)
                {
                    sb.Clear().Append(LOG_TAG).Append(func).Append("Adds a XrTrackerDevice product ").Append(product); DEBUG(sb);
                    RegisterActionMap(
                        product: product,
                        in_name: m_UltimateTrackerActionMap[product],
                        in_sn: m_UltimateTrackerSerialMap[product],
                        in_path: m_UltimateTrackerPathMap[product]
                    );
                }
            }
        }
#endif
    }
}
