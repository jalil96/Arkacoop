using UnityEngine;

public class BallModel : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
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
    
    public void ChangeDirection(bool vertical)
    {
        _direction = vertical ? new Vector2(_direction.x * -1, _direction.y) : new Vector2(_direction.x, _direction.y * -1);
    }
}