using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCrawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<City> cityList = new List<City>();
        List<Hotel> hotelList = new List<Hotel>();
        private async void button1_Click(object sender, EventArgs e)
        {
            var cityUrl = "http://hotels.ctrip.com/cityList";
            //var cityList = new List<City>();
            var cityCrawler = new SimpleCrawler();
            cityCrawler.OnStart +=cityCrawler_OnStart;
            cityCrawler.OnError += cityCrawler_OnError;
            cityCrawler.OnCompleted += cityCrawler_OnCompleted;
            await cityCrawler.Start(new Uri(cityUrl), null);
            
        }

        void cityCrawler_OnCompleted(object sender, OnCompletedArgs e)
        {
            var matches = Regex.Matches(e.PageSource, @"<a[^>]+href=""*(?<href>/hotel/[^>\s]+)""\s*[^>]*>(?<text>(?!.*img).*?)</a>");
            StringBuilder sb = new StringBuilder();
            foreach (Match m in matches)
            {
                var city = new City{ 
                    CityName=m.Groups["text"].Value,
                    Uri=new Uri("http://hotels.ctrip.com"+m.Groups["href"].Value)
                };
                if (!cityList.Contains(city))
                    cityList.Add(city);
                sb.AppendLine(city.CityName+"|"+city.Uri.ToString());
            }
            sb.AppendLine("==========================================");
            sb.AppendLine("爬虫抓取完成！");
            sb.AppendLine("耗时：" + e.Milliseconds + "毫秒");
            sb.AppendLine("线程:" + e.ThreadId);
            sb.AppendLine("地址:" + e.Uri.ToString());
            textBox1.Invoke(new Action(() =>
            {
                this.textBox1.Text = sb.ToString();
            }));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var cityCrawler = new SimpleCrawler();
            cityCrawler.OnStart += cityCrawler_OnStart;
            cityCrawler.OnError += cityCrawler_OnError;
            cityCrawler.OnCompleted += (s, arg)=>{
                var matches = Regex.Matches(arg.PageSource, "><a[^>]+href=\"*(?<href>/hotel/[^>\\s]+)\"\\s*data-dopost[^>]*><span[^>]+>.*?</span>(?<text>.*?)</a>");
                StringBuilder sb = new StringBuilder();
                foreach (Match m in matches)
                {
                    var hotel = new Hotel
                    {
                        HotelName = m.Groups["text"].Value,
                        Uri = new Uri("http://hotels.ctrip.com" + m.Groups["href"].Value)
                    };
                    if (!hotelList.Contains(hotel))
                        hotelList.Add(hotel);
                    sb.AppendLine(hotel.HotelName + "|" + hotel.Uri.ToString());
                }
                sb.AppendLine("==========================================");
                textBox1.Invoke(new Action(() =>
                {
                    this.textBox2.Text += sb.ToString();
                }));
            };
            var taskList = new List<Task>();
            foreach (var city in cityList)
            {
                var t = cityCrawler.Start(city.Uri, null);
                taskList.Add(t);
            }
            await Task.WhenAll(taskList).ContinueWith(t=> MessageBox.Show("所有任务完成"));
        }

        void cityCrawler_OnError(object sender, Exception e)
        {
            Console.WriteLine("爬虫抓取出现错误：" + e.Message);
        }

        void cityCrawler_OnStart(object sender, OnStartEventArgs e)
        {
            Console.WriteLine("爬虫开始抓取地址:" + e.Uri.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var cityCrawler = new SimpleCrawler();
            cityCrawler.OnStart += cityCrawler_OnStart;
            cityCrawler.OnError += cityCrawler_OnError;
            cityCrawler.OnCompleted += (s, arg) =>
            {
                var matches = Regex.Matches(arg.PageSource, "><a[^>]+href=\"*(?<href>/hotel/[^>\\s]+)\"\\s*data-dopost[^>]*><span[^>]+>.*?</span>(?<text>.*?)</a>");
                StringBuilder sb = new StringBuilder();
                foreach (Match m in matches)
                {
                    var hotel = new Hotel
                    {
                        HotelName = m.Groups["text"].Value,
                        Uri = new Uri("http://hotels.ctrip.com" + m.Groups["href"].Value)
                    };
                    if (!hotelList.Contains(hotel))
                        hotelList.Add(hotel);
                    sb.AppendLine(hotel.HotelName + "|" + hotel.Uri.ToString());
                }
                sb.AppendLine("==========================================");
                textBox1.Invoke(new Action(() =>
                {
                    this.textBox2.Text += sb.ToString();
                }));
            };
            var sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, cityList.Count, async(i) => {
                var city = cityList[i];
                await cityCrawler.Start(city.Uri, null);
            });
        }
    }
}
