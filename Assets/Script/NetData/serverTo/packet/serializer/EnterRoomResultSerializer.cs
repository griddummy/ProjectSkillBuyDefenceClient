using UnityEngine;
using System.Collections;

public class EnterRoomResultSerializer : Serializer {

    public bool Serialize(EnterRoomResultData data)
    {
        bool ret = true;
        ret &= Serialize(data.result);
        return ret;
    }

    public bool Deserialize(ref EnterRoomResultData element)
    {
        if (GetDataSize() == 0)
        {
            // 데이터가 설정되지 않았다.
            return false;
        }

        bool ret = true;
        string result;
        ret &= Deserialize(out result, (int)GetDataSize());
        element.result = result;
        return ret;
    }
}
