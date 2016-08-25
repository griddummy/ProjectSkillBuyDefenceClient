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
    }

    /*
     * stageManager.stages.Add(new Stage(1, 400));
     * stageManager.stages.Add(new Stage(2, 400));
     * stageManager.stages.Add(new Stage(3, 400));
     * stageManager.stages.Add(new Stage(4, 400));
     * stageManager.stages.Add(new Stage(5, 400));
     * stageManager.stages.Add(new Stage(6, 600));
     * stageManager.stages.Add(new Stage(7, 600));
     * stageManager.stages.Add(new Stage(8, 600));
     * stageManager.stages.Add(new Stage(9, 600));
     * stageManager.stages.Add(new Stage(10, 600));
     * 
     * stageManager.stages[0].SetStage(new SpawnMonster(2, 1));
     * 
     * stageManager.stages[1].SetStage(new SpawnMonster(2, 2));
     * 
     * stageManager.stages[2].SetStage(new SpawnMonster(2, 3));
     * 
     * stageManager.stages[3].SetStage(new SpawnMonster(2, 2));
     * stageManager.stages[3].SetStage(new SpawnMonster(3, 1));
     * 
     * stageManager.stages[4].SetStage(new SpawnMonster(2, 2));
     * stageManager.stages[4].SetStage(new SpawnMonster(3, 3));
     * 
     * stageManager.stages[5].SetStage(new SpawnMonster(4, 2));
     * 
     * stageManager.stages[6].SetStage(new SpawnMonster(2, 2));
     * stageManager.stages[6].SetStage(new SpawnMonster(4, 2));
     * 
     * stageManager.stages[7].SetStage(new SpawnMonster(2, 4));
     * stageManager.stages[7].SetStage(new SpawnMonster(3, 3));
     * 
     * stageManager.stages[8].SetStage(new SpawnMonster(3, 1));
     * stageManager.stages[8].SetStage(new SpawnMonster(4, 3));
     * 
     * stageManager.stages[9].SetStage(new SpawnMonster(2, 3));
     * stageManager.stages[9].SetStage(new SpawnMonster(3, 3));
     * stageManager.stages[9].SetStage(new SpawnMonster(4, 3));
     
     */

    /* In GameManager
     * 
     * 초기화
     * 
     * StageManager stageManager = new StageManager();
     * UIControl uiControl 
     * GameObject[] MonsterSpawnPoint = new GameObject [curRoomInfo.playerCount];
     * 
     * ...저기 위에 있는 Setting...
     * 
     * currentStageNum = 1;
     * GameObject[,] monsterChecker;
     * bool stageIsOver = false;
     * 
     * 생성 할 때
     * 
     * 지금 스테이지의 몬스터 스폰 정보를 받아옴
     * List<SpawnMonster> monsters = GetStageData(currentStageNum);
     * int monsterNum = monsters.Count;
     * 
     * 생성되는 몬스터를 체크하기 위한 배열
     * monsterChecker = new GameObject [curRoomInfo.playerCount, monsterNum];
     * 
     * for(int i = 0; i < MonsterSpawnPoint.Lenght; i++)
     * {
     *     for (int j = 0; j < monsters.Count; j++)
     *     {
     *         string monsterName = DataBase.Instance.GetUnitData(monsters[i].Id)).UnitName;
     *         GameObject monster = Instantiate(Resources.Load<GameObject>(monsterName), spawnPoint.transform.position, Quaternion.identity) as GameObject;
     *         monsterChecker[i, j] = monster;
     *     }
     * }
     * 
     * 생성한 뒤에 체크 코루틴 실행
     * StartCoroutine (CheckStageOver);
     * 
     * Updata문또는 코루틴에서 stageIsOver == true 를 감지해서
     *  
     * 
     * 
     * IEnumerator CheckStageOver ()
     * {
     *     return yield new WaitForSecond (0.5f);
     *     
     *     for(int i = 0; i < MonsterSpawnPoint.Lenght; i++)
     *     {
     *         for (int j = 0; j < monsters.Count; j++)
     *         {
     *              if(monsterChecker[i,j] != null)
     *              {
     *                  stageIsOver = false;
     *                  yield return null;
     *              }
     *              else
     *              {
     *                  stageIsOver = true;         
     *              }
     *         }
     *     }
     *     
     *     if(stageIsOver)
     *     {
     *         GetComponent<PlayerController>().mainUI.gold += stageManager.stages[currentStageNum].Gold;
     *         currentStageNum++;
     *         stageIsOver = false;
     *         StopCoroutine (CheckStageOver);
     *     }
     * }
    */

    public List<SpawnMonster> GetStageData(int num)
    {
        return stages[num - 1].monsters;
    }
}

class Stage
{
    int stageNum;
    public int rewardGold;
    public List<SpawnMonster> monsters;

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

public struct SpawnMonster
{
    public int Id;
    public int number;
}
