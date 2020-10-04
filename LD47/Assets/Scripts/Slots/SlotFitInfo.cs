using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotFitInfo", menuName = "ScriptableObjects/Slot Fit Info", order = 1)]
public class SlotFitInfo : ScriptableObject
{
    public string id;
    public int price;
    public Sprite sprite;
    public GameObject prefab;

}