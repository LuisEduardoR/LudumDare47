using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
   
    [SerializeField] protected float timeToDestroy = 5.0f;
    private float timer;

    void Start()
    {
        timer = 0.0f;
    }

    void Update()
    {
        timer += Time.fixedDeltaTime;

        if(timer >= timeToDestroy)
            Destroy(gameObject);
    }
}
