using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			Box temp = col.gameObject.GetComponent<Box>();
			temp.Hit( 40f );
			Debug.Log( "Hit" );
		}


	}
}
