using FredericRP.PlayerCurrency;
using FredericRP.StringDataList;
using System.Collections.Generic;

namespace FredericRP.GameQuest
{
  [System.Serializable]
  public class GameQuestInfo
  {
    public string gameQuestID;
    public string localizationId = "";

    /// <summary>
    /// Is this quest enabled only on a specific month? (-1 otherwise)
    /// </summary>
    public int month = -1;
    /// <summary>
    /// Is this quest enabled only a specific day in the month? (-1 otherwise)
    /// </summary>
    public int dayInMonth = -1;
    /// <summary>
    /// Is this quest enabled only a specific year? (-1 otherwise)
    /// </summary>
    public int year = -1;

    /// <summary>
    /// Duration in seconds of this quest when activated
    /// </summary>
    public int duration = 0;

    /// <summary>
    /// Target amount to reach
    /// </summary>
    public int target = 5;

    /// <summary>
    /// What does the target represents? Link that to your favorite Enum or own IDs in game
    /// </summary>
    [Select(PlayerCurrencyData.CurrencyList)]
    public int targetId;

    public List<GameQuestReward> gameQuestRewardList;

    public GameQuestSavedData.QuestProgress runtimeQuestProgress;

    public static System.TimeSpan RemainingTime(GameQuestInfo questInfo, GameQuestSavedData.QuestProgress questProgress)
    {
      System.TimeSpan time;
      // Return remaining time until the end of the quest
      if (questProgress.LaunchDate <= System.DateTime.Now)
        time = questProgress.LaunchDate.AddSeconds(questInfo.duration).Subtract(System.DateTime.Now);
      else // or time until it launches if not yet launched
        time = questProgress.LaunchDate.Subtract(System.DateTime.Now);

      if (time.TotalSeconds > 0)
        return time;

      return new System.TimeSpan(0);
    }
  }
}
