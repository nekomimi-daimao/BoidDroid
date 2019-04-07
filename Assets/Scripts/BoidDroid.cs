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

    public void Position(Vector2 position)
    {
        Rigidbody.position = new Vector3(position.x, 0, position.y);
    }
}