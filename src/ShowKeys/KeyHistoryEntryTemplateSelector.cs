using System.Windows;
using System.Windows.Controls;
using ShowKeys.History;

namespace ShowKeys;

public class KeyHistoryEntryTemplateSelector : DataTemplateSelector
{
    public DataTemplate? KeyComboTemplate { get; set; }
    public DataTemplate? TimePaddingTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is KeyHistoryEntry keyEntry)
        {
            return keyEntry switch
            {
                TimePaddingEntry => TimePaddingTemplate,
                KeyComboEntry => KeyComboTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }

        return base.SelectTemplate(item, container);
    }
}