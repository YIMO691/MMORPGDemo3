using MmoDemo.Domain;

namespace MmoDemo.Application;

public interface IQuestService
{
    QuestDefinition? GetDefinition(int questId);
    PlayerQuestState? AcceptQuest(string playerId, int questId);
    PlayerQuestState? GetActiveQuest(string playerId);
    /// Returns quest update if progressed, null otherwise
    PlayerQuestState? OnMonsterKilled(string playerId, string monsterType);
    /// Returns the completed quest, null if nothing completed
    QuestDefinition? CheckComplete(string playerId);
}
