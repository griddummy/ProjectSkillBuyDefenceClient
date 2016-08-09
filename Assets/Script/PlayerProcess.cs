using UnityEngine;
using System.Collections;

public class PlayerProcess : MonoBehaviour
{
	//simple field
	[SerializeField] Vector3 destination;
	[SerializeField] float moveSpeed;
	[SerializeField] State presentState;

	//complex field
	[SerializeField] Animator characterAnimator;

	public enum State : int
	{
		Idle = 1,
		Run = 2}
;

	//property
	public Vector3 Destination
	{
		get { return destination; }
		set { destination = value; }
	}

	//initialize this script
	void Start()
	{
		destination = transform.position;
		characterAnimator = GetComponent<Animator>();
	}

	void Update()
	{
		
		Move();
	}

	void Move()
	{
		Vector3 direction = destination - transform.position;
		if (direction.magnitude >= 0.5f)
		{
			SetState( State.Run );
			transform.LookAt( destination );
			direction.Normalize();
			transform.Translate( direction * Time.deltaTime * moveSpeed, Space.World );
		}
		else
			SetState( State.Idle );
	}

	void SetState( State present )
	{
		switch (present)
		{
			case State.Idle:
				presentState = State.Idle;
				characterAnimator.SetInteger( "State", (int) State.Idle );
				break;
			case State.Run:
				presentState = State.Run;
				characterAnimator.SetInteger( "State", (int) State.Run );
				break;
		}
	}
}
