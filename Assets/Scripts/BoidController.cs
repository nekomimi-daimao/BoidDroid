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

    public const int DroidCount = 20;
    private const int Threshold = 2;
    private const float Distance = 2.0f;

    [SerializeField] private BoidDroid _boidPrefab;
    private BoidsPool _droidPool;
    private UniTask<Unit> _preload;

    private readonly List<BoidDroid> _boids = new List<BoidDroid>();

    [SerializeField] private Transform target;

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
        var group = BoidGroup.Create(_droidPool, target);
        group.Ready();

        this.UpdateAsObservable().Subscribe((unit => group.CalcBoidsAverage()));
        this.FixedUpdateAsObservable().Subscribe((unit => { group.Calc(); }));
    }
}
