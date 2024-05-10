using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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

        private bool _isClosed = false;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _isClosed = true;
        }

        public bool IsClosed
        {
            get { return _isClosed; }
        }

        public RelizeMaker()
        {
            InitializeComponent();
            create_file_without_watermark.IsChecked = true;
            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png")))
            {
                watermark_file_holder.Text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
            }

            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fmpeg", "bin", "ffmpeg.exe")))
            {
                ffmpeg = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fmpeg", "bin", "ffmpeg.exe");
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
                logo_types = config["file_types"]["logo"];
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
            subtitles_file_holder.Text =
            Tools.AskFile(sub_types);
        }

        private void choose_second_subtitles_file_button_Click(object sender, RoutedEventArgs e)
        {
            second_subtitles_file_holder.Text =
            Tools.AskFile(sub_types);
        }

        private void choose_audio_file_button_Click(object sender, RoutedEventArgs e)
        {
            audio_file_holder.Text =
            Tools.AskFile(audio_types);
        }

        private void choose_watermark_file_button_Click(object sender, RoutedEventArgs e)
        {
            watermark_file_holder.Text =
            Tools.AskFile(logo_types);
        }

        private void choose_result_folder_button_Click(object sender, RoutedEventArgs e)
        {
            result_holder.Text =
            Tools.AskDirectory();
        }
        private void open_result_folder_button_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(result_holder.Text))
            {
                Tools.OpenFolder(result_holder.Text);
            }
            else
            {
                Tools.CreateErrorBox("Не знайдено директорії з результатом");
            }
        }

        private void create_button_Click(object sender, RoutedEventArgs e)
        {

            if (File.Exists(video_file_holder.Text))
            {
                if (File.Exists(subtitles_file_holder.Text))
                {
                    if (File.Exists(audio_file_holder.Text))
                    {
                        if (File.Exists(watermark_file_holder.Text))
                        {
                            if (Directory.Exists(result_holder.Text))
                            {
                                if (use_second_subtitles.IsChecked == true && File.Exists(second_subtitles_file_holder.Text) || use_second_subtitles.IsChecked == false)
                                {
                                    if (!is_working)
                                    {
                                        string output_st = "";
                                        is_working = true;


                                        var clearing_process = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = ffmpeg,
                                                Arguments = $"-i \"{video_file_holder.Text}\" -c copy -an -sn clear.mkv",
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            }
                                        };

                                        clearing_process.OutputDataReceived += (sender, e) =>
                                        {
                                            if (!String.IsNullOrEmpty(e.Data))
                                            {
                                                output_st += e.Data + Environment.NewLine;
                                            }
                                        };


                                        clearing_process.Start();
                                        Console.Write(output_st);
                                        clearing_process.WaitForExit();
                                        output.Text += "Відеофайл очищено\n";



                                        File.Copy(subtitles_file_holder.Text, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub.ass"), true);
                                        string subs = "ass=sub.ass";
                                        if (use_second_subtitles.IsChecked == true)
                                        {
                                            File.Copy(second_subtitles_file_holder.Text, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub1.ass"), true);
                                            subs += ",ass=sub1.ass";
                                        }

                                        Console.WriteLine(subs);
                                        output_st += subs;
                                        var sub_burning_process = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = ffmpeg,
                                                Arguments = $"-i \"{video_file_holder.Text}\" -vf \"{subs}:fontsdir=fonts\" sub.mkv",
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            },
                                            EnableRaisingEvents = true
                                        };

                                        sub_burning_process.OutputDataReceived += (sender, e) =>
                                        {
                                            if (!String.IsNullOrEmpty(e.Data))
                                            {
                                                output_st += e.Data + Environment.NewLine;
                                            }
                                        };

                                        sub_burning_process.Start();
                                        Console.Write(output_st);
                                        sub_burning_process.WaitForExit();
                                        output.Text += "Субтитри вшито\n";
                                        File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clear.mkv"));



                                        var audio_changing_process = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = ffmpeg,
                                                Arguments = $"-i \"sub.mkv\" -i \"{audio_file_holder.Text}\" -c:v copy -map 0:v:0 -map 1:a:0 audio.mp4",
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            }
                                        };
                                        audio_changing_process.OutputDataReceived += (sender, e) =>
                                        {
                                            if (!String.IsNullOrEmpty(e.Data))
                                            {
                                                output_st += e.Data + Environment.NewLine;
                                            }
                                        };


                                        audio_changing_process.Start();
                                        Console.Write(output_st);
                                        audio_changing_process.WaitForExit();
                                        output.Text += "Аудіо змінено\n";
                                        File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub.mkv"));



                                        if (create_file_without_watermark.IsChecked == true)
                                        {
                                            /*File.Copy(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio.mp4"),
                                                System.IO.Path.Combine(result_holder.Text, System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text) + "_ww.mp4"));*/
                                            var copy_process = new Process
                                            {
                                                StartInfo = new ProcessStartInfo
                                                {
                                                    FileName = ffmpeg,
                                                    Arguments = $"-i \"audio.mp4\" -i \"{watermark_file_holder.Text}\" -filter_complex \"[1]format=rgba,scale=w=326:h=490:force_original_aspect_ratio=decrease[logo];[0][logo]overlay=(W-w)/2:10:format=auto,format=yuv420p\" -c:a copy \"{System.IO.Path.Combine(result_holder.Text, System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text) + ".mp4")}\"",
                                                    RedirectStandardOutput = true,
                                                    StandardOutputEncoding = Encoding.UTF8,
                                                    UseShellExecute = false,
                                                    CreateNoWindow = false,
                                                }
                                            };
                                            copy_process.OutputDataReceived += (sender, e) =>
                                            {
                                                if (!String.IsNullOrEmpty(e.Data))
                                                {
                                                    output_st += e.Data + Environment.NewLine;
                                                }
                                            };

                                            copy_process.Start();
                                            copy_process.WaitForExit();
                                            output.Text += "Копійовано\n";
                                        }




                                        var logo_burning_process = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = ffmpeg,
                                                Arguments = $"-i \"audio.mp4\" -i \"{watermark_file_holder.Text}\" -filter_complex \"[1]format=rgba,scale=w=326:h=490:force_original_aspect_ratio=decrease[logo];[0][logo]overlay=W-w-10:10:format=auto,format=yuv420p\" -c:a copy \"{System.IO.Path.Combine(result_holder.Text, System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text) + ".mp4")}\"",
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            }

                                        };


                                        logo_burning_process.OutputDataReceived += (sender, e) =>
                                        {
                                            if (!String.IsNullOrEmpty(e.Data))
                                            {
                                                output_st += e.Data + Environment.NewLine;
                                            }
                                        };

                                        logo_burning_process.Start();
                                        Console.Write(output_st);
                                        logo_burning_process.WaitForExit();
                                        output.Text += "Лого вшито!\n";
                                        File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio.mp4"));
                                        File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub.ass"));
                                        if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub1.ass")))
                                        { File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sub1.ass")); }



                                        Tools.CreateInfoBox("Готово");
                                        var writeLogTask = Task.Run(() => WriteToLogAsync($"log {DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.txt", output_st));
                                        writeLogTask.Wait();

                                        is_working = false;
                                    }
                                    else
                                    {
                                        Tools.CreateErrorBox("Програма вже працює");
                                    }
                                }
                                else
                                {
                                    Tools.CreateErrorBox("Другі субтитри не знайдено!");
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

        async Task WriteToLogAsync(string logFileName, string logContent)
        {
            await Tools.WriteLOG(logFileName, logContent);
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
