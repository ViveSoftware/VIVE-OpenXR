using UnityEngine;
using VIVE.HandTracking;

namespace VIVE.HandTracking.Sample
{
    public class RenderModel : MonoBehaviour
    {
        private readonly Quaternion zBackModelRotFix = Quaternion.AngleAxis(180f, Vector3.up);

        public Transform[] nodes = new Transform[(int)XrHandJointEXT.XR_HAND_JOINT_MAX_ENUM_EXT];
        [Tooltip("Draw left hand if true, right hand otherwise")]
        public bool isLeft = false;
        [Tooltip("Use inferred or last-known posed when hand loses tracking if true.")]
        public bool allowUntrackedPose = false;
        [Tooltip("Root object of skinned mesh")]
        public GameObject Hand = null;
        private XrHandJointsMotionRangeEXT MotionType = XrHandJointsMotionRangeEXT.XR_HAND_JOINTS_MOTION_RANGE_MAX_ENUM_EXT;
        [Tooltip("Type of hand joints range of motion")]
        [ReadOnly]public string HandJointsMotionRange;


        // Start is called before the first frame update
        private void Start()
        {
            HandManager.StartFrameWork(isLeft);
        }

        // Update is called once per frame
        private void Update()
        {
            if (HandManager.GetJointLocation(isLeft, out var joints, ref MotionType))
            {
                setHandVisible(true);

                for (int i = (int)XrHandJointEXT.XR_HAND_JOINT_PALM_EXT; i < (int)XrHandJointEXT.XR_HAND_JOINT_MAX_ENUM_EXT; i++)
                {
                    var posValid = (joints[i].locationFlags & (ulong)XrSpaceLocationFlags.XR_SPACE_LOCATION_POSITION_VALID_BIT) != 0;
                    var posTracked = (joints[i].locationFlags & (ulong)XrSpaceLocationFlags.XR_SPACE_LOCATION_POSITION_TRACKED_BIT) != 0;
                    var rotValid = (joints[i].locationFlags & (ulong)XrSpaceLocationFlags.XR_SPACE_LOCATION_ORIENTATION_VALID_BIT) != 0;
                    var rotTracked = (joints[i].locationFlags & (ulong)XrSpaceLocationFlags.XR_SPACE_LOCATION_ORIENTATION_TRACKED_BIT) != 0;

                    var pos = new Vector3(joints[i].pose.position.x, joints[i].pose.position.y, -joints[i].pose.position.z);
                    var rot = new Quaternion(-joints[i].pose.orientation.x, -joints[i].pose.orientation.y, joints[i].pose.orientation.z, joints[i].pose.orientation.w) * zBackModelRotFix;

                    if (posValid && (allowUntrackedPose || posTracked)) { nodes[i].position = transform.TransformPoint(pos); }
                    if (rotValid && (allowUntrackedPose || rotTracked)) { nodes[i].rotation = transform.rotation * rot; }
                }
                switch (MotionType)
                {
                    case XrHandJointsMotionRangeEXT.XR_HAND_JOINTS_MOTION_RANGE_UNOBSTRUCTED_EXT:
                        HandJointsMotionRange = "UNOBSTRUCTED";
                        break;
                    case XrHandJointsMotionRangeEXT.XR_HAND_JOINTS_MOTION_RANGE_CONFORMING_TO_CONTROLLER_EXT:
                        HandJointsMotionRange = "CONTROLLER";
                        break;
                    default:
                        HandJointsMotionRange = "";
                        break;
                }
            }
            else
            {
                setHandVisible(false);
            }
        }

        private void OnDestroy()
        {
            HandManager.StopFrameWork(isLeft);
        }

        public void setHandVisible(bool isVisible)
        {
            Hand.SetActive(isVisible);
        }
    }

}
