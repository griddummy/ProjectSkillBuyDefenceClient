using UnityEngine;
using System;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
	[SerializeField] GameObject target;
	[SerializeField] float moveSpeed;

	void Update()
	{
		try
		{
			transform.position = Vector3.Slerp( transform.position, target.transform.position, Time.deltaTime * moveSpeed );
		}
		catch (MissingReferenceException e)
		{
			Debug.Log( e.InnerException );
			Destroy( gameObject );
		}
	}

	public void SetTargetAndSpeed( GameObject _target, float _moveSpeed )
	{
		target = _target;
		moveSpeed = _moveSpeed;
	}

	void OnTriggerEnter( Collider col )
	{
		Debug.Log( "Hit" );
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			Box temp = col.gameObject.GetComponent<Box>();
			temp.Hit( 40f );
			Destroy( gameObject );
			Debug.Log( "Hit" );
		}
		else if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
			Destroy( gameObject );


	}
}
