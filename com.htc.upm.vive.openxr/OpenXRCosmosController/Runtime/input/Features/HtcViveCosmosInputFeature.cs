using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;
   
   
namespace UnityEngine.XR.OpenXR.Features
{ 
    /// <summary>
    /// This <see cref="OpenXRInteractionFeature"/> enables the use of HTC Vive Controllers interaction profiles in OpenXR.
    /// </summary>
#if UNITY_EDITOR  
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "HTC Vive Cosmos Controller Support",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
        Company = "HTC",
        Desc = "Allows for mapping input to the HTC Vive Cosmos Controller interaction profile.",
        DocumentationLink = "https://developer.vive.com/resources/openxr/openxr-pcvr/tutorials/unity/cosmos-controller-openxr-feature-unity/",
        OpenxrExtensionStrings = "XR_HTC_vive_cosmos_controller_interaction XR_EXT_palm_pose",
        Version = "0.0.2",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction,
		FeatureId = featureId)]
#endif
    public class HtcViveCosmosInputFeature : OpenXRInteractionFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.htc.openxr.feature.input.htcvivecosmos";

        /// <summary>
        /// An Input System device based off the <see cref="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_vive_cosmos_controller_interaction">HTC Vive Controller</see>.
        /// </summary>
        ///
        [Preserve, InputControlLayout(displayName = "HTC Vive Cosmos Controller (OpenXR)", commonUsages = new[] { "LeftHand", "RightHand" })]
        public class ViveCosmosController : XRControllerWithRumble
        {
            /// <summary>
            /// A <see cref="AxisControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.squeeze"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripAxis" })]
            public AxisControl grip { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.squeeze"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripButton" })]
            public ButtonControl gripPressed { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing the <see cref="HtcViveCosmosInputFeature.b"/> <see cref="HtcViveCosmosInputFeature.y"/> OpenXR bindings, depending on handedness.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "B", "Y" })]
            public ButtonControl secondaryButton { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing the <see cref="HtcViveCosmosInputFeature.a"/> <see cref="HtcViveCosmosInputFeature.x"/> OpenXR bindings, depending on handedness.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "A", "X" })]
            public ButtonControl primaryButton { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.shoulder"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "ShoulderButton" })]
            public ButtonControl shoulderPressed { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.menu"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Primary", "menubutton" })]
            public ButtonControl menu { get; private set; }

            /// <summary>
            /// A <see cref="AxisControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.trigger"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "triggeraxis" })]
            public AxisControl trigger { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.triggerClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "triggerbutton" })]
            public ButtonControl triggerPressed { get; private set; }

            /// <summary>
            /// A <see cref="Vector2Control"/> representing information from the <see cref="HtcViveCosmosInputFeature.thumbstick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Primary2DAxis", "joystickaxes" })]
            public Vector2Control joystick { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.thumbstickClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "joystickorpadpressed", "joystickpressed" })]
            public ButtonControl joystickClicked { get; private set; }

            /// <summary>
            /// A <see cref="ButtonControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.thumbstickTouch"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "joystickorpadtouched", "joysticktouched" })]
            public ButtonControl joystickTouched { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.grip"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, alias = "device")]
            public PoseControl devicePose { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.aim"/> OpenXR binding.
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

            /// <summary>
            /// A <see cref="Vector3Control"/> required for back compatibility with the XRSDK layouts. this is the pointer position. this value is equivalent to mapping pointerPose/position
            /// </summary>
            [Preserve, InputControl(offset = 96)]
            public Vector3Control pointerPosition { get; private set; }

            /// <summary>
            /// A <see cref="QuaternionControl"/> required for back compatibility with the XRSDK layouts. this is the pointer rotation. this value is equivalent to mapping pointerPose/rotation
            /// </summary>
            [Preserve, InputControl(offset = 108, aliases = new[] { "pointerOrientation" })]
            public QuaternionControl pointerRotation { get; private set; }
            /// <summary>
            /// A <see cref="HapticControl"/> that represents the <see cref="HtcViveCosmosInputFeature.haptic"/> binding.
            /// </summary>
            [Preserve, InputControl(usage = "Haptic")]
            public HapticControl haptic { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> representing information from the <see cref="HtcViveCosmosInputFeature.palm"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl()]
            public PoseControl palm { get; private set; }


            /// <inheritdoc/>
            protected override void FinishSetup()
            {
                base.FinishSetup();
                grip = GetChildControl<AxisControl>("grip");
                gripPressed = GetChildControl<ButtonControl>("gripPressed");
                primaryButton = GetChildControl<ButtonControl>("primaryButton");
                secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
                shoulderPressed = GetChildControl<ButtonControl>("shoulderPressed");
                menu = GetChildControl<ButtonControl>("menu");
                trigger = GetChildControl<AxisControl>("trigger");
                triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
                joystick = GetChildControl<Vector2Control>("joystick");
                joystickClicked = GetChildControl<ButtonControl>("joystickClicked");
                joystickTouched = GetChildControl<ButtonControl>("joystickTouched");

                devicePose = GetChildControl<PoseControl>("devicePose");
                pointer = GetChildControl<PoseControl>("pointer");

                isTracked = GetChildControl<ButtonControl>("isTracked");
                trackingState = GetChildControl<IntegerControl>("trackingState");
                devicePosition = GetChildControl<Vector3Control>("devicePosition");
                deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
                pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
                pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");
                haptic = GetChildControl<HapticControl>("haptic");
                palm = GetChildControl<PoseControl>("palm");
            }
        }
        /// <summary>The interaction profile string used to reference the <see cref="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_vive_cosmos_controller_interaction">HTC Vive Controller</see>.</summary>
        public const string profile = "/interaction_profiles/htc/vive_cosmos_controller";

        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/trigger/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string select = "/input/trigger/click";

        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/a/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string a = "/input/a/click";

        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/b/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string b = "/input/b/click";

        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/x/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string x = "/input/x/click";

        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/y/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string y = "/input/y/click";


        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/y/shoulder' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string shoulder = "/input/shoulder/click";

        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/squeeze/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string squeeze = "/input/squeeze/click";
        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/menu/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string menu = "/input/menu/click";
        /// <summary>
        /// Constant for a <see cref="ActionType.Axis1D"/> interaction binding '.../input/trigger/value' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string trigger = "/input/trigger/value";
        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/trigger/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string triggerClick = "/input/trigger/click";
        /// <summary>
        /// Constant for a <see cref="ActionType.Axis2D"/> interaction binding '.../input/trackpad' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string thumbstick = "/input/thumbstick";
        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/trackpad/click' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string thumbstickClick = "/input/thumbstick/click";
        /// <summary>
        /// Constant for a <see cref="ActionType.Binary"/> interaction binding '.../input/trackpad/touch' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string thumbstickTouch = "/input/thumbstick/touch";
        /// <summary>
        /// Constant for a <see cref="ActionType.Pose"/> interaction binding '.../input/grip/pose' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string grip = "/input/grip/pose";
        /// <summary>
        /// Constant for a <see cref="ActionType.Pose"/> interaction binding '.../input/aim/pose' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string aim = "/input/aim/pose";
        /// <summary>
        /// Constant for a <see cref="ActionType.Vibrate"/> interaction binding '.../output/haptic' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string haptic = "/output/haptic";

        /// <summary>
        /// Constant for a <see cref="ActionType.Pose"/> interaction binding '.../input/palm_ext/pose' OpenXR Input Binding. Used by <see cref="XrActionConfig"/> to bind actions to physical inputs.
        /// </summary>
        public const string palm = "/input/palm_ext/pose";


        private const string kDeviceLocalizedName = "HTC Vive Cosmos Controller OpenXR";

        /// <summary>
        /// Registers the <see cref="ViveCosmosController"/> layout with the Input System. Matches the <see cref="ActionMapConfig"/> that is registered with <see cref="RegisterControllerMap"/>.
        /// </summary>
        protected override void RegisterDeviceLayout()
        {
            InputSystem.InputSystem.RegisterLayout(typeof(ViveCosmosController),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedName));
        }


        /// <summary>
        /// Removes the <see cref="ViveCosmosController"/> layout from the Input System. Matches the <see cref="ActionMapConfig"/> that is registered with <see cref="RegisterControllerMap"/>.
        /// </summary>
        protected override void UnregisterDeviceLayout()
        {
            InputSystem.InputSystem.RemoveLayout(typeof(ViveCosmosController).Name);
        }

        /// <summary>
        /// Registers an <see cref="ActionMapConfig"/> with OpenXR that matches the <see cref="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_vive_cosmos_controller_interaction">HTC Vive Controller</see>. Also calls <see cref="RegisterDeviceLayout"/> when the Input System package is available.
        /// </summary>
        protected override void RegisterActionMapsWithRuntime()
        {
            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = "vivecosmoscontroller",
                localizedName = kDeviceLocalizedName,
                desiredInteractionProfile = profile,
                manufacturer = "HTC",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left),
                        userPath = UserPaths.leftHand
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right),
                        userPath = UserPaths.rightHand
                    }
                },
                actions = new List<ActionConfig>()
                {
                    new ActionConfig()
                    {
                        name = "primarybutton",
                        localizedName = "Primary Button",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "PrimaryButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                             new ActionBinding()
                            {
                                interactionPath = x,
                                interactionProfileName = profile,
                                userPaths = new List<string>() { UserPaths.leftHand }
                            },
                            new ActionBinding()
                            {
                                interactionPath = a,
                                interactionProfileName = profile,
                                userPaths = new List<string>() { UserPaths.rightHand }
                            },
                        }
                    },
                     new ActionConfig()
                    {
                        name = "secondarybutton",
                        localizedName = "Secondary Button",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "SecondaryButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                             new ActionBinding()
                            {
                                interactionPath = y,
                                interactionProfileName = profile,
                                userPaths = new List<string>() { UserPaths.leftHand }
                            },
                            new ActionBinding()
                            {
                                interactionPath = b,
                                interactionProfileName = profile,
                                userPaths = new List<string>() { UserPaths.rightHand }
                            },
                        }
                    },
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
                        name = "grippressed",
                        localizedName = "Grip Pressed",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "GripPressed",
                            "GripButton"
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
                                userPaths = new List<string>() { UserPaths.leftHand }
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "shoulderpressed",
                        localizedName = "Shoulder Pressed",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "ShoulderButton"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = shoulder,
                                interactionProfileName = profile,
                            }
                        }
                    }, 
                    new ActionConfig()
                    {
                        name = "trigger",
                        localizedName = "Trigger",
                        type = ActionType.Axis1D,
                        usages = new List<string>()
                        {
                            "Trigger"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = trigger,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "triggerpressed",
                        localizedName = "Trigger Pressed",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "TriggerPressed",
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
                    new ActionConfig()
                    {
                        name = "thumbstick",
                        localizedName = "Thumbstick",
                        type = ActionType.Axis2D,
                        usages = new List<string>()
                        {
                            "Primary2DAxis"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = thumbstick,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "thumbsticktouched",
                        localizedName = "Thumbstick Touched",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "Primary2DAxisTouch"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = thumbstickTouch,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "thumbstickclicked",
                        localizedName = "Thumbstick Clicked",
                        type = ActionType.Binary,
                        usages = new List<string>()
                        {
                            "Primary2DAxisClick"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = thumbstickClick,
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
                    },
                    new ActionConfig()
                    {
                        name = "haptic",
                        localizedName = "Haptic Output",
                        type = ActionType.Vibrate,
                        usages = new List<string>(){ "Haptic" },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = haptic,
                                interactionProfileName = profile,
                            }
                        }
                    },
                    new ActionConfig()
                    {
                        name = "palm",
                        localizedName = "Palm Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Palm"
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = palm,
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