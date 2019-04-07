using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidDroid : MonoBehaviour
{
    [SerializeField] public Rigidbody Rigidbody;

    public Vector2 PlainPosition()
    {
        return new Vector2(transform.position.x, transform.position.z);
    }

    public Vector2 PlainVelocity()
    {
        return new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z);
    }

    private Vector2 velocity;
    private Vector2 accel;
    private Vector2 pos;
    private const float min = 0.1f;
    private const float max = 1f;


    void Update()
    {
        var dt = Time.deltaTime;

        velocity += accel * dt;
        var dir = velocity.normalized;
        var speed = velocity.magnitude;
        velocity = Mathf.Clamp(speed, min, max) * dir;
        pos += velocity * dt;

        var rot = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.y));
        transform.SetPositionAndRotation(pos, rot);

        accel = Vector3.zero;
    }

    void OnEnable()
    {
        transform.position = new Vector3(Random.value, Random.value, Random.value);
    }


    private Vector2 Rule1()
    {
        var where = PlainPosition();
        var neighbors = BoidController.Instance.GetNeighbors(where);
        if (neighbors.Count == 0)
        {
            return Vector2.zero;
        }

        var vec = Vector2.zero;
        foreach (var boidDroid in neighbors)
        {
            var diff = boidDroid.PlainPosition() - where;
            vec += -1 * diff.normalized * 10.0f / (diff.sqrMagnitude);
        }

        return vec / neighbors.Count;
    }

    private Vector2 Rule2()
    {
        var neighbors = BoidController.Instance.GetNeighbors(PlainPosition());

        if (neighbors.Count == 0)
        {
            return Vector2.zero;
        }

        var vec = Vector2.zero;
        foreach (var boidDroid in neighbors)
        {
            vec += boidDroid.PlainVelocity();
        }

        return (vec / neighbors.Count) - PlainVelocity();
    }

    private Vector2 Rule3()
    {
        var neighbors = BoidController.Instance.GetNeighbors(PlainPosition());

        if (neighbors.Count == 0)
            return Vector2.zero;

        var pos = Vector2.zero;

        foreach (var boidDroid in neighbors)
        {
            pos += boidDroid.PlainPosition();
        }

        return (pos / neighbors.Count) - PlainPosition();
    }
}
