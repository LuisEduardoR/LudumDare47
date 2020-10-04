using UnityEngine;

using System.Collections;

public class Flamethrower : WeaponBase
{

    [SerializeField] protected SpriteRenderer flameRenderer = null;
    [SerializeField] protected float flameDuration = 0.1f;

    protected Coroutine flameCoroutine;

    protected override void Awake() {
        
        flameRenderer.enabled = false;
        flameCoroutine = null;
        base.Awake();

    }

    protected override void Fire() { // Litteraly

        // Starts the flame effect.
        if(flameCoroutine != null)
            StopCoroutine(flameCoroutine);
        flameCoroutine = StartCoroutine(Flame());

        // Instantiates the bullet.
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        
        // Plays a sound if available.
        if(shotAudio != null && !shotAudio.isPlaying)
            shotAudio.Play();

    }

    protected IEnumerator Flame() {

        // Enables flame.
        flameRenderer.enabled = true;

        // Waits for the delay.
        float time = 0.0f;
        while(time < flameDuration) {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Disables flame.
        flameRenderer.enabled = false;
        flameCoroutine = null;

    }

}
