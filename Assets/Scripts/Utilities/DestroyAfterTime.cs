using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5f;

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
}