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
        _direction = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)) * Vector2.up;
        _direction = _direction.normalized;
    }
    
    public void Move()
    {
        _rigidbody.velocity = _direction * _speed;
    }
    
    public void ChangeDirection(bool vertical, bool flip)
    {

        if (flip && vertical)
        {
            _direction = new Vector2(_direction.x , _direction.y);
        } 
        else if (!flip && vertical)
        {
            _direction = new Vector2(_direction.x * -1, _direction.y);   
        }
        else if (flip && !vertical)
        {
            _direction = new Vector2(_direction.x, _direction.y);
        }
        else 
        {
            _direction = new Vector2(_direction.x, _direction.y * -1);
        }
        
        // if (flip)
        // {
        //     _direction = new Vector2(vertical ? -1 : 1 * _direction.x, -1 * _direction.y);
        // }
        // else
        // {
        //     _direction = new Vector2(vertical ? -1 : 1 * _direction.x, _direction.y);
        // }
    }
}