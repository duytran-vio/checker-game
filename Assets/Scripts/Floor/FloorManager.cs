using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulatedCell : ICloneable
{
    public Vector2Int Cell;

    public abstract object Clone();
}

public class FloorSimulated : SimulatedCell
{
    public FloorSimulated(FloorSimulated original) : this(original.Cell) { }
    public FloorSimulated(FloorManager manager) : this(manager.Cell) { }
    public FloorSimulated(Vector2Int cell)
    {
        Cell = cell;
    }
    public override object Clone()
    {
        return (object)new FloorSimulated(this);
    }

    public override string ToString()
    {
        return $"Floor [{Cell.ToString()}]";
    }
}

public class FloorManager : MonoBehaviour
{
    private Renderer _renderer;
    private Color _originalColor;
    public Vector2Int Cell;

    // Start is called before the first frame update
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.GetColor("_Color");
    }
    void Start()
    {

    }

    public void Init(int i, int j)
    {
        Cell = new Vector2Int(i, j);
    }

    public void ResetFloorColor()
    {
        ChangeFloorColor(_originalColor);
    }

    public void SelectFloor()
    {
        ChangeFloorColor(Color.green);
    }

    private void ChangeFloorColor(Color color)
    {
        _renderer.material.SetColor("_Color", color);
    }

    public void Warning()
    {
        StartCoroutine(WarningCoroutine());
    }

    IEnumerator WarningCoroutine()
    {
        ChangeFloorColor(Color.red);
        yield return new WaitForSeconds(0.5f);
        ResetFloorColor();
        yield return new WaitForSeconds(0.5f);
        ChangeFloorColor(Color.red);
        yield return new WaitForSeconds(0.5f);
        ResetFloorColor();
    }
}
