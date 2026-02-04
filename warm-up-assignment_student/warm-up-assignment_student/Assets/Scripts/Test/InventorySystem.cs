using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    Dictionary<string, int> inventory = new Dictionary<string, int>();

    List<int> items = new List<int>();

    public int coins;
    public int potions;
    public int wood;
    public int metal;

    void Start()
    {
        items.Add(coins);
        items.Add(potions);
        items.Add(wood);
        items.Add(metal);
    }

    void Update()
    {
        inventory["Coins"] = coins;
        inventory["Potions"] = potions;
        inventory["Wood"] = wood;
        inventory["Metal"] = metal;
    }

    [Button]
    void Checker()
    {
        Debug.Log(inventory["Potions"]);
    }

    [Button]
    void AddItem()
    {
        int item = Random.Range(0, items.Count + 1);

        if (item == 1)
        {
            coins++;
        }

        if (item == 2)
        {
            potions++;
        }

        if (item == 3)
        {
            wood++;
        }

        if (item == 4)
        {
            metal++;
        }

        else
        {
            return;
        }
    }

    [Button]
    void RemoveItem()
    {
        int item = Random.Range(0, items.Count + 1);

        if (item == 1)
        {
            if (coins > 0)
            {
                coins--;
            }

            else
            {
                return;
            }
        }

        if (item == 2)
        {
            if (potions > 0)
            {
                potions--;
            }

            else
            {
                return;
            }
        }

        if (item == 3)
        {   
            if(wood > 0)
            {
                wood--;
            }

            else
            {
                return;
            }
        }

        if (item == 4)
        {
            if (metal > 0)
            {
                metal--;
            }

            else
            {
                return;
            }
        }

        else
        {
            return;
        }
    }
}
