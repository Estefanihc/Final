using System.Collections;
using UnityEngine;

public class buildSystem : MonoBehaviour
{
    public static buildSystem Instance;

    [Header("Settings")]
    [SerializeField] private float maxBuildDistance = 10f;
    [SerializeField] private float blockSizeMod = 1f;
    [SerializeField] private float buildCooldown = 0.1f;

    [Header("References")]
    [SerializeField] private GameObject player;

    private GameObject buildTemplate;
    private SpriteRenderer buildTemplateRenderer;

    private ItemScriptableObject currentBuildItem;

    public bool BuildModeOn { get; private set; } = false;
    private bool buildBlocked = false;
    private bool canPlace = true;

    [SerializeField] private LayerMask sideDetectionLayers;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!BuildModeOn || buildTemplate == null||inventory.Instance.IsInventoryOpen) return;

        UpdateTemplatePosition();
        buildBlocked = IsPlacementBlocked();
        UpdateTemplateColor();

        if (Input.GetMouseButton(0) && !buildBlocked && canPlace)
            StartCoroutine(PlaceBuildableWithCooldown());
    }

    private void UpdateTemplatePosition()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = Mathf.Round(mouseWorldPos.x / blockSizeMod) * blockSizeMod;
        float y = Mathf.Round(mouseWorldPos.y / blockSizeMod) * blockSizeMod;
        buildTemplate.transform.position = new Vector2(x, y);

        buildTemplate.GetComponent<Collider2D>().isTrigger = true;
    }

    private bool IsPlacementBlocked()
    {
        Vector2 pos = buildTemplate.transform.position;

        bool isObstructed = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, currentBuildItem.detectionLayer);
        bool tooFar = Vector2.Distance(player.transform.position, pos) > maxBuildDistance;
        bool noNeighbor = !CheckIfHasAnyNeighbor(pos);

        return isObstructed || tooFar || noNeighbor;
    }

    private void UpdateTemplateColor()
    {
        buildTemplateRenderer.color = buildBlocked ? Color.red : Color.white;
    }

    private IEnumerator PlaceBuildableWithCooldown()
    {
        PlaceBuildable();
        canPlace = false;
        yield return new WaitForSeconds(buildCooldown);
        canPlace = true;
    }

    private void PlaceBuildable()
    {
        if (inventory.Instance.getItemAmount(currentBuildItem.itemID) <= 0)
        {
            ToggleBuildMode(false);
            return;
        }

        GameObject newBuildable = Instantiate(currentBuildItem.buildPrefab, buildTemplate.transform.position, Quaternion.identity);

        if (currentBuildItem.isSolid)
        {
            newBuildable.layer = LayerMask.NameToLayer("SolidBlock");
            if (newBuildable.GetComponent<DestructibleObject>().dontUseShadowCaster == false)
                newBuildable.GetComponent<DestructibleObject>().UpdateDetection(true);
        }
        else
            newBuildable.layer = LayerMask.NameToLayer("BackingBlock");

        inventory.Instance.RmoveItemAmount(currentBuildItem.itemID, 1);
    }

    public void SetCurrentBuildable(ItemScriptableObject buildItem)
    {
        currentBuildItem = buildItem;
    }

    public void ToggleBuildMode(bool enable)
    {
        BuildModeOn = enable;

        if (buildTemplate != null)
            Destroy(buildTemplate);

        if (enable)
        {
            if (currentBuildItem == null)
            {
                Debug.LogWarning("No buildable item selected!");
                return;
            }

            buildTemplate = Instantiate(currentBuildItem.buildPrefab);
            buildTemplateRenderer = buildTemplate.GetComponent<SpriteRenderer>();
            buildTemplateRenderer.sortingOrder = 5;
        }
    }

    public void ActivateBuildMode(bool isActive) => ToggleBuildMode(isActive);

    private bool CheckIfHasAnyNeighbor(Vector2 pos)
    {
        float checkDistance = blockSizeMod; // Same size as a block

        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (var dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos + dir * checkDistance, Vector2.zero, Mathf.Infinity, sideDetectionLayers);
            if (hit.collider != null)
                return true;
        }

        return false;
    }
}
