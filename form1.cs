//for FormWindow
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms; 

//for Accounts_Management_Portal
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace FormWindow
{
    public class Form1 : Form
    {
        public Button button1;

        public Form1()
        {
            button1 = new Button();
            button1.Size = new Size(40, 40);
            button1.Location = new Point(30, 30);
            button1.Text = "";
            this.Controls.Add(button1);
            button1.Click += new EventHandler(button1_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello World");
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Form1());
            string[] accounts = Accounts_Management_Portal.AMP.initializeAccounts();
            string[] balances = Accounts_Management_Portal.AMP.initializeBalances();
            string[] organizations = Accounts_Management_Portal.AMP.initializeOrganizations();
        }
    }
}

//Accounts Management Portal
// --command line version--

namespace Accounts_Management_Portal
{

    class AMP
    {
        public static string[] initializeAccounts()
        {
            //create file if doesn't exist, open if it does
            var fileName = "savedData.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = Path.Combine(currentDirectory, fileName);

            FileStream fs = new FileStream(purchaseOrderFilepath, FileMode.OpenOrCreate);


            //if the file was just created, return a string array of arbitrary length
            if (new FileInfo(fileName).Length == 0)
            {
                fs.Close();
                return new string[2];
            }
            fs.Close();

            //close file after length check, open XML doc file to parse tree for accounts

            XDocument doc = XDocument.Load(purchaseOrderFilepath);
            XElement root = doc.Element("Root");

            IEnumerable<XElement> list = root.Elements();

            int size = 0;
            foreach (XElement element in list)
            {
                size++;
            }

            string[] accArray = new string[size + 1];   //initialize string array one larger than current number of XML tree children
            size = 0;

            //load existing accounts into accounts array
            foreach (XElement element in list)
            {
                accArray[size] = element.Attribute("Name").Value;
                size++;
            }

            return accArray;
        }

        public static string[] initializeOrganizations()
        {
            //open file, it will be created in initializeAccounts if it doesn't exist
            var fileName = "savedData.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = Path.Combine(currentDirectory, fileName);

            FileStream fs = new FileStream(purchaseOrderFilepath, FileMode.Open);

            //if the file was just created, return a string array of arbitrary length
            if (new FileInfo(fileName).Length == 0)
            {
                fs.Close();
                return new string[2];
            }
            fs.Close();

            XDocument doc = XDocument.Load(purchaseOrderFilepath);
            XElement root = doc.Element("Root");

            IEnumerable<XElement> list = root.Elements();

            int size = 0;
            foreach (XElement element in list)
            {
                size++;
            }

            string[] orgArray = new string[size + 1];   //initialize string array one larger than current number of XML tree children
            size = 0;

            foreach (XElement element in list)
            {
                orgArray[size] = element.Element("Organization").Value;
                size++;
            }

            return orgArray;
        }

        public static string[] initializeBalances()
        {
            //open file, it will be created in initializeAccounts if it doesn't exist
            var fileName = "savedData.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = Path.Combine(currentDirectory, fileName);

            FileStream fs = new FileStream(purchaseOrderFilepath, FileMode.Open);

            //if the file was just created, return a string array of arbitrary length
            if (new FileInfo(fileName).Length == 0)
            {
                fs.Close();
                return new string[2];
            }
            fs.Close();

            XDocument doc = XDocument.Load(purchaseOrderFilepath);
            XElement root = doc.Element("Root");

            IEnumerable<XElement> list = root.Elements();

            int size = 0;
            foreach (XElement element in list)
            {
                size++;
            }

            string[] balArray = new string[size + 1];   //initialize string array one larger than current number of XML tree children
            size = 0;

            foreach (XElement element in list)
            {
                balArray[size] = element.Element("Balance").Value;
                size++;
            }

            return balArray;
        }

