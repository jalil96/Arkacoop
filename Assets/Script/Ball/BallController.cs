using System;
using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    [SerializeField] private float _minBounceAngle;
    [SerializeField] private float _maxBounceAngle;
    [SerializeField] private int _pointsOnCollision = 50;
    [SerializeField] private int _pointsOnBreak = 100;
    [SerializeField] private float _spawningDuration = 2f;

    [SerializeField] private Vector2 _lastPosition;
    [SerializeField] private float _minTraveledDistance;
    [SerializeField] private float _traveledDistance;
    
    private BallModel _ballModel;
    
    
    private void Awake()
    {
        if (!photonView.IsMine) Destroy(this);
    }

    private void Start()
    {
        _ballModel = GetComponent<BallModel>();
        _ballModel.InitDirection();
        Invoke(nameof(StopSpawning), _spawningDuration);
        // InvokeRepeating(nameof(CheckTraveledDistance), 0, 3f);
        InvokeRepeating(nameof(CheckStuck), 0, 3f);
    }

    private void Update()
    {
        if (_ballModel.Spawning) return;
        _ballModel.Move();
        _traveledDistance += Vector2.Distance(transform.position, _lastPosition);
        _lastPosition = transform.position;
    }

    private void CheckStuck()
    {
        if (_traveledDistance <= _minTraveledDistance && !_ballModel.Spawning)
        {
            _ballModel.LastCollision = null;
            _ballModel.ReflectDirection();
        }
        _traveledDistance = 0;
    }
    
    private void StopSpawning()
    {
        _ballModel.StopSpawning();
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!photonView.IsMine) return;
        if (_ballModel.LastCollision != null && col.collider == _ballModel.LastCollision) return;
        var angle = 0f;

        var collisionable = col.gameObject.GetComponent<ICollisionable>();
        if (collisionable == null) return;
        
        var character = col.gameObject.GetComponent<CharacterModel>();
        if (character != null)
        {
            angle = CalculateAngleFromCollision(character, col);
            _ballModel.LastCharacterCollision = character;
        }

        var brick = col.gameObject.GetComponent<BrickModel>();
        if (brick != null && _ballModel.HasLastCharacterCollision)
        {
            var points = brick.Hits == 1 ? _pointsOnBreak : _pointsOnCollision;
            _ballModel.LastCharacterCollision.AddScore(points);
            brick.Damage(1);
        }
        
        if (_ballModel != null) _ballModel.ChangeDirection(col.contacts[0], angle);
        _ballModel.LastCollision = col.collider;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var wallOfDeath = col.gameObject.GetComponent<CharacterWallOfDeath>();
        if (wallOfDeath)
        {
            _ballModel.Die();
        }
    }

    private float CalculateAngleFromCollision(CharacterModel character, Collision2D col)
    {
        var topLeft = character.GetTopLeft();
        var topRight = character.GetTopRight();

        var distanceLeft = Vector2.Distance(col.contacts[0].point, topLeft);
        var distanceRight = Vector2.Distance(col.contacts[0].point, topRight);

        var dir = distanceLeft < distanceRight ? 1 : -1;
            
        var distance = distanceLeft < distanceRight ? distanceLeft : distanceRight;

        var center = character.GetTopCenter();
            
        // Debug.Log($"Distance between center and left/right {distance}");
            
        var distanceCenter = Vector2.Distance(distanceLeft < distanceRight ? topLeft : topRight , center);

        return Mathf.Lerp(_maxBounceAngle, _minBounceAngle, distance / distanceCenter) * dir;
    }

}