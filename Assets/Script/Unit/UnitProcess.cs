using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//player unit process -> movable unit
public class UnitProcess : MonoBehaviour
{
	//simple data field
	[SerializeField] protected Rigidbody playerRig;
	[SerializeField] protected Vector3 destination;
	[SerializeField] protected State presentState;
	[SerializeField] protected UnitInformation info;
	[SerializeField] protected int presentSkillIndex;
	[SerializeField] protected Collider[] enemy;

	//complex data field
	[SerializeField] protected GameObject unitTarget;
	[SerializeField] protected GameObject throwObject;
	[SerializeField] protected Animator animator;
	[SerializeField] protected AnimatorStateInfo animatorInfo;
	[SerializeField] protected NavMeshAgent moveAgent;
	[SerializeField] protected GameManager manager;
	[SerializeField] protected GameObject[] auraEffect;

	public enum State
	{
		Idle,
		Move,
		Attack,
		AttackMove,
		Hold,
		CastunitTargetSkill,
		Die}
;

	public enum AnimatorState : int
	{
		Idle = 1,
		Run = 2,
		Attack = 3,
		ThrowAttack = 4,
		Casting = 5,
		CastingUltimate = 6,
		Die = 7}
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

	public GameObject Target
	{
		get { return unitTarget; }
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
		manager = FindObjectOfType<GameManager>();
		DataInitialize();
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
			case State.CastunitTargetSkill:
				CastTargetSkillProcess();
				break;
			case State.Die:
				DieProcess();
				break;
		}
	}

	//set layer -> use player information
	protected void DataInitialize()
	{
		info.SkillInitalize();
		info.SetDefaultMelee();

		if (info.PlayerNumber == manager.PlayerNumber)
			gameObject.layer = LayerMask.NameToLayer( "Player" );
		else if (manager.CheckAlly[info.PlayerNumber - 1])
			gameObject.layer = LayerMask.NameToLayer( "Ally" );
		else
			gameObject.layer = LayerMask.NameToLayer( "Enemy" );

		auraEffect = new GameObject[6];
	}

	//synchronization nav mesh data
	protected virtual void PreProcess()
	{
		moveAgent.speed = info.MoveSpeed;
		animator.speed = info.AttackSpeed;
		animatorInfo = this.animator.GetCurrentAnimatorStateInfo( 0 );

		if (animatorInfo.IsName( "Attack" ))
			moveAgent.updatePosition = false;
		else
			moveAgent.updatePosition = true;

		//aura move
		for (int i = 0; i < auraEffect.Length; i++)
			if (auraEffect[i] != null)
				auraEffect[i].transform.position = transform.position + ( Vector3.up * 0.3f );

		if (info.PresentHealthPoint <= 0)
			presentState = State.Die;
		
	}

	//check skill and process coolTime
	protected void SkillProcess()
	{
		//check active skill cool Time
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
		//create aura effect 
		for (int i = 0; i < info.PassiveSkillSet.Length; i++)
		{
			if (info.PassiveSkillSet[i].SkillType == Skill.Type.PassiveAura && !info.UnitBuffSet[i].Activate)
			{
				auraEffect[i] = (GameObject) Instantiate( Resources.Load<GameObject>( "SkillEffect/" + info.PassiveSkillSet[i].Name ), transform.position + ( Vector3.up * 0.3f ), new Quaternion ( 0f, 0f, 0f, 0f ) );
			}
		}

		//process buff
		for (int i = 0; i < info.UnitBuffSet.Length; i++)
		{
			if (info.UnitBuffSet[i].ID != 0)
				info.UnitBuffSet[i].ActiveBuff( this );	
		}


	}

	//idle state process
	//find unitTarget -> if find unitTarget state chanage by attack
	protected virtual void IdleProcess()
	{
		ActiveAnimator( AnimatorState.Idle );

		if (FindUnitTarget())
			presentState = State.Attack;
		else
			return;
	}

	//move state process
	//only move to destination
	protected void MoveProcess()
	{
		if (Vector3.Distance( transform.position, destination ) <= 0.1f)
		{
			presentState = State.Idle;
			ActiveAnimator( AnimatorState.Idle );
			// send unit stop
			manager.UnitStop( this );
		}
		else
		{
			if (!animatorInfo.IsName( "Run" ))
				animator.Play( "Idle" );
			ActiveAnimator( AnimatorState.Run );
			transform.LookAt( destination );
			// send unit data - call every frame...
		}
	}

	//attack state process
	//chase and attack unitTarget
	protected virtual void AttackProcess()
	{
		if (( unitTarget != null ) && ( Vector3.Distance( unitTarget.transform.position, transform.position ) > 3f ))
		{
			// Chase
			if (animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );
			moveAgent.SetDestination( unitTarget.transform.position );
			transform.LookAt( unitTarget.transform );
			ActiveAnimator( AnimatorState.Run );
		}
		else if (( unitTarget != null ) && ( ( Vector3.Distance( unitTarget.transform.position, transform.position ) <= 3f ) ))
		{
			//Attack
			if (!animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );
			moveAgent.ResetPath();
			transform.LookAt( unitTarget.transform );
			ActiveAnimator( AnimatorState.Attack );
			//send unit data
			manager.UnitAttack( this, unitTarget );
		}
		else if (unitTarget == null)
		{
			// set animation state
			if (animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );
			
			// find unitTarget -> no unitTarget state idle
			// -> reset unitTarget
			if (FindUnitTarget())
			{
				return;
			}
			else if (presentState == State.AttackMove)
			{
				// send unit set destination
				manager.UnitSetDestination( this, destination );
				return;
			}
			else
			{
				presentState = State.Idle;
				// send unit stop
				manager.UnitStop( this );
			}			
		}
	}

	//attack move state process
	//move to destination
	//if find unitTarget -> battle by unitTarget
	//battle over find unitTarget
	//no unitTarget -> move destination
	protected void AttackMoveProcess()
	{
		if (( unitTarget == null ) && !FindUnitTarget() && !animatorInfo.IsName( "Attack" ))
		{
			moveAgent.SetDestination( destination );
			MoveProcess();
		}
		else
			AttackProcess();
	}

	//hold state process
	//hold on transfrom position
	//if find unitTarget -> battle by unitTarget
	protected void HoldProcess()
	{
		if (!animatorInfo.IsName( "Attack" ))
			animator.Play( "Idle" );

		if (( unitTarget == null ) && !FindUnitTarget())
			ActiveAnimator( AnimatorState.Idle );
		else if (Vector3.Distance( unitTarget.transform.position, transform.position ) > info.AttackRange)
		{
			animator.Play( "Idle" );
			unitTarget = null;
		}
		else if (!animatorInfo.IsName( "Attack" ))
		{
			transform.LookAt( unitTarget.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );
			// send unit data - Attack
			manager.UnitAttack( this, unitTarget );
		}
	}

	//cast unitTarget skill state process
	//chase unitTarget & cast skill by unitTarget
	protected void CastTargetSkillProcess()
	{        
		if (( unitTarget != null ) && Vector3.Distance( unitTarget.transform.position, transform.position ) >= info.ActiveSkillSet[presentSkillIndex].SkillRange + unitTarget.transform.lossyScale.x / 2)
		{		
			//set animation state
			if (animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );

			//chase unitTarget
			moveAgent.SetDestination( unitTarget.transform.position );
			transform.LookAt( unitTarget.transform );
			ActiveAnimator( AnimatorState.Run );
		}
		else if (( unitTarget != null ) && ( !info.OnSkill[presentSkillIndex] ) && Vector3.Distance( unitTarget.transform.position, transform.position ) < info.ActiveSkillSet[presentSkillIndex].SkillRange + unitTarget.transform.lossyScale.x / 2)
		{
			//active skill
			if (!animatorInfo.IsName( "Casting" ))
				animator.Play( "Idle" );
			moveAgent.ResetPath();
			transform.LookAt( unitTarget.transform );
			ActiveAnimator( AnimatorState.Casting );

			info.ActiveSkillUse( presentSkillIndex, unitTarget.GetComponent<UnitProcess>(), destination );

			// send cast skill - target
			manager.UnitCastSkillActiveTarget( this, presentSkillIndex, unitTarget );
		}
		else if (Vector3.Distance( destination, transform.position ) >= info.ActiveSkillSet[presentSkillIndex].SkillRange)
		{	
			//set animation state
			if (animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );

			//chase unitTarget
			moveAgent.SetDestination( destination );
			transform.LookAt( destination );
			ActiveAnimator( AnimatorState.Run );
		}
		else if (!info.OnSkill[presentSkillIndex] && Vector3.Distance( destination, transform.position ) < info.ActiveSkillSet[presentSkillIndex].SkillRange)
		{
			//active skill
			if (!animatorInfo.IsName( "Casting" ))
				animator.Play( "Idle" );
			moveAgent.ResetPath();
			transform.LookAt( destination );
			ActiveAnimator( AnimatorState.Casting );
		
			info.ActiveSkillUse( presentSkillIndex, null, destination );

			info.OnSkill[presentSkillIndex] = true;
			//send unit data - non target - ground
			manager.UnitCastSkillTargetArea( this, presentSkillIndex, destination );
		}
	}

	protected void DieProcess()
	{
		if (!animatorInfo.IsName( "Die" ))
		{
			ActiveAnimator( AnimatorState.Die );
			//send unit data
			manager.UnitDeath( this );
			Destroy( gameObject, 3f );
		}
	}

	//use process

	//find enemy unitTarget
	protected bool FindUnitTarget()
	{
		if (this.gameObject.layer == LayerMask.NameToLayer( "Player" ))
		{
			//make collider array -> enemy unitTarget in range
			if (presentState != State.Hold)
				enemy = Physics.OverlapSphere( transform.position, info.SearchRange, 1 << LayerMask.NameToLayer( "Enemy" ) );
			else
				enemy = Physics.OverlapSphere( transform.position, info.AttackRange, 1 << LayerMask.NameToLayer( "Enemy" ) );

			//if no enemy -> return false
			if (enemy.Length == 0)
				return false;
			else
			{
				//find shortest distance unitTarget
				for (int i = 0; i < enemy.Length; i++)
				{
					if (( ( i == 0 ) || ( Vector3.Distance( enemy[i].transform.position, transform.position ) <= Vector3.Distance( unitTarget.transform.position, transform.position ) ) )
					    && ( enemy[i].gameObject.layer == LayerMask.NameToLayer( "Enemy" ) ))
					{
						unitTarget = enemy[i].gameObject;
						//send unit set target
						manager.UnitSetTarget( this, unitTarget );
						break;
					}					
				}
			
				return true;		
			}
		}
		else if (this.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			int layer = LayerMask.NameToLayer( "Player" );
			layer |= LayerMask.NameToLayer( "Ally" );

			//make collider array -> enemy unitTarget in range
			if (presentState != State.Hold)
				enemy = Physics.OverlapSphere( transform.position, info.SearchRange, 1 << layer );
			else
				enemy = Physics.OverlapSphere( transform.position, info.AttackRange, 1 << layer );

			//if no enemy -> return false
			if (enemy.Length == 0)
				return false;
			else
			{
				//find shortest distance unitTarget
				for (int i = 0; i < enemy.Length; i++)
				{
					if (( ( i == 0 ) || ( Vector3.Distance( enemy[i].transform.position, transform.position ) <= Vector3.Distance( unitTarget.transform.position, transform.position ) ) )
					    && ( enemy[i].gameObject.layer == LayerMask.NameToLayer( "Player" ) ))
					{
						unitTarget = enemy[i].gameObject;
						//send unit set target
						manager.UnitSetTarget( this, unitTarget );
						break;
					}					
				}

				return true;		
			}
		}
		else
			return true;
			
	}

	//set player animation - use present state
	protected virtual void ActiveAnimator( AnimatorState present )
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
				if (info.IsMelee)
					animator.SetInteger( "State", (int) AnimatorState.Attack );
				else
					animator.SetInteger( "State", (int) AnimatorState.ThrowAttack );
				break;
			case AnimatorState.Casting:
				animator.SetInteger( "State", (int) AnimatorState.Casting );
				break;
			case AnimatorState.CastingUltimate:
				animator.SetInteger( "State", (int) AnimatorState.Casting );
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
		if (presentState != State.Die)
		{
			presentState = State.Idle;
			unitTarget = null;
			moveAgent.ResetPath();
			ActiveAnimator( AnimatorState.Idle );
			//send unit data
			manager.UnitStop( this );
		}
	}

	//set hold state
	public void SetHold()
	{
		if (presentState != State.Die)
		{
			presentState = State.Hold;
			unitTarget = null;
			moveAgent.ResetPath();
			ActiveAnimator( AnimatorState.Idle );
			//send unit hold
			manager.UnitStop( this );
		}
	}

	//set attack state & set attack unitTarget
	public virtual void SetAttackTarget( GameObject target )
	{
		if (presentState != State.Die)
		{
			destination = transform.position;
			unitTarget = target;
			presentState = State.Attack;
			//send unit set target
			manager.UnitSetTarget( this, target );
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
			//send unit set destination
			manager.UnitSetDestination( this, destination );
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
			// send unit set destination
			manager.UnitSetDestination( this, destination );
		}
	}

	//unit use active skill -> non unitTarget
	public void ActiveSkill( int index )
	{		
		if (presentState != State.Die && info.PresentManaPoint >= info.ActiveSkillSet[index].SkillCost && !info.OnSkill[index])
		{
			info.ActiveSkillUse( index, null, transform.position );
			moveAgent.ResetPath();
			animator.Play( "Casting" );
			//send unit castskill
			manager.UnitCastSkillActiveNonTarget( this, index );
		}
	}

	//unit use active skill -> unitTarget skill
	public void ActiveSkill( int index, GameObject _unitTarget )
	{
		if (presentState != State.Die && info.PresentManaPoint >= info.ActiveSkillSet[index].SkillCost && !info.OnSkill[index])
		{
			presentState = State.CastunitTargetSkill;
			unitTarget = _unitTarget;
			presentSkillIndex = index;
		}
	}

	//unit use active skill -> range Target skill
	public void ActiveSkill( int index, Vector3 point )
	{
		if (presentState != State.Die && info.PresentManaPoint >= info.ActiveSkillSet[index].SkillCost && !info.OnSkill[index])
		{
			presentState = State.CastunitTargetSkill;
			unitTarget = null;
			destination = point;
			presentSkillIndex = index;
		}
	}

	//use throw attack
	public void ThrowAttackObject()
	{		
		GameObject temp = (GameObject) Instantiate( throwObject, transform.position + new Vector3 ( -0.2f, 1.5f, 0.2f ), transform.rotation );
		temp.GetComponent<ThrowObject>().SetTargetAndSpeed( this, unitTarget );
	}

	//reset animator information
	public void ReloadAnimatorInfo()
	{
		animatorInfo = animator.GetCurrentAnimatorStateInfo( 0 );
	}

	//unit damage calculate
	public virtual void Damaged( float damage )
	{
		info.PresentHealthPoint -= damage;
		//send unit damaged
		manager.UnitDamaged( this, damage );
	}

	// unit damage calculate + don't send others
	public void SelfDamaged( float damage )
	{
		info.PresentHealthPoint -= damage;
	}

	//use attack animation -> animation event
	public void EndAttackAnimation()
	{
		
	}

	//use casting animation -> animation event
	public void EndCastingAnimation()
	{
		presentState = State.Idle;
		destination = transform.position;
	}

	public bool AddSkill( Skill data )
	{
		if (info.AddSkill( data ))
		{
			//send packet
			return true;
		}
		else
			return false;
	}

	public void SetUp( UnitInformation newInfo, Vector3 vec )
	{
		info = new UnitInformation ( newInfo );        
		transform.position = vec;
	}

	public void SetUp( UnitInformation newInfo )
	{
		info = new UnitInformation ( newInfo );
	}
}