using UnityEditor;
using UnityEditor.XR.OpenXR.Features;

namespace HTC.Vive.OpenXR.Editor
{
    [OpenXRFeatureSet(
        FeatureSetId = featureSetId,
        FeatureIds = new string[]
        {
           "com.htc.openxr.sceneunderstanding.feature",
           "com.htc.openxr.facialtracking.feature",
           "com.htc.openxr.feature.input.htcvivecosmos",
           "com.company.openxr.handtracking.feature",
           "com.htc.openxr.feature.input.htcvivefocus3",
           "com.htc.openxr.feature.input.htcvivehandinteraction"
        },
        DefaultFeatureIds = new string[]
        {
           "com.htc.openxr.sceneunderstanding.feature",
           "com.htc.openxr.facialtracking.feature",
           "com.htc.openxr.feature.input.htcvivecosmos",
           "com.company.openxr.handtracking.feature",
           "com.htc.openxr.feature.input.htcvivefocus3",
           "com.htc.openxr.feature.input.htcvivehandinteraction"
        },
        UiName = "VIVE OpenXR",
        Description = "Enable the full suite of features for Vive OpenXR.",
        SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Standalone }
    )]
    sealed class VIVEFeatureSet
    {
        internal const string featureSetId = "com.HTC.openxr.featureset.vive";
    }
}
