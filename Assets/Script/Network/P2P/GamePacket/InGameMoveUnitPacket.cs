using UnityEngine;
using System.Collections;

public class InGameUnitMovePacket : IPacket<InGameUnitMoveData>
{
    public class InGameUnitMoveSerializer : Serializer
    {
        public bool Serialize(InGameUnitMoveData data)
        {
            bool ret = true;
            ret &= Serialize(data.unitOwner);
            ret &= Serialize(data.unitId);
            ret &= Serialize(data.posX);
            ret &= Serialize(data.posY);
            ret &= Serialize(data.posZ);
            return ret;
        }

        public bool Deserialize(ref InGameUnitMoveData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.unitOwner);
            ret &= Deserialize(ref element.unitId);
            ret &= Deserialize(ref element.posX);
            ret &= Deserialize(ref element.posY);
            ret &= Deserialize(ref element.posZ);

            return ret;
        }
    }
    InGameUnitMoveData m_data;

    public InGameUnitMovePacket(InGameUnitMoveData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameUnitMovePacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitMoveSerializer serializer = new InGameUnitMoveSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitMoveData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitMoveSerializer serializer = new InGameUnitMoveSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitMoveData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitMove;
    }
}
