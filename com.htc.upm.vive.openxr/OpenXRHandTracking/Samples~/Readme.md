# VIVE OpenXR Hand Tracking Unity Feature

To help software developers create an application for locating hand joints with the OpenXR hand tracking extension [XR_EXT_hand_tracking](https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_hand_tracking).

## Load sample code
**Window** > **Package Manager** > **VIVE OpenXR Plugin - Windows** > **Samples** > Click to import **HandTracking Example**

## Play the sample scene    
1. **Edit** > **Project Settings** > **XR Plug-in Management** > Select **OpenXR** , click Exclamation mark next to it then choose **Fix All**.
2. **Edit** > **Project Settings** > **XR Plug-in Management** > **OpenXR** > Add Interaction profiles for your device.
3. **Edit** > **Project Settings** > **XR Plug-in Management** > **OpenXR** > Select **Hand Tracking** under **VIVE OpenXR** Feature Groups.
4. In the Unity Project window, select the sample scene file in **Assets** > **Samples** > **VIVE OpenXR Plugin - Windows** > **1.0.13** > **HandTracking Example** > **Scenes** > **HandTrackingScene.unity** then click Play.

## Use VIVE OpenXR Hand Tracking Unity Feature to draw skeleton hand.
1. Import VIVE OpenXR Plugin - Windows
2. Add Hand gameobject to the Unity scene
    - Refer to functions **StartFrameWork** and **StopFrameWork** in **HandManager.cs** for creating and releasing handle for hand.
    - Refer to the function **GetJointLocation** in **HandManager.cs** for getting the information to locate hand joints.
    - Drag "SkeletonHand" prefab into scene hierarchy or Create an empty object and attach **RenderHand.cs**.

## Use VIVE OpenXR Hand Tracking Unity Feature to draw 3D hand.
1. Import VIVE OpenXR Plugin - Windows
2. Add Hand gameobject to the Unity scene
    - Refer to functions **StartFrameWork** and **StopFrameWork** in **HandManager.cs** for creating and releasing handle for hand.
    - Refer to the function **GetJointLocation** in **HandManager.cs** for getting the information to locate hand joints.
    - Drag "3DHand" prefab into scene hierarchy or attach **RenderModel.cs** to "ObjModelHandLeft_26.fbx" and "ObjModelHandRight_26.fbx".