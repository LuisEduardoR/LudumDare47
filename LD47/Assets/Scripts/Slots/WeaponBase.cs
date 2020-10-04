using UnityEngine;

public class WeaponBase : Slot
{
   
   [Tooltip("Delay between shots of this weapon (sec)")]
    public float delayToFire = 0.5f;

    protected float currentDelay;

    [Tooltip("Place where the weapon's bullets will spawn")]
    public Transform bulletSpawn;

    [Tooltip("GameObject for the bullet")]
    public GameObject bulletPrefab;

    [Tooltip("AudioSource to play the shooting sound.")]
    public AudioSource shotAudio;

    protected void Awake() {
        currentDelay = 0.0f;
    }

    protected void FixedUpdate() {

        if(currentDelay > 0.0f)
            currentDelay -= Time.fixedDeltaTime;

    }

    public override void Use() {

        // Only fires if there's no delay remaining.
        if(currentDelay <= 0.0f) {

            // Instantiates the bullet.
            Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

            // Plays a sound if available.
            if(shotAudio != null)
                shotAudio.Play();

            // Resets the delay.
            currentDelay = delayToFire;

        }

    }

}
