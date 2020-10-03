using UnityEngine;

public class SuicideEnemy : BaseEnemy
{

    protected void Update()
    {
        Move();
    }

    protected void Move() {

        if(targetCars == null)
            return;

        // Looks for a target car.
        Car target = GetTarget();
        if(target == null)
            return;

        // Moves towards the target.
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.Translate(velocity * Time.deltaTime * dir);

    }

    protected Car GetTarget() {

        // Gets the nearest target.
        Car target = null;
        float minDist = Mathf.Infinity;
        foreach(Car car in targetCars) {

            // Checks if the car is null as it could have been destroyed after the list was created.
            if(car == null)
                continue;

            // Checks if the current car is the closest one.
            float currentDist = (Vector2.Distance(transform.position, car.transform.position));
            if (currentDist < minDist) {
                target = car;
                minDist = currentDist;
            }

        }

        return target;

    }

}
