using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace compression_tool
{
    public class CompressionTool
    {
        public static void WriteHeader(string outputFilePath, Dictionary<char, int> frequencyTable) {
            using (var writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create))) {
                // Write a header identifier (e.g., "HEADER" to mark the start)
                writer.Write("HEADER");

                // Write the frequency table
                writer.Write(frequencyTable.Count);
                foreach (var entry in frequencyTable) {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value);
                }

                // Write a header terminator (e.g., "DATA" to mark the end of the header and start of data)
                writer.Write("DATA");
            }
        }
        public static void WriteCompressedData(string outputFilePath, string inputText, Dictionary<char, string> codeTable) {
            using (var writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Append))) {
                // Convert the input text to a bit string using the code table
                string bitString = string.Empty;
                foreach (char c in inputText) {
                    bitString += codeTable[c];
                }

                // Pack the bit string into bytes and write to the file
                for (int i = 0; i < bitString.Length; i += 8) {
                    string byteString = bitString.Substring(i, Math.Min(8, bitString.Length - i));
                    byte b = Convert.ToByte(byteString, 2);
                    writer.Write(b);
                }
            }
        }

        public static Dictionary<char, int> GetCharCount(string filePath)
        {
            Dictionary<char, int> charCountDictionary = new Dictionary<char, int>();

            try
            {
                using StreamReader streamReader = new StreamReader(filePath);

                string content = streamReader.ReadToEnd();


                for (int i = 0; i < content.Length; i++)
                {
                    if (charCountDictionary.ContainsKey(content[i])) charCountDictionary[content[i]]++;
                    else charCountDictionary.Add(content[i], 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read file: " + filePath + "\nError: " + ex.Message);
            }
            
            return charCountDictionary;
        }

        public static List<HuffTree> CharCountPiorityQueue(Dictionary<char, int> charCountDict)
        {
            List<HuffTree> prioQueue = new List<HuffTree>();

            foreach(var keyValuePair in charCountDict)
            {
                Console.WriteLine(keyValuePair.Key + ": " + keyValuePair.Value);
                prioQueue.Add(new HuffTree(keyValuePair.Key, keyValuePair.Value));
            }

            prioQueue.Sort((a, b) => a.Weight().CompareTo(b.Weight()));

            return prioQueue;
        }
        public static HuffTree BuildTree(List<HuffTree> prioQueue) {
            HuffTree tmp1, tmp2, tmp3 = null;

            while (prioQueue.Count > 1) { // While two items left
                tmp1 = prioQueue[0];
                prioQueue.RemoveAt(0);
                tmp2 = prioQueue[0];
                prioQueue.RemoveAt(0);
                tmp3 = new HuffTree(tmp1.Root(), tmp2.Root(), tmp1.Weight() + tmp2.Weight());
                prioQueue.Add(tmp3);   // Return new tree to heap
            }

            return tmp3;            // Return the tree
        }

        public static Dictionary<char, string> BuildCodeTable(HuffTree huffmanTree) {
            Dictionary<char, string> codeTable = new Dictionary<char, string>();
            BuildCodeTableRecursive(huffmanTree.Root(), "", codeTable);
            return codeTable;
        }

        private static void BuildCodeTableRecursive(IHuffBaseNode node, string prefix, Dictionary<char, string> codeTable) {
            if (node.IsLeaf()) {
                HuffLeafNode leafNode = (HuffLeafNode)node;
                codeTable[leafNode.Value()] = prefix;
            } else {
                HuffInternalNode internalNode = (HuffInternalNode)node;
                BuildCodeTableRecursive(internalNode.Left(), prefix + "0", codeTable);
                BuildCodeTableRecursive(internalNode.Right(), prefix + "1", codeTable);
            }
        }
        public static Dictionary<char, int> ReadHeader(string inputFilePath) {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();
            
            using (var reader = new BinaryReader(File.Open(inputFilePath, FileMode.Open))) {
                string header = reader.ReadString();
                if (header != "HEADER") {
                    throw new InvalidDataException("Invalid file format: missing HEADER");
                }

                int frequencyTableCount = reader.ReadInt32();
                for (int i = 0; i < frequencyTableCount; i++) {
                    char key = reader.ReadChar();
                    int value = reader.ReadInt32();
                    frequencyTable[key] = value;
                }

                string dataMarker = reader.ReadString();
                if (dataMarker != "DATA") {
                    throw new InvalidDataException("Invalid file format: missing DATA marker");
                }
            }

            return frequencyTable;
        }

        public static void DecodeFile(string inputFilePath, string outputFilePath) {
            Dictionary<char, int> frequencyTable = ReadHeader(inputFilePath);
            HuffTree huffmanTree = BuildTree(CharCountPiorityQueue(frequencyTable));
            Dictionary<string, char> reverseCodeTable = BuildReverseCodeTable(huffmanTree);

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

        public static Dictionary<string, char> BuildReverseCodeTable(HuffTree huffmanTree) {
            Dictionary<char, string> codeTable = BuildCodeTable(huffmanTree);
            return codeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

    }
}