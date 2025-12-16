using System.ComponentModel;

namespace pr12.Models
{
    public class EngineType : INotifyPropertyChanged
    {
        private string _name;
        private decimal _extraCost;
        private string _description;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public decimal ExtraCost
        {
            get => _extraCost;
            set { _extraCost = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string CostFormatted => $"+{ExtraCost:N0} ₽";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}