using CommandLine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;

namespace ByPassVPL
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(x => ParsedMain(x))
                .WithNotParsed(x => Console.WriteLine("Error while parsing arguments"));

            Console.ReadLine();
        }

        private static void ParsedMain(Options x)
        {
            string contents = File.ReadAllText(x.File)
                .Split('\n').Select(x => x.Trim()).Aggregate((l,r) => l + "\n" + r);

            ChromeDriver drv = new ChromeDriver();
            drv.Navigate().GoToUrl("http://drnguyentt.com/moodle30/login/index.php");
            drv.FindElement(By.Id("username")).SendKeys(x.UserName);
            drv.FindElement(By.Id("password")).SendKeys(x.Password);
            drv.FindElement(By.Id("loginbtn")).Click();
            drv.Navigate().GoToUrl(x.Address);


            //try
            //{
            //    var newFile = drv.FindElement(By.Id("vpl_ide_input_newfilename"));
            //    if (newFile != null)
            //    {
            //        WebDriverWait waiter = new WebDriverWait(drv, TimeSpan.FromMinutes(1));
            //        waiter.Until(d =>
            //        {
            //            try
            //            {
            //                newFile.SendKeys(x.RemoteFileName);
            //                drv.FindElement(By.XPath("/html/body/div[5]/div[3]/div/button[1]")).Click();
            //                return true;
            //            }
            //            catch
            //            {
            //                return false;
            //            }

            //        });

            //    }

            //}
            //catch { }
            //        

            WebDriverWait waiter = new WebDriverWait(drv, TimeSpan.FromMinutes(1));
            waiter.Until(d =>
            {
                try
                {
                    new Actions(drv)
                        .Click(drv.FindElement(By.ClassName("ace_content")))
                        .SendKeys(contents)
                        .Build()
                        .Perform();
                    return true;
                }
                catch 
                {
                    return false;
                }
            });
            

            drv.ExecuteScript("alert(\"Pasted! please review your work before submiting, after that feel free to close the program!\")");
        }
    }


    class Options
    {
        [Option('u', HelpText = "Moodle username", Required = true)]
        public string UserName { get; set; }

        [Option('p', HelpText = "Moodle password", Required = true)]
        public string Password { get; set; }

        [Option('f', HelpText = "File to write", Required = false)]
        public string File { get; set; }

        [Option('a', HelpText = "Link workshop", Required = true)]
        public string Address { get; set; }

        [Option('r', HelpText = "Remote file name", Required = false)]
        public string RemoteFileName { get; set; }
    }
}
