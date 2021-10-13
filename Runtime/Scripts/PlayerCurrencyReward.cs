using FredericRP.GameQuest;
using FredericRP.PlayerCurrency;
using FredericRP.StringDataList;
using UnityEngine;

namespace Frederic.GameQuest
{
  [CreateAssetMenu(menuName ="FredericRP/Game Quest/Reward/Player currency")]
  [System.Serializable]
  public class PlayerCurrencyReward : GameQuestReward
  {
    [SerializeField]
    [Select(PlayerCurrencyData.CurrencyList)]
    string moneyId;
    [SerializeField]
    int rewardCount = 50;

    public string MoneyId { get => moneyId; }
    public int RewardCount { get => rewardCount; }

    public override void GiveReward(int playerId = 0)
    {
      PlayerCurrencyManager.Instance.AddToCurrency(moneyId, rewardCount);
    }
  }
}