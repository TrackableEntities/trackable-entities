namespace TrackableEntities.EF.Tests.TestData
{
    /// <summary>
    /// Interface pro test data items used in <see cref="Xunit.ClassDataAttribute"/>.
    /// </summary>
    public interface ITestDataItem
    {
        /// <summary>
        /// Returns test data item as array of objects.
        /// </summary>
        /// <returns></returns>
        object[] AsArray();
    }
}