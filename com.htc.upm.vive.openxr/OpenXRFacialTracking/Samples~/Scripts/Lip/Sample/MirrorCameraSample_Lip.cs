//========= Copyright 2019, HTC Corporation. All rights reserved. ===========
using UnityEngine;

namespace VIVE
{
    namespace FacialTracking.Sample
    {
        /// <summary>
        /// A very basic mirror.
        /// </summary>
        [RequireComponent(typeof(Camera))]
        public class MirrorCameraSample_Lip : MonoBehaviour
        {
            private const float Distance = 0.6f;

            private void Update()
            {                                
                if (Lip_Framework.Status != Lip_Framework.FrameworkStatus.WORKING) return;
                SetMirroTransform();
            }

            private void SetMirroTransform()
            {
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * Distance;
                transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
                transform.LookAt(Camera.main.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
        
    }
}