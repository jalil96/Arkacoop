using System;
using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    [SerializeField] private float _minBounceAngle;
    [SerializeField] private float _maxBounceAngle;
    
    private BallModel _ballModel;
    private Collider2D _lastCollision;
    
    private void Start()
    {
        if (!photonView.IsMine) Destroy(this);
        _ballModel = GetComponent<BallModel>();
        _ballModel.InitDirection();
    }

    private void Update()
    {
        _ballModel.Move();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider == _lastCollision) return;
        
        var character = col.gameObject.GetComponent<CharacterModel>();
        if (character != null)
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

            var angle = Mathf.Lerp(_maxBounceAngle, _minBounceAngle, distance / distanceCenter) * dir;
            
            _ballModel.ChangeDirection(col.contacts[0], angle);

        }
        
        var brick = col.gameObject.GetComponent<BrickModel>();
        if (brick != null)
        {
            _ballModel.ChangeDirection(col.contacts[0]);
        }

        _lastCollision = col.collider;
    }

}