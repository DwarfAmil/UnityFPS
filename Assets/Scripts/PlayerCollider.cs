using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Debug.Log("tq");
            other.GetComponent<ItemBase>().Use(transform.parent.gameObject);
            Debug.Log("tqasd");
        }
    }
}
