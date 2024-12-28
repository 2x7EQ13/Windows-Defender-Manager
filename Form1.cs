using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace WinDefendMan
{
    public partial class Form1 : Form
    {
        private Timer timer;
        PowerShell powerShell;
        NotifyIcon notifyIcon;
        string logFile = "RunLog.txt";
        public Form1()
        {
            InitializeComponent();
            notifyIcon = new NotifyIcon
            {
                Icon = Resource1.broken_shield_60, // Set your icon here
                Text = "Defender Manager",
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Exit", (s, e) => Application.Exit())
                })
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            string tempFolderPath = Path.GetTempPath();
            logFile = Path.Combine(tempFolderPath,logFile);
            try
            {
                File.WriteAllText(logFile, ""); //clear log file
                powerShell = PowerShell.Create();
                //test
                //bool tmp = GetTamperProtectionStatus();
                //tmp = GetRealtimeProtectionStatus();
            } catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message + Environment.NewLine);
                this.Close();
            }
            string currentRunningPath2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (currentRunningPath2.Contains(@"Program Files"))
            {
                File.AppendAllText(logFile, "Running as Schedule Task" + Environment.NewLine);
                //disable loud delivered and sample submission
                DisableCloudDelivered();
                DisableSubmitSamples();               
                //running in Program Files path, this is run from task schedule
                timer = new Timer();
                timer.Interval = 60000; // Set the interval to 60,000 milliseconds (1 minute)
                timer.Tick += Timer_Tick; // Subscribe to the Tick event
                timer.Start(); // Start the timer
                //
                notifyIcon.BalloonTipTitle = "Windows Defender Manager"; // Set the title of the balloon tip
                notifyIcon.BalloonTipText = "Real time protection is turned off"; // Set the message of the balloon tip
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info; // Set the icon type (Info, Warning, Error)
                // Show the balloon tip for 5 seconds (5000 milliseconds)
                notifyIcon.ShowBalloonTip(5000);
            }
            else
            {
                //this is when user double click on installer
                Form2 form2 = new Form2();
                DialogResult dialogResult = form2.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if(GetTamperProtectionStatus())
                    {
                        MessageBox.Show("You must manual Turn Off 'Tamper Protection' first!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    if (!CreateLogonTask())
                    {
                        MessageBox.Show("Install Manager failed. Check log file for more infomations", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //disable now, without re-login
                        DisableCloudDelivered();
                        DisableSubmitSamples();
                        DisableRealtimeMonitoring();
                        MessageBox.Show("Install Manager successfully");
                    }
                }
                else
                {
                    this.Close();
                }
                this.Close();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // This code will run every minute
            if(GetRealtimeProtectionStatus())
            {
                DisableRealtimeMonitoring();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer != null)
                timer.Stop();
        }
        bool GetTamperProtectionStatus()
        {
            powerShell.Commands.Clear();
            powerShell.AddScript("Get-MpComputerStatus | Select-Object -ExpandProperty IsTamperProtected");
            try
            {
                var results = powerShell.Invoke();
                if (powerShell.HadErrors)
                {
                    string err = "";
                    foreach (var error in powerShell.Streams.Error)
                    {
                        err = err + error;
                    }
                    File.AppendAllText(logFile, err + Environment.NewLine);
                }
                else if (results.Count > 0)
                {
                    // Get the IsTamperProtected value
                    bool isTamperProtected = (bool)results[0].BaseObject;
                    return isTamperProtected;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message + Environment.NewLine);
                return false;
            }
            return false;
        }
        bool GetRealtimeProtectionStatus()
        {
            powerShell.Commands.Clear();
            powerShell.AddScript("Get-MpComputerStatus | Select-Object -ExpandProperty RealTimeProtectionEnabled");
            try
            {
                var results = powerShell.Invoke();
                if (powerShell.HadErrors)
                {
                    string err = "";
                    foreach (var error in powerShell.Streams.Error)
                    {
                        err = err + error;
                    }
                    File.AppendAllText(logFile, err + Environment.NewLine);
                }
                else if (results.Count > 0)
                {
                    // Get the IsTamperProtected value
                    bool isTamperProtected = (bool)results[0].BaseObject;
                    return isTamperProtected;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message + Environment.NewLine);
                return false;
            }
            return false;
        }
        bool DisableCloudDelivered()
        {
            powerShell.Commands.Clear();
            powerShell.AddScript("Set-MpPreference -MAPSReporting Disabled");
            try
            {
                var results = powerShell.Invoke();
                if (powerShell.HadErrors)
                {
                    string err = "";
                    foreach (var error in powerShell.Streams.Error)
                    {
                        err = err + error;
                    }
                    File.AppendAllText(logFile, err + Environment.NewLine);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message + Environment.NewLine);
                return false;
            }
            return false;
        }
        bool DisableSubmitSamples()
        {
            powerShell.Commands.Clear();
            powerShell.AddScript("Set-MpPreference -SubmitSamplesConsent NeverSend");
            try
            {
                var results = powerShell.Invoke();
                if (powerShell.HadErrors)
                {
                    string err = "";
                    foreach (var error in powerShell.Streams.Error)
                    {
                        err = err + error;
                    }
                    File.AppendAllText(logFile, err + Environment.NewLine);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message + Environment.NewLine);
                return false;
            }
            return false;
        }
        bool DisableRealtimeMonitoring()
        {
            powerShell.Commands.Clear();
            powerShell.AddScript("Set-MpPreference -DisableRealtimeMonitoring $true");
            try
            {
                var results = powerShell.Invoke();
                if (powerShell.HadErrors)
                {
                    string err = "";
                    foreach (var error in powerShell.Streams.Error)
                    {
                        err = err + error;
                    }
                    File.AppendAllText(logFile, err + Environment.NewLine);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message + Environment.NewLine);
                return false;
            }
            return false;
        }

        bool CreateLogonTask()
        {
            bool bResult = true;
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string newFolderPath = Path.Combine(programFilesPath, "WinDefendMan");
            try
            {
                // Create the directory if it doesn't exist
                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }
                // Get the current executable's directory
                string currentExePath = AppDomain.CurrentDomain.BaseDirectory;
                string[] files = Directory.GetFiles(currentExePath);
                foreach (string file in files)
                {
                    string destFile;
                    if (Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        // Rename .exe files to WinDefendMan.exe
                        destFile = Path.Combine(newFolderPath, "WinDefendMan.exe");
                    }
                    else
                    {
                        destFile = Path.Combine(newFolderPath, Path.GetFileName(file));
                    }
                    File.Copy(file, destFile, true);
                }
                //sample create task: schtasks /create /sc onlogon /tn "MyElevatedTask" /tr "C:\Windows\System32\cmd.exe /k" /rl HIGHEST /f
                string taskName = "Windows Defender Manager Task";
                string taskCommand = "\'" + newFolderPath + @"\WinDefendMan.exe" + "\'";
                string schtasksCommand = $"/create /sc onlogon /tn \"{taskName}\" /tr \"{taskCommand}\" /rl HIGHEST /f";
                // Create a new process to run the schtasks command
                ProcessStartInfo processInfo = new ProcessStartInfo("schtasks", schtasksCommand)
                {
                    RedirectStandardOutput = true, // Redirect the output
                    RedirectStandardError = true, // Redirect the error output (optional)
                    UseShellExecute = false, // Required for redirection
                    CreateNoWindow = true, // Optional: do not create a window,
                    Verb = "runas"
                };
                Process process = new Process();
                process.StartInfo = processInfo;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        File.AppendAllText(logFile, e.Data + "\r\n");
                    }
                };

                // Subscribe to error data received event (optional)
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        File.AppendAllText(logFile, e.Data + "\r\n");
                    }
                };
                process.Start();
                process.WaitForExit(); // Wait for the process to complete
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, "An error occurred: " + ex.Message);
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bResult = false;
            }
            return bResult;
        }
    }
}
