using AdventOfCode.Lib;

namespace AdventOfCode._2024.Day09;

[ProblemName("Disk Fragmenter")]
public class Solution : ISolver
{
    public object PartOne(string input)
    {
        var files = ParseInput(input);
        var diskSize = files.Sum(e => e.Size);
        var disk = new int[diskSize];

        var front = 0;
        var back = files.Count - 1;
        var i = 0;

        while (front <= back)
        {
            var frontFile = files[front];
            var backFile = files[back];

            if (!frontFile.Empty)
            {
                FillDisk(disk, i, frontFile);
                i += frontFile.Size;
                front++;
            }
            else if (backFile.Empty)
            {
                back--;
            }
            else
            {
                while (frontFile.Remainder > 0 && backFile.Remainder > 0)
                {
                    frontFile.Remainder--;
                    backFile.Remainder--;
                    disk[i++] = backFile.Index;
                }

                if (frontFile.Remainder == 0)
                {
                    front++;
                }

                if (backFile.Remainder == 0)
                {
                    back--;
                }
            }
        }

        return CalculateChecksum(disk);
    }

    public object PartTwo(string input)
    {
        var files = ParseInput(input);

        for (var i = files.Count - 1; i > 0; i--)
        {
            var file = files[i];

            if (file.Empty)
            {
                continue;
            }

            for (var j = 0; j < i; j++)
            {
                var compareFile = files[j];

                if (!compareFile.Empty || compareFile.Remainder < file.Remainder)
                {
                    continue;
                }

                file.StartingIndex = GetNewStartingIndex(compareFile);
                compareFile.Remainder -= file.Size;

                break;
            }
        }

        var sortedFiles = files
            .Where(e => !e.Empty)
            .OrderBy(e => e.StartingIndex)
            .ToList();

        return CalculateChecksum(sortedFiles);
    }

    private static List<DiskFile> ParseInput(string input)
    {
        var files = new List<DiskFile>();
        var index = 0;

        for (var i = 0; i < input.Length; i += 2)
        {
            AddFile(files, input[i] - '0', i / 2, false, ref index);

            if (i + 1 < input.Length)
            {
                AddFile(files, input[i + 1] - '0', 0, true, ref index);
            }
        }

        return files;
    }

    private static void AddFile(List<DiskFile> files, int size, int index, bool isEmpty, ref int globalIndex)
    {
        files.Add(new DiskFile(size, index, isEmpty, size, globalIndex));
        globalIndex += size;
    }

    private static void FillDisk(int[] disk, int startIndex, DiskFile file)
    {
        for (var j = 0; j < file.Remainder; j++)
        {
            disk[startIndex + j] = file.Index;
        }
    }

    private static long CalculateChecksum(int[] disk)
    {
        long checksum = 0;

        for (var j = 0; j < disk.Length; j++)
        {
            checksum += j * disk[j];
        }

        return checksum;
    }

    private static long CalculateChecksum(IEnumerable<DiskFile> files)
    {
        long checksum = 0;

        foreach (var file in files)
        {
            for (var i = 0; i < file.Size; i++)
            {
                checksum += (i + file.StartingIndex) * file.Index;
            }
        }

        return checksum;
    }

    private static int GetNewStartingIndex(DiskFile file) => file.StartingIndex - (file.Remainder - file.Size);

    private record DiskFile
    {
        public DiskFile(int size, int index, bool empty, int remainder, int startingIndex)
        {
            Size = size;
            Index = index;
            Empty = empty;
            Remainder = remainder;
            StartingIndex = startingIndex;
        }

        public int Size { get; set; }

        public int Index { get; set; }

        public bool Empty { get; set; }

        public int Remainder { get; set; }

        public int StartingIndex { get; set; }
    }
}
