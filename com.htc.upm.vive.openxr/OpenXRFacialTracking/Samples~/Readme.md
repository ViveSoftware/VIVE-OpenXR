# VIVE OpenXR Facial Tracking Unity Feature

To help software developers create an application with actual facial expressions on 3D avatars with the OpenXR facial tracing extension [XR_HTC_facial_tracking](https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_facial_tracking).

## Load sample code
**Window** > **Package Manager** > **VIVE OpenXR Plugin - Windows** > **Samples** > Click to import **FacialTracking Example**

## Play the sample scene    
1. **Edit** > **Project Settings** > **XR Plug-in Management** > Select **OpenXR** , click Exclamation mark next to it then choose **Fix All**.
2. **Edit** > **Project Settings** > **XR Plug-in Management** > **OpenXR** > Add Interaction profiles for your device.
3. **Edit** > **Project Settings** > **XR Plug-in Management** > **OpenXR** > Select **Facial Tracking** under **VIVE OpenXR** Feature Groups.
4. In the Unity Project window, select the sample scene file in **Assets** > **Samples** > **VIVE OpenXR Plugin - Windows** > **1.0.13** > **FacialTracking Example** > **Scenes** > **FaceSample.unity** then click Play.

## How to use VIVE OpenXR Facial Tracking Unity Feature
1. Import VIVE OpenXR Plugin - Windows
2. Add your avatar object to the Unity scene.
    - Attach "AvatarEyeSample.cs" and "AvatarLipSample.cs" to your avatar object or Drag "Avatar_Shieh_V2" prefab into scene hierarchy.
    - Refer to functions **StartFrameWork** and **StopFrameWork** in **FacialManager.cs** for creating and releasing handle for face.
    - Refer to the function **GetWeightings** in **FacialManager.cs** for getting the weightings of blendshapes.