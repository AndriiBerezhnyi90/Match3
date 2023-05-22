using UnityEngine;
using System.Collections.Generic;

public sealed class BoardGenerator : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private List<BaseFruit> _fruitTemplates;
    [SerializeField] private Cell _cellTemplate;
    [SerializeField] private float _moveSpeed;

    public void Create(out Dictionary<Vector2, Cell> grid, out int width, out int height)
    {
        grid = new Dictionary<Vector2, Cell>();
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
}