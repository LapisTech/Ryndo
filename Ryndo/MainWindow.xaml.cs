using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ryndo
{
    public partial class MainWindow : Window
    {
        private Data data;

        public MainWindow()
        {
            InitializeComponent();

            // Init tab data.
            this.data = new Data(this.Tab);

            // Set tab data.
            this.Tab.ItemsSource = data.List;

            // Resize event.
            SizeChanged += (s, e) => { this.data.Resize(this.Tab.SelectedIndex); };

            // Add tab event.
            this.data.AddTab();
        }

        public void OnAddTab(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("AddTabEvent:" + sender.ToString());
            this.data.AddTab("C:");
        }
    }

    class Data
    {
        private TabControl tab;
        private ObservableCollection<ExplorerTabData> source;

        public Data(TabControl tab)
        {
            this.tab = tab;
            this.source = new ObservableCollection<ExplorerTabData>();

            string basedir = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ryndo");
            this.CreateSaveDir(basedir);
        }

        private DirectoryInfo CreateSaveDir(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }
            return Directory.CreateDirectory(path);
        }

        public ObservableCollection<ExplorerTabData> List
        {
            get { return this.source; }
        }

        public void Resize(int index)
        {
            Explorer explorer = this.source[index].Explorer;
            if (explorer == null) { return; }
            explorer.OnResize();
        }

        public void AddTab()
        {
            ExplorerTabData tab = new ExplorerTabData();
            source.Add(tab);
        }

        public int AddTab(string path)
        {
            Explorer explorer = new Explorer();

            ExplorerTabData tab = new ExplorerTabData(explorer, this);

            explorer.OpenDir(path);

            int select = this.source.Count - 1;
            source.Insert(select, tab);
            
            return this.ResetIndex(select);
        }

        public int RemoveTab(ExplorerTabData data)
        {
            int index = this.source.IndexOf(data);
            this.source.Remove(data);
            return this.ResetIndex(index);
        }

        public int ResetIndex(int index)
        {
            if (this.source.Count <= 1)
            {
                // Tab empty.
                index = 0;
            } else if (this.source.Count - 1 <= index)
            {
                // Tab selected "New button".
                // So, selected prev New button.
                index = this.source.Count - 2;
            }
            this.tab.SelectedIndex = index;

            return index;
        }
    }

    class ExplorerTabData
    {
        private Explorer explorer;

        public ExplorerTabData()
        {
        }

        public ExplorerTabData(Explorer explorer, Data data)
        {
            this.explorer = explorer;
            this.Content = new WindowsFormsHost();
            this.Content.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.Content.VerticalAlignment = VerticalAlignment.Stretch;
            this.Content.Loaded += (object sender, RoutedEventArgs e) =>
            {
                if (explorer == null) { return; }
                explorer.Maximize();
            };
            this.Content.ClipToBounds = true;
            //this.Content.Margin = new Thickness(0, -10, 0, 0);

            this.Close = new Button();
            this.Close.Content = "x";
            this.Close.Click += (object sender, RoutedEventArgs e) =>
            {
                data.RemoveTab(this);
            };

            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            panel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);

            explorer.ChangeParent(panel);

            this.Content.Child = panel;
        }

        public string Dir
        {
            get { return this.explorer.GetDirName(); }
        }

        public WindowsFormsHost Content { get; set; }

        public Button Close { get; set; }

        public Explorer Explorer
        {
            get { return this.explorer; }
        }
    }

    public class TabStyleSelector : System.Windows.Controls.StyleSelector
    {
        public Style TabStyle { get; set; }
        public Style NewStyle { get; set; }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            return ((ExplorerTabData)item).Explorer == null ? NewStyle : TabStyle;
        }
    }
}
