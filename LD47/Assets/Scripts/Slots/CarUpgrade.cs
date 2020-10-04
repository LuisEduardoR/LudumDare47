using UnityEngine;

[CreateAssetMenu(fileName = "NewCarUpgrade", menuName = "ScriptableObjects/Car Upgrade", order = 2)]
public class CarUpgrade : ScriptableObject
{
    public string slotFitId;
    public int price;
    public Sprite sprite;

}