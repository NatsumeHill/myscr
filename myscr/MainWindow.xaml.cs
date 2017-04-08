using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transitionals;
using Transitionals.Controls;

namespace myscr
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Type> transitionTypes = new ObservableCollection<Type>();
        //图片帧
        private ArrayList frames;
        //文件夹信息
        private FileInfo[] fileInfos;
        //随机数产生
        private Random rand;

        //计时器
        private System.Windows.Threading.DispatcherTimer dtimer;
        private System.Windows.Threading.DispatcherTimer clocktimer;

        //鼠标位置纪录
        private Point point;

        //图片路径
        private String path;

        public MainWindow()
        {
            InitializeComponent();

            FileStream fileStream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "myscr.ini", FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fileStream);
            path = streamReader.ReadLine();
            if (path == null)
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            streamReader.Close();
            fileStream.Close();

            fileInfos = SetResourcePath(path);
            rand = new Random();
            Transitionals.Transitions.FadeAndGrowTransition t = new Transitionals.Transitions.FadeAndGrowTransition();

            frames = GetChildren(myGrid, "TransitionElement");
            foreach(TransitionElement frame in frames)
            {
                frame.Transition = t;
                DoTrans(frame);
            }

            //创建计时器
            if (dtimer == null)
            {
                dtimer = new System.Windows.Threading.DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                dtimer.Tick += Dtimer_Tick;
            }
            dtimer.Start();
            //用于显示时间
            clocktimer = new System.Windows.Threading.DispatcherTimer();
            clocktimer.Tick += new EventHandler(ShowCurrentTime);
            clocktimer.Interval = new TimeSpan(0, 0, 0, 1);
            clocktimer.Start();

            point = new Point(0, 0);
        }

        /// <summary>
        /// 计时器处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Dtimer_Tick(object sender, EventArgs e)
        {
            ChangeFrame();
        }

        public void ShowCurrentTime(object sender, EventArgs e)
        {
            Time.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 切换一个帧的图片
        /// </summary>
        public void ChangeFrame()
        {
            int nmbers = rand.Next(3);
            for(int k = 0; k < nmbers+1; k++)
            {
                int i = rand.Next(20);
                TransitionElement frame = (TransitionElement)frames[i];
                DoTrans(frame);
            }
        }

        
        /// <summary>
        /// 设置图片资源路径
        /// </summary>
        /// <returns></returns>
        public FileInfo[] SetResourcePath(String folder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories);
            return fileInfos;
        }
        /// <summary>
        /// 获取所有子元素
        /// </summary>
        /// <param name="root"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public ArrayList GetChildren(DependencyObject root, string childName)
        {
            ArrayList ret = new ArrayList();
            IEnumerable children = LogicalTreeHelper.GetChildren(root);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    if (((DependencyObject)child).DependencyObjectType.Name == childName)
                        ret.Add(child);
                    ret.AddRange(GetChildren((DependencyObject)child, childName));
                }
            }
            return ret;
        }

        /// <summary>
        /// 更改图像资源
        /// </summary>
        /// <param name="image"></param>
        /// <param name="path"></param>
        public void SetUri(Image image, String path)
        {
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(path);
            myBitmapImage.DecodePixelWidth = 500;
            myBitmapImage.EndInit();
            image.Source = myBitmapImage;
        }

        /// <summary>
        /// 随机获取图片路径
        /// </summary>
        /// <returns></returns>
        public String GetRandomFile(FileInfo[] fileInfos, Random rand )
        {
            int i = rand.Next(fileInfos.Length);
            return fileInfos[i].FullName;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
                default:
                    clocktimer.Start();
                    dtimer.Stop();
                    Application.Current.Shutdown();
                    break;
            }
        }

        /// <summary>
        /// 切换某帧的图片
        /// </summary>
        /// <param name="frame"></param>
        public void DoTrans(TransitionElement frame)
        {
            Image image = new Image()
            {
                Width = frame.Width,
                Height = frame.Height
            };
            image.Stretch = Stretch.UniformToFill;
            SetUri(image, GetRandomFile(fileInfos ,rand));
            frame.Content = image;
        }

        /// <summary>
        /// 外部依赖加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTransitions(Assembly.GetAssembly(typeof(Transition)));
        }

        private void LoadTransitions(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (transitionTypes.Contains(type)) { continue; }

                // Must not be abstract.
                if ((typeof(Transition).IsAssignableFrom(type)) && (!type.IsAbstract))
                {
                    transitionTypes.Add(type);
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //Point newpoint = e.GetPosition(myGrid);
            //if (point.X == 0 && point.Y == 0)
            //{
            //    Mouse.OverrideCursor = Cursors.None;
            //    point.X = newpoint.X;
            //    point.Y = newpoint.Y;
            //}
            //else if(Math.Abs(point.X-newpoint.X)>400 || Math.Abs( point.X - newpoint.Y)>400)
            //{
            //    Mouse.OverrideCursor = null;
            //    clocktimer.Start();
            //    dtimer.Stop();
            //    Application.Current.Shutdown();
            //}
        }
    }
}
