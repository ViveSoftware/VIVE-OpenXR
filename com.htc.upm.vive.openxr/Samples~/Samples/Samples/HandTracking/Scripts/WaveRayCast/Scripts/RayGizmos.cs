#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VIVE.OpenXR.Samples.Ray
{
    public class RayGizmos : MonoBehaviour
    {
        public WaveRay WR;
        public float CastRadius;
        public float CastLength;
        void OnDrawGizmos()
        {
            CastRadius = WR.CastRange;
            CastLength = WR.RayLength;

            Gizmos.color = Color.cyan;
            //Gizmos.DrawWireSphere(transform.position, RayRadius);
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, CastRadius);
            Start_Circles = Get_CirclePoints(transform.position, transform.forward).ToArray();
            End_Circles = Get_CirclePoints(transform.position + transform.forward * CastLength, transform.forward).ToArray();
            for (int i = 0; i < Start_Circles.Length; i++)
            {
                UnityEditor.Handles.DrawLine(Start_Circles[i], End_Circles[i]);
            }
            UnityEditor.Handles.DrawWireDisc(transform.position + transform.forward * CastLength, transform.forward, CastRadius);


            //Gizmos.DrawLine(roofTransform.position + new Vector3(roofCheckX, 0), roofTransform.position + new Vector3(roofCheckX, roofCheckY));
        }

        public Vector3[] Start_Circles;
        public Vector3[] End_Circles;
        public List<Vector3> Get_CirclePoints(Vector3 _Point, Vector3 _Dir)
        {
            List<Vector3> _Points = new List<Vector3>();

            for (int i = 0; i <= 360; i += 36)
            {
                float _Angle = i * Mathf.PI / 180;
                _Points.Add(new Vector3(Mathf.Cos(_Angle), Mathf.Sin(_Angle), 0) * CastRadius);
            }
            for (int i = 0; i < _Points.Count; i++)
            {
                _Points[i] = Quaternion.FromToRotation(Vector3.forward, _Dir) * _Points[i];
                _Points[i] += _Point;
            }
            return _Points;
        }


    }
}
#endif
