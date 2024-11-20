using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float cooldown = 1;

    private IEnumerable<Superpower> superpowers;

    private void Awake()
    {
        superpowers = GetComponents<Superpower>();
    }

    private void Start()
    {
        StartCoroutine(ActivatePowers());
    }

    private IEnumerator ActivatePowers()
    {
        while (true)
        {
            foreach (Superpower sp in superpowers) sp.Activate();
            yield return new WaitForSeconds(cooldown);
        }
    }
}
