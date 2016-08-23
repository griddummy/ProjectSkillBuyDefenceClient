using UnityEngine;
using System.Collections;

public class InGameUnitDeathPacket : IPacket<InGameUnitDeathData>
{
    public class InGameUnitDeathSerializer : Serializer
    {
        public bool Serialize(InGameUnitDeathData data)
        {
            bool ret = true;

            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);

            return ret;
        }

        public bool Deserialize(ref InGameUnitDeathData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);

            return ret;
        }
    }
    InGameUnitDeathData m_data;

    public InGameUnitDeathPacket(InGameUnitDeathData data)
    {
        m_data = data;
    }

    public InGameUnitDeathPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitDeathSerializer serializer = new InGameUnitDeathSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitDeathData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitDeathSerializer serializer = new InGameUnitDeathSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitDeathData GetData()
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitDeath;
    }
}
