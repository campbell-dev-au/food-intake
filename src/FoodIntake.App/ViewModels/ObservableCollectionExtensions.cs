using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FoodIntake.App.ViewModels;

public static class ObservableCollectionExtensions
{
    /// <summary>
    /// Clears <paramref name="collection"/> and refills it from <paramref name="items"/>
    /// </summary>
    public static void ReplaceAll<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        foreach (T item in items)
            collection.Add(item);
    }
}
