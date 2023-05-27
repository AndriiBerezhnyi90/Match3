using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

        _matchHandler.HasMatch += OnHasMatch;
    }

    private void OnDisable()
    {
        foreach (var item in _grid)
        {
            item.Value.Swipe -= OnSwipe;
        }

        _matchHandler.HasMatch -= OnHasMatch;
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
        BaseFruit startFruit = _grid[startPosition].RemoveFruit();
        BaseFruit targetFruit = _grid[targetPosition].RemoveFruit();

        _grid[startPosition].SetNewFruit(targetFruit);
        _grid[targetPosition].SetNewFruit(startFruit);
    }

    private void OnHasMatch(List<List<Vector2>> matchList)
    {
        foreach (var match in matchList)
        {
            foreach (var position in match)
            {
                _grid[position].Destroy();
                _boardGenerator.CreateNewFruit(position);
            }
        }

        StartCoroutine(Collapse());
    }

    private IEnumerator Collapse()
    {
        yield return null;

        foreach (var item in _grid)
        {
            if(item.Value.Fruit == null)
            {
                var upFruit = UpFruit(item.Key);
                item.Value.SetNewFruit(upFruit);
            }
        }
    }

    private BaseFruit UpFruit(Vector2 position)
    {
        Vector2 upPosition = position + Vector2.up;

        if (_grid.ContainsKey(upPosition))
        {
            if(_grid[upPosition].Fruit == null)
            {
                return UpFruit(upPosition);
            }
            else
            {
                return _grid[upPosition].RemoveFruit();
            }
        }
        else
        {
            return _boardGenerator.SpawnFruit(position);
        }
    }
}