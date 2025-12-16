using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using pr12.Models;

namespace pr12.Models
{
    public class CarConfiguration : INotifyPropertyChanged
    {
        private CarModel _selectedModel;
        private EngineType _selectedEngine;
        private ColorOption _selectedColor;
        private ObservableCollection<ExtraOption> _selectedOptions = new ObservableCollection<ExtraOption>();
        private decimal _totalPrice;
        private CreditParameters _creditParameters = new CreditParameters();
        private CustomerInfo _customerInfo = new CustomerInfo();

        public CarModel SelectedModel
        {
            get => _selectedModel;
            set { _selectedModel = value; OnPropertyChanged(); RecalculatePrice(); }
        }

        public EngineType SelectedEngine
        {
            get => _selectedEngine;
            set { _selectedEngine = value; OnPropertyChanged(); RecalculatePrice(); }
        }

        public ColorOption SelectedColor
        {
            get => _selectedColor;
            set { _selectedColor = value; OnPropertyChanged(); RecalculatePrice(); }
        }

        public ObservableCollection<ExtraOption> SelectedOptions
        {
            get => _selectedOptions;
            set { _selectedOptions = value; OnPropertyChanged(); RecalculatePrice(); }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        public string TotalPriceFormatted => $"{TotalPrice:N0} ₽";

        public CreditParameters CreditParameters
        {
            get => _creditParameters;
            set { _creditParameters = value; OnPropertyChanged(); }
        }

        public CustomerInfo CustomerInfo
        {
            get => _customerInfo;
            set { _customerInfo = value; OnPropertyChanged(); }
        }

        private void RecalculatePrice()
        {
            decimal price = 0;

            if (SelectedModel != null)
                price += SelectedModel.BasePrice;

            if (SelectedEngine != null)
                price += SelectedEngine.ExtraCost;

            if (SelectedColor != null)
                price += SelectedColor.ExtraCost;

            if (SelectedOptions != null)
                price += SelectedOptions.Where(o => o.IsSelected).Sum(o => o.Cost);

            TotalPrice = price;
            CreditParameters.CarPrice = price;
        }

        public decimal GetOptionsTotal()
        {
            return SelectedOptions?.Where(o => o.IsSelected).Sum(o => o.Cost) ?? 0;
        }

        public string GetOptionsTotalFormatted()
        {
            return $"{GetOptionsTotal():N0} ₽";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}