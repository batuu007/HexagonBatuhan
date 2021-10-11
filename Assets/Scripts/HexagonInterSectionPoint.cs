using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonInterSectionPoint : MonoBehaviour
{
    public float rotationZValue { get; set; }

    public bool hasSelected { get; set; }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var select = GameObject.Find("HexagonFrame");
            select.transform.GetChild(0).gameObject.SetActive(true);
            select.transform.position = this.gameObject.transform.position;
            select.transform.rotation = Quaternion.Euler(0, 0, rotationZValue);
            this.hasSelected = true;
        }
    }
}
