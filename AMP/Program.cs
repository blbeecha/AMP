using System;
using AMLibrary;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;


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
                    Console.WriteLine("\texportToExcel -ExcelFileName");

                    Console.WriteLine("To terminate the program, use the 'exit' command");
                    Console.WriteLine("Following the termination of the program,");
		            Console.WriteLine("   an XML file with updated account information will be found in the current directory");
                    Console.WriteLine("--------------------------------------------------------------------------------------");

                }
                else if (commandArgs[0] == "exportToExcel")
                {

                    try { 
                        DataSet ds = new DataSet();

                        //Convert the XML into Dataset
                        ds.ReadXml(@"savedData.xml");

                        //Retrieve the table fron Dataset
                        System.Data.DataTable dt = ds.Tables[0];

                        // Create an Excel object
                        /*Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                        
                        //Create workbook object
                        string str = "test.xlsx";
                        Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Open(Filename: str);

                        //Create worksheet object
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet) workbook.ActiveSheet;

                        // Column Headings
                        int iColumn = 0;

                        foreach (DataColumn c in dt.Columns)
                        {
                            iColumn++;
                            excel.Cells[1, iColumn] = c.ColumnName;
                        }

                        // Row Data
                        int iRow = worksheet.UsedRange.Rows.Count - 1;

                        foreach (DataRow dr in dt.Rows)
                        {
                            iRow++;

                            // Row's Cell Data
                            iColumn = 0;
                            foreach (DataColumn c in dt.Columns)
                            {
                                iColumn++;
                                excel.Cells[iRow + 1, iColumn] = dr[c.ColumnName];
                            }
                        }

                        ((Microsoft.Office.Interop.Excel._Worksheet)worksheet).Activate();

                        //Save the workbook
                        workbook.Save();

                        //Close the Workbook
                        workbook.Close();

                        // Finally Quit the Application
                        ((Microsoft.Office.Interop.Excel._Application)excel).Quit();
                        string Xlfile = commandArgs[1] + ".xlsx";
                        */
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

        static System.Data.DataTable convertXmlToEl()
        {
            string XmlFile = "savedData.xml";
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(XmlFile);
                Dt.Load(ds.CreateDataReader());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Dt;
        }
    }
}
