using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistInBothWorlds : MonoBehaviour
{
    public GameObject counterPart;

    private bool changedByCounterPart;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (changedByCounterPart)
        {
            lastPosition = this.transform.position;
            changedByCounterPart = false;
        }

        float change = (this.transform.position - lastPosition).magnitude;
        if (change > 0.1 && !changedByCounterPart)
        {
            Vector3 movement = transform.position - lastPosition;
            counterPart.transform.position += movement;
            counterPart.GetComponent<ExistInBothWorlds>().changedByCounterPart = true;
            lastPosition = transform.position;
        }
    }
}