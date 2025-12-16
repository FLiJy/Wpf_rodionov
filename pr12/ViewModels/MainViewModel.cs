using System;
using System.Windows.Input;
using pr12.Models;
using pr12;
using pr12.Models;
using pr12.ViewModels;

namespace pr12.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentStep;
        private int _currentStepIndex = 0;
        private readonly CarConfiguration _configuration = new CarConfiguration();
        private bool _isExitConfirmed = false;

        public object CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            set => SetProperty(ref _currentStepIndex, value);
        }

        public CarConfiguration Configuration => _configuration;

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SubmitCommand { get; }

        public MainViewModel()
        {
            NextCommand = new RelayCommand(NextStep, CanGoNext);
            PreviousCommand = new RelayCommand(PreviousStep, CanGoPrevious);
            SubmitCommand = new RelayCommand(SubmitApplication, CanSubmit);

            NavigateToStep(0);
        }

        private void NavigateToStep(int stepIndex)
        {
            CurrentStepIndex = stepIndex;

            switch (stepIndex)
            {
                case 0:
                    CurrentStep = new Step1ViewModel(_configuration);
                    break;
                case 1:
                    CurrentStep = new Step2ViewModel(_configuration);
                    break;
                case 2:
                    CurrentStep = new Step3ViewModel(_configuration);
                    break;
                case 3:
                    CurrentStep = new Step4ViewModel(_configuration);
                    break;
                case 4:
                    CurrentStep = new Step5ViewModel(_configuration);
                    break;
                default:
                    CurrentStep = new Step1ViewModel(_configuration);
                    break;
            }
        }

        private void NextStep()
        {
            if (CurrentStepIndex == 4)
            {
                SubmitApplication();
            }
            else if (CurrentStepIndex < 4)
            {
                NavigateToStep(CurrentStepIndex + 1);
            }
        }

        private void PreviousStep()
        {
            if (CurrentStepIndex > 0)
            {
                NavigateToStep(CurrentStepIndex - 1);
            }
        }

        private void SubmitApplication()
        {
            string summary = $"Заявка оформлена!\n\n" +
                           $"Модель: {Configuration.SelectedModel?.Name}\n" +
                           $"Двигатель: {Configuration.SelectedEngine?.Name}\n" +
                           $"Цвет: {Configuration.SelectedColor?.Name}\n" +
                           $"Итоговая стоимость: {Configuration.TotalPriceFormatted}\n" +
                           $"Клиент: {Configuration.CustomerInfo.Name}\n" +
                           $"Телефон: {Configuration.CustomerInfo.PhoneFormatted}";

            System.Windows.MessageBox.Show(summary, "Заявка оформлена",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            System.Windows.Application.Current.Shutdown();
        }

        private bool CanGoNext()
        {
            if (CurrentStepIndex == 4) return true;

            return CurrentStepIndex switch
            {
                0 => Configuration.SelectedModel != null && Configuration.SelectedEngine != null,
                1 => Configuration.SelectedColor != null,
                _ => true
            };
        }

        private bool CanGoPrevious() => CurrentStepIndex > 0;

        private bool CanSubmit() => CurrentStepIndex == 4 && Configuration.CustomerInfo.IsValid;

        public bool ConfirmExit()
        {
            if (_isExitConfirmed) return true;

            var result = System.Windows.MessageBox.Show(
                "Вы уверены, что хотите выйти? Несохраненные данные будут потеряны.",
                "Подтверждение выхода",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _isExitConfirmed = true;
                return true;
            }

            return false;
        }
    }
}   