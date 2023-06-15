using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using System.Linq;
using System.IO;
using VIVE.FacialTracking;
namespace UnityEditor.XR.OpenXR.Samples.FacialTracking
{
    [InitializeOnLoad]
    public class FacialTrackingFeatureInstaller : Editor
    {
#if !UNITY_SAMPLE_DEV
        private const string k_ScriptPath = "FacialTracking Example/Editor/FacialTrackingFeatureInstaller.cs";
        static FacialTrackingFeatureInstaller()
        {
            FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
            var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
            if (feature != null)
            {
                if (feature.enabled != true)
                {
                    feature.enabled = true;
                }
            }
            Debug.Log(AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(k_ScriptPath)).Select(AssetDatabase.GUIDToAssetPath));
            var source = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(k_ScriptPath))
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(r => r.Contains(k_ScriptPath));

            if (string.IsNullOrEmpty(source))
            {
                Debug.LogError("File Not Exist");
                return;
            }

            source = Path.GetDirectoryName(source);
            Debug.Log(source);
            AssetDatabase.DeleteAsset(Path.Combine(Path.GetDirectoryName(source), "Editor"));
        }
#endif
    }
}