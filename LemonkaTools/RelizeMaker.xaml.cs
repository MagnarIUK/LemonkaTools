using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace LemonkaTools
{
    public partial class RelizeMaker : Window
    {
        private string movie_types { get; set; }
        private string sub_types { get; set; }
        private string audio_types { get; set; }
        private string logo_types { get; set; }
        private bool is_working { get; set; } = false;
        string ffmpeg;

        public RelizeMaker()
        {
            InitializeComponent();
            create_file_without_watermark.IsChecked = true;
            if(File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png")))
            {
                watermark_file_holder.Text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
            }

            if(File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fmpeg/bin/ffmpeg.exe")))
            {
                ffmpeg = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fmpeg/bin/ffmpeg.exe");
            }
            else
            {
                Tools.CreateErrorBox("Виконуваний файл ffmpeg не знайдено!");
                this.Close();
            }

            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")))
            {
                var config = Tools.Read<Dictionary<string, Dictionary<string, string>>>("config.json");
                movie_types = config["file_types"]["movie"];
                sub_types = config["file_types"]["sub"];
                audio_types = config["file_types"]["audio"];
                logo_types = config["file_types"]["audio"];
            }
            else
            {
                Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>
                {
                    ["file_types"] = new Dictionary<string, string>
                    {
                        ["movie"] = "\u0412\u0456\u0434\u0435\u043e\u0444\u0430\u0439\u043b\u0438 (*.mkv)|*.mkv",
                        ["sub"] = "\u0424\u0430\u0439\u043b\u0438 \u0434\u0443\u043f\u0446\u0456 (*.ass)|*.ass",
                        ["audio"] = "\u0424\u0430\u0439\u043b wav (*.wav)|*.wav",
                        ["logo"] = "\u0424\u0430\u0439\u043b \u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u043d\u044f (*.png)|*.png"
                    }
                };

                Task task = Task.Run(async () =>
                {
                    await Tools.Write<Dictionary<string, Dictionary<string, string>>>("config.json", data);
                });
                task.Wait();
                movie_types = "\u0412\u0456\u0434\u0435\u043e\u0444\u0430\u0439\u043b\u0438 (*.mkv)|*.mkv";
                sub_types = "\u0424\u0430\u0439\u043b\u0438 \u0434\u0443\u043f\u0446\u0456 (*.ass)|*.ass";
                audio_types = "\u0424\u0430\u0439\u043b wav (*.wav)|*.wav";
                logo_types = "\u0424\u0430\u0439\u043b \u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u043d\u044f (*.png)|*.png";

            }



        }

        private void choose_videofile_button_Click(object sender, RoutedEventArgs e)
        {
            video_file_holder.Text = Tools.AskFile(movie_types);
        }

        private void choose_subtitles_file_button_Click(object sender, RoutedEventArgs e)
        {
            subtitles_file_holder.Text=
            Tools.AskFile(sub_types);
        }

        private void choose_second_subtitles_file_button_Click(object sender, RoutedEventArgs e)
        {
            second_subtitles_file_holder.Text=
            Tools.AskFile(sub_types);
        }

        private void choose_audio_file_button_Click(object sender, RoutedEventArgs e)
        {
            audio_file_holder.Text=
            Tools.AskFile(audio_types);
        }

        private void choose_watermark_file_button_Click(object sender, RoutedEventArgs e)
        {
            watermark_file_holder.Text=
            Tools.AskFile(logo_types);
        }

        private void choose_result_folder_button_Click(object sender, RoutedEventArgs e)
        {
            result_holder.Text=
            Tools.AskDirectory();
        }


        private void create_button_Click(object sender, RoutedEventArgs e)
        {
            
            if(File.Exists(video_file_holder.Text))
            {
                if (File.Exists(subtitles_file_holder.Text))
                {
                    if (File.Exists(audio_file_holder.Text))
                    {
                        if (File.Exists(watermark_file_holder.Text))
                        {
                            if (Directory.Exists(result_holder.Text))
                            {
                                if (!is_working)
                                {
                                    string output_st = "";
                                    is_working = true;

                                    File.Copy(subtitles_file_holder.Text, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub.ass"));

                                    File.Copy(second_subtitles_file_holder.Text, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub1.ass"));

                                    var clearing_process = new Process
                                    {
                                        StartInfo = new ProcessStartInfo
                                        {
                                            FileName = ffmpeg,
                                            Arguments = $"-i \"{video_file_holder.Text}\" -c copy -an -sn clear.mkv",
                                            RedirectStandardOutput = true,
                                            StandardOutputEncoding = System.Text.Encoding.UTF8,
                                            UseShellExecute = true,
                                            CreateNoWindow = false,
                                        },
                                        EnableRaisingEvents = true
                                    };
                                    clearing_process.OutputDataReceived += (sender, e) =>
                                    {
                                        if (!string.IsNullOrEmpty(e.Data))
                                        {
                                            output_st += e.Data;
                                        }
                                    };


                                    clearing_process.Start();
                                    clearing_process.WaitForExit();
                                    output.Text += "Відеофайл очищено";

                                    while (true)
                                    {
                                        if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clear.mkv")))
                                        {
                                            break;
                                        }
                                    }

                                    var sub_burning_process = new Process
                                    {
                                        StartInfo = new ProcessStartInfo
                                        {
                                            FileName = ffmpeg,
                                            Arguments = $"-i \"clear.mkv\" -vf, f\"ass=\"",
                                            RedirectStandardOutput = true,
                                            StandardOutputEncoding = System.Text.Encoding.UTF8,
                                            UseShellExecute = true,
                                            CreateNoWindow = false,
                                        },
                                        EnableRaisingEvents = true
                                    };
                                    clearing_process.OutputDataReceived += (sender, e) =>
                                    {
                                        if (!string.IsNullOrEmpty(e.Data))
                                        {
                                            output_st += e.Data;
                                        }
                                    };




                                    //Інші етапи








                                    Task t = Tools.WriteTXT($"log {DateTime.Today.ToString()}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.txt", output_st);
                                }
                                else
                                {
                                    Tools.CreateErrorBox("Програма вже працює");
                                }

                            }
                            else
                            {
                                Tools.CreateErrorBox("Не знайдено заданої директорії для результатів");
                            }
                        }
                        else
                        {
                            Tools.CreateErrorBox("Не знайдено заданого файлу водяного знаку");
                        }
                    }
                    else
                    {
                        Tools.CreateErrorBox("Не знайдено заданого аудіофайлу");
                    }
                }
                else
                {
                    Tools.CreateErrorBox("Не знайдено заданого файлу субтитрів (1)");
                }
            }
            else
            {
                Tools.CreateErrorBox("Не знайдено заданого відеофайлу");
            }


        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string sanitizedData = Regex.Replace(e.Data, @"[^\u0020-\u007E]", " ");

                Dispatcher.Invoke(() =>
                {
                    output.Text += sanitizedData + Environment.NewLine;

                    if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                    {
                        scrollViewer.ScrollToEnd();
                    }
                });
            }
        }
    }
}
