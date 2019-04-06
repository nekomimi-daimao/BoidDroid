using UniRx.Toolkit;
using UnityEngine;

public class BoidsPool : ObjectPool<BoidDroid>
{
    private BoidDroid _prefab;
    private Transform _parent;

    private BoidsPool(BoidDroid prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    // コンストラクタ隠蔽.趣味.
    public static BoidsPool CreatePool(BoidDroid prefab, Transform parent)
    {
        return new BoidsPool(prefab, parent);
    }

    protected override BoidDroid CreateInstance()
    {
        return GameObject.Instantiate(_prefab, _parent).GetComponent<BoidDroid>();
    }
}