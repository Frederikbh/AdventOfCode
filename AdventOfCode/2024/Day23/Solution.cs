using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day23;

[ProblemName("LAN Party")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var graph = ParseInput(input);
        var nodes = graph.GetNodes();

        var tNodes = nodes.Where(e => e.StartsWith('t'))
            .ToList();
        var sets = new HashSet<string>();

        foreach (var tNode in tNodes)
        {
            var neighbors = graph.GetNeighbours(tNode)
                .ToList();

            for (var i = 0; i < neighbors.Count; i++)
            {
                for (var j = i + 1; j < neighbors.Count; j++)
                {
                    if (graph.IsConnected(neighbors[i], neighbors[j]))
                    {
                        var set = SetToKey([tNode, neighbors[i], neighbors[j]]);
                        sets.Add(set);
                    }
                }
            }
        }

        return sets.Count;
    }

    public object PartTwo(string input)
    {
        var graph = ParseInput(input);
        graph.FindMaximalCliques();
        var size = graph.MaximalCliques.MaxBy(e => e.Count);

        return string.Join(',', size!.OrderBy(e => e));
    }

    private static string SetToKey(List<string> set) => string.Join(',', set.OrderBy(e => e));

    private static Graph ParseInput(string input)
    {
        var graph = new Graph();
        var lines = input.Split('\n');

        foreach (var line in lines)
        {
            var nodes = line.Split('-');
            graph.AddEdge(nodes[0], nodes[1]);
        }

        return graph;
    }

    private class Graph
    {
        private readonly Dictionary<string, HashSet<string>> _nodes = new();

        public List<HashSet<string>> MaximalCliques { get; } = [];

        public List<string> GetNodes() => _nodes.Keys.ToList();

        public HashSet<string> GetNeighbours(string node) => _nodes[node];

        public bool IsConnected(string node1, string node2) =>
            _nodes[node1]
                .Contains(node2) || _nodes[node2]
                .Contains(node1);

        private void AddNode(string node)
        {
            if (!_nodes.ContainsKey(node))
            {
                _nodes[node] = [];
            }
        }

        public void AddEdge(string node1, string node2)
        {
            AddNode(node1);
            AddNode(node2);

            _nodes[node1].Add(node2);
            _nodes[node2].Add(node1);
        }

        public void FindMaximalCliques()
        {
            var r = new HashSet<string>();
            var p = new HashSet<string>(_nodes.Keys);
            var x = new HashSet<string>();
            BronKerbosch(r, p, x);
        }

        private void BronKerbosch(HashSet<string> r, HashSet<string> p, HashSet<string> x)
        {
            if (p.Count == 0 && x.Count == 0)
            {
                MaximalCliques.Add([..r]);
                return;
            }

            var pivot = ChoosePivot(p, x);
            var pivotNeighbors = _nodes[pivot];
            var candidates = new HashSet<string>(p.Except(pivotNeighbors));

            foreach (var vertex in candidates)
            {
                r.Add(vertex);
                var newP = new HashSet<string>(p.Intersect(_nodes[vertex]));
                var newX = new HashSet<string>(x.Intersect(_nodes[vertex]));
                BronKerbosch(r, newP, newX);
                r.Remove(vertex);
                p.Remove(vertex);
                x.Add(vertex);
            }
        }

        private string ChoosePivot(HashSet<string> p, HashSet<string> x)
        {
            var union = new HashSet<string>(p);
            union.UnionWith(x);

            string? pivot = null;
            var maxDegree = -1;

            foreach (var vertex in union)
            {
                var degree = _nodes[vertex].Count;

                if (degree > maxDegree)
                {
                    maxDegree = degree;
                    pivot = vertex;
                }
            }

            return pivot ?? "";
        }
    }
}
