using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public UnityEvent<MaterialData> OnSelectedMaterialChanged = new UnityEvent<MaterialData>();

    private MaterialData _selectedMaterial;
    public MaterialData SelectedMaterial
    {
        get { return _selectedMaterial; }
        private set
        {
            if (_selectedMaterial != value)
            {
                _selectedMaterial = value;
                OnSelectedMaterialChanged.Invoke(_selectedMaterial);
                Debug.Log($"Inventory: Selected {_selectedMaterial?.materialName ?? "None"}");
            }
        }
    }

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

    public void SelectMaterial(MaterialData newMaterial)
    {
        SelectedMaterial = newMaterial;
    }
}
