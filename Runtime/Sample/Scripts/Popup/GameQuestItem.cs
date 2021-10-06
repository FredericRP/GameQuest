using Frederic.GameQuest;
using FredericRP.EasyLoading;
using FredericRP.SimpleLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FredericRP.GameQuest
{
  public class GameQuestItem : MonoBehaviour
  {
    [Header("Links")]
    [SerializeField]
    Text titleText;
    [SerializeField]
    Text descriptionText;
    [SerializeField]
    TextMeshProUGUI completionText;
    [SerializeField]
    TextMeshProUGUI rewardText;
    [SerializeField]
    MonitorGaugeProgress gaugeProgress;
    [SerializeField]
    GameObject rewardButton;

    private GameQuestInfo questInfo;
    private GameQuestSavedData.QuestProgress questProgress;

    public void SetQuest(GameQuestInfo questInfo, GameQuestSavedData.QuestProgress questProgress)
    {
      this.questInfo = questInfo;
      this.questProgress = questProgress;

      titleText.text = LocalizationManager.GetString("quest." + questInfo.localizationId + ".title");
      descriptionText.text = LocalizationManager.GetString("quest." + questInfo.localizationId + ".description");
      completionText.text = System.String.Format(LocalizationManager.GetString("quest.progress"), questProgress.currentProgress, questInfo.target);
      // For this sample, there is only one reward, which is a player currency (seeds) reward
      PlayerCurrencyReward currencyReward = questInfo.gameQuestRewardList[0] as PlayerCurrencyReward;
      if (currencyReward != null)
        rewardText.text = currencyReward.RewardCount.ToString();
      gaugeProgress.UpdateProgress((float)questProgress.currentProgress / questInfo.target);
      // Useful if you need immediate update when the quest popup is displayed and an event can update its progress at the same time
      gaugeProgress.GaugeId = "gameQuest-" + questInfo.gameQuestID;
      // activate reward button if quest is waiting for reward
      rewardButton.SetActive(questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.WaitingForReward);
    }

    public void Validate()
    {
      if (questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.WaitingForEnable)
      {
        LaunchQuest();
      }
      else if (questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.WaitingForReward)
      {
        GetReward();
      }
    }

    public void LaunchQuest()
    {
      GameQuestManager.Instance.LaunchQuest(questInfo, questProgress);
    }

    public void GetReward()
    {
      GameQuestManager.Instance.GiveGameQuestReward(questInfo, questProgress);
      // immediately deactivate button as the reward has been given
      rewardButton.SetActive(false);
    }
  }
}