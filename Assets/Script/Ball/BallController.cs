using System;
using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    [SerializeField] private float _minBounceAngle;
    [SerializeField] private float _maxBounceAngle;
    [SerializeField] private int _pointsOnCollision = 50;
    [SerializeField] private int _pointsOnBreak = 100;
    
    private BallModel _ballModel;
    
    private void Awake()
    {
        if (!photonView.IsMine) Destroy(this);
    }

    private void Start()
    {
        _ballModel = GetComponent<BallModel>();
        _ballModel.InitDirection();
    }

    private void Update()
    {
        _ballModel.Move();
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