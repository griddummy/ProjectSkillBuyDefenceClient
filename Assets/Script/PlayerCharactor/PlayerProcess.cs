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
	[SerializeField] Collider[] enemy;
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

	public GameObject TargetEnemy
	{
		get { return targetEnemy; }
	}

	public AnimatorStateInfo AnimatorInfo
	{
		get { return animatorInfo; }
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
		Debug.Log( animatorInfo.IsName( "Attack" ) );
	}

	//synchronization navmesh data
	void UpdateNavMeshData()
	{
		moveAgent.speed = info.MoveSpeed;
	}

	//idle state process
	//find target -> if find target state chanage by attack
	void IdleProcess()
	{
		ActiveAnimator( AnimatorState.Idle );

		if (FindTarget())
			presentState = State.Attack;
		else
			return;
	}

	//move state process
	//only move to destination
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

	//attack state process
	//chase and attack target
	void AttackProcess()
	{
		if (( targetEnemy != null ) && ( Vector3.Distance( targetEnemy.transform.position, transform.position ) > info.AttackRange ))
		{
			moveAgent.SetDestination( targetEnemy.transform.position );
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Run );
		}
		else if (( targetEnemy != null ) && !isAttack && ( Vector3.Distance( targetEnemy.transform.position, transform.position ) <= info.AttackRange ))
		{
			moveAgent.ResetPath();
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );
			isAttack = true;
		}
		else if (targetEnemy == null)
		{
			//find target -> no target state idle
			// -> reset target
			if (FindTarget())
				return;
			else if (presentState == State.AttackMove)
				return;
			else
				presentState = State.Idle;				
		}
	}

	//attack move state process
	//move to destination
	//if find target -> battle by target
	//battle over find target
	//no target -> move destination
	void AttackMoveProcess()
	{
		if (( targetEnemy == null ) && !FindTarget())
		{
			moveAgent.SetDestination( destination );
			MoveProcess();
		}
		else
			AttackProcess();
	}

	//hold state process
	//hold on transfrom position
	//if find target -> battle by target
	void HoldProcess()
	{
		if (( targetEnemy == null ) && !FindTarget())
			ActiveAnimator( AnimatorState.Idle );
		else if (!isAttack)
		{
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );
			isAttack = true;
		}
	}

	//use process
	bool FindTarget()
	{
		//make collider array -> enemy target in range
		if (presentState != State.Hold)
			enemy = Physics.OverlapSphere( transform.position, info.SearchRange, 1 << LayerMask.NameToLayer( "Enemy" ) );
		else
			enemy = Physics.OverlapSphere( transform.position, info.AttackRange, 1 << LayerMask.NameToLayer( "Enemy" ) );

		//if no enemy -> return false
		if (enemy.Length == 0)
			return false;
		else
		{
			//find shortest distance target
			for (int i = 0; i < enemy.Length; i++)
			{
				if (( ( i == 0 ) || ( Vector3.Distance( enemy[i].transform.position, transform.position ) <= Vector3.Distance( TargetEnemy.transform.position, transform.position ) ) )
				    && ( enemy[i].gameObject.layer == LayerMask.NameToLayer( "Enemy" ) ))
					targetEnemy = enemy[i].gameObject;
			}
			return true;		
		}
	}

	//set player present state
	void ActiveAnimator( AnimatorState present )
	{
		switch (present)
		{
			case AnimatorState.Idle:
				animator.SetInteger( "State", (int) AnimatorState.Idle );
				if (!animatorInfo.IsName( "Idle" ))
					animator.Play( "Idle" );				
				break;
			case AnimatorState.Run:
				animator.SetInteger( "State", (int) AnimatorState.Run );
				if (!animatorInfo.IsName( "Run" ))
					animator.Play( "Run" );	
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

	//character stop
	public void SetStop()
	{
		presentState = State.Idle;
		targetEnemy = null;
		moveAgent.ResetPath();
		ActiveAnimator( AnimatorState.Idle );
	}

	//set hold state
	public void SetHold()
	{
		presentState = State.Hold;
		targetEnemy = null;
		moveAgent.ResetPath();
		ActiveAnimator( AnimatorState.Idle );
	}

	//set attack state & set attack target
	public void SetAttackTarget( GameObject target )
	{
		destination = transform.position;
		targetEnemy = target;
		presentState = State.Attack;
		isAttack = false;
	}

	//set destination for nav mesh move
	public void SetDestination( Vector3 point )
	{
		presentState = State.Move;
		destination = point;
		moveAgent.destination = destination;
	}

	//set destination for nav mesh move
	//search range -> find enemy
	public void SetAttackDestination( Vector3 point )
	{
		presentState = State.AttackMove;
		destination = point;
		moveAgent.destination = destination;
	}

	//use attack animation
	public void EndAttackAnimation()
	{
		isAttack = false;	
	}

}


