using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorkbenchManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Transform slotsParent; // スロットを生成する親オブジェクト

    [SerializeField]
    private GameObject slotPrefab; // スロットのプレハブ

    [Header("Settings")]
    [SerializeField]
    private int workbenchSize = 9; // 3x3=9

    // 作業台の状態が更新されたときに発行されるイベント
    public UnityEvent OnWorkbenchUpdated = new UnityEvent();

    // 生成した全てのスロットを管理するリスト
    public List<SlotController> Slots { get; private set; } = new List<SlotController>();

    private void Start()
    {
        CreateWorkbench();
        // InventoryManagerのイベントを購読する
        InventoryManager.Instance.OnSelectedMaterialChanged.AddListener(OnSelectedMaterialChanged);
    }

    private void OnDestroy()
    {
        // このオブジェクトが破棄されるときに、イベントの購読を解除する（メモリリーク防止）
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSelectedMaterialChanged.RemoveListener(
                OnSelectedMaterialChanged
            );
        }
    }

    private void CreateWorkbench()
    {
        for (int i = 0; i < workbenchSize; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent);
            SlotController slot = slotGO.GetComponent<SlotController>();
            slot.ClearSlot(); // 初期状態をクリアに設定
            slot.OnSlotClicked += HandleSlotClicked; // 各スロットのクリックイベントを購読
            Slots.Add(slot);
        }
    }

    // スロットがクリックされたときに呼び出されるメソッド
    private void HandleSlotClicked(SlotController clickedSlot)
    {
        MaterialData selectedMaterial = InventoryManager.Instance.SelectedMaterial;

        // 何か物質を選択中、かつクリックされたスロットが空の場合
        if (selectedMaterial != null && clickedSlot.CurrentMaterial == null)
        {
            clickedSlot.SetMaterial(selectedMaterial);
            OnWorkbenchUpdated.Invoke(); // 作業台が更新されたことを通知
        }
        // クリックされたスロットに既に何かある場合
        else if (clickedSlot.CurrentMaterial != null)
        {
            clickedSlot.ClearSlot();
            OnWorkbenchUpdated.Invoke(); // 作業台が更新されたことを通知
        }
    }

    // 選択中の物質が変わったときに呼び出されるメソッド（今は何もしないが、将来の拡張用）
    private void OnSelectedMaterialChanged(MaterialData material)
    {
        // 例えば、選択中の物質に合わせてカーソルのアイコンを変えるなどの処理をここに追加できる
    }

    // 全てのスロットを空にする公開メソッド
    public void ClearAllSlots()
    {
        foreach (var slot in Slots)
        {
            slot.ClearSlot();
        }
        OnWorkbenchUpdated.Invoke();
    }
}
