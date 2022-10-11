using FredericRP.EventManagement;
using FredericRP.ObjectPooling;
using FredericRP.StringDataList;
using UnityEngine;

namespace FredericRP.GameQuest
{
  public class FlowerSpawner : MonoBehaviour
  {
    [Header("Spawn config")]
    [SerializeField]
    [Select("PoolId")]
    string ObjectPoolId;
    [SerializeField]
    [Select("ObjectId")]
    string flowerPrefabName;
    [SerializeField]
    int startBurst = 10;
    [SerializeField]
    float intervalBetwenSpawn = 0.2f;
    [SerializeField]
    int spread = 350;
    [Header("Events")]
    [SerializeField]
    GameEvent flowerGrabEvent;

    public float IntervalBetwenSpawn { get { return intervalBetwenSpawn; } }

    float nextSpawn;

    // Start is called before the first frame update
    void Start()
    {
      for (int i = 0; i < startBurst; i++)
        SpawnFlower();

      nextSpawn = intervalBetwenSpawn;
    }

    private void OnEnable()
    {
      flowerGrabEvent.Listen<int>(FlowerGrabbed);
    }

    private void OnDisable()
    {
      flowerGrabEvent.Delete<int>(FlowerGrabbed);
    }

    void FlowerGrabbed(int id)
    {
      // Respawn a new flower when grabbing one
      SpawnFlower();
    }

    private void Update()
    {
      if (Time.time > nextSpawn)
      {
        nextSpawn = Time.time + intervalBetwenSpawn;
        SpawnFlower();
      }
    }

    void SpawnFlower()
    {
      Flower flower = ObjectPool.GetObjectPool(ObjectPoolId).GetFromPool<Flower>(flowerPrefabName);
      // spawn on the side of the screen
      flower.GetComponent<RectTransform>().anchoredPosition = Vector2.zero + Vector2.up * Random.Range(-spread, spread);
      // set color and target position
      flower.SetFlower(Random.Range(0, 40) > 25 ? 1 : 2);
    }
  }
}