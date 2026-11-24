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
      LaunchTodayQuests(gameQuestCatalog);
      int todayQuestCount = gameQuestCatalog.TodayQuestCount();
      for (int i = 0; i < todayQuestCount; i++)
      {
        GameQuestInfo questInfo = gameQuestCatalog.GetAvailableQuest(i);
        GameQuestSavedData.QuestProgress questProgress = questSavedData.GetQuestProgress(questInfo.gameQuestID);
        // Do not display completed quests
        if (questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.Complete)
          continue;
        GameQuestItem item = Instantiate<GameQuestItem>(questItemPrefab, listParent);
        item.SetQuest(questInfo, questProgress);
      }
    }

    /// <summary>
    /// Launch today's quests that are not started yet, each one 5 minutes after the previous one, and relaunch the ones that are over.<br/>
    /// Called when the game starts, so the first quest is active without opening the popup, and again when the popup opens.
    /// </summary>
    /// <param name="gameQuestCatalog"></param>
    public static void LaunchTodayQuests(GameQuestCatalog gameQuestCatalog)
    {
      GameQuestSavedData questSavedData = PersistentDataSystem.Instance.GetSavedData<GameQuestSavedData>();
      int todayQuestCount = gameQuestCatalog.TodayQuestCount();
      for (int i = 0; i < todayQuestCount; i++)
      {
        GameQuestInfo questInfo = gameQuestCatalog.GetAvailableQuest(i);
        GameQuestSavedData.QuestProgress questProgress = questSavedData.GetQuestProgress(questInfo.gameQuestID);
        bool notStarted = questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.WaitingForEnable;
        // Only relaunch quests still in progress: a quest waiting for its reward keeps it
        bool over = questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.InProgress
          && GameQuestInfo.RemainingTime(questInfo, questProgress).TotalSeconds <= 0;
        if (notStarted || over)
        {
          GameQuestManager.Instance.LaunchQuest(questInfo, questProgress);
          questProgress.LaunchDate = questProgress.LaunchDate.AddMinutes(5 * i);
        }
      }
    }
  }
}
