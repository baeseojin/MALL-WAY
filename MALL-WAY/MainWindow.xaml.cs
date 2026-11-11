using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace MallWay
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                if (args.OldValue is MainViewModel previous) previous.PropertyChanged -= OnViewModelChanged;
                if (args.NewValue is MainViewModel next) next.PropertyChanged += OnViewModelChanged;
            };
            Closed += (sender, args) =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.PropertyChanged -= OnViewModelChanged;
                    vm.Cleanup();
                }
            };
        }

        private void OnViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(MainViewModel.IsDetailOpen)) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (((MainViewModel)DataContext).IsDetailOpen) DetailBackButton.Focus();
                else SearchInput.Focus();
            }), DispatcherPriority.Input);
        }
    }
}
