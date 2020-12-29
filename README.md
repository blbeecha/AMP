# AMP
Accounts Management Portal. A Desktop and/or command-line application that allows easy input, sorting, and updating, for any monetary accounts that one client may have. Therefore, reducing time spent on updating excel spreadsheets.

## Installation

TBD

## Required Third-Party Software Installations

TBD

## Usage

--Command-line Application--
First option must be a command specifier, followed by necessary arguments:
  addAccount -accountName -organization -newBalance
  setOrg -accountName -newOrg
  setBalance -accountName -newBalance
  removeAccount -accountName
  listAccounts
  decrement -accountName -decBy
  increment -accountName -incBy
  balanceSum
  
  To terminate the program, use the 'exit' command.
  Following the termination of the program, an XML file with updated account information will be found in the current directory
  
  If you are stuck at any point, try the '--help' command to see the above command specifiers listed in the terminal.

--Desktop Application

## Contributing
This is a personal project. Sharing is allowed, however no changes can be made unless explicit permission is given. Pull requests are not welcome. For major changes, please open an issue first to discuss what you would like to change/add.

Please make sure to update/add tests as appropriate.

## License

TBD
