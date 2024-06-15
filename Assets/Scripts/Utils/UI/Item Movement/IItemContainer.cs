namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Acts both as a source and destination for moving items.
    /// If we are moving items between two containers then it
    /// is possible to swap between them.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public interface IItemContainer<T> : IItemDestination<T>, IItemSource<T> where T : class
    {
        
    }
}