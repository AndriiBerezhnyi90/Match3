using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _swipeDeathZone;
    [SerializeField] private DestroyEffect _destroyEffect;

    private BaseFruit _fruit;
    private Vector2 _startSwipePosition;
    private Vector2 _endSwipePosition;
    private float _moveSpeed;
    private bool _isFruitHome;

    public UnityAction<Vector2, Vector2> Swipe;
    public UnityAction FruitHome;

    public bool IsFruitHome => _isFruitHome;

    public Type Fruit
    {
        get
        {
            if (_fruit)
            {
                return _fruit.GetType();
            }
            else
            {
                return null;
            }
        }
    }

    private float Angle
    {
        get
        {
            Vector2 direction = _endSwipePosition - _startSwipePosition;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
    }

    private Vector2 SwipeTargetPosition
    {
        get
        {
            if(Angle >= 45 && Angle < 135)
            {
                return transform.position + Vector3.up;
            }
            else if(Angle > -135 && Angle <= -45)
            {
                return transform.position + Vector3.down;
            }
            else if(Angle < -135 || Angle > 135)
            {
                return transform.position + Vector3.left;
            }
            else
            {
                return transform.position + Vector3.right;
            }
        }
    }

    public void Initialize(BaseFruit fruit, float moveSpeed)
    {
        _fruit = fruit;
        _fruit.transform.SetParent(this.transform);
        _moveSpeed = moveSpeed;
        _isFruitHome = true;
    }

    public void DestroyFruit()
    {
        var effect = Instantiate(_destroyEffect, transform.position, Quaternion.identity, transform);
        effect.SetMaterial(_fruit.Material);
        Destroy(_fruit.gameObject);
    }

    public void Replace(BaseFruit newFruit)
    {
        Destroy(_fruit.gameObject);
        _fruit = newFruit;
        _fruit.transform.SetParent(transform);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startSwipePosition = Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _endSwipePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 direction = _endSwipePosition - _startSwipePosition;

        if (Vector2.SqrMagnitude(direction) >= _swipeDeathZone)
        {
            Swipe?.Invoke(transform.position,SwipeTargetPosition);
        }
    }

    public BaseFruit RemoveFruit()
    {
        if (_fruit)
        {
            var tempFruit = _fruit;
            _fruit.transform.SetParent(null);
            _fruit = null;

            return tempFruit;
        }
        else
        {
            return null;
        }
    }

    public void SetNewFruit(BaseFruit fruit)
    {
        if (fruit == true)
        {
            _fruit = fruit;
            _isFruitHome = false;
            StartCoroutine(Moving());
        }
    }

    private IEnumerator Moving()
    {
        while (Vector2.Distance(transform.position, _fruit.transform.position) > 0)
        {
            Vector2 tempPosition = Vector2.MoveTowards(_fruit.transform.position, transform.position, _moveSpeed * Time.deltaTime);
            _fruit.transform.position = tempPosition;
            yield return null;
        }

        _fruit.transform.SetParent(transform);
        _isFruitHome = true;
        FruitHome?.Invoke();
    }
}