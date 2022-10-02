﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallModel : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
    
    public Collider2D LastCollision { get; set; }
    public CharacterModel LastCharacterCollision { get; set; }

    public bool HasLastCharacterCollision => LastCharacterCollision != null;
    
    private Vector2 _direction;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void InitDirection()
    {
        _direction = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.up;
        _direction = _direction.normalized;
    }
    
    public void Move()
    {
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
}
