using compression_tool;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("Please specify a file name");
            return;
        }

        Dictionary<char, int> charCountDict = CompressionTool.GetCharCount(args);

        List<KeyValuePair<char, int>> prioQueue = CompressionTool.CharCountPiorityQueue(charCountDict);

        
    }
}