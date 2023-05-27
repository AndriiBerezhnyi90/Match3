using UnityEngine;
using System.Collections.Generic;

public sealed class BoardGenerator : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private List<BaseFruit> _fruitTemplates;
    [SerializeField] private Cell _cellTemplate;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _fallHeight;

    private List<Queue<BaseFruit>> _newFruits;

    public void Create(out Dictionary<Vector2, Cell> grid, out int width, out int height)
    {
        grid = new Dictionary<Vector2, Cell>();
        InitalizeNewFruits();
        width = _width;
        height = _height;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 tempPosition = new Vector2(x, y);

                var tempCell = Instantiate(_cellTemplate, tempPosition, Quaternion.identity, transform.parent);
                tempCell.name = $"( {x} , {y} )";
                tempCell.Initialize(NewFruit(tempPosition), _moveSpeed);

                grid.Add(tempPosition, tempCell);
            }
        }
    }

    public BaseFruit NewFruit(Vector2 position)
    {
        int templateIndex = Random.Range(0, _fruitTemplates.Count);
        var tempFruit = Instantiate(_fruitTemplates[templateIndex], position, Quaternion.identity);

        return tempFruit;
    }

    public void CreateNewFruit(Vector2 position)
    {
        Queue<BaseFruit> tempQueue = _newFruits[(int)position.x];
        Vector2 tempPosition = new Vector2(position.x, _height + _fallHeight + tempQueue.Count);

        tempQueue.Enqueue(NewFruit(tempPosition));
    }

    public BaseFruit SpawnFruit(Vector2 position)
    {
        var currentQueue = _newFruits[(int)position.x];

        return currentQueue.Dequeue();
    }

    private void InitalizeNewFruits()
    {
        _newFruits = new List<Queue<BaseFruit>>(_width);

        for (int i = 0; i < _width; i++)
        {
            Queue<BaseFruit> tempQueue = new Queue<BaseFruit>();
            _newFruits.Add(tempQueue);
        }
    }
}