using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ConnectionSwitcher
{
    public class Core : Form
    {
        private NotifyIcon itemTray;
        private IContainer components;
        private bool _succeded;

        public Core()
        {
            InitializeComponent();
            CreateHotkey();
        }

        private void CreateHotkey()
        {
            if (RegisterHotKey(Handle, 0, 16390, 79))
                _succeded = true;
            else
                throw new Exception("Error creating the hotkey");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!_succeded) return;
            if (m.Msg == 0x312) // WM_HOTKEY //TODO: As more hotkeys are gonna get added add a system to check which was pressed
            {
                byte newConnection = (byte)(GetCurrentGateway()[3] == 254 ? 251 : 254); //TODO: Make a config and add a list of ips and hotkeys there
                int code = ChangeGateway(192, 168, 1, newConnection);
                switch (code)
                {
                    case 0:
                        itemTray.ShowBalloonTip(100, "Connection Switch", $"Now on 192.168.1.{newConnection}", ToolTipIcon.None);
                        break;
                    case 1:
                        // Cancelled by user
                        break;
                    case 2:
                        itemTray.ShowBalloonTip(100, "Connection Switcher", "You need admin authorization for change connection.", ToolTipIcon.Warning);
                        break;
                    default: // -1
                        itemTray.ShowBalloonTip(100, "Connection Switcher", "Unhandled error occurred", ToolTipIcon.Error);
                        break;
                }
            }
        }

        private static byte[] GetCurrentGateway() => NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up ||
                            x.OperationalStatus == OperationalStatus.Down)
                .Where(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(x => x.GetIPProperties()?.GatewayAddresses)
                .Select(x => x?.Address)
                .FirstOrDefault(x => x?.AddressFamily == AddressFamily.InterNetwork)?.GetAddressBytes();

        private static int ChangeGateway(byte first, byte second, byte third, byte fourth)
        {
            Process p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = $"/k route change 0.0.0.0 mask 0.0.0.0 {first}.{second}.{third}.{fourth} && exit",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Minimized,
                },
            };
            try
            {
                p.Start();
            }
            catch (Win32Exception) // Could be caused by something else... Using OperationCanceledException 2hard
            {
                return 1; // Cancelled by user
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
            this.itemTray.Text = "Connection Switcher";
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

        private static void OnItemTrayDoubleClick(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
