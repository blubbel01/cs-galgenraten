using System;
using System.Collections.Generic;
using System.Text;

namespace Playground
{
    class Program
    {
        static string theWord;
        static string displayWord;
        static List<string> checkedCharactesList = new List<string>();
        static int tries = 0;

        static void Main(string[] args)
        {
            PrintTitle();
            Setup();
            GuessCharacter();
        }

        static void PrintTitle()
        {
            Console.WriteLine("===============================================================================================");
            Console.WriteLine(
                " ██████╗  █████╗ ██╗      ██████╗ ███████╗███╗   ██╗██████╗  █████╗ ████████╗███████╗███╗   ██╗" + "\n" +
                "██╔════╝ ██╔══██╗██║     ██╔════╝ ██╔════╝████╗  ██║██╔══██╗██╔══██╗╚══██╔══╝██╔════╝████╗  ██║" + "\n" +
                "██║  ███╗███████║██║     ██║  ███╗█████╗  ██╔██╗ ██║██████╔╝███████║   ██║   █████╗  ██╔██╗ ██║" + "\n" +
                "██║   ██║██╔══██║██║     ██║   ██║██╔══╝  ██║╚██╗██║██╔══██╗██╔══██║   ██║   ██╔══╝  ██║╚██╗██║" + "\n" +
                "╚██████╔╝██║  ██║███████╗╚██████╔╝███████╗██║ ╚████║██║  ██║██║  ██║   ██║   ███████╗██║ ╚████║" + "\n" +
                " ╚═════╝ ╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚══════╝╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═══╝"
            );
            Console.WriteLine("===============================================================================================");
            Console.WriteLine("v0.1 - JPD");
            Console.WriteLine("===============================================================================================");
        }

        static void Setup()
        {
            Console.Write("Dein Wort: ");
            theWord = Console.ReadLine();

            displayWord = "";
            for (int i = 0; i < theWord.Length; i++)
            {
                if (theWord[i] == ' ')
                {
                    displayWord += " ";
                }
                else
                {
                    displayWord += "#";
                }
            }
        }

        static void GuessCharacter()
        {
            Console.Clear();
            PrintTitle();

            Console.WriteLine("Das Wort: " + displayWord);
            Console.WriteLine("Fehlversuche: " + tries);

            // schreibe bereits geratene Buchstaben auf
            if (checkedCharactesList.Count > 0)
            {
                var tempText = "Bereits geratene Buchstaben: ";
                foreach (var character in checkedCharactesList)
                {
                    tempText += character + " ";
                }

                Console.WriteLine(tempText);
            }

            // Buchstabenabfrage
            Console.Write("Rate mal einen Buchstaben: ");
            var inputCharacter = Console.ReadLine();

            // darf nicht mehr als 1 Zeichen sein
            if (inputCharacter.Length != 1)
            {
                GuessCharacter();
                return;
            }

            if (checkedCharactesList.Contains(inputCharacter.ToUpper()))
            {
                GuessCharacter();
                return;
            }

            var newDisplayWordStringBuilder = new StringBuilder(displayWord);
            var success = false;

            for (int i = 0; i < theWord.Length; i++)
            {
                // passt der Buchstabe, ignoriert g-k-schreibung
                if (theWord.ToUpper()[i] == inputCharacter.ToUpper()[0])
                {
                    newDisplayWordStringBuilder[i] = theWord[i];
                    success = true;
                }
            }

            displayWord = newDisplayWordStringBuilder.ToString();

            if (!success)
            {
                tries++;
                checkedCharactesList.Add(inputCharacter.ToUpper());
            }

            // Checken ob man gewonnen hat und Progeamm schließen.
            if (theWord == displayWord)
            {
                Console.Clear();
                PrintTitle();
                Console.WriteLine("Du hast es geschafft!");
                Console.WriteLine("Das Wort: " + theWord);
                Console.WriteLine("Du hast " + tries + " versuche gebraucht.");
                Console.WriteLine("Drücke die Enter-Taste zum beenden . . .");
                Console.ReadLine();
                System.Environment.Exit(200);
                return;
            }

            GuessCharacter();
        }
    }
}