using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public bool enable = true;

    [SerializeField, Tooltip("Object that will be applied the effect")] private Transform swayObject;

    [SerializeField, Tooltip("Sway effect multiplication")] private float sensibility = 1;
    [SerializeField, Range(0, 10), Tooltip("Default: 7")] private float swaySmoothness = 7f;
    [Space]
    [SerializeField, Tooltip("The min angle it will rotate")] private float clampMin = -7.5f;
    [SerializeField, Tooltip("The max angle it will rotate")] private float clampMax = 7.5f;

    PlayerInputs playerInputs;

    private float turn = 0f;

    private void Start()
    {
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    /// <summary>
    /// Will rotate the <see cref="swayObject"/> based on mouse input.
    /// </summary>
    void Update()
    {
        if (!enable) return;

        Vector2 mouseDelta = playerInputs.LookInput;

        turn = Mathf.Lerp(turn, Mathf.Clamp(mouseDelta.x * sensibility, clampMin, clampMax), swaySmoothness * Time.deltaTime);

        Vector3 rot = swayObject.localRotation.eulerAngles;
        rot.z = turn;
        swayObject.localRotation = Quaternion.Euler(rot);
    }

}
