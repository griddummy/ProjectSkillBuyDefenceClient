using UnityEngine;
using System.Collections;

public class InGameUnitLevelUpPacket : IPacket<InGameUnitLevelUpData>
{
    public class InGameUnitLevelUpSerializer : Serializer
    {
        public bool Serialize(InGameUnitLevelUpData data)
        {
            bool ret = true;

            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.level);

            return ret;
        }

        public bool Deserialize(ref InGameUnitLevelUpData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.level);

            return ret;
        }
    }
    InGameUnitLevelUpData m_data;

    public InGameUnitLevelUpPacket(InGameUnitLevelUpData data)
    {
        m_data = data;
    }

    public InGameUnitLevelUpPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitLevelUpSerializer serializer = new InGameUnitLevelUpSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitLevelUpData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitLevelUpSerializer serializer = new InGameUnitLevelUpSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitLevelUpData GetData()
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitLevelUp;
    }
}
