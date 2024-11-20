using UnityEngine;
using UnityEngine.Events;

public class CheckAllPuzzles : MonoBehaviour
{
    private int totalPlacableItems;
    private int totalcorrect;

    public void AllCorrectItems(){
        totalPlacableItems = 0;
        totalcorrect = 0;
        foreach (Transform child in this.transform)
        {
            totalPlacableItems += 1;
            InventoryItemMatcher inventoryItemMatcher = child.GetComponent<InventoryItemMatcher>();

            if(inventoryItemMatcher.HasRightItem() == true){
                totalcorrect += 1;
            }
        }
        if(totalcorrect == totalPlacableItems){
            Debug.Log("Completou o puzzle");
        }
    }
}
