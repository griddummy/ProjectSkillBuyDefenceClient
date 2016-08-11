using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
	[SerializeField] Transform player;
	[SerializeField] Vector3 distanceVector;
	[SerializeField] float lerpSpeed;
	[SerializeField][Range( 0.1f, 10f )] float sensitivity;

	void Update()
	{
		if (Input.GetAxis( "Mouse ScrollWheel" ) > 0)
			distanceVector -= new Vector3 ( 0, Time.deltaTime * 20f * sensitivity, Time.deltaTime * -10f * sensitivity );
		if (Input.GetAxis( "Mouse ScrollWheel" ) < 0)
			distanceVector += new Vector3 ( 0, Time.deltaTime * 20f * sensitivity, Time.deltaTime * -10f * sensitivity );
		
		float y = Mathf.Clamp( distanceVector.y, 5f, 20f );
		float z = Mathf.Clamp( distanceVector.z, -10f, -2.5f );
		distanceVector = new Vector3 ( 0, y, z );

		Camera.main.transform.position = Vector3.Lerp( Camera.main.transform.position, player.position + distanceVector, Time.deltaTime * lerpSpeed );
	}
}
