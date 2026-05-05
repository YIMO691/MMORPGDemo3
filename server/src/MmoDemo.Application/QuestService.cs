using System.Collections.Concurrent;
using MmoDemo.Domain;

namespace MmoDemo.Application;

public class QuestService : IQuestService
{
    private static readonly Dictionary<int, QuestDefinition> _definitions = new()
    {
        [1] = new QuestDefinition { QuestId = 1, Name = "Slime Extermination", Description = "Kill 3 Slimes", TargetMonsterType = "slime", TargetCount = 3, ExpReward = 30, GoldReward = 20 },
        [2] = new QuestDefinition { QuestId = 2, Name = "Goblin Threat", Description = "Kill 2 Goblins", TargetMonsterType = "goblin", TargetCount = 2, ExpReward = 50, GoldReward = 35 },
        [3] = new QuestDefinition { QuestId = 3, Name = "Wolf Hunt", Description = "Kill 1 Wolf", TargetMonsterType = "wolf", TargetCount = 1, ExpReward = 40, GoldReward = 25 },
    };

    private readonly ConcurrentDictionary<string, PlayerQuestState> _active = new();

    public QuestDefinition? GetDefinition(int questId) =>
        _definitions.TryGetValue(questId, out var d) ? d : null;

    public PlayerQuestState? AcceptQuest(string playerId, int questId)
    {
        if (!_definitions.ContainsKey(questId)) return null;
        var state = new PlayerQuestState { QuestId = questId, Progress = 0 };
        _active[playerId] = state;
        return state;
    }

    public PlayerQuestState? GetActiveQuest(string playerId) =>
        _active.TryGetValue(playerId, out var s) && !s.Completed ? s : null;

    public PlayerQuestState? OnMonsterKilled(string playerId, string monsterType)
    {
        if (!_active.TryGetValue(playerId, out var state)) return null;
        if (state.Completed) return null;
        if (!_definitions.TryGetValue(state.QuestId, out var def)) return null;
        if (!string.Equals(def.TargetMonsterType, monsterType, StringComparison.OrdinalIgnoreCase))
            return null;

        state.Progress++;
        return state;
    }

    public QuestDefinition? CheckComplete(string playerId)
    {
        if (!_active.TryGetValue(playerId, out var state)) return null;
        if (state.Completed) return null;
        if (!_definitions.TryGetValue(state.QuestId, out var def)) return null;
        if (state.Progress < def.TargetCount) return null;

        state.Completed = true;
        return def;
    }
}
