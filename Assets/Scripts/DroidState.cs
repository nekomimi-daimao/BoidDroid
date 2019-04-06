using UnityEngine;

public class DroidState : MonoBehaviour
{
    [SerializeField] public Rigidbody Rigidbody;

    public void SetPosition(Vector3 position, Quaternion? rotation = null)
    {
        Rigidbody.gameObject.transform.SetPositionAndRotation(position, rotation ?? Random.rotation);
    }
}