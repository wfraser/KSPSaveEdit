using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KSPSaveEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SFSNode RootNode = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Data.DataContext == null)
            {
                RootNode = ReadSFS(Filename.Text);
                PopulateGrid();
                Filename.IsEnabled = false;
                BrowseButton.IsEnabled = false;
                DeleteButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                ((Button)sender).Content = "Unload";
            }
            else
            {
                Data.DataContext = null;
                RootNode = null;
                Filename.IsEnabled = true;
                BrowseButton.IsEnabled = true;
                DeleteButton.IsEnabled = false;
                SaveButton.IsEnabled = false;
                ((Button)sender).Content = "Load";
            }
        }

        private void PopulateGrid()
        {
            Data.DataContext = RootNode.Children.Where(node => node.NodeName == "FLIGHTSTATE").Single().Children.Where(node => node.NodeName == "VESSEL").ToList();

            if (Data.Columns.Count != 0)
            {
                return;
            }

            foreach (var pair in ((IList<SFSNode>)Data.DataContext)[0].Attributes)
            {
                Data.Columns.Add(new DataGridTextColumn()
                {
                    Header = pair.Key,
                    Binding = new Binding(string.Format("Attributes[{0}]", pair.Key))
                });
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "persistent.sfs|persistent.sfs|All files|*";
            bool? result = dlg.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            Filename.Text = dlg.FileName;
        }

        private SFSNode ReadSFS(string inputSfs)
        {
            string inStr = string.Empty;
            using (FileStream input = new FileStream(inputSfs, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                System.Diagnostics.Debug.Assert(input.Length <= (long)int.MaxValue);

                byte[] bytes = new byte[input.Length];
                input.Read(bytes, 0, (int)input.Length);
                inStr = Encoding.UTF8.GetString(bytes).Replace("\r", string.Empty);
            }

            SFSNode root = null;
            Stack<SFSNode> stack = new Stack<SFSNode>();
            string[] lines = inStr.Split('\n');

            string nodeName = null;
            foreach (string line in lines)
            {
                if (line.EndsWith("{"))
                {
                    SFSNode newNode = new SFSNode(nodeName);
                    if (root == null)
                    {
                        root = newNode;
                    }
                    else
                    {
                        stack.Peek().Children.Add(newNode);
                    }

                    stack.Push(newNode);
                }
                else if (line.EndsWith("}"))
                {
                    stack.Pop();
                }
                else if (line.Contains(" = "))
                {
                    string[] parts = line.Split(new string[] { " = " }, StringSplitOptions.None);
                    stack.Peek().Attributes[parts[0].Trim()] =  parts[1];
                }
                else
                {
                    nodeName = line.Trim();
                }
            }

            return root;
        }

#if false
        //
        // This method converts a .sfs file into XML format. Might be useful later maybe?
        //
        private string Transform(string inputSfs, string outputXml = null)
        {
            string inStr = string.Empty;
            using (FileStream input = new FileStream(inputSfs, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                System.Diagnostics.Debug.Assert(input.Length <= (long)int.MaxValue);

                byte[] bytes = new byte[input.Length];
                input.Read(bytes, 0, (int)input.Length);
                inStr = Encoding.UTF8.GetString(bytes).Replace("\r", string.Empty);
            }

            StringBuilder result = new StringBuilder();
            Stack<string> tagStack = new Stack<string>();
            string[] lines = inStr.Split('\n');

            string tagName = null;
            bool open = false;
            foreach (string line in lines)
            {
                if (line.EndsWith("{"))
                {
                    result.Append(line.Replace("{", string.Format("<{0} ", tagName)));
                    tagStack.Push(tagName);
                    open = true;
                }
                else if (line.EndsWith("}"))
                {
                    if (tagStack.Peek() == tagName)
                    {
                        result.AppendLine("/>");
                        tagStack.Pop();
                        open = false;
                    }
                    else
                    {
                        result.Append(line.Replace("}", string.Format("</{0}>", tagStack.Pop())));
                        result.AppendLine();
                        open = false;
                    }
                }
                else if (line.Contains(" = "))
                {
                    string[] parts = line.Split(new string[] { " = " }, StringSplitOptions.None);
                    result.AppendFormat("{0}=\"{1}\" ", parts[0].Trim(), parts[1]);
                }
                else
                {
                    if (open)
                    {
                        result.AppendLine(">");
                        open = false;
                    }

                    tagName = line.Trim();
                }
            }

            string resultStr = result.ToString();

            if (outputXml != null)
            {
                using (FileStream output = new FileStream(outputXml, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    output.Write(resultStr);
                }
            }

            return resultStr;
        }
#endif

        private void Filename_TextChanged(object sender, TextChangedEventArgs e)
        {
            OpenButton.IsEnabled = File.Exists(((TextBox)sender).Text);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            SFSNode flightState = RootNode.Children.Where(node => node.NodeName == "FLIGHTSTATE").Single();

            foreach (object item in Data.SelectedItems)
            {
                flightState.Children.Remove((SFSNode)item);
            }

            PopulateGrid();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(Filename.Text, Regex.Replace(Filename.Text, @"\.sfs$", "_backup.sfs"), true);

            using (FileStream output = new FileStream(Filename.Text, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                WriteSFS(output, RootNode, 0);
            }
        }

        private void WriteSFS(Stream output, SFSNode current, int indent)
        {
            Indent(output, indent);
            output.Write(current.NodeName);
            output.Write("\r\n");
            Indent(output, indent);
            output.Write("{\r\n");

            foreach (var pair in current.Attributes)
            {
                Indent(output, indent + 1);
                output.Write(pair.Key);
                output.Write(" = ");
                output.Write(pair.Value);
                output.Write("\r\n");
            }

            foreach (SFSNode child in current.Children)
            {
                WriteSFS(output, child, indent + 1);
            }

            Indent(output, indent);
            output.Write("}\r\n");
        }

        private void Indent(Stream output, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                output.Write((byte)'\t');
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }
    }
}
