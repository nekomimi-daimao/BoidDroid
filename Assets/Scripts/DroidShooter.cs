using System;
using System.Threading;
using UnityEngine;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;

public class DroidShooter : MonoBehaviour
{
    private const int DroidCount = 400;
    private const int Threshold = 10;
    private DroidPool _droidPool;
    private UniTask<Unit> _preload;

    private const int ShootIntervalMilliSeconds = 10;
    private const int FadeTimeSecond = 10;

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [SerializeField] private DroidState _droidState;


    void Awake()
    {
        _droidPool = DroidPool.CreatePool(_droidState, this.transform);
        _preload = _droidPool.PreloadAsync(DroidCount, Threshold).ToUniTask(_cancellationTokenSource.Token);
        this.OnDestroyAsObservable().Subscribe((unit =>
        {
            _cancellationTokenSource.Cancel();
            _droidPool.Dispose();
        }));
    }

    void Start()
    {
        var task = StartedToRainAsync(_cancellationTokenSource.Token);
    }

    private async UniTask StartedToRainAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await _preload;

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Observable.Interval(TimeSpan.FromMilliseconds(ShootIntervalMilliSeconds))
            .AsUnitObservable()
            .BatchFrame(0, FrameCountType.FixedUpdate)
            .TakeUntilDestroy(this)
            .Select(l => _droidPool.Rent())
            .Do(droid =>
            {
                Shoot(
                    droid, this.gameObject.transform.position,
                    new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 5f, UnityEngine.Random.Range(-10.0f, 10.0f))
                );
            })
            .Delay(TimeSpan.FromSeconds(FadeTimeSecond))
            .Subscribe(state => { _droidPool.Return(state); });
    }


    private static void Shoot(DroidState droidState, Vector3 from, Vector3 toward)
    {
        droidState.SetPosition(from);
        droidState.Rigidbody.AddForce(toward, ForceMode.Impulse);
    }
}