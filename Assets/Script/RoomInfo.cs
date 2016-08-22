using UnityEngine;
using System.Collections.Generic;

// 호스트가 방을 만들면 생기는 방정보
public class RoomInfo
{
    public enum State { Wait, Play }        // 빈방, 대기, 게임중
    public enum PlayerType{ Host, Guest }   // 플레이어의 타입 [ 게스트, 호스트 ]
    public const int MaxPlayer = 4;         // 최대 플레이어
    public const int HostNumber = 1;

    public int roomNumber;                  // 방번호
    public string title;                    // 방제목
    public int map;                         // 맵번호
    public State state;                     // 방 상태            
    public PlayerType myType;           // 나의 타입
    public int myNumber;                     // 나의 넘버(호스트가 1부터 시작)

    private PlayerInfo[] players;   // 방장 포함 . 순서대로. 나가면 빈칸이 있을 수 있음. 빈칸포함 앞번호부터 채워짐.
    private int m_playerCount;      // 플레이어 수  
    
    

    public RoomInfo(int roomNumber, string title, int map, PlayerInfo hostInfo, PlayerType myType)
    {
        this.roomNumber = roomNumber;
        this.title = title;
        m_playerCount = 0;

        players = new PlayerInfo[MaxPlayer];

        for (int i = 0; i < MaxPlayer; i++)
            players[i] = null;

        AddGuest(hostInfo);
        hostInfo.number = HostNumber;
        state = State.Wait;

        this.myType = myType;

        if(myType == PlayerType.Host)
        {
            myNumber = HostNumber;
        }
    }

    public bool isHost
    {
        get { return myType == PlayerType.Host; }
    }

    public int PlayerCount
    {
        get { return m_playerCount; }
    }

    public int AddGuest(PlayerInfo guestInfo)
    {
        if (m_playerCount >= MaxPlayer)
            return -1;
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i] == null)
            {
                players[i] = guestInfo;
                guestInfo.number = i+1;
                m_playerCount++;
                return guestInfo.number;
            }
        }
        return -1;
        
    }

    public bool AddGuest(PlayerInfo guestInfo, int number)
    {
        if (players[number-1] != null)
        {
            Debug.Log("AddGuest::게스트정보가 해당 자리에 이미 있습니다" + players[number-1].playerName);
            return false;
        }
        players[number-1] = guestInfo;
        guestInfo.number = number-1;
        m_playerCount++;
        return true;
    }

    public void RemoveGuest(int number)
    {
        if (number <= 1 || number >= MaxPlayer + 1)
            return;
        if (players[number - 1] != null)
        {
            players[number - 1] = null;
            m_playerCount--;
        }      
    }

    public PlayerInfo GetGuestInfo(int number)
    {        
        return players[number-1];
    }

    public PlayerInfo GetHostInfo()
    {
        return players[0];
    }

    public void GetAllGuestInfo(out byte[] number, out string[] userName)
    {
        number = new byte[m_playerCount];
        userName = new string[m_playerCount];
        int count = 0;
        for(int i = 0; i < m_playerCount; i++)
        {
            if(players[i] != null)
            {
                number[count] = (byte)players[i].number;
                userName[count] = players[i].playerName;
                count++;
            }
        }
    }
}