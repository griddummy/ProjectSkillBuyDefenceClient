using UnityEngine;
using System.Collections;
using System;

public class MoveUnitPacket : IPacket<MoveUnitData>
{
	MoveUnitData m_data;

	public MoveUnitPacket(MoveUnitData data) // 데이터로 초기화(송신용)
	{
		m_data = data;
	}

	public MoveUnitPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
	{
		MoveUnitSerializer serializer = new MoveUnitSerializer();
		serializer.SetDeserializedData(data);
		m_data = new MoveUnitData();
		serializer.Deserialize(ref m_data); 
	}

	public byte[] GetPacketData() // 바이트형 패킷(송신용)
	{
		MoveUnitSerializer serializer = new MoveUnitSerializer();
		serializer.Serialize(m_data);
		return serializer.GetSerializedData();
	}

	public MoveUnitData GetData() // 데이터 얻기(수신용)
	{
		return m_data;
	}

	public int GetPacketId()
	{
		return (int)GuestPacketId.MoveUnit;
	}
}
