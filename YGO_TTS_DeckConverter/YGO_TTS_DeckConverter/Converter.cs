using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace YGO_TTS_DeckConverter
{

    public class Config
    {
        public string InstallPath { get; set; }
        public string OutputPath { get; set; }
        public List<string> Decks { get; set; }
    }
    public class Converter
    {

        
        string deck1path = "C:\\ProjectIgnis\\deck\\2025-TenpaiCustom.ydk";
        

        Deck deckA = new Deck();
       

        List<Card> aCards = new List<Card>();

        public string InstallPath { get; set; } = "";
        public List<string> Decks { get; set; } = new List<string>();
        string OutputPath = "";


        public Converter() {

            LoadConfig();

            foreach (string d in Decks)
            {
                deckA = LoadDeck(InstallPath + "deck\\" + d);

                InitializeCards();

                CreateTTSImages(d);
            }
        }

        

        public void LoadConfig()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "INPUT.txt");

            string json = File.ReadAllText(filePath);
            Config config = JsonSerializer.Deserialize<Config>(json);
            InstallPath = config.InstallPath;
            Decks = config.Decks;
            OutputPath = config.OutputPath;

        }


        private void CreateTTSImages(string deckname)
        {

            CompileImages(aCards, InstallPath + "pics", OutputPath, deckname);
            

            

        }

        private void InitializeCards()
        {

            foreach (string s in deckA.Main)
            {
                Card c2 = new Card();
                
                aCards.Add(c2);
                c2.ID = s;
                

            }

            foreach (string s in deckA.Extra)
            {
                Card c2 = new Card();
                
                aCards.Add(c2);
                c2.ID = s;
                
            }

            foreach (string s in deckA.Side)
            {
                Card c2 = new Card();

                aCards.Add(c2);
                c2.ID = s;

            }

            


        }

        public void CompileImages(List<Card> cards, string imageDirectory, string outputFilePath, string deckname)
        {
            // Constants for the final image size and layout
            const int imagesPerRow = 10;
            const int rows = 7;
            const int imageWidth = 421;
            const int imageHeight = 614;
            const int finalImageWidth = imagesPerRow * imageWidth;
            const int finalImageHeight = rows * imageHeight;

            // Check if the output directory exists, if not, create it
            string outputDirectory = Path.GetDirectoryName(outputFilePath);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Create the final bitmap to hold the compiled images
            using (Bitmap finalImage = new Bitmap(finalImageWidth, finalImageHeight))
            {



                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    // Fill the entire background with black
                    g.Clear(Color.Black);

                    for (int i = 0; i < cards.Count && i < imagesPerRow * rows; i++)
                    {
                        string id = cards[i].ID;
                        string imagePath = Path.Combine(imageDirectory, id + ".jpg");

                        // Check if the image file exists
                        if (File.Exists(imagePath))
                        {
                            using (Image img = Image.FromFile(imagePath))
                            {
                                // Calculate the position in the grid
                                int rowIndex = i / imagesPerRow;
                                int colIndex = i % imagesPerRow;

                                // Calculate the position where the image should be placed
                                int x = colIndex * imageWidth;
                                int y = rowIndex * imageHeight;

                                // Draw the image onto the final image
                                g.DrawImage(img, x, y, imageWidth, imageHeight);
                            }
                        }
                    }
                }
                string outputPath = "C:\\temp\\" + deckname + ".png";
                finalImage.Save(outputPath, ImageFormat.Png); // Save as PNG


            }


        }


        public Deck LoadDeck(string path)
        {
            string filePath = path; // Replace with the actual file path
            Deck deck = new Deck();

            // Read all lines from the file
            var lines = File.ReadAllLines(filePath + ".ydk");

            // Initialize a variable to track the current section
            string currentSection = "main";

            foreach (var line in lines)
            {
                // Ignore comment lines
                if (line.StartsWith("#") || line.StartsWith("!"))
                {
                    // Update current section based on the line
                    if (line.Contains("main"))
                    {
                        currentSection = "main";
                    }
                    else if (line.Contains("extra"))
                    {
                        currentSection = "extra";
                    }
                    else if (line.Contains("side"))
                    {
                        currentSection = "side";
                    }
                    continue;
                }


                // Add the card to the appropriate list based on the current section
                switch (currentSection)
                {
                    case "main":
                        deck.Main.Add(line);
                        break;
                    case "extra":
                        deck.Extra.Add(line);
                        break;
                    case "side":
                        deck.Side.Add(line);
                        break;
                }
            }

            // Output to check if the data is read correctly
            Console.WriteLine("Main Deck:");
            foreach (var card in deck.Main)
            {
                Console.WriteLine(card);
            }

            Console.WriteLine("\nExtra Deck:");
            foreach (var card in deck.Extra)
            {
                Console.WriteLine(card);
            }

            Console.WriteLine("\nSide Deck:");
            foreach (var card in deck.Side)
            {
                Console.WriteLine(card);
            }

            return deck;
        }

    }
}
