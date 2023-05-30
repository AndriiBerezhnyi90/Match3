using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public sealed class DestroyEffect : MonoBehaviour
{
    [SerializeField] private float _destroyDelay;

    private ParticleSystemRenderer _particleSystemRender;

    private void Awake()
    {
        _particleSystemRender = GetComponent<ParticleSystemRenderer>();
        Destroy(gameObject, _destroyDelay);
    }

    public void SetMaterial(Material material)
    {
        _particleSystemRender.material = material;
    }
}