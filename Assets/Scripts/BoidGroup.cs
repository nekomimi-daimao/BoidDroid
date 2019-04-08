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


    private int targetRatio = 4;
    private int centerRatio = 1;
    private int separateRatio = 3;
    private int averageRatio = 2;

    private float SpeedLimit = 3;

    public void Calc()
    {
        var target = new Vector2(_target.position.x, _target.position.z);
        foreach (var boidDroid in _boids)
        {
            if (Random.Range(0, 20) == 0)
            {
                boidDroid.Rigidbody.velocity = Vector3.zero;
                var force = Random.insideUnitSphere * 10f;
                force.y = 0;
                boidDroid.Rigidbody.AddForce(force, ForceMode.Impulse);
                return;
            }

            var targetForce = (target - boidDroid.PlainPosition()).normalized * targetRatio;

            var centerForce = (_center - boidDroid.PlainPosition()).normalized * centerRatio;
            var averageForce = (_velocity - boidDroid.PlainPosition()).normalized * averageRatio;

            var separateForce = Vector2.zero;
            foreach (var droidSeparate in _boids)
            {
                separateForce += droidSeparate.PlainPosition();
            }

            separateForce /= _boids.Count;
            separateForce = separateForce.normalized * separateRatio;

            var sumForce = (targetForce + centerForce + averageForce + separateForce).normalized * Random.value;

            boidDroid.Rigidbody.AddForce(new Vector3(sumForce.x, 0, sumForce.y), ForceMode.Impulse);

            var forceRatio = boidDroid.Rigidbody.velocity.sqrMagnitude / SpeedLimit;

            if (forceRatio > 1)
            {
                boidDroid.Rigidbody.velocity = ((boidDroid.Rigidbody.velocity / forceRatio) + new Vector3(_velocity.x, 0, _velocity.y)) / 2;
            }
        }
    }
}