using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    private const int DroidCount = 4;
    private const int Threshold = 1;

    [SerializeField] private BoidDroid _droidState;
    private BoidsPool _droidPool;
    private UniTask<Unit> _preload;

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private BoidDroid[] BoidsChildren;

    private void Awake()
    {
        _droidPool = BoidsPool.CreatePool(_droidState, this.transform);
        _preload = _droidPool.PreloadAsync(DroidCount, Threshold).ToUniTask(_cancellationTokenSource.Token);
        this.OnDestroyAsObservable().Subscribe((unit =>
        {
            _cancellationTokenSource.Cancel();
            _droidPool.Dispose();
        }));
    }

    private void Start()
    {
        BoidsChildren = new BoidDroid[DroidCount];
        for (var count = 0; count < BoidsChildren.Length; count++)
        {
            BoidsChildren[count] = _droidPool.Rent();
        }
    }
}