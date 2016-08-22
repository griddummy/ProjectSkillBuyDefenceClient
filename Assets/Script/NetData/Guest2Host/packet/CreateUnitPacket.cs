using UnityEngine;
using System.Collections;
using System;

public class CreateUnitPacket : IPacket<CreateUnitData>
{
	CreateUnitData m_data;

	public CreateUnitPacket(CreateUnitData data) // 데이터로 초기화(송신용)
	{
		m_data = data;
	}

	public CreateUnitPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
	{
		CreateUnitSerializer serializer = new CreateUnitSerializer();
		serializer.SetDeserializedData(data);
		m_data = new CreateUnitData();
		serializer.Deserialize(ref m_data); 
	}

	public byte[] GetPacketData() // 바이트형 패킷(송신용)
	{
		CreateUnitSerializer serializer = new CreateUnitSerializer();
		serializer.Serialize(m_data);
		return serializer.GetSerializedData();
	}

	public CreateUnitData GetData() // 데이터 얻기(수신용)
	{
		return m_data;
	}

	public int GetPacketId()
	{
		return (int)GuestPacketId.CreateUnit;
	}
}
