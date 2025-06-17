
// using System.Collections;
// using UnityEngine;

// public class CaseOpener : MonoBehaviour
// {
//     // 物品定义 ID
//     private readonly ulong[] itemDefIDs = { 1001, 1002, 1003 }; // 示例 ID

//     // 物品掉率（百分比）
//     private readonly int[] itemProbabilities = { 70, 20, 10 }; // 示例概率

//     void Start()
//     {
//         // 初始化 Steamworks
//         if (SteamAPI.Init())
//         {
//             Debug.Log("Steam Initialized Successfully");
//         }
//         else
//         {
//             Debug.LogError("Steam Initialization Failed");
//         }
//     }

//     void OnApplicationQuit()
//     {
//         // 关闭 Steamworks
//         SteamAPI.Shutdown();
//     }

//     public void OpenCase()
//     {
//         // 获取随机数
//         int randomValue = Random.Range(0, 100);

//         // 计算掉落物品的索引
//         int cumulativeProbability = 0;
//         int selectedItemIndex = 0;
//         for (int i = 0; i < itemProbabilities.Length; i++)
//         {
//             cumulativeProbability += itemProbabilities[i];
//             if (randomValue < cumulativeProbability)
//             {
//                 selectedItemIndex = i;
//                 break;
//             }
//         }

//         // 获取选中的物品定义 ID
//         ulong selectedItemDefID = itemDefIDs[selectedItemIndex];

//         // 添加物品到玩家库存
//         AddItemToInventory(selectedItemDefID);

//         // 显示结果
//         Debug.Log($"You got item with ID: {selectedItemDefID}");
//     }

//     private void AddItemToInventory(ulong itemDefID)
//     {
//         // 创建物品
//         SteamInventoryResult_t result;
//         bool success = SteamInventory.CreateResult(out result);

//         if (success)
//         {
//             // 添加物品到库存
//             SteamInventory.AddItem(result, itemDefID);
//             Debug.Log($"Item with ID {itemDefID} added to inventory.");
//         }
//         else
//         {
//             Debug.LogError("Failed to create inventory result.");
//         }
//     }
// }
