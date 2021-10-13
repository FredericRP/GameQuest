using FredericRP.EventManagement;
using UnityEditor;

namespace FredericRP.GameQuest
{
  [CustomEditor(typeof(GameQuestEvent))]
  public class GameQuestEventInspector : TwoTypeGameEventInspector<GameQuestInfo, GameQuestSavedData.QuestProgress>
  { }
}