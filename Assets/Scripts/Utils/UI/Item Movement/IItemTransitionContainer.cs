namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Acts as a temporary place to put items while in movement.
    /// For example, a single-slot inventory on a cursor.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public interface IItemTransitionContainer<T> : IItemContainer<T> where T : class
    {
        
    }
}