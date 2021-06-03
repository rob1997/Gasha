using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Transform[] _batteries;

    private void Start()
    {
        _batteries = new Transform[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            _batteries[i] = transform.GetChild(i);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InvokeEntry(other);
        
        GameObject guidedMissile = Instantiate(guidedMissilePrefab, 
            GetClosestBattery(other.transform.position).position, Quaternion.LookRotation(Vector3.up));

        other.GetComponent<ProjectileMissileController>().OnDetonation += collision =>
        {
            if (guidedMissile != null)
            {
                guidedMissile.GetComponent<GuidedMissileController>().Destroy();
            }
        };
        
        guidedMissile.GetComponent<SeekerAi>().target = other.transform;
    }

    private Transform GetClosestBattery(Vector3 position)
    {
        return _batteries.OrderBy(b => Vector3.Distance(b.position, position)).First();
    }
}
