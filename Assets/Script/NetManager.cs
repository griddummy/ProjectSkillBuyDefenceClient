using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using System.Runtime.InteropServices;

public class NetManager : MonoBehaviour {

    TcpClient m_client;         // 서버 연결 소켓

    PacketQueue m_recvQueue;    // 패킷 받는 큐
    PacketQueue m_sendQueue;    // 패킷 보내는 큐
    
    byte[] m_sendBuffer;
    byte[] m_recvBuffer;
    const int BufferSize = 2048;

    public delegate void RecvNotifier(byte[] data);
    private Dictionary<int, RecvNotifier> m_notifier = new Dictionary<int, RecvNotifier>();

    public string IP;
    public int PORT = 9800;

    void Awake()
    {        
        m_sendBuffer = new byte[BufferSize];
        m_recvBuffer = new byte[BufferSize];
        m_client = new TcpClient();        
        m_recvQueue = new PacketQueue();
        m_sendQueue = new PacketQueue();
        m_client.OnReceived += OnReceivedPacket;
        m_client.Setup(IP, PORT);
    }

    void Update()
    {
        Receive();
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            m_client.DisConnect();
        }
    }   
    void OnApplicationQuit()
    {
        Debug.Log("SocketClose");
        ProgramExitPacket packet = new ProgramExitPacket();
        Send(packet);
        m_client.DisConnect();
    }
    public bool Connect()
    {
        return m_client.Connect();
    }
    private void Receive()
    {        
        int Count = m_recvQueue.Count;
        
        for( int i = 0; i < Count; i++)
        {
            int recvSize = 0;
            recvSize = m_recvQueue.Dequeue(ref m_recvBuffer, m_recvBuffer.Length);                
            
            if (recvSize > 0)
            {
                byte[] msg = new byte[recvSize];

                Array.Copy(m_recvBuffer, msg, recvSize);
                ReceivePacket(msg);
            }
        }        
    }

    private void OnReceivedPacket(byte[] msg, int size)
    {
       m_recvQueue.Enqueue(msg, size);             
    }

    public int Send<T>(IPacket<T> packet) // 패킷에 헤더를 부여하고 송신하는 메서드
    {        
        int sendSize = 0;
        byte[] packetData = packet.GetPacketData(); // 패킷의 데이터를 바이트화

        // 헤더 생성
        PacketHeader header = new PacketHeader();        
        HeaderSerializer serializer = new HeaderSerializer();
                
        header.length = (short)packetData.Length; // 패킷 데이터의 길이를 헤더에 입력
        header.id = (byte)packet.GetPacketId(); // 패킷 데이터에서 ID를 가져와 헤더에 입력
        Debug.Log("패킷 전송 - id : " + header.id.ToString() + " length :" + header.length);
        byte[] headerData = null;
        if(serializer.Serialize(header) == false)
        {
            return 0;
        }

        headerData = serializer.GetSerializedData(); // 헤더 데이터를 패킷 바이트로 변환
        

        byte[] data = new byte[headerData.Length + header.length]; // 최종 패킷의 길이 = 헤더패킷길이+내용패킷길이

        // 헤더와 내용을 하나의 배열로 복사
        int headerSize = Marshal.SizeOf( header.id ) + Marshal.SizeOf(header.length);
        Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
        Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);

        //전송
        sendSize = m_client.Send(data, data.Length);
        return sendSize;
    }

    public void RegisterReceiveNotification( int id, RecvNotifier notifier)
    {
        m_notifier.Add(id, notifier);
    }
    public void UnRegisterReceiveNotification(int id)
    {
        m_notifier.Remove(id);
    }
    private void ReceivePacket(byte[] data)
    {
        PacketHeader header = new PacketHeader();
        HeaderSerializer serializer = new HeaderSerializer();

        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref header);

        int packetId = header.id;
        int headerSize = Marshal.SizeOf(header.id)+ Marshal.SizeOf(header.length);
        int packetDataSize = data.Length - headerSize;
        byte[] packetData = null;
        if (packetDataSize > 0) //헤더만 있는 패킷을 대비해서 예외처리, 데이터가 있는 패킷만 데이터를 만든다
        {
            packetData = new byte[packetDataSize];
            Buffer.BlockCopy(data, headerSize, packetData, 0, packetData.Length);
        }
        Debug.Log("받은 패킷 - id : " + header.id + " dataLength : " + packetData.Length);
        RecvNotifier recvNoti;
        if (m_notifier.TryGetValue(packetId, out recvNoti))
        {
            recvNoti(packetData);
        }
        else
        {
            Debug.Log("NetManager::ReceivePacket() - 존재하지 않는 타입 패킷 :"+ packetId);
        }        
    }
}
