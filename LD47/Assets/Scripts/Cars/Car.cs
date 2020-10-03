using UnityEngine;

public class Car : LoopTransform, IEntity
{

    [Header("Train")]

    public Car nextCar;
    public Car previousCar;

    [Range(0,360f)]
    public float followAngle = 20.0f;

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

    [Header("Slot")]
    public Slot slot;

    public Transform slotSpawn;

    public override float Angle {
        get {
            return angle;
        }
        set {

            // Updates this car.
            angle = value;
            OnUpdateLoopTransform();

            // Updates the next car.
            if(nextCar != null)
                nextCar.Angle = value + followAngle;

        }
    }

    protected override void Awake()
    {

        // Updates to follow the previous car if there is one.
        if(previousCar != null) {
            Angle = previousCar.Angle + followAngle;
            Radius = previousCar.Radius;
            Center = previousCar.Center;
        }

        base.Awake();
    }

    protected virtual void Update() {

        // Doesn't execute if not on the correct game state.
        if(GameController.Instance != null && GameController.Instance.CurrentState != GameController.GameState.Gameplay)
            return;

        if(Input.GetButtonDown("Fire1")) {
            if(slot != null)
                slot.Use();
            else
                Debug.LogWarning("Attempting to used empty slot at car \"" + transform.name + "\"");
        }

    }

    public void Damage(float amount) {

        Health -= amount;
        OnUpdateHealth();

    }

    public void OnUpdateHealth() {

    }

}
