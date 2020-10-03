using UnityEngine;

public class MainCar : Car
{
 
    [Header("Control")]

    [Tooltip("Acceleration of the main car (deg/sec^2)")]
    public float angularAcceleration = 15.0f;

    [Tooltip("Acceleration used to reduce speed of the main car when no input was provided  (deg/sec^2)")]
    public float angularDesacceleration = 7.5f;

    [Tooltip("Current angular velocity of the car (deg/sec)")]
    public float angularVelocity;

    [Tooltip("Maximum angular velocity of the car in any direction (deg/sec)")]
    public float maxAngularVelocity = 180.0f;


    protected override void Update() {

        // Doesn't execute if not on the correct game state.
        if(GameController.Instance != null && GameController.Instance.CurrentState != GameController.GameState.Gameplay)
            return;

        // Moves the main car.
        float input = Input.GetAxis("Horizontal");

        // Accelerates the car based on input.
        if(Mathf.Abs(input) > 0.1f) {
            angularVelocity = Mathf.Clamp(angularVelocity + (angularAcceleration * (-input) * Time.deltaTime), -maxAngularVelocity, maxAngularVelocity);
        
        // Gradually stops the car when there's no input.
        } else if(angularVelocity != 0.0f) {
            angularVelocity = Mathf.Clamp(angularVelocity - Mathf.Sign(angularVelocity) * (angularDesacceleration * Time.deltaTime), -maxAngularVelocity, maxAngularVelocity);
            if(Mathf.Abs(angularVelocity) < 0.5f)
                angularVelocity = 0.0f;
        }
        Angle += angularVelocity * Time.deltaTime;

        base.Update();

    }

}
