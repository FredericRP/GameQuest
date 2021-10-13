using FredericRP.EventManagement;
using UnityEditor;

namespace FredericRP.GameQuest
{
  [CustomEditor(typeof(GameQuestRewardEvent))]
  public class GameQuestRewardEventInspector : OneTypeGameEventInspector<GameQuestReward>
  { }
}