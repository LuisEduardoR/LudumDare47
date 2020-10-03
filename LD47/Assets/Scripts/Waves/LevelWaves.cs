using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelWave", menuName = "ScriptableObjects/Waves/Level Waves", order = 2)]
public class LevelWaves : ScriptableObject
{

    public List<Wave> waves;

}