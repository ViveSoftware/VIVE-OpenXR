using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VIVE.OpenXR.Samples.Hand
{
    public class HitObject : MonoBehaviour
    {
        public Material SelectM, UnSelectM;
        MeshRenderer MR;
        void Start()
        {
            MR = GetComponent<MeshRenderer>();
        }

        void Update()
        {

        }

        public void Select()
        {
            MR.material = SelectM;
        }

        public void UnSelect()
        {
            MR.material = UnSelectM;
        }
    }
}
