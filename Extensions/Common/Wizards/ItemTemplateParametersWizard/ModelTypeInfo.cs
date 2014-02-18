using System;

namespace ItemTemplateParametersWizard
{
    [Serializable]
    public class ModelTypeInfo
    {
        public string DisplayName { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public ModelType ModelType { get; set; }
    }
}
