using LocalAI.Views;

namespace LocalAI;
class Program
{
    public static void Main()
    {
        var application = Adw.Application.New("LocalAI", Gio.ApplicationFlags.FlagsNone);
        application.OnActivate += (sender, args) =>
        {
            var window = Gtk.ApplicationWindow.New((Adw.Application)sender);
            window.Title = "LocalAI";
            window.SetDefaultSize(300, 300);
            window.SetChild(new MainView());
            window.Show();
        };
        application.RunWithSynchronizationContext(null);
    }
}
