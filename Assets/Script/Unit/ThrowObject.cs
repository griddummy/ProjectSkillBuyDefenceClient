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
			transform.position = Vector3.Slerp( transform.position, target.transform.position + new Vector3 ( 0f, target.transform.lossyScale.y / 2, 0f ), Time.deltaTime * moveSpeed );
		}
		catch (MissingReferenceException e)
		{
			Debug.Log( e.InnerException );
			Destroy( gameObject );
		}
		Debug.Log( transform.position );
	}

	public void SetTargetAndSpeed( UnitProcess _player, GameObject _target )
	{
		player = _player;
		target = _target;
		moveSpeed = 3f;
	}

	void OnTriggerEnter( Collider col )
	{
		Debug.Log( "Active!" );
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			UnitProcess temp = col.gameObject.GetComponent<UnitProcess>();
			temp.Damaged( player.Info.Damage );
			Destroy( gameObject );
		}
		if (col.gameObject.layer != LayerMask.NameToLayer( "Player" ))
			Destroy( gameObject );
	}
}
