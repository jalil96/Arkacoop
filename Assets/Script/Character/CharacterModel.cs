using System;
using Photon.Pun;
using UnityEngine;

public class CharacterModel : MonoBehaviourPun
{
    [SerializeField] private float _speed;
    
    [SerializeField] private Transform _boundaryLeft;
    [SerializeField] private Transform _boundaryRight;
    [Range(0,1)][SerializeField] private float _boundaryOffset;
    
    [SerializeField] private SpriteRenderer _sprite;

    private float _spriteOffset;

    private bool _vertical;
    
    public bool Vertical => _vertical;
    
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

}
