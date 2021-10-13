using UnityEngine;
using FredericRP.Popups;
using System.Collections.Generic;
using FredericRP.PersistentData;

namespace FredericRP.GameQuest
{
  public class GameQuestListPopup : PopupBase
  {
    [Header("Quest Prefab")]
    [SerializeField]
    GameQuestItem questItemPrefab;
    [SerializeField]
    Transform listParent;

    public override void Init(object parameters)
    {
      base.Init(parameters);
      Init((GameQuestCatalog)GetParameter(0));
    }
    public void Init(GameQuestCatalog gameQuestCatalog)
    {
      GameQuestSavedData questSavedData = PersistentDataSystem.Instance.GetSavedData<GameQuestSavedData>();
      GameQuestSavedData.QuestProgress questProgress = null;
      int todayQuestCount = gameQuestCatalog.TodayQuestCount();
      for (int i = 0; i < todayQuestCount; i++)
      {
        GameQuestInfo questInfo = gameQuestCatalog.GetAvailableQuest(i);
        if (questSavedData != null)
          questProgress = questSavedData.GetQuestProgress(questInfo.gameQuestID);
        // Do not display completed quests
        if (questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.Complete)
          continue;
        GameQuestItem item = Instantiate<GameQuestItem>(questItemPrefab, listParent);
        // Start each quest 5 minutes from the previous one, if not started
        if (questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.WaitingForEnable)
        {
          GameQuestManager.Instance.LaunchQuest(questInfo, questProgress);
          questProgress.LaunchDate = questProgress.LaunchDate.AddMinutes(5 * i);
        }
        // Relaunch quest if over
        if (GameQuestInfo.RemainingTime(questInfo, questProgress).TotalSeconds <= 0)
        {
          GameQuestManager.Instance.LaunchQuest(questInfo, questProgress);
          questProgress.LaunchDate = questProgress.LaunchDate.AddMinutes(5 * i);
        }
        item.SetQuest(questInfo, questProgress);
      }
    }
  }
}
