using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
public class PacketQueue
{

    struct PacketInfo
    {
        public int offset; // 읽어야 되는 위치
        public int size; // 마지막위치+1 ?
    };

    // 데이터를 보존 할 버퍼
    private MemoryStream m_streamBuffer;

    // 패킷 정보 관리 리스트
    private List<PacketInfo> m_offsetList;

    private int m_offest; // 메모리 배치 오프셋

    private object lockObj = new object(); // 락 오브젝트
    byte[] tempBuffer;
    int tempSize;
    int tempCompleteSize;
    public PacketQueue() // 생성자
    {
        tempBuffer = new byte[2048];
        tempSize = 0;
        tempCompleteSize = 0;
        m_streamBuffer = new MemoryStream();
        m_offsetList = new List<PacketInfo>();
    }
    public int Count
    {
        get
        {
            return m_offsetList.Count;
        }
    }

    public void AddTempBuffer(byte[] data, int size)
    {
        if (tempCompleteSize == 0) // 헤더사이즈가 없으면 새로 구한다.
        {
            short contentSize = BitConverter.ToInt16(data, 0);
            tempCompleteSize = contentSize + sizeof(short) + sizeof(byte); // 한 패킷 전체 사이즈
            Debug.Log("total:" + tempCompleteSize + " " + contentSize);
        }

        Debug.Log("tempSize:"+ tempSize+" size:"+size + " Total:"+ tempCompleteSize);
        if (tempSize + size == tempCompleteSize) // 임시버퍼+현재데이터의 사이즈가 완성패킷의 크기라면
        {            
            byte[] complete = new byte[tempCompleteSize];
            Array.Copy(tempBuffer, complete, tempSize); // 임시버퍼에 있는 내용과
            Array.Copy(data, 0, complete, tempSize, size); // 새로 들어온 내용을 합쳐서
            Debug.Log("tempSize + size == tempCompleteSize:" + tempCompleteSize );
            EnqueueReal(complete, tempCompleteSize); // 큐 삽입
            // 초기화
            tempSize = 0;
            tempCompleteSize = 0;
        }
        else if (tempSize + size < tempCompleteSize) // 아직 덜왓으면
        {
            Array.Copy(data, 0, tempBuffer, tempSize, size); // 임시 버퍼에 Add
            tempSize = tempSize + size; // 새 사이즈 = 기존버퍼사이즈 + 지금 추가 사이즈
        }
        else if (tempSize + size > tempCompleteSize) // 더 많이 왓으면
        {
            byte[] complete = new byte[tempCompleteSize];
            Array.Copy(tempBuffer, complete, tempSize);// 임시버퍼에 있는거와
            Array.Copy(data, 0, complete, tempSize, tempCompleteSize - tempSize); // 새로들어온거 일부를 합쳐서
            EnqueueReal(complete, tempCompleteSize); // 큐 삽입
            // 나머지 부분은 다시 임시버퍼에 추가
            int newSize = size - (tempCompleteSize - tempSize);
            byte[] temp = new byte[newSize];
            Array.Copy(temp, 0, data, tempCompleteSize - tempSize, newSize);
            // 초기화
            tempSize = 0;
            tempCompleteSize = 0;
            AddTempBuffer(temp, temp.Length);
        }
    }
    public int EnqueueReal(byte[] data, int size)
    {

        PacketInfo info = new PacketInfo();
        // 패킷 정보 작성
        info.offset = m_offest;
        info.size = size;

        lock (lockObj)
        {
            m_offsetList.Add(info); // 패킷 저장 정보 보존

            // 패킷데이터 쓰기
            m_streamBuffer.Position = m_offest;
            m_streamBuffer.Write(data, 0, size);
            m_streamBuffer.Flush();
            m_offest += size;
        }
        return size;
    }

    public int Enqueue(byte[] data, int size)
    {
        //AddTempBuffer(data, size);
        PacketInfo info = new PacketInfo();
        // 패킷 정보 작성
        info.offset = m_offest;
        info.size = size;

        lock (lockObj)
        {
            m_offsetList.Add(info); // 패킷 저장 정보 보존

            // 패킷데이터 쓰기
            m_streamBuffer.Position = m_offest;
            m_streamBuffer.Write(data, 0, size);
            m_streamBuffer.Flush();
            m_offest += size;
        }
        return size;
    }

    public int Dequeue(ref byte[] buffer, int size)
    {
        if (m_offsetList.Count <= 0)
        {
            return -1;
        }

        int recvSize = 0;
        lock (lockObj)
        {
            PacketInfo info = m_offsetList[0];

            // 버퍼에서 데이터 가져오기
            int dataSize = Math.Min(size, info.size);
            m_streamBuffer.Position = info.offset;
            recvSize = m_streamBuffer.Read(buffer, 0, dataSize);

            // 큐 데이터를 꺼냈으면 맨 앞 데이터는 삭제
            if (recvSize > 0)
            {
                m_offsetList.RemoveAt(0);
            }

            // 모든 큐 데이터를 꺼냇으면 스트림 정리해서 메모리 절약한다.
            if (m_offsetList.Count == 0)
            {
                Clear();
                m_offest = 0;
            }
        }
        return recvSize;
    }

    public void Clear()
    {
        byte[] buffer = m_streamBuffer.GetBuffer();
        Array.Clear(buffer, 0, buffer.Length);

        m_streamBuffer.Position = 0;
        m_streamBuffer.SetLength(0);
    }
}
