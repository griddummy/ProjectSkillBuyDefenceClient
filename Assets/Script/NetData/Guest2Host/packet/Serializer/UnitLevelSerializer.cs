
public class UnitLevelSerializer : Serializer {

	public bool Serialize(UnitLevelData data)
	{
		bool ret = true;
		ret &= Serialize (data.playerNum);
		ret &= Serialize (data.Id);
		ret &= Serialize (data.level);

		return ret;
	}

	public bool Deserialize(ref UnitLevelData element)
	{
		if (GetDataSize() == 0)
		{
			// 데이터가 설정되지 않았다.
			return false;
		}

		bool ret = true;

		byte playerNum = 0;
		byte Id = 0;
		byte level = 0;

		ret &= Deserialize (ref playerNum);
		ret &= Deserialize (ref Id);
		ret &= Deserialize (ref level);

		element.playerNum = playerNum;
		element.Id = Id;
		element.level = level;

		return ret;
	}
}
