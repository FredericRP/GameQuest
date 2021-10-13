using FredericRP.PersistentData;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace FredericRP.GameQuest
{
  [Serializable]
  public class GameQuestSavedData : SavedData
  {
    public enum LaunchMode { Immediate, OnUserAction };

    /// <summary>
    /// A quest can be locked, waiting for activation (unlocked but user has to activate it), in progress, ended and waiting for the user to get its reward,
    /// or complete (reward has been dispatched)
    /// </summary>
    public enum GameQuestStatus { Locked, WaitingForEnable, InProgress, WaitingForReward, Complete };

    [System.NonSerialized]
    const string dateTimeFormat = "dd/MM/yyyy HH:mm";
    [System.NonSerialized]
    static CultureInfo dateTimeProvider = CultureInfo.InvariantCulture;

    [System.Serializable]
    public class QuestProgress : IEquatable<QuestProgress>
    {
      public string gameQuestId;
      [SerializeField]
      protected string launchDate;
      /// <summary>
      /// Launch date of the quest. If it's in the future, it is not launched yet (!)
      /// </summary>
      public System.DateTime LaunchDate { get { return String.IsNullOrEmpty(launchDate) ? System.DateTime.Now.AddYears(1) : System.DateTime.ParseExact(launchDate, dateTimeFormat, dateTimeProvider); } set { launchDate = value.ToString(dateTimeFormat); } }
      public LaunchMode launchMode;
      public GameQuestStatus gameQuestStatus = GameQuestStatus.Locked;
      public int currentProgress;

      public QuestProgress(string _gameQuestID, System.DateTime _launchDate, LaunchMode _gameQuestLaunchMode, GameQuestStatus _gameQuestStatus)
      {
        gameQuestId = _gameQuestID;
        LaunchDate = _launchDate;
        launchMode = _gameQuestLaunchMode;
        gameQuestStatus = _gameQuestStatus;
      }

      public QuestProgress(string _gameQuestID, GameQuestStatus _gameQuestStatus)
      {
        gameQuestId = _gameQuestID;
        launchMode = LaunchMode.Immediate;
        LaunchDate = DateTime.Now;
        gameQuestStatus = _gameQuestStatus;
      }

      public bool Equals(QuestProgress other)
      {
        return other.gameQuestId.Equals(gameQuestId);
      }
    }

    /// <summary>
    /// Reference title of quest that the player has unlocked (used as key to find the quest), completed or activated
    /// </summary>
    [SerializeField]
    List<QuestProgress> questProgressList = new List<QuestProgress>();
    [SerializeField]
    protected string lastCheckDate;
    public DateTime LastCheckedDate { get { return String.IsNullOrEmpty(lastCheckDate) ? DateTime.Now : DateTime.ParseExact(lastCheckDate, dateTimeFormat, dateTimeProvider); } set { lastCheckDate = value.ToString(dateTimeFormat); } }
    public override void onDataCreated(string dataVersion)
    {
      base.onDataCreated(dataVersion);
      dataName = "GameQuestSavedData";
    }

    /// <summary>
    /// Get the saved quest progress<br/>
    /// Can filter on not complete nor locked quests, and create right away a quest on the locked status
    /// </summary>
    /// <param name="gameQuestId"></param>
    /// <param name="includeComplete"></param>
    /// <param name="includeLocked"></param>
    /// <param name="createIfNull"></param>
    /// <returns></returns>
    public QuestProgress GetQuestProgress(string gameQuestId, bool includeComplete = true, bool includeLocked = true, bool createIfNull = true)
    {
      QuestProgress questProgress = questProgressList.Find(quest => quest.gameQuestId == gameQuestId && (includeLocked ? true : quest.gameQuestStatus != GameQuestStatus.Locked) && (includeComplete ? true : quest.gameQuestStatus != GameQuestStatus.Complete));
      if (questProgress == null && createIfNull)
      {
        questProgress = new QuestProgress(gameQuestId, GameQuestStatus.Locked);
        questProgress = SetQuestProgress(questProgress);
      }
      return questProgress;
    }

    /// <summary>
    /// Add the progress to the saved data list, or copy data in the existing one and returns it to keep only one reference
    /// </summary>
    /// <param name="questProgress"></param>
    /// <returns></returns>
    public QuestProgress SetQuestProgress(QuestProgress questProgress)
    {
      if (!questProgressList.Contains(questProgress))
      {
        questProgressList.Add(questProgress);
        return questProgress;
      }
      // Here, that means that we can have a questProgress with the same game quest id but not the same data in it (two created objects, can happen with serialization)
      QuestProgress existingQuestProgress = questProgressList.Find(progress => progress.gameQuestId.Equals(questProgress.gameQuestId));
      // Copy data
      existingQuestProgress.currentProgress = questProgress.currentProgress;
      existingQuestProgress.gameQuestStatus = questProgress.gameQuestStatus;
      existingQuestProgress.LaunchDate = questProgress.LaunchDate;
      existingQuestProgress.launchMode = questProgress.launchMode;
      return existingQuestProgress;
    }

    public override void onPlayerDataLoaded()
    {
      base.onPlayerDataLoaded();
      // Link quest progress and quest info for faster usage in the game
      for (int i = 0; i < questProgressList.Count; i++)
      {
        GameQuestManager.Instance.GameQuestCatalog.GetQuest(questProgressList[i].gameQuestId).runtimeQuestProgress = questProgressList[i];
      }
    }
  }
}
