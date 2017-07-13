using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace CYC
{
    public class LinkTextBlock : WrapPanel
    {
        public static DependencyProperty TextProperty;
        public static DependencyProperty WordStyleProperty;
        public static DependencyProperty LinkStyleProperty;
        public static DependencyProperty CheckLinkMethodProperty;

        public delegate void LinkClickDelegate(string url);

        static LinkTextBlock()
        {
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LinkTextBlock), new PropertyMetadata(String.Empty, TextChangedCallback));
            WordStyleProperty = DependencyProperty.Register("WordStyle", typeof(Style), typeof(LinkTextBlock), new PropertyMetadata(null, TextChangedCallback));
            LinkStyleProperty = DependencyProperty.Register("LinkStyle", typeof(Style), typeof(LinkTextBlock), new PropertyMetadata(null, TextChangedCallback));
        }

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LinkTextBlock)d;
            control.BuildControls();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Style WordStyle
        {
            get { return (Style)GetValue(WordStyleProperty); }
            set { SetValue(WordStyleProperty, value); }
        }

        public Style LinkStyle
        {
            get { return (Style)GetValue(LinkStyleProperty); }
            set { SetValue(LinkStyleProperty, value); }
        }

        private void BuildControls()
        {
            Children.Clear();

            foreach (UIElement control in from word in Text.Split(' ')
                                          select BuildWordControl(word))
            {
                Children.Add(control);
            }
        }

        private UIElement BuildWordControl(string text)
        {
            UIElement result;
            string url, displayText;

            if (CheckLink(text, out url, out displayText) == true)
            {
                Uri navigateUri = null;

                try
                {
                    navigateUri = new Uri(url);
                }
                catch (ArgumentException)
                {
                }
                catch (UriFormatException)
                {
                }

                result = new HyperlinkButton() { Content = displayText, Tag = url, NavigateUri = navigateUri, TargetName = "_blank", FontSize = 24, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left, HorizontalAlignment = System.Windows.HorizontalAlignment.Left, Margin = new Thickness(0) };
            }
            else
            {
                result = new TextBlock() { Text = text + " ", FontSize = 24, Margin = new Thickness(0) };

                if (WordStyle != null)
                {
                    result.SetValue(StyleProperty, WordStyle);
                }
            }

            return result;
        }

        private bool CheckLink(string text, out string url, out string displayText)
        {
            url = string.Empty;
            displayText = string.Empty;

            if (text.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)
                || text.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
            {
                url = text.TrimEnd('.');
                displayText = text;
                return true;
            }
            else if (text.StartsWith("@", StringComparison.CurrentCultureIgnoreCase) && text.Length > 1)
            {
                url = "http://twitter.com/" + text.Replace("@", string.Empty);
                displayText = text;
                return true;
            }
            else if (text.StartsWith("#", StringComparison.CurrentCultureIgnoreCase) && text.Length > 1)
            {
                url = "https://twitter.com/search/?src=hash&q=%23" + text.Replace("#", string.Empty); ;
                displayText = text;
                return true;
            }
            return false;
        }
    }
}
