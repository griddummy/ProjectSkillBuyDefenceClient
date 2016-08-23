using UnityEngine;
using System.Collections;

public class InGameCreateUnitPacket : IPacket<InGameCreateUnitData>
{

    public class InGameCreateUnitSerializer : Serializer
    {
        public bool Serialize(InGameCreateUnitData data)
        {
            bool ret = true;
            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.unitType);
            ret &= Serialize(data.position.x);
            ret &= Serialize(data.position.y);
            ret &= Serialize(data.position.z);
            ret &= Serialize(data.level);
            return ret;
        }

        public bool Deserialize(ref InGameCreateUnitData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;
            
            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.unitType);
            ret &= Deserialize(ref element.position.x);
            ret &= Deserialize(ref element.position.y);
            ret &= Deserialize(ref element.position.z);
            ret &= Deserialize(ref element.level);

            return ret;
        }
    }
    InGameCreateUnitData m_data;

    public InGameCreateUnitPacket(InGameCreateUnitData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameCreateUnitPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameCreateUnitSerializer serializer = new InGameCreateUnitSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameCreateUnitData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameCreateUnitSerializer serializer = new InGameCreateUnitSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameCreateUnitData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.CreateUnit;
    }
}
