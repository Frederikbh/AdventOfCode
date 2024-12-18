using System.Collections.Immutable;

using AdventOfCode.Lib;

namespace AdventOfCode._2017.Day12;

[ProblemName("Digital Plumber")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var graph = ParseInput(input);

        return TraverseGroup(graph, 0)
            .Count;
    }

    public object PartTwo(string input)
    {
        var graph = ParseInput(input);
        var groupCount = 0;
        var nodes = graph.GetNodes()
            .ToHashSet();

        while (nodes.Count > 0)
        {
            groupCount++;
            var node = nodes.First();
            var group = TraverseGroup(graph, node);
            nodes = nodes.Except(group)
                .ToHashSet();
        }

        return groupCount;
    }

    private static HashSet<int> TraverseGroup(Graph graph, int start)
    {
        var visited = new HashSet<int>();

        var queue = new Queue<int>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            visited.Add(node);

            foreach (var neighbor in graph.GetConnectedNodes(node))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited;
    }

    private static Graph ParseInput(string input)
    {
        var lines = input.Split("\n");
        var graph = new Graph();

        foreach (var line in lines)
        {
            var parts = line.Split(" <-> ");
            var node = int.Parse(parts[0]);
            var neighbors = parts[1]
                .Split(", ")
                .Select(int.Parse)
                .ToImmutableList();

            foreach (var neighbor in neighbors)
            {
                graph.AddEdge(node, neighbor);
            }
        }

        return graph;
    }

    private class Graph
    {
        private readonly Dictionary<int, HashSet<int>> _nodes = new();

        private void AddNode(int node)
        {
            if (!_nodes.ContainsKey(node))
            {
                _nodes[node] = [];
            }
        }

        public void AddEdge(int node1, int node2)
        {
            AddNode(node1);
            AddNode(node2);
            _nodes[node1]
                .Add(node2);
            _nodes[node2]
                .Add(node1);
        }

        public IEnumerable<int> GetConnectedNodes(int node) => _nodes[node];

        public List<int> GetNodes() => _nodes.Keys.ToList();
    }
}
