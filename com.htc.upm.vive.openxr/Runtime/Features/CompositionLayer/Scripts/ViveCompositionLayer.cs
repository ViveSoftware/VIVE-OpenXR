// Copyright HTC Corporation All Rights Reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using AOT;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace VIVE.OpenXR.CompositionLayer
{
#if UNITY_EDITOR
	[OpenXRFeature(UiName = "VIVE XR Composition Layer",
		Desc = "Enable this feature to use the Composition Layer.",
		Company = "HTC",
		DocumentationLink = "..\\Documentation",
		OpenxrExtensionStrings = kOpenxrExtensionStrings,
		Version = "1.0.0",
		BuildTargetGroups = new[] { BuildTargetGroup.Android },
		FeatureId = featureId
	)]
#endif
	public class ViveCompositionLayer : OpenXRFeature
	{
		const string LOG_TAG = "VIVE.OpenXR.ViveCompositionLayer";
		static void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
		static void WARNING(string msg) { Debug.LogWarning(LOG_TAG + " " + msg); }
		static void ERROR(string msg) { Debug.LogError(LOG_TAG + " " + msg); }

		/// <summary>
		/// Enable auto fallback or Not.
		/// </summary>
		public bool enableAutoFallback = false;

		/// <summary>
		/// The feature id string. This is used to give the feature a well known id for reference.
		/// </summary>
		public const string featureId = "vive.openxr.feature.compositionlayer";

		public const string kOpenxrExtensionStrings = "";

		#region OpenXR Life Cycle
		private bool m_XrInstanceCreated = false;
		/// <summary>
		/// The XR instance is created or not.
		/// </summary>
		public bool XrInstanceCreated
		{
			get { return m_XrInstanceCreated; }
		}
		private XrInstance m_XrInstance = 0;
		protected override bool OnInstanceCreate(ulong xrInstance)
		{
			foreach (string kOpenxrExtensionString in kOpenxrExtensionStrings.Split(' '))
			{
				if (!OpenXRRuntime.IsExtensionEnabled(kOpenxrExtensionString))
				{
					WARNING("OnInstanceCreate() " + kOpenxrExtensionString + " is NOT enabled.");
				}
			}

			m_XrInstanceCreated = true;
			m_XrInstance = xrInstance;
			DEBUG("OnInstanceCreate() " + m_XrInstance);

			return GetXrFunctionDelegates(m_XrInstance);
		}

		protected override void OnInstanceDestroy(ulong xrInstance)
		{
			m_XrInstanceCreated = false;
			DEBUG("OnInstanceDestroy() " + m_XrInstance);
		}

		private XrSystemId m_XrSystemId = 0;
		protected override void OnSystemChange(ulong xrSystem)
		{
			m_XrSystemId = xrSystem;
			DEBUG("OnSystemChange() " + m_XrSystemId);
		}

		private bool m_XrSessionCreated = false;
		/// <summary>
		/// The XR session is created or not.
		/// </summary>
		public bool XrSessionCreated
		{
			get { return m_XrSessionCreated; }
		}
		private XrSession m_XrSession = 0;
		protected override void OnSessionCreate(ulong xrSession)
		{
			m_XrSession = xrSession;
			m_XrSessionCreated = true;
			DEBUG("OnSessionCreate() " + m_XrSession);
		}

		private bool m_XrSessionEnding = false;
		/// <summary>
		/// The XR session is ending or not.
		/// </summary>
		public bool XrSessionEnding
		{
			get { return m_XrSessionEnding; }
		}

		private XrSpace m_WorldLockSpaceOriginOnHead = 0, m_WorldLockSpaceOriginOnFloor = 0, m_HeadLockSpace = 0;
		/// <summary>
		/// The XrSpace of world lock space origin on head.
		/// </summary>
		public XrSpace WorldLockSpaceOriginOnHead
		{
			get { return m_WorldLockSpaceOriginOnHead; }
		}
		/// <summary>
		/// The XrSpace of world lock space origin on floor.
		/// </summary>
		public XrSpace WorldLockSpaceOriginOnFloor
		{
			get { return m_WorldLockSpaceOriginOnFloor; }
		}
		/// <summary>
		/// The XrSpace of head lock space.
		/// </summary>
		public XrSpace HeadLockSpace
		{
			get { return m_HeadLockSpace; }
		}

		protected override void OnSessionBegin(ulong xrSession)
		{
			m_XrSessionEnding = false;
			DEBUG("OnSessionBegin() " + m_XrSession);

			// Enumerate supported reference space types and create the XrSpace.
			XrReferenceSpaceType[] spaces = new XrReferenceSpaceType[Enum.GetNames(typeof(XrReferenceSpaceType)).Count()];
			UInt32 spaceCountOutput;
			if (EnumerateReferenceSpaces(
				spaceCapacityInput: 0,
				spaceCountOutput: out spaceCountOutput,
				spaces: out spaces[0]) == XrResult.XR_SUCCESS)
			{
				//DEBUG("spaceCountOutput: " + spaceCountOutput);

				Array.Resize(ref spaces, (int)spaceCountOutput);
				if (EnumerateReferenceSpaces(
					spaceCapacityInput: spaceCountOutput,
					spaceCountOutput: out spaceCountOutput,
					spaces: out spaces[0]) == XrResult.XR_SUCCESS)
				{
					if (spaces.Contains(XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_LOCAL))
					{
						XrReferenceSpaceCreateInfo referenceSpaceCreateInfoWorldLock;
						referenceSpaceCreateInfoWorldLock.type = XrStructureType.XR_TYPE_REFERENCE_SPACE_CREATE_INFO;
						referenceSpaceCreateInfoWorldLock.next = IntPtr.Zero;
						referenceSpaceCreateInfoWorldLock.referenceSpaceType = XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_LOCAL;
						referenceSpaceCreateInfoWorldLock.poseInReferenceSpace.orientation = new XrQuaternionf(0, 0, 0, 1);
						referenceSpaceCreateInfoWorldLock.poseInReferenceSpace.position = new XrVector3f(0, 0, 0);

						if (CreateReferenceSpace(
						createInfo: ref referenceSpaceCreateInfoWorldLock,
						space: out m_WorldLockSpaceOriginOnHead) == XrResult.XR_SUCCESS)
						{
							//DEBUG("CreateReferenceSpace: " + m_WorldLockSpaceOriginOnHead);
						}
						else
						{
							ERROR("CreateReferenceSpace for world lock layers on head failed.");
						}
					}
					else
					{
						ERROR("CreateReferenceSpace no space type for world lock on head layers.");
					}

					if (spaces.Contains(XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_STAGE))
					{
						XrReferenceSpaceCreateInfo referenceSpaceCreateInfoWorldLock;
						referenceSpaceCreateInfoWorldLock.type = XrStructureType.XR_TYPE_REFERENCE_SPACE_CREATE_INFO;
						referenceSpaceCreateInfoWorldLock.next = IntPtr.Zero;
						referenceSpaceCreateInfoWorldLock.referenceSpaceType = XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_STAGE;
						referenceSpaceCreateInfoWorldLock.poseInReferenceSpace.orientation = new XrQuaternionf(0, 0, 0, 1);
						referenceSpaceCreateInfoWorldLock.poseInReferenceSpace.position = new XrVector3f(0, 0, 0);

						if (CreateReferenceSpace(
						createInfo: ref referenceSpaceCreateInfoWorldLock,
						space: out m_WorldLockSpaceOriginOnFloor) == XrResult.XR_SUCCESS)
						{
							//DEBUG("CreateReferenceSpace: " + m_WorldLockSpaceOriginOnFloor);
						}
						else
						{
							ERROR("CreateReferenceSpace for world lock layers on floor failed.");
						}
					}
					else
					{
						ERROR("CreateReferenceSpace no space type for world lock on floor layers.");
					}

					if (spaces.Contains(XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_VIEW))
					{
						XrReferenceSpaceCreateInfo referenceSpaceCreateInfoHeadLock;
						referenceSpaceCreateInfoHeadLock.type = XrStructureType.XR_TYPE_REFERENCE_SPACE_CREATE_INFO;
						referenceSpaceCreateInfoHeadLock.next = IntPtr.Zero;
						referenceSpaceCreateInfoHeadLock.referenceSpaceType = XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_VIEW;
						referenceSpaceCreateInfoHeadLock.poseInReferenceSpace.orientation = new XrQuaternionf(0, 0, 0, 1);
						referenceSpaceCreateInfoHeadLock.poseInReferenceSpace.position = new XrVector3f(0, 0, 0);

						if (CreateReferenceSpace(
						createInfo: ref referenceSpaceCreateInfoHeadLock,
						space: out m_HeadLockSpace) == XrResult.XR_SUCCESS)
						{
							//DEBUG("CreateReferenceSpace: " + m_HeadLockSpace);
						}
						else
						{
							ERROR("CreateReferenceSpace for head lock layers failed.");
						}
					}
					else
					{
						ERROR("CreateReferenceSpace no space type for head lock layers.");
					}
				}
				else
				{
					ERROR("EnumerateReferenceSpaces(" + spaceCountOutput + ") failed.");
				}
			}
			else
			{
				ERROR("EnumerateReferenceSpaces(0) failed.");
			}
		}

		protected override void OnSessionEnd(ulong xrSession)
		{
			m_XrSessionEnding = true;
			DEBUG("OnSessionEnd() " + m_XrSession);
		}


		protected override void OnSessionDestroy(ulong xrSession)
		{
			m_XrSessionCreated = false;
			DEBUG("OnSessionDestroy() " + xrSession);

			if (m_HeadLockSpace != 0)
			{
				DestroySpace(m_HeadLockSpace);
				m_HeadLockSpace = 0;
			}
			if (m_WorldLockSpaceOriginOnFloor != 0)
			{
				DestroySpace(m_WorldLockSpaceOriginOnFloor);
				m_WorldLockSpaceOriginOnFloor = 0;
			}
			if (m_WorldLockSpaceOriginOnHead != 0)
			{
				DestroySpace(m_WorldLockSpaceOriginOnHead);
				m_WorldLockSpaceOriginOnHead = 0;
			}
		}

		/// <summary>
		/// The current XR Session state.
		/// </summary>
		public XrSessionState XrSessionCurrentState
		{
			get { return m_XrSessionNewState; }
		}
		private XrSessionState m_XrSessionNewState = XrSessionState.XR_SESSION_STATE_UNKNOWN;
		private XrSessionState m_XrSessionOldState = XrSessionState.XR_SESSION_STATE_UNKNOWN;
		protected override void OnSessionStateChange(int oldState, int newState)
		{
			DEBUG("OnSessionStateChange() oldState: " + oldState + " newState:" + newState);

			if (Enum.IsDefined(typeof(XrSessionState), oldState))
			{
				m_XrSessionOldState = (XrSessionState)oldState;
			}
			else
			{
				DEBUG("OnSessionStateChange() oldState undefined");
			}

			if (Enum.IsDefined(typeof(XrSessionState), newState)) 
			{
				m_XrSessionNewState = (XrSessionState)newState;
			}
			else
			{
				DEBUG("OnSessionStateChange() newState undefined");
			}
			
		}
		#endregion

		#region OpenXR function delegates
		/// xrGetInstanceProcAddr
		OpenXRHelper.xrGetInstanceProcAddrDelegate XrGetInstanceProcAddr;

		/// xrGetSystemProperties
		OpenXRHelper.xrGetSystemPropertiesDelegate xrGetSystemProperties;
		/// <summary>
		/// Helper function to get this feature' properties.
		/// See <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrGetSystemProperties">xrGetSystemProperties</see>
		/// </summary>
		public XrResult GetSystemProperties(ref XrSystemProperties properties)
		{
			if (m_XrInstanceCreated)
			{
				return xrGetSystemProperties(m_XrInstance, m_XrSystemId, ref properties);
			}

			return XrResult.XR_ERROR_INSTANCE_LOST;
		}

		/// xrEnumerateReferenceSpaces
		OpenXRHelper.xrEnumerateReferenceSpacesDelegate xrEnumerateReferenceSpaces;
		/// <summary>
		/// Enumerate available reference spaces
		/// See <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/xrEnumerateReferenceSpaces.html">xrEnumerateReferenceSpaces</see>
		/// </summary>
		public XrResult EnumerateReferenceSpaces(UInt32 spaceCapacityInput, out UInt32 spaceCountOutput, out XrReferenceSpaceType spaces)
		{
			if (!m_XrSessionCreated)
			{
				spaceCountOutput = 0;
				spaces = XrReferenceSpaceType.XR_REFERENCE_SPACE_TYPE_UNBOUNDED_MSFT;
				return XrResult.XR_ERROR_SESSION_NOT_RUNNING;
			}

			return xrEnumerateReferenceSpaces(m_XrSession, spaceCapacityInput, out spaceCountOutput, out spaces);
		}

		/// xrCreateReferenceSpace
		OpenXRHelper.xrCreateReferenceSpaceDelegate xrCreateReferenceSpace;
		/// <summary>
		/// Creates a reference space
		/// See <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/xrCreateReferenceSpace.html">xrCreateReferenceSpace</see>
		/// </summary>
		public XrResult CreateReferenceSpace(ref XrReferenceSpaceCreateInfo createInfo, out XrSpace space)
		{
			if (!m_XrSessionCreated)
			{
				space = 0;
				return XrResult.XR_ERROR_SESSION_NOT_RUNNING;
			}

			return xrCreateReferenceSpace(m_XrSession, ref createInfo, out space);
		}

		/// xrDestroySpace
		OpenXRHelper.xrDestroySpaceDelegate xrDestroySpace;
		/// <summary>
		/// Destroys an XrSpace
		/// See <see href="https://registry.khronos.org/OpenXR/specs/1.0/man/html/xrDestroySpace.html">xrDestroySpace</see>
		/// </summary>
		public XrResult DestroySpace(XrSpace space)
		{
			if (space != 0)
			{
				return xrDestroySpace(space);
			}
			return XrResult.XR_ERROR_REFERENCE_SPACE_UNSUPPORTED;
		}

		private bool GetXrFunctionDelegates(XrInstance xrInstance)
        {
            /// xrGetInstanceProcAddr
            if (xrGetInstanceProcAddr != null && xrGetInstanceProcAddr != IntPtr.Zero)
            {
                DEBUG("Get function pointer of xrGetInstanceProcAddr.");
                XrGetInstanceProcAddr = Marshal.GetDelegateForFunctionPointer(
                    xrGetInstanceProcAddr,
                    typeof(OpenXRHelper.xrGetInstanceProcAddrDelegate)) as OpenXRHelper.xrGetInstanceProcAddrDelegate;
            }
            else
            {
                ERROR("xrGetInstanceProcAddr");
                return false;
            }

            IntPtr funcPtr = IntPtr.Zero;
            /// xrGetSystemProperties
            if (XrGetInstanceProcAddr(xrInstance, "xrGetSystemProperties", out funcPtr) == XrResult.XR_SUCCESS)
            {
                if (funcPtr != IntPtr.Zero)
                {
                    DEBUG("Get function pointer of xrGetSystemProperties.");
                    xrGetSystemProperties = Marshal.GetDelegateForFunctionPointer(
                        funcPtr,
                        typeof(OpenXRHelper.xrGetSystemPropertiesDelegate)) as OpenXRHelper.xrGetSystemPropertiesDelegate;
                }
            }
            else
            {
                ERROR("xrGetSystemProperties");
                return false;
            }
			/// xrEnumerateReferenceSpaces
			if (XrGetInstanceProcAddr(xrInstance, "xrEnumerateReferenceSpaces", out funcPtr) == XrResult.XR_SUCCESS)
			{
				if (funcPtr != IntPtr.Zero)
				{
					DEBUG("Get function pointer of xrEnumerateReferenceSpaces.");
					xrEnumerateReferenceSpaces = Marshal.GetDelegateForFunctionPointer(
						funcPtr,
						typeof(OpenXRHelper.xrEnumerateReferenceSpacesDelegate)) as OpenXRHelper.xrEnumerateReferenceSpacesDelegate;
				}
			}
			else
			{
				ERROR("xrEnumerateReferenceSpaces");
				return false;
			}
			/// xrCreateReferenceSpace
			if (XrGetInstanceProcAddr(xrInstance, "xrCreateReferenceSpace", out funcPtr) == XrResult.XR_SUCCESS)
			{
				if (funcPtr != IntPtr.Zero)
				{
					DEBUG("Get function pointer of xrCreateReferenceSpace.");
					xrCreateReferenceSpace = Marshal.GetDelegateForFunctionPointer(
						funcPtr,
						typeof(OpenXRHelper.xrCreateReferenceSpaceDelegate)) as OpenXRHelper.xrCreateReferenceSpaceDelegate;
				}
			}
			else
			{
				ERROR("xrCreateReferenceSpace");
				return false;
			}
			/// xrDestroySpace
			if (XrGetInstanceProcAddr(xrInstance, "xrDestroySpace", out funcPtr) == XrResult.XR_SUCCESS)
			{
				if (funcPtr != IntPtr.Zero)
				{
					DEBUG("Get function pointer of xrDestroySpace.");
					xrDestroySpace = Marshal.GetDelegateForFunctionPointer(
						funcPtr,
						typeof(OpenXRHelper.xrDestroySpaceDelegate)) as OpenXRHelper.xrDestroySpaceDelegate;
				}
			}
			else
			{
				ERROR("xrDestroySpace");
				return false;
			}

			if (CompositionLayer_GetFuncAddrs(xrInstance, xrGetInstanceProcAddr) == XrResult.XR_SUCCESS)
			{
				DEBUG("Get function pointers in native.");
			}
			else
			{
				ERROR("CompositionLayer_GetFuncAddrs");
				return false;
			}

			return true;
        }
		#endregion

		#region Wrapper Functions
		private const string ExtLib = "viveopenxr";
		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_GetExternalSurfaceObj")]
		public static extern IntPtr VIVEOpenXR_Compositionlayer_GetExternalSurfaceObj();
		public IntPtr Compositionlayer_GetExternalSurfaceObj()
		{
			return VIVEOpenXR_Compositionlayer_GetExternalSurfaceObj();
		}

		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_GetExternalSurfaceObj2")]
		public static extern IntPtr VIVEOpenXR_Compositionlayer_GetExternalSurfaceObj2(int layerID);
		public IntPtr Compositionlayer_GetExternalSurfaceObj2(int layerID)
		{
			return VIVEOpenXR_Compositionlayer_GetExternalSurfaceObj2(layerID);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_Init")]
		public static extern int VIVEOpenXR_CompositionLayer_Init(XrSession session, uint textureWidth, uint textureHeight, GraphicsAPI graphicsAPI, bool isDynamic, bool isProtected, out uint imageCount, bool isExternal);
		/// <summary>
		/// Init composion layer.
		/// </summary>
		public int CompositionLayer_Init(uint textureWidth, uint textureHeight, GraphicsAPI graphicsAPI, bool isDynamic, bool isProtected, out uint imageCount, bool isExternal = false)
		{
			if (!m_XrSessionCreated)
			{
				ERROR("Xr Session not found");
				imageCount = 0;
				return 0;
			}

			return VIVEOpenXR_CompositionLayer_Init(m_XrSession, textureWidth, textureHeight, graphicsAPI, isDynamic, isProtected, out imageCount, isExternal);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_GetTexture")]
		public static extern IntPtr VIVEOpenXR_CompositionLayer_GetTexture(int layerID, out uint imageIndex);
		/// <summary>
		/// Get composition layer texture.
		/// </summary>
		public IntPtr CompositionLayer_GetTexture(int layerID, out uint imageIndex)
		{
			return VIVEOpenXR_CompositionLayer_GetTexture(layerID, out imageIndex);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_ReleaseTexture")]
		public static extern bool VIVEOpenXR_CompositionLayer_ReleaseTexture(int layerID);
		/// <summary>
		/// release composition layer texture.
		/// </summary>
		public bool CompositionLayer_ReleaseTexture(int layerID)
		{
			return VIVEOpenXR_CompositionLayer_ReleaseTexture(layerID);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_Destroy")]
		public static extern bool VIVEOpenXR_CompositionLayer_Destroy(int layerID);
		/// <summary>
		/// destroy composition layer.
		/// </summary>
		public bool CompositionLayer_Destroy(int layerID)
		{
			return VIVEOpenXR_CompositionLayer_Destroy(layerID);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "submit_CompositionLayerQuad")]
		public static extern void VIVEOpenXR_Submit_CompositionLayerQuad(XrCompositionLayerQuad quad, LayerType layerType, uint compositionDepth, int layerID);
		/// <summary>
		/// submit compostion layer of type quad.
		/// </summary>
		public void Submit_CompositionLayerQuad(XrCompositionLayerQuad quad, LayerType layerType, uint compositionDepth, int layerID)
		{
			VIVEOpenXR_Submit_CompositionLayerQuad(quad, layerType, compositionDepth, layerID);
			return;
		}
		#endregion

		#region Hook native functions
		protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
		{
			DEBUG("CompositionLayer_GetInstanceProcAddr");
			return CompositionLayer_GetInstanceProcAddr(func);
		}

		[DllImport(ExtLib, EntryPoint = "compositionlayer_intercept_xrGetInstanceProcAddr")]
		private static extern IntPtr CompositionLayer_GetInstanceProcAddr(IntPtr func);

		[DllImportAttribute(ExtLib, EntryPoint = "compositionlayer_GetFuncAddrs")]
		public static extern XrResult VIVEOpenXR_CompositionLayer_GetFuncAddrs(XrInstance xrInstance, IntPtr xrGetInstanceProcAddrFuncPtr);
		/// <summary>
		/// get function address of composition layer.
		/// </summary>
		public XrResult CompositionLayer_GetFuncAddrs(XrInstance xrInstance, IntPtr xrGetInstanceProcAddrFuncPtr)
		{
			return VIVEOpenXR_CompositionLayer_GetFuncAddrs(xrInstance, xrGetInstanceProcAddrFuncPtr);
		}

		#endregion
	}
}
