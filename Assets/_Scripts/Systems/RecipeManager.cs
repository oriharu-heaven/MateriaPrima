using System.Collections.Generic;
using System.Linq; // LINQを使うために必要
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField]
    private List<RecipeData> allRecipes; // Inspectorから全てのレシピデータを設定

    // 参照する他のマネージャー
    private WorkbenchManager workbenchManager;

    private void Start()
    {
        // 他のマネージャーのインスタンスを取得
        workbenchManager = FindObjectOfType<WorkbenchManager>();
        if (workbenchManager == null)
        {
            Debug.LogError("RecipeManager: WorkbenchManager not found in the scene!");
            return;
        }

        // WorkbenchManagerのイベントを購読
        workbenchManager.OnWorkbenchUpdated.AddListener(CheckWorkbenchForRecipes);
    }

    private void OnDestroy()
    {
        // イベントの購読を解除
        if (workbenchManager != null)
        {
            workbenchManager.OnWorkbenchUpdated.RemoveListener(CheckWorkbenchForRecipes);
        }
    }

    // 作業台の状態が変わるたびに呼び出されるメソッド
    private void CheckWorkbenchForRecipes()
    {
        // 現在作業台にある物質のリストを取得
        List<MaterialData> currentMaterialsOnBench = workbenchManager
            .Slots.Where(slot => slot.CurrentMaterial != null) // 物質が入っているスロットのみを抽出
            .Select(slot => slot.CurrentMaterial) // 物質データ(MaterialData)のみを抽出
            .ToList(); // 新しいリストに変換

        if (currentMaterialsOnBench.Count < 2)
            return; // 材料が2つ未満なら判定しない

        // 登録されている全レシピと照合
        foreach (var recipe in allRecipes)
        {
            if (DoesRecipeMatch(recipe, currentMaterialsOnBench))
            {
                Debug.Log($"Recipe MATCHED!: {recipe.name}");
                ExecuteRecipe(recipe);
                return; // 一度に一つのレシピのみ実行して終了
            }
        }
    }

    // レシピが現在の材料と一致するかを判定するメソッド
    private bool DoesRecipeMatch(RecipeData recipe, List<MaterialData> currentMaterials)
    {
        // 材料の数が違う時点で不一致
        if (recipe.ingredients.Count != currentMaterials.Count)
        {
            return false;
        }

        // レシピの全ての材料が、現在の材料リストに含まれているかを確認
        // (順不同でチェック)
        return recipe.ingredients.All(ingredient => currentMaterials.Contains(ingredient));
    }

    // レシピを実行するメソッド
    private void ExecuteRecipe(RecipeData recipe)
    {
        // 材料を消費（スロットからクリア）
        foreach (var ingredient in recipe.ingredients)
        {
            var slotToClear = workbenchManager.Slots.First(slot =>
                slot.CurrentMaterial == ingredient
            );
            slotToClear.ClearSlot();
        }

        // 結果を空いている最初のスロットに配置
        var emptySlot = workbenchManager.Slots.FirstOrDefault(slot => slot.CurrentMaterial == null);
        if (emptySlot != null)
        {
            emptySlot.SetMaterial(recipe.result);
            // 再度イベントを発行して、連鎖反応の可能性に備える
            workbenchManager.OnWorkbenchUpdated.Invoke();
        }
        else
        {
            Debug.LogWarning("No empty slot to place the recipe result!");
            // ここで結果の物質をインベントリに戻すなどの処理も考えられる
        }
    }
}
