using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine;

public class BoidController : SingletonMonoBehaviour<BoidController>
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private const int DroidCount = 4;
    private const int Threshold = 1;
    private const float Distance = 2.0f;

    [SerializeField] private BoidDroid _boidPrefab;
    private BoidsPool _droidPool;
    private UniTask<Unit> _preload;

    private readonly List<BoidDroid> _boids = new List<BoidDroid>();

    private void Awake()
    {
        _droidPool = BoidsPool.CreatePool(_boidPrefab, this.transform);
        _preload = _droidPool.PreloadAsync(DroidCount, Threshold).ToUniTask(_cancellationTokenSource.Token);
        this.OnDestroyAsObservable().Subscribe((unit =>
        {
            _cancellationTokenSource.Cancel();
            _droidPool.Dispose();
        }));
    }

    private void Start()
    {
        Ready();
    }


    private void Update()
    {
        CalcBoids();
    }

    public List<BoidDroid> GetNeighbors(Vector2 self)
    {
        var list = new List<BoidDroid>();
        foreach (var boidDroid in _boids)
        {
            if (boidDroid.PlainPosition() == self)
            {
                continue;
            }

            if ((boidDroid.PlainPosition() - self).sqrMagnitude <= Distance)
            {
                list.Add(boidDroid);
            }
        }

        return list;
    }


    [SerializeField] private GameObject _center;

    private void CalcBoids()
    {
        var center = Vector2.zero;
        var velocity = Vector3.zero;

        foreach (var boid in _boids)
        {
            center += boid.PlainPosition();
            velocity = boid.Rigidbody.velocity;
        }

        center = center / DroidCount;
        velocity = velocity / DroidCount;
    }

    private void Ready()
    {
        for (int count = 0; count < DroidCount; count++)
        {
            _boids.Add(_droidPool.Rent());
        }
    }
}