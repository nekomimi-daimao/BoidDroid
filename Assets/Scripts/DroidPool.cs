using UniRx.Toolkit;
using UnityEngine;

public class DroidPool : ObjectPool<DroidState>
{
    private DroidState _prefab;
    private Transform _parent;


    private DroidPool(DroidState prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    // コンストラクタ隠蔽.趣味.
    static DroidPool CreatePool(DroidState prefab, Transform parent)
    {
        return new DroidPool(prefab, parent);
    }

    protected override DroidState CreateInstance()
    {
        return GameObject.Instantiate(_prefab, _parent).GetComponent<DroidState>();
    }
    
    
}
