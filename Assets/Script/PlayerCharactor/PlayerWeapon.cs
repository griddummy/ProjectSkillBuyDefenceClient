using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
	[SerializeField] PlayerProcess info;
	[SerializeField] Collider weapon;

	void Start()
	{
		weapon = GetComponent<Collider>();
	}

	void Update()
	{
		//Debug.Log( info.AnimatorInfo.IsName( "Attack" ) );		
		weapon.enabled = info.AnimatorInfo.IsName( "Attack" );		
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
