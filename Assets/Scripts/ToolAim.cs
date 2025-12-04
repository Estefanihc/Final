using UnityEngine;

public class ToolAim : MonoBehaviour
{
    private Camera mainCam;

    [Header("Tool Settings")]
    public float radius = 1.5f; // How far the tool is from the player
    [SerializeField] private SpriteRenderer toolSpriteRenderer;

    private Transform player;

    void Start()
    {
        mainCam = Camera.main;
        player = transform.parent;

        if (toolSpriteRenderer == null)
            toolSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        RotateAroundPlayer();
        FlipSpriteBasedOnRotation();
    }

    private void RotateAroundPlayer()
    {
        if (player == null) return;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - player.position).normalized;

        // Set tool position based on radius and direction
        transform.position = player.position + direction * radius;

        // Rotate tool to face mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FlipSpriteBasedOnRotation()
    {
        if (toolSpriteRenderer == null) return;

        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180) zRotation -= 360;

        toolSpriteRenderer.flipY = !(zRotation <= 90 && zRotation >= -90);
    }
}
