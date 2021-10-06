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
        GameQuestItem item = Instantiate<GameQuestItem>(questItemPrefab, listParent);
        if (questSavedData != null)
          questProgress = questSavedData.GetQuestProgress(questInfo.gameQuestID);

        item.SetQuest(questInfo, questProgress);
      }
    }
  }
}
