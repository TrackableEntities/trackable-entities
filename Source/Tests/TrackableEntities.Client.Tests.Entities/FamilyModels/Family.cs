using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackableEntities.Client.Tests.Entities.FamilyModels
{
    [JsonObject]
    public class Family : ModelBase<Family>, ITrackable, IEquatable<Family>
    {
        private Parent _mother;
        private Parent _father;
        private Child _child;

        public Parent Mother
        {
            get { return _mother; }
            set
            {
                if (value == _mother) return;
                _mother = value;
                MotherChangeTracker = _mother == null ? null : new ChangeTrackingCollection<Parent>(_mother);
                NotifyPropertyChanged(m => m.Mother);
            }
        }
        private ChangeTrackingCollection<Parent> MotherChangeTracker { get; set; }


        public Parent Father
        {
            get { return _father; }
            set
            {
                if (value == _father) return;
                _father = value;
                FatherChangeTracker = _father == null ? null : new ChangeTrackingCollection<Parent> {_father};
                NotifyPropertyChanged(f => f.Father);

            }
        }
        private ChangeTrackingCollection<Parent> FatherChangeTracker { get; set; }


        public Child Child
        {
            get { return _child; }
            set
            {
                if (value == _child) return;
                _child = value;
                ChildChangeTracker = _child == null ? null : new ChangeTrackingCollection<Child> {_child};
                NotifyPropertyChanged(c => c.Child);
            }
        }
        private ChangeTrackingCollection<Child> ChildChangeTracker { get; set; }


        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        bool IEquatable<Family>.Equals(Family other)
        {
            if (EntityIdentifier != default(Guid))
                return EntityIdentifier == other.EntityIdentifier;
            return false;
        }

#pragma warning disable 414
        [JsonProperty]
        private Guid EntityIdentifier { get; set; }
        [JsonProperty]
        private Guid _entityIdentity = default(Guid);
#pragma warning restore 414
    }
}
