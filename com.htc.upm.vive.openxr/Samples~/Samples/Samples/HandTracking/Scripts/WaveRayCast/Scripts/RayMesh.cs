using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VIVE.OpenXR.Samples.Ray {
    public class RayMesh : MonoBehaviour
    {
        //public Vector3 StartPoint, EndPoint;
        //public Vector3 StartPoint_Local, EndPoint_Local;
        public float Radius = 0.025f;

        //public GameObject Ball;

        public Material UsingMaterial;
        List<Vector3> Vertices = new List<Vector3>();
        List<int> Triangles = new List<int>();
        MeshFilter MeshF;
        MeshRenderer MeshR;
        Material Mat;
        private void Start()
        {
            /*
            //CreateShape(new Vector3[] { Vector3.zero, Vector3.right, Vector3.right + Vector3.up, Vector3.up });
            AddMesh(Get_CircleIndex().ToArray());
            AddMesh(Get_CornIndex().ToArray());
            UpdateMesh();
            */
        }

        public List<Vector3> Get_CircleIndex(Vector3 _Point, Vector3 _Dir)
        {
            List<Vector3> _Indexs = new List<Vector3>();

            for (int i = 0; i <= 360; i += 10)
            {
                float _Angle = i * Mathf.PI / 180;
                _Indexs.Add(new Vector3(Mathf.Cos(_Angle), Mathf.Sin(_Angle), 0) * Radius);
            }
            for (int i = 0; i < _Indexs.Count; i++)
            {
                _Indexs[i] = Quaternion.FromToRotation(Vector3.forward, _Dir) * _Indexs[i];
                _Indexs[i] += _Point;
            }
            _Indexs.Insert(0, new Vector3(_Point.x, _Point.y, _Point.z));
            return _Indexs;
        }

        public List<Vector3> Get_TubeIndex(Vector3 _StartPoint, Vector3 _EndPoint, float _StartRadius, float _EndRadius, Vector3 _StartDir, Vector3 _EndDir)
        {
            List<Vector3> _Indexs = new List<Vector3>();

            for (int i = 0; i <= 360; i += 10)
            {
                float _Angle = i * Mathf.PI / 180;
                _Indexs.Add(new Vector3(Mathf.Cos(_Angle), Mathf.Sin(_Angle), 0) * _StartRadius);
                _Indexs.Add(new Vector3(Mathf.Cos(_Angle), Mathf.Sin(_Angle), 0) * _EndRadius);
            }
            for (int i = 0; i < _Indexs.Count; i++)
            {
                //_Indexs[i] = Quaternion.FromToRotation(Vector3.forward, _EndPoint - _StartPoint) * _Indexs[i];
                _Indexs[i] = (i % 2 == 0 ? Quaternion.FromToRotation(Vector3.forward, (_EndPoint - _StartPoint).normalized + _StartDir.normalized) * _Indexs[i] : Quaternion.FromToRotation(Vector3.forward, (_EndPoint - _StartPoint).normalized + _EndDir.normalized) * _Indexs[i]);
                _Indexs[i] += (i % 2 == 0 ? _StartPoint : _EndPoint);
            }

            return _Indexs;
        }

        public List<Vector3> Get_CornIndex(Vector3 _StartPoint, Vector3 _EndPoint, float _StartRadius, Vector3 _Dir)
        {
            //Debug.Log("EndPoint: " + _EndPoint);
            List<Vector3> _Indexs = new List<Vector3>();

            for (int i = 360; i >= 0; i -= 10)
            {
                float _Angle = i * Mathf.PI / 180;
                _Indexs.Add(new Vector3(Mathf.Cos(_Angle), Mathf.Sin(_Angle), 0) * _StartRadius);
            }

            for (int i = 0; i < _Indexs.Count; i++)
            {
                _Indexs[i] = Quaternion.FromToRotation(Vector3.forward, _Dir) * _Indexs[i];
                _Indexs[i] += _StartPoint;
            }
            _Indexs.Insert(0, new Vector3(_EndPoint.x, _EndPoint.y, _EndPoint.z));
            return _Indexs;
        }
        int CurrentIndexNum = 0;
        public void AddMesh_TypeOne(Vector3[] _Vertices)
        {
            Vertices.AddRange(_Vertices);
            List<int> _Triangles = new List<int>();
            for (int i = CurrentIndexNum + 2; i < CurrentIndexNum + _Vertices.Length; i++)
            {
                _Triangles.Add(i - 1);
                _Triangles.Add(CurrentIndexNum);
                _Triangles.Add(i);
            }
            Triangles.AddRange(_Triangles);
            CurrentIndexNum += _Vertices.Length;
        }

        public void AddMesh_TypeTwo(Vector3[] _Vertices)
        {
            Vertices.AddRange(_Vertices);
            List<int> _Triangles = new List<int>();
            for (int i = CurrentIndexNum; i < CurrentIndexNum + _Vertices.Length - 3; i += 2)
            {
                _Triangles.Add(i + 1);
                _Triangles.Add(i);
                _Triangles.Add(i + 2);

                _Triangles.Add(i + 1);
                _Triangles.Add(i + 2);
                _Triangles.Add(i + 3);

            }
            Triangles.AddRange(_Triangles);
            CurrentIndexNum += _Vertices.Length;
        }

        public void CleanMesh()
        {
            Vertices.Clear();
            Triangles.Clear();
            CurrentIndexNum = 0;
        }

        public void UpdateMesh()
        {
            if (MeshF == null)
            {
                MeshF = GetComponent<MeshFilter>();
            }
            MeshF.mesh.Clear();
            MeshF.mesh.vertices = Vertices.ToArray();
            MeshF.mesh.triangles = Triangles.ToArray();
            CleanMesh();
            Give_Color();
        }

        void Give_Color()
        {
            if (MeshR == null)
            {
                MeshR = GetComponent<MeshRenderer>();
            }
            Mat = UsingMaterial;
            MeshR.material = Mat;
        }
        /*
        //---
        private void FixedUpdate()
        {
            //UpdateRay();
        }
        public float yAngle = 0;
        public float zAngle = 0;
        public float xAngle = 0;
        public Transform RayPose;
        List<GameObject> Points = new List<GameObject>();
        public void UpdateRay()
        {
            //StartPoint = Vector3.zero;
            //EndPoint = Quaternion.AngleAxis(xAngle, Vector3.right) * (Quaternion.AngleAxis(zAngle, Vector3.forward) * (Quaternion.AngleAxis(yAngle, Vector3.up)* (Vector3.forward * 100)));

            StartPoint_Local = RayPose.localPosition;
            EndPoint_Local = RayPose.localPosition + RayPose.forward * Length;
            StartPoint = RayPose.position;
            EndPoint = RayPose.position + RayPose.forward * Length;
            Debug.Log(RayPose.position);

            AddMesh(Get_CircleIndex(StartPoint_Local, RayPose.forward).ToArray());
            AddMesh(Get_CornIndex(StartPoint_Local, EndPoint_Local, RayPose.forward).ToArray());
            UpdateMesh();
            for(int i = 0; i < Points.Count; i++)
            {
                Destroy(Points[i]);
            }

            Points = new List<GameObject>();
            foreach (Vector3 _Pos in Get_Points())
            {
                Points.Add(Instantiate(Ball, _Pos, Ball.transform.rotation));
            }
        }



        public Transform PointTrans;
        Vector3 Point;
        Vector3 Dir_to_Point;
        Vector3 RayDir;
        Vector3 Normal;
        Vector3 Meeting_Point;
        int Scale = 10;
        int Start_Angle = 0;
        int End_Angle = 135;
        List<Vector3> Get_Points()
        {
            SetUp();
            float _Scaler;
            float _ButtonLength = Vector3.Distance(Point, Meeting_Point) / ((End_Angle - Start_Angle) / Scale);
            //Debug.Log(_ButtonLength);

            List<Vector3> _Points = new List<Vector3>();
            _Points.Add(Point);
            Vector3 _CurrentPoint = Point;
            for (int i = Scale + Start_Angle; i <= End_Angle; i += Scale)
            {
                float _Angle = i * Mathf.PI / 180;
                _Scaler = _ButtonLength / Mathf.Cos(_Angle);
                _CurrentPoint = _CurrentPoint + (Quaternion.AngleAxis(i, Normal) * -Dir_to_MeetingPoint) * _Scaler;
                //Debug.Log("Cos: " + (_Angle < 90 ? Mathf.Cos(_Angle) : -Mathf.Cos(_Angle))  + ", Scaler: " + _Scaler);
                _Points.Add(_CurrentPoint);
            }
            return _Points;
        }

        void SetUp()
        {
            Point = PointTrans.position;
            Dir_to_Point = Point - StartPoint;
            RayDir = EndPoint - StartPoint;
            Meeting_Point = Get_Meeting_Point();
            End_Angle = (int)Vector3.Angle(RayDir, Vector3.ProjectOnPlane(RayDir, Vector3.back)== Vector3.zero ? Vector3.one: Vector3.ProjectOnPlane(RayDir, Vector3.back));
            //Debug.Log("Angle: " + End_Angle);
        }
        Vector3 Dir_to_MeetingPoint = Vector3.zero;
        Vector3 Get_Meeting_Point()
        {
            Normal = Vector3.Cross(Dir_to_Point, RayDir).normalized;
            Vector3 _Temp = Normal * Get_Dis_from_Point_to_Line();
            _Temp = Quaternion.AngleAxis(90, RayDir) * _Temp;
            Dir_to_MeetingPoint = _Temp.normalized;
            return Point + _Temp;
        }

        float Get_Dis_from_Point_to_Line()
        {
            float _Angle = Vector3.Angle(Dir_to_Point, RayDir);
            return Dir_to_Point.magnitude * Mathf.Sin(_Angle * Mathf.PI / 180);
        }
        //---
        */















    }


}
