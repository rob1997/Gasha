using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Dome : MonoBehaviour
{
    [SerializeField] private GameObject guidedMissilePrefab;

    #region Entry

    public delegate void Entry(Collider other);

    public event Entry OnEntry;

    private void InvokeEntry(Collider other)
    {
        OnEntry?.Invoke(other);
    }

    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        InvokeEntry(other);
        
        GameObject guidedMissile = Instantiate(guidedMissilePrefab);
        
        guidedMissile.GetComponent<TrackerAi>().target = other.transform;

        guidedMissile.transform.position = new Vector3(50f,2.75f,50f);
        guidedMissile.transform.rotation = Quaternion.LookRotation(Vector3.up);
    }
}
