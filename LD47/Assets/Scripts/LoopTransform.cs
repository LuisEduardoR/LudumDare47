using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LoopTransform : MonoBehaviour
{

    [SerializeField] protected float angle = 0.0f;
    public virtual float Angle {
        get {
            return angle;
        }
        set {
            angle = value;
            OnUpdateLoopTransform();
        }
    }

    [SerializeField] protected float radius = 1.0f;
    public virtual float Radius {
        get {
            return radius;
        }
        set {
            radius = value;
            OnUpdateLoopTransform();
        }
    }

    [SerializeField] protected Vector2 center = Vector2.zero;
    public virtual Vector2 Center {
        get {
            return center;
        }
        set {
            center = value;
            OnUpdateLoopTransform();
        }
    }

    protected void OnUpdateLoopTransform() {

        // Sets new position
        Vector3 newPos = Center;
        newPos.x += Radius * Mathf.Sin(Angle * Mathf.Deg2Rad);
        newPos.y += Radius * Mathf.Cos(Angle * Mathf.Deg2Rad);
        transform.position = newPos;

        // Sets new rotation
        Vector2 dir = new Vector2(newPos.x - Center.x, newPos.y - Center.y);
        float lookAngle = 90.0f + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.eulerAngles = new Vector3(0, 0, lookAngle);

    }

    protected virtual void Awake() {
        OnUpdateLoopTransform();
    }

}
