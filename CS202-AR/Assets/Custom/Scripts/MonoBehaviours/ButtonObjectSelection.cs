using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonObjectSelection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Interactable interactableToSpawn;
    public int maxCopy = 1;
    public List<MaterialSet> previewMaterialSets = new List<MaterialSet>();

    private int numCopy;
    private LayerMask mask;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private RaycastHit hit;
    private Button button;
    private bool isDragging;

    private void Awake()
    {
        button = GetComponent<Button>();
        mask = LayerMask.GetMask("Terrain");
        interactableToSpawn.gameObject.SetActive(false);

        PopulatePreviewMaterials();
    }

    private void Update()
    {
        UpdatePreviewMaterial();
    }

    private void LateUpdate()
    {
        if (!isDragging) return;

        RaycastFindTerrain();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (numCopy < maxCopy)
        {
            FlagObjectForPlacement();

            if (numCopy == (maxCopy - 1))
            {
                DisableButton();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (hit.transform && DetectOverlap.overlaps.Count == 0)
        {
            SpawnCopy();
        }
        RestoreAvailability();

    }

    private void FlagObjectForPlacement()
    {
        isDragging = true;

        SetObjectLayer(interactableToSpawn.transform, "Overlay");
        interactableToSpawn.gameObject.SetActive(true);

        MoveAlongOverlay();
    }

    private void SetObjectLayer(Transform _transform, string _layerName)
    {
        Transform[] transforms = _transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            t.gameObject.layer = LayerMask.NameToLayer(_layerName);
        }
    }

    private void MoveAlongOverlay()
    {
        SetObjectLayer(interactableToSpawn.transform, "Overlay");

        Vector3 pos = Input.mousePosition;
        pos.z = transform.forward.z - Camera.main.transform.forward.z + 3;

        interactableToSpawn.transform.position = Camera.main.ScreenToWorldPoint(pos);
        interactableToSpawn.transform.rotation = Camera.main.transform.rotation;
    }

    private void MoveAlongTerrain()
    {
        SetObjectLayer(interactableToSpawn.transform, "Default");

        spawnPosition = hit.point;
        interactableToSpawn.transform.position = spawnPosition;

        spawnRotation = Quaternion.FromToRotation(interactableToSpawn.transform.up, hit.normal) * interactableToSpawn.transform.rotation;
        interactableToSpawn.transform.rotation = spawnRotation;
    }

    private void RaycastFindTerrain()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 90.0f, mask))
        {
            MoveAlongTerrain();
        }
        else
        {
            MoveAlongOverlay();
        }
    }

    private void SpawnCopy()
    {
        Transform t = Instantiate(interactableToSpawn, hit.point, spawnRotation, CustomTrackableEventHandler.instance.transform).transform;
        t.localScale = interactableToSpawn.transform.localScale;
        SetObjectLayer(t, "Default");

        Destroy(t.GetComponent<DetectOverlap>());
        numCopy++;
        //Debug.Log(numCopy);
    }

    private void RestoreAvailability()
    {
        RecolourMaterialsInHierarchy(Color.white);

        hit = new RaycastHit();

        SetObjectLayer(interactableToSpawn.transform, "Overlay");
        interactableToSpawn.gameObject.SetActive(false);
    }

    private void DisableButton()
    {
        button.interactable = false;
        button.enabled = false;
        GetComponent<Image>().color = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5f);
    }

    //MATERIALS
    [Serializable]
    public class MaterialSet
    {
        public MaterialSet(Material _material)
        {
            name = _material.name;
            material = _material;
            color = material.color;

            if (!material.HasProperty("_Color")) {
                Debug.LogWarning(material.name);
            }
        }

        public string name;
        public Material material;
        public Color color;
    }

    // Create a separate list of materials we can mess with when toggling between green/red for free or overlapped geometry.
    private void PopulatePreviewMaterials() {
        Renderer[] renderers = interactableToSpawn.gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (Renderer r in renderers) {
            foreach (Material m in r.materials) {
                previewMaterialSets.Add(new MaterialSet(m));
            }
        }
    }

    // Colour our materials red or green if our dragged object is above terrain, otherwise colour it white.
    private void UpdatePreviewMaterial() {
        if (previewMaterialSets == null || previewMaterialSets.Count <= 0) { return; }

        if (hit.transform) {
            // Colour our objects red if the preview position overlaps with another object, otherwise green.
            RecolourMaterialsInHierarchy((DetectOverlap.overlaps.Count > 0) ? Color.red : Color.green);
        } else {
            RecolourMaterialsInHierarchy(Color.white);
        }
    }

    // Just a method to summarize what happens when recolouring our materials.
    private void RecolourMaterialsInHierarchy(Color _colour) {
        Renderer[] renderers = interactableToSpawn.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].material.color = _colour == Color.white ? previewMaterialSets[i].color : _colour;

            if (!renderers[i].material.HasProperty("_Color"))
            {
                Debug.LogWarning(renderers[i].material.name);
            }
        }
    }
}
