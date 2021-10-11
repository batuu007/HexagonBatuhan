using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public int coordinateX { get; set; }
    public int coordinateY { get; set; }
    public int _colorCode { get; set; }

    public float coordinateValueX { get; set; }
    public float coordinateValueY { get; set; }

    public bool hasBomb { get; set; }

    [SerializeField] public int x;
    [SerializeField] public int y;
    [SerializeField] public int colorCode;
    private void Start()
    {
        x = this.coordinateX;
        y = this.coordinateY;
    }
    private void Update()
    {
        x = this.coordinateX;
        y = this.coordinateY;
        colorCode = this._colorCode;
    }
}
