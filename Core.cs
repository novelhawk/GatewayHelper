using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ConnectionSwitcher
{
    public class Core : Form
    {
        private const int WM_HOTKEY = 786;
        private NotifyIcon itemTray;
        private IContainer components;
        private int _id;

        public Core()
        {
#if !DEBUG
            var processes = Process.GetProcessesByName("ConnectionSwitcher");
            if (processes.Length > 1)
            {
                switch (MessageBox.Show(
                    "Other instances of the program are already running. Do you want to kill them and proceed with this one?",
                    "Process is already running", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.No:
                        Environment.Exit(0);
                        return;
                    default:
                        foreach (Process process in processes)
                            process.Kill();
                        break;
                }
            }
#endif

            InitializeComponent();
            CreateHotkey(Modifiers.Control, Keys.F11);
            CreateHotkey(Modifiers.Control, Keys.F12);
        }

        private void CreateHotkey(Modifiers modifiers, Keys key)
        {
            if (!RegisterHotKey(Handle, _id++, (uint)modifiers, (uint)key))
                throw new Exception("Error creating the hotkey");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg != WM_HOTKEY) return;

            //TODO: Change balloons to custom GUI
            switch (m.WParam.ToInt32())
            {
                case 0:
                {
                    //TODO: Make a config and add a list of ips and hotkeys there
                    byte[] ip = GetCurrentGateway();
                    ip[3] = ip[3] == 251 ? (byte) 254 : (byte) 251;
                    switch (ChangeGateway(ip))
                    {
                        case 0:
                            itemTray.ShowBalloonTip(0, "Connection Switch",
                                $"Switched to {ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}", ToolTipIcon.None);
                            break;
                        case 1:
                            // Cancelled by user
                            break;
                        case 2:
                            itemTray.ShowBalloonTip(0, "Connection Switcher",
                                "You must have admin rights to change connection.", ToolTipIcon.Warning);
                            break;
                        default: // -1
                            itemTray.ShowBalloonTip(0, "Connection Switcher", "Unhandled error occurred",
                                ToolTipIcon.Error);
                            break;
                    }
                    break;
                }
                case 1:
                {
                    byte[] ip = GetCurrentGateway();
                    itemTray.ShowBalloonTip(0, "Connection Switcher", $"Currently on {ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}",
                        ToolTipIcon.None);
                    break;
                }
                default:
                    throw new Exception($"Unhandled hotkey. (ID: {m.WParam})");
            }
        }

        private static byte[] GetCurrentGateway() => NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up ||
                            x.OperationalStatus == OperationalStatus.Down)
                .Where(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(x => x.GetIPProperties()?.GatewayAddresses)
                .Select(x => x?.Address)
                .FirstOrDefault(x => x?.AddressFamily == AddressFamily.InterNetwork)?.GetAddressBytes();

        private static int ChangeGateway(byte[] ip)
        {
            Process p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = $"/c route change 0.0.0.0 mask 0.0.0.0 {ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Minimized,
                },
            };
            try
            {
                p.Start();
            }
            catch (Win32Exception)
            {
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
            if (!p.HasExited && !p.WaitForExit(200))
            {
                p.Kill();
                return 2;
            }

            return 0;
        }

        private static void OnItemTrayDoubleClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint identifiers, uint vk);

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Core));
            this.itemTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // itemTray
            // 
            this.itemTray.Icon = ((System.Drawing.Icon)(resources.GetObject("itemTray.Icon")));
            this.itemTray.Visible = true;
            this.itemTray.DoubleClick += OnItemTrayDoubleClick;
            // 
            // Core
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Core";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);

        }
    }
}
