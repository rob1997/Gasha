using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    public Transform target;
    public float angle;
    
    private ProjectileMissileController _projectileMissileController;
    private Rigidbody _rBody;

    private void Start()
    {
        _projectileMissileController = GetComponent<ProjectileMissileController>();
        _rBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rBody.isKinematic = false;
            _projectileMissileController.Launch(angle, target.position);
        }
    }
}
