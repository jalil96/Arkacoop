using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallModel : MonoBehaviourPun
{
    public Action OnStopSpawning;
    
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
    
    public Collider2D LastCollision { get; set; }
    public CharacterModel LastCharacterCollision { get; set; }
    
    public bool HasLastCharacterCollision => LastCharacterCollision != null;
    public bool Spawning => _spawning;
    
    private Vector2 _direction;
    private bool _active = true;
    private bool _spawning;

    public Action<BallModel> OnDie = delegate { };

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (!photonView.IsMine) photonView.RPC(nameof(RequestSpawning), photonView.Owner, PhotonNetwork.LocalPlayer);
    }

    public void InitDirection()
    {
        _spawning = true;
        _direction = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.up;
        _direction = _direction.normalized;
    }
    
    public void Move()
    {
        if (_spawning) return;
        _rigidbody.velocity = _direction * _speed;
    }
    
    public void ChangeDirection(ContactPoint2D col, float angle = 0)
    {
        //_direction = vertical ? new Vector2(_direction.x * -1, _direction.y) : new Vector2(_direction.x, _direction.y * -1);
        // Debug.Log($"Direction before change {_direction}");

        _direction = angle == 0 ? Vector2.Reflect(_direction, col.normal) : col.normal; //

        // Debug.Log($"Direction after change {_direction}");
        
        _direction = _direction.Rotate(angle);
        
        // Debug.Log($"Direction before rotation {_direction} by angle: {angle}");
    }

    public void Die()
    {
        OnDie.Invoke(this);
        photonView.RPC(nameof(UpdateDie), RpcTarget.Others);
        Destroy(gameObject);
    }

    public void StopSpawning()
    {
        _spawning = false;
        photonView.RPC(nameof(UpdateSpawning), RpcTarget.All, _spawning);
    }

    [PunRPC]
    private void UpdateDie()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    private void RequestSpawning(Player player)
    {
        photonView.RPC(nameof(UpdateSpawning), player, _spawning);
    }

    [PunRPC]
    private void UpdateSpawning(bool spawning)
    {
        _spawning = spawning;
        if (!spawning) OnStopSpawning.Invoke();
    }
}
