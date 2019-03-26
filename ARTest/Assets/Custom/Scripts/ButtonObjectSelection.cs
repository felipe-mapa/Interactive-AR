using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonObjectSelection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Interactable interactableToSpawn;

    private LayerMask mask;
    private Vector3 initialPosition;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private RaycastHit hit;
    private Button button;
    private List<Material> previewMaterials = new List<Material>();

    private void Awake()
    {
        button = GetComponent<Button>();
        mask = LayerMask.GetMask("Terrain");
        initialPosition = interactableToSpawn.transform.position;

        PopulatePreviewMaterials();
    }

    private void Update()
    {
        UpdatePreviewMaterial();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FlagObjectForPlacement();
    }
   
    public void OnDrag(PointerEventData eventData)
    {
        RaycastFindTerrain();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hit.transform && DetectOverlap.overlaps.Count == 0)
        {
            SpawnCopy();
        }

        RestoreAvailability();
    }

    private void FlagObjectForPlacement()
    {
        SetObjectLayer(interactableToSpawn.transform, "Overlay");

        button.interactable = false;

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

    // public void OnDrag(PointerEventData eventData) {
    //     MoveAlongOverlay();
    // }

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
        if (Physics.Raycast(ray, out hit, 50.0f, mask))
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

        CopyMaterialsToNewIntsanceWithDefaultColour(t);
        Destroy(t.GetComponent<DetectOverlap>());
    }

    private void RestoreAvailability()
    {
        RecolourMaterialsInHierarchy(Color.white);
        hit = new RaycastHit();

        SetObjectLayer(interactableToSpawn.transform, "Overlay");

        button.interactable = true;
        interactableToSpawn.transform.position = initialPosition;
    }

    // Create a separate list of materials we can mess with when toggling between green/red for free or overlapped geometry.
    private void PopulatePreviewMaterials()
    {
        Renderer[] renderers = interactableToSpawn.gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                previewMaterials.Add(m);
            }
        }
    }

    // Colour our materials red or green if our dragged object is above terrain, otherwise colour it white.
    private void UpdatePreviewMaterial()
    {
        if (previewMaterials == null || previewMaterials.Count <= 0) { return; }

        if (hit.transform)
        {
            // Colour our objects red if the preview position overlaps with another object, otherwise green.
            RecolourMaterialsInHierarchy((DetectOverlap.overlaps.Count > 0) ? Color.red : Color.green);
        }
        else
        {
            RecolourMaterialsInHierarchy(Color.white);
        }
    }

    // Just a method to summarize what happens when recolouring our materials.
    private void RecolourMaterialsInHierarchy(Color _colour)
    {
        foreach (Material m in previewMaterials)
        {
            m.color = _colour;
        }
    }

    // In amongst the preview colours, our materials can get a little destroyed.
    // When spawning a copy of our object, we also transfer our untainted, original material properties to the copy.
    private void CopyMaterialsToNewIntsanceWithDefaultColour(Transform _transform)
    {
        Renderer[] renderers = _transform.gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            for (int i = 0; i < r.materials.Length; i++)
            {
                r.materials[i] = new Material(r.materials[i]);
                r.materials[i].color = Color.white;
            }
        }
    }
}
