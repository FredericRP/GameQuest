using FredericRP.EventManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace FredericRP.GameQuest
{
  public class Flower : MonoBehaviour, IPointerDownHandler
  {
    [Header("Links")]
    [SerializeField]
    Image coloredSprite;
    [SerializeField]
    RectTransform[] flowers;
    [Header("Sources")]
    [SerializeField]
    Sprite[] spriteList;
    [Header("Config")]
    [SerializeField]
    float rotationSpeed = 3;
    [SerializeField]
    float minSpeed = 0.2f;
    [SerializeField]
    float maxSpeed = 0.3f;
    [SerializeField]
    GameEvent grabFlowerEvent;
    [SerializeField]
    AnimationCurve accelerationCurve;
    [SerializeField]
    float verticalMinMoveSpeed = -0.2f;
    [SerializeField]
    float verticalMaxMoveSpeed = 0.2f;

    public int id { get; private set; }

    RectTransform rectTransform;
    float lifeProgress;
    Vector2 screenSize;
    float speed;
    float verticalMoveSpeed;

    private void Awake()
    {
      rectTransform = GetComponent<RectTransform>();
      screenSize = GetComponentInParent<CanvasScaler>().referenceResolution;
    }

    public void SetFlower(int id)
    {
      this.id = id;
      speed = Random.Range(minSpeed, maxSpeed);
      verticalMoveSpeed = Random.Range(verticalMinMoveSpeed, verticalMaxMoveSpeed);
      coloredSprite.sprite = spriteList[id - 1]; // ID is 1 or 2, index is 0 or 1
      // Ensure all flowers images have no rotation
      for (int i = 0; i < flowers.Length; i++)
      {
        flowers[i].localEulerAngles = Vector3.zero;
      }
    }

    private void Update()
    {
      if (rectTransform.anchoredPosition.x < screenSize.x)
      {
        lifeProgress += speed * Time.deltaTime;
        for (int i = 0; i < flowers.Length; i++)
        {
          flowers[i].Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        }
        Vector2 newPosition = rectTransform.anchoredPosition;
        newPosition.x = accelerationCurve.Evaluate(lifeProgress) * screenSize.x;
        newPosition.y += verticalMoveSpeed * (accelerationCurve.Evaluate(lifeProgress) - 0.5f) * screenSize.y * Time.deltaTime;
        rectTransform.anchoredPosition = newPosition;
      }
      else
      {
        Destroy(gameObject);
      }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
      grabFlowerEvent.Raise<int>(id);
      Destroy(gameObject);
    }
  }
}