        static void constructXML(string[] accounts, string[] organizations, string[] balances)
        {
            XElement[] XArray = new XElement[accounts.Length];
            int Xindex = 0;

            XElement accXMLTree = new XElement("Root");

            foreach (string account in accounts)
            {
                if (account != null)
                {
                    accXMLTree.Add(new XElement("Account",
                        new XAttribute("Name", account),
                        new XElement("Organization", organizations[Xindex]),
                        new XElement("Balance", balances[Xindex]))
                    );
                }

                Xindex++;
            }

            var fileName = "savedData.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = Path.Combine(currentDirectory, fileName);

            accXMLTree.Save(purchaseOrderFilepath);
            String str = File.ReadAllText(purchaseOrderFilepath);
            Console.WriteLine(str);
        }

        static string[] incBal(int index, string incBy, string[] balances)
        {
            //increment account
            int incValue = (Int32.Parse(incBy));
            int curBal = (Int32.Parse(balances[index]));

            balances[index] = (Convert.ToString(curBal + incValue));
            return balances;
        }

        static string[] decBal(int index, string decBy, string[] balances)
        {
            //decrement account
            int decValue = (Int32.Parse(decBy));
            int curBal = (Int32.Parse(balances[index]));

            balances[index] = (Convert.ToString(curBal - decValue));
            return balances;
        }

        static string[] setBal(string newBalance, int index, string[] balances)
        {
            //set new balance
            balances[index] = newBalance;
            return balances;
        }

        static string[] addAcc(string newAccount, string[] accounts)
        {
            //add new account to account array
            string[] newAccounts = new string[accounts.Length + 1];

            int index = 0;
            foreach (string account in accounts)
            {
                newAccounts[index] = account;
                index++;
            }

            newAccounts[newAccounts.Length - 1] = newAccount;

            return newAccounts;
        }

        static string[] addOrg(string newOrg, string[] organizations)
        {
            //add new organization to organization array
            string[] newOrgs = new string[organizations.Length + 1];

            int index = 0;
            foreach (string org in organizations)
            {
                newOrgs[index] = org;
                index++;
            }

            newOrgs[newOrgs.Length - 1] = newOrg;

            return newOrgs;
        }

        static string[] addBal(string newBal, string[] balances)
        {
            //add new balance to balance array
            string[] newBalances = new string[balances.Length + 1];

            int index = 0;
            foreach (string balance in balances)
            {
                newBalances[index] = balance;
                index++;
            }

            newBalances[newBalances.Length - 1] = newBal;

            return newBalances;
        }

        static void listAcc(string[] accounts, string[] organizations, string[] balances)
        {
            //list all accounts currently stored for user review (command line at first, maybe modal change later)
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Accounts             Organizations               Balances");
            Console.WriteLine("----------------------------------------------------------");
            int orgSpace = 21;
            int balSpace = 28;
            int xsize = 0, ysize = 0;
            for (int index = 0; index < accounts.Length; index++)
            {
                if (accounts[index] != null)
                {
                    Console.Write(accounts[index]);
                    xsize = accounts[index].Length;
                    while (xsize < orgSpace) { Console.Write(" "); xsize++; }

                    if (organizations[index] == null) { Console.WriteLine("IT BAD: index = " + index); }
                    Console.Write(organizations[index]);
                    ysize = organizations[index].Length;
                    while (ysize < balSpace) { Console.Write(" "); ysize++; }

                    Console.Write(balances[index] + "\n");
                }
            }
        }

        static string[] remAcc(int index, string[] accounts)
        {
            //remove target account from account array
            accounts[index] = null;
            return accounts;
        }

        static string[] remOrg(int index, string[] organizations)
        {
            //remove target organization from organizations array
            organizations[index] = null;
            return organizations;
        }

        static string[] remBal(int index, string[] balances)
        {
            //remove target balance from balances array
            balances[index] = null;
            return balances;
        }

        static int getIndex(string targetAccount, string[] accounts)
        {
            //get index that matches the associated account
            int index = -1;

            for (int i = 0; i < accounts.Length; i++)
            {
                if (targetAccount == accounts[i])
                {
                    index = i;
                }
            }

            return index;
        }

        static string[] setOrg(string newOrg, int index, string[] organizations)
        {
            //replace organization of targetAccount with given newOrg
            organizations[index] = newOrg;
            return organizations;
        }

