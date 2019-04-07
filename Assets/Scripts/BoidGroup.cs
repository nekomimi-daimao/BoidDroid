using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidGroup
{
    private BoidsPool _pool;
    private Transform _target;

    private Vector2 _center;
    private Vector2 _velocity;

    private readonly List<BoidDroid> _boids = new List<BoidDroid>();

    private BoidGroup(BoidsPool pool, Transform target)
    {
        this._pool = pool;
        this._target = target;
    }

    public static BoidGroup Create(BoidsPool pool, Transform target)
    {
        return new BoidGroup(pool, target);
    }

    public void Ready()
    {
        for (int count = 0; count < BoidController.DroidCount; count++)
        {
            _boids.Add(_pool.Rent());
        }

        foreach (var boidDroid in _boids)
        {
            boidDroid.Position(new Vector2(Random.Range(-3, 3), Random.Range(-3, 3)));
        }
    }

    public void CalcBoidsAverage()
    {
        var center = Vector2.zero;
        var velocity = Vector3.zero;
        foreach (var boid in _boids)
        {
            center += boid.PlainPosition();
            velocity = boid.Rigidbody.velocity;
        }

        _center = center / BoidController.DroidCount;
        _velocity = velocity / BoidController.DroidCount;
    }


    public void Calc()
    {
        var target = new Vector2(_target.position.x, _target.position.z);
        foreach (var boidDroid in _boids)
        {
            var add = (target - boidDroid.PlainPosition()).normalized * Random.value;
            boidDroid.Rigidbody.AddForce(new Vector3(add.x, 0, add.y));
        }
    }
}
