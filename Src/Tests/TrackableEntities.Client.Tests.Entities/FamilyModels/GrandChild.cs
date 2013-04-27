using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.FamilyModels
{
    [JsonObject]
    public class GrandChild : ModelBase<GrandChild>
    {
        public GrandChild() { }
        public GrandChild(string name)
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

        private ChangeTrackingCollection<GrandGrandChild> _children;
        public ChangeTrackingCollection<GrandGrandChild> Children
        {
            get { return _children; }
            set
            {
                if (Equals(value, _children)) return;
                _children = value;
                NotifyPropertyChanged(m => m.Children);
            }
        }
    }
}
