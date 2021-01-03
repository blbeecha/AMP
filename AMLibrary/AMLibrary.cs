/*
 * AUTHOR: Brendan Beecham
 * FILE NAME: AMLibrary.cs
 * SOLUTION: AMP(Accounts Management Portal)
 * PROGRAM SCOPE: .NET 5.0 Library
 * PROGRAM PURPOSE:
 *      The purpose of this library is to assist with operation commands on the account information.
 *      The scope of the operations is contained (as best as possible) within this library to better
 *      abstract the solution. Additionally, the library maintains a JSON log within the project directory
 *      to detail each session of the application. The JSON log is overwritten upon each usage so only the
 *      most recent session is recorded.
 * CALLED BY:
 *      Program.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace AMLibrary
{

    public class AMOperations
    {
        JsonWriter writer;

        public AMOperations()
        {
            //constructor
            StreamWriter logFile = File.CreateText("AMOperationslog.json");
            logFile.AutoFlush = true;
            writer = new JsonTextWriter(logFile);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("Operations");
            writer.WriteStartArray();
        }

        public void argError()
        {
            Console.WriteLine("The necessary number of arguments were not input. Try '--help' to see command schema.\n");
        }

        public string[] initializeAccounts()
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

        public string[] initializeOrganizations()
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

        public string[] initializeBalances()
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

        public void constructXML(string[] accounts, string[] organizations, string[] balances)
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
            //Console.WriteLine(str);
        }

        public string[] incBal(int index, string incBy, string[] balances, string account)
        {
            //increment account
            int incValue = (Int32.Parse(incBy));
            int curBal = (Int32.Parse(balances[index]));

            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue("Increment Balance");
            writer.WritePropertyName("Account Name");
            writer.WriteValue(account);
            writer.WritePropertyName("Increment By");
            writer.WriteValue(incValue);
            writer.WritePropertyName("Current Balance");
            writer.WriteValue(curBal);

            balances[index] = (Convert.ToString(curBal + incValue));
            writer.WritePropertyName("Result");
            writer.WriteValue(balances[index]);
            writer.WriteEndObject();

            return balances;
        }

        public string[] decBal(int index, string decBy, string[] balances, string account)
        {
            //decrement account
            int decValue = (Int32.Parse(decBy));
            int curBal = (Int32.Parse(balances[index]));

            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue("Decrement Balance");
            writer.WritePropertyName("Account Name");
            writer.WriteValue(account);
            writer.WritePropertyName("Decrement By");
            writer.WriteValue(decValue);
            writer.WritePropertyName("Current Balance");
            writer.WriteValue(curBal);

            balances[index] = (Convert.ToString(curBal - decValue));
            writer.WritePropertyName("Result");
            writer.WriteValue(balances[index]);
            writer.WriteEndObject();

            return balances;
        }

        public string[] setBal(string newBalance, int index, string[] balances, string account)
        {
            //set new balance
            balances[index] = newBalance;
            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue(account);
            writer.WritePropertyName("Account Name");
            writer.WriteValue(account);
            writer.WritePropertyName("New Balance");
            writer.WriteValue(newBalance);
            writer.WriteEndObject();

            return balances;
        }

        public void logNewAccount(string account, string org, string bal)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue("Add Account");
            writer.WritePropertyName("Account Name");
            writer.WriteValue(account);
            writer.WritePropertyName("Organization");
            writer.WriteValue(org);
            writer.WritePropertyName("Balance");
            writer.WriteValue(bal);
            writer.WriteEndObject();
        }

        public Boolean duplicateAccount(string newAccount, string[] accounts)
        {
            //ensure that the account name to be added isn't already in accounts
            if (accounts.Contains(newAccount))
            {
                Console.WriteLine("The account name input already exists in the system. Please Try again.");
                return true;
            }
            return false;
        }

        public string[] addAcc(string newAccount, string[] accounts)
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

        public string[] addOrg(string newOrg, string[] organizations)
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

        public string[] addBal(string newBal, string[] balances)
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

        public void listAcc(string[] accounts, string[] organizations, string[] balances)
        {
            //list all accounts currently stored for user review (command line at first, maybe modal change later)
            Console.WriteLine("Here are the current accounts saved...");
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

                    Console.Write(organizations[index]);
                    ysize = organizations[index].Length;
                    while (ysize < balSpace) { Console.Write(" "); ysize++; }

                    Console.Write(balances[index] + "\n");
                }
            }
        }

        public string[] remAcc(int index, string[] accounts, string accountName)
        {
            //remove target account from account array
            accounts[index] = null;
            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue("Remove Account");
            writer.WritePropertyName("Account Name");
            writer.WriteValue(accountName);
            writer.WriteEndObject();
            return accounts;
        }

        public string[] remOrg(int index, string[] organizations)
        {
            //remove target organization from organizations array
            organizations[index] = null;
            return organizations;
        }

        public string[] remBal(int index, string[] balances)
        {
            //remove target balance from balances array
            balances[index] = null;
            return balances;
        }

        public int getIndex(string targetAccount, string[] accounts)
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

        public string[] setOrg(string newOrg, int index, string[] organizations, string account)
        {
            //replace organization of targetAccount with given newOrg
            organizations[index] = newOrg;
            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue("Set Organization");
            writer.WritePropertyName("Account Name");
            writer.WriteValue(account);
            writer.WritePropertyName("New Organization");
            writer.WriteValue(newOrg);
            writer.WriteEndObject();
            return organizations;
        }

        public void AllBalanceSum(string[] balances)
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

            int sum = intArray.Sum();

            writer.WriteStartObject();
            writer.WritePropertyName("Operation");
            writer.WriteValue("AllBalanceSum");
            writer.WritePropertyName("Sum");
            writer.WriteValue(sum);
            writer.WriteEndObject();
            Console.WriteLine(sum);
        }

        public void Finish()    //write to JSON log file and close writer
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Close();
        }

    }
}
