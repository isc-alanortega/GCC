namespace Nubetico.Frontend.Services.Core
{
    /// <summary>
    /// Provides functionality to move items up or down within a list and update their sequence numbers.
    /// </summary>
    public static class ListOrderingService
    {    /// <summary>
         /// Moves an item one position up in the list and updates the sequence numbers of all items.
         /// </summary>
         /// <typeparam name="T">The type of the items in the list.</typeparam>
         /// <param name="item">The item to move up.</param>
         /// <param name="listItem">The list of items where the item should be moved.</param>
         /// <returns>A list with the item moved up and sequence updated.</returns>
        public static List<T> MoveItemUp<T>(T item, List<T> listItem) where T : class
        {
            if (listItem.Count <= 1) return listItem;

            int index = listItem.IndexOf(item);

            // Verificamos si no es el primer elemento
            if (index == 0) return listItem;

            // Intercambiamos las posiciones con el anterior
            var temp = listItem[index - 1];
            listItem[index - 1] = listItem[index];
            listItem[index] = temp;

            return UpdateSequence(listItem);
        }

        /// <summary>
        /// Moves an item one position down in the list and updates the sequence numbers of all items.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="item">The item to move down.</param>
        /// <param name="listItem">The list of items where the item should be moved.</param>
        /// <returns>A list with the item moved down and sequence updated.</returns>
        public static List<T> MoveItemDown<T>(T item, List<T> listItem) where T : class
        {
            if (listItem.Count <= 0) return [];

            int index = listItem.IndexOf(item);

            // Verificamos si no es el último elemento
            if (index == listItem.Count - 1) return listItem;

            // Intercambiamos las posiciones con el siguiente
            var temp = listItem[index + 1];
            listItem[index + 1] = listItem[index];
            listItem[index] = temp;

            return UpdateSequence(listItem);
        }

        /// <summary>
        /// Updates the sequence property of all items in the list and orders them based on the sequence value.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="listItem">The list of items whose sequence will be updated.</param>
        /// <returns>A list with updated sequences and sorted order.</returns>
        public static List<T> UpdateSequence<T>(List<T> listItem) where T : class
        {
            int sequence = 0;
            foreach (var item in listItem)
            {
                // Ensure the 'Sequence' property exists and is writable
                var sequenceProperty = item.GetType().GetProperty("Sequence");

                if (sequenceProperty != null && sequenceProperty.CanWrite)
                {
                    // If the value of Sequence is not null, update it
                    var currentValue = sequenceProperty.GetValue(item);
                    if (currentValue != null)
                    {
                        sequenceProperty.SetValue(item, sequence + 1);
                        sequence++;
                    }
                }
            }

            return listItem.OrderBy(x =>
            {
                var sequenceValue = x.GetType().GetProperty("Sequence")?.GetValue(x);
                return sequenceValue == null ? 1 : 0;
            })
            .ThenBy(x =>
            {
                var sequenceValue = x.GetType().GetProperty("Sequence")?.GetValue(x);
                return sequenceValue;
            })
            .ToList();
        }
    }
}
