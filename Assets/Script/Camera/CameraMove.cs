using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
	[SerializeField] Vector3 distanceVector;
	[SerializeField] float lerpSpeed;
	[SerializeField] float pointX;
	[SerializeField] float pointY;
	[SerializeField] float pointZ;
	[SerializeField] float screenHeight;
	[SerializeField] float screenWidth;
	[SerializeField] Vector3 mousePosition;
	[SerializeField] Vector3 mouseDirection;
	[SerializeField] Vector3 keyboardDirection;
	[SerializeField][Range( 0.1f, 1.6f )] float sensitivity;

	void Start()
	{
		lerpSpeed = 1f;
		sensitivity = 0.8f;
		pointY = 20f;
	}

	void Update()
	{
		pointX = Input.GetAxis( "Horizontal" ) * sensitivity;
		pointZ = Input.GetAxis( "Vertical" ) * sensitivity;

		keyboardDirection = new Vector3 ( pointX, 0f, pointZ );

		mousePosition = Input.mousePosition;
		screenHeight = Screen.height;
		screenWidth = Screen.width;

		if (mousePosition.y <= screenHeight * 0.01f && mousePosition.x <= screenWidth * 0.01f)
		{
			pointX = -1f * sensitivity;
			pointZ = -1f * sensitivity;
		}
		else if (mousePosition.y >= screenHeight * 0.99f && mousePosition.x >= screenWidth * 0.99f)
		{
			pointX = 1f * sensitivity;
			pointZ = 1f * sensitivity;
		}
		else if (mousePosition.y >= screenHeight * 0.99f && mousePosition.x <= screenWidth * 0.01f)
		{
			pointX = -1f * sensitivity;
			pointZ = 1f * sensitivity;
		}
		else if (mousePosition.y <= screenHeight * 0.01f && mousePosition.x >= screenWidth * 0.99f)
		{
			pointX = 1f * sensitivity;
			pointZ = -1f * sensitivity;
		}
		else if (mousePosition.x <= screenWidth * 0.01f)
		{
			pointX = -1f * sensitivity;
			pointZ = 0f;
		}
		else if (mousePosition.y >= screenHeight * 0.99f)
		{
			pointX = 0f;
			pointZ = 1f * sensitivity;
		}
		else if (mousePosition.x >= screenWidth * 0.99f)
		{
			pointX = 1f * sensitivity;
			pointZ = 0f;
		}
		else if (mousePosition.y <= screenHeight * 0.01f)
		{
			pointX = 0f;
			pointZ = -1f * sensitivity;
		}

		mouseDirection = new Vector3 ( pointX, 0f, pointZ );

		transform.position += ( ( keyboardDirection + mouseDirection ).normalized );

		pointY = 0f;
		pointZ = 0f;

		if (Input.GetAxis( "Mouse ScrollWheel" ) > 0)
		{
			pointY += transform.forward.y * Time.deltaTime * 20f * sensitivity;
			pointZ -= transform.forward.z * Time.deltaTime * -10f * sensitivity;
		}
		else if (Input.GetAxis( "Mouse ScrollWheel" ) < 0)
		{
			pointY -= transform.forward.y * Time.deltaTime * 20f * sensitivity;
			pointZ += transform.forward.z * Time.deltaTime * -10f * sensitivity;
		}

		transform.position += new Vector3 ( 0f, pointY, pointZ );
	}
}
