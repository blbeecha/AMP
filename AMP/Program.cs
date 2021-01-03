/*
 * AUTHOR: Brendan Beecham
 * FILE NAME: Program.cs
 * SOLUTION: AMP (Accounts Management Portal)
 * PROGRAM SCOPE: .NET 5.0 Command-line Application
 * PROGRAM PURPOSE:
 *      The purpose of this program is to allow for easy organization of monetary accounts.
 *      The basic functionality of the program is comparable to a typical bank account with greater
 *      flexibility for setting organization names and current budgets. At the exit of the program,
 *      an updated 'savedData.xml' will be created in the project directory to store account data
 *      between app sessions. The standout functionality of the project is the ability to export
 *      accounts data and the corresponding information to a Microsoft Office Excel 2016 workbook.
 *      For additional usage information on how to use the app and it's extended functionality,
 *      use the '--help' command.
 */

//From system libraries
using System;

//From Custom Libraries
using AMLibrary;
using FormatConversionLibrary;

namespace AMP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine("Accounts Management Portal");
            Console.WriteLine("--------------------------");

            AMOperations operations = new AMOperations();

            string[] accounts = operations.initializeAccounts();
            string[] organizations = operations.initializeOrganizations();
            string[] balances = operations.initializeBalances();

            string input;
            string[] commandArgs;

            //main program loop
            do
            {
                //accept user input for command
                Console.WriteLine();
                Console.Write("What would you like to do?: ");
                input = Console.ReadLine();
                commandArgs = input.Split(" ", 2);

                if (commandArgs[0] == "increment")
                {
                    //schema: "incBy --accountName --incBy"
                    //increment account by value given
                    try { 
                        string[] temp = commandArgs[1].Split(" ", 3);
                        if (temp[0] == null || temp[1] == null) { operations.argError(); }
                        int index = operations.getIndex(temp[0], accounts);
                        balances = operations.incBal(index, temp[1], balances, temp[0]);
                        operations.listAcc(accounts, organizations, balances);
                        continue;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    }
                }
                else if (commandArgs[0] == "decrement")
                {
                    //schema: "decBal --accountName --decBy"
                    //decrement account by value given
                    try { 
                        string[] temp = commandArgs[1].Split(" ", 2);
                        int index = operations.getIndex(temp[0], accounts);
                        balances = operations.decBal(index, temp[1], balances, temp[0]);
                        operations.listAcc(accounts, organizations, balances);
                        continue;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    }
                }
                else if (commandArgs[0] == "listAccounts")
                {
                    //schema: "listAccounts"
                    //list current accounts and associated information
                    operations.listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "removeAccount")
                {
                    //schema: "removeAccount --accountName"
                    //remove account from xml
                    try { 
                        int index = operations.getIndex(commandArgs[1], accounts);
                        accounts = operations.remAcc(index, accounts, commandArgs[1]);
                        organizations = operations.remOrg(index, organizations);
                        balances = operations.remBal(index, balances);
                        operations.listAcc(accounts, organizations, balances);
                        continue;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    }
                }
                else if (commandArgs[0] == "addAccount")
                {
                    //schema: "addAccount --accountName --organization --newBalance"
                    //add account to xml
                    try
                    {
                        string[] temp = commandArgs[1].Split(" ", 3);
                        if (!operations.duplicateAccount(temp[0], accounts))
                        {
                            operations.logNewAccount(temp[0], temp[1], temp[2]);
                            accounts = operations.addAcc(temp[0], accounts);
                            organizations = operations.addOrg(temp[1], organizations);
                            balances = operations.addBal(temp[2], balances);
                            operations.listAcc(accounts, organizations, balances);
                        }
                        continue;
                    } catch( System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    } 
                }
                else if (commandArgs[0] == "setBalance")
                {
                    //schema: "setBalance --accountName --newBalance"
                    //set balance of account to value given
                    try { 
                        string[] temp = commandArgs[1].Split(" ", 2);
                        int index = operations.getIndex(temp[0], accounts);
                        string newBalance = temp[1];
                        balances = operations.setBal(newBalance, index, balances, temp[0]);
                        operations.listAcc(accounts, organizations, balances);
                        continue;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    }
                }
                else if (commandArgs[0] == "setOrg")
                {
                    //schema: "setOrg --accountName --newOrg"
                    //replace organization association for account with given string
                    try { 
                        string[] temp = commandArgs[1].Split(" ", 2);
                        int index = operations.getIndex(temp[0], accounts);
                        string newOrg = temp[1];
                        organizations = operations.setOrg(newOrg, index, organizations, temp[0]);
                        operations.listAcc(accounts, organizations, balances);
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    }
                }
                else if (commandArgs[0] == "balanceSum")
                {
                    //schema: "balanceSum"
                    //print sum of all balances currently in system
                    operations.AllBalanceSum(balances);
                }
                else if (commandArgs[0] == "--help")
                {
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine("First option must be a command specifier, followed by necessary arguments:");
                    Console.WriteLine("\taddAccount -accountName -organization -newBalance");
                    Console.WriteLine("\tsetOrg -accountName -newOrg");
                    Console.WriteLine("\tsetBalance -accountName -newBalance");
                    Console.WriteLine("\tremoveAccount -accountName");
                    Console.WriteLine("\tlistAccounts");
                    Console.WriteLine("\tdecrement -accountName -decBy");
                    Console.WriteLine("\tincrement -accountName -incBy");
                    Console.WriteLine("\tbalanceSum");
                    Console.WriteLine("\texportToExcel -ExcelFileName ");
                    Console.WriteLine("\texportToExcel --Path -PathWithExcelFileName");
                    Console.Write("\t\t\t");
                    Console.Write(@"example usage: exportToExcel --Path C:\Users\bob\Documents\doc");

                    Console.WriteLine("\n\nTo terminate the program, use the 'exit' command");
                    Console.WriteLine("Following the termination of the program,");
		            Console.WriteLine("   an XML file with updated account information will be found in the current directory");
                    Console.WriteLine("--------------------------------------------------------------------------------------");

                }
                else if (commandArgs[0] == "exportToExcel")
                {

                    FormatExcel export = new FormatExcel();
                    
                    try {

                        string[] temp;
                        string Xlfile;
                        
                        //determine whether --Path flag was used, else use current directory
                        if (commandArgs[1].Contains("--Path"))
                        {
                            temp = commandArgs[1].Split(" ", 2);
                            Xlfile = temp[1] + ".xslx";
                            Console.WriteLine("Excel Document is being created at path: '" + Xlfile + "'");
                        }
                        else
                        {
                            Xlfile = commandArgs[1] + ".xlsx";
                            Console.WriteLine("Excel Document has been created in the User's Document directory. Please specify --Path for another destination.");
                        }

                        //update XML savedData to save recent changes
                        operations.constructXML(accounts, organizations, balances);

                        //Convert the XML into Dataset
                        System.Data.DataTable dt = export.convertXmlToEl();

                        // Create an Excel object
                        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                        //Create a workbook or open existing workbook if filename matches
                        string fileName = commandArgs[1];
                        Microsoft.Office.Interop.Excel.Workbook workbook = export.CreateWorkbook(fileName, excel);

                        //Create worksheet object
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;

                        //populate workbook with saved DataTable data   
                        workbook = export.PopulateWorkbook(workbook, worksheet, excel, dt);

                        //Save the workbook
                        try
                        {
                            workbook.SaveAs(Xlfile);
                        } catch(System.Runtime.InteropServices.COMException)
                        {
                            Console.WriteLine("The filepath input to export to was invalid. Please make sure to include the filename at the end.");
                            Console.WriteLine("Please try again or save via the window.\n");
                            continue;
                        }

                        //Close the Workbook
                        workbook.Close(true);

                        // Finally Quit the Application
                        ((Microsoft.Office.Interop.Excel.Application)excel).Quit();

                        Console.WriteLine("Excel Document has been exported!\n");
                        continue;
                        
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        operations.argError();
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("No valid command given. Try Again or type --help");
                }
            }
            while (commandArgs[0] != "exit");       //end when user inputs terminal command

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Saving data to local XML document");
            Console.WriteLine("--------------------------------------");
            operations.constructXML(accounts, organizations, balances);
            
            Console.WriteLine();

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Writing to local JSON log file");
            Console.WriteLine("--------------------------------------");
            operations.Finish();

            Console.WriteLine();

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Terminating Accounts Management Portal");
            Console.WriteLine("--------------------------------------");

        }

       
    }
}
