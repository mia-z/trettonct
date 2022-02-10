# <center>Tretton37 Code Test

## <center>Author: Ryan Cockram

### <center> Task Outline

>Create a console program in C# that can recursively traverse and download www.tretton37.com and save it to disk while keeping the online file structure. 
>Show download progress in the console. Focus on building a solid application that showcases your overall coding and C# skills, don’t get caught up in technical details like html link extraction (a simple regex is totally fine). 
>On top of the basics, we do appreciate it if your program displays a good use of **asynchronicity**, **parallelism** and **threading**.

---
#### Foreword
##### Overall, I found the task really interesting because I interpreted it as "create a webcrawler" and that's something I haven't done, nor really given any real thought into, until now.

---

### Planning and thought process
So I first thought about the two main aspects of this tasks that need tackling: **fetching all the links on all the pages** and then **maintaining their path/location**.
Fetching all the links didn't seem too hard at first since all I needed to do was create some basic regex patterns for matching \<anchor> tags and for extracting the `href` inside each tag. Ofcourse recursing through these would _probably_ cause infinite recursion, so I also had to keep track of which paths I've visited. While doing this I also need to make sure I'm keeping a reference to the original, full path, of the page for later use when writing the files to disk. 
The basic flow I thought of is as follows;

 1. Go to root page.
 2. Fetch all \<anchor> tags.
 3. Visit the pages of all the previously found \<anchor> tags and find tags in those pages.
 4. While doing this, I flag which paths have been checked.
 5. Repeat 3 and 4 until no matches are found that aren't already flagged as being visited.

After having fetched all possible data, the files should be written to disk with the root url they were found at in order to preserve the filetree and proper locations.
It was a bit trickier, and there may even be some erroneous results, since a webpage doesn't _always_ (explicitly) have a file extension - since it's 2022 and tedious things like this are handled by modern web browsers. To compensate for this, if a page, without an extension, is found to have no children then it is treated as an html page and has ".html" appended to it when writing to disk. 

---
### Approach and methods used
Personally I enjoy using some of the more advanced methods in programming (more often than not when they're really overkill, blessing and a curse type of thing), so being able to display some of the methods I'm capable of using was the most fun part here.

 - **Singleton Pattern for keeping state/monitoring a global value** - I used a basic Singleton pattern in my code just to hold the values of which paths had already been visited. There were two big benefits to this; it held a global value with single source of truth, and it was thread safe. Since the values in the Singleton were going to be accepted by tasks on other threads, it was important that the values are properly accessible and not incorrect at the time of their access.
 - **Running Asynchronous Tasks on separate threads** - This is what sped the process up from several minutes to several seconds (there were a lot of anchor threads being recursed!). By performing each page recursion on it's own thread I didn't have to wait for a search to finish before starting a new search. 
 - **HashSet usage and implementing Equals/GetHashCode in a custom Class** - I haven't worked much with hashes before so this was really interesting for me. In the Singleton I use a HashSet for storing the paths/urls that have been checked which allows for fast lookup when a task needs to check if a found \<anchor> tag is worth looking into. Added to this, I used created a custom class that implements from the IEquatable<T> interface so that I could make concrete, unique, indicies for the virtual filetree used when mapping data to disk-write locations.

---
### Afterword
As stated previously, overall I found this enjoyable. I got to learn some new things and program a new concept that I havent done before - this is something good I can take away regardless of the outcome of this interview process.

## Instructions for running

 - The program will attempt to create a folder in the C:/User/{username}/Documents folder called "RCT37CT". It's in here where all the html files are written.
- If an unauthorized access excepotion is thrown, you may have to make sure the directories are **not** read-only. You can do this by checking the subfolders of RCT37CT after the program throws the exception, or by running the program as admin (It's safe I promise).
- If it doesn't work.. Well.. It worked on my machine!

---

Thanks for the oppurtunity.

Ryan C.