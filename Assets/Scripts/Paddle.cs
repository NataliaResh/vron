using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private GameObject _raft;
    [SerializeField] private Collider _leftOfRaft;
    [SerializeField] private Collider _firstLeftOfRaft;
    [SerializeField] private Collider _secondLeftOfRaft;
    [SerializeField] private Collider _rightOfRaft;
    [SerializeField] private Collider _firstRightOfRaft;
    [SerializeField] private Collider _secondRightOfRaft;

    [SerializeField] private const float MaxDeltaTime = 1.0f;
    [SerializeField] private const float _raftDeltaSpeed = 1.0f;
    [SerializeField] private const float _raftDeltaAngle = 1.0f;

    private Raft _raftScript;

    private bool _onLeft = false;
    private bool _onRight = false;
    private bool _inLeftFirst = false;
    private bool _inRightFirst = false;
    private float _entryTime = 0;

    private void Start()
    {
        if (_raft.TryGetComponent(out Raft raftScript) == false)
        {
            Debug.LogError("Raft not specified");
        }
        _raftScript = raftScript;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == _leftOfRaft) {
            _onLeft = true;
         }

        else if (collision.collider == _rightOfRaft)
        {
            _onRight = true;
        }

        else if (collision.collider == _firstLeftOfRaft)
        {
            _inLeftFirst = true;
            _entryTime = Time.time;
        }

        else if (collision.collider == _firstRightOfRaft)
        {
            _inRightFirst = true;
            _entryTime = Time.time;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (Time.time - _entryTime > MaxDeltaTime)
            return;

        if (_onLeft)
        {
            if (_inLeftFirst == false)
                return;

            if (collision.collider == _secondLeftOfRaft)
            {
                _raftScript.speed += _raftDeltaSpeed;
                _raftScript.angle -= _raftDeltaAngle;
            }
        }

        else if (_onRight)
        {
            if (_inRightFirst == false)
                return;

            if (collision.collider == _secondRightOfRaft)
            {
                _raftScript.speed += _raftDeltaSpeed;
                _raftScript.angle += _raftDeltaAngle;
            }
        }
    }
}
