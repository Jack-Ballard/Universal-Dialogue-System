using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace Honours_Stage_Project.Models
{
    public class ConditionModel : INotifyPropertyChanged
    {
        private int _id;
        private string _value = string.Empty;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public object Export() => new { ID = Id, Value };

        public void Import(object data)
        {
            var attributeData = (Newtonsoft.Json.Linq.JObject)data;
            Id = (int)attributeData["ID"];
            Value = (string)attributeData["Value"];
        }
    }
}
