using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public sealed class MatchHandler : MonoBehaviour
{
    private Dictionary<Vector2, Cell> _grid;
    private List<Vector2> _match;
    private List<Vector2> _matches;

    public UnityAction<List<Vector2>,bool> HasMatch;
    public bool AreFruitsHome
    {
        get
        {
            bool areFruitsHome = true;

            foreach (var item in _grid)
            {
                if (item.Value.IsFruitHome == false)
                {
                    areFruitsHome = false;
                }
            }
            return areFruitsHome;
        }
    }

    public void Initialize(Dictionary<Vector2, Cell> grid)
    {
        _grid = grid;
        _match = new List<Vector2>();
        _matches = new List<Vector2>();
    }

    private void Start()
    {
        foreach (var item in _grid)
        {
            item.Value.FruitHome += OnFruitHome;
        }

        FindAll(true);
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
        return _grid.ContainsKey(position) && _grid[position].Fruit != null;
    }

    private void OnFruitHome()
    {
        if (AreFruitsHome)
        {
            FindAll();
        }
    }

    private void FindAll(bool isStartFind = false)
    {
        _matches.Clear();

        foreach (var item in _grid)
        {
            Horizontal(item.Key);
            Vertical(item.Key);
        }

        if (_matches.Count > 0)
        {
            if (isStartFind)
            {
                HasMatch?.Invoke(_matches,isStartFind);
                FindAll(true);
            }
            else
            {
                HasMatch?.Invoke(_matches, isStartFind);
            }
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
            foreach (var position in match)
            {
                if (_matches.Contains(position) == false)
                {
                    _matches.Add(position);
                }
            }
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