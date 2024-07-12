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
        } catch (Exception ex)
        {
            Console.WriteLine("Failed to read file: " + filePath + "\nError: " + ex.Message);
        }


    }
}