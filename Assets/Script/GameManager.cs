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
    }

    void OnDestroy()
    {
        // 리시버 해제
        netManager.UnRegisterReceiveNotificationP2P((int)P2PPacketType.LoadComplete);
        netManager.UnRegisterReceiveNotificationP2P((int)P2PPacketType.StartGame);
        netManager.UnRegisterReceiveNotificationP2P((int)InGamePacketID.CreateUnit);
    }

	//initialize this script
	void Start()
	{
        if (curRoomInfo.isHost)
        {
            StartCoroutine(CheckAllLoadComplete()); // 로딩완료 검사 코루틴 시작
        }
            
        InitializeData(); // 초기화
        
        // 자신이 호스트라면
        if (curRoomInfo.isHost)
        {
            LoadCompleteCount++;
        }
        else // 게스트라면 호스트에게 로딩 완료 메세지 전송
        {            
            P2PLoadCompleteData data = new P2PLoadCompleteData();
            data.playerNumber = (byte)playerNumber;
            P2PLoadCompletePacket packet = new P2PLoadCompletePacket(data);
            netManager.SendToHost(packet);
        }
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
        data.posX = playerNumber*2f;
        data.posY = 1;
        data.posZ = 1;
        data.unitId = 1;
        data.unitType = 1;
        data.unitOwner = (byte)playerNumber;
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

    // 게임 시작 패킷 수신 메서드 [호스트->게스트]
    void OnReceiveStartGame(Socket client, byte[] data)
    {
        // TODO
        // 카메라 이벤트 실행
        Debug.Log("게임시작");
        gameState = State.Playing;
    }

    // 로딩 끝 패킷 수신 메서드 [ 게스트 -> 호스트 ]
    void OnReceiveLoadComplete(Socket client, byte[] data)
    {
        P2PLoadCompletePacket packet = new P2PLoadCompletePacket(data);
        Debug.Log(curRoomInfo.GetGuestInfo(packet.GetData().playerNumber).playerName + "로딩끝");
        // 로딩 완료 인원 증가
        LoadCompleteCount++;
    }

    IEnumerator CheckAllLoadComplete() //  로딩완료 검사 루틴 - 호스트만 실행
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

    GameObject CreateUnit(InGameCreateUnitData createData)
    {
        try
        {
            GameObject unitObj = Instantiate(Resources.Load<GameObject>(getResourcePath(createData.unitType)));
            UnitData unitData = unitDatabase.unitData[(int)createData.unitType];
            UnitLevelData unitLevelData = unitData.levelData[(int)createData.level];
            Vector3 position = new Vector3(createData.posX, createData.posY, createData.posZ);
            unitObj.GetComponent<UnitProcess>().SetUp(new UnitInformation(createData.unitId, createData.unitOwner, unitData, unitLevelData), position);

            return unitObj;
        }
        catch
        {
            return null;
        }
    }

    void UnitMove (InGameUnitMoveData unitMoveData) // 유닛 이동
    {
            GameObject unitObj = unitManager.unitData[unitMoveData.unitOwner, unitMoveData.unitId];
            Vector3 destination = new Vector3(unitMoveData.posX, unitMoveData.posY, unitMoveData.posZ);        
            unitObj.GetComponent<UnitProcess>().Destination = destination;
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

    void OnReceiveCreateUnit(Socket client, byte[] data)
    {        
        InGameCreateUnitPacket packet = new InGameCreateUnitPacket(data);
        InGameCreateUnitData createData = packet.GetData();
        
        if (curRoomInfo.isHost) // 호스트면 다른 게스트에게 재 전송
        {
            netManager.SendToAllGuest(packet);
        }
        unitManager.InsertSlot(CreateUnit(createData));
    }

    string getResourcePath(int id)
    {
        // TODO 
        //ID에 따른 리소스 경로 얻기
        return "NoBladeGirl";
    }
}
