using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyUpdateAnimator : MonoBehaviour
{
  [SerializeField]
  CurrencyMoveAnimator currencyPrefab;
  [SerializeField]
  RectTransform prefabParent;
  [SerializeField]
  Rect deviationFromParent;
  [SerializeField]
  AnimationCurve prefabCount;
  [SerializeField]
  int batchPrefabCount = 6;
  [SerializeField]
  float instanciationDuration = 0.2f;
  [SerializeField]
  float displayDuration = 0.2f;
  [SerializeField]
  float pauseDuration = 0.1f;
  [SerializeField]
  float mergeDuration = 0.4f;

  int totalCount;
  float deltaBetweenBatch;
  float nextInstantiationTime;
  float endPauseTime;
  float endMoveTime;
  int batchCount;
  int currentBatch;

  public void LaunchAnimation(int currencyDelta)
  {
    // Get prefab to instantiate from currency delta amount
    totalCount = (int)prefabCount.Evaluate(currencyDelta);
    // calculate number of batches
    currentBatch = 0;
    batchCount = Mathf.CeilToInt(totalCount / batchPrefabCount);
    deltaBetweenBatch = instanciationDuration / batchCount;
    endPauseTime = Time.time + instanciationDuration + displayDuration + pauseDuration;
    endMoveTime = endPauseTime + mergeDuration;
    nextInstantiationTime = Time.time;
    enabled = true;
  }

  // Update is called once per frame
  void Update()
  {
    if (Time.time > nextInstantiationTime)
    {
      nextInstantiationTime = Time.time + deltaBetweenBatch;
      currentBatch++;
      InstantiateBatch();
    }
  }

  private void InstantiateBatch()
  {
    for (int i = 0; i < batchPrefabCount; i++)
    {
      Vector2 position = Vector2.zero;
      position.x = Random.Range(deviationFromParent.min.x, deviationFromParent.max.x);
      position.y = Random.Range(deviationFromParent.min.y, deviationFromParent.max.y);
      CurrencyMoveAnimator newGO = Instantiate<CurrencyMoveAnimator>(currencyPrefab, prefabParent);
      newGO.transform.localPosition = Vector2.zero;
      newGO.SetMove(position, Vector2.zero, endPauseTime, endMoveTime);
    }
    if (currentBatch >= batchCount)
      enabled = false;
  }
}
