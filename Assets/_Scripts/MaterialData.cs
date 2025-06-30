using UnityEngine;

// この行を追加することで、Unityのメニューからこのデータアセットを作成できるようになります。
[CreateAssetMenu(fileName = "New MaterialData", menuName = "Materia Prima/Material Data")]
public class MaterialData : ScriptableObject
{
    public string materialName; // 物質の名前 (例: "水")
    public Color materialColor; // 物質の色
    // public Sprite materialIcon; // 将来的にはアイコン画像も設定できるように準備
}
