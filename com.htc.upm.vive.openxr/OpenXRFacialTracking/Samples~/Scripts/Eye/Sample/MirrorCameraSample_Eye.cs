//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.Runtime.InteropServices;
using UnityEngine;

namespace VIVE
{
    namespace FacialTracking.Sample
    {
        /// <summary>
        /// A very basic mirror.
        /// </summary>
        [RequireComponent(typeof(Camera))]
        public class MirrorCameraSample_Eye : MonoBehaviour
        {
            private const float Distance = 0.6f;
            private void Update()
            {
                if (Eye_Framework.Status != Eye_Framework.FrameworkStatus.WORKING) return;

            }

            private void Release()
            {
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