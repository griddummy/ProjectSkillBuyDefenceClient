using UnityEngine;
using System;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
	[SerializeField] UnitProcess player;
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

	public void SetTargetAndSpeed( UnitProcess _player, GameObject _target, float _moveSpeed )
	{
		player = _player;
		target = _target;
		moveSpeed = _moveSpeed;
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			UnitProcess temp = col.gameObject.GetComponent<UnitProcess>();
			temp.Damaged( player.Info.Damage );
			Destroy( gameObject );
		}
		Destroy( gameObject );
	}
}
