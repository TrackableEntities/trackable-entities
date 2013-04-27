using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.FamilyModels
{
    [JsonObject]
    public class Parent : ModelBase<Parent>
    {
        public Parent() { }
        public Parent(string name)
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

        private ChangeTrackingCollection<Child> _children;
        public ChangeTrackingCollection<Child> Children
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
