namespace HtmlRaportGenerator.Tools.StateHelper;

public interface IPreviousState<T>
    where T : class
{
    void Load(T current);

    void Rollback();

    T? OriginalValue { get; }

    T? CurrentValue { get; }

    bool IsValueChanged();

    void Update();

    /// <summary>
    /// Implement this when IsValueChanged data is cached
    /// </summary>
    public void UpdateIsValueChanged()
    {
    }
}