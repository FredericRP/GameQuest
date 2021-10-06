using FredericRP.EventManagement;
using FredericRP.PlayerCurrency;
using FredericRP.StringDataList;
using TMPro;
using UnityEngine;

public class CurrencyDisplayer : MonoBehaviour
{
  [SerializeField]
  GameEvent CurrencyUpdateEvent;//<string, int>;
  [SerializeField]
  TextMeshProUGUI text;
  [SerializeField]
  [Select(PlayerCurrencyData.CurrencyList)]
  string moneyId;
  [SerializeField]
  float updateDuration = 1.6f;
  [SerializeField]
  CurrencyUpdateAnimator animator;

  int targetCount;
  float currentCount;
  float speed = 1;

  private void OnEnable()
  {
    CurrencyUpdateEvent.Listen<string, int>(CheckCurrency);
    // update right away the text from current player currency
    currentCount = 0;
    targetCount = PlayerCurrencyManager.Instance.GetCurrencyCount(moneyId);
    SetSpeed();
    text.text = "0";
  }

  private void OnDisable()
  {
    CurrencyUpdateEvent.Delete<string, int>(CheckCurrency);
  }

  private void Update()
  {
    if ((targetCount > (int)currentCount && speed > 0) || ((targetCount < (int)currentCount && speed < 0)))
    {
      currentCount += speed * Time.deltaTime;
    }
    else
    {
      currentCount = targetCount;
    }
    text.text = ((int)currentCount).ToString();
  }

  /// <summary>
  /// Calculate increment between <c>currentCount</c> to <c>targetCount</c> to reach it using <c>updateDuration</c> seconds.
  /// </summary>
  void SetSpeed()
  {
    speed = (targetCount - currentCount) / updateDuration;
  }

  private void CheckCurrency(string moneyId, int newCount)
  {
    if (this.moneyId.Equals(moneyId))
    {
      int delta = (int)(newCount - currentCount);
      targetCount = newCount;
      SetSpeed();
      animator.LaunchAnimation(delta);
    }
  }
}
