
public enum BossEventType { Tiger, Cat, hawk, }//Cow, Snake, Horse, Dragon, sheep }
public enum TileEventType { None, Card, Boar, BoarStop, Save, Nuts, River, HawkStop }
public enum CardType{None,PlusTwoRoll,Trap}

[System.Serializable]
public class RankEntry
{
    public string CharacterName;
    public bool IsPlayer;
    public int GoalTurn;
    public bool IsGoal;
    public int TiebreakerRoll;

    public RankEntry(string name, bool isPlayer, int turn, bool isGoal)
    {
        CharacterName = name;
        IsPlayer = isPlayer;
        GoalTurn = turn;
        IsGoal = isGoal;
    }
}