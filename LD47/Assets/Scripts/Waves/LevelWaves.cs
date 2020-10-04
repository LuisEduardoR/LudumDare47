using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelWave", menuName = "ScriptableObjects/Waves/Level Waves", order = 2)]
public class LevelWaves : ScriptableObject
{

    public bool scalingDifficulty = false;
    public float scalingDifficultyModifier = 0.1f;

    public List<Wave> waves;

}