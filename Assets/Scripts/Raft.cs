using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Raft : MonoBehaviour
{
    public float speed = 0;
    public float angle = 0;
    [SerializeField] private Collider _leftOfRaft;
    [SerializeField] private Collider _rightOfRaft;

    private Rigidbody _rdRaft;

    private void Start()
    {
        _rdRaft = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        _rdRaft.AddForce(transform.forward * speed * Time.deltaTime);
    }


}
