using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public sealed class MatchHandler : MonoBehaviour
{
    private Dictionary<Vector2, Cell> _grid;
    private List<Vector2> _match;

    public UnityAction<List<Vector2>> Match;

    public void Initialize( Dictionary<Vector2, Cell> grid)
    {
        _grid = grid;
        _match = new List<Vector2>();
    }

    private void Start()
    {
        foreach (var item in _grid)
        {
            item.Value.FruitHome += OnFruitHome;
        }
    }

    private void OnDisable()
    {
        foreach (var item in _grid)
        {
            item.Value.FruitHome -= OnFruitHome;
        }
    }

    private void OnFruitHome()
    {
        bool allFruitsHome = true;

        foreach (var item in _grid)
        {
            if(item.Value.IsFruitHome == false)
            {
                allFruitsHome = false;
            }
        }

        if (allFruitsHome)
        {
            FindAll();
        }
    }

    private void FindAll()
    {
        foreach (var item in _grid)
        {
            Horizontal(item.Key);
            Vertical(item.Key);
        }
    }

    private void Horizontal(Vector2 position)
    {
        _match.Add(position);
        FindAt(position, Direction.Left);
        FindAt(position, Direction.Right);
        CheckMatch(_match);
    }

    private void Vertical(Vector2 position)
    {
        _match.Add(position);
        FindAt(position, Direction.Up);
        FindAt(position, Direction.Down);
        CheckMatch(_match);
    }

    private void FindAt(Vector2 position,Direction direction)
    {
        Vector2 targetPosition = position;

        switch (direction)
        {
            case Direction.Up:
                targetPosition += Vector2.up;
                break;
            case Direction.Down:
                targetPosition += Vector2.down;
                break;
            case Direction.Left:
                targetPosition += Vector2.left;
                break;
            case Direction.Right:
                targetPosition += Vector2.right;
                break;
        }

        if (_grid.ContainsKey(targetPosition))
        {
            if(_grid[targetPosition].Fruit == _grid[position].Fruit)
            {
                _match.Add(targetPosition);
                FindAt(targetPosition, direction);
            }
        }
    }

    private void CheckMatch(List<Vector2> match)
    {
        if (_match.Count >= 3)
        {
            Match?.Invoke(_match);
        }

        _match.Clear();
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}