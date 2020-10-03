using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotFitInfo", menuName = "ScriptableObjects/Slot Fit Info", order = 1)]
public class SlotFitInfo : ScriptableObject
{
    public string id;

    public int cost;
    public GameObject prefab;

}