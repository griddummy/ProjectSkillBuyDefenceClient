using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;

//class - policy control
public class GameManager : MonoBehaviour
{
    public enum State { Waiting, Playing, End } // 로딩 대기, 게임 중, 게임 끝

	[SerializeField] int playerNumber;
	[SerializeField] bool[] checkAlly;
	[SerializeField] bool isAI;
    State gameState;                    // 현재 게임 상태
    MainManager mainManager;
    NetManager netManager;
    RoomInfo curRoomInfo;
    UnitDatabase unitDatabase;
    UnitManager unitManager;

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
        unitDatabase = new UnitDatabase("UnitData.data", FileMode.Open);
        unitManager = new UnitManager();

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
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitMove, OnReceiveUnitMove);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitMove, OnReceiveUnitImmediatelyMove);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitStop, OnReceiveUnitStop);
        netManager.RegisterReceiveNotificationP2P((int)InGamePacketID.UnitAttack, OnReceiveUnitAttack);
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
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitMove);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitimmediatelyMove);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitStop);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.UnitAttack);
    }

    //initialize this script
    void Start()
	{
        
         StartCoroutine(DelayLoad()); // 로딩완료 검사 코루틴 시작
                    
        
    }

    void Update()
    {
        if(gameState == State.Playing)
        {
            
        }
    }

    void InitializeData() 
	{
		checkAlly = new bool[8];
		checkAlly[playerNumber-1] = true;
        InGameCreateUnitData data = new InGameCreateUnitData();
        data.level = 1;
        data.identity.unitOwner = (byte)playerNumber;
        data.unitType = 1;
        data.identity.unitId = 1;
        data.position.x = playerNumber*2f;
        data.position.y = 1;
        data.position.z = 1;
        
        SendAndCreateUnit(data);
    }

	//start game -> game load process
	public void LoadProcess()
	{

	}

	//send unit data -> changed unit data
	public void SendUnitData()
	{

	}

	//receive unit data -> changed unit data
	public void ReceiveUnitData()
	{

	}
    
    IEnumerator DelayLoad() // 로딩 + 로딩완료 검사 루틴 - 호스트만 실행
    {
        yield return null;

        InitializeData(); // 초기화

        // 자신이 호스트라면
        if (curRoomInfo.isHost)
        {
            LoadCompleteCount++;
        }
        else // 게스트라면 호스트에게 로딩 완료 메세지 전송
        {
            P2PLoadCompleteData loadCompleteData = new P2PLoadCompleteData();
            loadCompleteData.playerNumber = (byte)playerNumber;
            P2PLoadCompletePacket loadCompletePacket = new P2PLoadCompletePacket(loadCompleteData);
            netManager.SendToHost(loadCompletePacket);
        }
        if (curRoomInfo.isHost)
        {
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
    }

    GameObject CreateUnit(InGameCreateUnitData createData)
    {
        try
        {
            GameObject unitObj = Instantiate(Resources.Load<GameObject>(getResourcePath(createData.unitType)));
            UnitData unitData = unitDatabase.unitData[(int)createData.unitType];
            UnitLevelData unitLevelData = unitData.levelData[(int)createData.level];
            unitObj.GetComponent<UnitProcess>().SetUp(new UnitInformation(createData, unitData, unitLevelData), createData.position);

            return unitObj;
        }
        catch
        {
            return null;
        }
    }

    void UnitMove (InGameUnitMoveData unitMoveData) // 유닛 이동
    {
            GameObject unitObj = unitManager.unitData[unitMoveData.identity.unitOwner, unitMoveData.identity.unitId];
            unitObj.GetComponent<UnitProcess>().SetDestination(unitMoveData.destination);
    }

    void SendAndCreateUnit(InGameCreateUnitData createData) // 자신이 유닛이 생성할 때 부르는 메서드
    {
        InGameCreateUnitPacket packet = new InGameCreateUnitPacket(createData);
        if (curRoomInfo.isHost)
        {
            netManager.SendToAllGuest(packet);
        }
        else
        {
            netManager.SendToHost(packet);
        }

        unitManager.InsertSlot(CreateUnit(createData));
    }

   

    string getResourcePath(int id)
    {
        // TODO 
        //ID에 따른 리소스 경로 얻기
        return "ProtoType1";
    }

    // 유닛 생성 수신 리시버 [ 게스트 -> 호스트 ]
    void OnReceiveCreateUnit(Socket client, byte[] data)
    {
        InGameCreateUnitPacket packet = new InGameCreateUnitPacket(data);
        InGameCreateUnitData createData = packet.GetData();

        if (curRoomInfo.isHost) // 호스트면 다른 게스트에게 재 전송
        {
            netManager.SendToAllGuest(client, packet); // 방금 보낸 게스트를 제외하고 전송
        }
        unitManager.InsertSlot(CreateUnit(createData));
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
        // TODO
        // 카메라 이벤트 실행
        Debug.Log("게임시작");
        gameState = State.Playing;
    }    
    
    // 스킬 사용 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitCastSkill(Socket client, byte[] data)
    {
        InGameUnitCastSkillPacket packet = new InGameUnitCastSkillPacket(data);
        InGameUnitCastSkillData castSkillData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 피해 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitDamaged(Socket client, byte[] data)
    {
        InGameUnitDamagedPacket packet = new InGameUnitDamagedPacket(data);
        InGameUnitDamagedData damagedData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 죽음 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitDeath(Socket client, byte[] data)
    {
        InGameUnitDeathPacket packet = new InGameUnitDeathPacket(data);
        InGameUnitDeathData deathData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 레벨업 리시버[게스트->호스트, 호스트->모든게스트]
    void OnReceiveUnitLevelUp(Socket client, byte[] data)
    {
        InGameUnitLevelUpPacket packet = new InGameUnitLevelUpPacket(data);
        InGameUnitLevelUpData levelUpData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 이동 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitMove(Socket client, byte[] data)
    {
        InGameUnitMovePacket packet = new InGameUnitMovePacket(data);
        InGameUnitMoveData moveData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 즉시 이동 리시버 [ 게스트 -> 호스트, 호스트 -> 모든게스트 ]
    void OnReceiveUnitImmediatelyMove(Socket client, byte[] data)
    {
        InGameUnitImmediatelyMovePacket packet = new InGameUnitImmediatelyMovePacket(data);
        InGameUnitImmediatlyMoveData moveData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 멈춤 리시버[게스트->호스트, 호스트->모든게스트]
    void OnReceiveUnitStop(Socket client, byte[] data)
    {
        InGameUnitStopPacket packet = new InGameUnitStopPacket(data);
        InGameUnitStopData stopData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }

    // 유닛 공격 [게스트 -> 호스트, 호스트 -> 모든게스트]
    void OnReceiveUnitAttack(Socket client, byte[] data)
    {
        InGameUnitAttackPacket packet = new InGameUnitAttackPacket(data);
        InGameUnitAttackData stopData = packet.GetData();
        netManager.SendToAllGuest(client, packet);
    }
}
