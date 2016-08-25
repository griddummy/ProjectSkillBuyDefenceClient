using UnityEngine;
using System.Collections;

public class InGameUnitImmediatelyMovePacket : IPacket<InGameUnitImmediatelyMoveData>
{
    public class InGameUnitMoveSerializer : Serializer
    {
        public bool Serialize(InGameUnitImmediatelyMoveData data)
        {
            bool ret = true;
            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.destination.x);
            ret &= Serialize(data.destination.y);
            ret &= Serialize(data.destination.z);
            ret &= Serialize(data.forward.x);
            ret &= Serialize(data.forward.y);
            ret &= Serialize(data.forward.z); 

            return ret;
        }

        public bool Deserialize(ref InGameUnitImmediatelyMoveData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.destination.x);
            ret &= Deserialize(ref element.destination.y);
            ret &= Deserialize(ref element.destination.z);
            ret &= Deserialize(ref element.forward.x);
            ret &= Deserialize(ref element.forward.y);
            ret &= Deserialize(ref element.forward.z);

            return ret;
        }
    }
    InGameUnitImmediatelyMoveData m_data;

    public InGameUnitImmediatelyMovePacket(InGameUnitImmediatelyMoveData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameUnitImmediatelyMovePacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitMoveSerializer serializer = new InGameUnitMoveSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitImmediatelyMoveData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitMoveSerializer serializer = new InGameUnitMoveSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitImmediatelyMoveData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitImmediatelyMove;
    }
}
