using FredericRP.EventManagement;
using UnityEngine;

namespace FredericRP.GameQuest
{
  /// <summary>
  /// Simplify the call to generic methods of GameEvent
  /// </summary>
  [CreateAssetMenu(menuName = "FredericRP/Game Quest/Quest event")]
  public class GameQuestEvent : TwoTypeGameEvent<GameQuestInfo, GameQuestSavedData.QuestProgress>
  {
  }
}