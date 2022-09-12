using FredericRP.EventManagement;
using FredericRP.PersistentData;
using System.Collections.Generic;
using UnityEngine;

namespace FredericRP.GameQuest
{
  public class FlowerGrabQuestReceiver : MonoBehaviour
  {
    [SerializeField]
    GameEvent flowerGrabEvent;
    [SerializeField]
    GameQuestEvent questLaunched;
    [SerializeField]
    GameQuestEvent questValidate;

    /// <summary>
    /// GameQuestManager does not allow to grab directly the full quest list, so we store the temp today's quests here
    /// </summary>
    List<GameQuestInfo> questInfoList;

    private void Start()
    {
      // Initialise quest data from quest manager and persistent data system
      GameQuestSavedData questSavedData = PersistentDataSystem.Instance.GetSavedData<GameQuestSavedData>();
      GameQuestSavedData.QuestProgress questProgress = null;
      questInfoList = new List<GameQuestInfo>();
      int count = GameQuestManager.Instance.GameQuestCatalog.TodayQuestCount();
      for (int i = 0; i < count; i++)
      {
        GameQuestInfo questInfo = GameQuestManager.Instance.GameQuestCatalog.GetAvailableQuest(i);
        questProgress = questSavedData.GetQuestProgress(questInfo.gameQuestID);
        // Unlock quest by default
        if (questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.Locked)
          questProgress.gameQuestStatus = GameQuestSavedData.GameQuestStatus.WaitingForEnable;
        questInfoList.Add(questInfo);
      }
    }

    private void OnEnable()
    {
      flowerGrabEvent.Listen<int>(FlowerGrabbed);
      questLaunched.Listen<GameQuestInfo, GameQuestSavedData.QuestProgress>(QuestLaunched);
    }
    private void OnDisable()
    {
      flowerGrabEvent.Delete<int>(FlowerGrabbed);
      questLaunched.Delete<GameQuestInfo, GameQuestSavedData.QuestProgress>(QuestLaunched);
    }

    private void QuestLaunched(GameQuestInfo questInfo, GameQuestSavedData.QuestProgress questProgress)
    {
      questProgress.currentProgress = 0;
    }

    void FlowerGrabbed(int id)
    {
      Debug.Log("FlowerGrabbed " + id);
      List<GameQuestInfo> questInfoFilteredList = questInfoList?.FindAll(item => item.targetId == id);

      if (questInfoFilteredList == null)
        return;
      for (int i = 0; i < questInfoFilteredList.Count; i++)
      {
        GameQuestInfo questInfo = questInfoFilteredList[i];
        Debug.Log("FlowerGrabbed " + id + " -> quest " + questInfo);
        if (questInfo != null && questInfo.runtimeQuestProgress?.gameQuestStatus == GameQuestSavedData.GameQuestStatus.InProgress)
        {
          questInfo.runtimeQuestProgress.currentProgress++;
          Debug.Log("FlowerGrabbed " + id + " -> progress " + questInfo.runtimeQuestProgress.currentProgress + " / " + questInfo.target);
          if (questInfo.runtimeQuestProgress.currentProgress >= questInfo.target)
          {
            questInfo.runtimeQuestProgress.currentProgress = questInfo.target;
            GameQuestManager.Instance.ValidateGameQuest(questInfo, questInfo.runtimeQuestProgress);
          }
        }
      }
    }
  }
}