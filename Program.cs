using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Controller;

static class Program
{
    [STAThread]
 
    static bool CheckCommand(List<string> listOfCommands, string enteredCommand)
    {
        foreach(string currentCommand in listOfCommands)
        {
            string cmd = enteredCommand.Trim().ToLower();
            if(cmd == currentCommand || cmd.StartsWith(currentCommand + " ")) return true;
        }
        return false;
    }

    static void Main()
    {
        ApplicationConfiguration.Initialize();
        // Collecting some user data
        //! Note: only YOU can see this. This data isn't shared anywhere with anyone
        string currentTime = DateTime.Now.ToString("HH:mm:ss");
        string machineName = Environment.MachineName;
        string userName = Environment.UserName;
        string OSInfo = Environment.OSVersion.ToString();
        string currentDirectory = Environment.CurrentDirectory;

        Form logForm = new Form();
        logForm.Text = "Log";
        logForm.BackColor = Color.Black;
        logForm.ForeColor = Color.Red;
        logForm.Font = new Font("Fira Code", 10);
        logForm.Size = new Size(500, 500);
        logForm.StartPosition = FormStartPosition.Manual;


        Form mainForm = new Form();
        mainForm.Text = "Controller";
        mainForm.BackColor = Color.Black;
        mainForm.ForeColor = Color.Green;
        mainForm.Font = new Font("Lucida Console", 10);
        mainForm.StartPosition = FormStartPosition.CenterScreen;
        mainForm.Size = new Size(800, 500);

        Form infoPanel = new Form();
        infoPanel.Text = "Info Panel";
        infoPanel.BackColor = Color.Black;
        infoPanel.ForeColor = Color.White;
        infoPanel.Font = new Font("Fira Code", 10);
        infoPanel.Size = new Size(400, 500);
        infoPanel.StartPosition = FormStartPosition.Manual;

        mainForm.Load += (s, e) =>
        {
            logForm.Location = new Point(mainForm.Location.X + 785, mainForm.Location.Y);
            infoPanel.Location = new Point(mainForm.Location.X - 385, mainForm.Location.Y);
            logForm.Show();
            infoPanel.Show();
        };

        


        //? Widgets
        RichTextBox resultLabel = new RichTextBox();
        resultLabel.Text = "This app was made in C# via WinForms by the user Yikebones (2026.01.07). For a list of commands type `--help` or `help`.";
        resultLabel.AutoSize = false;
        resultLabel.Size = new Size(785, 375);
        resultLabel.Font = new Font("Fira Code", 10);
        resultLabel.ReadOnly = true;
        resultLabel.ScrollBars = RichTextBoxScrollBars.Vertical;
        resultLabel.ForeColor = Color.Lime;
        resultLabel.BackColor = Color.Black;
        resultLabel.BorderStyle = BorderStyle.FixedSingle;


        TextBox commandLine = new TextBox();
        commandLine.Size = new Size(785, 75);
        commandLine.Location = new Point(0, 377);
        commandLine.BackColor = Color.Black;
        commandLine.ForeColor = Color.Lime;
        commandLine.Font = new Font("Monolisa", 10); // if you don't have monolisa on your system, then the font is gonna look bad

        RichTextBox log = new RichTextBox();
        log.Text = "This is your log. Your last commands will be shown here.";
        log.AutoSize = false;
        log.Size = new Size(484, 484);
        log.ReadOnly = true;
        log.ScrollBars = RichTextBoxScrollBars.Vertical;
        log.BorderStyle = BorderStyle.FixedSingle;
        log.ForeColor = Color.Red;
        log.BackColor = Color.Black;

        Label userInfo = new Label();
        userInfo.Text = $"CWD: {currentDirectory}\n\nUsername: {userName}\nMachine name: {machineName}\nOS: {OSInfo}\nCurrent Time: {currentTime}";
        userInfo.TextAlign = ContentAlignment.TopLeft;
        userInfo.BackColor = Color.Black;
        userInfo.ForeColor = Color.White;
        userInfo.Font = new Font("Fira Code", 10);
        userInfo.Dock = DockStyle.Fill;
        userInfo.Padding = new Padding(10);

        //! functionalities, events and systems
        commandLine.KeyDown += (s, e) =>
        {
            // this basically senses when a key is pushed. We'll find out which key
            if(e.KeyCode == Keys.Enter)
            {
                string command = commandLine.Text.ToLower();
                string commandU = commandLine.Text; // normal command
                commandLine.Text = ""; // clear out the textbox

                List<string> commands = new List<string> {"--help", "help", "clear", "cls", "ls", "pwd", "pud", "printmydata", "exit", "quit",
                "cfile", "touch", "createfile", "cfile ", "touch ", "createfile ", "clearlog"
                };

                if(CheckCommand(commands, commandU))
                {
                    log.AppendText($"\n[{currentTime}] {userName} executed the command: {commandU}.");
                }else {
                    resultLabel.AppendText($"\n\n\n`{commandU}` is not a valid command.");
                    log.AppendText($"\n\n[{currentTime}] {userName} tried to execute the invalid command: `{commandU}`.");
                }
                //! using else-if statements for the system-like commands (file creation / deletion, folder creation/deletion, etc...)
                //? `touch <filename>`, `cfile <filename>`, `createFile <filename>` triggers this if statement
                if(command.StartsWith("touch ") || command.StartsWith("cfile ") || command.StartsWith("createfile "))
                {
                    string path = "";

                    if(command.StartsWith("touch ") || command.StartsWith("cfile "))
                    {
                        // `touch ` and `cfile ` is the same length, so that is convinient
                        path = commandU["touch ".Length..]; // same as command["cfile ".Length..].
                    } else if(command.StartsWith("createfile "))
                    {
                        path = commandU["createFile ".Length..];
                    }

                    // check if the given path already exists
                    if(!File.Exists(path))
                    {
                        File.Create(path).Close(); // if it doesnt exist, create it
                        resultLabel.AppendText($"\nSuccessfully created file '{path}'.");
                    } else
                    {
                        resultLabel.AppendText($"\nFile '{path}' already exists.");
                    }

                //? this else-if statement gets triggered by `touch`, `cfile`, `createFile` (when no path is specified)
                } else if(command.StartsWith("touch") || command.StartsWith("cfile") || command.StartsWith("createFile "))
                {
                    resultLabel.AppendText($"Usage: {command} <filename>");
                }



                //! Using switch for the minor commands (like `--help`, `clear`, `ls`, etc...)
                switch(command)
                {
                    case "":
                    case " ":
                        break;
                    case "help":
                    case "--help":
                        resultLabel.AppendText(@"
                        
                        
help, --help                        -> shows this menu
ls                                  -> list the contents of your currently working directory
pwd                                 -> prints working directory
pud, PrintUserData                  -> Prints all the data the app knows about you
clear, cls                          -> clear terminal
clearLog                            -> clear log (duh)
exit, quit                          -> terminate running process
touch, cfile, createFile <filename> -> Create a file with the given name");

                        break;
                    case "clear":
                    case "cls":
                        resultLabel.Clear(); // resultLabel.Text = ""; would work the same
                        log.AppendText($"\n[{currentTime}] {userName} cleared terminal.");
                        break;
                    case "pwd":
                        resultLabel.AppendText($"\n\n\nCurrently Working Directory: {Environment.CurrentDirectory}");
                        break;
                    
                    case "clearlog":
                        log.Clear();
                        break;
                    
                    case "pud":
                    case "printuserdata":
                        resultLabel.AppendText($"\n\n\nEverything this app knows about you:\nCurrent time: {currentTime}\nYour username: {userName}\nYour machine's name: {machineName}\nYour OS: {OSInfo}\nCWD: {currentDirectory}\nNote: No one sees this information, except for you and the app.");
                        break;

                    case "quit":
                    case "exit":
                        Application.Exit();
                        break;
                    case "ls":
                        resultLabel.AppendText($"\n\nContent of {currentDirectory}:\n");
                        List<string> folderNames = new List<string>(); // for sorting and clearer output purposes
                        List<string> fileNames = new List<string>();   // for sorting and clearer output purposes
                        foreach(string item in Directory.GetFileSystemEntries(currentDirectory))
                        {
                            string shortFileName = Path.GetFileName(item);

                            if(Directory.Exists(item)) folderNames.Add(shortFileName); // if item is a folder, add it to the folders list
                            else fileNames.Add(shortFileName); // if the item is a file, add it to the file list
                        }

                        // now comes the sorting part
                        // iterate through our folderNames array and print everything
                        // then do the same with fileNames array
                        foreach(string iterator in folderNames)
                        {
                            resultLabel.AppendText($"{iterator}\t\t[FOLDER]\n");
                        }

                        foreach(string iterator in fileNames)
                        {
                            resultLabel.AppendText($"{iterator}\n");
                        }


                        resultLabel.AppendText("\n");
                        break;
                }
                e.SuppressKeyPress = true;
            }
        };

        mainForm.Controls.Add(commandLine);
        mainForm.Controls.Add(resultLabel);
        logForm.Controls.Add(log);
        infoPanel.Controls.Add(userInfo);
        Application.Run(mainForm);
    }    
}