using FredericRP.GameQuest;
using FredericRP.Popups;
using TMPro;
using UnityEngine;

public class ShowQuestPopup : ShowPopup
{
  [SerializeField]
  bool onAwake = false;
  [SerializeField]
  GameQuestEvent onGameQuestComplete;
  [SerializeField]
  GameQuestEvent onGameQuestValidate;
  [SerializeField]
  GameObject alertIcon;
  [SerializeField]
  TextMeshProUGUI text;

  private void Awake()
  {
    if (onAwake)
      DisplayPopup();
  }

  private void OnEnable()
  {
    onGameQuestComplete.Listen<GameQuestInfo, GameQuestSavedData.QuestProgress>(UpdateAlertIcon);
    onGameQuestValidate.Listen<GameQuestInfo, GameQuestSavedData.QuestProgress>(UpdateAlertIcon);
  }

  private void OnDisable()
  {
    onGameQuestComplete.Delete<GameQuestInfo, GameQuestSavedData.QuestProgress>(UpdateAlertIcon);
    onGameQuestValidate.Delete<GameQuestInfo, GameQuestSavedData.QuestProgress>(UpdateAlertIcon);
  }

  private void UpdateAlertIcon(GameQuestInfo questInfo, GameQuestSavedData.QuestProgress questProgress)
  {
    int currentWaitingQuestCount = GameQuestManager.Instance.GetWaitingForRewardCount();
    alertIcon.SetActive(currentWaitingQuestCount > 0);
    text.text = currentWaitingQuestCount.ToString();
  }

  public override void DisplayPopup()
  {
    /*
    // 1st: show complete quest that is waiting for reward
    GameQuestInfo questInfo = GameQuestManager.Instance.GetQuestFromStatus(GameQuestSavedData.GameQuestStatus.WaitingForReward);
    if (questInfo == null)
    {
      // If none, show current quest
      questInfo = GameQuestManager.Instance.GetInProgressQuest();
      if (questInfo == null)
      {
        // If none, show next available quest
        questInfo = GameQuestManager.Instance.GetWaitingForEnableQuest();
        if (questInfo == null)
          return;
      }
    }
    GameQuestSavedData questSavedData = PersistentDataSystem.Instance.GetSavedData<GameQuestSavedData>();
    GameQuestSavedData.QuestProgress questProgress = null;
    if (questSavedData != null)
      questProgress = questSavedData.GetQuestProgress(questInfo.gameQuestID);
      // */
    PopupHandler.Instance.ShowPopup(popup, new object[] { GameQuestManager.Instance.GameQuestCatalog });
  }
}
