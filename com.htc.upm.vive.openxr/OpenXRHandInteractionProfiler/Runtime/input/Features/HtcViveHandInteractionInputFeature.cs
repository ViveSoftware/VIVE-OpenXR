using System.Collections.Generic;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Input;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_INPUT_SYSTEM_POSE_CONTROL
using PoseControl = UnityEngine.InputSystem.XR.PoseControl;
#else
using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;
#endif

namespace UnityEngine.XR.OpenXR.Features
{
    /// <summary>
    /// This <see cref="OpenXRInteractionFeature"/> enables the use of HTC Vive hand interaction profiles in OpenXR.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "HTC Vive hand interaction Support",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
        Company = "HTC",
        Desc = "Allows for mapping input to the HTC Vive hand interaction interaction profile.",
        DocumentationLink = "https://developer.vive.com/resources/openxr/openxr-pcvr/tutorials/unity/hand-interaction-profile/",
        OpenxrExtensionStrings = "XR_HTC_hand_interaction",
        Version = "0.0.1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction,
        FeatureId = featureId)]
#endif
    public class HtcViveHandInteractionInputFeature : OpenXRInteractionFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.htc.openxr.feature.input.htcvivehandinteraction";

        /// <summary>
        /// An Input System device based off the <see cref="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_hand_interaction">HTC Vive Controller</see>.
        /// </summary>
        ///
        [Preserve, InputControlLayout(displayName = "HTC Vive hand interaction (OpenXR)", commonUsages = new[] { "LeftHand", "RightHand" })]
        public class ViveHandInteraction : XRController
        {
            /// <summary>
            /// A <see cref="AxisControl"/> representing information from the <see cref="HtcViveHandInteraction.squeeze"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripAxis" })]
            public AxisControl grip { get; private set; }
            /// <summary>
            /// A <see cref="AxisControl"/> representing information from the <see cref="HtcViveHandInteraction.select"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "selectaxis" })]
            public AxisControl select { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> representing information from the <see cref="HtcViveHandInteraction.grip"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, alias = "device")]
            public PoseControl devicePose { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> representing information from the <see cref="HtcViveHandInteraction.aim"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0)]
            public PoseControl pointer { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> required for back compatibility with the XRSDK layouts. this represents the overall tracking state of the device. this value is equivalent to mapping devicePose/isTracked
            /// </summary>
            [Preserve, InputControl(offset = 28)]
            new public ButtonControl isTracked { get; private set; }

            /// <summary>
            /// A <see cref="IntegerControl"/> required for back compatibility with the XRSDK layouts. this represents the bit flag set indicating what data is valid. this value is equivalent to mapping devicePose/trackingState
            /// </summary>
            [Preserve, InputControl(offset = 32)]
            new public IntegerControl trackingState { get; private set; }

            /// <summary>
            /// A <see cref="Vector3Control"/> required for back compatibility with the XRSDK layouts. this is the device position, or grip position. this value is equivalent to mapping devicePose/position
            /// </summary>
            [Preserve, InputControl(offset = 36, aliases = new[] { "gripPosition" })]
            new public Vector3Control devicePosition { get; private set; }

            /// <summary>
            /// A <see cref="QuaternionControl"/> required for back compatibility with the XRSDK layouts. this is the device orientation, or grip orientation. this value is equivalent to mapping devicePose/rotation
            /// </summary>
            [Preserve, InputControl(offset = 48, aliases = new[] { "gripOrientation", "gripRotation" })]
            new public QuaternionControl deviceRotation { get; private set; }

            /// A <see cref="Vector3Control"/> required for back compatibility with the XRSDK layouts. this is the pointer position. this value is equivalent to mapping pointerPose/position
            /// </summary>
            [Preserve, InputControl(offset = 96)]
            public Vector3Control pointerPosition { get; private set; }

            /// <summary>
            /// A <see cref="QuaternionControl"/> required for back compatibility with the XRSDK layouts. this is the pointer rotation. this value is equivalent to mapping pointerPose/rotation
            /// </summary>
            [Preserve, InputControl(offset = 108, aliases = new[] { "pointerOrientation" })]
            public QuaternionControl pointerRotation { get; private set; }

            protected override void FinishSetup()
            {
                base.FinishSetup();
                grip = GetChildControl<AxisControl>("grip");
                select = GetChildControl<AxisControl>("select");

                devicePose = GetChildControl<PoseControl>("devicePose");
                pointer = GetChildControl<PoseControl>("pointer");

                isTracked = GetChildControl<ButtonControl>("isTracked");
                trackingState = GetChildControl<IntegerControl>("trackingState");
                devicePosition = GetChildControl<Vector3Control>("devicePosition");
                deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
                pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
                pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");
            }
        }
        /// <summary>The interaction profile string used to reference the <see cref="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_hand_interaction">HTC Vive hand interaction</see>.</summary>
        public const string profile = "/interaction_profiles/htc/hand_interaction";

        /// <summary>
        /// Constant for a <see cref="ActionType.Axis1D"/> interaction binding '.../input/squeeze/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string squeeze = "/input/squeeze/value";
        /// <summary>
        /// Constant for a <see cref="ActionType.Axis1D"/> interaction binding '.../input/select/value' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string select = "/input/select/value";
        /// <summary>
        /// Constant for a <see cref="ActionType.Pose"/> interaction binding '.../input/grip/pose' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string grip = "/input/grip/pose";
        /// <summary>
        /// Constant for a <see cref="ActionType.Pose"/> interaction binding '.../input/aim/pose' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string aim = "/input/aim/pose";

        private const string kDeviceLocalizedName = "HTC Vive hand interaction OpenXR";
        /// <summary>
        /// Registers the <see cref="ViveHandInteraction"/> layout with the Input System. Matches the <see cref="ActionMapConfig"/> that is registered with <see cref="RegisterControllerMap"/>.
        /// </summary>
        protected override void RegisterDeviceLayout()
        {
            InputSystem.InputSystem.RegisterLayout(typeof(ViveHandInteraction),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedName));
        }


        /// <summary>
        /// Removes the <see cref="ViveHandInteraction"/> layout from the Input System. Matches the <see cref="ActionMapConfig"/> that is registered with <see cref="RegisterControllerMap"/>.
        /// </summary>
        protected override void UnregisterDeviceLayout()
        {
            InputSystem.InputSystem.RemoveLayout(typeof(ViveHandInteraction).Name);
        }

        /// <summary>
        /// Registers an <see cref="ActionMapConfig"/> with OpenXR that matches the <see cref="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_hand_interaction">HTC Vive hand interaction</see>. Also calls <see cref="RegisterDeviceLayout"/> when the Input System package is available.
        /// </summary>
        protected override void RegisterActionMapsWithRuntime()
        {
            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = "ViveHandInteraction",
                localizedName = kDeviceLocalizedName,
                desiredInteractionProfile = profile,
                manufacturer = "HTC",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Left),
                        userPath = "/user/hand_htc/left"
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Right),
                        userPath = "/user/hand_htc/right"
                    }
                },
                actions = new List<ActionConfig>()
                {
                    new ActionConfig()
                    {
                        name = "grip",
                        localizedName = "Grip",
                        type = ActionType.Axis1D,
                        usages = new List<string>()
                        {
                            "Grip"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = squeeze,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "select",
                        localizedName = "Select",
                        type = ActionType.Axis1D,
                        usages = new List<string>()
                        {
                            "Select"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = select,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "devicepose",
                        localizedName = "Device Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = grip,
                                interactionProfileName = profile,
                            }
                        }
                    },
                     new ActionConfig()
                    {
                        name = "pointer",
                        localizedName = "Pointer Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Pointer"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = aim, 
                                interactionProfileName = profile,
                            }
                        }
                    }
                }
            };
            AddActionMap(actionMap);
        }
    }
}