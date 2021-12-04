using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Randero.Utils
{
    /// <summary>
    /// Components that implement this interfaces can act as the destination for
    /// dragging a `DragItem`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDragDestination<T> where T : class
    {
        /// <summary>
        /// Update the UI and any data to reflect adding the item to this destination.
        /// </summary>
        /// <param name="item">The item type.</param>
        /// <param name="number">The quantity of items.</param>
        void AddItem(T item);
    }
}