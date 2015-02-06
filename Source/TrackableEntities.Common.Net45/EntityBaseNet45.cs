using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TrackableEntities
{
    public abstract partial class EntityBase
    {
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
