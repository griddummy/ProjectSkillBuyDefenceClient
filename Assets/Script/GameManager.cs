﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;

//class - policy control
public class GameManager : MonoBehaviour
{
    public const int AIPlayerNum = 5;
    public enum State { Waiting, Playing, End } // 로딩 대기, 게임 중, 게임 끝

    [SerializeField] int playerNumber;
    [SerializeField] bool[] checkAlly;
    [SerializeField] bool isAI;
    [SerializeField] int currentStageNum;
    [SerializeField] bool stageIsOver = true;
    [SerializeField] int monsterNum;

    State gameState;                    // 현재 게임 상태
    MainManager mainManager;            // 메인메니져, 로그인정보, 방정보를 얻을 수 있음
    NetManager netManager;              // 네트워크 메니져

    RoomInfo curRoomInfo;

    UnitManager unitManager;
    Database dataBase;

    StageManager stageManager;

    UIControl uiControl;
    GameObject[] playerSpawnPoint;
    GameObject[] monsterSpawnPoint;
    GameObject[] monsterChecker;
    List<SpawnMonster> monsters;

    // Load 관련
    int LoadCompleteCount; // 몇명이나 로딩이 끝났는지

    //property
    public int PlayerNumber { get { return playerNumber; } }

    public bool[] CheckAlly { get { return checkAlly; } }

