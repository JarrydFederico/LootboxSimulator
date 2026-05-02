using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains useful methods that can be called from anywhere
/// </summary>

public static class GeneralUtils
{
    public static float GetPercentage(float valueCurrent, float valueMax)
    {
        if (valueMax <= 0f)
            return 1f;

        return valueCurrent / valueMax;
    }

    public static float GetPercentage(int valueCurrent, int valueMax)
    {
        if (valueMax <= 0)
            return 1f;

        return (float)valueCurrent / valueMax;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int swapIndex = Random.Range(i, list.Count);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
    }

    public static List<TComponent> GetComponentsFromChildren<TComponent>(Transform parent) where TComponent : Component
    {   
        //Will return a list containing all references of a script
        //that is on a child of the parent object
        List<TComponent> components = new List<TComponent>();

        foreach (Transform child in parent)
        {
            TComponent component = child.GetComponent<TComponent>();
            if (component != null)
                components.Add(component);
        }

        return components;
    }

    public static T ChooseRandomAndRemove<T>(this IList<T> list)
    {
        //Select a random value from a list and remove it from that list
        if (list == null || list.Count == 0)
            throw new System.InvalidOperationException("List is empty");

        int randomIndex = Random.Range(0, list.Count);
        T value = list[randomIndex];
        list.RemoveAt(randomIndex);

        return value;
    }

    public static List<T> ChooseRandomAndRemove<T>(this IList<T> list, int count)
    {
        //Selects X number of random values from a list, each unique
        //If the list is not long enough, it will return a reduced amount
        if (list == null || list.Count == 0)
            throw new System.InvalidOperationException("List is empty");

        List<T> selectedItems = new();
        while(selectedItems.Count < count && list.Count > 0)
        {
            selectedItems.Add(list.ChooseRandomAndRemove());
        }

        return selectedItems;
    }

}
