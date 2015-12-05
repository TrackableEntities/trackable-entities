namespace Xunit
{
    /// <summary>
    /// XUnit backwards compatibility for feature generator
    /// </summary>
    /// <remarks>
    /// Delete this when generator no longer uses this class - 5/25/15
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IUseFixture<T> : Xunit.IClassFixture<T> where T : class, new()
    {
    }
}