        static void AllBalanceSum(string[] balances)
        {
            //output total balance between all accounts
            int[] intArray = new int[balances.Length];
            for (int index = 0; index < balances.Length; index++)
            {
                if (balances[index] != null)
                {
                    intArray[index] = (Int32.Parse(balances[index]));
                }
            }

            Console.WriteLine(intArray.Sum());
        }

        /*
        static void Main(string[] args)
        {

            string[] accounts = initializeAccounts();
            string[] organizations = initializeOrganizations();
            string[] balances = initializeBalances();

            string input;
            string[] commandArgs;

            //main program loop
            
            do
            {
                //accept user input for command
                Console.Write("What would you like to do?: ");
                input = Console.ReadLine();
                commandArgs = input.Split(" ", 2);

                if (commandArgs[0] == "increment")
                {
                    //schema: "incBy --accountName --incBy"
                    //increment account by value given
                    string[] temp = commandArgs[1].Split(" ", 3);
                    int index = getIndex(temp[0], accounts);
                    balances = incBal(index, temp[1], balances);
                    listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "decrement")
                {
                    //schema: "decBal --accountName --decBy"
                    //decrement account by value given
                    string[] temp = commandArgs[1].Split(" ", 2);
                    int index = getIndex(temp[0], accounts);
                    balances = decBal(index, temp[1], balances);
                    listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "listAccounts")
                {
                    //schema: "listAccounts"
                    //list current accounts and associated information
                    listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "removeAccount")
                {
                    //schema: "removeAccount --accountName"
                    //remove account from xml
                    int index = getIndex(commandArgs[1], accounts);
                    accounts = remAcc(index, accounts);
                    organizations = remOrg(index, organizations);
                    balances = remBal(index, balances);
                    listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "addAccount")
                {
                    //schema: "addAccount --accountName --organization --newBalance"
                    //add account to xml
                    string[] temp = commandArgs[1].Split(" ", 3);
                    accounts = addAcc(temp[0], accounts);
                    organizations = addOrg(temp[1], organizations);
                    balances = addBal(temp[2], balances);
                    listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "setBalance")
                {
                    //schema: "setBalance --accountName --newBalance"
                    //set balance of account to value given
                    string[] temp = commandArgs[1].Split(" ", 2);
                    int index = getIndex(temp[0], accounts);
                    string newBalance = temp[1];
                    balances = setBal(newBalance, index, balances);
                    listAcc(accounts, organizations, balances);
                    continue;
                }
                else if (commandArgs[0] == "setOrg")
                {
                    //schema: "setOrg --accountName --newOrg"
                    //replace organization association for account with given string
                    string[] temp = commandArgs[1].Split(" ", 2);
                    int index = getIndex(temp[0], accounts);
                    string newOrg = temp[1];
                    organizations = setOrg(newOrg, index, organizations);
                    listAcc(accounts, organizations, balances);

                }
                else if (commandArgs[0] == "balanceSum")
                {
                    //schema: "balanceSum"
                    //print sum of all balances currently in system
                    AllBalanceSum(balances);
                }
                else if (commandArgs[0] == "--help")
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("First option must be a command specifier, followed by necessary arguments:");
                    Console.WriteLine("\taddAccount -accountName -organization -newBalance");
                    Console.WriteLine("\tsetOrg -accountName -newOrg");
                    Console.WriteLine("\tsetBalance -accountName -newBalance");
                    Console.WriteLine("\tremoveAccount -accountName");
                    Console.WriteLine("\tlistAccounts");
                    Console.WriteLine("\tdecrement -accountName -decBy");
                    Console.WriteLine("\tincrement -accountName -incBy");
                    Console.WriteLine("\tbalanceSum");

                    Console.WriteLine("To terminate the program, use the 'exit' command");
                    Console.WriteLine("Following the termination of the program, an XML file with updated account information will be found in the current directory");
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------");

                }
                else
                {
                    Console.WriteLine("No valid command given. Try Again or type --help");
                }
            }
            while (commandArgs[0] != "exit");       //end when user inputs terminal command
            

            constructXML(accounts, organizations, balances);

        }
        */
    }
}