    void Awake()
    {
        //메인 매니저, 넷 매니저, 게임매니저 가져오기
        mainManager = MainManager.instance;
        netManager = mainManager.netManager;
        curRoomInfo = mainManager.currentRoomInfo;
        playerNumber = curRoomInfo.myNumber;
        dataBase = Database.Instance;
        unitManager = new UnitManager();
        stageManager = new StageManager();
        playerSpawnPoint = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        monsterSpawnPoint = GameObject.FindGameObjectsWithTag("MonsterSpawnPoint");
        currentStageNum = 1;

        // 초기 게임 상태 : 대기
        gameState = State.Waiting;

        // 리시버 등록
        netManager.RegisterReceiveNotificationP2P((int)P2PPacketType.LoadComplete, OnReceiveLoadComplete);
        netManager.RegisterReceiveNotificationP2P((int)P2PPacketType.StartGame, OnReceiveStartGame);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.CreateUnit, OnReceiveCreateUnit);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitCastSkill, OnReceiveUnitCastSkill);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitDamaged, OnReceiveUnitDamaged);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitDeath, OnReceiveUnitDeath);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitLevelUp, OnReceiveUnitLevelUp);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitSetTarget, OnReceiveUnitSetTarget);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitSetDestination, OnReceiveUnitSetDetination);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitImmediatelyMove, OnReceiveUnitImmediatelyMove);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitStop, OnReceiveUnitStop);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitAttack, OnReceiveUnitAttack);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitInterpolation, OnReceiveUnitInterpolation);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitAddSKill, OnReceiveUniAddSkill);
    }

    void OnDestroy()
    {
        // 리시버 해제
        netManager.UnRegisterReceiveNotificationP2P((int)P2PPacketType.LoadComplete);
        netManager.UnRegisterReceiveNotificationP2P((int)P2PPacketType.StartGame);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.CreateUnit);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitCastSkill);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitDamaged);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitDeath);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitLevelUp);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitSetTarget);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitSetDestination);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitImmediatelyMove);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitStop);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitAttack);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitInterpolation);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitAddSKill);
    }

    //initialize this script
    void Start()
    {
        StartCoroutine(GameLoading()); // 로딩 + 로딩완료검사
    }

    void InitializeData()
    {
        // 동맹 배열
        checkAlly = new bool[8];
        //checkAlly[playerNumber-1] = true; // 자기 자신은 true;

        // 플레이어는 전부 동맹
        List<PlayerInfo> listPlayer = curRoomInfo.GetAllGuestInfo();
        for (int i = 0; i < listPlayer.Count; i++)
        {
            checkAlly[listPlayer[i].number - 1] = true;
        }

        // 자신의 케릭터 생성하기
        InGameCreateUnitData data = new InGameCreateUnitData();

        data.level = 1;
        data.identity.unitOwner = (byte)playerNumber;
        data.unitType = 1;
        data.identity.unitId = 1;
        //TODO
        // 자신의 스타팅 포인트에 케릭터를 위치시킴. 해야되지만 테스트용으로 땜빵함.
        data.position = new Vector3(playerNumber * 2f, 0f, 0f);

        UnitCreate(data); //  유닛 생성
    }

    IEnumerator GameLoading() // 로딩 + 로딩완료 검사 루틴 - 호스트만 실행
    {
        yield return null; // 한 프레임 기다리기        

        InitializeData(); // 초기화

        // 자신이 호스트라면
        if (curRoomInfo.isHost)
        {
            LoadCompleteCount++;

            while (LoadCompleteCount < curRoomInfo.PlayerCount)
            {
                yield return null;
            }

            // 모두 로딩 완료

            // 게임 시작 메세지 전송
            P2PStartGamePacket packet = new P2PStartGamePacket();
            netManager.SendToAllGuest(packet);
            // 게임 상태 변경 
            gameState = State.Playing; // 게임중
        }
        else // 게스트라면 호스트에게 로딩 완료 메세지 전송
        {
            P2PLoadCompleteData loadCompleteData = new P2PLoadCompleteData();
            loadCompleteData.playerNumber = (byte)playerNumber;
            P2PLoadCompletePacket loadCompletePacket = new P2PLoadCompletePacket(loadCompleteData);
            netManager.SendToHost(loadCompletePacket);
        }

        // 유닛의 이동 동기
        if(curRoomInfo.PlayerCount > 1)
        {
            StartCoroutine(SyncMoving());
        }        
    }

    IEnumerator SyncMoving()
    {
        // 이동 동기화
        while (true)
        {
            SendSyncMove(playerNumber);
            if (curRoomInfo.isHost)
            {
                SendSyncMove(AIPlayerNum);
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void SendSyncMove(int number)
    {
        LinkedList<GameObject> listUnit = unitManager.GetUnitList(number);
        InGameUnitInterpolationData data = new InGameUnitInterpolationData();
        LinkedListNode<GameObject> node = listUnit.First;
        while(node != null)
        {
            var next = node.Next;
            if (node.Value == null)
            {
                listUnit.Remove(node);
            }
            else
            {
                data.identity.unitOwner = (byte)number;
                data.identity.unitId = (byte)node.Value.GetComponent<UnitProcess>().Info.UnitID;
                data.forward = node.Value.transform.forward;
                data.position = node.Value.transform.position;

                // 패킷 만들어서 보내기
                InGameUnitInterpolationPacket packet = new InGameUnitInterpolationPacket(data);
                if (curRoomInfo.isHost)
                {
                    netManager.SendToAllGuest(packet);
                }
                else
                {
                    netManager.SendToHost(packet);
                }
            }
            node = next;
        }
    }
    // 자신의 유닛(호스트일 경우 AI 유닛 포함)을 생성하고 메세지를 전송한다.
    public GameObject UnitCreate(InGameCreateUnitData createUnitData)
    {
        // 유닛 생성이 가능한지 물어봄
        int unitId = unitManager.FindEmptySlot(createUnitData.identity.unitOwner);

        if (unitId < 0)
        {
            return null; // Full unit
        }

        createUnitData.identity.unitId = (byte)unitId;
        string unitName = "Prefab/" + dataBase.GetUnitData(createUnitData.unitType).unitName;
        GameObject unitObj = Instantiate(Resources.Load<GameObject>(unitName), createUnitData.position, Quaternion.identity) as GameObject;

        // 유닛 타입 정보 얻기
        UnitData unitData = dataBase.GetUnitData(createUnitData.unitType);
        UnitLevelData unitLevelData = dataBase.GetUnitLevelData(createUnitData.unitType, createUnitData.level);

        
        
        // 유닛 정보 초기화, 자신의 유닛은 UnitProcess를 붙인다.
        unitObj.AddComponent<UnitProcess>().SetUp(new UnitInformation(createUnitData, unitData, unitLevelData), createUnitData.position);

        // 유닛 매니져에 유닛 등록
        if (!unitManager.InsertSlot(unitObj, unitId))
        {
            Destroy(unitObj); // 실패시 삭제
            Debug.Log("UnitCreate 실패 : 플레이어" + createUnitData.identity.unitOwner + " " + unitName);
            return null;
        }
        Debug.Log("UnitCreate 성공 : 플레이어" + createUnitData.identity.unitOwner + " " + unitName);
        // 생성 결과를 다른 사람들에게 알림.
        InGameCreateUnitPacket packet = new InGameCreateUnitPacket(createUnitData);
        SendChangedData(packet);
        return unitObj;
    }

    public void UnitSetDestination(UnitProcess unit, Vector3 destination) // 목표지점 설정
    {
        /*
        if (playerNumber != unit.Info.PlayerNumber) // 자기유닛이 아니고
        {
            if (curRoomInfo.isHost) // 호스트라면
            {
                if (unit.Info.PlayerNumber <= RoomInfo.MaxPlayer) // ai 유닛이 아니라면?
                {
                    return;
                }
            }
            else // 게스트면
            {
                return;
            }
        }*/
        
        InGameUnitSetDestinationData data = new InGameUnitSetDestinationData();
        data.destination = destination;
        data.currentPosition = unit.transform.position;
        data.forward = unit.transform.forward;        
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        InGameUnitSetDestinationPacket packet = new InGameUnitSetDestinationPacket(data);
        SendChangedData(packet);
        Debug.Log("어택땅 전송 : player" + data.identity.unitOwner + " id" + unit.Info.UnitID);
    }

    public void UnitSetTarget(UnitProcess sourceUnit, GameObject targetUnit) // 목표 설정
    {
        InGameUnitSetTargetData data = new InGameUnitSetTargetData();
        data.identitySource.unitId = (byte)sourceUnit.Info.UnitID;
        data.identitySource.unitOwner = (byte)sourceUnit.Info.PlayerNumber;
        data.currentPosition = sourceUnit.transform.position;
        data.forward = sourceUnit.transform.forward;        
        
        UnitProcess target = targetUnit.GetComponent<UnitProcess>();
        if(target == null)
        {
            target = targetUnit.GetComponent<UnitPlayer>();            
        }
        if(target != null)
        {
            data.identityTarget.unitId = (byte)target.Info.UnitID;
            data.identityTarget.unitOwner = (byte)target.Info.PlayerNumber;
            InGameUnitSetTargetPacket packet = new InGameUnitSetTargetPacket(data);
            SendChangedData(packet);
            Debug.Log("목표설정 전송 : player" + sourceUnit.Info.PlayerNumber + " id" + sourceUnit.Info.UnitID);
        }
    }

    public void UnitImmediateMove(UnitProcess unit, Vector3 position) // 해당지점으로 즉시 이동
    {
        InGameUnitImmediatelyMoveData data = new InGameUnitImmediatelyMoveData();
        data.destination = position;
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        data.forward = unit.transform.forward;        
        InGameUnitImmediatelyMovePacket packet = new InGameUnitImmediatelyMovePacket(data);
        SendChangedData(packet);
        Debug.Log("즉시이동 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID);
    }

    public void UnitAttack(UnitProcess sourceUnit, GameObject targetUnit) // 자신의 유닛 공격
    {
        InGameUnitAttackData data = new InGameUnitAttackData();
        data.identitySource.unitId = (byte)sourceUnit.Info.UnitID;
        data.identitySource.unitOwner = (byte)sourceUnit.Info.PlayerNumber;
        data.currentPosition = sourceUnit.transform.position;
        data.forward = sourceUnit.transform.forward;
        
        UnitProcess target = targetUnit.GetComponent<UnitProcess>();
        if(target == null)
        {
            target = targetUnit.GetComponent<UnitPlayer>();
        }
        if(target != null)
        {
            data.identityTarget.unitId = (byte)target.Info.UnitID;
            data.identityTarget.unitOwner = (byte)target.Info.PlayerNumber;
            InGameUnitAttackPacket packet = new InGameUnitAttackPacket(data);
            SendChangedData(packet);
            //Debug.Log("공격 전송 : player" + sourceUnit.Info.PlayerNumber + " id" + sourceUnit.Info.UnitID);
        }
    }

    public void UnitAddSkill(UnitProcess unit, int skillid)
    {
        UnitPlayer player = unit as UnitPlayer;
        if(player == null)
        {
            Debug.Log("UnitAddSkill:: " + skillid);
            InGameUnitAddSkillData data = new InGameUnitAddSkillData();
            data.identity.unitId = (byte)unit.Info.UnitID;
            data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
            data.skillid = (byte)skillid;
            InGameUnitAddSkillPacket packet = new InGameUnitAddSkillPacket(data);
            SendChangedData(packet);
        }
    }

    public void UnitCastSkillActiveNonTarget(UnitProcess unit, int skillIndex)    // 스킬 시전
    {
        if(unit.Info.PlayerNumber != playerNumber)
        {
            if(!( curRoomInfo.isHost && (unit.Info.PlayerNumber == AIPlayerNum)) )
            {
                return;
            }
        }
        InGameUnitCastSkillData data = new InGameUnitCastSkillData();
        data.currentPosition = unit.transform.position;
        data.forward = unit.transform.forward;
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        data.type = Skill.Type.ActiveNonTarget;
        InGameUnitCastSkillPacket packet = new InGameUnitCastSkillPacket(data);
        SendChangedData(packet);
        Debug.Log("스킬 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID + " skill"+ skillIndex);
    }
    public void UnitCastSkillTargetArea(UnitProcess unit, int skillIndex, Vector3 targetPosition)    // 스킬 시전
    {
        InGameUnitCastSkillData data = new InGameUnitCastSkillData();
        data.currentPosition = unit.transform.position;
        data.forward = unit.transform.forward;
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        data.type = Skill.Type.ActiveTargetArea;
        data.destination = targetPosition;
        InGameUnitCastSkillPacket packet = new InGameUnitCastSkillPacket(data);
        SendChangedData(packet);
        Debug.Log("스킬 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID + " skill" + skillIndex);
    }
    public void UnitCastSkillActiveTarget(UnitProcess unit, int skillIndex, GameObject targetUnit)    // 스킬 시전
    {
        InGameUnitCastSkillData data = new InGameUnitCastSkillData();
        data.currentPosition = unit.transform.position;
        data.forward = unit.transform.forward;
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        data.type = Skill.Type.ActiveTarget;

        UnitProcess target = targetUnit.GetComponent<UnitProcess>();
        if (target == null)
        {
            target = targetUnit.GetComponent<UnitPlayer>();
        }
        if (target != null)
        {
            data.identityTarget.unitId = (byte)target.Info.UnitID;
            data.identityTarget.unitOwner = (byte)target.Info.PlayerNumber;
            InGameUnitCastSkillPacket packet = new InGameUnitCastSkillPacket(data);
            SendChangedData(packet);
            Debug.Log("스킬 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID + " skill" + skillIndex);
        }
    }

    public void UnitStop(UnitProcess unit) // 유닛 멈춤 또는 홀드
    {
        InGameUnitStopData data = new InGameUnitStopData();
        data.currentPosition = unit.transform.position;
        data.forward = unit.transform.forward;
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        InGameUnitStopPacket packet = new InGameUnitStopPacket(data);
        SendChangedData(packet);
        Debug.Log("스탑 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID);
    }

    public void UnitLevelUp(UnitProcess unit, int level) // 레벨업
    {
        InGameUnitLevelUpData data = new InGameUnitLevelUpData();
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        data.level = (byte)level;
        InGameUnitLevelUpPacket packet = new InGameUnitLevelUpPacket(data);
        SendChangedData(packet);
        Debug.Log("레벨업 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID);
    }

    public void UnitDamaged(UnitProcess unit, float damage) // 피해입음
    {
        InGameUnitDamagedData data = new InGameUnitDamagedData();        
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        data.damage = damage;
        InGameUnitDamagedPacket packet = new InGameUnitDamagedPacket(data);
        SendChangedData(packet);
        Debug.Log("데미지드 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID);
    }

    public void UnitDeath(UnitProcess unit) // 죽음
    {
        unitManager.DeleteSlot(unit.Info.PlayerNumber, unit.Info.UnitID);
        InGameUnitDeathData data = new InGameUnitDeathData();
        data.identity.unitId = (byte)unit.Info.UnitID;
        data.identity.unitOwner = (byte)unit.Info.PlayerNumber;
        InGameUnitDeathPacket packet = new InGameUnitDeathPacket(data);
        SendChangedData(packet);
        Debug.Log("죽음 전송 : player" + unit.Info.PlayerNumber + " id" + unit.Info.UnitID);
    }

    // 패킷을 전송할 대상을 정해 전송하는 메서드
    void SendChangedData<T>(IPacket<T> packet)
    {
        if (curRoomInfo.isHost) // 자신이 호스트라면
        {
            netManager.SendToAllGuest(packet); // 모든 게스트에게 보낸다.
        }
        else // 자신이 게스트라면
        {
            netManager.SendToHost(packet); // 호스트에게 보낸다.
        }
    }

    // 로딩 끝 패킷 수신 메서드 [ 게스트 -> 호스트 ]
    void OnReceiveLoadComplete(Socket client, byte[] data)
    {
        P2PLoadCompletePacket packet = new P2PLoadCompletePacket(data);
        Debug.Log(curRoomInfo.GetGuestInfo(packet.GetData().playerNumber).playerName + "로딩끝");
        // 로딩 완료 인원 증가
        LoadCompleteCount++;
    }

    // 게임 시작 패킷 수신 메서드 [ 호스트 -> 게스트 ]
    void OnReceiveStartGame(Socket client, byte[] data)
    {
        // TODO : 카메라 이벤트 실행
        Debug.Log("게임시작");
        gameState = State.Playing;
    }

    // 유닛 생성 수신 리시버 [ 게스트 -> 호스트 ]
    void OnReceiveCreateUnit(Socket client, byte[] data)
    {
        InGameCreateUnitPacket packet = new InGameCreateUnitPacket(data);
        InGameCreateUnitData createUnitData = packet.GetData();

        string unitName = "Prefab/" + dataBase.GetUnitData(createUnitData.unitType).unitName;
        // TODO : 데이터베이스에 프리팹의 경로가 있어야 할듯!
        GameObject unitObj = Instantiate(Resources.Load<GameObject>(unitName), createUnitData.position, Quaternion.identity) as GameObject;

        // 유닛 타입 정보 얻기
        //TODO : 데이터베이스에서 유닛 자료를 얻어야함.
        UnitData unitData = dataBase.GetUnitData(createUnitData.unitType);
        UnitLevelData unitLevelData = dataBase.GetUnitLevelData(createUnitData.unitType, createUnitData.level);

        // 유닛 정보 초기화, 다른사람의 유닛은 UnitPlayer 스크립트를 붙인다.
        unitObj.AddComponent<UnitPlayer>().SetUp(new UnitInformation(createUnitData, unitData, unitLevelData), createUnitData.position);

        // 유닛 매니져에 유닛 등록
        unitManager.InsertSlot(unitObj, createUnitData.identity.unitId);

        // 자신이 호스트라면
        if (curRoomInfo.isHost)
        {
            // 모두에게 전송, 너만빼고
            PlayerInfo player = curRoomInfo.GetGuestInfo(createUnitData.identity.unitOwner);
            netManager.SendToAllGuest(client, packet);
        }
    }
    void OnReceiveUniAddSkill(Socket client, byte[] data)
    {
        InGameUnitAddSkillPacket packet = new InGameUnitAddSkillPacket(data);
        InGameUnitAddSkillData addSkillData = packet.GetData();

        GameObject obj = unitManager.GetUnitObject(addSkillData.identity.unitOwner, addSkillData.identity.unitId);
        if(obj != null)
        {
            Debug.Log("OnReceive:: 스킬 추가"+ addSkillData.skillid);
            UnitPlayer unit = obj.GetComponent<UnitPlayer>();
            unit.AddSkill(Database.Instance.FindSkillByID(addSkillData.skillid));
        }

        if (curRoomInfo.isHost)
            netManager.SendToAllGuest(client, packet);

    }
    // 스킬 사용 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitCastSkill(Socket client, byte[] data)
    {
        InGameUnitCastSkillPacket packet = new InGameUnitCastSkillPacket(data);
        InGameUnitCastSkillData castSkillData = packet.GetData();
        GameObject obj = unitManager.GetUnitObject(castSkillData.identity.unitOwner, castSkillData.identity.unitId);
        UnitPlayer unit = obj.GetComponent<UnitPlayer>();
        unit.transform.position = castSkillData.currentPosition;
        //unit.transform.forward = castSkillData.forward;
        // TODO : 스킬 사용
        // 모션 사용
        unit.ReceiveData(unit.transform.position, UnitProcess.AnimatorState.Casting);

        // 스킬 타입 알아내야함
        if (castSkillData.type == Skill.Type.ActiveNonTarget) // 논타겟 스킬
        {
            
        }
        else if (castSkillData.type == Skill.Type.ActiveTarget) // 유닛 타겟 스킬
        {
            GameObject objTargetUnit = unitManager.GetUnitObject(castSkillData.identityTarget.unitOwner, castSkillData.identityTarget.unitId);
            UnitPlayer unitTarget = objTargetUnit.GetComponent<UnitPlayer>();
        }
        else if (castSkillData.type == Skill.Type.ActiveTargetArea) // 지역 타겟 스킬
        {

        }
       

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    // 유닛 피해 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitDamaged(Socket client, byte[] data)
    {
        InGameUnitDamagedPacket packet = new InGameUnitDamagedPacket(data);
        InGameUnitDamagedData damagedData = packet.GetData();

        GameObject obj = unitManager.GetUnitObject(damagedData.identity.unitOwner, damagedData.identity.unitId);
        
        if(obj != null)
        {
            // TODO : 유닛 피해입음    
            if (damagedData.identity.unitOwner == playerNumber)
            {
                // 자기꺼라면
                obj.GetComponent<UnitProcess>().SelfDamaged(damagedData.damage);
            }
            else
            {
                // 다른사람
                UnitPlayer unit = obj.GetComponent<UnitPlayer>();
                if (unit == null)
                {
                    obj.GetComponent<UnitProcess>().SelfDamaged(damagedData.damage);
                }
                else
                {
                    unit.SelfDamaged(damagedData.damage);
                }
            }
        }

        if (curRoomInfo.isHost)
            netManager.SendToAllGuest(client, packet);
    }

    // 유닛 죽음 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitDeath(Socket client, byte[] data)
    {
        InGameUnitDeathPacket packet = new InGameUnitDeathPacket(data);
        InGameUnitDeathData deathData = packet.GetData();        
        try
        {
            GameObject obj = unitManager.GetUnitObject(deathData.identity.unitOwner, deathData.identity.unitId);
            UnitPlayer unit = obj.GetComponent<UnitPlayer>();

            //TODO : 유닛 죽음
            unit.ReceiveData(unit.transform.position, UnitProcess.AnimatorState.Die);
            unitManager.DeleteSlot(unit.Info.PlayerNumber, unit.Info.UnitID);
            Destroy(unit.gameObject, 2f);
        }
        catch
        {
            Debug.Log("OnReceive::유닛죽음..실패" + deathData.identity.unitOwner + " " + deathData.identity.unitId);
        }
        
        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    // 유닛 레벨업 리시버[게스트->호스트, 호스트->모든게스트]
    void OnReceiveUnitLevelUp(Socket client, byte[] data)
    {
        InGameUnitLevelUpPacket packet = new InGameUnitLevelUpPacket(data);
        InGameUnitLevelUpData levelUpData = packet.GetData();

        GameObject obj = unitManager.GetUnitObject(levelUpData.identity.unitOwner, levelUpData.identity.unitId);
        UnitPlayer unit = obj.GetComponent<UnitPlayer>();
        // TODO : 레벨업

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    void OnReceiveUnitSetTarget(Socket client, byte[] data)
    {
        InGameUnitSetTargetPacket packet = new InGameUnitSetTargetPacket(data);
        InGameUnitSetTargetData targetData = packet.GetData();

        // TODO 타겟 설정...
        GameObject obj = unitManager.GetUnitObject(targetData.identitySource.unitOwner, targetData.identitySource.unitId);
        UnitPlayer unit = obj.GetComponent<UnitPlayer>();

        GameObject objTarget = unitManager.GetUnitObject(targetData.identityTarget.unitOwner, targetData.identityTarget.unitId);
        unit.SetAttackTarget(objTarget);
        Debug.Log("OnReceive::목표 유닛 설정, " + unit.Info.PlayerNumber + " " + unit.Info.Name);
    }

    // 유닛 이동 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitSetDetination(Socket client, byte[] data)
    {
        InGameUnitSetDestinationPacket packet = new InGameUnitSetDestinationPacket(data);
        InGameUnitSetDestinationData moveData = packet.GetData();
        GameObject obj = unitManager.GetUnitObject(moveData.identity.unitOwner, moveData.identity.unitId);
        UnitPlayer unit = obj.GetComponent<UnitPlayer>();
        unit.transform.position = moveData.currentPosition;
        //unit.transform.forward = moveData.forward;
        //TODO  :  유닛 이동시키기
        Debug.Log("OnReceive::유닛 이동, " + unit.Info.PlayerNumber + " " + unit.Info.Name);
        unit.ReceiveData(moveData.destination, UnitProcess.AnimatorState.Run);

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    // 유닛 즉시 이동 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitImmediatelyMove(Socket client, byte[] data)
    {
        InGameUnitImmediatelyMovePacket packet = new InGameUnitImmediatelyMovePacket(data);
        InGameUnitImmediatelyMoveData moveData = packet.GetData();

        GameObject obj = unitManager.GetUnitObject(moveData.identity.unitOwner, moveData.identity.unitId);
        UnitPlayer unit = obj.GetComponent<UnitPlayer>();
        //unit.transform.forward = moveData.forward;
        //TODO : 즉시 이동
        unit.transform.position = moveData.destination; // 임시 ..

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    // 유닛 멈춤 리시버[게스트->호스트, 호스트->모든게스트]
    void OnReceiveUnitStop(Socket client, byte[] data)
    {
        InGameUnitStopPacket packet = new InGameUnitStopPacket(data);
        InGameUnitStopData stopData = packet.GetData();

        GameObject obj = unitManager.GetUnitObject(stopData.identity.unitOwner, stopData.identity.unitId);
        UnitPlayer unit = obj.GetComponent<UnitPlayer>();

        unit.transform.position = stopData.currentPosition;
        //unit.transform.forward = stopData.forward;
        //TODO : 유닛 멈춤
        Debug.Log("OnReceive::유닛 멈춤, " + unit.Info.PlayerNumber + " " + unit.Info.Name);
        unit.ReceiveData(unit.transform.position, UnitProcess.AnimatorState.Idle);

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    // 유닛 공격 [게스트 -> 호스트, 호스트 -> 모든게스트]
    void OnReceiveUnitAttack(Socket client, byte[] data)
    {
        InGameUnitAttackPacket packet = new InGameUnitAttackPacket(data);
        InGameUnitAttackData stopData = packet.GetData();

        GameObject objSourceUnit = unitManager.GetUnitObject(stopData.identitySource.unitOwner, stopData.identitySource.unitId);
        UnitPlayer unitSource = objSourceUnit.GetComponent<UnitPlayer>();

        GameObject objTargetUnit = unitManager.GetUnitObject(stopData.identityTarget.unitOwner, stopData.identityTarget.unitId);
        if(objTargetUnit != null)
        {
            if (stopData.identityTarget.unitOwner == playerNumber) // 목표가 내 유닛이면
            {
                UnitProcess unitMine = objTargetUnit.GetComponent<UnitPlayer>();

            }
            else // 목표가 다른사람 유닛 이면
            {
                UnitPlayer unitTarget = objTargetUnit.GetComponent<UnitPlayer>();
            }

            unitSource.transform.position = stopData.currentPosition;
            //unitSource.transform.forward = stopData.forward;

            //TODO : 목표유닛 공격
            unitSource.ReceiveData(unitSource.transform.position, UnitProcess.AnimatorState.Attack);
        }      

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    void OnReceiveUnitInterpolation(Socket client, byte[] data)
    {
        InGameUnitInterpolationPacket packet = new InGameUnitInterpolationPacket(data);
        InGameUnitInterpolationData posData = packet.GetData();

        GameObject obj = unitManager.GetUnitObject(posData.identity.unitOwner, posData.identity.unitId);

        if(obj != null)
        {
            obj.transform.position = posData.position;
            obj.transform.forward = posData.forward;
        }
        

        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(client, packet);
        }
    }

    //스테이지 시작
    public void StageStart()
    {
        if (stageIsOver)
        {
            stageIsOver = false;
            monsters = stageManager.GetStageData(currentStageNum).Monsters;
            monsterNum = 0;

            for (int i = 0; i < monsters.Count; i++)
            {
                monsterNum += stageManager.GetStageData(currentStageNum).Monsters[i].number;
            }

            monsterChecker = new GameObject[curRoomInfo.PlayerCount * monsterNum];
            monsterNum = 0;
            int spawnPointCount = monsterSpawnPoint.Length;
            for (int i = 0; i < spawnPointCount; i++)
            {
                for (int j = 0; j < monsters.Count; j++)
                {
                    for (int k = 0; k < monsters[j].number; k++)
                    {
                        InGameCreateUnitData data = new InGameCreateUnitData();

                        data.level = 1;
                        data.identity.unitOwner = AIPlayerNum;
                        data.unitType = (byte) monsters[j].Id;
                        data.position = monsterSpawnPoint[i].transform.position + Vector3.forward * j * 3 + Vector3.right * k * 3;
                        
                        monsterChecker[monsterNum++] = UnitCreate(data);
                    }
                }
            }
            StartCoroutine(CheckStageOver());
        }
    }

    //몬스터가 다 죽었는지 체크
    IEnumerator CheckStageOver()
    {
        bool stop = false;
        
        while (!stop)
        {
            yield return new WaitForSeconds(0.5f);            

            for (int i = 0; i < monsterNum; i++)
            {
                if (monsterChecker[i] != null)
                {
                    stageIsOver = false;
                    yield return null;
                }
                else
                {
                    stageIsOver = true;
                }
            }

            if (stageIsOver)
            {
                int rewardGold = stageManager.GetStageData(currentStageNum).RewardGold;
                GameObject.FindGameObjectWithTag("MainUI").GetComponent<UIControl>().Gold += rewardGold;
                currentStageNum++;
                stageIsOver = true;
                stop = true;
            }
        }
    }
}