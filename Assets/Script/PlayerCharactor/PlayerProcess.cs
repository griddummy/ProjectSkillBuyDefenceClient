using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerProcess : MonoBehaviour
{
	//simple field
	[SerializeField] bool isAttack;
	[SerializeField] bool isMove;
	[SerializeField] bool isChase;
	[SerializeField] Rigidbody playerRig;
	[SerializeField] Vector3 destination;
	[SerializeField] State presentState;
	[SerializeField] PlayerInformation info;
	[SerializeField] Vector3 velocity;

	//complex field
	[SerializeField] GameObject targetEnemy;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorStateInfo animatorInfo;
	[SerializeField] NavMeshAgent moveAgent;

	public enum State
	{
		Idle,
		Move,
		Attack,
		AttackMove,
		Hold,
		Skill,
		Die}
;

	public enum AnimatorState : int
	{
		Idle = 1,
		Run = 2,
		Attack = 3,
		Casting = 4,
		CastingUltimate = 5,
		Die = 6}
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
		playerRig = GetComponent<Rigidbody>();
		destination = transform.position;
		animator = GetComponent<Animator>();
		animatorInfo = animator.GetCurrentAnimatorStateInfo( 0 );
		moveAgent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		UpdateNavMeshData();
	
		switch (presentState)
		{
			case State.Idle:
				IdleProcess();
				break;
			case State.Move:
				MoveProcess();
				break;
			case State.Attack:
				AttackProcess();
				break;
			case State.AttackMove:
				AttackMoveProcess();
				break;
			case State.Hold:
				HoldProcess();
				break;
		}
	}

	//synchronization navmesh data
	void UpdateNavMeshData()
	{
		moveAgent.speed = info.MoveSpeed;
	}

	void IdleProcess()
	{
		ActiveAnimator( AnimatorState.Idle );
	}

	void MoveProcess()
	{
		ActiveAnimator( AnimatorState.Run );
		transform.LookAt( destination );
		if (Vector3.Distance( transform.position, destination ) <= 0.1f)
		{
			presentState = State.Idle;
			ActiveAnimator( AnimatorState.Idle );
		}
	}

	void AttackProcess()
	{
		if (( targetEnemy != null ) && ( Vector3.Distance( targetEnemy.transform.position, transform.position ) > info.AttackRange ))
		{
			moveAgent.SetDestination( targetEnemy.transform.position );
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Run );
			Debug.Log( moveAgent.destination );
		}
		else if (( targetEnemy != null ) && !isAttack && ( Vector3.Distance( targetEnemy.transform.position, transform.position ) <= info.AttackRange ))
		{
			moveAgent.ResetPath();
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Attack );
			isAttack = true;
			Debug.Log( "Attack" );
		}
		else if (targetEnemy == null)
		{
			//find target -> no target state idle
			// -> reset target
			if (FindTarget())
				return;
			else
				presentState = State.Idle;				
		}
	}

	void AttackMoveProcess()
	{

	}

	void HoldProcess()
	{

	}

	//use process
	bool FindTarget()
	{
		return false;
	}

	//set player present state
	void ActiveAnimator( AnimatorState present )
	{
		switch (present)
		{
			case AnimatorState.Idle:
				animator.SetInteger( "State", (int) AnimatorState.Idle );
				break;
			case AnimatorState.Run:
				animator.SetInteger( "State", (int) AnimatorState.Run );
				break;
			case AnimatorState.Attack:
				animator.Play( "Idle" );
				animator.SetTrigger( "Attack" );
				break;
			case AnimatorState.Casting:
				animator.SetTrigger( "Casting" );
				break;
			case AnimatorState.CastingUltimate:
				animator.SetTrigger( "CastingUltimate" );
				break;
			case AnimatorState.Die:
				animator.SetTrigger( "Die" );
				break;

		}
	}

	//public method

	//set attack target
	public void SetAttackTarget( GameObject target )
	{
		destination = transform.position;
		targetEnemy = target;
		presentState = State.Attack;
		isAttack = false;
		Debug.Log( "SetAttack" );
	}

	//set destination for navmesh move
	public void SetDestination( Vector3 point )
	{
		destination = point;
		moveAgent.destination = destination;
		presentState = State.Move;
	}

	//use attack animation
	public void EndAttackAnimation()
	{
		isAttack = false;
	}

}


