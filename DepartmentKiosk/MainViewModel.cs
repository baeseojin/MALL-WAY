using GalaSoft.MvvmLight;

namespace DepartmentKiosk
{
    public sealed class MainViewModel : ViewModelBase
    {
        public string MallName { get; }
        public string FloorTitle { get; }
        public string KioskFloorLabel { get; }
        public FloorMap Map { get; }

        public MainViewModel(KioskSettings settings, MapRepository repository)
        {
            MallName = settings.MallName;
            Map = repository.ReadFloor(settings.DefaultFloor, settings.StartNodeId);
            FloorTitle = $"{settings.DefaultFloor}F  {Map.Name}";
            KioskFloorLabel = $"{Map.KioskFloor}층 안내 키오스크";
        }
    }
}
