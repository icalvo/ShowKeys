using System.Windows;

namespace ShowKeys;

public partial class SettingsWindow
{
    public int MaxEntries { get; private set; }

    public SettingsWindow(int currentMaxEntries)
    {
        InitializeComponent();
        MaxEntriesTextBox.Text = currentMaxEntries.ToString();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(MaxEntriesTextBox.Text, out int maxEntries) && maxEntries > 0)
        {
            MaxEntries = maxEntries;
            DialogResult = true;
        }
        else
        {
            MessageBox.Show("Please enter a valid positive number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
