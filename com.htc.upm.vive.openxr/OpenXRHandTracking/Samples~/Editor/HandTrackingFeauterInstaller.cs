using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using VIVE.HandTracking;
using UnityEngine.XR.OpenXR.Features;
namespace UnityEditor.XR.OpenXR.Samples.HandTracking
{
    [InitializeOnLoad]
    public class HandTrackingFeauterInstaller : Editor
    {
#if !UNITY_SAMPLE_DEV
        private const string k_ScriptPath = "HandTracking Example/Editor/HandTrackingFeauterInstaller.cs";
        static HandTrackingFeauterInstaller()
        {
            FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
            var feature = OpenXRSettings.Instance.GetFeature<HandTracking_OpenXR_API>();
            var HandInteractionfeature = OpenXRSettings.Instance.GetFeature<HtcViveHandInteractionInputFeature>();
            if (feature != null)
            {
                if (feature.enabled != true)
                {
                    feature.enabled = true;
                }
            }
            if (HandInteractionfeature != null)
            {
                if (HandInteractionfeature.enabled != true)
                {
                    HandInteractionfeature.enabled = true;
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