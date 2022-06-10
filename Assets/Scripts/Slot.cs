using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool empty;
    public GameObject indicatorPrefab;
    public Controller controller;
    public Piece p;
    public GameObject indicator;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        indicator = Instantiate(indicatorPrefab, transform.position - new Vector3(0, 0.6f, 0), transform.rotation);
        indicator.transform.parent = transform;
        indicator.name = "Indicator";
        indicator.SetActive(false);
        empty = true;
    }

    private void OnTriggerStay(Collider other)
    {
        empty = false;
        p = other.GetComponent<Piece>();
    }

    private void OnTriggerExit(Collider other)
    {
        empty = true;
        p = null;
    }

    private void FixedUpdate()
    {
        if (controller.selected is null) indicator.SetActive(false);
    }
}
