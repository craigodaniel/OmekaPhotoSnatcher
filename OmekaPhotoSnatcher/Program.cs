
namespace OmekaPhotoSnatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            bool endApp = false;

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("--------OMEKA PHOTO SNATCHER-------");
            Console.WriteLine("-----------------------------------\n");

            Console.WriteLine("This program will:");
            Console.WriteLine("-Scan the .txt file you select for item numbers");
            Console.WriteLine("-Call the Omeka API for getting files by item number");
            Console.WriteLine("-Download all files with \"orignial\" tag");
            Console.WriteLine("-Save the downloaded files as Item_#### in same folder where this program is located.");
            Console.WriteLine("-Creates a log file in the same folder where this program is located\n");
            



            while (!endApp)
            {
                string? filePath;
                string? runInput;
                string? quitInput;

                Console.WriteLine("Enter full file path and name:");
                filePath = Console.ReadLine();

                Console.Write("Run Program? Y/N: \n");
                runInput = Console.ReadLine();
                if (runInput == "Y" || runInput == "y") { PhotoSnatcher.Main(filePath); }

                Console.Write("Quit? Y/N: \n");
                quitInput = Console.ReadLine();
                if (quitInput == "Y" || quitInput == "y") { endApp = true; }

            }

        }
    }
}