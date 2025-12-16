using System.ComponentModel;

namespace pr12.Models
{
    public class CreditParameters : INotifyPropertyChanged
    {
        private decimal _carPrice;
        private decimal _downPaymentPercent = 20;
        private int _loanTerm = 36;
        private const decimal AnnualRate = 10.0m;

        public decimal CarPrice
        {
            get => _carPrice;
            set { _carPrice = value; OnPropertyChanged(); RecalculateCredit(); }
        }

        public decimal DownPaymentPercent
        {
            get => _downPaymentPercent;
            set
            {
                if (value < 10) value = 10;
                if (value > 90) value = 90;
                _downPaymentPercent = value;
                OnPropertyChanged();
                RecalculateCredit();
            }
        }

        public int LoanTerm
        {
            get => _loanTerm;
            set
            {
                if (value < 12) value = 12;
                if (value > 96) value = 96;
                _loanTerm = value;
                OnPropertyChanged();
                RecalculateCredit();
            }
        }

        public decimal DownPaymentAmount => CarPrice * (DownPaymentPercent / 100);
        public string DownPaymentAmountFormatted => $"{DownPaymentAmount:N0} ₽";

        public decimal LoanAmount => CarPrice - DownPaymentAmount;
        public string LoanAmountFormatted => $"{LoanAmount:N0} ₽";

        public decimal MonthlyPayment { get; private set; }
        public string MonthlyPaymentFormatted => $"{MonthlyPayment:N0} ₽";

        private void RecalculateCredit()
        {
            if (LoanTerm <= 0 || LoanAmount <= 0)
            {
                MonthlyPayment = 0;
                return;
            }

            decimal monthlyRate = AnnualRate / 100 / 12;
            double pow = System.Math.Pow((double)(1 + monthlyRate), LoanTerm);
            decimal numerator = monthlyRate * (decimal)pow;
            decimal denominator = (decimal)pow - 1;

            if (denominator == 0)
                MonthlyPayment = 0;
            else
                MonthlyPayment = LoanAmount * (numerator / denominator);

            OnPropertyChanged(nameof(DownPaymentAmount));
            OnPropertyChanged(nameof(DownPaymentAmountFormatted));
            OnPropertyChanged(nameof(LoanAmount));
            OnPropertyChanged(nameof(LoanAmountFormatted));
            OnPropertyChanged(nameof(MonthlyPayment));
            OnPropertyChanged(nameof(MonthlyPaymentFormatted));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}