using Frederic.GameQuest;
using FredericRP.EasyLoading;
using FredericRP.SimpleLocalization;
using FredericRP.StringDataList;
using System;
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
    TextMeshProUGUI remainingTimeText;
    [SerializeField]
    MonitorGaugeProgress gaugeProgress;
    [SerializeField]
    GameObject rewardButton;
    [SerializeField]
    Image background;
    [SerializeField]
    Color enabledColor;
    [SerializeField]
    Color disabledColor;
    [SerializeField]
    Color waitingForRewardColor;

    private GameQuestInfo questInfo;
    private GameQuestSavedData.QuestProgress questProgress;

    public void SetQuest(GameQuestInfo questInfo, GameQuestSavedData.QuestProgress questProgress)
    {
      this.questInfo = questInfo;
      this.questProgress = questProgress;

      // Hack: we know what will be in the list and message, so we change the text accordingly ("Blue flower" -> "blue flowers")
      string targetName = DataListLoader.GetDataList("CurrencyList")?[questInfo.targetId].ToLower() + "s";
      // Set quest title and description
      titleText.text = string.Format(LocalizationManager.GetString("quest.title"), targetName);
      descriptionText.text = string.Format(LocalizationManager.GetString("quest.description"), questInfo.target, targetName);
      // Clamp the progress to the target
      int progress = Mathf.Clamp(questProgress.currentProgress, 0, questInfo.target);
      completionText.text = System.String.Format(LocalizationManager.GetString("quest.progress"), progress, questInfo.target);
      // For this sample, there is only one reward, which is a player currency (seeds) reward
      PlayerCurrencyReward currencyReward = questInfo.gameQuestRewardList[0] as PlayerCurrencyReward;
      if (currencyReward != null)
        rewardText.text = currencyReward.RewardCount.ToString();
      gaugeProgress.UpdateProgress((float)progress / questInfo.target);
      // Useful if you need immediate update when the quest popup is displayed and an event can update its progress at the same time
      gaugeProgress.GaugeId = "gameQuest-" + questInfo.gameQuestID;
      // activate reward button if quest is waiting for reward
      rewardButton.SetActive(questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.WaitingForReward);
      UpdateBackground();
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

    private void Update()
    {
      if (questInfo.duration >= 0 && questProgress.LaunchDate != null && questProgress.gameQuestStatus == GameQuestSavedData.GameQuestStatus.InProgress)
      {
        // show if quest is in progress
        UpdateBackground();

        TimeSpan remainingTime = GameQuestInfo.RemainingTime(questInfo, questProgress);

        if (remainingTime.TotalSeconds == 0)
          remainingTimeText.text = "";
        else
        {
          bool launched = questProgress.LaunchDate <= DateTime.Now;
          string message = launched ? "" : "starts in ";
          if (remainingTime.Hours > 0)
            message += remainingTime.Hours.ToString() + " h ";
          if (remainingTime.Minutes > 0)
            message += remainingTime.Minutes.ToString() + " min ";
          if (remainingTime.Seconds > 0)
            message += remainingTime.Seconds.ToString() + " sec ";
          if (launched)
            message += "left";
          remainingTimeText.text = message;
        }
      }
      else if (remainingTimeText.enabled)
      {
        remainingTimeText.text = "";
        remainingTimeText.enabled = false;
        // show if quest is in progress
        UpdateBackground();
      }
    }

    private void UpdateBackground()
    {
      switch(questProgress.gameQuestStatus)
      {
        case GameQuestSavedData.GameQuestStatus.WaitingForReward:
          background.color = waitingForRewardColor; break;
        case GameQuestSavedData.GameQuestStatus.InProgress:
          background.color = (questProgress.LaunchDate <= DateTime.Now) ? enabledColor : disabledColor; break;
        default:
          background.color = disabledColor; break;
      }
    }
  }
}