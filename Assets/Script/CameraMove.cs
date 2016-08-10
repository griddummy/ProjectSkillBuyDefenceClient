using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
	[SerializeField] Transform player;
	[SerializeField] Vector3 distanceVector;
	[SerializeField] float lerpSpeed;

	void Update()
	{
		Camera.main.transform.position = Vector3.Lerp( Camera.main.transform.position, player.position + distanceVector, Time.deltaTime * lerpSpeed );
	}
}
