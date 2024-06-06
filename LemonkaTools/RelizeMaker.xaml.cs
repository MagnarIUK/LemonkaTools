using System;
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
        string ffmpeg, mkvmerge;

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

            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mkvtoolnix", "mkvmerge.exe")))
            {
                mkvmerge = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mkvtoolnix", "mkvmerge.exe");
            }
            else
            {
                Tools.CreateErrorBox("Виконуваний файл mkvmerge не знайдено!");
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
                                       
                                        

                                        string mkv_args = "";
                                        if(use_second_subtitles.IsChecked == true)
                                        {
                                            mkv_args = $"-o out.mkv -A -S --no-global-tags \"{video_file_holder.Text}\" -D --language 0:ukr \"{audio_file_holder.Text}\" --language 0:ukr \"{subtitles_file_holder.Text}\" --language 0:ukr \"{second_subtitles_file_holder.Text}\" ";
                                        }
                                        else
                                        {
                                            mkv_args = $"-o out.mkv -A -S --no-global-tags \"{video_file_holder.Text}\" -D --language 0:ukr \"{audio_file_holder.Text}\" --language 0:ukr \"{subtitles_file_holder.Text}\"";
                                        }
                                        var mkvmaster = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = mkvmerge,
                                                Arguments = mkv_args,
//                                                Arguments = $"-o {System.IO.Path.Combine(result_holder.Text, System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text))+ ".mkv"} {video_file_holder.Text} --language 0:ukr {audio_file_holder.Text} --language 0:ukr {subs_name} --attachments {video_file_holder.Text}",
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            }
                                        };

                                        mkvmaster.OutputDataReceived += (sender, e) =>
                                        {
                                            if (!String.IsNullOrEmpty(e.Data))
                                            {
                                                output.Text += e.Data + Environment.NewLine;
                                            }
                                        };


                                        mkvmaster.Start();
                                        mkvmaster.WaitForExit();
                                        output.Text += "MKV готовий\n";



                                        var compiler = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = ffmpeg,
                                                Arguments = $"-i out.mkv -vf subtitles=out.mkv \"{System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shorts", "in",  System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text) + "(ww).mp4")}\"",
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            }
                                        };


                                        //string logo_args = $"-i \"audio.mp4\" -i \"{watermark_file_holder.Text}\" -filter_complex \"[1]format=rgba,scale=w=326:h=490:force_original_aspect_ratio=decrease[logo];[0][logo]overlay=W-w-10:10:format=auto,format=yuv420p\" -c:a copy \"{System.IO.Path.Combine(result_holder.Text, System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text) + ".mp4")}\"";
                                        string logo_args = $"-i out.mkv -i \"{watermark_file_holder.Text}\" -filter_complex \"[1]format=rgba,scale=w=326:h=490:force_original_aspect_ratio=decrease[logo];[0][logo]overlay=W-w-10:10:format=auto,format=yuv420p\" -vf subtitles=out.mkv \"{System.IO.Path.Combine(result_holder.Text, System.IO.Path.GetFileNameWithoutExtension(video_file_holder.Text) + ".mp4")}\"";
                                        var logo_burn = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = ffmpeg,
                                                Arguments = logo_args,
                                                RedirectStandardOutput = true,
                                                StandardOutputEncoding = Encoding.UTF8,
                                                UseShellExecute = false,
                                                CreateNoWindow = false,
                                            }
                                        };
                                        if(create_file_without_watermark.IsChecked == true)
                                        {
                                            logo_burn.Start();
                                            compiler.Start();
                                        }
                                        else
                                        {
                                            logo_burn.Start();
                                        }

                                       
                                        compiler.WaitForExit();
                                        logo_burn.WaitForExit();
                                        output.Text += "MP4 готовий\n";


                                        File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out.mkv"));
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
