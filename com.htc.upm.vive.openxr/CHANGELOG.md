# **VIVE OpenXR Plugin - Windows** For Unity -  v1.0.13
Copyright HTC Corporation. All Rights Reserved.

**VIVE OpenXR Plugin - Windows**: This plugin provides support for openxr based on the following specifications.
 - [Vive Facial Tracking](https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_facial_tracking)
 - [Vive Cosmos Controller](https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_vive_cosmos_controller_interaction)
 - [Scene Understanding](https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_MSFT_scene_understanding)
 - [Hand Tracking](https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_hand_tracking)
 - [Vive Focus3 Controller](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_vive_focus3_controller_interaction)
 - [Hand Interaction](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_hand_interaction)
 - [Palm pose](https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_palm_pose)
---
## Changes for v1.0.13 - 2023/06/26
 - Fix problem that "OpenXR.Input.PoseControl" and "OpenXR.Input.Pose" are deprecated after OpenXR Plugin 1.6.0. 
   1. When **USE_INPUT_SYSTEM_POSE_CONTROL** is defined, switch to use InputSystem.XR.PoseControl; otherwise, use OpenXR.Input.PoseControl for backward compatibility.
   2. When **USE_INPUT_SYSTEM_POSE_CONTROL** is defined, switch to use InputSystem.XR.PoseState; otherwise, use OpenXR.Input.Pose for backward compatibility.

## Changes for v1.0.12 - 2023/06/02
 - Remove Eye gaze sample.It is recommended to use the Controller sample provided by the Unity OpenXR Plugin to test eye gaze.

## Changes for v1.0.11 - 2023/02/16
 - Add openxr XR_EXT_palm_pose support for Vive Focus3 controller and Vive Cosmos controller.
 - Add Hand Interaction extension support.
 - Add hand interaction demo in hand tracking sample.

## Changes for v1.0.10 - 2023/01/13
 - Add eye gaze sample.

## Changes for v1.0.9 - 2022/11/10
 - Fix the dependency issue with OpenXR plugin.
 

## Changes for v1.0.8 - 2022/10/11
 - Update the package name from **Vive Wave OpenXR Plugin - Windows** to **VIVE OpenXR Plugin - Windows**.
 - Fixed problem that blendshapes and input element columns of facial tracking sample not aligned correctly in Unity Inspector.​
 - Fix haptic problem for cosmos controller profile.
 - Add Focus3 controller extension support.

## Changes for v1.0.7 - 2022/09/26
 - Fixed function type conversion problem when using handtracking feature with other OpenXR features at the same time.

## Changes for v1.0.6 - 2022/09/15
### Vive Hand Tracking
 - Fixed delay problem when locating controller with HandTracking extension.

## Changes for v1.0.5 - 2022/06/24
 - Update documentation links for (1) Vive Facial Tracking (2) Vive Cosmos Controller (3) Scene Understanding (4) Hand Tracking.
 - Refine plugin and sample for (1) Vive Facial Tracking (2) Hand Tracking.
### Vive Hand Tracking
 - Implement extension XR_EXT_hand_joints_motion_range for Hand tracking.
 - Fixed incorrect joint rotation.

### Vive Facial Tracking
 - Fixed incorrect eye gaze direction for sample.

## Changes for v1.0.4 - 2022/4/28:
 - Update the package name from **Vive OpenXR Plugin** to **VIVE Wave OpenXR Plugin - Windows**.
 - Fixed missing material for Hand Tracking sample.
 - Fixed Hand tracking sample crashed issue.
 - Add 3D HandTracking Sample.

## Changes for v1.0.3 - 2022/4/08:
 - Refactor Hand Tracking sample.
 - Fixed build error related to Scene Understanding plugin.

## Changes for v1.0.2 - 2022/3/23:
 - Add support for openxr hand tracking extension.

## Changes for v1.0.1 - 2022/2/10:

### Vive Cosmos Controller
 - Correct the input path of menu key.

### Scene Understanding
 - Move Mesh subsystem from plugin part to sample code.

## Changes for v1.0.0 - 2021/1/06:

* This is the first release of Vive OpenXR Unity Plugin.

