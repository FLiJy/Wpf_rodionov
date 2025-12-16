using System.ComponentModel;
using System.Text.RegularExpressions;

namespace pr12.Models
{
    public class CustomerInfo : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name = "";
        private string _phone = "";
        private string _email = "";

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _phone = Regex.Replace(value, @"[^\d]", "");
                else
                    _phone = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string PhoneFormatted
        {
            get
            {
                if (string.IsNullOrEmpty(Phone)) return "";
                if (Phone.Length == 11)
                    return $"+7 ({Phone.Substring(1, 3)}) {Phone.Substring(4, 3)}-{Phone.Substring(7, 2)}-{Phone.Substring(9, 2)}";
                return Phone;
            }
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Name) && Name.Length >= 2 &&
            !string.IsNullOrWhiteSpace(Phone) && Phone.Length >= 10 &&
            !string.IsNullOrWhiteSpace(Email) && Email.Contains("@") && Email.Contains(".");

        public string Error => this[nameof(Name)] ?? this[nameof(Phone)] ?? this[nameof(Email)];

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrWhiteSpace(Name))
                            return "Введите имя";
                        if (Name.Length < 2)
                            return "Минимум 2 символа";
                        break;

                    case nameof(Phone):
                        if (string.IsNullOrWhiteSpace(Phone))
                            return "Введите телефон";
                        if (!Regex.IsMatch(Phone, @"^\d+$"))
                            return "Только цифры";
                        if (Phone.Length < 10)
                            return "Минимум 10 цифр";
                        break;

                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                            return "Введите email";
                        if (!Email.Contains("@") || !Email.Contains("."))
                            return "Неверный формат email";
                        break;
                }
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}