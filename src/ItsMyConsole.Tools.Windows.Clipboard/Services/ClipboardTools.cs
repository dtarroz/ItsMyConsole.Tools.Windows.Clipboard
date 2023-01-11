﻿using System.Threading;
using System.Windows.Forms;

namespace ItsMyConsole.Tools.Windows.Clipboard
{
    /// <summary>
    /// Outils d'accès au presse papier Windows
    /// </summary>
    public class ClipboardTools
    {
        internal ClipboardTools() { }

        /// <summary>
        /// Récupérer le texte du presse papier
        /// </summary>
        public string GetText()
        {
            string text = null;
            RunThread(() =>
            {
                text = System.Windows.Forms.Clipboard.GetText();
            });
            return text;
        }

        /// <summary>
        /// Récupérer le texte du presse papier en tant que HTML
        /// </summary>
        public string GetTextAsHtml()
        {
            string text = null;
            RunThread(() =>
            {
                text = System.Windows.Forms.Clipboard.GetText(TextDataFormat.Html);
            });
            return text;
        }

        /// <summary>
        /// Mettre un texte dans le presse papier
        /// </summary>
        /// <param name="text">Le texte à mettre dans le presse papier</param>
        public void SetText(string text)
        {
            RunThread(() =>
            {
                System.Windows.Forms.Clipboard.SetText(text);
            });
        }

        /// <summary>
        /// Mettre un texte dans le presse papier en tant que HTML
        /// </summary>
        /// <param name="htmlText">Le texte à mettre dans le presse papier en tant que HTML</param>
        /// <param name="plainText">Le texte "plat" à mettre dans le presse papier pour les logiciels qui ne gére pas le texte en tant que HTML</param>
        public void SetTextAsHtml(string htmlText, string plainText)
        {
            string htmlClipboard = ConvertToHtmlClipboard(htmlText);
            RunThread(() =>
            {
                DataObject dataObject = new DataObject();
                dataObject.SetData(DataFormats.Html, htmlClipboard);
                dataObject.SetData(DataFormats.Text, plainText);
                System.Windows.Forms.Clipboard.SetDataObject(dataObject, true);
            });
        }

        private void RunThread(ThreadStart start)
        {
            Thread thread = new Thread(start);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private string ConvertToHtmlClipboard(string text)
        {
            string header = "Version:0.9\r\nStartHTML:0000000000\r\nEndHTML:0000000000\r\nStartFragment:0000000000\r\nEndFragment:0000000000\r\n";
            string htmlStart = "<html>\r\n<body>\r\n<!--StartFragment-->";
            string htmlEnd = "<!--EndFragment-->\r\n</body>\r\n</html>";
            string html = header.Replace("StartHTML:0000000000", $"StartHTML:{header.Length:0000000000}") + htmlStart;
            html = html.Replace("StartFragment:0000000000", $"StartFragment:{html.Length:0000000000}") + text;
            html = html.Replace("EndFragment:0000000000", $"EndFragment:{html.Length:0000000000}") + htmlEnd;
            html = html.Replace("EndHTML:0000000000", $"EndHTML:{html.Length:0000000000}");
            return html;
        }
    }
}
