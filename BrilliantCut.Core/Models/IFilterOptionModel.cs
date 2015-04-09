namespace BrilliantCut.Core.Models
{
    public interface IFilterOptionModel
    {
        string Id { get; }
        string Text { get; }
        object Value { get; }
        object DefaultValue { get; }
        int Count { get; }
    }
}
