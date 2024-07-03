using System.Collections;
using UnityEngine;
using VIVE.OpenXR.Toolkits.RealisticHandInteraction;

namespace VIVE.OpenXR.Samples.RealisticHandInteraction
{
	public class CheckPackage : MonoBehaviour
	{
		private void OnEnable()
		{
			StartCoroutine(CheckHandTrackingValid());
		}

		private IEnumerator CheckHandTrackingValid()
		{
			yield return new WaitUntil(() => DataWrapper.Validate());
			gameObject.SetActive(false);
		}
	}
}
