using System.Text;
using Gtk;
using OllamaSharp;

namespace LocalAI.Views;
class MainView : Box
{
    TextView textView;
    Entry entry;
    StringBuilder stringBuilder = new StringBuilder();
    Uri uri;
    OllamaApiClient ollama;
    List<OllamaSharp.Models.Model> models;
    DropDown modelSelect = DropDown.NewFromStrings(new string[] { "" });
    string[] modelsStrings = new string[] { };
    ConversationContext context = null;

    public MainView()
    {

        uri = new Uri("http://localhost:11434");
        ollama = new OllamaApiClient(uri);

        getModels();

        SetOrientation(Orientation.Vertical);


        ScrolledWindow scrolledWindow = ScrolledWindow.New();
        scrolledWindow.WidthRequest = 600;
        scrolledWindow.HeightRequest = 400;


        textView = TextView.New();
        textView.MarginTop = 12;
        textView.MarginBottom = 12;
        textView.MarginStart = 12;
        textView.MarginEnd = 12;

        textView.Editable = false;
        textView.WrapMode = WrapMode.Word;

        scrolledWindow.Child = textView;
        Append(scrolledWindow);

        Box sendBox = Box.New(Orientation.Horizontal, 2);

        sendBox.Append(modelSelect);

        entry = Entry.New();
        entry.PlaceholderText = "Enter Query Here";
        entry.WidthRequest = 500;
        entry.OnActivate += sendQueryEntered;
        sendBox.Append(entry);

        Button sendButton = Button.New();
        sendButton.IconName = "go-next-symbolic";
        sendButton.GetStyleContext().AddClass("pill");
        sendButton.OnClicked += sendQueryClicked;
        sendBox.Append(sendButton);

        Button clearContextButton = Button.New();
        clearContextButton.IconName = "edit-clear-symbolic";
        clearContextButton.GetStyleContext().AddClass("pill");
        clearContextButton.OnClicked += clearContextClicked;
        sendBox.Append(clearContextButton);


        Append(sendBox);

    }

    private void clearContextClicked(Button sender, EventArgs args)
    {
        context = null;
        textView.GetBuffer().SetText("", -1);
        stringBuilder.Clear();
    }

    private void sendQueryEntered(Entry sender, EventArgs args)
    {
        queryModel();
    }

    private async void getModels()
    {
        List<OllamaSharp.Models.Model> models = (await ollama.ListLocalModels()).ToList();
        modelsStrings = new string[models.Count()];
        // ListStore listStore = new ListStore(GObject.Type.String);
        for (int i = 0; i < models.Count(); i++)
        {
            modelsStrings[i] = models[i].Name;
        }
        // return modelsStrings;
        modelSelect.Model = StringList.New(modelsStrings);

    }

    private void sendQueryClicked(Button sender, EventArgs args)
    {
        queryModel();
    }

    private async void queryModel()
    {
        // select a model which should be used for further operations
        uint selectedIndex = modelSelect.GetSelected();

        ollama.SelectedModel = modelsStrings[selectedIndex];

        // keep reusing the context to keep the chat topic going
        // context = null;
        // context = await ollama.StreamCompletion("How are you today?", context, stream => Console.Write(stream.Response));

        context = await ollama.StreamCompletion(entry.GetText(), context, stream => setText(stream.Response));
        stringBuilder.Append("\n\n");
        textView.GetBuffer().SetText(stringBuilder.ToString(), -1);
    }

    private void setText(string response)
    {
        stringBuilder.Append(response);
        TextBuffer textBuffer = textView.GetBuffer();
        textBuffer.SetText(stringBuilder.ToString(), -1);
        entry.SetText("");
    }

    private void buttonClicked(Button sender, EventArgs args)
    {
        Adw.AboutWindow dialog = Adw.AboutWindow.New();
        dialog.ApplicationName = "LocalAI";
        dialog.Version = "1.0";
        dialog.DeveloperName = "Lucas Benninger";
        dialog.AddLink("Thanks to awaescher for their great OllamaSharp Library", "https://github.com/awaescher/OllamaSharp");
        dialog.Present();
    }
}
