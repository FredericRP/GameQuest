using FredericRP.PersistentData;
using UnityEngine;

namespace FredericRP.GameQuest
{
  public class ClearPlayerProgress : MonoBehaviour
  {
    public void ClearPlayerData()
    {
      PersistentDataSystem.Instance.EraseAllSavedData<GameQuestSavedData>(PersistentDataSystem.SaveType.Player);
    }
  }
}