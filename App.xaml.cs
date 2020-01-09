using Microsoft.Win32;
using System;
using System.Media;
using System.Windows;
using System.Windows.Forms;
using System.Threading;


namespace SilentApp
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>private NotifyIcon trayIcon;
    /// 

    public interface IOnPlayFinishedListener {
        void OnPlayFinished();
    }

    public class PlayerWork
    {
        private readonly IOnPlayFinishedListener listener;

        public PlayerWork(IOnPlayFinishedListener listener) {
            this.listener = listener;
        }

        public void PlaySound() {
            GC.Collect();
            SoundPlayer soundPlayer = new SoundPlayer(SilentApp.Properties.Resources.silence);
            soundPlayer.PlaySync();
            soundPlayer.Stop();
            listener.OnPlayFinished();
        }   
    }

    public partial class App : System.Windows.Application, IOnPlayFinishedListener
    {
        private NotifyIcon trayIcon;

        protected override void OnStartup(StartupEventArgs e) {
            OnLoad();
            PlaySound();
            AddApplicationToStartup();
        }

        public void OnPlayFinished() {
            PlaySound();
        }

        private void OnLoad()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = SilentApp.Properties.Resources.AppIcon,
                Text = "Silent App",
                ContextMenu = new ContextMenu(
                    new MenuItem[] {
                    new MenuItem("Exit", CloseApp)
                }),
                Visible = true
            };
        }

        private void PlaySound()
        {
            PlayerWork playerWork = new PlayerWork(this);
            Thread t = new Thread(playerWork.PlaySound);
            t.Start();
        }

        private void CloseApp(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            Environment.Exit(0);
        }

        public void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("Silent App", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
            }
        }
    }
}
