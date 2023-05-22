using UnityEngine;
using System.Collections.Generic;

public sealed class Board : MonoBehaviour
{
    [SerializeField] private BoardGenerator _boardGenerator;
    [SerializeField] private float _moveSpeed;

    private int _width;
    private int _height;

    private Dictionary<Vector2, Cell> _grid;

    public int Width => _width;
    public int Height => _height;

    private void Awake()
    {
        _boardGenerator.Create(out _grid, out _width, out _height);
    }
}