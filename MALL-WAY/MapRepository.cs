using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using GalaSoft.MvvmLight;

namespace MallWay
{
    public sealed class MapRepository
    {
        private readonly string databasePath;

        public MapRepository(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public void Initialize()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
            using (var connection = Open(SqliteOpenMode.ReadWriteCreate))
            using (var transaction = connection.BeginTransaction(deferred: false))
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "PRAGMA user_version;";
                var version = Convert.ToInt32(command.ExecuteScalar());
                if (version == 0)
                {
                    var script = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "initialize.sql");
                    command.CommandText = File.ReadAllText(script) + "\nPRAGMA user_version = 1;";
                    command.ExecuteNonQuery();
                }
                else if (version != 1)
                {
                    throw new InvalidDataException("지원하지 않는 지도 DB 버전입니다.");
                }

                transaction.Commit();
            }
        }

        public FloorMap ReadFloor(int floor, string startNodeId)
        {
            using (var connection = Open(SqliteOpenMode.ReadOnly))
            using (var command = connection.CreateCommand())
            {
                command.Parameters.AddWithValue("$floor", floor);
                command.Parameters.AddWithValue("$start", startNodeId);
                command.CommandText = "SELECT Name FROM Floors WHERE Number = $floor;";
                var name = command.ExecuteScalar() as string;
                if (name == null)
                    throw new InvalidDataException("설정에 지정한 층이 지도 DB에 없습니다.");

                command.CommandText = "SELECT Floor FROM Nodes WHERE Id = $start AND Kind = 'kiosk';";
                var kioskFloor = command.ExecuteScalar();
                if (kioskFloor == null)
                    throw new InvalidDataException("설정에 지정한 키오스크 위치가 지도 DB에 없습니다.");

                var map = new FloorMap { Name = name, KioskFloor = Convert.ToInt32(kioskFloor) };
                map.Places.AddRange(ReadPlaces(connection, floor));

                command.CommandText = @"SELECT Id, X, Y, Kind, Label FROM Nodes
                    WHERE Floor = $floor AND (Kind IN ('elevator', 'escalator') OR Id = $start);";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var kind = reader.GetString(3);
                        map.Markers.Add(new MapMarker
                        {
                            X = reader.GetInt32(1), Y = reader.GetInt32(2),
                            IsKiosk = kind == "kiosk",
                            Label = kind == "kiosk" ? "현재 위치" : reader.GetString(4)
                        });
                    }
                }

                return map;
            }
        }

        public List<FloorInfo> ReadFloors()
        {
            var floors = new List<FloorInfo>();
            using (var connection = Open(SqliteOpenMode.ReadOnly))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Number, Name FROM Floors ORDER BY Number;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        floors.Add(new FloorInfo { Number = reader.GetInt32(0), Name = reader.GetString(1) });
                }
            }

            return floors;
        }

        public List<MapPlace> ReadDirectory()
        {
            using (var connection = Open(SqliteOpenMode.ReadOnly))
                return ReadPlaces(connection, null);
        }

        private static List<MapPlace> ReadPlaces(SqliteConnection connection, int? floor)
        {
            var places = new List<MapPlace>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT p.Name, p.Category, p.Area, p.Hours, p.X, p.Y,
                    p.Width, p.Height, p.EntranceNodeId, n.Floor
                    FROM Places p JOIN Nodes n ON n.Id = p.EntranceNodeId
                    WHERE ($floor IS NULL OR n.Floor = $floor) ORDER BY n.Floor, p.Y, p.X;";
                command.Parameters.AddWithValue("$floor", (object)floor ?? DBNull.Value);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        places.Add(new MapPlace
                        {
                            Name = reader.GetString(0), Category = reader.GetString(1),
                            Area = reader.GetString(2), Hours = reader.GetString(3),
                            X = reader.GetInt32(4), Y = reader.GetInt32(5),
                            Width = reader.GetInt32(6), Height = reader.GetInt32(7),
                            EntranceNodeId = reader.GetString(8), Floor = reader.GetInt32(9)
                        });
                    }
                }
            }

            return places;
        }

        public RouteGraph ReadGraph()
        {
            var graph = new RouteGraph();
            using (var connection = Open(SqliteOpenMode.ReadOnly))
            using (var transaction = connection.BeginTransaction(deferred: true))
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT Id, Floor, X, Y, Label FROM Nodes ORDER BY Id;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var node = new RouteNode
                        {
                            Id = reader.GetString(0), Floor = reader.GetInt32(1),
                            X = reader.GetInt32(2), Y = reader.GetInt32(3), Label = reader.GetString(4)
                        };
                        graph.Nodes.Add(node.Id, node);
                    }
                }
                command.CommandText = "SELECT FromNodeId, ToNodeId, Cost, Kind FROM Edges ORDER BY FromNodeId, ToNodeId;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        graph.Edges.Add(new RouteEdge
                        {
                            FromId = reader.GetString(0), ToId = reader.GetString(1),
                            Cost = reader.GetInt32(2), Kind = reader.GetString(3)
                        });
                    }
                }
                transaction.Commit();
            }
            return graph;
        }

        private SqliteConnection Open(SqliteOpenMode mode)
        {
            var connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = databasePath, Mode = mode, ForeignKeys = true
            }.ToString());
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }
    }

    public sealed class FloorMap
    {
        public string Name { get; set; }
        public int KioskFloor { get; set; }
        public List<MapPlace> Places { get; } = new List<MapPlace>();
        public List<MapMarker> Markers { get; } = new List<MapMarker>();
    }

    public sealed class FloorInfo
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Label => $"{Number}F  {Name}";
    }

    public sealed class MapPlace : ObservableObject
    {
        private bool isSelected;
        public string EntranceNodeId { get; set; }
        public int Floor { get; set; }
        public string FloorLabel => $"{Floor}층 · {Area}";
        public bool IsSelected
        {
            get => isSelected;
            set => Set(ref isSelected, value);
        }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Area { get; set; }
        public string Hours { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public sealed class MapMarker
    {
        public string Label { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsKiosk { get; set; }
    }
}
