using System;
using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    [SerializeField] private float _minBounceAngle;
    [SerializeField] private float _maxBounceAngle;
    
    private BallModel _ballModel;
    private Collider2D _lastCollision;

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
        if (_lastCollision != null && col.collider == _lastCollision) return;
        var angle = 0f;

        var collisionable = col.gameObject.GetComponent<ICollisionable>();
        if (collisionable == null) return;
        
        var character = col.gameObject.GetComponent<CharacterModel>();
        if (character != null)
            angle = CalculateAngleFromCollision(character, col);

        if (_ballModel != null) _ballModel.ChangeDirection(col.contacts[0], angle);
        _lastCollision = col.collider;
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