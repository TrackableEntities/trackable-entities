using System;

namespace EntitiesSelectionWizard
{
    [Flags]
    public enum EntitiesSelection
    {
        None = 0,
        Client = 1,
        Service = 2,
        ClientService = 4,
        Shared = 8,
        Portable = 16,
        DotNet45 = 32
    }
}
