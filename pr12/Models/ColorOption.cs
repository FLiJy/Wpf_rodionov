using System.ComponentModel;

namespace pr12.Models
{
    public class ColorOption : INotifyPropertyChanged
    {
        private string _name;
        private decimal _extraCost;
        private string _hexColor;

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

        public string HexColor
        {
            get => _hexColor;
            set { _hexColor = value; OnPropertyChanged(); }
        }

        public string CostFormatted => _extraCost > 0 ? $"+{ExtraCost:N0} ₽" : "Бесплатно";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}