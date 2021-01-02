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
                    if (commandArgs[1] == "" || commandArgs[2] == "") { operations.argError(); }
                    //schema: "incBy --accountName --incBy"
                    //increment account by value given
                    string[] temp = commandArgs[1].Split(" ", 3);
                    if (temp[0] == null || temp[1] == null) { operations.argError(); }
                    int index = operations.getIndex(temp[0], accounts);
                    balances = operations.incBal(index, temp[1], balances, temp[0]);
                    operations.listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "decrement")
                {
                    //schema: "decBal --accountName --decBy"
                    //decrement account by value given
                    string[] temp = commandArgs[1].Split(" ", 2);
                    if (temp[0] == null || temp[1] == null) { operations.argError(); }
                    int index = operations.getIndex(temp[0], accounts);
                    balances = operations.decBal(index, temp[1], balances, temp[0]);
                    operations.listAcc(accounts, organizations, balances);
                    continue;
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
                    if (commandArgs[1] == null) { operations.argError(); }
                    //schema: "removeAccount --accountName"
                    //remove account from xml
                    int index = operations.getIndex(commandArgs[1], accounts);
                    accounts = operations.remAcc(index, accounts, commandArgs[1]);
                    organizations = operations.remOrg(index, organizations);
                    balances = operations.remBal(index, balances);
                    operations.listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "addAccount")
                {
                    //schema: "addAccount --accountName --organization --newBalance"
                    //add account to xml
                    string[] temp = commandArgs[1].Split(" ", 3);
                    if (temp[0] == null || temp[1] == null || temp[2] == null) { operations.argError(); }
                    if (!operations.duplicateAccount(temp[0], accounts))
                    {
                        operations.logNewAccount(temp[0], temp[1], temp[2]);
                        accounts = operations.addAcc(temp[0], accounts);
                        organizations = operations.addOrg(temp[1], organizations);
                        balances = operations.addBal(temp[2], balances);
                        operations.listAcc(accounts, organizations, balances);
                    }
                    continue;
                }
                else if (commandArgs[0] == "setBalance")
                {
                    //schema: "setBalance --accountName --newBalance"
                    //set balance of account to value given
                    string[] temp = commandArgs[1].Split(" ", 2);
                    if (temp[0] == null || temp[1] == null) { operations.argError(); }
                    int index = operations.getIndex(temp[0], accounts);
                    string newBalance = temp[1];
                    balances = operations.setBal(newBalance, index, balances, temp[0]);
                    operations.listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "setOrg")
                {
                    //schema: "setOrg --accountName --newOrg"
                    //replace organization association for account with given string
                    string[] temp = commandArgs[1].Split(" ", 2);
                    if (temp[0] == null || temp[1] == null) { operations.argError(); }
                    int index = operations.getIndex(temp[0], accounts);
                    string newOrg = temp[1];
                    organizations = operations.setOrg(newOrg, index, organizations, temp[0]);
                    operations.listAcc(accounts, organizations, balances);

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
                    /*if( commandArgs[1] == null) { operations.argError(); }
                    string Xlfile = commandArgs[1] + ".xlsx";
                    System.Data.DataTable table = convertXmlToEl();
                    Console.Write(table);
                    
                    //ExportDataTableToExcel(table, Xlfile);*/
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

        static void ExportDataTableToExcel(System.Data.DataTable table, string Xlfile)
        {
            //progressBar1.Value = 0;
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook book = excel.Application.Workbooks.Add(Type.Missing);
            excel.Visible = false;
            excel.DisplayAlerts = false;
            Worksheet excelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)book.ActiveSheet;
            excelWorkSheet.Name = table.TableName;

            //progressBar1.Maximum = table.Columns.Count;
            for (int i = 1; i < table.Columns.Count + 1; i++) // Creating Header Column In Excel  
            {
                excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                /*if (progressBar1.Value < progressBar1.Maximum)
                {
                    progressBar1.Value++;
                    int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                    progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new System.Drawing.Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
                    System.Windows.Forms.Application.DoEvents();
                }*/
            }


            //progressBar1.Maximum = table.Rows.Count;
            for (int j = 0; j < table.Rows.Count; j++) // Exporting Rows in Excel  
            {
                for (int k = 0; k < table.Columns.Count; k++)
                {
                    excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                }

                /*if (progressBar1.Value < progressBar1.Maximum)
                {
                    progressBar1.Value++;
                    int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                    progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new System.Drawing.Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
                    System.Windows.Forms.Application.DoEvents();
                }*/
            }


            book.SaveAs(Xlfile);
            book.Close(true);
            excel.Quit();

            Marshal.ReleaseComObject(book);
            Marshal.ReleaseComObject(book);
            Marshal.ReleaseComObject(excel);

        }
    }
}
