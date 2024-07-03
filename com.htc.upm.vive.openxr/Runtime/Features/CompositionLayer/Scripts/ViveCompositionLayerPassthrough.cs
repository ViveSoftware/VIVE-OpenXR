﻿// Copyright HTC Corporation All Rights Reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using AOT;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace VIVE.OpenXR.CompositionLayer.Passthrough
{
#if UNITY_EDITOR
	[OpenXRFeature(UiName = "VIVE XR Composition Layer (Passthrough)",
		Desc = "Enable this feature to use the HTC Passthrough feature.",
		Company = "HTC",
		DocumentationLink = "..\\Documentation",
		OpenxrExtensionStrings = kOpenxrExtensionStrings,
		Version = "1.0.0",
		BuildTargetGroups = new[] { BuildTargetGroup.Android ,BuildTargetGroup.Standalone},
		FeatureId = featureId
	)]
#endif
	public class ViveCompositionLayerPassthrough : OpenXRFeature
	{
		const string LOG_TAG = "VIVE.OpenXR.ViveCompositionLayerPassthrough";
		static void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
		static void WARNING(string msg) { Debug.LogWarning(LOG_TAG + " " + msg); }
		static void ERROR(string msg) { Debug.LogError(LOG_TAG + " " + msg); }

		private List<int> passthroughIDList = new List<int>();
		/// <summary>
		/// The List of passthrough ID.
		/// </summary>
		public List<int> PassthroughIDList { get{ return new List<int>(passthroughIDList); } }

		private List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();

		/// <summary>
		/// The feature id string. This is used to give the feature a well known id for reference.
		/// </summary>
		public const string featureId = "vive.openxr.feature.compositionlayer.passthrough";

		/// <summary>
		/// The extension string.
		/// </summary>
		public const string kOpenxrExtensionStrings = "XR_HTC_passthrough";

		private bool m_HTCPassthroughExtensionEnabled = true;
		/// <summary>
		/// The HTC Passthrough extension is enabled or not.
		/// </summary>
		public bool HTCPassthroughExtensionEnabled
		{
			get { return m_HTCPassthroughExtensionEnabled; }
		}

		#region OpenXR Life Cycle
		private bool m_XrInstanceCreated = false;
		/// <summary>
		/// The XR instance is created or not.
		/// </summary>
		public bool XrInstanceCreated
		{
			get { return m_XrInstanceCreated; }
		}
#if UNITY_STANDALONE
		private static IntPtr xrGetInstanceProcAddr_prev;
		private static IntPtr XrEndFrame_prev;
		private static IntPtr XrWaitFrame_prev;
		private static List<IntPtr> layerListOrigin = new List<IntPtr>();
		private static List<IntPtr> layerListModified = new List<IntPtr>();
		private static IntPtr layersModified = Marshal.AllocHGlobal((int)(Marshal.SizeOf(typeof(IntPtr)) * 30)); //Preallocate a layer buffer with sufficient size and reuse it for each frame.
		protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
		{
			UnityEngine.Debug.Log("EXT: registering our own xrGetInstanceProcAddr");
			xrGetInstanceProcAddr_prev = func;
			return Marshal.GetFunctionPointerForDelegate(Intercept_xrGetInstanceProcAddr);
		}
		[MonoPInvokeCallback(typeof(OpenXRHelper.xrGetInstanceProcAddrDelegate))]
		private static XrResult InterceptXrEndFrame_xrGetInstanceProcAddr(XrInstance instance, string name, out IntPtr function)
		{
			if (xrGetInstanceProcAddr_prev == null || xrGetInstanceProcAddr_prev == IntPtr.Zero)
			{
				UnityEngine.Debug.LogError("xrGetInstanceProcAddr_prev is null");
				function = IntPtr.Zero;
				return XrResult.XR_ERROR_VALIDATION_FAILURE;
			}

			// Get delegate of old xrGetInstanceProcAddr.
			var xrGetProc = Marshal.GetDelegateForFunctionPointer<OpenXRHelper.xrGetInstanceProcAddrDelegate>(xrGetInstanceProcAddr_prev);
			XrResult result = xrGetProc(instance, name, out function);
			if (name == "xrEndFrame")
			{
				XrEndFrame_prev = function;
				m_intercept_xrEndFrame = intercepted_xrEndFrame;
				function = Marshal.GetFunctionPointerForDelegate(m_intercept_xrEndFrame); ;
				UnityEngine.Debug.Log("Getting xrEndFrame func");
			}
			if (name == "xrWaitFrame")
			{
				XrWaitFrame_prev = function;
				m_intercept_xrWaitFrame = intercepted_xrWaitFrame;
				function = Marshal.GetFunctionPointerForDelegate(m_intercept_xrWaitFrame); ;
				UnityEngine.Debug.Log("Getting xrWaitFrame func");
			}
			return result;
		}

		[MonoPInvokeCallback(typeof(OpenXRHelper.xrEndFrameDelegate))]
		private static XrResult intercepted_xrEndFrame(XrSession session, ref XrFrameEndInfo frameEndInfo)
		{
			XrResult res;
			// Get delegate of prev xrEndFrame.
			var xrEndFrame = Marshal.GetDelegateForFunctionPointer<OpenXRHelper.xrEndFrameDelegate>(XrEndFrame_prev);

			layerListOrigin.Clear();
			uint layerCount = frameEndInfo.layerCount;
			IntPtr layers = frameEndInfo.layers;
			for (int i = 0; i < layerCount; i++)
			{
				IntPtr ptr = Marshal.ReadIntPtr(layers, i * Marshal.SizeOf(typeof(IntPtr)));
				XrCompositionLayerBaseHeader header = (XrCompositionLayerBaseHeader)Marshal.PtrToStructure(ptr, typeof(XrCompositionLayerBaseHeader));
				layerListOrigin.Add(ptr);
			}
			List<IntPtr> layerListNew;
			if (layerListModified.Count != 0)
			{
				layerListNew = new List<IntPtr>(layerListModified);
			}
			else
			{
				layerListNew = new List<IntPtr>(layerListOrigin);
			}
			for (int i = 0; i < layerListNew.Count; i++)
			{
				Marshal.WriteIntPtr(layersModified, i * Marshal.SizeOf(typeof(IntPtr)), layerListNew[i]);
			}
			frameEndInfo.layers = layersModified;
			frameEndInfo.layerCount = (uint)layerListNew.Count;

			res = xrEndFrame(session, ref frameEndInfo);
			return res;
		}
		private static XrFrameWaitInfo m_frameWaitInfo;
		private static XrFrameState m_frameState;
		[MonoPInvokeCallback(typeof(OpenXRHelper.xrWaitFrameDelegate))]
		private static int intercepted_xrWaitFrame(ulong session, ref XrFrameWaitInfo frameWaitInfo, ref XrFrameState frameState)
		{
			var xrWaitFrame = Marshal.GetDelegateForFunctionPointer<OpenXRHelper.xrWaitFrameDelegate>(XrWaitFrame_prev);
			int res = xrWaitFrame(session, ref frameWaitInfo, ref frameState);
			m_frameWaitInfo = frameWaitInfo;
			m_frameState = frameState;
			return res;
		}
		public void GetOriginEndFrameLayerList(out List<IntPtr> layers)
		{
			layers = new List<IntPtr>(layerListOrigin);
		}

		public void SubmitLayers(List<IntPtr> layers)
		{
			layerListModified = new List<IntPtr>(layers);
			//UnityEngine.Debug.Log("####Update submit end " + layerListModified.Count);
		}
		public XrFrameState GetFrameState()
		{
			return m_frameState;
		}
#endif

		private XrInstance m_XrInstance = 0;
		protected override bool OnInstanceCreate(ulong xrInstance)
		{
			foreach (string kOpenxrExtensionString in kOpenxrExtensionStrings.Split(' '))
			{
				if (!OpenXRRuntime.IsExtensionEnabled(kOpenxrExtensionString))
				{
					WARNING("OnInstanceCreate() " + kOpenxrExtensionString + " is NOT enabled.");

					m_HTCPassthroughExtensionEnabled = false;
					return false;
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
		private XrSpace WorldLockSpaceOriginOnHead
		{
			get { return m_WorldLockSpaceOriginOnHead; }
		}
		private XrSpace WorldLockSpaceOriginOnFloor
		{
			get { return m_WorldLockSpaceOriginOnFloor; }
		}
		private XrSpace HeadLockSpace
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

		/// <summary>
		/// The delegate of Passthrough Session Destroy.
		/// </summary>
		public delegate void OnPassthroughSessionDestroyDelegate(int passthroughID);
		private Dictionary<int, OnPassthroughSessionDestroyDelegate> OnPassthroughSessionDestroyHandlerDictionary = new Dictionary<int, OnPassthroughSessionDestroyDelegate>();
		protected override void OnSessionDestroy(ulong xrSession)
		{
			m_XrSessionCreated = false;
			DEBUG("OnSessionDestroy() " + xrSession);
#if UNITY_ANDROID
			//Notify that all passthrough layers should be destroyed
			List<int> currentPassthroughIDs = PassthroughIDList;
			foreach (int passthroughID in currentPassthroughIDs)
			{
				OnPassthroughSessionDestroyDelegate OnPassthroughSessionDestroyHandler = OnPassthroughSessionDestroyHandlerDictionary[passthroughID];

				OnPassthroughSessionDestroyHandler?.Invoke(passthroughID);
			}
#endif
#if UNITY_STANDALONE
			//Notify that all passthrough layers should be destroyed
			List<XrPassthroughHTC> currentPassthroughs = PassthroughList;
			foreach (XrPassthroughHTC passthrough in currentPassthroughs)
			{
				DestroyPassthroughHTC(passthrough);
			}
#endif
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
		/// The current XR session state.
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
#if UNITY_STANDALONE
		OpenXRHelper.xrGetInstanceProcAddrDelegate Intercept_xrGetInstanceProcAddr =
			new OpenXRHelper.xrGetInstanceProcAddrDelegate(InterceptXrEndFrame_xrGetInstanceProcAddr);
#endif
		private static OpenXRHelper.xrEndFrameDelegate m_intercept_xrEndFrame;
		private static OpenXRHelper.xrWaitFrameDelegate m_intercept_xrWaitFrame;
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
		private XrResult DestroySpace(XrSpace space)
		{
			if (space != 0)
			{
				return xrDestroySpace(space);
			}
			return XrResult.XR_ERROR_REFERENCE_SPACE_UNSUPPORTED;
		}
#if UNITY_STANDALONE
		private List<XrPassthroughHTC> passthroughList = new List<XrPassthroughHTC>();
		public List<XrPassthroughHTC> PassthroughList { get { return new List<XrPassthroughHTC>(passthroughList); } }
		ViveCompositionLayerHelper.xrCreatePassthroughHTCDelegate xrCreatePassthroughHTC;
		public XrResult CreatePassthroughHTC(XrPassthroughCreateInfoHTC createInfo, out XrPassthroughHTC passthrough)
		{
			if (!m_XrSessionCreated)
			{
				ERROR("CreatePassthroughHTC() XR_ERROR_SESSION_LOST.");
				passthrough = 0;
				return XrResult.XR_ERROR_SESSION_LOST;
			}
			if (!m_XrInstanceCreated)
			{
				ERROR("CreatePassthroughHTC() XR_ERROR_INSTANCE_LOST.");
				passthrough = 0;
				return XrResult.XR_ERROR_INSTANCE_LOST;
			}

			XrResult res = xrCreatePassthroughHTC(m_XrSession, createInfo, out passthrough);
			if (res == XrResult.XR_SUCCESS)
				passthroughList.Add(passthrough);
			else
				ERROR("CreatePassthroughHTC() "+res);
			return res;
		}

		ViveCompositionLayerHelper.xrDestroyPassthroughHTCDelegate xrDestroyPassthroughHTC;
		public XrResult DestroyPassthroughHTC(XrPassthroughHTC passthrough)
		{
			DEBUG("Entry");
			XrResult res = xrDestroyPassthroughHTC(passthrough);
			if (res == XrResult.XR_SUCCESS)
			{
				passthroughList.Remove(passthrough);
			}
			return res;
		}
		/// <summary>
		/// According to XRInputSubsystem's tracking origin mode, return the corresponding XrSpace.
		/// </summary>
		/// <returns></returns>
		public XrSpace GetTrackingSpace()
		{
			var s = GetCurrentAppSpace();
			Debug.Log("VivePassthrough GetTrackingSpace() s=" + s);
			return s;
		}
#endif
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
#if UNITY_STANDALONE
			/// xrCreatePassthroughHTC
			if (XrGetInstanceProcAddr(xrInstance, "xrCreatePassthroughHTC", out funcPtr) == XrResult.XR_SUCCESS)
			{
				if (funcPtr != IntPtr.Zero)
				{
					DEBUG("Get function pointer of xrCreatePassthroughHTC.");
					xrCreatePassthroughHTC = Marshal.GetDelegateForFunctionPointer(
						funcPtr,
						typeof(ViveCompositionLayerHelper.xrCreatePassthroughHTCDelegate)) as ViveCompositionLayerHelper.xrCreatePassthroughHTCDelegate;
				}
			}
			else
			{
				ERROR("xrCreatePassthroughHTC");
				return false;
			}
			/// xrCreatePassthroughHTC
			if (XrGetInstanceProcAddr(xrInstance, "xrDestroyPassthroughHTC", out funcPtr) == XrResult.XR_SUCCESS)
			{
				if (funcPtr != IntPtr.Zero)
				{
					DEBUG("Get function pointer of xrDestroyPassthroughHTC.");
					xrDestroyPassthroughHTC = Marshal.GetDelegateForFunctionPointer(
						funcPtr,
						typeof(ViveCompositionLayerHelper.xrDestroyPassthroughHTCDelegate)) as ViveCompositionLayerHelper.xrDestroyPassthroughHTCDelegate;
				}
			}
			else
			{
				ERROR("xrDestroyPassthroughHTC");
				return false;
			}
#endif
#if UNITY_ANDROID
			if (HTCPassthrough_GetFuncAddrs(xrInstance, xrGetInstanceProcAddr) == XrResult.XR_SUCCESS)
			{
				DEBUG("Get function pointers in native.");
			}
			else
			{
				ERROR("HTCPassthrough_GetFuncAddrs");
				return false;
			}
#endif
			return true;
        }
#endregion

#region Wrapper Functions
		private const string ExtLib = "viveopenxr";
		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_CreatePassthrough")]
		private static extern int VIVEOpenXR_HTCPassthrough_CreatePassthrough(XrSession session, LayerType layerType, PassthroughLayerForm layerForm, uint compositionDepth = 0);
		/// <summary>
		/// Create Passthrough.
		/// </summary>
		public int HTCPassthrough_CreatePassthrough(LayerType layerType, PassthroughLayerForm layerForm, OnPassthroughSessionDestroyDelegate onDestroyPassthroughHandler, uint compositionDepth = 0)
		{
			if (!m_XrSessionCreated || m_XrSession == 0)
			{
				ERROR("Xr Session not found");
				return 0;
			}

			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_CreatePassthrough: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return 0;
			}

			int passthroughID = VIVEOpenXR_HTCPassthrough_CreatePassthrough(m_XrSession, layerType, layerForm, compositionDepth);

			if (passthroughID != 0)
			{
				passthroughIDList.Add(passthroughID);
				OnPassthroughSessionDestroyHandlerDictionary.Add(passthroughID, onDestroyPassthroughHandler);
			}

			return passthroughID;
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetAlpha")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetAlpha(int passthroughID, float alpha);
		/// <summary>
		/// Set Passthough Alpha.
		/// </summary>
		public bool HTCPassthrough_SetAlpha(int passthroughID, float alpha)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetAlpha: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetAlpha(passthroughID, alpha);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetLayerType")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetLayerType(int passthroughID, LayerType layerType, uint compositionDepth = 0);
		/// <summary>
		/// Set Passthough Layer Type.
		/// </summary>
		public bool HTCPassthrough_SetLayerType(int passthroughID, LayerType layerType, uint compositionDepth = 0)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetLayerType: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetLayerType(passthroughID, layerType, compositionDepth);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetMesh")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetMesh(int passthroughID, uint vertexCount, [In, Out] XrVector3f[] vertexBuffer, uint indexCount, [In, Out] uint[] indexBuffer);
		/// <summary>
		/// Set Passthough Mesh.
		/// </summary>
		public bool HTCPassthrough_SetMesh(int passthroughID, uint vertexCount, [In, Out] XrVector3f[] vertexBuffer, uint indexCount, [In, Out] uint[] indexBuffer)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetMesh: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetMesh(passthroughID, vertexCount, vertexBuffer, indexCount, indexBuffer);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetMeshTransform")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetMeshTransform(int passthroughID, XrSpace meshSpace, XrPosef meshPose, XrVector3f meshScale);
		/// <summary>
		/// Set Passthough Mesh Transform.
		/// </summary>
		public bool HTCPassthrough_SetMeshTransform(int passthroughID, XrSpace meshSpace, XrPosef meshPose, XrVector3f meshScale)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetMeshTransform: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetMeshTransform(passthroughID, meshSpace, meshPose, meshScale);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetMeshTransformSpace")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetMeshTransformSpace(int passthroughID, XrSpace meshSpace);
		/// <summary>
		/// Set Passthough Mesh Transform Space.
		/// </summary>
		public bool HTCPassthrough_SetMeshTransformSpace(int passthroughID, XrSpace meshSpace)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetMeshTransformSpace: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetMeshTransformSpace(passthroughID, meshSpace);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetMeshTransformPosition")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetMeshTransformPosition(int passthroughID, XrVector3f meshPosition);
		/// <summary>
		/// Set Passthough Mesh Transform Position.
		/// </summary>
		public bool HTCPassthrough_SetMeshTransformPosition(int passthroughID, XrVector3f meshPosition)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetMeshTransformPosition: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetMeshTransformPosition(passthroughID, meshPosition);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetMeshTransformOrientation")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetMeshTransformOrientation(int passthroughID, XrQuaternionf meshOrientation);
		/// <summary>
		/// Set Passthough Mesh Transform orientation.
		/// </summary>
		public bool HTCPassthrough_SetMeshTransformOrientation(int passthroughID, XrQuaternionf meshOrientation)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetMeshTransformOrientation: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetMeshTransformOrientation(passthroughID, meshOrientation);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_SetMeshTransformScale")]
		private static extern bool VIVEOpenXR_HTCPassthrough_SetMeshTransformScale(int passthroughID, XrVector3f meshScale);
		/// <summary>
		/// Set Passthough Mesh Transform scale.
		/// </summary>
		public bool HTCPassthrough_SetMeshTransformScale(int passthroughID, XrVector3f meshScale)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_SetMeshTransformScale: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			return VIVEOpenXR_HTCPassthrough_SetMeshTransformScale(passthroughID, meshScale);
		}

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_DestroyPassthrough")]
		private static extern bool VIVEOpenXR_HTCPassthrough_DestroyPassthrough(int passthroughID);
		/// <summary>
		/// Destroy Passthough.
		/// </summary>
		public bool HTCPassthrough_DestroyPassthrough(int passthroughID)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("HTCPassthrough_DestroyPassthrough: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return false;
			}

			bool destroyed = VIVEOpenXR_HTCPassthrough_DestroyPassthrough(passthroughID);

			if (destroyed)
			{
				passthroughIDList.Remove(passthroughID);
				OnPassthroughSessionDestroyHandlerDictionary.Remove(passthroughID);
			}

			return destroyed;
		}
#endregion

#region Hook native functions

		[DllImportAttribute(ExtLib, EntryPoint = "htcpassthrough_GetFuncAddrs")]
		private static extern XrResult VIVEOpenXR_HTCPassthrough_GetFuncAddrs(XrInstance xrInstance, IntPtr xrGetInstanceProcAddrFuncPtr);
		private XrResult HTCPassthrough_GetFuncAddrs(XrInstance xrInstance, IntPtr xrGetInstanceProcAddrFuncPtr)
		{
			if (!HTCPassthroughExtensionEnabled)
			{
				ERROR("VIVEOpenXR_HTCPassthrough_GetFuncAddrs: " + kOpenxrExtensionStrings + " is NOT enabled.");
				return XrResult.XR_ERROR_FEATURE_UNSUPPORTED;
			}

			return VIVEOpenXR_HTCPassthrough_GetFuncAddrs(xrInstance, xrGetInstanceProcAddrFuncPtr);
		}

#endregion

#region Helper Funcs
		/// <summary>
		/// Helper function to get XrSpace from space type.
		/// </summary>
		public XrSpace GetXrSpaceFromSpaceType(ProjectedPassthroughSpaceType spaceType)
		{
			XrSpace meshSpace = 0;
			switch (spaceType)
			{
				case ProjectedPassthroughSpaceType.Headlock:
					meshSpace = HeadLockSpace;
					break;
				case ProjectedPassthroughSpaceType.Worldlock:
				default:
					XRInputSubsystem subsystem = null;
					SubsystemManager.GetInstances(inputSubsystems);
					if (inputSubsystems.Count > 0)
					{
						subsystem = inputSubsystems[0];
					}

					if (subsystem != null)
					{
						TrackingOriginModeFlags trackingOriginMode = subsystem.GetTrackingOriginMode();

						switch (trackingOriginMode)
						{
							default:
							case TrackingOriginModeFlags.Floor:
								meshSpace = WorldLockSpaceOriginOnFloor;
								break;
							case TrackingOriginModeFlags.Device:
								meshSpace = WorldLockSpaceOriginOnHead;
								break;
						}
					}
					else
					{
						meshSpace = WorldLockSpaceOriginOnFloor;
					}
					break;
			}

			return meshSpace;
		}

#endregion
	}
}
