using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIVE.HandTracking.Sample
{
    public class SwitchHand : MonoBehaviour
    {
        public InputMaster inputMaster;
        public GameObject hand3D;
        public GameObject handSkeleton;
        private bool isSwitch = false;
        private void Awake()
        {
            inputMaster = new InputMaster();
            inputMaster.Enable();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float ispressed = inputMaster.Keyboard.SpacePress.ReadValue<float>();
            if (ispressed > 0.5)
            {
                if(!isSwitch)
                {
                    UnityEngine.Debug.Log("press space");
                    hand3D.SetActive(!hand3D.activeSelf);
                    handSkeleton.SetActive(!handSkeleton.activeSelf);
                    isSwitch = true;
                }
            }
            else
            {
                isSwitch = false;
            }

        }
    }

}
