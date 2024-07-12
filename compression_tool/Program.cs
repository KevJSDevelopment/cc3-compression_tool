internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length <= 0) {
            Console.WriteLine("Please specify a file name");
            return;
        }

        string filePath = args[0];

        try {
            using StreamReader streamReader = new StreamReader(filePath);

            string content = streamReader.ReadToEnd();
            
            Dictionary<char, int> charCountDictionary  = new Dictionary<char, int>();

            for(int i = 0; i < content.Length;i++){
                if(charCountDictionary.ContainsKey(content[i])) charCountDictionary[content[i]]++;
                else charCountDictionary.Add(content[i], 1);
            }

            foreach(var keyValuePair in charCountDictionary){
                Console.WriteLine(keyValuePair.Key + ": " + keyValuePair.Value);
            }

        } catch (Exception ex)
        {
            Console.WriteLine("Failed to read file: " + filePath + "\nError: " + ex.Message);
        }
    }
}