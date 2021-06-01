using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileMissileController : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    
    public bool trainingMode;
    
    #region Detonation

    public delegate void Detonation(Collision collision);

    public event Detonation OnDetonation;

    private void InvokeDetonation(Collision collision)
    {
        OnDetonation?.Invoke(collision);
    }

    #endregion
    
    // cache
    private Rigidbody _rBody;

    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        transform.rotation = Quaternion.LookRotation(_rBody.velocity);
    }

    public void Launch(float launchAngle, Vector3 destination)
    {
        Vector3 origin = transform.position;

        Vector3 originXz = new Vector3(origin.x, 0.0f, origin.z);
        Vector3 destinationXz = new Vector3(destination.x, 0.0f, destination.z);
        
        // rotate the object to face the target
        transform.LookAt(destinationXz);

        float gravity = Physics.gravity.y;
        
        float distanceX = Vector3.Distance(originXz, destinationXz);
        float height = destination.y - origin.y;

        float launchAngleTan = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        float localVelocityZ = Mathf.Sqrt(gravity * distanceX * distanceX / (2.0f * (height - distanceX * launchAngleTan)) );
        float localVelocityY = launchAngleTan * localVelocityZ;

        Vector3 localVelocity = new Vector3(0f, localVelocityY, localVelocityZ);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);
        
        _rBody.velocity = globalVelocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!trainingMode)
        {
            Destroy();
        }
        
        InvokeDetonation(other);
    }

    public void Sleep()
    {
        _rBody.Sleep();
    }

    public void Destroy()
    {
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity); 
            
        Destroy(explosion, 2f);
    }
}
