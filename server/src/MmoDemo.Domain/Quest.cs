namespace MmoDemo.Domain;

public class QuestDefinition
{
    public int QuestId { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string TargetMonsterType { get; init; } = "";
    public int TargetCount { get; init; }
    public int ExpReward { get; init; }
    public int GoldReward { get; init; }
}

public class PlayerQuestState
{
    public int QuestId { get; set; }
    public int Progress { get; set; }
    public bool Completed { get; set; }
}
