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
        BaseFruit startFruit = _grid[startPosition].GetFruit();
        BaseFruit targetFruit = _grid[targetPosition].GetFruit();

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
            }
        }

        StartCoroutine(Collapse());
        matchList.Clear();
    }

    private IEnumerator Collapse()
    {
        yield return null;

        foreach (var item in _grid)
        {
            Vector2 targetPosition = item.Key + Vector2.down;

            if(CanCollapse(item.Key, targetPosition))
            {
                BaseFruit tempFruit = _grid[item.Key].GetFruit();
                _grid[targetPosition].SetNewFruit(tempFruit);
            }
        }
    }

    private bool CanCollapse(Vector2 position, Vector2 targetPosition)
    {
        return _grid.ContainsKey(targetPosition) && _grid[targetPosition].Fruit == null && _grid[position].Fruit != null;
    }
}