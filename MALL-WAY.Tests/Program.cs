using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MallWay;

internal static class Program
{
    private static int checks;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        checks++;
    }

    [STAThread]
    private static void Main()
    {
        try { Run(); }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FAIL after " + checks + " checks: " + ex.GetType().FullName + ": " + ex.Message);
            Environment.ExitCode = 1;
        }
    }

    private static void Run()
    {
        var app = new Application();
        var display = DisplaySettings.Current;
        display.SetLanguage(false);
        display.SetLowVision(false);
        var log = new StringWriter();
        PresentationTraceSources.DataBindingSource.Listeners.Add(new TextWriterTraceListener(log));
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
        var database = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "display-test-" + Guid.NewGuid() + ".db");
        var repository = new MapRepository(database);
        repository.Initialize();
        var settings = new KioskSettings { StartNodeId = "1-kiosk", DefaultFloor = 1, MallName = "아르온 백화점" };
        var vm = new MainViewModel(settings, repository, new RouteService(repository));
        var window = new MainWindow { DataContext = vm };
        var root = (FrameworkElement)window.Content;

        vm.SearchText = "mono";
        Check(vm.SearchResults.Count == 1 && vm.SearchResults[0].Name == "모노핏", "English search in Korean mode");
        vm.SelectPlaceCommand.Execute(vm.SearchResults.Single());
        Check(vm.IsDetailOpen && !vm.IsMapEnabled && vm.CurrentFloor == 2, "Selecting search result opens details on destination floor");
        vm.CloseDetailCommand.Execute(null);
        Check(!vm.IsDetailOpen && vm.HasSelection, "Back preserves selection");
        vm.OpenDetailCommand.Execute(null);
        vm.FindRouteCommand.Execute(null);
        Check(vm.HasRoute && !vm.IsDetailOpen && vm.CurrentFloor == 1, "Directions returns to departure map");
        var destination = vm.SelectedDestinationNodeId;
        var steps = vm.VisibleRouteSteps.Count;
        display.SetLanguage(true);
        Check(vm.SelectedDestinationNodeId == destination && vm.VisibleRouteSteps.Count == steps && vm.HasRoute, "Language switch preserves route");
        Check(vm.RouteInstructions.Last().Contains("Mono Fit entrance"), "English destination instructions");
        Check(vm.RouteInstructions.Any(s => s.Contains("Take Central elevator")), "English transfer instructions");
        Check(vm.SelectedPlace.FloorLabel == "2F · West zone", "English location");
        Check(vm.ResultsSummary == "1 result", "Localized result count");
        Check((string)app.Resources["길찾기"] == "Get directions", "Dynamic UI resource");
        Check(display.Translate("사용자 매장") == "사용자 매장", "Custom data fallback");
        vm.ChangeFloorCommand.Execute(vm.Floors.Single(f => f.Number == 2));
        Check(vm.HasRoute && vm.DestinationMarkers.Count == 1, "Destination floor retains route");
        display.SetLowVision(true);
        Check(vm.HasRoute && vm.SelectedDestinationNodeId == destination && vm.LowVision, "Low vision retains selection and route");
        Check((double)app.Resources["Font12"] >= 20 && (double)app.Resources["ButtonHeight"] >= 60, "Large text and targets");
        Check((double)app.Resources["RouteThickness"] > 5, "Thicker route");
        foreach (var english in new[] { false, true })
        foreach (var lowVision in new[] { false, true })
        {
            display.SetLanguage(english);
            display.SetLowVision(lowVision);
            foreach (var detail in new[] { false, true })
            {
                if (detail) vm.OpenDetailCommand.Execute(null); else vm.CloseDetailCommand.Execute(null);
                root.Measure(new Size(1100, 700));
                root.Arrange(new Rect(0, 0, 1100, 700));
                root.UpdateLayout();
                Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                Check(root.ActualWidth == 1100 && root.ActualHeight == 700, "Minimum window layout");
                Check(vm.IsDetailOpen == detail, "Detail state across display combinations");
            }
        }
        display.SetLanguage(false);
        display.SetLowVision(false);
        Check(vm.RouteInstructions.Last().Contains("모노핏 입구 도착"), "Korean route restored");
        Check((double)app.Resources["Font12"] == 12 && double.IsNaN((double)app.Resources["MapDisplayWidth"]), "Standard theme restored");
        vm.CloseDetailCommand.Execute(null);
        vm.ResetSearchCommand.Execute(null);
        vm.SelectedCategory = "패션";
        vm.SearchText = "leaf";
        display.SetLanguage(true);
        Check(vm.SearchResults.Count == 1 && vm.SearchResults[0].Name == "리프스튜디오" && vm.SelectedCategory == "패션", "Category and bilingual search survive switching");
        vm.SearchText = "없는 매장";
        Check(vm.HasNoResults && !vm.HasSelection && !vm.HasRoute, "No results clears stale destination");
        vm.FindFacilityCommand.Execute("화장실");
        Check(vm.SearchResults.Count == 3, "Facility shortcut in English mode");
        vm.SelectPlaceCommand.Execute(vm.Map.Places.First());
        Check(vm.IsDetailOpen && vm.HasSelection, "Map selection opens details");
        Check(string.IsNullOrWhiteSpace(log.ToString()), "WPF binding errors: " + log);
        window.Close();
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        File.Delete(database);
        Console.WriteLine("PASS: " + checks + " display and navigation checks");
    }
}
