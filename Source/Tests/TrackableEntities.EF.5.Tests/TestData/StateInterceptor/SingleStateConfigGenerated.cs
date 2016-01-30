using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TrackableEntities.EF.Tests.TestData.StateInterceptor
{
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    [SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
    public class SingleStateConfigGenerated : GeneratedTestData<StateConfig>
    {
        public override IEnumerable<StateConfig> GetExcludedItems()
        {
            return Enumerable.Empty<StateConfig>();
        }

        public override IEnumerable<StateConfig> GetAllCombinations()
        {
            foreach (var useInterceptor in new[] { false, true })
            {
                foreach (var initState in StateHelper.GetAllInitStates())
                {
                    foreach (var finalStates in StateHelper.GetFinalStates(initState, useInterceptor))
                    {
                        yield return new StateConfig(initState, finalStates, useInterceptor);
                    }
                }
            }
        }
    }
}