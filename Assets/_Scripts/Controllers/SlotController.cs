using System; // Actionを使うために必要
using UnityEngine;
using UnityEngine.EventSystems; // UIのクリックイベントを取得するために必要
using UnityEngine.UI;

public class SlotController : MonoBehaviour, IPointerClickHandler
{
    // このスロットがクリックされたことを外部に通知するためのイベント
    public event Action<SlotController> OnSlotClicked;

    [SerializeField]
    private Image slotImage; // InspectorからスロットのImageコンポーネントをセットする

    public MaterialData CurrentMaterial { get; private set; }

    private void Awake()
    {
        // 念のため、slotImageが設定されていなければ自分自身のImageコンポーネントを探す
        if (slotImage == null)
        {
            slotImage = GetComponent<Image>();
        }
    }

    // 物質をこのスロットにセットし、見た目を更新する
    public void SetMaterial(MaterialData newMaterial)
    {
        CurrentMaterial = newMaterial;
        slotImage.color = newMaterial.materialColor;
    }

    // このスロットを空にする
    public void ClearSlot()
    {
        CurrentMaterial = null;
        // 半透明の濃い灰色に戻す
        slotImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    }

    // このスロットがクリックされたときにUnityが自動で呼び出すメソッド
    public void OnPointerClick(PointerEventData eventData)
    {
        // イベントを発行し、自分自身（このSlotController）を渡す
        OnSlotClicked?.Invoke(this);
    }
}
