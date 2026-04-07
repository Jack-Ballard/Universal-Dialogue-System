using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Honours_Stage_Project.Models
{
    public class OutgoingConnectionModel
    {
        private int _id { get; set; }

        public ObservableCollection<ConditionModel> Conditions { get; } = new ObservableCollection<ConditionModel>();

        public OutgoingConnectionModel(int id = -1)
        {
            Id = id;
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }


        public object Export()
        {
            var exportedConditions = new List<object>();
            foreach (var condition in Conditions)
                exportedConditions.Add(condition.Export());

            return new
            {
                _id,
                Conditions = exportedConditions
            };
        }

        public void Import(dynamic outgoingData)
        {
            Id = outgoingData._id;
            var conditionDataList = outgoingData.Conditions as IEnumerable<dynamic> ?? new List<dynamic>();
            foreach (var conditionData in conditionDataList)
            {
                var conditionModel = new ConditionModel();
                conditionModel.Import(conditionData);
                Conditions.Add(conditionModel);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}