using UnityEngine;
using System.Collections;

public class UnitWeapon : MonoBehaviour
{
	[SerializeField] UnitProcess unit;
	[SerializeField] Collider weapon;

	void Start()
	{
		weapon = GetComponent<Collider>();
	}

	void Update()
	{		
		weapon.enabled = unit.AnimatorInfo.IsName( "Attack" );		
	}

	void OnTriggerEnter( Collider col )
	{
		Debug.Log( "On" );
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			Box temp = col.gameObject.GetComponent<Box>();
			temp.Hit( 40f );
			Debug.Log( "Hit" );
		}
	}
}
