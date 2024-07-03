using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	public class PhysicsInteractable : MonoBehaviour
	{
		[SerializeField]
		private float forceMultiplier = 1.0f;

		private readonly int MIN_POSE_SAMPLES = 2;
		private readonly int MAX_POSE_SAMPLES = 10;
		private readonly float MIN_VELOCITY = 0.5f;

		private Rigidbody interactableRigidbody;
		private List<Pose> movementPoses = new List<Pose>();
		private List<float> timestamps = new List<float>();
		private bool isBegin = false;
		private bool isEnd = false;
		private object lockVel = new object();

		private void Update()
		{
			if (interactableRigidbody == null) { return; }

			if (isBegin)
			{
				RecordMovement();
			}
		}

		private void FixedUpdate()
		{
			if (interactableRigidbody == null) { return; }

			if (isEnd)
			{
				interactableRigidbody.velocity = Vector3.zero;
				interactableRigidbody.angularVelocity = Vector3.zero;

				Vector3 velocity = CalculateVelocity();
				if (velocity.magnitude > MIN_VELOCITY)
				{
					interactableRigidbody.AddForce(velocity * forceMultiplier, ForceMode.Impulse);
				}
				interactableRigidbody = null;

				movementPoses.Clear();
				timestamps.Clear();
				isEnd = false;
			}
		}

		private void RecordMovement()
		{
			float time = Time.time;
			if (movementPoses.Count == 0 ||
				timestamps[movementPoses.Count - 1] != time)
			{
				movementPoses.Add(new Pose(interactableRigidbody.position, interactableRigidbody.rotation));
				timestamps.Add(time);
			}

			if (movementPoses.Count > MAX_POSE_SAMPLES)
			{
				movementPoses.RemoveAt(0);
				timestamps.RemoveAt(0);
			}
		}

		private Vector3 CalculateVelocity()
		{
			if (movementPoses.Count >= MIN_POSE_SAMPLES)
			{
				List<Vector3> velocities = new List<Vector3>();
				for (int i = 0; i < movementPoses.Count - 1; i++)
				{
					for (int j = i + 1; j < movementPoses.Count; j++)
					{
						velocities.Add(GetVelocity(i, j));
					}
				}
				Vector3 finalVelocity = FindBestVelocity(velocities);
				return finalVelocity;
			}
			return Vector3.zero;
		}

		private Vector3 GetVelocity(int idx1, int idx2)
		{
			if (idx1 < 0 || idx1 >= movementPoses.Count
				|| idx2 < 0 || idx2 >= movementPoses.Count
				|| movementPoses.Count < MIN_POSE_SAMPLES)
			{
				return Vector3.zero;
			}

			if (idx2 < idx1)
			{
				(idx1, idx2) = (idx2, idx1);
			}

			Vector3 currentPos = movementPoses[idx2].position;
			Vector3 previousPos = movementPoses[idx1].position;
			float currentTime = timestamps[idx2];
			float previousTime = timestamps[idx1];
			float timeDelta = currentTime - previousTime;
			if (currentPos == null || previousPos == null || timeDelta == 0)
			{
				return Vector3.zero;
			}

			Vector3 velocity = (currentPos - previousPos) / timeDelta;
			return velocity;
		}

		private Vector3 FindBestVelocity(List<Vector3> velocities)
		{
			Vector3 bestVelocity = Vector3.zero;
			float bestScore = float.PositiveInfinity;

			Parallel.For(0, velocities.Count, i =>
			{
				float score = 0f;
				for (int j = 0; j < velocities.Count; j++)
				{
					if (i != j)
					{
						score += (velocities[i] - velocities[j]).magnitude;
					}
				}

				lock (lockVel)
				{
					if (score < bestScore)
					{
						bestVelocity = velocities[i];
						bestScore = score;
					}
				}
			});

			return bestVelocity;
		}

		public void OnBeginInteractabled(IGrabbable grabbable)
		{
			if (grabbable is HandGrabInteractable handGrabbable)
			{
				interactableRigidbody = handGrabbable.rigidbody;
			}
			isBegin = true;
		}

		public void OnEndInteractabled(IGrabbable grabbable)
		{
			isBegin = false;
			isEnd = true;
		}
	}
}
