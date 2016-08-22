
public class AttackUnitSerializer : Serializer {

	public bool Serialize(AttackUnitData data)
	{
		bool ret = true;
		ret &= Serialize(data.playerNum);
		ret &= Serialize(data.Id);
		ret &= Serialize(data.targetPlayerNum);
		ret &= Serialize(data.targetId);

		return ret;
	}

	public bool Deserialize(ref AttackUnitData element)
	{
		if (GetDataSize() == 0)
		{
			// 데이터가 설정되지 않았다.
			return false;
		}

		bool ret = true;

		byte playerNum = 0;
		byte Id = 0;
		byte targetPlayerNum = 0;
		byte targetId = 0;

		ret &= Deserialize (ref playerNum);
		ret &= Deserialize (ref Id);
		ret &= Deserialize (ref targetPlayerNum);
		ret &= Deserialize (ref targetId);
		element.playerNum = playerNum;
		element.Id = Id;
		element.targetPlayerNum = targetPlayerNum;
		element.targetId = targetId;

		return ret;
	}
}
