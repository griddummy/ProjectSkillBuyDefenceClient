using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{

	//complex data field
	[SerializeField] GameManager manager;

	// Use this for initialization
	void Start()
	{
		manager = GameObject.FindWithTag( "GameManager" ).GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
