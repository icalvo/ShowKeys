using System.Windows;
using System.Windows.Controls;

namespace KeyboardJedi
{
    public class KeyEntryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? StandardTemplate { get; set; }
        public DataTemplate? SpaceTemplate { get; set; }
        public DataTemplate? ErrorTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is KeyEntryItem keyEntry)
            {
                return keyEntry switch
                {
                    TimePaddingEntry => SpaceTemplate,
                    KeyCombinationEntry => StandardTemplate,
                    ErrorEntry => ErrorTemplate,
                    _ => base.SelectTemplate(item, container)
                };
            }

            return base.SelectTemplate(item, container);
        }
    }
}
