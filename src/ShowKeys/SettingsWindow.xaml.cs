using System.Windows;

namespace ShowKeys;

public partial class SettingsWindow
{
    public int MaxEntries { get; private set; }
    public int FontSize { get; private set; }

    public SettingsWindow(int currentMaxEntries, int currentFontSize)
    {
        InitializeComponent();
        MaxEntriesTextBox.Text = currentMaxEntries.ToString();
        FontSizeTextBox.Text = currentFontSize.ToString();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        bool isValid = true;
        string errorMessage = "";

        // Validate max entries
        if (int.TryParse(MaxEntriesTextBox.Text, out int maxEntries) && maxEntries > 0)
        {
            MaxEntries = maxEntries;
        }
        else
        {
            isValid = false;
            errorMessage = "Please enter a valid positive number for maximum entries.";
        }

        // Validate font size
        if (int.TryParse(FontSizeTextBox.Text, out int fontSize) && fontSize > 0)
        {
            FontSize = fontSize;
        }
        else
        {
            isValid = false;
            errorMessage = errorMessage.Length > 0 
                ? "Please enter valid positive numbers for all fields." 
                : "Please enter a valid positive number for font size.";
        }

        if (isValid)
        {
            DialogResult = true;
        }
        else
        {
            MessageBox.Show(errorMessage, "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
