namespace H2DT.Management.Saving
{
    public interface ISavable
    {
        string savableId { get; }
        string savableContainer { get; }
        object saveableData { get; }
    }
}