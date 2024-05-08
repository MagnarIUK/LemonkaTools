using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LemonkaTools
{
    internal class Tools
    {
        public static async Task Write<T>(string file_name, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file_name), false))
            {
                await writer.WriteAsync(json);
            }
        }
        public static async Task WriteTXT(string file_name, string data)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file_name), false))
            {
                await writer.WriteAsync(data);
            }
        }
        public static void CreateErrorBox(string Message)
        {
            MessageBox.Show(Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void CreateInfoBox(string Message)
        {
            MessageBox.Show(Message, "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void  OpenFolder(string Path)
        {
            if (Directory.Exists(Path))
            {
                Process.Start("explorer.exe", Path);
            }
            else
            {
                MessageBox.Show("Директорія не існує!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public static void OpenLink(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }

        public static List<string> GetFilesInFolder(string folderPath)
        {
            List<string> files = new List<string>();

            try
            {
                string[] filePaths = Directory.GetFiles(folderPath);
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    files.Add(fileName);
                }
            }
            catch (DirectoryNotFoundException)
            {
                files.Add("Файлів нема");
            }

            return files;
        }

        public static void ClearFile(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    stream.SetLength(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        public static T Read<T>(string fileName)
        {
            try
            {
                var extension = Path.GetExtension(fileName);

                if (extension == ".json")
                {
                    var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else if (extension == ".txt")
                {
                    var text = File.ReadAllText(fileName);
                    return (T)(object)text;
                }
                else
                {
                    throw new ArgumentException("Непідтримуваний формат файлу");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виникла помилка: {ex.Message}");
                return default;
            }
        }


        public static string AskDirectory()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return null;

        }

        public static string AskFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                return selectedFileName;
            }
            return null;

        }

    }
}
