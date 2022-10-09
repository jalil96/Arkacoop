using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BrickModel : MonoBehaviourPun, ICollisionable
{
    public Action OnBrickDestroyed = delegate {};
    public Action OnBrickHited = delegate {};
    
    [SerializeField] private int maxHits;
    [SerializeField] private int _hits;
    [SerializeField] private BoxCollider2D _collider;

    private bool _destroyed;
    
    public bool Destroyed => _destroyed;
    public int Hits => _hits;
    
    private void Awake()
    {
        _hits = maxHits;
    }
    
    public void Damage(int amount)
    {
        if (_destroyed) return;
        _hits -= amount;
        OnBrickHited?.Invoke();
        if (_hits <= 0)
        {
            // PhotonNetwork.Destroy(gameObject);
            Debug.Log($"Brick destroyed: {photonView.ViewID}");
            _destroyed = true;
            photonView.RPC(nameof(UpdateDestroyed), RpcTarget.All, _destroyed);
            OnBrickDestroyed.Invoke();
            _collider.enabled = false;
            return;
        }
        photonView.RPC(nameof(UpdateHits), RpcTarget.Others, _hits);
    }

    [PunRPC]
    private void UpdateHits(int hits)
    {
        _hits = hits;
        OnBrickHited?.Invoke();
    }

    [PunRPC]
    private void UpdateDestroyed(bool destroyed)
    {
        _destroyed = destroyed;
        Invoke(nameof(DeactivateObject), 1f);
    }

    private void DeactivateObject()
    {
        gameObject.SetActive(!_destroyed);
    }
    
}
