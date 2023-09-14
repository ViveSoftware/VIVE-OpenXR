// Copyright HTC Corporation All Rights Reserved.
#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.XR.OpenXR.Features;

namespace VIVE.OpenXR
{
	[OpenXRFeatureSet(
	   FeatureIds = new string[] {
			VIVEFocus3Feature.featureId,
			VIVEFocus3Profile.featureId,
			Hand.ViveHandTracking.featureId,
			"vive.vive.openxr.feature.compositionlayer",
			"vive.vive.openxr.feature.compositionlayer.cylinder",
			"vive.vive.openxr.feature.compositionlayer.colorscalebias",
			Tracker.ViveWristTracker.featureId,
			Hand.ViveHandInteraction.featureId,
			"vive.vive.openxr.feature.foveation",
			FacialTracking.ViveFacialTracking.featureId,
		   },
	   UiName = "VIVE XR Support",
	   Description = "Necessary to deploy an VIVE XR compatible app.",
	   FeatureSetId = "com.htc.vive.openxr.featureset.vivexr",
	   DefaultFeatureIds = new string[] { VIVEFocus3Feature.featureId, VIVEFocus3Profile.featureId, },
	   SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Android }
   )]
	sealed class VIVEFocus3FeatureSet { }
}
#endif
