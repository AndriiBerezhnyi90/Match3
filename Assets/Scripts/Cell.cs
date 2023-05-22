using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class Cell : MonoBehaviour
{
    private BaseFruit _fruit;

    public void Initialize(BaseFruit fruit)
    {
        _fruit = fruit;
        _fruit.transform.SetParent(this.transform);
    }
}