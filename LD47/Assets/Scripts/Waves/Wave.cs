using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "ScriptableObjects/Waves/Wave", order = 1)]
public class Wave : ScriptableObject
{

    public float delayToNext = 25.0f;

    public float delayBetweenEnemies = 1.0f;

    public List<GameObject> enemies;

}