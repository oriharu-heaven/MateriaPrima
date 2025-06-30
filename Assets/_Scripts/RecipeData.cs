using System.Collections.Generic; // Listを使うために必要
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeData", menuName = "Materia Prima/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public List<MaterialData> ingredients; // 反応に必要な材料のリスト
    public MaterialData result; // 反応後の生成物
}
