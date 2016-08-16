using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitProcess : MonoBehaviour
{
	//simple field
	[SerializeField] protected bool isMeleeAttack;
	[SerializeField] protected bool onMeleeAttack;
	[SerializeField] protected Rigidbody playerRig;
	[SerializeField] Vector3 destination;
	[SerializeField] protected State presentState;
	[SerializeField] protected UnitInformation info;
	[SerializeField] Vector3 velocity;
	[SerializeField] protected Collider[] enemy;

	//complex field
	[SerializeField] protected GameObject targetEnemy;
	[SerializeField] protected GameObject throwObject;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorStateInfo animatorInfo;
	[SerializeField] protected NavMeshAgent moveAgent;
	[SerializeField] protected GameManager manager;

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

	public UnitInformation Info
	{
		get { return info; }
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
		presentState = State.Idle;
		animator = GetComponent<Animator>();
		moveAgent = GetComponent<NavMeshAgent>();
		manager = GameObject.FindWithTag( "GameManager" ).GetComponent<GameManager>();
		SetLayer();
	}

	//Update is called once per frame
	void Update()
	{
		PreProcess();
		SkillProcess();
	
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
			case State.Die:
				DieProcess();
				break;
		}
	}

	//set layer -> use player information
	protected void SetLayer()
	{
		if (info.PlayerNumber == manager.PlayerNumber)
			gameObject.layer = LayerMask.NameToLayer( "Player" );
		else if (manager.CheckAlly[info.PlayerNumber])
			gameObject.layer = LayerMask.NameToLayer( "Ally" );
		else
			gameObject.layer = LayerMask.NameToLayer( "Enemy" );
	
	}

	//synchronization nav mesh data
	protected virtual void PreProcess()
	{
		moveAgent.speed = info.MoveSpeed;
		animatorInfo = this.animator.GetCurrentAnimatorStateInfo( 0 );

		if (info.HealthPoint <= 0)
			presentState = State.Die;
	}

	//check skill and process coolTime
	protected void SkillProcess()
	{
		for (int i = 0; i < info.ActiveSkillSet.Length; i++)
		{
			if (info.OnSkill[i])
			{
				info.CoolTime[i] += Time.deltaTime;
				if (info.CoolTime[i] > info.ActiveSkillSet[i].CoolTime)
				{
					info.CoolTime[i] = 0.0f;
					info.OnSkill[i] = false;
				}
			}
		}
	}

	//idle state process
	//find target -> if find target state chanage by attack
	protected virtual void IdleProcess()
	{
		ActiveAnimator( AnimatorState.Idle );

		if (FindTarget())
			presentState = State.Attack;
		else
			return;
	}

	//move state process
	//only move to destination
	protected void MoveProcess()
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
	protected virtual void AttackProcess()
	{
		if (( targetEnemy != null ) && ( Vector3.Distance( targetEnemy.transform.position, transform.position ) > info.AttackRange ) && !animatorInfo.IsName( "Attack" ))
		{
			moveAgent.SetDestination( targetEnemy.transform.position );
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Run );
		}
		else if (( targetEnemy != null ) && !animatorInfo.IsName( "Attack" ) && ( onMeleeAttack || ( Vector3.Distance( targetEnemy.transform.position, transform.position ) <= info.AttackRange ) ))
		{
			moveAgent.ResetPath();
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );
		}
		else if (targetEnemy == null)
		{
			onMeleeAttack = false;

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
	protected void AttackMoveProcess()
	{
		if (( targetEnemy == null ) && !FindTarget() && !animatorInfo.IsName( "Attack" ))
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
	protected void HoldProcess()
	{
		if (( targetEnemy == null ) && !FindTarget())
			ActiveAnimator( AnimatorState.Idle );
		else if (!animatorInfo.IsName( "Attack" ))
		{
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );		
		}
	}

	protected void DieProcess()
	{
		if (!animatorInfo.IsName( "Die" ))
		{
			ActiveAnimator( AnimatorState.Die );
			Destroy( gameObject, 3f );
		}
	}

	//use process
	protected bool FindTarget()
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
				break;
			case AnimatorState.Run:
				animator.SetInteger( "State", (int) AnimatorState.Run );
				break;
			case AnimatorState.Attack:
				if (isMeleeAttack && !( animatorInfo.IsName( "Attack" ) || animatorInfo.IsName( "ThrowAttack" ) ))
					animator.SetTrigger( "Attack" );
				else if (!( animatorInfo.IsName( "Attack" ) || animatorInfo.IsName( "ThrowAttack" ) ))
					animator.SetTrigger( "ThrowAttack" );
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

	//on Collision Enter
	public void OnCollisionEnter( Collision col )
	{
		Debug.Log( col.gameObject );
		if (isMeleeAttack && col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
			onMeleeAttack = true;
			
	}
	//character stop
	public void SetStop()
	{
		if (presentState != State.Die)
		{
			presentState = State.Idle;
			targetEnemy = null;
			moveAgent.ResetPath();
			ActiveAnimator( AnimatorState.Idle );
		}
	}

	//set hold state
	public void SetHold()
	{
		if (presentState != State.Die)
		{
			presentState = State.Hold;
			targetEnemy = null;
			moveAgent.ResetPath();
			ActiveAnimator( AnimatorState.Idle );
		}
	}

	//set attack state & set attack target
	public void SetAttackTarget( GameObject target )
	{
		if (presentState != State.Die)
		{
			destination = transform.position;
			targetEnemy = target;
			presentState = State.Attack;
		}
	}

	//set destination for nav mesh agent
	public void SetDestination( Vector3 point )
	{
		if (presentState != State.Die)
		{
			presentState = State.Move;
			destination = point;
			moveAgent.destination = destination;
		}
	}

	//set destination for nav mesh agent
	//search range -> find enemy & attack
	public void SetAttackDestination( Vector3 point )
	{
		if (presentState != State.Die)
		{
			presentState = State.AttackMove;
			destination = point;
			moveAgent.destination = destination;
		}
	}

	//unit use active skill
	public void ActiveSkill( int index )
	{		
		if (presentState != State.Die)
		{
			info.OnSkill[index] = true;
			info.ActiveSkillSet[index].UseSkill();
			moveAgent.ResetPath();
			animator.Play( "Casting" );
		}
	}

	//use throw attack
	public void ThrowAttackObject()
	{		
		GameObject temp = (GameObject) Instantiate( throwObject, transform.position, transform.rotation );
		temp.GetComponent<ThrowObject>().SetTargetAndSpeed( this, targetEnemy, info.AttackSpeed );
	}

	public void ReloadAnimatorInfo()
	{
		animatorInfo = animator.GetCurrentAnimatorStateInfo( 0 );
	}

	public void Damaged( int damage )
	{
		info.HealthPoint -= damage;
	}

	//use attack animation -> animation event
	public void EndAttackAnimation()
	{
		
	}

	//use casting animation -> animation event
	public void EndCastingAnimation()
	{

	}

}


