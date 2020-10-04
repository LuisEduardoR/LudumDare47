using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BaseEnemy : MonoBehaviour, IEntity
{

    [Header("Health")]
    [SerializeField] protected float health = 100.0f;
    public virtual float Health {
        get {
            return health;
        }
        set {
            health = value;
            OnUpdateHealth();
        }
    }

    [SerializeField] protected int killValue = 100;

    [Tooltip("Prefab instantiated when this enemy dies")]
    [SerializeField] protected GameObject deathEffect = null;

    // Used to prevent "double death"
    protected bool dead;

    [SerializeField] protected float collisionDamage = 15.0f;

    [Header("Movement")]
    [SerializeField] protected float velocity = 2.0f;

    [Header("Animation")]

    [SerializeField] protected Animator enemyAnimator;

    // Used to store of a list of cars on the scene that can be used for targetting.
    protected Car[] targetCars;

    protected virtual void Start() {

        // Gets the cars on the scene.
        targetCars = FindObjectsOfType<Car>();

    }

    public virtual void Damage(float amount) {

        Health -= amount;
        OnUpdateHealth();

    }

    public virtual void OnUpdateHealth() {

        if(Health <= 0.0f && !dead) {
            Die();
            return;
        }

    }

    public virtual void Die() {

        dead = true;
        
        // Gives points and money.
        GameController.Instance.Points += killValue;
        GameController.Instance.Money += (killValue / 10);

        // Instantiates the death effect ands destroys this object
        if(deathEffect != null)
            Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);

    }

    protected void OnCollisionEnter2D(Collision2D other) {

        // If colliding with a player car does damage to it and kills the enemy
        if(other.transform.tag == "Player") {

            // Player damage.
            IEntity entity = other.gameObject.GetComponent<IEntity>();
            if(entity != null)
                entity.Damage(collisionDamage);

            // Kills the enemy.
            Damage(Health);

        }

    }

}
