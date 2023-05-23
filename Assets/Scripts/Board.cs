using UnityEngine;
using System.Collections.Generic;

public sealed class Board : MonoBehaviour
{
    [SerializeField] private BoardGenerator _boardGenerator;
    [SerializeField] private MatchHandler _matchHandler;

    private int _width;
    private int _height;

    private Dictionary<Vector2, Cell> _grid;

    public int Width => _width;
    public int Height => _height;

    private void Awake()
    {
        _boardGenerator.Create(out _grid, out _width, out _height);
        _matchHandler.Initialize(_grid);
    }

    private void OnEnable()
    {
        foreach (var item in _grid)
        {
            item.Value.Swipe += OnSwipe;
        }

        _matchHandler.Match += OnMatch;
    }

    private void OnDisable()
    {
        foreach (var item in _grid)
        {
            item.Value.Swipe -= OnSwipe;
        }

        _matchHandler.Match -= OnMatch;
    }

    private void OnSwipe(Vector2 startPosition, Vector2 targetPosition)
    {
        if (_grid.ContainsKey(targetPosition))
        {
            SwitchFruits(startPosition, targetPosition);
        }
    }

    private void SwitchFruits(Vector2 startPosition, Vector2 targetPosition)
    {
        BaseFruit startFruit = _grid[startPosition].GetFruit();
        BaseFruit targetFruit = _grid[targetPosition].GetFruit();

        _grid[startPosition].SetNewFruit(targetFruit);
        _grid[targetPosition].SetNewFruit(startFruit);
    }

    private void OnMatch(List<Vector2> match)
    {
        foreach (var position in match)
        {
            _grid[position].Destroy();
        }
    }
}