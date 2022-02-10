using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrettonCodeTest;
using TrettonCodeTest.Services;

namespace TrettonCodeTest.Models
{
    internal class HtmlPage
    {
        //Hard coded - can probably set via cl-parameter later. or not.🤷‍
        internal readonly string BASE_ADDRESS = "https://tretton37.com";

        /// <summary>
        /// Holds the value of the path this page.
        /// </summary>
        public string? Path { get; set; }
        
        /// <summary>
        /// Returns the full url for this page, for the purpose of fetching it.
        /// </summary>
        public string FullPath 
        { 
            get
            {
                return string.Join("/", BASE_ADDRESS, Path);
            }
        }
        
        /// <summary>
        /// Holds the HTML of this page, as a string.
        /// </summary>
        public string? RawHtml { get; set; }
        
        /// <summary>
        /// List of the children under this page.
        /// "children" being any anchor tag found in the HTML that isnt already being tracked by the PathMonitor service
        /// </summary>
        public List<HtmlPage>? Children { get; private set; }

        public HtmlPage(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Finds all the children contained in the HTML string of this page.
        /// The function is called recursively on all found anchor tags that aren't being tracked. 
        /// If a found achor tag isnt being tracked, a child will be created under this HtmlPage.
        /// </summary>
        /// <returns>HtmlPage</returns>
        public async Task<HtmlPage> GetChildren()
        {
            try
            {
                //Scope-bound http-client which will auto dispose once the scope (this function) is no longer accessible.
                //We fetch the HTML data of the current page in FullPath
                using (var client = new HttpClient())
                    RawHtml = await client.GetStringAsync(FullPath);

                //First we use the utility function GetAnchors to extract the anchor tags in the html..
                var hrefs = Utils.GetAnchors(RawHtml)
                    //Then use the utility function GetHrefText to extract the text inside href of all the anchor tags..
                    .Select(x => Utils.GetHrefText(x))
                    //Make sure they aren't links we've previously checked and that they're not blank..
                    .Where(x => !PathMonitor.Instance.PathExists(x) && x != "")
                    //.. and that they're unique!
                    .Distinct()
                    .ToList();

                //Here we create the children for the current page depending on the Hrefs we found in the current page, above.
                Children = hrefs
                    .Select(x => new HtmlPage(x))
                    .ToList();

                //Update the monitor for the impending recursion!
                foreach (string path in hrefs)
                {
                    if (!PathMonitor.Instance.PathExists(path))
                        PathMonitor.Instance.AddPath(path);
                }

                //If there _are_ any children, we need to recurse and check those children for their children and their children for their children.. I can go on.
                if (Children.Count > 0)
                {
                    //Prepare a list of tasks for every child we are going to recurse
                    List<Task> childProcesses = new List<Task>();
                    PathMonitor.Instance.IncrementTaskCount(Children.Count);
                    //Loop through the children, calling their GetChildren function in parallel with eachother. 
                    //(I also tried this synchronously.. I'm still waiting for it to finish)
                    foreach (var child in Children)
                    {
                        childProcesses.Add(child.GetChildren());
                    }

                    //Wait until all the children of children(n) have been created 
                    while (childProcesses.Any())
                    {
                        //Simple wait loop which will report progress when a task completes
                        var finishedSearch = await Task.WhenAny(childProcesses);
                        childProcesses.Remove(finishedSearch);
                        PathMonitor.Instance.DecrementTaskCount();
                    }
                }

                //Not entirely neccessary, but I like function chaining so I opted for this.
                return this;
            } 
            catch (Exception ex)
            {
                //Very basic error handling if anything above throws an exception.
                //Ideally youd want a log or a retry counter, but seems unneccessary for this task.
                Console.Write($"Unknown error occured in program execution\n{ex.Message}\n\n{ex.StackTrace}\n\nPress enter to quit program");
                Console.ReadLine();
                Environment.Exit(1);
                return null; //Probably not the _best_ way to handle the exception, but I dont want to go overboard
            }
        }
    }
}
