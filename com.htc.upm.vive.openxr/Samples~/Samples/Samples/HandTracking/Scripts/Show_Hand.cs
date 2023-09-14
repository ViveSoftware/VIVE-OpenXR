using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIVE.OpenXR.Samples.Hand
{
    public class Show_Hand : MonoBehaviour
    {
        /// <summary>
        /// This is a comment
        /// </summary>
        public GameObject jointPrefab;
        public Transform leftHand;
        public Transform rightHand;
        void Start()
        {
            GameObject temp;
            for (int i = 0; i < 26; i++)
            {
                temp = Instantiate(jointPrefab, leftHand);
                temp.GetComponent<Joint_Movement>().isLeft = true;
                temp.GetComponent<Joint_Movement>().jointNum = i;
            }

            for (int i = 0; i < 26; i++)
            {
                temp = Instantiate(jointPrefab, rightHand);
                temp.GetComponent<Joint_Movement>().isLeft = false;
                temp.GetComponent<Joint_Movement>().jointNum = i;
            }
        }
    }
}
