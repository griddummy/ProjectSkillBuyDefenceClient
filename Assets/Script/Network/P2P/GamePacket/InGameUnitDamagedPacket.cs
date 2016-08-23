using UnityEngine;
using System.Collections;

public class InGameUnitDamagedPacket : IPacket<InGameUnitDamagedData>
{
    public class InGameUnitDamagedSerializer : Serializer
    {
        public bool Serialize(InGameUnitDamagedData data)
        {
            bool ret = true;

            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.damage);

            return ret;
        }

        public bool Deserialize(ref InGameUnitDamagedData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.damage);

            return ret;
        }
    }
    InGameUnitDamagedData m_data;

    public InGameUnitDamagedPacket(InGameUnitDamagedData data)
    {
        m_data = data;
    }

    public InGameUnitDamagedPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitDamagedSerializer serializer = new InGameUnitDamagedSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitDamagedData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitDamagedSerializer serializer = new InGameUnitDamagedSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitDamagedData GetData()
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitDamaged;
    }
}
