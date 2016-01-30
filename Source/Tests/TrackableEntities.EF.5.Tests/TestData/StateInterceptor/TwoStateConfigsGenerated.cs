using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TrackableEntities.EF.Tests.TestData.StateInterceptor
{
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    [SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
    public class TwoStateConfigsGenerated : GeneratedTestData<TwoStateConfigsAsArray>
    {
        public override IEnumerable<TwoStateConfigsAsArray> GetExcludedItems()
        {
            return Enumerable.Empty<TwoStateConfigsAsArray>();
        }

        public override IEnumerable<TwoStateConfigsAsArray> GetAllCombinations()
        {
            foreach (var stateConfig1 in new SingleStateConfigGenerated().GetTypedEnumerator())
            {
                foreach (var stateConfig2 in new SingleStateConfigGenerated().GetTypedEnumerator())
                {
                    yield return new TwoStateConfigsAsArray(stateConfig1, stateConfig2);
                }
            }
        }
    }
}