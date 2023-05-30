using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public sealed class Board : MonoBehaviour
{
    [SerializeField] private BoardCreator _boardCreator;
    [SerializeField] private MatchHandler _matchHandler;

    private bool _hasMatch;
    private bool _canSwipe;
    private Dictionary<Vector2, Cell> _grid;
    private WaitForSeconds _swipeBackDelay;

    private void Awake()
    {
        _boardCreator.New(out _grid, out _swipeBackDelay);
        _matchHandler.Initialize(_grid);
        _canSwipe = true;
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
        if (_grid.ContainsKey(targetPosition) && _canSwipe && _matchHandler.AreFruitsHome)
        {
            _hasMatch = false;
            _canSwipe = false;
            SwitchFruits(startPosition, targetPosition);

            StartCoroutine(SwipeBack(startPosition, targetPosition));
        }
    }

    private void SwitchFruits(Vector2 startPosition, Vector2 targetPosition)
    {
        BaseFruit startFruit = _grid[startPosition].RemoveFruit();
        BaseFruit targetFruit = _grid[targetPosition].RemoveFruit();

        _grid[startPosition].SetNewFruit(targetFruit);
        _grid[targetPosition].SetNewFruit(startFruit);
    }

    private void OnHasMatch(List<Vector2> _matches, bool isStartFind)
    {
        if (isStartFind)
        {
            foreach (var position in _matches)
            {
                var currentCell = _grid[position];
                _boardCreator.ReplaceFruit(currentCell);
            }
        }
        else
        {
            _hasMatch = true;

            foreach (var position in _matches)
            {
                _grid[position].DestroyFruit();
                _boardCreator.PutFruitToSpawn(position);
            }

            StartCoroutine(Collapse());
        }
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
            return _boardCreator.SpawnFruit(position);
        }
    }

    private IEnumerator SwipeBack(Vector2 startPosition, Vector2 targetPosition)
    {
        yield return _swipeBackDelay;

        if(_hasMatch == false)
        {
            SwitchFruits(startPosition, targetPosition);
        }

        _canSwipe = true;
    }
}