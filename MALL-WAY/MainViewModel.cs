using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Data.Sqlite;

namespace MallWay
{
    public sealed class MainViewModel : ViewModelBase
    {
        private readonly KioskSettings settings;
        private readonly MapRepository repository;
        private readonly List<MapPlace> directory;
        private FloorMap map;
        private int currentFloor;
        private string searchText = "";
        private string selectedCategory = "전체";
        private List<MapPlace> searchResults;
        private MapPlace selectedPlace;
        private string errorMessage;

        public string MallName { get; }
        public List<FloorInfo> Floors { get; }
        public List<string> Categories { get; }
        public string FloorTitle => $"{currentFloor}F  {Map.Name}";
        public string KioskFloorLabel => $"{Map.KioskFloor}층 안내 키오스크";
        public int CurrentFloor => currentFloor;
        public FloorMap Map => map;
        public List<MapPlace> SearchResults => searchResults;
        public string ResultsSummary => $"전체 층 검색 결과 {SearchResults.Count}곳";
        public bool HasNoResults => SearchResults.Count == 0;
        public MapPlace SelectedPlace => selectedPlace;
        public bool HasSelection => SelectedPlace != null;
        public string SelectedDestinationNodeId => SelectedPlace?.EntranceNodeId;
        public string ErrorMessage => errorMessage;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (Set(ref searchText, value ?? ""))
                    RefreshResults();
            }
        }

        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                if (Set(ref selectedCategory, value ?? "전체"))
                    RefreshResults();
            }
        }

        public RelayCommand<FloorInfo> ChangeFloorCommand { get; }
        public RelayCommand<MapPlace> SelectPlaceCommand { get; }
        public RelayCommand<string> FindFacilityCommand { get; }
        public RelayCommand ResetSearchCommand { get; }

        public MainViewModel(KioskSettings settings, MapRepository repository)
        {
            this.settings = settings;
            this.repository = repository;
            MallName = settings.MallName;
            Floors = repository.ReadFloors();
            directory = repository.ReadDirectory();
            Categories = new[] { "전체" }.Concat(directory.Select(place => place.Category).Distinct()).ToList();
            map = repository.ReadFloor(settings.DefaultFloor, settings.StartNodeId);
            currentFloor = settings.DefaultFloor;
            ChangeFloorCommand = new RelayCommand<FloorInfo>(floor =>
            {
                if (floor != null && TryShowFloor(floor.Number))
                    SetSelection(null);
            });
            SelectPlaceCommand = new RelayCommand<MapPlace>(place =>
            {
                if (place != null && TryShowFloor(place.Floor))
                    SetSelection(place);
            });
            FindFacilityCommand = new RelayCommand<string>(name => SetFilters(name, "편의시설"));
            ResetSearchCommand = new RelayCommand(() => SetFilters("", "전체"));
            RefreshResults();
        }

        private bool TryShowFloor(int floor)
        {
            try
            {
                var nextMap = repository.ReadFloor(floor, settings.StartNodeId);
                map = nextMap;
                currentFloor = floor;
                Set(ref errorMessage, null, nameof(ErrorMessage));
                RaisePropertyChanged(nameof(Map));
                RaisePropertyChanged(nameof(CurrentFloor));
                RaisePropertyChanged(nameof(FloorTitle));
                RaisePropertyChanged(nameof(KioskFloorLabel));
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is SqliteException)
            {
                Set(ref errorMessage, "지도를 불러오지 못했습니다. 다시 선택해 주세요.", nameof(ErrorMessage));
                return false;
            }
        }

        private void SetFilters(string text, string category)
        {
            Set(ref searchText, text ?? "", nameof(SearchText));
            Set(ref selectedCategory, category, nameof(SelectedCategory));
            RefreshResults();
        }

        private void RefreshResults()
        {
            var query = SearchText.Trim();
            searchResults = directory.Where(place =>
                (SelectedCategory == "전체" || place.Category == SelectedCategory)
                && place.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (SelectedPlace != null && !searchResults.Any(place =>
                place.EntranceNodeId == SelectedDestinationNodeId))
                SetSelection(null);
            RaisePropertyChanged(nameof(SearchResults));
            RaisePropertyChanged(nameof(ResultsSummary));
            RaisePropertyChanged(nameof(HasNoResults));
        }

        private void SetSelection(MapPlace place)
        {
            selectedPlace = place;
            foreach (var item in directory.Concat(Map.Places))
                item.IsSelected = item.EntranceNodeId == SelectedDestinationNodeId;
            RaisePropertyChanged(nameof(SelectedPlace));
            RaisePropertyChanged(nameof(HasSelection));
            RaisePropertyChanged(nameof(SelectedDestinationNodeId));
        }
    }
}
