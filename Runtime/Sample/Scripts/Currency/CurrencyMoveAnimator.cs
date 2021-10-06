using UnityEngine;

public class CurrencyMoveAnimator : MonoBehaviour
{
  [SerializeField]
  float minSpeed = 0.1f;

  Vector2 initialPosition;
  Vector2 pauseLocalPosition;
  Vector2 targetLocalPosition;
  float endPauseTime;
  float endMoveTime;

  RectTransform rectTransform;
  bool beforePause;
  float progress;
  float speed;

  private void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
  }

  public void SetMove(Vector2 pausePosition, Vector2 targetPosition, float endPause, float endMove)
  {
    beforePause = true;
    initialPosition = rectTransform.anchoredPosition;
    pauseLocalPosition = pausePosition;
    targetLocalPosition = targetPosition;
    progress = 0;
    endPauseTime = endPause;
    endMoveTime = endMove;
    speed = (pauseLocalPosition - initialPosition).magnitude / (endPause - Time.time) / 100;
    if (speed < minSpeed)
      speed = minSpeed;
  }

  // Update is called once per frame
  void Update()
  {
    
    if (beforePause)
    {
      progress += speed * Time.deltaTime;
      rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, pauseLocalPosition, progress);
      if (progress >= 1)
      {
        rectTransform.anchoredPosition = pauseLocalPosition;
        beforePause = false;
        progress = 0;
        // switch to next move
        speed = (targetLocalPosition - pauseLocalPosition).magnitude / (endMoveTime - endPauseTime) / 100;
        if (speed < minSpeed)
          speed = minSpeed;
      }
    }
    else if (Time.time > endPauseTime)
    {
      progress += speed * Time.deltaTime;
      rectTransform.anchoredPosition = Vector2.Lerp(pauseLocalPosition, targetLocalPosition, progress);
      if (progress >= 1)
      {
        // end move
        Destroy(gameObject);
      }
    }
  }
}
