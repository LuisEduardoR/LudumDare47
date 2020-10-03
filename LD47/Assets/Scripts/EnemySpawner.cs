using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    protected bool started;
    protected LevelWaves levelWaves;

    protected int currentWave;
    protected float waveTimer;


    protected void Awake() {

        started = false;

    }

    public void Initialize(LevelWaves levelWaves) {

        this.levelWaves = levelWaves;
        
        // Sets to start first wave.
        started = true;
        currentWave = -1;
        waveTimer = 0.0f;

    }

    public void FixedUpdate() {

        // Spawn the waves.
        if(started) {

            if(currentWave == -1 || (currentWave < levelWaves.waves.Count && waveTimer >= levelWaves.waves[currentWave].delayToNext)) {

                currentWave++;

                if(currentWave >= levelWaves.waves.Count) {

                    // TODO: win the level when out of enemies
                    Debug.LogWarning("Level won!");
                    return;
                    
                }

                StartCoroutine(SpawnWave(currentWave));
                waveTimer = 0.0f;

            } else
                waveTimer += Time.fixedDeltaTime;

        }

    }

    protected IEnumerator SpawnWave(int num) {

        Wave wave = levelWaves.waves[num];

        // Start spawning enemeis.
        float timer = wave.delayBetweenEnemies;
        int currentEnemy = 0;
        while(currentEnemy < wave.enemies.Count) {

            // Waits for the timer and spawns an enemy.
            timer -= Time.fixedDeltaTime;
            if(timer <= 0) {
                SpawnEnemy(wave, currentEnemy);
                timer = wave.delayBetweenEnemies;
                currentEnemy++;
            }

            yield return new WaitForFixedUpdate();

        }

    }

    protected void SpawnEnemy(Wave wave, int current) {

        // TODO: random positions.
        Instantiate(wave.enemies[current], new Vector3(5.0f, 0.0f), new Quaternion());

    }

}
