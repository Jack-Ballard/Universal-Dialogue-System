using System;
using System.ComponentModel;

namespace Honours_Stage_Project.Models
{
    public class AttributeModel : INotifyPropertyChanged
    {
        private int _id;
        private string _value = string.Empty;
        private string _name = string.Empty;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public object Export() => new { ID = Id, Value, Name };
    }
}
