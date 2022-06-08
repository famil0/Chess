using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool empty;
    public GameObject indicatorPrefab;
    public Controller controller;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        GameObject g = Instantiate(indicatorPrefab, transform.position - new Vector3(0, 0.6f, 0), transform.rotation);
        g.transform.parent = transform;
        g.name = "Indicator";
        g.SetActive(false);
        empty = true;
    }

    private void OnTriggerStay(Collider other)
    {
        empty = false;
    }

    private void OnTriggerExit(Collider other)
    {
        empty = true;
    }

    private void FixedUpdate()
    {
        if (controller.selected is null) transform.GetChild(0).gameObject.SetActive(false);
    }
}
