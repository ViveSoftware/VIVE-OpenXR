using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VIVE.OpenXR.Samples.Ray
{
    public class WaveRay : MonoBehaviour
    {
        public GameObject SelectingGameObject;

        //public GameObject Ball;
        public Transform RayPose;
        public GameObject RayMesh;
        public RayCasting.RayCastingHit HittingTarget;
        Vector3 StartPoint, EndPoint;
        Vector3 StartPoint_Local, EndPoint_Local;
        public float RayRadius = 0.025f;
        public float RayLength = 30f;
        public float CastRange = 30f;
        RayMesh RM;
        RayCasting RC;
        private void Start()
        {
            RM = transform.GetComponentInChildren<RayMesh>();
            RC = transform.GetComponentInChildren<RayCasting>();
        }

        public GameObject Get_HittingGameObject()
        {
            if (HittingTarget == null)
            {
                return null;
            }
            return HittingTarget.GObj;
        }

        //---
        private void FixedUpdate()
        {
            Update_Ray();
        }


        List<GameObject> GObjPoints = new List<GameObject>();
        List<Vector3> Points = new List<Vector3>();

        public void Update_Ray()
        {
            StartPoint_Local = RayPose.localPosition;
            EndPoint_Local = RayPose.localPosition + RayPose.forward * RayLength;
            StartPoint = RayPose.position;
            EndPoint = RayPose.position + RayPose.forward * RayLength;
            RM.Radius = RayRadius;

            for (int i = 0; i < GObjPoints.Count; i++)
            {
                Destroy(GObjPoints[i]);
            }

            HittingTarget = RC.Get_HittingGObj();
            if (HittingTarget != null)
            {
                GObjPoints = new List<GameObject>();
                Points = Get_Points();

                if (Points.Count > 1)
                {
                    Update_RayPoints();

                    RM.AddMesh_TypeOne(RM.Get_CircleIndex(StartPoint_Local, RayPoints[0].Pos - StartPoint_Local).ToArray());
                    RM.AddMesh_TypeTwo(RM.Get_TubeIndex(StartPoint_Local, RayPoints[0].Pos, RayRadius, RayPoints[0].Radius, RayPoints[0].Pos - StartPoint_Local, RayPoints[0].Dir).ToArray());
                    int _Temp = 0;

                    for (int i = 0; i < RayPoints.Count - 1; i++)
                    {
                        _Temp++;
                        if (i == 0)
                        {
                            RM.AddMesh_TypeTwo(RM.Get_TubeIndex(RayPoints[0].Pos, RayPoints[1].Pos, RayPoints[0].Radius, RayPoints[1].Radius, RayPoints[0].Pos - StartPoint_Local, RayPoints[1].Dir).ToArray());
                        }
                        else
                        {
                            RM.AddMesh_TypeTwo(RM.Get_TubeIndex(RayPoints[i].Pos, RayPoints[i + 1].Pos, RayPoints[i].Radius, RayPoints[i + 1].Radius, RayPoints[i - 1].Dir, RayPoints[i + 1].Dir).ToArray());
                        }
                    }

                    RM.UpdateMesh();
                }
                else
                {
                    RM.AddMesh_TypeOne(RM.Get_CircleIndex(StartPoint_Local, transform.InverseTransformPoint(Point) - StartPoint_Local).ToArray());
                    RM.AddMesh_TypeOne(RM.Get_CornIndex(StartPoint_Local, transform.InverseTransformPoint(Point), RayRadius, transform.InverseTransformPoint(Point) - StartPoint_Local).ToArray());
                    RM.UpdateMesh();
                }
            }
            else
            {
                RM.AddMesh_TypeOne(RM.Get_CircleIndex(StartPoint_Local, RayPose.forward).ToArray());
                RM.AddMesh_TypeOne(RM.Get_CornIndex(StartPoint_Local, EndPoint_Local, RayRadius, RayPose.forward).ToArray());
                RM.UpdateMesh();
            }
        }




        Vector3 Point;
        Vector3 Dir_to_Point;
        Vector3 RayDir;
        Vector3 Normal;
        Vector3 Meeting_Point;

        int Scale = 1;
        public int Start_Angle = 0;
        int End_Angle = 70;
        List<Vector3> _Get_Points()
        {
            SetUp();
            float _Scaler;
            float _ButtonLength = Vector3.Distance(Point, Meeting_Point) / ((End_Angle - Start_Angle) / Scale);
            int _PointCount = 0;

            List<Vector3> _Points = new List<Vector3>();
            _Points.Add(Point);
            Vector3 _CurrentPoint = Point;
            bool Is_Limited = true;
            _PointCount = (End_Angle - Start_Angle) / Scale;
            float _LeftOverAngle = End_Angle - Start_Angle;
            float _Scale = Scale;
            float _CurrentAngle = Start_Angle;
            for (int i = 1; i <= _PointCount; i++)
            {
                if (Is_Limited)
                {
                    Is_Limited = false;
                }
                _CurrentAngle = (i == _PointCount ? End_Angle : _CurrentAngle + (i * _Scale / 10));
                _Scale = (End_Angle - _CurrentAngle) / ((_PointCount + 1) - i);
                float _AngleR = _CurrentAngle * Mathf.PI / 180;
                _Scaler = _ButtonLength / Mathf.Cos(_AngleR);
                _CurrentPoint = _CurrentPoint + (Quaternion.AngleAxis(_CurrentAngle, Normal) * -Dir_to_MeetingPoint) * _Scaler;
                _Points.Add(_CurrentPoint);
            }

            if (Is_Limited)
            {
                _CurrentPoint = Meeting_Point;
                _Points.Add(_CurrentPoint);
            }
            return _Points;
        }

        List<Vector3> Get_Points()
        {
            SetUp();
            List<Vector3> _Points = new List<Vector3>();
            bool _FirstTime = true;
            End_Angle = 90;
            while (_FirstTime || Vector3.Angle((_Points[_Points.Count - 1] - StartPoint), EndPoint - StartPoint) > 90)
            {
                End_Angle -= 1;
                _FirstTime = false;
                float _Scaler;
                float _ButtonLength = Vector3.Distance(Point, Meeting_Point) / ((End_Angle - Start_Angle) / Scale);
                int _PointCount = 0;


                _Points = new List<Vector3>();
                List<float> _PointTans = new List<float>();
                _Points.Add(Point);
                _PointTans.Add(0);
                Vector3 _CurrentPoint = Point;
                bool Is_Limited = true;
                _PointCount = (End_Angle - Start_Angle) / Scale;
                float _LeftOverAngle = End_Angle - Start_Angle;
                float _Scale = Scale;
                float _CurrentAngle = Start_Angle;

                for (int i = 1; i <= _PointCount; i++)
                {
                    if (Is_Limited)
                    {
                        Is_Limited = false;
                    }
                    _CurrentAngle = (i == _PointCount ? End_Angle : _CurrentAngle + (i * _Scale / 10));
                    _Scale = (End_Angle - _CurrentAngle) / ((_PointCount + 1) - i);
                    float _AngleR = _CurrentAngle * Mathf.PI / 180;
                    _Scaler = _ButtonLength / Mathf.Cos(_AngleR);
                    _PointTans.Add(_ButtonLength * Mathf.Tan(_AngleR));
                    _CurrentPoint = _CurrentPoint + (Quaternion.AngleAxis(_CurrentAngle, Normal) * -Dir_to_MeetingPoint) * _Scaler;
                    _Points.Add(_CurrentPoint);
                }
                if (End_Angle <= 46)
                {
                    _Points = new List<Vector3>();
                    _Points.Add(Point);
                    break;
                }
            }
            Debug.Log(End_Angle);
            return _Points;
        }

        void SetUp()
        {
            Point = HittingTarget.HitPoint;
            Start_Angle = (int)((CastRange - Get_Dis_between_Points_on_ClipPlane(Point, RayPose.position, RayPose.position)) / CastRange * 90);

            Dir_to_Point = Point - StartPoint;
            RayDir = EndPoint - StartPoint;
            Meeting_Point = Get_Meeting_Point();
            Debug.Log("Angle: " + End_Angle);
        }
        Vector3 Dir_to_MeetingPoint = Vector3.zero;
        Vector3 Get_Meeting_Point()
        {
            Normal = Vector3.Cross(Dir_to_Point, RayDir).normalized;
            Vector3 _Temp = Normal * Get_Dis_from_Point_to_Line();
            _Temp = Quaternion.AngleAxis(90, RayDir) * _Temp;
            Dir_to_MeetingPoint = _Temp.normalized;
            return Point - _Temp;
        }

        float Get_Dis_from_Point_to_Line()
        {
            float _Angle = Vector3.Angle(Dir_to_Point, RayDir);
            return Dir_to_Point.magnitude * Mathf.Sin(_Angle * Mathf.PI / 180);
        }

        float Get_Dis_between_Points_on_ClipPlane(Vector3 _PointA, Vector3 _PointB, Vector3 _PlanePoint)
        {
            return Vector3.Distance(ProjectPointOnPlane(RayPose.forward, _PlanePoint, _PointA), ProjectPointOnPlane(RayPose.forward, _PlanePoint, _PointB));
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

        List<RayPoint> RayPoints = new List<RayPoint>();
        void Update_RayPoints()
        {
            if (Points.Count <= 1)
            {
                RayPoints = new List<RayPoint>();
                return;
            }
            float _CurrentLength = 0;
            RayPoints = new List<RayPoint>();
            List<Vector3> _LocalPoints = new List<Vector3>();
            for (int i = 0; i < Points.Count; i++)
            {
                _LocalPoints.Add(transform.InverseTransformPoint(Points[i]));
            }

            for (int i = _LocalPoints.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    _CurrentLength += Vector3.Distance(_LocalPoints[i], _LocalPoints[i + 1]);
                    RayPoints.Add(new RayPoint(_LocalPoints[i], Vector3.zero, _CurrentLength));
                }
                else if (i == _LocalPoints.Count - 1)
                {
                    _CurrentLength += Vector3.Distance(_LocalPoints[i], StartPoint_Local);
                    RayPoints.Add(new RayPoint(_LocalPoints[i], _LocalPoints[i - 1] - _LocalPoints[i], _CurrentLength));
                }
                else
                {
                    _CurrentLength += Vector3.Distance(_LocalPoints[i], _LocalPoints[i + 1]);
                    RayPoints.Add(new RayPoint(_LocalPoints[i], _LocalPoints[i - 1] - _LocalPoints[i], _CurrentLength));
                }
            }
            RayPoint.TotalLength = _CurrentLength;
            RayPoint.RayRadius = RayRadius;
        }

        class RayPoint
        {
            public static float TotalLength = 0;
            public static float RayRadius = 0;
            public float CurrentLength = 0;
            public Vector3 Pos { get; }
            public Vector3 Dir { get; }
            public float Radius { get { return (TotalLength - CurrentLength) / TotalLength * (RayRadius * 2 / 3) + (RayRadius / 3); } }
            public RayPoint(Vector3 _Pos, Vector3 _Dir, float _CurrentLength)
            {
                Pos = _Pos;
                Dir = _Dir;
                CurrentLength = _CurrentLength;
            }
        }

    }
}
