using System;
using System.Windows;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace MallWay
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                var settings = KioskSettings.Load();
                var repository = new MapRepository(settings.DatabasePath);
                repository.Initialize();

                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                SimpleIoc.Default.Register(() => settings);
                SimpleIoc.Default.Register(() => repository);
                SimpleIoc.Default.Register<RouteService>();
                SimpleIoc.Default.Register<MainViewModel>();

                MainWindow = new MainWindow
                {
                    DataContext = ServiceLocator.Current.GetInstance<MainViewModel>()
                };
                MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("키오스크를 시작할 수 없습니다.\n" + ex.Message, "시작 오류",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}
