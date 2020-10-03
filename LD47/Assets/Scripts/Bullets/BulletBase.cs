using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletBase : MonoBehaviour
{

    public float damage = 5.0f;

    public float velocity = 5f;

    public float maxLifetime = 5.0f;

    protected float currentLifetime;

    protected virtual void Start()
    {

        // Initializes velocity and lifetime.
        currentLifetime = 0.0f;
        GetComponent<Rigidbody2D>().velocity = transform.up * velocity;
        
    }

    protected virtual void FixedUpdate()
    {
        
        // Destroy bullet after a certain time has passed.
        currentLifetime += Time.fixedDeltaTime;
        if(currentLifetime >= maxLifetime)
            Destroy(gameObject);

    }

    protected void OnTriggerEnter2D(Collider2D other) {

        IEntity entity = other.GetComponent<IEntity>();
        if(entity != null)
            entity.Damage(damage);

        Destroy(gameObject);

    }

}
