using compression_tool;

internal class Program
{
    private static void Main(string[] args)
    {
         if (args.Length < 3) {
            Console.WriteLine("Usage: program <encode/decode> <input file> <output file>");
            return;
        }

        string mode = args[0];
        string inputFilePath = args[1];
        string outputFilePath = args[2];

        if (mode == "encode") {
            EncodeFile(inputFilePath, outputFilePath);
        } else if (mode == "decode") {
            DecodeFile(inputFilePath, outputFilePath);
        } else {
            Console.WriteLine("Invalid mode. Use 'encode' or 'decode'.");
        };
    }

     public static void EncodeFile(string filePath, string outPutPath) {
                // Read the input file
        string inputText = File.ReadAllText(filePath);

        Dictionary<char, int> charCountDict = CompressionTool.GetCharCount(filePath);

        List<HuffTree> prioQueue = CompressionTool.CharCountPiorityQueue(charCountDict);

        HuffTree tree = CompressionTool.BuildTree(prioQueue);
        Dictionary<char, string> codeTable = CompressionTool.BuildCodeTable(tree);


        // Write the header
        CompressionTool.WriteHeader(outPutPath, charCountDict);

        // Write the compressed data
        CompressionTool.WriteCompressedData(outPutPath, inputText, codeTable);
    }
    public static void DecodeFile(string inputFilePath, string outputFilePath) {
        Dictionary<char, int> frequencyTable = CompressionTool.ReadHeader(inputFilePath);
        HuffTree huffmanTree = CompressionTool.BuildTree(CompressionTool.CharCountPiorityQueue(frequencyTable));
        Dictionary<string, char> reverseCodeTable = CompressionTool.BuildReverseCodeTable(huffmanTree);

        using (var reader = new BinaryReader(File.Open(inputFilePath, FileMode.Open)))
        using (var writer = new StreamWriter(File.Open(outputFilePath, FileMode.Create))) {
            // Skip the header
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            while (reader.ReadString() != "DATA") { }
            
            // Read the encoded data
            List<byte> encodedBytes = new List<byte>();
            while (reader.BaseStream.Position != reader.BaseStream.Length) {
                encodedBytes.Add(reader.ReadByte());
            }

            // Decode the data
            string bitString = string.Join(string.Empty, encodedBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
            string currentCode = string.Empty;

            foreach (char bit in bitString) {
                currentCode += bit;
                if (reverseCodeTable.ContainsKey(currentCode)) {
                    writer.Write(reverseCodeTable[currentCode]);
                    currentCode = string.Empty;
                }
            }
        }
    }
}