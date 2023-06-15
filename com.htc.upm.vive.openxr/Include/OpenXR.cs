using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VIVE
{
    delegate int xrGetInstanceProcDelegate(ulong instance, string name, out IntPtr function);

    public enum XrStructureType
    {
        XR_TYPE_UNKNOWN = 0,
        XR_TYPE_API_LAYER_PROPERTIES = 1,
        XR_TYPE_EXTENSION_PROPERTIES = 2,
        XR_TYPE_INSTANCE_CREATE_INFO = 3,
        XR_TYPE_SYSTEM_GET_INFO = 4,
        XR_TYPE_SYSTEM_PROPERTIES = 5,
        XR_TYPE_VIEW_LOCATE_INFO = 6,
        XR_TYPE_VIEW = 7,
        XR_TYPE_SESSION_CREATE_INFO = 8,
        XR_TYPE_SWAPCHAIN_CREATE_INFO = 9,
        XR_TYPE_SESSION_BEGIN_INFO = 10,
        XR_TYPE_VIEW_STATE = 11,
        XR_TYPE_FRAME_END_INFO = 12,
        XR_TYPE_HAPTIC_VIBRATION = 13,
        XR_TYPE_EVENT_DATA_BUFFER = 16,
        XR_TYPE_EVENT_DATA_INSTANCE_LOSS_PENDING = 17,
        XR_TYPE_EVENT_DATA_SESSION_STATE_CHANGED = 18,
        XR_TYPE_ACTION_STATE_BOOLEAN = 23,
        XR_TYPE_ACTION_STATE_FLOAT = 24,
        XR_TYPE_ACTION_STATE_VECTOR2F = 25,
        XR_TYPE_ACTION_STATE_POSE = 27,
        XR_TYPE_ACTION_SET_CREATE_INFO = 28,
        XR_TYPE_ACTION_CREATE_INFO = 29,
        XR_TYPE_INSTANCE_PROPERTIES = 32,
        XR_TYPE_FRAME_WAIT_INFO = 33,
        XR_TYPE_COMPOSITION_LAYER_PROJECTION = 35,
        XR_TYPE_COMPOSITION_LAYER_QUAD = 36,
        XR_TYPE_REFERENCE_SPACE_CREATE_INFO = 37,
        XR_TYPE_ACTION_SPACE_CREATE_INFO = 38,
        XR_TYPE_EVENT_DATA_REFERENCE_SPACE_CHANGE_PENDING = 40,
        XR_TYPE_VIEW_CONFIGURATION_VIEW = 41,
        XR_TYPE_SPACE_LOCATION = 42,
        XR_TYPE_SPACE_VELOCITY = 43,
        XR_TYPE_FRAME_STATE = 44,
        XR_TYPE_VIEW_CONFIGURATION_PROPERTIES = 45,
        XR_TYPE_FRAME_BEGIN_INFO = 46,
        XR_TYPE_COMPOSITION_LAYER_PROJECTION_VIEW = 48,
        XR_TYPE_EVENT_DATA_EVENTS_LOST = 49,
        XR_TYPE_INTERACTION_PROFILE_SUGGESTED_BINDING = 51,
        XR_TYPE_EVENT_DATA_INTERACTION_PROFILE_CHANGED = 52,
        XR_TYPE_INTERACTION_PROFILE_STATE = 53,
        XR_TYPE_SWAPCHAIN_IMAGE_ACQUIRE_INFO = 55,
        XR_TYPE_SWAPCHAIN_IMAGE_WAIT_INFO = 56,
        XR_TYPE_SWAPCHAIN_IMAGE_RELEASE_INFO = 57,
        XR_TYPE_ACTION_STATE_GET_INFO = 58,
        XR_TYPE_HAPTIC_ACTION_INFO = 59,
        XR_TYPE_SESSION_ACTION_SETS_ATTACH_INFO = 60,
        XR_TYPE_ACTIONS_SYNC_INFO = 61,
        XR_TYPE_BOUND_SOURCES_FOR_ACTION_ENUMERATE_INFO = 62,
        XR_TYPE_INPUT_SOURCE_LOCALIZED_NAME_GET_INFO = 63,
        XR_TYPE_COMPOSITION_LAYER_CUBE_KHR = 1000006000,
        XR_TYPE_INSTANCE_CREATE_INFO_ANDROID_KHR = 1000008000,
        XR_TYPE_COMPOSITION_LAYER_DEPTH_INFO_KHR = 1000010000,
        XR_TYPE_VULKAN_SWAPCHAIN_FORMAT_LIST_CREATE_INFO_KHR = 1000014000,
        XR_TYPE_EVENT_DATA_PERF_SETTINGS_EXT = 1000015000,
        XR_TYPE_COMPOSITION_LAYER_CYLINDER_KHR = 1000017000,
        XR_TYPE_COMPOSITION_LAYER_EQUIRECT_KHR = 1000018000,
        XR_TYPE_DEBUG_UTILS_OBJECT_NAME_INFO_EXT = 1000019000,
        XR_TYPE_DEBUG_UTILS_MESSENGER_CALLBACK_DATA_EXT = 1000019001,
        XR_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT = 1000019002,
        XR_TYPE_DEBUG_UTILS_LABEL_EXT = 1000019003,
        XR_TYPE_GRAPHICS_BINDING_OPENGL_WIN32_KHR = 1000023000,
        XR_TYPE_GRAPHICS_BINDING_OPENGL_XLIB_KHR = 1000023001,
        XR_TYPE_GRAPHICS_BINDING_OPENGL_XCB_KHR = 1000023002,
        XR_TYPE_GRAPHICS_BINDING_OPENGL_WAYLAND_KHR = 1000023003,
        XR_TYPE_SWAPCHAIN_IMAGE_OPENGL_KHR = 1000023004,
        XR_TYPE_GRAPHICS_REQUIREMENTS_OPENGL_KHR = 1000023005,
        XR_TYPE_GRAPHICS_BINDING_OPENGL_ES_ANDROID_KHR = 1000024001,
        XR_TYPE_SWAPCHAIN_IMAGE_OPENGL_ES_KHR = 1000024002,
        XR_TYPE_GRAPHICS_REQUIREMENTS_OPENGL_ES_KHR = 1000024003,
        XR_TYPE_GRAPHICS_BINDING_VULKAN_KHR = 1000025000,
        XR_TYPE_SWAPCHAIN_IMAGE_VULKAN_KHR = 1000025001,
        XR_TYPE_GRAPHICS_REQUIREMENTS_VULKAN_KHR = 1000025002,
        XR_TYPE_GRAPHICS_BINDING_D3D11_KHR = 1000027000,
        XR_TYPE_SWAPCHAIN_IMAGE_D3D11_KHR = 1000027001,
        XR_TYPE_GRAPHICS_REQUIREMENTS_D3D11_KHR = 1000027002,
        XR_TYPE_GRAPHICS_BINDING_D3D12_KHR = 1000028000,
        XR_TYPE_SWAPCHAIN_IMAGE_D3D12_KHR = 1000028001,
        XR_TYPE_GRAPHICS_REQUIREMENTS_D3D12_KHR = 1000028002,
        XR_TYPE_SYSTEM_EYE_GAZE_INTERACTION_PROPERTIES_EXT = 1000030000,
        XR_TYPE_EYE_GAZE_SAMPLE_TIME_EXT = 1000030001,
        XR_TYPE_VISIBILITY_MASK_KHR = 1000031000,
        XR_TYPE_EVENT_DATA_VISIBILITY_MASK_CHANGED_KHR = 1000031001,
        XR_TYPE_SESSION_CREATE_INFO_OVERLAY_EXTX = 1000033000,
        XR_TYPE_EVENT_DATA_MAIN_SESSION_VISIBILITY_CHANGED_EXTX = 1000033003,
        XR_TYPE_COMPOSITION_LAYER_COLOR_SCALE_BIAS_KHR = 1000034000,
        XR_TYPE_SPATIAL_ANCHOR_CREATE_INFO_MSFT = 1000039000,
        XR_TYPE_SPATIAL_ANCHOR_SPACE_CREATE_INFO_MSFT = 1000039001,
        XR_TYPE_VIEW_CONFIGURATION_DEPTH_RANGE_EXT = 1000046000,
        XR_TYPE_GRAPHICS_BINDING_EGL_MNDX = 1000048004,
        XR_TYPE_SPATIAL_GRAPH_NODE_SPACE_CREATE_INFO_MSFT = 1000049000,
        XR_TYPE_SYSTEM_HAND_TRACKING_PROPERTIES_EXT = 1000051000,
        XR_TYPE_HAND_TRACKER_CREATE_INFO_EXT = 1000051001,
        XR_TYPE_HAND_JOINTS_LOCATE_INFO_EXT = 1000051002,
        XR_TYPE_HAND_JOINT_LOCATIONS_EXT = 1000051003,
        XR_TYPE_HAND_JOINT_VELOCITIES_EXT = 1000051004,
        XR_TYPE_SYSTEM_HAND_TRACKING_MESH_PROPERTIES_MSFT = 1000052000,
        XR_TYPE_HAND_MESH_SPACE_CREATE_INFO_MSFT = 1000052001,
        XR_TYPE_HAND_MESH_UPDATE_INFO_MSFT = 1000052002,
        XR_TYPE_HAND_MESH_MSFT = 1000052003,
        XR_TYPE_HAND_POSE_TYPE_INFO_MSFT = 1000052004,
        XR_TYPE_SECONDARY_VIEW_CONFIGURATION_SESSION_BEGIN_INFO_MSFT = 1000053000,
        XR_TYPE_SECONDARY_VIEW_CONFIGURATION_STATE_MSFT = 1000053001,
        XR_TYPE_SECONDARY_VIEW_CONFIGURATION_FRAME_STATE_MSFT = 1000053002,
        XR_TYPE_SECONDARY_VIEW_CONFIGURATION_FRAME_END_INFO_MSFT = 1000053003,
        XR_TYPE_SECONDARY_VIEW_CONFIGURATION_LAYER_INFO_MSFT = 1000053004,
        XR_TYPE_SECONDARY_VIEW_CONFIGURATION_SWAPCHAIN_CREATE_INFO_MSFT = 1000053005,
        XR_TYPE_CONTROLLER_MODEL_KEY_STATE_MSFT = 1000055000,
        XR_TYPE_CONTROLLER_MODEL_NODE_PROPERTIES_MSFT = 1000055001,
        XR_TYPE_CONTROLLER_MODEL_PROPERTIES_MSFT = 1000055002,
        XR_TYPE_CONTROLLER_MODEL_NODE_STATE_MSFT = 1000055003,
        XR_TYPE_CONTROLLER_MODEL_STATE_MSFT = 1000055004,
        XR_TYPE_VIEW_CONFIGURATION_VIEW_FOV_EPIC = 1000059000,
        XR_TYPE_HOLOGRAPHIC_WINDOW_ATTACHMENT_MSFT = 1000063000,
        XR_TYPE_ANDROID_SURFACE_SWAPCHAIN_CREATE_INFO_FB = 1000070000,
        XR_TYPE_SWAPCHAIN_STATE_ANDROID_SURFACE_DIMENSIONS_FB = 1000071000,
        XR_TYPE_SWAPCHAIN_STATE_SAMPLER_OPENGL_ES_FB = 1000071001,
        XR_TYPE_INTERACTION_PROFILE_ANALOG_THRESHOLD_VALVE = 1000079000,
        XR_TYPE_HAND_JOINTS_MOTION_RANGE_INFO_EXT = 1000080000,
        XR_TYPE_LOADER_INIT_INFO_ANDROID_KHR = 1000089000,
        XR_TYPE_VULKAN_INSTANCE_CREATE_INFO_KHR = 1000090000,
        XR_TYPE_VULKAN_DEVICE_CREATE_INFO_KHR = 1000090001,
        XR_TYPE_VULKAN_GRAPHICS_DEVICE_GET_INFO_KHR = 1000090003,
        XR_TYPE_COMPOSITION_LAYER_EQUIRECT2_KHR = 1000091000,
        XR_TYPE_SCENE_OBSERVER_CREATE_INFO_MSFT = 1000097000,
        XR_TYPE_SCENE_CREATE_INFO_MSFT = 1000097001,
        XR_TYPE_NEW_SCENE_COMPUTE_INFO_MSFT = 1000097002,
        XR_TYPE_VISUAL_MESH_COMPUTE_LOD_INFO_MSFT = 1000097003,
        XR_TYPE_SCENE_COMPONENTS_MSFT = 1000097004,
        XR_TYPE_SCENE_COMPONENTS_GET_INFO_MSFT = 1000097005,
        XR_TYPE_SCENE_COMPONENT_LOCATIONS_MSFT = 1000097006,
        XR_TYPE_SCENE_COMPONENTS_LOCATE_INFO_MSFT = 1000097007,
        XR_TYPE_SCENE_OBJECTS_MSFT = 1000097008,
        XR_TYPE_SCENE_COMPONENT_PARENT_FILTER_INFO_MSFT = 1000097009,
        XR_TYPE_SCENE_OBJECT_TYPES_FILTER_INFO_MSFT = 1000097010,
        XR_TYPE_SCENE_PLANES_MSFT = 1000097011,
        XR_TYPE_SCENE_PLANE_ALIGNMENT_FILTER_INFO_MSFT = 1000097012,
        XR_TYPE_SCENE_MESHES_MSFT = 1000097013,
        XR_TYPE_SCENE_MESH_BUFFERS_GET_INFO_MSFT = 1000097014,
        XR_TYPE_SCENE_MESH_BUFFERS_MSFT = 1000097015,
        XR_TYPE_SCENE_MESH_VERTEX_BUFFER_MSFT = 1000097016,
        XR_TYPE_SCENE_MESH_INDICES_UINT32_MSFT = 1000097017,
        XR_TYPE_SCENE_MESH_INDICES_UINT16_MSFT = 1000097018,
        XR_TYPE_SERIALIZED_SCENE_FRAGMENT_DATA_GET_INFO_MSFT = 1000098000,
        XR_TYPE_SCENE_DESERIALIZE_INFO_MSFT = 1000098001,
        XR_TYPE_EVENT_DATA_DISPLAY_REFRESH_RATE_CHANGED_FB = 1000101000,
        XR_TYPE_SYSTEM_PASS_THROUGH_PROPERTIES_HTC = 1000103000,
        XR_TYPE_PASS_THROUGH_CREATE_INFO_HTC = 1000103001,
        XR_TYPE_PASS_THROUGH_FRAME_HTC = 1000103002,
        XR_TYPE_PASS_THROUGH_ACQUIRE_INFO_HTC = 1000103003,
        XR_TYPE_EVENT_DATA_RUNTIME_EVENT_HTC = 1000103004,
        XR_TYPE_SYSTEM_FACIAL_TRACKING_PROPERTIES_HTC = 1000104000,
        XR_TYPE_FACIAL_TRACKER_CREATE_INFO_HTC = 1000104001,
        XR_TYPE_FACIAL_EXPRESSIONS_HTC = 1000104002,
        XR_TYPE_SYSTEM_COLOR_SPACE_PROPERTIES_FB = 1000108000,
        XR_TYPE_BINDING_MODIFICATIONS_KHR = 1000120000,
        XR_TYPE_VIEW_LOCATE_FOVEATED_RENDERING_VARJO = 1000121000,
        XR_TYPE_FOVEATED_VIEW_CONFIGURATION_VIEW_VARJO = 1000121001,
        XR_TYPE_SYSTEM_FOVEATED_RENDERING_PROPERTIES_VARJO = 1000121002,
        XR_TYPE_COMPOSITION_LAYER_DEPTH_TEST_VARJO = 1000122000,
        XR_TYPE_GRAPHICS_BINDING_VULKAN2_KHR = XR_TYPE_GRAPHICS_BINDING_VULKAN_KHR,
        XR_TYPE_SWAPCHAIN_IMAGE_VULKAN2_KHR = XR_TYPE_SWAPCHAIN_IMAGE_VULKAN_KHR,
        XR_TYPE_GRAPHICS_REQUIREMENTS_VULKAN2_KHR = XR_TYPE_GRAPHICS_REQUIREMENTS_VULKAN_KHR,
        XR_STRUCTURE_TYPE_MAX_ENUM = 0x7FFFFFFF
    }
    public enum XrResult
    {
        XR_SUCCESS = 0,
        XR_TIMEOUT_EXPIRED = 1,
        XR_SESSION_LOSS_PENDING = 3,
        XR_EVENT_UNAVAILABLE = 4,
        XR_SPACE_BOUNDS_UNAVAILABLE = 7,
        XR_SESSION_NOT_FOCUSED = 8,
        XR_FRAME_DISCARDED = 9,
        XR_ERROR_VALIDATION_FAILURE = -1,
        XR_ERROR_RUNTIME_FAILURE = -2,
        XR_ERROR_OUT_OF_MEMORY = -3,
        XR_ERROR_API_VERSION_UNSUPPORTED = -4,
        XR_ERROR_INITIALIZATION_FAILED = -6,
        XR_ERROR_FUNCTION_UNSUPPORTED = -7,
        XR_ERROR_FEATURE_UNSUPPORTED = -8,
        XR_ERROR_EXTENSION_NOT_PRESENT = -9,
        XR_ERROR_LIMIT_REACHED = -10,
        XR_ERROR_SIZE_INSUFFICIENT = -11,
        XR_ERROR_HANDLE_INVALID = -12,
        XR_ERROR_INSTANCE_LOST = -13,
        XR_ERROR_SESSION_RUNNING = -14,
        XR_ERROR_SESSION_NOT_RUNNING = -16,
        XR_ERROR_SESSION_LOST = -17,
        XR_ERROR_SYSTEM_INVALID = -18,
        XR_ERROR_PATH_INVALID = -19,
        XR_ERROR_PATH_COUNT_EXCEEDED = -20,
        XR_ERROR_PATH_FORMAT_INVALID = -21,
        XR_ERROR_PATH_UNSUPPORTED = -22,
        XR_ERROR_LAYER_INVALID = -23,
        XR_ERROR_LAYER_LIMIT_EXCEEDED = -24,
        XR_ERROR_SWAPCHAIN_RECT_INVALID = -25,
        XR_ERROR_SWAPCHAIN_FORMAT_UNSUPPORTED = -26,
        XR_ERROR_ACTION_TYPE_MISMATCH = -27,
        XR_ERROR_SESSION_NOT_READY = -28,
        XR_ERROR_SESSION_NOT_STOPPING = -29,
        XR_ERROR_TIME_INVALID = -30,
        XR_ERROR_REFERENCE_SPACE_UNSUPPORTED = -31,
        XR_ERROR_FILE_ACCESS_ERROR = -32,
        XR_ERROR_FILE_CONTENTS_INVALID = -33,
        XR_ERROR_FORM_FACTOR_UNSUPPORTED = -34,
        XR_ERROR_FORM_FACTOR_UNAVAILABLE = -35,
        XR_ERROR_API_LAYER_NOT_PRESENT = -36,
        XR_ERROR_CALL_ORDER_INVALID = -37,
        XR_ERROR_GRAPHICS_DEVICE_INVALID = -38,
        XR_ERROR_POSE_INVALID = -39,
        XR_ERROR_INDEX_OUT_OF_RANGE = -40,
        XR_ERROR_VIEW_CONFIGURATION_TYPE_UNSUPPORTED = -41,
        XR_ERROR_ENVIRONMENT_BLEND_MODE_UNSUPPORTED = -42,
        XR_ERROR_NAME_DUPLICATED = -44,
        XR_ERROR_NAME_INVALID = -45,
        XR_ERROR_ACTIONSET_NOT_ATTACHED = -46,
        XR_ERROR_ACTIONSETS_ALREADY_ATTACHED = -47,
        XR_ERROR_LOCALIZED_NAME_DUPLICATED = -48,
        XR_ERROR_LOCALIZED_NAME_INVALID = -49,
        XR_ERROR_GRAPHICS_REQUIREMENTS_CALL_MISSING = -50,
        XR_ERROR_RUNTIME_UNAVAILABLE = -51,
        XR_ERROR_ANDROID_THREAD_SETTINGS_ID_INVALID_KHR = -1000003000,
        XR_ERROR_ANDROID_THREAD_SETTINGS_FAILURE_KHR = -1000003001,
        XR_ERROR_CREATE_SPATIAL_ANCHOR_FAILED_MSFT = -1000039001,
        XR_ERROR_SECONDARY_VIEW_CONFIGURATION_TYPE_NOT_ENABLED_MSFT = -1000053000,
        XR_ERROR_CONTROLLER_MODEL_KEY_INVALID_MSFT = -1000055000,
        XR_ERROR_REPROJECTION_MODE_UNSUPPORTED_MSFT = -1000066000,
        XR_ERROR_COMPUTE_NEW_SCENE_NOT_COMPLETED_MSFT = -1000097000,
        XR_ERROR_SCENE_COMPONENT_ID_INVALID_MSFT = -1000097001,
        XR_ERROR_SCENE_COMPONENT_TYPE_MISMATCH_MSFT = -1000097002,
        XR_ERROR_SCENE_MESH_BUFFER_ID_INVALID_MSFT = -1000097003,
        XR_ERROR_SCENE_COMPUTE_FEATURE_INCOMPATIBLE_MSFT = -1000097004,
        XR_ERROR_SCENE_COMPUTE_CONSISTENCY_MISMATCH_MSFT = -1000097005,
        XR_ERROR_DISPLAY_REFRESH_RATE_UNSUPPORTED_FB = -1000101000,
        XR_ERROR_COLOR_SPACE_UNSUPPORTED_FB = -1000108000,
        XR_ERROR_SPATIAL_ANCHOR_NAME_NOT_FOUND_MSFT = -1000142001,
        XR_ERROR_SPATIAL_ANCHOR_NAME_INVALID_MSFT = -1000142002,
        XR_RESULT_MAX_ENUM = 0x7FFFFFFF
    }

    public struct XrSystemGraphicsProperties
    {
        public uint maxSwapchainImageHeight;
        public uint maxSwapchainImageWidth;
        public uint maxLayerCount;
    }

    public struct XrSystemTrackingProperties
    {
        public uint orientationTracking;
        public uint positionTracking;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct XrSystemProperties
    {
        public XrStructureType type;
        public IntPtr next;
        public ulong systemId;
        public uint vendorId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] systemName;//char systemName[XR_MAX_SYSTEM_NAME_SIZE];
        public XrSystemGraphicsProperties graphicsProperties;
        public XrSystemTrackingProperties trackingProperties;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrSystemFacialTrackingPropertiesHTC
    {
        public XrStructureType type;
        public IntPtr next;
        public uint supportEyeFacialTracking;
        public uint supportLipFacialTracking;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrFacialExpressionsHTC
    {
        public XrStructureType type;
        public IntPtr next;
        public uint isActive;
        public Int64 sampleTime;
        public int expressionCount;
        public IntPtr blendShapeWeightings;
        public XrFacialExpressionsHTC(XrStructureType type_, IntPtr next_, uint isActive_, Int64 sampleTime_, int expressionCount_, IntPtr blendShapeWeightings_)
        {
            type = type_;
            next = next_;
            isActive = isActive_;
            sampleTime = sampleTime_;
            expressionCount = expressionCount_;
            blendShapeWeightings = blendShapeWeightings_;
        }
    }
    public struct XrFacialTrackerCreateInfoHTC
    {
        XrStructureType type;
        public IntPtr next;
        XrFacialTrackingTypeHTC expressionType;
        public XrFacialTrackerCreateInfoHTC(XrStructureType type_, IntPtr next_, XrFacialTrackingTypeHTC expressionType_)
        {
            type = type_;
            next = next_;
            expressionType = expressionType_;
        }
    }

    public enum XrFacialTrackingTypeHTC
    {
        XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC = 1,
        XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC = 2,
    }

    public enum XrEyeShapeHTC
    {
        XR_EYE_SHAPE_NONE_HTC = -1,
        XR_EYE_EXPRESSION_LEFT_BLINK_HTC = 0,
        XR_EYE_EXPRESSION_LEFT_WIDE_HTC = 1,
        XR_EYE_EXPRESSION_RIGHT_BLINK_HTC = 2,
        XR_EYE_EXPRESSION_RIGHT_WIDE_HTC = 3,
        XR_EYE_EXPRESSION_LEFT_SQUEEZE_HTC = 4,
        XR_EYE_EXPRESSION_RIGHT_SQUEEZE_HTC = 5,
        XR_EYE_EXPRESSION_LEFT_DOWN_HTC = 6,
        XR_EYE_EXPRESSION_RIGHT_DOWN_HTC = 7,
        XR_EYE_EXPRESSION_LEFT_OUT_HTC = 8,
        XR_EYE_EXPRESSION_RIGHT_IN_HTC = 9,
        XR_EYE_EXPRESSION_LEFT_IN_HTC = 10,
        XR_EYE_EXPRESSION_RIGHT_OUT_HTC = 11,
        XR_EYE_EXPRESSION_LEFT_UP_HTC = 12,
        XR_EYE_EXPRESSION_RIGHT_UP_HTC = 13,
        XR_EYE_EXPRESSION_MAX_ENUM_HTC = 14,
    }

    public enum XrLipShapeHTC
    {
        XR_LIP_SHAPE_NONE_HTC = -1,
        XR_LIP_SHAPE_JAW_RIGHT_HTC = 0,
        XR_LIP_SHAPE_JAW_LEFT_HTC = 1,
        XR_LIP_SHAPE_JAW_FORWARD_HTC = 2,
        XR_LIP_SHAPE_JAW_OPEN_HTC = 3,
        XR_LIP_SHAPE_MOUTH_APE_SHAPE_HTC = 4,
        XR_LIP_SHAPE_MOUTH_UPPER_RIGHT_HTC = 5,
        XR_LIP_SHAPE_MOUTH_UPPER_LEFT_HTC = 6,
        XR_LIP_SHAPE_MOUTH_LOWER_RIGHT_HTC = 7,
        XR_LIP_SHAPE_MOUTH_LOWER_LEFT_HTC = 8,
        XR_LIP_SHAPE_MOUTH_UPPER_OVERTURN_HTC = 9,
        XR_LIP_SHAPE_MOUTH_LOWER_OVERTURN_HTC = 10,
        XR_LIP_SHAPE_MOUTH_POUT_HTC = 11,
        XR_LIP_SHAPE_MOUTH_SMILE_RIGHT_HTC = 12,
        XR_LIP_SHAPE_MOUTH_SMILE_LEFT_HTC = 13,
        XR_LIP_SHAPE_MOUTH_SAD_RIGHT_HTC = 14,
        XR_LIP_SHAPE_MOUTH_SAD_LEFT_HTC = 15,
        XR_LIP_SHAPE_CHEEK_PUFF_RIGHT_HTC = 16,
        XR_LIP_SHAPE_CHEEK_PUFF_LEFT_HTC = 17,
        XR_LIP_SHAPE_CHEEK_SUCK_HTC = 18,
        XR_LIP_SHAPE_MOUTH_UPPER_UPRIGHT_HTC = 19,
        XR_LIP_SHAPE_MOUTH_UPPER_UPLEFT_HTC = 20,
        XR_LIP_SHAPE_MOUTH_LOWER_DOWNRIGHT_HTC = 21,
        XR_LIP_SHAPE_MOUTH_LOWER_DOWNLEFT_HTC = 22,
        XR_LIP_SHAPE_MOUTH_UPPER_INSIDE_HTC = 23,
        XR_LIP_SHAPE_MOUTH_LOWER_INSIDE_HTC = 24,
        XR_LIP_SHAPE_MOUTH_LOWER_OVERLAY_HTC = 25,
        XR_LIP_SHAPE_TONGUE_LONGSTEP1_HTC = 26,
        XR_LIP_SHAPE_TONGUE_LEFT_HTC = 27,
        XR_LIP_SHAPE_TONGUE_RIGHT_HTC = 28,
        XR_LIP_SHAPE_TONGUE_UP_HTC = 29,
        XR_LIP_SHAPE_TONGUE_DOWN_HTC = 30,
        XR_LIP_SHAPE_TONGUE_ROLL_HTC = 31,
        XR_LIP_SHAPE_TONGUE_LONGSTEP2_HTC = 32,
        XR_LIP_SHAPE_TONGUE_UPRIGHT_MORPH_HTC = 33,
        XR_LIP_SHAPE_TONGUE_UPLEFT_MORPH_HTC = 34,
        XR_LIP_SHAPE_TONGUE_DOWNRIGHT_MORPH_HTC = 35,
        XR_LIP_SHAPE_TONGUE_DOWNLEFT_MORPH_HTC = 36,
        XR_LIP_SHAPE_MAX_ENUM_HTC = 37
    }

    public struct XrFrameWaitInfo
    {
        public XrStructureType type;
        public IntPtr next;
        public XrFrameWaitInfo(IntPtr next_, XrStructureType type_)
        {
            next = next_;
            type = type_;
        }
    }

    public struct XrFrameState
    {
        public XrStructureType type;
        public IntPtr next;
        public Int64 predictedDisplayTime;
        public Int64 predictedDisplayPeriod;
        public bool shouldRender;
    }

    public struct XrVector3f
    {
        public float x;
        public float y;
        public float z;
        public XrVector3f(float x_, float y_, float z_)
        {
            x = x_;
            y = y_;
            z = z_;
        }
    }

    public struct XrQuaternionf
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public XrQuaternionf(float x_, float y_, float z_, float w_)
        {
            x = x_;
            y = y_;
            z = z_;
            w = w_;
        }
    }

    public struct XrPosef
    {
        public XrQuaternionf orientation;
        public XrVector3f position;
    }

    #region XRSPace
    [Flags]
    public enum XrSpaceLocationFlags : UInt64
    {
        XR_SPACE_LOCATION_ORIENTATION_VALID_BIT = 0x00000001,
        XR_SPACE_LOCATION_POSITION_VALID_BIT = 0x00000002,
        XR_SPACE_LOCATION_ORIENTATION_TRACKED_BIT = 0x00000004,
        XR_SPACE_LOCATION_POSITION_TRACKED_BIT = 0x00000008,
    }

    public enum XrReferenceSpaceType
    {
        XR_REFERENCE_SPACE_TYPE_VIEW = 1,
        XR_REFERENCE_SPACE_TYPE_LOCAL = 2,
        XR_REFERENCE_SPACE_TYPE_STAGE = 3,
        XR_REFERENCE_SPACE_TYPE_UNBOUNDED_MSFT = 1000038000,
        XR_REFERENCE_SPACE_TYPE_COMBINED_EYE_VARJO = 1000121000,
        XR_REFERENCE_SPACE_TYPE_MAX_ENUM = 0x7FFFFFFF
    }

    public struct XrReferenceSpaceCreateInfo
    {
        public XrStructureType type;
        public IntPtr next;
        public XrReferenceSpaceType referencespacetype;
        public XrPosef poseInReferenceSpace;
    }
    #endregion

    namespace HandTracking
    {
        public enum XrResult
        {
            XR_SUCCESS = 0,
            XR_TIMEOUT_EXPIRED = 1,
            XR_SESSION_LOSS_PENDING = 3,
            XR_EVENT_UNAVAILABLE = 4,
            XR_SPACE_BOUNDS_UNAVAILABLE = 7,
            XR_SESSION_NOT_FOCUSED = 8,
            XR_FRAME_DISCARDED = 9,
            XR_ERROR_VALIDATION_FAILURE = -1,
            XR_ERROR_RUNTIME_FAILURE = -2,
            XR_ERROR_OUT_OF_MEMORY = -3,
            XR_ERROR_API_VERSION_UNSUPPORTED = -4,
            XR_ERROR_INITIALIZATION_FAILED = -6,
            XR_ERROR_FUNCTION_UNSUPPORTED = -7,
            XR_ERROR_FEATURE_UNSUPPORTED = -8,
            XR_ERROR_EXTENSION_NOT_PRESENT = -9,
            XR_ERROR_LIMIT_REACHED = -10,
            XR_ERROR_SIZE_INSUFFICIENT = -11,
            XR_ERROR_HANDLE_INVALID = -12,
            XR_ERROR_INSTANCE_LOST = -13,
            XR_ERROR_SESSION_RUNNING = -14,
            XR_ERROR_SESSION_NOT_RUNNING = -16,
            XR_ERROR_SESSION_LOST = -17,
            XR_ERROR_SYSTEM_INVALID = -18,
            XR_ERROR_PATH_INVALID = -19,
            XR_ERROR_PATH_COUNT_EXCEEDED = -20,
            XR_ERROR_PATH_FORMAT_INVALID = -21,
            XR_ERROR_PATH_UNSUPPORTED = -22,
            XR_ERROR_LAYER_INVALID = -23,
            XR_ERROR_LAYER_LIMIT_EXCEEDED = -24,
            XR_ERROR_SWAPCHAIN_RECT_INVALID = -25,
            XR_ERROR_SWAPCHAIN_FORMAT_UNSUPPORTED = -26,
            XR_ERROR_ACTION_TYPE_MISMATCH = -27,
            XR_ERROR_SESSION_NOT_READY = -28,
            XR_ERROR_SESSION_NOT_STOPPING = -29,
            XR_ERROR_TIME_INVALID = -30,
            XR_ERROR_REFERENCE_SPACE_UNSUPPORTED = -31,
            XR_ERROR_FILE_ACCESS_ERROR = -32,
            XR_ERROR_FILE_CONTENTS_INVALID = -33,
            XR_ERROR_FORM_FACTOR_UNSUPPORTED = -34,
            XR_ERROR_FORM_FACTOR_UNAVAILABLE = -35,
            XR_ERROR_API_LAYER_NOT_PRESENT = -36,
            XR_ERROR_CALL_ORDER_INVALID = -37,
            XR_ERROR_GRAPHICS_DEVICE_INVALID = -38,
            XR_ERROR_POSE_INVALID = -39,
            XR_ERROR_INDEX_OUT_OF_RANGE = -40,
            XR_ERROR_VIEW_CONFIGURATION_TYPE_UNSUPPORTED = -41,
            XR_ERROR_ENVIRONMENT_BLEND_MODE_UNSUPPORTED = -42,
            XR_ERROR_NAME_DUPLICATED = -44,
            XR_ERROR_NAME_INVALID = -45,
            XR_ERROR_ACTIONSET_NOT_ATTACHED = -46,
            XR_ERROR_ACTIONSETS_ALREADY_ATTACHED = -47,
            XR_ERROR_LOCALIZED_NAME_DUPLICATED = -48,
            XR_ERROR_LOCALIZED_NAME_INVALID = -49,
            XR_ERROR_GRAPHICS_REQUIREMENTS_CALL_MISSING = -50,
            XR_ERROR_RUNTIME_UNAVAILABLE = -51,
            XR_ERROR_ANDROID_THREAD_SETTINGS_ID_INVALID_KHR = -1000003000,
            XR_ERROR_ANDROID_THREAD_SETTINGS_FAILURE_KHR = -1000003001,
            XR_ERROR_CREATE_SPATIAL_ANCHOR_FAILED_MSFT = -1000039001,
            XR_ERROR_SECONDARY_VIEW_CONFIGURATION_TYPE_NOT_ENABLED_MSFT = -1000053000,
            XR_ERROR_CONTROLLER_MODEL_KEY_INVALID_MSFT = -1000055000,
            XR_ERROR_REPROJECTION_MODE_UNSUPPORTED_MSFT = -1000066000,
            XR_ERROR_COMPUTE_NEW_SCENE_NOT_COMPLETED_MSFT = -1000097000,
            XR_ERROR_SCENE_COMPONENT_ID_INVALID_MSFT = -1000097001,
            XR_ERROR_SCENE_COMPONENT_TYPE_MISMATCH_MSFT = -1000097002,
            XR_ERROR_SCENE_MESH_BUFFER_ID_INVALID_MSFT = -1000097003,
            XR_ERROR_SCENE_COMPUTE_FEATURE_INCOMPATIBLE_MSFT = -1000097004,
            XR_ERROR_SCENE_COMPUTE_CONSISTENCY_MISMATCH_MSFT = -1000097005,
            XR_ERROR_DISPLAY_REFRESH_RATE_UNSUPPORTED_FB = -1000101000,
            XR_ERROR_COLOR_SPACE_UNSUPPORTED_FB = -1000108000,
            XR_ERROR_SPATIAL_ANCHOR_NAME_NOT_FOUND_MSFT = -1000142001,
            XR_ERROR_SPATIAL_ANCHOR_NAME_INVALID_MSFT = -1000142002,
            XR_RESULT_MAX_ENUM = 0x7FFFFFFF
        }
        public struct XrVector3f
        {
            public float x;
            public float y;
            public float z;
            public XrVector3f(float x_, float y_, float z_)
            {
                x = x_;
                y = y_;
                z = z_;
            }
        }

        public struct XrQuaternionf
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public XrQuaternionf(float x_, float y_, float z_, float w_)
            {
                x = x_;
                y = y_;
                z = z_;
                w = w_;
            }
        }

        public struct XrPosef
        {
            public XrQuaternionf orientation;
            public XrVector3f position;
        }
        public enum XrHandJointEXT
        {
            XR_HAND_JOINT_PALM_EXT = 0,
            XR_HAND_JOINT_WRIST_EXT = 1,
            XR_HAND_JOINT_THUMB_METACARPAL_EXT = 2,
            XR_HAND_JOINT_THUMB_PROXIMAL_EXT = 3,
            XR_HAND_JOINT_THUMB_DISTAL_EXT = 4,
            XR_HAND_JOINT_THUMB_TIP_EXT = 5,
            XR_HAND_JOINT_INDEX_METACARPAL_EXT = 6,
            XR_HAND_JOINT_INDEX_PROXIMAL_EXT = 7,
            XR_HAND_JOINT_INDEX_INTERMEDIATE_EXT = 8,
            XR_HAND_JOINT_INDEX_DISTAL_EXT = 9,
            XR_HAND_JOINT_INDEX_TIP_EXT = 10,
            XR_HAND_JOINT_MIDDLE_METACARPAL_EXT = 11,
            XR_HAND_JOINT_MIDDLE_PROXIMAL_EXT = 12,
            XR_HAND_JOINT_MIDDLE_INTERMEDIATE_EXT = 13,
            XR_HAND_JOINT_MIDDLE_DISTAL_EXT = 14,
            XR_HAND_JOINT_MIDDLE_TIP_EXT = 15,
            XR_HAND_JOINT_RING_METACARPAL_EXT = 16,
            XR_HAND_JOINT_RING_PROXIMAL_EXT = 17,
            XR_HAND_JOINT_RING_INTERMEDIATE_EXT = 18,
            XR_HAND_JOINT_RING_DISTAL_EXT = 19,
            XR_HAND_JOINT_RING_TIP_EXT = 20,
            XR_HAND_JOINT_LITTLE_METACARPAL_EXT = 21,
            XR_HAND_JOINT_LITTLE_PROXIMAL_EXT = 22,
            XR_HAND_JOINT_LITTLE_INTERMEDIATE_EXT = 23,
            XR_HAND_JOINT_LITTLE_DISTAL_EXT = 24,
            XR_HAND_JOINT_LITTLE_TIP_EXT = 25,
            XR_HAND_JOINT_MAX_ENUM_EXT = 26
        }
        public enum XrHandEXT
        {
            XR_HAND_LEFT_EXT = 1,
            XR_HAND_RIGHT_EXT = 2,
            XR_HAND_MAX_ENUM_EXT = 0x7FFFFFFF
        }

        public enum XrHandJointSetEXT
        {
            XR_HAND_JOINT_SET_DEFAULT_EXT = 0,
            XR_HAND_JOINT_SET_HAND_WITH_FOREARM_ULTRALEAP = 1000149000,
            XR_HAND_JOINT_SET_MAX_ENUM_EXT = 0x7FFFFFFF
        }
        public struct XrSystemHandTrackingPropertiesEXT
        {
            public XrStructureType type;
            public IntPtr next;
            public uint supportsHandTracking;
        }

        public struct XrHandJointsLocateInfoEXT
        {
            public XrStructureType type;
            public IntPtr next;
            public ulong baseSpace; //XrSpace
            public Int64 time;

            public XrHandJointsLocateInfoEXT(XrStructureType type_, ref IntPtr next_, ulong baseSpace_, Int64 time_)
            {
                type = type_;
                next = next_;
                baseSpace = baseSpace_;
                time = time_;
            }
        }

        public struct XrHandJointLocationsEXT
        {
            public XrStructureType type;
            public IntPtr next;
            public uint isActive;
            public int jointCount;
            public IntPtr jointLocations;  // XrHandJointLocationEXT*

            public XrHandJointLocationsEXT(XrStructureType type_, IntPtr next_, uint isActive_, int jointCount_, IntPtr jointLocations_)
            {
                type = type_;
                next = next_;
                isActive = isActive_;
                jointCount = jointCount_;
                jointLocations = jointLocations_;
            }
        }

        public struct XrHandJointsMotionRangeInfoEXT // chain under XrHandJointsLocateInfoEXT
        {
            public XrStructureType type;
            public IntPtr next;
            public XrHandJointsMotionRangeEXT handJointsMotionRange;
        }

        public struct XrHandJointLocationEXT
        {
            public UInt64 locationFlags; // XrSpaceLocationFlags
            public XrPosef pose;
            public float radius;
        }

        public struct XrHandTrackerCreateInfoEXT
        {
            public XrStructureType type;
            public IntPtr next;
            public XrHandEXT hand;
            public XrHandJointSetEXT handJointSet;

            public XrHandTrackerCreateInfoEXT(XrStructureType type_, IntPtr next_, XrHandEXT hand_, XrHandJointSetEXT handJointSet_)
            {
                type = type_;
                next = next_;
                hand = hand_;
                handJointSet = handJointSet_;
            }
        }

        public struct XrHandJointVelocitiesEXT // chain under XrHandJointLocationsEXT
        {
            public XrStructureType type;
            public IntPtr next;
            public UInt32 jointCount;
            public IntPtr jointVelocities; // XrHandJointVelocitiesEXT*
        }

        public struct XrHandJointVelocityEXT
        {
            public XrSpaceVelocityFlags velocityFlags;
            public XrVector3f linearVelocity;
            public XrVector3f angularVelocity;
        }

        [Flags]
        public enum XrSpaceVelocityFlags : UInt64
        {
            XR_SPACE_VELOCITY_LINEAR_VALID_BIT = 0x00000001,
            XR_SPACE_VELOCITY_ANGULAR_VALID_BIT = 0x00000002,
        }
        #region XRSPace
        [Flags]
        public enum XrSpaceLocationFlags : UInt64
        {
            XR_SPACE_LOCATION_ORIENTATION_VALID_BIT = 0x00000001,
            XR_SPACE_LOCATION_POSITION_VALID_BIT = 0x00000002,
            XR_SPACE_LOCATION_ORIENTATION_TRACKED_BIT = 0x00000004,
            XR_SPACE_LOCATION_POSITION_TRACKED_BIT = 0x00000008,
        }

        public enum XrReferenceSpaceType
        {
            XR_REFERENCE_SPACE_TYPE_VIEW = 1,
            XR_REFERENCE_SPACE_TYPE_LOCAL = 2,
            XR_REFERENCE_SPACE_TYPE_STAGE = 3,
            XR_REFERENCE_SPACE_TYPE_UNBOUNDED_MSFT = 1000038000,
            XR_REFERENCE_SPACE_TYPE_COMBINED_EYE_VARJO = 1000121000,
            XR_REFERENCE_SPACE_TYPE_MAX_ENUM = 0x7FFFFFFF
        }

        public struct XrReferenceSpaceCreateInfo
        {
            public XrStructureType type;
            public IntPtr next;
            public XrReferenceSpaceType referencespacetype;
            public XrPosef poseInReferenceSpace;
        }
        #endregion

        public enum XrHandJointsMotionRangeEXT
        {
            XR_HAND_JOINTS_MOTION_RANGE_UNOBSTRUCTED_EXT = 1,
            XR_HAND_JOINTS_MOTION_RANGE_CONFORMING_TO_CONTROLLER_EXT = 2,
            XR_HAND_JOINTS_MOTION_RANGE_MAX_ENUM_EXT = 0x7FFFFFFF
        }
    }




   
    public struct XrFovf
    {
        public float    angleLeft;
        public float    angleRight;
        public float    angleUp;
        public float    angleDown;
    }

    public struct XrExtent2Df
    {
        public float    width;
        public float    height;
    }

    
    namespace SceneUnderstanding
    {
        public struct XrSystemTrackingProperties
        {
            public uint orientationTracking;
            public uint positionTracking;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct XrSystemProperties
        {
            public XrStructureType type;
            public IntPtr next;
            public ulong systemId;
            public uint vendorId;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] systemName;//char systemName[XR_MAX_SYSTEM_NAME_SIZE];
            public XrSystemGraphicsProperties graphicsProperties;
            public XrSystemTrackingProperties trackingProperties;
        }
        public enum XrStructureType
        {
            XR_TYPE_UNKNOWN = 0,
            XR_TYPE_API_LAYER_PROPERTIES = 1,
            XR_TYPE_EXTENSION_PROPERTIES = 2,
            XR_TYPE_INSTANCE_CREATE_INFO = 3,
            XR_TYPE_SYSTEM_GET_INFO = 4,
            XR_TYPE_SYSTEM_PROPERTIES = 5,
            XR_TYPE_VIEW_LOCATE_INFO = 6,
            XR_TYPE_VIEW = 7,
            XR_TYPE_SESSION_CREATE_INFO = 8,
            XR_TYPE_SWAPCHAIN_CREATE_INFO = 9,
            XR_TYPE_SESSION_BEGIN_INFO = 10,
            XR_TYPE_VIEW_STATE = 11,
            XR_TYPE_FRAME_END_INFO = 12,
            XR_TYPE_HAPTIC_VIBRATION = 13,
            XR_TYPE_EVENT_DATA_BUFFER = 16,
            XR_TYPE_EVENT_DATA_INSTANCE_LOSS_PENDING = 17,
            XR_TYPE_EVENT_DATA_SESSION_STATE_CHANGED = 18,
            XR_TYPE_ACTION_STATE_BOOLEAN = 23,
            XR_TYPE_ACTION_STATE_FLOAT = 24,
            XR_TYPE_ACTION_STATE_VECTOR2F = 25,
            XR_TYPE_ACTION_STATE_POSE = 27,
            XR_TYPE_ACTION_SET_CREATE_INFO = 28,
            XR_TYPE_ACTION_CREATE_INFO = 29,
            XR_TYPE_INSTANCE_PROPERTIES = 32,
            XR_TYPE_FRAME_WAIT_INFO = 33,
            XR_TYPE_COMPOSITION_LAYER_PROJECTION = 35,
            XR_TYPE_COMPOSITION_LAYER_QUAD = 36,
            XR_TYPE_REFERENCE_SPACE_CREATE_INFO = 37,
            XR_TYPE_ACTION_SPACE_CREATE_INFO = 38,
            XR_TYPE_EVENT_DATA_REFERENCE_SPACE_CHANGE_PENDING = 40,
            XR_TYPE_VIEW_CONFIGURATION_VIEW = 41,
            XR_TYPE_SPACE_LOCATION = 42,
            XR_TYPE_SPACE_VELOCITY = 43,
            XR_TYPE_FRAME_STATE = 44,
            XR_TYPE_VIEW_CONFIGURATION_PROPERTIES = 45,
            XR_TYPE_FRAME_BEGIN_INFO = 46,
            XR_TYPE_COMPOSITION_LAYER_PROJECTION_VIEW = 48,
            XR_TYPE_EVENT_DATA_EVENTS_LOST = 49,
            XR_TYPE_INTERACTION_PROFILE_SUGGESTED_BINDING = 51,
            XR_TYPE_EVENT_DATA_INTERACTION_PROFILE_CHANGED = 52,
            XR_TYPE_INTERACTION_PROFILE_STATE = 53,
            XR_TYPE_SWAPCHAIN_IMAGE_ACQUIRE_INFO = 55,
            XR_TYPE_SWAPCHAIN_IMAGE_WAIT_INFO = 56,
            XR_TYPE_SWAPCHAIN_IMAGE_RELEASE_INFO = 57,
            XR_TYPE_ACTION_STATE_GET_INFO = 58,
            XR_TYPE_HAPTIC_ACTION_INFO = 59,
            XR_TYPE_SESSION_ACTION_SETS_ATTACH_INFO = 60,
            XR_TYPE_ACTIONS_SYNC_INFO = 61,
            XR_TYPE_BOUND_SOURCES_FOR_ACTION_ENUMERATE_INFO = 62,
            XR_TYPE_INPUT_SOURCE_LOCALIZED_NAME_GET_INFO = 63,
            XR_TYPE_COMPOSITION_LAYER_CUBE_KHR = 1000006000,
            XR_TYPE_INSTANCE_CREATE_INFO_ANDROID_KHR = 1000008000,
            XR_TYPE_COMPOSITION_LAYER_DEPTH_INFO_KHR = 1000010000,
            XR_TYPE_VULKAN_SWAPCHAIN_FORMAT_LIST_CREATE_INFO_KHR = 1000014000,
            XR_TYPE_EVENT_DATA_PERF_SETTINGS_EXT = 1000015000,
            XR_TYPE_COMPOSITION_LAYER_CYLINDER_KHR = 1000017000,
            XR_TYPE_COMPOSITION_LAYER_EQUIRECT_KHR = 1000018000,
            XR_TYPE_DEBUG_UTILS_OBJECT_NAME_INFO_EXT = 1000019000,
            XR_TYPE_DEBUG_UTILS_MESSENGER_CALLBACK_DATA_EXT = 1000019001,
            XR_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT = 1000019002,
            XR_TYPE_DEBUG_UTILS_LABEL_EXT = 1000019003,
            XR_TYPE_GRAPHICS_BINDING_OPENGL_WIN32_KHR = 1000023000,
            XR_TYPE_GRAPHICS_BINDING_OPENGL_XLIB_KHR = 1000023001,
            XR_TYPE_GRAPHICS_BINDING_OPENGL_XCB_KHR = 1000023002,
            XR_TYPE_GRAPHICS_BINDING_OPENGL_WAYLAND_KHR = 1000023003,
            XR_TYPE_SWAPCHAIN_IMAGE_OPENGL_KHR = 1000023004,
            XR_TYPE_GRAPHICS_REQUIREMENTS_OPENGL_KHR = 1000023005,
            XR_TYPE_GRAPHICS_BINDING_OPENGL_ES_ANDROID_KHR = 1000024001,
            XR_TYPE_SWAPCHAIN_IMAGE_OPENGL_ES_KHR = 1000024002,
            XR_TYPE_GRAPHICS_REQUIREMENTS_OPENGL_ES_KHR = 1000024003,
            XR_TYPE_GRAPHICS_BINDING_VULKAN_KHR = 1000025000,
            XR_TYPE_SWAPCHAIN_IMAGE_VULKAN_KHR = 1000025001,
            XR_TYPE_GRAPHICS_REQUIREMENTS_VULKAN_KHR = 1000025002,
            XR_TYPE_GRAPHICS_BINDING_D3D11_KHR = 1000027000,
            XR_TYPE_SWAPCHAIN_IMAGE_D3D11_KHR = 1000027001,
            XR_TYPE_GRAPHICS_REQUIREMENTS_D3D11_KHR = 1000027002,
            XR_TYPE_GRAPHICS_BINDING_D3D12_KHR = 1000028000,
            XR_TYPE_SWAPCHAIN_IMAGE_D3D12_KHR = 1000028001,
            XR_TYPE_GRAPHICS_REQUIREMENTS_D3D12_KHR = 1000028002,
            XR_TYPE_SYSTEM_EYE_GAZE_INTERACTION_PROPERTIES_EXT = 1000030000,
            XR_TYPE_EYE_GAZE_SAMPLE_TIME_EXT = 1000030001,
            XR_TYPE_VISIBILITY_MASK_KHR = 1000031000,
            XR_TYPE_EVENT_DATA_VISIBILITY_MASK_CHANGED_KHR = 1000031001,
            XR_TYPE_SESSION_CREATE_INFO_OVERLAY_EXTX = 1000033000,
            XR_TYPE_EVENT_DATA_MAIN_SESSION_VISIBILITY_CHANGED_EXTX = 1000033003,
            XR_TYPE_COMPOSITION_LAYER_COLOR_SCALE_BIAS_KHR = 1000034000,
            XR_TYPE_SPATIAL_ANCHOR_CREATE_INFO_MSFT = 1000039000,
            XR_TYPE_SPATIAL_ANCHOR_SPACE_CREATE_INFO_MSFT = 1000039001,
            XR_TYPE_VIEW_CONFIGURATION_DEPTH_RANGE_EXT = 1000046000,
            XR_TYPE_GRAPHICS_BINDING_EGL_MNDX = 1000048004,
            XR_TYPE_SPATIAL_GRAPH_NODE_SPACE_CREATE_INFO_MSFT = 1000049000,
            XR_TYPE_SYSTEM_HAND_TRACKING_PROPERTIES_EXT = 1000051000,
            XR_TYPE_HAND_TRACKER_CREATE_INFO_EXT = 1000051001,
            XR_TYPE_HAND_JOINTS_LOCATE_INFO_EXT = 1000051002,
            XR_TYPE_HAND_JOINT_LOCATIONS_EXT = 1000051003,
            XR_TYPE_HAND_JOINT_VELOCITIES_EXT = 1000051004,
            XR_TYPE_SYSTEM_HAND_TRACKING_MESH_PROPERTIES_MSFT = 1000052000,
            XR_TYPE_HAND_MESH_SPACE_CREATE_INFO_MSFT = 1000052001,
            XR_TYPE_HAND_MESH_UPDATE_INFO_MSFT = 1000052002,
            XR_TYPE_HAND_MESH_MSFT = 1000052003,
            XR_TYPE_HAND_POSE_TYPE_INFO_MSFT = 1000052004,
            XR_TYPE_SECONDARY_VIEW_CONFIGURATION_SESSION_BEGIN_INFO_MSFT = 1000053000,
            XR_TYPE_SECONDARY_VIEW_CONFIGURATION_STATE_MSFT = 1000053001,
            XR_TYPE_SECONDARY_VIEW_CONFIGURATION_FRAME_STATE_MSFT = 1000053002,
            XR_TYPE_SECONDARY_VIEW_CONFIGURATION_FRAME_END_INFO_MSFT = 1000053003,
            XR_TYPE_SECONDARY_VIEW_CONFIGURATION_LAYER_INFO_MSFT = 1000053004,
            XR_TYPE_SECONDARY_VIEW_CONFIGURATION_SWAPCHAIN_CREATE_INFO_MSFT = 1000053005,
            XR_TYPE_CONTROLLER_MODEL_KEY_STATE_MSFT = 1000055000,
            XR_TYPE_CONTROLLER_MODEL_NODE_PROPERTIES_MSFT = 1000055001,
            XR_TYPE_CONTROLLER_MODEL_PROPERTIES_MSFT = 1000055002,
            XR_TYPE_CONTROLLER_MODEL_NODE_STATE_MSFT = 1000055003,
            XR_TYPE_CONTROLLER_MODEL_STATE_MSFT = 1000055004,
            XR_TYPE_VIEW_CONFIGURATION_VIEW_FOV_EPIC = 1000059000,
            XR_TYPE_HOLOGRAPHIC_WINDOW_ATTACHMENT_MSFT = 1000063000,
            XR_TYPE_ANDROID_SURFACE_SWAPCHAIN_CREATE_INFO_FB = 1000070000,
            XR_TYPE_SWAPCHAIN_STATE_ANDROID_SURFACE_DIMENSIONS_FB = 1000071000,
            XR_TYPE_SWAPCHAIN_STATE_SAMPLER_OPENGL_ES_FB = 1000071001,
            XR_TYPE_INTERACTION_PROFILE_ANALOG_THRESHOLD_VALVE = 1000079000,
            XR_TYPE_HAND_JOINTS_MOTION_RANGE_INFO_EXT = 1000080000,
            XR_TYPE_LOADER_INIT_INFO_ANDROID_KHR = 1000089000,
            XR_TYPE_VULKAN_INSTANCE_CREATE_INFO_KHR = 1000090000,
            XR_TYPE_VULKAN_DEVICE_CREATE_INFO_KHR = 1000090001,
            XR_TYPE_VULKAN_GRAPHICS_DEVICE_GET_INFO_KHR = 1000090003,
            XR_TYPE_COMPOSITION_LAYER_EQUIRECT2_KHR = 1000091000,
            XR_TYPE_SCENE_OBSERVER_CREATE_INFO_MSFT = 1000097000,
            XR_TYPE_SCENE_CREATE_INFO_MSFT = 1000097001,
            XR_TYPE_NEW_SCENE_COMPUTE_INFO_MSFT = 1000097002,
            XR_TYPE_VISUAL_MESH_COMPUTE_LOD_INFO_MSFT = 1000097003,
            XR_TYPE_SCENE_COMPONENTS_MSFT = 1000097004,
            XR_TYPE_SCENE_COMPONENTS_GET_INFO_MSFT = 1000097005,
            XR_TYPE_SCENE_COMPONENT_LOCATIONS_MSFT = 1000097006,
            XR_TYPE_SCENE_COMPONENTS_LOCATE_INFO_MSFT = 1000097007,
            XR_TYPE_SCENE_OBJECTS_MSFT = 1000097008,
            XR_TYPE_SCENE_COMPONENT_PARENT_FILTER_INFO_MSFT = 1000097009,
            XR_TYPE_SCENE_OBJECT_TYPES_FILTER_INFO_MSFT = 1000097010,
            XR_TYPE_SCENE_PLANES_MSFT = 1000097011,
            XR_TYPE_SCENE_PLANE_ALIGNMENT_FILTER_INFO_MSFT = 1000097012,
            XR_TYPE_SCENE_MESHES_MSFT = 1000097013,
            XR_TYPE_SCENE_MESH_BUFFERS_GET_INFO_MSFT = 1000097014,
            XR_TYPE_SCENE_MESH_BUFFERS_MSFT = 1000097015,
            XR_TYPE_SCENE_MESH_VERTEX_BUFFER_MSFT = 1000097016,
            XR_TYPE_SCENE_MESH_INDICES_UINT32_MSFT = 1000097017,
            XR_TYPE_SCENE_MESH_INDICES_UINT16_MSFT = 1000097018,
            XR_TYPE_SERIALIZED_SCENE_FRAGMENT_DATA_GET_INFO_MSFT = 1000098000,
            XR_TYPE_SCENE_DESERIALIZE_INFO_MSFT = 1000098001,
            XR_TYPE_EVENT_DATA_DISPLAY_REFRESH_RATE_CHANGED_FB = 1000101000,
            XR_TYPE_SYSTEM_PASS_THROUGH_PROPERTIES_HTC = 1000103000,
            XR_TYPE_PASS_THROUGH_CREATE_INFO_HTC = 1000103001,
            XR_TYPE_PASS_THROUGH_FRAME_HTC = 1000103002,
            XR_TYPE_PASS_THROUGH_ACQUIRE_INFO_HTC = 1000103003,
            XR_TYPE_EVENT_DATA_RUNTIME_EVENT_HTC = 1000103004,
            XR_TYPE_SYSTEM_FACIAL_TRACKING_PROPERTIES_HTC = 1000104000,
            XR_TYPE_FACIAL_TRACKER_CREATE_INFO_HTC = 1000104001,
            XR_TYPE_FACIAL_EXPRESSIONS_HTC = 1000104002,
            XR_TYPE_SYSTEM_COLOR_SPACE_PROPERTIES_FB = 1000108000,
            XR_TYPE_BINDING_MODIFICATIONS_KHR = 1000120000,
            XR_TYPE_VIEW_LOCATE_FOVEATED_RENDERING_VARJO = 1000121000,
            XR_TYPE_FOVEATED_VIEW_CONFIGURATION_VIEW_VARJO = 1000121001,
            XR_TYPE_SYSTEM_FOVEATED_RENDERING_PROPERTIES_VARJO = 1000121002,
            XR_TYPE_COMPOSITION_LAYER_DEPTH_TEST_VARJO = 1000122000,
            XR_TYPE_GRAPHICS_BINDING_VULKAN2_KHR = XR_TYPE_GRAPHICS_BINDING_VULKAN_KHR,
            XR_TYPE_SWAPCHAIN_IMAGE_VULKAN2_KHR = XR_TYPE_SWAPCHAIN_IMAGE_VULKAN_KHR,
            XR_TYPE_GRAPHICS_REQUIREMENTS_VULKAN2_KHR = XR_TYPE_GRAPHICS_REQUIREMENTS_VULKAN_KHR,
            XR_STRUCTURE_TYPE_MAX_ENUM = 0x7FFFFFFF
        }
        public enum XrResult
        {
            XR_SUCCESS = 0,
            XR_TIMEOUT_EXPIRED = 1,
            XR_SESSION_LOSS_PENDING = 3,
            XR_EVENT_UNAVAILABLE = 4,
            XR_SPACE_BOUNDS_UNAVAILABLE = 7,
            XR_SESSION_NOT_FOCUSED = 8,
            XR_FRAME_DISCARDED = 9,
            XR_ERROR_VALIDATION_FAILURE = -1,
            XR_ERROR_RUNTIME_FAILURE = -2,
            XR_ERROR_OUT_OF_MEMORY = -3,
            XR_ERROR_API_VERSION_UNSUPPORTED = -4,
            XR_ERROR_INITIALIZATION_FAILED = -6,
            XR_ERROR_FUNCTION_UNSUPPORTED = -7,
            XR_ERROR_FEATURE_UNSUPPORTED = -8,
            XR_ERROR_EXTENSION_NOT_PRESENT = -9,
            XR_ERROR_LIMIT_REACHED = -10,
            XR_ERROR_SIZE_INSUFFICIENT = -11,
            XR_ERROR_HANDLE_INVALID = -12,
            XR_ERROR_INSTANCE_LOST = -13,
            XR_ERROR_SESSION_RUNNING = -14,
            XR_ERROR_SESSION_NOT_RUNNING = -16,
            XR_ERROR_SESSION_LOST = -17,
            XR_ERROR_SYSTEM_INVALID = -18,
            XR_ERROR_PATH_INVALID = -19,
            XR_ERROR_PATH_COUNT_EXCEEDED = -20,
            XR_ERROR_PATH_FORMAT_INVALID = -21,
            XR_ERROR_PATH_UNSUPPORTED = -22,
            XR_ERROR_LAYER_INVALID = -23,
            XR_ERROR_LAYER_LIMIT_EXCEEDED = -24,
            XR_ERROR_SWAPCHAIN_RECT_INVALID = -25,
            XR_ERROR_SWAPCHAIN_FORMAT_UNSUPPORTED = -26,
            XR_ERROR_ACTION_TYPE_MISMATCH = -27,
            XR_ERROR_SESSION_NOT_READY = -28,
            XR_ERROR_SESSION_NOT_STOPPING = -29,
            XR_ERROR_TIME_INVALID = -30,
            XR_ERROR_REFERENCE_SPACE_UNSUPPORTED = -31,
            XR_ERROR_FILE_ACCESS_ERROR = -32,
            XR_ERROR_FILE_CONTENTS_INVALID = -33,
            XR_ERROR_FORM_FACTOR_UNSUPPORTED = -34,
            XR_ERROR_FORM_FACTOR_UNAVAILABLE = -35,
            XR_ERROR_API_LAYER_NOT_PRESENT = -36,
            XR_ERROR_CALL_ORDER_INVALID = -37,
            XR_ERROR_GRAPHICS_DEVICE_INVALID = -38,
            XR_ERROR_POSE_INVALID = -39,
            XR_ERROR_INDEX_OUT_OF_RANGE = -40,
            XR_ERROR_VIEW_CONFIGURATION_TYPE_UNSUPPORTED = -41,
            XR_ERROR_ENVIRONMENT_BLEND_MODE_UNSUPPORTED = -42,
            XR_ERROR_NAME_DUPLICATED = -44,
            XR_ERROR_NAME_INVALID = -45,
            XR_ERROR_ACTIONSET_NOT_ATTACHED = -46,
            XR_ERROR_ACTIONSETS_ALREADY_ATTACHED = -47,
            XR_ERROR_LOCALIZED_NAME_DUPLICATED = -48,
            XR_ERROR_LOCALIZED_NAME_INVALID = -49,
            XR_ERROR_GRAPHICS_REQUIREMENTS_CALL_MISSING = -50,
            XR_ERROR_RUNTIME_UNAVAILABLE = -51,
            XR_ERROR_ANDROID_THREAD_SETTINGS_ID_INVALID_KHR = -1000003000,
            XR_ERROR_ANDROID_THREAD_SETTINGS_FAILURE_KHR = -1000003001,
            XR_ERROR_CREATE_SPATIAL_ANCHOR_FAILED_MSFT = -1000039001,
            XR_ERROR_SECONDARY_VIEW_CONFIGURATION_TYPE_NOT_ENABLED_MSFT = -1000053000,
            XR_ERROR_CONTROLLER_MODEL_KEY_INVALID_MSFT = -1000055000,
            XR_ERROR_REPROJECTION_MODE_UNSUPPORTED_MSFT = -1000066000,
            XR_ERROR_COMPUTE_NEW_SCENE_NOT_COMPLETED_MSFT = -1000097000,
            XR_ERROR_SCENE_COMPONENT_ID_INVALID_MSFT = -1000097001,
            XR_ERROR_SCENE_COMPONENT_TYPE_MISMATCH_MSFT = -1000097002,
            XR_ERROR_SCENE_MESH_BUFFER_ID_INVALID_MSFT = -1000097003,
            XR_ERROR_SCENE_COMPUTE_FEATURE_INCOMPATIBLE_MSFT = -1000097004,
            XR_ERROR_SCENE_COMPUTE_CONSISTENCY_MISMATCH_MSFT = -1000097005,
            XR_ERROR_DISPLAY_REFRESH_RATE_UNSUPPORTED_FB = -1000101000,
            XR_ERROR_COLOR_SPACE_UNSUPPORTED_FB = -1000108000,
            XR_ERROR_SPATIAL_ANCHOR_NAME_NOT_FOUND_MSFT = -1000142001,
            XR_ERROR_SPATIAL_ANCHOR_NAME_INVALID_MSFT = -1000142002,
            XR_RESULT_MAX_ENUM = 0x7FFFFFFF
        }
    public enum XrSceneComputeConsistencyMSFT
    {
        SnapshotComplete = 1,
        SnapshotIncompleteFast = 2,
        OcclusionOptimized = 3,
    }

    public enum XrSceneBoundType
    {
        Sphere = 1,
        OrientedBox = 2,
        Frustum = 3,
    }

    #region XR_MSFT_scene_understanding

    public enum XrSceneComputeFeatureMSFT
    {
        XR_SCENE_COMPUTE_FEATURE_PLANE_MSFT = 1,
        XR_SCENE_COMPUTE_FEATURE_PLANE_MESH_MSFT = 2,
        XR_SCENE_COMPUTE_FEATURE_VISUAL_MESH_MSFT = 3,
        XR_SCENE_COMPUTE_FEATURE_COLLIDER_MESH_MSFT = 4,
        XR_SCENE_COMPUTE_FEATURE_SERIALIZE_SCENE_MSFT = 1000098000,
        XR_SCENE_COMPUTE_FEATURE_OCCLUSION_HINT_MSFT = 1000099000,
        XR_SCENE_COMPUTE_FEATURE_MAX_ENUM_MSFT = 0x7FFFFFFF
    }

    public enum XrMeshComputeLodMSFT
    {
        Coarse = 1,
        Medium = 2,
        Fine = 3,
        Unlimited = 4,
    }

    public enum XrSceneComponentTypeMSFT
    {
        XR_SCENE_COMPONENT_TYPE_INVALID_MSFT = -1,
        XR_SCENE_COMPONENT_TYPE_OBJECT_MSFT = 1,
        XR_SCENE_COMPONENT_TYPE_PLANE_MSFT = 2,
        XR_SCENE_COMPONENT_TYPE_VISUAL_MESH_MSFT = 3,
        XR_SCENE_COMPONENT_TYPE_COLLIDER_MESH_MSFT = 4,
        XR_SCENE_COMPONENT_TYPE_SERIALIZED_SCENE_FRAGMENT_MSFT = 1000098000,
        XR_SCENE_COMPONENT_TYPE_MAX_ENUM_MSFT = 0x7FFFFFFF
    }

    public enum XrSceneObjectTypeMSFT
    {
        XR_SCENE_OBJECT_TYPE_UNCATEGORIZED_MSFT = -1,
        XR_SCENE_OBJECT_TYPE_BACKGROUND_MSFT = 1,
        XR_SCENE_OBJECT_TYPE_WALL_MSFT = 2,
        XR_SCENE_OBJECT_TYPE_FLOOR_MSFT = 3,
        XR_SCENE_OBJECT_TYPE_CEILING_MSFT = 4,
        XR_SCENE_OBJECT_TYPE_PLATFORM_MSFT = 5,
        XR_SCENE_OBJECT_TYPE_INFERRED_MSFT = 6,
        XR_SCENE_OBJECT_TYPE_MAX_ENUM_MSFT = 0x7FFFFFFF
    }

    public enum XrScenePlaneAlignmentTypeMSFT
    {
        XR_SCENE_PLANE_ALIGNMENT_TYPE_NON_ORTHOGONAL_MSFT = 0,
        XR_SCENE_PLANE_ALIGNMENT_TYPE_HORIZONTAL_MSFT = 1,
        XR_SCENE_PLANE_ALIGNMENT_TYPE_VERTICAL_MSFT = 2,
        XR_SCENE_PLANE_ALIGNMENT_TYPE_MAX_ENUM_MSFT = 0x7FFFFFFF
    }

    public enum XrSceneComputeStateMSFT
    {
        XR_SCENE_COMPUTE_STATE_NONE_MSFT = 0,
        XR_SCENE_COMPUTE_STATE_UPDATING_MSFT = 1,
        XR_SCENE_COMPUTE_STATE_COMPLETED_MSFT = 2,
        XR_SCENE_COMPUTE_STATE_COMPLETED_WITH_ERROR_MSFT = 3,
        XR_SCENE_COMPUTE_STATE_MAX_ENUM_MSFT = 0x7FFFFFFF
    }

    public struct XrUuidMSFT
    {
        public byte    byte0;
        public byte    byte1;
        public byte    byte2;
        public byte    byte3;
        public byte    byte4;
        public byte    byte5;
        public byte    byte6;
        public byte    byte7;
        public byte    byte8;
        public byte    byte9;
        public byte    byte10;
        public byte    byte11;
        public byte    byte12;
        public byte    byte13;
        public byte    byte14;
        public byte    byte15;
        public byte    byte16;

    }

    public struct XrSceneObserverCreateInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
    }

    public struct XrSceneCreateInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
    }

    public struct XrSceneSphereBoundMSFT
    {
        public XrVector3f center;
        public float radius;
    }

    public struct XrSceneOrientedBoxBoundMSFT
    {
        public XrPosef pose;
        public XrVector3f extents;
    }

    public struct XrSceneFrustumBoundMSFT
    {
        public XrPosef pose;
        public XrFovf fov;
        public float farDistance;
    }

    public struct XrSceneBoundsMSFT
    {
        public ulong space;
        public long time;
        public uint sphereCount;

        // XrSceneSphereBoundMSFT
        public IntPtr         spheres;
        public uint                              boxCount;

        // XrSceneOrientedBoxBoundMSFT
        public IntPtr    boxes;
        public uint                              frustumCount;

        // XrSceneFrustumBoundMSFT
        public IntPtr        frustums;
    }

    public struct XrNewSceneComputeInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public uint                            requestedFeatureCount;

        // XrSceneComputeFeatureMSFT array
        public IntPtr    requestedFeatures;
        public uint                            disableInferredSceneObjects;
        public XrSceneBoundsMSFT                   bounds;
    }

    // XrVisualMeshComputeLodInfoMSFT extends XrNewSceneComputeInfoMSFT
    public struct XrVisualMeshComputeLodInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public XrMeshComputeLodMSFT        lod;
    }

    public struct XrSceneComponentMSFT
    {
        public XrSceneComponentTypeMSFT    componentType;
        public XrUuidMSFT                  componentId;
        public XrUuidMSFT                  parentObjectId;
        public long                      updateTime;
    }

    public struct XrSceneComponentsMSFT
    {
        public XrStructureType type;
        public IntPtr       next;
        public uint                 componentCapacityInput;
        public uint                 componentCountOutput;

        // XrSceneComponentMSFT array
        public IntPtr    components;
    }

    public struct XrSceneComponentsGetInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public XrSceneComponentTypeMSFT    componentType;
    }

    public struct XrSceneComponentLocationMSFT
    {
        public ulong    flags;
        public XrPosef                 pose;
    }

    public struct XrSceneComponentLocationsMSFT
    {
        public XrStructureType type;
        public IntPtr               next;
        public uint                         locationCount;

        // XrSceneComponentLocationMSFT array
        public IntPtr    locations;
    }

    public struct XrSceneComponentsLocateInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;

        // XrSpace
        public ulong                        baseSpace;

        // XrTime
        public long                      time;
        public uint                    idCount;

        // XrUuidMSFT array
        public IntPtr           ids;
    }

    public struct XrSceneObjectMSFT
    {
        public XrSceneObjectTypeMSFT    objectType;
    }

    // XrSceneObjectsMSFT extends XrSceneComponentsMSFT
    public struct XrSceneObjectsMSFT
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint              sceneObjectCount;

        // XrSceneObjectMSFT array
        public IntPtr    sceneObjects;
    }

    // XrSceneComponentParentFilterInfoMSFT extends XrSceneComponentsGetInfoMSFT
    public struct XrSceneComponentParentFilterInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public XrUuidMSFT                  parentObjectId;
    }

    // XrSceneObjectTypesFilterInfoMSFT extends XrSceneComponentsGetInfoMSFT
    public struct XrSceneObjectTypesFilterInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public uint                        objectTypeCount;

        // XrSceneObjectTypeMSFT array
        public IntPtr    objectTypes;
    }

    public struct XrScenePlaneMSFT
    {
        public XrScenePlaneAlignmentTypeMSFT    alignment;
        public XrExtent2Df                      size;
        public ulong                         meshBufferId;

        // XrBool32
        public uint                         supportsIndicesUint16;
    }

    // XrScenePlanesMSFT extends XrSceneComponentsMSFT
    public struct XrScenePlanesMSFT
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint              scenePlaneCount;

        // XrScenePlaneMSFT array
        public IntPtr     scenePlanes;
    }

    // XrScenePlaneAlignmentFilterInfoMSFT extends XrSceneComponentsGetInfoMSFT
    public struct XrScenePlaneAlignmentFilterInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public uint                                alignmentCount;

        // XrScenePlaneAlignmentTypeMSFT array
        public IntPtr    alignments;
    }

    public struct XrSceneMeshMSFT
    {
        public ulong    meshBufferId;

        // XrBool32
        public uint    supportsIndicesUint16;
    }

    // XrSceneMeshesMSFT extends XrSceneComponentsMSFT
    public struct XrSceneMeshesMSFT
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint              sceneMeshCount;

        // XrSceneMeshMSFT array
        public IntPtr      sceneMeshes;
    }

    public struct XrSceneMeshBuffersGetInfoMSFT
    {
        public XrStructureType type;
        public IntPtr next;
        public ulong                    meshBufferId;
    }

    public struct XrSceneMeshBuffersMSFT
    {
        public XrStructureType type;
        public IntPtr    next;
    }

    public struct XrSceneMeshVertexBufferMSFT
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint              vertexCapacityInput;
        public uint              vertexCountOutput;

        // XrVector3f array
        public IntPtr           vertices;
    }

    public struct XrSceneMeshIndicesUint32MSFT
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint              indexCapacityInput;
        public uint              indexCountOutput;

        // uint32_t array
        public IntPtr             indices;
    }

    public struct XrSceneMeshIndicesUint16MSFT
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint              indexCapacityInput;
        public uint              indexCountOutput;

        // uint16_t array
        public IntPtr             indices;
    }

    public struct XrSystemPassThroughCameraInfoHTC
    {
        public float focalLengthX;
        public float focalLengthY;
        public float opticalCenterX;
        public float opticalCenterY;
        public uint imageWidth;
        public uint imageHeight;
        public uint imageChannelCount;
    }
    public struct XrSystemPassThroughPropertiesHTC
    {
        public XrStructureType type;
        public IntPtr    next;
        public uint supportsPassThrough;
        XrSystemPassThroughCameraInfoHTC    leftCameraInfo;
        XrSystemPassThroughCameraInfoHTC    rightCameraInfo;
        public int deviceType;
        public long format;
    }
#endregion
    }
    
}