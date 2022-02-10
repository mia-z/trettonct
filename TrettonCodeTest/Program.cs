
using TrettonCodeTest.Models;
/// <summary>
/// Author: Ryan Cockram
/// 
/// Code Test for Technical interview re. Tretton37.
/// 
/// Written in C#, using .NET 6
/// 
/// Repository: https://github.com/mia-z/trettonct
/// 
/// Task outline
/// Create a simple console program that can recursively traverse the Tretton37 root website directory
/// and save it to disk while keeping the integrity of the file structure.
/// 
/// </summary>
namespace TrettonCodeTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"Welcome to the Code Test for Tretton37 by Ryan Cockram\n" +
                $"This program will recursively download all the pages from https://tretton37.com/ and preserve their paths,\n" +
                $"saving the Html files on disk\n\n" +
                $"Hit Enter to begin!");

            Console.ReadLine();

            var rootPage = await new HtmlPage("").GetChildren();

            Console.ReadLine();
        }
    }
}