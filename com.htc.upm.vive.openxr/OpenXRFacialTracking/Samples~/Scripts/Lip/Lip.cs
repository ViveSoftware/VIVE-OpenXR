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
        public class Lip
        {
            public const int WeightingCount = (int)XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC;
            private static int LastUpdateFrame = -1;
            private static Error LastUpdateResult = Error.FAILED;
            private static Dictionary<XrLipShapeHTC, float> Weightings;
            private static float[] blendshapes = new float[60];
            private static XrFacialExpressionsHTC LipExpression;

            static Lip()
            {
                Weightings = new Dictionary<XrLipShapeHTC, float>();
                for (int i = 0; i < WeightingCount; ++i) Weightings.Add((XrLipShapeHTC)i, 0.0f);
            }

            private static bool UpdateData()
            {
                if (Time.frameCount == LastUpdateFrame) return LastUpdateResult == Error.WORK;
                else LastUpdateFrame = Time.frameCount;
                LipExpression.expressionCount = 60; 
                LipExpression.type = XrStructureType.XR_TYPE_FACIAL_EXPRESSIONS_HTC;
                LipExpression.blendShapeWeightings = Marshal.AllocCoTaskMem(sizeof(float)* LipExpression.expressionCount);
                var feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
                int res = feature.xrGetFacialExpressionsHTC(OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>().m_expressionHandle_Lip, ref LipExpression);
                    

                if(res == (int)XrResult.XR_SUCCESS)
                {
                    LastUpdateResult = Error.WORK;
                    Marshal.Copy(LipExpression.blendShapeWeightings, blendshapes,0, LipExpression.expressionCount);
                    for (int i = 0; i < WeightingCount; ++i)
                    {
                        Weightings[(XrLipShapeHTC)(i)] = blendshapes[i];
                    }

                }
                else
                {
                    LastUpdateResult = Error.FAILED;
                }
                return LastUpdateResult == Error.WORK;
            }

            /// <summary>
            /// Gets weighting values from Lip module.
            /// </summary>
            /// <param name="shapes">Weighting values obtained from Lip module.</param>
            /// <returns>Indicates whether the values received are new.</returns>
            [Obsolete("Create FacialManager object and call member function GetWeightings instead")]
            public static bool GetLipWeightings(out Dictionary<XrLipShapeHTC, float> shapes)
            {
                bool update = UpdateData();
                shapes = Weightings;
                return update;
            }

        }
        
    }
}

