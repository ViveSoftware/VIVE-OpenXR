//========= Copyright 2019, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;
namespace VIVE
{
    namespace FacialTracking.Sample
    {
            public static class Eye
            {
                public const int WeightingCount = (int)XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC;
                private static XrFacialExpressionsHTC EyeExpression_;
                private static int LastUpdateFrame = -1;
                private static Error LastUpdateResult = Error.FAILED;
                private static Dictionary<XrEyeShapeHTC, float> Weightings;
                private static float[] blendshapes = new float[60];
                static Eye()
                {
                    Weightings = new Dictionary<XrEyeShapeHTC, float>();
                    for (int i = 0; i < WeightingCount; ++i) Weightings.Add((XrEyeShapeHTC)i, 0.0f);
                }
                private static bool UpdateData()
                {
                    if (Time.frameCount == LastUpdateFrame) return LastUpdateResult == Error.WORK;
                    else LastUpdateFrame = Time.frameCount;
                    EyeExpression_.expressionCount = 60;
                    EyeExpression_.type = XrStructureType.XR_TYPE_FACIAL_EXPRESSIONS_HTC;
                    EyeExpression_.blendShapeWeightings = Marshal.AllocCoTaskMem(sizeof(float) * EyeExpression_.expressionCount);
                    var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
                    int res = feature.xrGetFacialExpressionsHTC(OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>().m_expressionHandle, ref EyeExpression_);
                    if (res == (int)XrResult.XR_SUCCESS)
                    {
                        Marshal.Copy(EyeExpression_.blendShapeWeightings, blendshapes, 0, EyeExpression_.expressionCount);
                        LastUpdateResult = Error.WORK;

                    }
                    else
                    {
                        LastUpdateResult = Error.FAILED;
                    }
                    return LastUpdateResult == Error.WORK;
                }
                public static bool GetEyeWeightings(out Dictionary<XrEyeShapeHTC, float> shapes, XrFacialExpressionsHTC expression)
                {
                    for (int i = 0; i < WeightingCount; ++i)
                    {
                        Weightings[(XrEyeShapeHTC)(i)] = blendshapes[i];
                    }
                    shapes = Weightings;
                    return true;
                }


                /// <summary>
                /// Gets weighting values from Eye module.
                /// </summary>
                /// <param name="shapes">Weighting values obtained from Eye module.</param>
                /// <returns>Indicates whether the values received are new.</returns>\
                [Obsolete("Create FacialManager object and call member function GetWeightings instead")]
                public static bool GetEyeWeightings(out Dictionary<XrEyeShapeHTC, float> shapes)
                {
                    UpdateData();
                    return GetEyeWeightings(out shapes, EyeExpression_);
                }

            }
        
    }
}