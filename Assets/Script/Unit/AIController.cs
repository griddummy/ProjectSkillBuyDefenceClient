using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{

	//complex data field
	[SerializeField] GameManager manager;
	[SerializeField] GameObject[] terrainSet;

	// Use this for initialization
	void Start()
	{
		manager = GameObject.FindWithTag( "GameManager" ).GetComponent<GameManager>();
		terrainSet = GameObject.FindGameObjectsWithTag( "PlayerSection" );
		foreach (GameObject elements in terrainSet)
			Debug.Log( elements.transform.position );
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
}
