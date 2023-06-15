//========= Copyright 2019, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VIVE
{
    namespace FacialTracking.Sample
    {
        [Serializable]
        public class LipShapeTable
        {
            public SkinnedMeshRenderer skinnedMeshRenderer;
            public XrLipShapeHTC[] lipShapes;
        }
        public class AvatarLipSample : MonoBehaviour
        {
            [SerializeField] private List<LipShapeTable> LipShapeTables;

            public bool NeededToGetData = true;
            private Dictionary<XrLipShapeHTC, float> LipWeightings;
            private FacialManager facialManager = new FacialManager();
            private void Start()
            {
                facialManager.StartFramework(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC);
                SetLipShapeTables(LipShapeTables);
            }

            private void Update()
            {
                if (NeededToGetData)
                {
                    facialManager.GetWeightings(out LipWeightings);
                    //Lip.GetLipWeightings(out LipWeightings);
                    UpdateLipShapes(LipWeightings);
                }
            }

            public void SetLipShapeTables(List<LipShapeTable> lipShapeTables)
            {
                bool valid = true;
                if (lipShapeTables == null)
                {
                    valid = false;
                }
                else
                {
                    for (int table = 0; table < lipShapeTables.Count; ++table)
                    {
                        if (lipShapeTables[table].skinnedMeshRenderer == null)
                        {
                            valid = false;
                            break;
                        }
                        for (int shape = 0; shape < lipShapeTables[table].lipShapes.Length; ++shape)
                        {
                            XrLipShapeHTC lipShape = lipShapeTables[table].lipShapes[shape];
                            if (lipShape > XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC || lipShape < 0)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                }
                if (valid)
                    LipShapeTables = lipShapeTables;
            }

            public void UpdateLipShapes(Dictionary<XrLipShapeHTC, float> lipWeightings)
            {
                foreach (var table in LipShapeTables)
                    RenderModelLipShape(table, lipWeightings);
            }

            private void RenderModelLipShape(LipShapeTable lipShapeTable, Dictionary<XrLipShapeHTC, float> weighting)
            {
                for (int i = 0; i < lipShapeTable.lipShapes.Length; i++)
                {
                    int targetIndex = (int)lipShapeTable.lipShapes[i];
                    if (targetIndex > (int)XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC || targetIndex < 0) continue;
                    lipShapeTable.skinnedMeshRenderer.SetBlendShapeWeight(i, weighting[(XrLipShapeHTC)targetIndex] * 100);
                }
            }

            private void OnDestroy()
            {
                facialManager.StopFramework(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC);
            }
        }
        
    }
}