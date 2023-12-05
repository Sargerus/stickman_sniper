using DWTools;
using UnityEngine;

public interface IBulletView: IPooledItem<IBulletView>
{
    GameObject gameObject { get; }
    void Push();
}

[RequireComponent(typeof(Rigidbody))]
public class BulletView : MonoBehaviour, IBulletView
{
    [SerializeField] private float _force = 3f;

    private Rigidbody _rb;

    public IBulletView Item => this;
    public IPool<IBulletView> Pool { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Push()
    {
        _rb.AddForce(new Vector3(-1, 0.2f, -0.2f) * _force);
    }

    private void Update()
    {
        transform.Rotate(0.5f, 1f, 0.2f, Space.Self);
    }

    public void ReturnToPool()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        gameObject.SetActive(false);
        Pool.Add(Item);
    }
}
