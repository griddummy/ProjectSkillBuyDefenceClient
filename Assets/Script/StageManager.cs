using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class StageManager
{
    private const int stageNum = 10;
    List<Stage> stages;

    public StageManager()
    {
        stages = new List<Stage>();
        stages.Add(new Stage(1, 400));
        stages.Add(new Stage(2, 400));
        stages.Add(new Stage(3, 400));
        stages.Add(new Stage(4, 400));
        stages.Add(new Stage(5, 400));
        stages.Add(new Stage(6, 600));
        stages.Add(new Stage(7, 600));
        stages.Add(new Stage(8, 600));
        stages.Add(new Stage(9, 600));
        stages.Add(new Stage(10, 600));
        
        stages[0].SetStage(new SpawnMonster(2, 1));
        
        stages[1].SetStage(new SpawnMonster(2, 2));
        
        stages[2].SetStage(new SpawnMonster(2, 3));
        
        stages[3].SetStage(new SpawnMonster(2, 2));
        stages[3].SetStage(new SpawnMonster(3, 1));
        
        stages[4].SetStage(new SpawnMonster(2, 2));
        stages[4].SetStage(new SpawnMonster(3, 3));
        
        stages[5].SetStage(new SpawnMonster(4, 2));
        
        stages[6].SetStage(new SpawnMonster(2, 2));
        stages[6].SetStage(new SpawnMonster(4, 2));
        
        stages[7].SetStage(new SpawnMonster(2, 4));
        stages[7].SetStage(new SpawnMonster(3, 3));
        
        stages[8].SetStage(new SpawnMonster(3, 1));
        stages[8].SetStage(new SpawnMonster(4, 3));
        
        stages[9].SetStage(new SpawnMonster(2, 3));
        stages[9].SetStage(new SpawnMonster(3, 3));
        stages[9].SetStage(new SpawnMonster(4, 3));
    }

    public Stage GetStageData(int num)
    {
        return stages[num - 1];
    }
}

class Stage
{
    int stageNum;
    private int rewardGold;
    private List<SpawnMonster> monsters;

    public int RewardGold { get { return rewardGold; } }
    public List<SpawnMonster> Monsters { get { return monsters; } }

    public Stage(int num, int gold)
    {
        stageNum = num;
        rewardGold = gold;
        monsters = new List<SpawnMonster>();
    }

    public void SetStage(SpawnMonster monster)
    {
        monsters.Add(monster);
    }
}

public class SpawnMonster
{
    public int Id;
    public int number;

    public SpawnMonster(int newId, int newNumber)
    {
        Id = newId;
        number = newNumber;
    }
}
