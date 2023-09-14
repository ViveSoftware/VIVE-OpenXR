// "VIVE SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the VIVE SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace VIVE.OpenXR.CompositionLayer.Passthrough
{
	//[StructLayout(LayoutKind.Sequential)]
    public enum XrStructureTypeHTC
    {
		XR_TYPE_PASSTHROUGH_CREATE_INFO_HTC = 1000317001,
		XR_TYPE_PASSTHROUGH_COLOR_HTC = 1000317002,
		XR_TYPE_PASSTHROUGH_MESH_TRANSFORM_INFO_HTC = 1000317003,
		XR_TYPE_COMPOSITION_LAYER_PASSTHROUGH_HTC = 1000317004,
	}

	public enum PassthroughLayerForm
	{
		///<summary> Fullscreen Passthrough Form</summary>
		Planar = 0,
		///<summary> Projected Passthrough Form</summary>
		Projected = 1
	}

	public enum ProjectedPassthroughSpaceType
	{
		///<summary> 
		/// XR_REFERENCE_SPACE_TYPE_VIEW at (0,0,0) with orientation (0,0,0,1) 
		///</summary>
		Headlock = 0,
		///<summary> 
		/// When TrackingOriginMode is TrackingOriginModeFlags.Floor:
		/// XR_REFERENCE_SPACE_TYPE_STAGE at (0,0,0) with orientation (0,0,0,1) 
		/// 
		/// When TrackingOriginMode is TrackingOriginModeFlags.Device:
		/// XR_REFERENCE_SPACE_TYPE_LOCAL at (0,0,0) with orientation (0,0,0,1) 
		/// 
		///</summary>
		Worldlock = 1
	}
}
