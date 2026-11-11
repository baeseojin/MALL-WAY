using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MallWay
{
    public sealed class RouteService
    {
        private readonly MapRepository repository;

        public RouteService(MapRepository repository)
        {
            this.repository = repository;
        }

        public RouteResult FindRoute(string startId, string destinationId)
        {
            return Calculate(repository.ReadGraph(), startId, destinationId);
        }

        public static RouteResult Calculate(RouteGraph graph, string startId, string destinationId)
        {
            if (!graph.Nodes.ContainsKey(startId) || !graph.Nodes.ContainsKey(destinationId))
                throw new InvalidDataException("출발점 또는 목적지가 지도에 없습니다.");

            var adjacency = graph.Nodes.Keys.ToDictionary(id => id, id => new List<RouteStep>());
            foreach (var edge in graph.Edges)
            {
                if (edge.Cost <= 0 || !graph.Nodes.ContainsKey(edge.FromId) || !graph.Nodes.ContainsKey(edge.ToId))
                    throw new InvalidDataException("지도 연결과 이동 비용을 확인해 주세요.");
                var from = graph.Nodes[edge.FromId];
                var to = graph.Nodes[edge.ToId];
                if (edge.Kind != "walk" && edge.Kind != "elevator" && edge.Kind != "escalator")
                    throw new InvalidDataException("지원하지 않는 이동 방식입니다.");
                if (from.Floor != to.Floor && edge.Kind == "walk")
                    throw new InvalidDataException("층간 이동 시설을 확인해 주세요.");
                adjacency[from.Id].Add(new RouteStep { From = from, To = to, Cost = edge.Cost, Kind = edge.Kind });
                adjacency[to.Id].Add(new RouteStep { From = to, To = from, Cost = edge.Cost, Kind = edge.Kind });
            }

            var distances = graph.Nodes.Keys.ToDictionary(id => id, id => long.MaxValue);
            var previous = new Dictionary<string, RouteStep>();
            var remaining = new HashSet<string>(graph.Nodes.Keys);
            distances[startId] = 0;

            // The graph has 31 nodes. Scanning unsettled nodes keeps this implementation readable.
            while (remaining.Count > 0)
            {
                var current = remaining.OrderBy(id => distances[id]).ThenBy(id => id, StringComparer.Ordinal).First();
                if (distances[current] == long.MaxValue || current == destinationId)
                    break;
                remaining.Remove(current);

                foreach (var step in adjacency[current])
                {
                    var candidate = distances[current] + step.Cost;
                    if (remaining.Contains(step.To.Id) && candidate < distances[step.To.Id])
                    {
                        distances[step.To.Id] = candidate;
                        previous[step.To.Id] = step;
                    }
                }
            }

            if (distances[destinationId] == long.MaxValue)
                return null;

            var steps = new List<RouteStep>();
            var cursor = destinationId;
            while (cursor != startId)
            {
                var step = previous[cursor];
                steps.Add(step);
                cursor = step.From.Id;
            }
            steps.Reverse();
            var result = new RouteResult
            {
                Start = graph.Nodes[startId],
                Destination = graph.Nodes[destinationId],
                TotalCost = distances[destinationId],
                Steps = steps
            };
            result.Instructions = BuildInstructions(result);
            return result;
        }

        public static List<string> BuildInstructions(RouteResult route, bool english = false)
        {
            var instructions = new List<string>();
            for (var index = 0; index < route.Steps.Count; index++)
            {
                var step = route.Steps[index];
                if (step.From.Floor == step.To.Floor)
                    continue;

                var last = step.To;
                while (step.Kind == "elevator" && index + 1 < route.Steps.Count
                    && route.Steps[index + 1].Kind == step.Kind
                    && route.Steps[index + 1].From.Id == last.Id
                    && route.Steps[index + 1].From.Label == step.From.Label
                    && route.Steps[index + 1].To.Floor != last.Floor)
                {
                    last = route.Steps[++index].To;
                }
                instructions.Add(english
                    ? $"Take {DisplaySettings.Current.EnglishText(step.From.Label)} from {step.From.Floor}F → get off at {last.Floor}F"
                    : $"{step.From.Floor}F {step.From.Label} 탑승 → {last.Floor}F 하차");
            }
            instructions.Add(english
                ? $"Arrive at {route.Destination.Floor}F · {DisplaySettings.Current.EnglishText(route.Destination.Label)}"
                : $"{route.Destination.Floor}F {route.Destination.Label} 도착");
            return instructions;
        }
    }

    public sealed class RouteGraph
    {
        public Dictionary<string, RouteNode> Nodes { get; } = new Dictionary<string, RouteNode>();
        public List<RouteEdge> Edges { get; } = new List<RouteEdge>();
    }

    public sealed class RouteNode
    {
        public string Id { get; set; }
        public int Floor { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Label { get; set; }
    }

    public sealed class RouteEdge
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public int Cost { get; set; }
        public string Kind { get; set; }
    }

    public sealed class RouteStep
    {
        public RouteNode From { get; set; }
        public RouteNode To { get; set; }
        public int Cost { get; set; }
        public string Kind { get; set; }
    }

    public sealed class RouteResult
    {
        public RouteNode Start { get; set; }
        public RouteNode Destination { get; set; }
        public long TotalCost { get; set; }
        public List<RouteStep> Steps { get; set; }
        public List<string> Instructions { get; set; }
    }
}
