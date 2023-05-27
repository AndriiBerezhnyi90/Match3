using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public sealed class MatchHandler : MonoBehaviour
{
    private Dictionary<Vector2, Cell> _grid;
    private List<Vector2> _match;
    private List<List<Vector2>> _matchList;

    public UnityAction<List<List<Vector2>>> HasMatch;

    public void Initialize( Dictionary<Vector2, Cell> grid)
    {
        _grid = grid;
        _match = new List<Vector2>();
        _matchList = new List<List<Vector2>>();
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

    public bool IsAvailable(Vector2 position)
    {
        return _grid.ContainsKey(position) && _grid[position].Fruit != null && IsInMatch(position) == false;
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
        _matchList.Clear();

        foreach (var item in _grid)
        {
            Horizontal(item.Key);
            Vertical(item.Key);
        }

        HasMatch?.Invoke(_matchList);
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

        if (IsAvailable(targetPosition))
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
            var tempList = new List<Vector2>(match.Count);

            foreach (var position in match)
            {
                tempList.Add(position);
            }

            _matchList.Add(tempList);
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

    private bool IsInMatch(Vector2 position)
    {
        bool result = false;

        foreach (var list in _matchList)
        {
            if (list.Contains(position))
            {
                result = true;
            }
        }

        return result;
    }

    private IEnumerator ClearMatchList()
    {
        yield return null;

        _matchList.Clear();
    }
}