using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VIVE.OpenXR.Samples.Ray
{
    public class RayCasting : MonoBehaviour
    {
        public WaveRay WR;
        public float RayRadius;
        public float RayLength;
        public int Start_Angle;
        void Start()
        {

        }

        void Update()
        {
            Start_Angle = WR.Start_Angle;
        }

        public RayCastingHit Get_HittingGObj()
        {
            RayRadius = WR.CastRange;
            RayLength = WR.RayLength;
            RaycastHit _GObj;
            float _MinDis;
            RaycastHit[] HittingObjs = Physics.SphereCastAll(transform.position, RayRadius, transform.forward, RayLength);

            if (HittingObjs.Length <= 0)
            {
                return null;
            }

            _GObj = HittingObjs[0];
            _MinDis = Get_Dis_between_Points_on_ClipPlane(_GObj.transform.position, transform.position);

            for (int i = 1; i < HittingObjs.Length; i++)
            {
                RaycastHit __TempGObj = HittingObjs[i];
                float __TempDis = Get_Dis_between_Points_on_ClipPlane(__TempGObj.transform.position, transform.position);
                if (_MinDis > __TempDis)
                {
                    _GObj = __TempGObj;
                    _MinDis = __TempDis;
                }
            }
            return new RayCastingHit(_GObj.collider.gameObject, _GObj.point);
        }

        float Get_Dis_between_Points_on_ClipPlane(Vector3 _PointA, Vector3 _PointB)
        {
            return Vector3.Distance(Camera.main.WorldToScreenPoint(_PointA), Camera.main.WorldToScreenPoint(_PointA));
        }

        public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {

            float distance;
            Vector3 translationVector;

            //First calculate the distance from the point to the plane:
            distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

            //Reverse the sign of the distance
            distance *= -1;

            //Get a translation vector(
            translationVector = SetVectorLength(planeNormal, distance);

            //Translate the point to form a projection
            return point + translationVector;
        }



        //Get the shortest distance between a point and a plane. The output is signed so it holds information
        //as to which side of the plane normal the point is.
        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {

            return Vector3.Dot(planeNormal, (point - planePoint));
        }



        //create a vector of direction "vector" with length "size"
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {

            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }

        public class RayCastingHit
        {
            public GameObject GObj { get; }
            public Vector3 HitPoint { get; }
            public RayCastingHit(GameObject _GObj, Vector3 _HitPoint)
            {
                GObj = _GObj;
                HitPoint = _HitPoint;
            }
        }
    }
}
