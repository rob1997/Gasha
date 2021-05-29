using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuidedMissileController : MonoBehaviour
{
    [SerializeField] private float thrust;
    [SerializeField] private float torque;
    
    [Space]
    
    [SerializeField] private GameObject explosionEffect;
    
    public bool trainingMode;
    
    private Rigidbody _rBody;

    #region Detonation

    public delegate void Detonation(Collision other);

    public event Detonation OnDetonation;

    private void InvokeDetonation(Collision other)
    {
        OnDetonation?.Invoke(other);
    }

    #endregion
    
    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

//    private void Update()
//    {
//        float x = Input.GetAxis("Horizontal");
//        float y = Input.GetAxis("Vertical");
//        
//        Launch();
//        Steer(x, y);
//    }

    public void Launch()
    {
        _rBody.velocity = transform.forward.normalized * thrust;
    }
    
    public void Steer(float x, float y)
    {
        _rBody.angularVelocity = (transform.forward * x + transform.right * y) * torque;
    }

    public void Sleep()
    {
        _rBody.Sleep();
    }

    public Vector3 GetVelocity()
    {
        return _rBody.velocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!trainingMode)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                            
            Destroy(explosion, 2f);
        }
        
        InvokeDetonation(other);
    }
}