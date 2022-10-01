using System;
using Photon.Pun;
using UnityEngine;

public class CharacterModel : MonoBehaviourPun, ICollisionable
{
    public Action OnDied = delegate { };
    
    [SerializeField] private float _speed;
    
    [SerializeField] private Transform _boundaryLeft;
    [SerializeField] private Transform _boundaryRight;
    [Range(0,1)][SerializeField] private float _boundaryOffset;
    
    [SerializeField] private SpriteRenderer _sprite;

    
    private float _spriteOffset;

    private bool _vertical;
    private bool _dead;
    
    public bool Vertical => _vertical;
    public bool Dead => _dead;
    public void Init(bool vertical)
    {
        _vertical = vertical;
    }
    
    public void Move(Vector2 direction)
    {
        direction = direction.normalized * (_speed * Time.deltaTime);
        
        var newPosition = transform.localPosition + new Vector3(direction.x, direction.y);

        _spriteOffset = _vertical ? _sprite.bounds.size.y / 2 : _sprite.bounds.size.x / 2;
        
        if (!(newPosition.x > _boundaryRight.localPosition.x - _spriteOffset + _boundaryOffset || 
              newPosition.x < _boundaryLeft.localPosition.x + _spriteOffset - _boundaryOffset))
            transform.localPosition = newPosition;
        
        // transform.Translate(direction.x, direction.y, transform.position.z);
    }

    public Vector2 GetTopLeft()
    {
        return _sprite.transform.TransformPoint(new Vector3(_sprite.sprite.bounds.min.x, _sprite.sprite.bounds.max.y, 0));
    }

    public Vector2 GetTopRight()
    {
        return _sprite.transform.TransformPoint(_sprite.sprite.bounds.max);
    }

    public Vector2 GetTopCenter()
    {
        return Vector2.Lerp(GetTopLeft(), GetTopRight(), .5f);
    }

    public void Die()
    {
        _dead = true;
        photonView.RPC(nameof(UpdateDead), RpcTarget.All, _dead);
        OnDied?.Invoke();
    }

    [PunRPC]
    private void UpdateDead(bool dead)
    {
        _dead = dead;
        gameObject.SetActive(!_dead);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTopLeft(), .1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetTopRight(), .1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(GetTopCenter(), .1f);

    }
    
}
