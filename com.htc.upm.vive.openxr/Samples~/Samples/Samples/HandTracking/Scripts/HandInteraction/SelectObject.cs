using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VIVE.OpenXR.Samples.Ray;
namespace VIVE.OpenXR.Samples.Hand
{
    public class SelectObject : MonoBehaviour
    {
        [SerializeField] InputActionAsset ActionAsset;
        [SerializeField] InputActionReference LeftPinchStrengthR;
        [SerializeField] InputActionReference RightPinchStrengthR;


        WaveRay WR;
        [SerializeField] WaveRay LWR;
        [SerializeField] WaveRay RWR;

        GameObject SelectingGameObject;
        float LeftCurrentStrength = 0;
        float RightCurrentStrength = 0;
        bool Using_Left;
        bool Using_Right;
        void OnEnable()
        {
            if (ActionAsset != null)
            {
                ActionAsset.Enable();
            }
        }

        void Update()
        {
            if (!Using_Right)
            {
                WR = LWR;
                float _Strength = LeftPinchStrengthR.action.ReadValue<float>();
                if (_Strength > 0.5f && LeftCurrentStrength <= 0.5f)
                {
                    SelectHittingObject();
                    Using_Left = true;
                }
                else if (_Strength <= 0.5f && LeftCurrentStrength > 0.5f)
                {
                    UnSelectHittingObject();
                    Using_Left = false;
                }
                UpdateHittingObject();
                LeftCurrentStrength = _Strength;
            }
            if (!Using_Left)
            {
                WR = RWR;
                float _Strength = RightPinchStrengthR.action.ReadValue<float>();
                if (_Strength > 0.5f && RightCurrentStrength <= 0.5f)
                {
                    SelectHittingObject();
                    Using_Right = true;
                }
                else if (_Strength <= 0.5f && RightCurrentStrength > 0.5f)
                {
                    UnSelectHittingObject();
                    Using_Right = false;
                }
                UpdateHittingObject();
                RightCurrentStrength = _Strength;
            }
        }

        void SelectHittingObject()
        {
            GameObject _Temp = WR.Get_HittingGameObject();

            if (_Temp != null)
            {
                SelectingGameObject = _Temp;
                HitObject _HO = SelectingGameObject.GetComponent<HitObject>();
                if (_HO != null)
                {
                    _HO.Select();
                }
            }
        }

        void UnSelectHittingObject()
        {
            if (SelectingGameObject != null)
            {
                if (SelectingGameObject.GetComponent<HitObject>() != null)
                {
                    HitObject _HO = SelectingGameObject.GetComponent<HitObject>();
                    if (_HO != null)
                    {
                        _HO.UnSelect();
                    }
                }
                SelectingGameObject = null;
            }
        }

        void UpdateHittingObject()
        {
            if (WR.Get_HittingGameObject() == null)
            {
                if (SelectingGameObject != null)
                {
                    UnSelectHittingObject();
                }
            }
            else
            {
                if (SelectingGameObject != null)
                {
                    if (SelectingGameObject != WR.Get_HittingGameObject())
                    {
                        UnSelectHittingObject();
                    }
                }
            }
        }
    }
}
