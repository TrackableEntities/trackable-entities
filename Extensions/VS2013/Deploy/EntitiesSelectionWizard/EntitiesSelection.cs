using System;

namespace EntitiesSelectionWizard
{
    [Flags]
    public enum EntitiesSelection
    {
        None = 0,
        Client = 1,
        Service = 2,
        Shared = 4,
        Portable = 8,
        DotNet45 = 16
    }
}
