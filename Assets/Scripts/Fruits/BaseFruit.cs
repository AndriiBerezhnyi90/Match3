using UnityEngine;

public abstract class BaseFruit : MonoBehaviour
{
    [SerializeField] private Material _material;

    public Material Material => _material;
}