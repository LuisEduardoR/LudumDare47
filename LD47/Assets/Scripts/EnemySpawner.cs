using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    protected bool started;
    protected LevelWaves levelWaves;

    protected int currentWave;
    protected float waveTimer;


    [Tooltip("Distance away from the center the enemies will spawn.")]
    [SerializeField] protected float spawnDistance = 7.5f;


    [Tooltip("Height difference from the center the enemies will spawn.")]
    [SerializeField] protected float spawnHeightDifference = 2.5f;

    protected float scalingDificulty;

    protected void Awake() {

        started = false;

    }

    public void Initialize(LevelWaves levelWaves) {

        this.levelWaves = levelWaves;
        
        // Sets to start first wave.
        started = true;
        currentWave = -1;
        waveTimer = 0.0f;

        // Calculates the modifier for scalling dificulty.
        SetScalingDificulty();

    }

    protected void SetScalingDificulty() {

        // Disables scaling dificulty.
        if(!levelWaves.scalingDifficulty) {
            scalingDificulty = 1.0f;
            return;
        }

        scalingDificulty = 1.0f + GameController.Instance.GetCurrentLevel() * levelWaves.scalingDifficultyModifier;

    }

    public void FixedUpdate() {

        // Spawn the waves.
        if(started) {

            if(currentWave == -1 || (currentWave < levelWaves.waves.Count && waveTimer >= levelWaves.waves[currentWave].delayToNext)) {

                currentWave++;

                // Tells the GameController the player can now win the level
                if(currentWave >= levelWaves.waves.Count) {
                    GameController.Instance.WinLevel();
                    return;
                }

                // Starts the new wave.
                StartCoroutine(SpawnWave(currentWave));
                waveTimer = 0.0f;

            } else
                waveTimer += scalingDificulty * Time.fixedDeltaTime;

        }

    }

    protected IEnumerator SpawnWave(int num) {

        Wave wave = levelWaves.waves[num];

        // Start spawning enemeis.
        float timer = wave.delayBetweenEnemies;
        int currentEnemy = 0;
        while(currentEnemy < wave.enemies.Count) {

            // Waits for the timer and spawns an enemy.
            timer -= scalingDificulty * Time.fixedDeltaTime;
            if(timer <= 0) {
                SpawnEnemy(wave, currentEnemy);
                timer = wave.delayBetweenEnemies;
                currentEnemy++;
            }

            yield return new WaitForFixedUpdate();

        }

    }

    protected void SpawnEnemy(Wave wave, int current) {

        // Random position for the enemy to spawn.
        Vector2 randomPos = Vector2.zero;
        // Gets a random side.
        randomPos.x = spawnDistance * ((Random.Range(0, 2) == 0) ? 1 : -1);
        // Gets a random height.
        randomPos.y = spawnHeightDifference * Random.value * ((Random.Range(0, 2) == 0) ? 1 : -1);

        Instantiate(wave.enemies[current], randomPos, new Quaternion());

    }

}
