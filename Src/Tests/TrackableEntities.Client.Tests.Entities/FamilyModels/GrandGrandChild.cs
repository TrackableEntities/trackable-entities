using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.FamilyModels
{
    [JsonObject]
    public class GrandGrandChild : ModelBase<GrandGrandChild>
    {
        public GrandGrandChild() { }
        public GrandGrandChild(string name)
        {
            _name = name;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyPropertyChanged(m => m.Name);
            }
        }
    }
}
