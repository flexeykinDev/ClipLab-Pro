    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NReco.VideoConverter;
using ClipLab.Core;
using ClipLab.Notifications;


namespace ClipLab.Forms
{
    public partial class FormEdit : Form
    {

        private string? inputFile1;
        private string? inputFile2;
        private string? outputFile;
        private string? inputFileTab3;
        private string? outputFileTab3;

        void AlertBox(Color backColor, Color color, string title, string text, Image icon)
        {
            AlertBox aRt = new AlertBox();
            aRt.BackColor = backColor;
            aRt.ColorAlertBox = color;
            aRt.TitleAlertBox = title;
            aRt.TextAlertBox = text;
            aRt.IconeAlertBox = icon;
            aRt.ShowDialog();
            aRt.TopMost = false;


        }
        string? videoName, videoPath, musicPath, musicName;

        public FormEdit()
        {
            InitializeComponent();
            LoadTheme();

        }

        private void FormEdit_Load(object sender, EventArgs e)
        {
            LoadTheme();
        }
        private void LoadTheme()
        {
            foreach (Control btns in this.tabPage1.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }
            foreach (Control btns in this.tabPage2.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }
            foreach (Control btns in this.tabPage3.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }
            

            lbl1.ForeColor = ThemeColor.SecondaryColor;
            
            lbl2.ForeColor = ThemeColor.SecondaryColor;
            
            lbl3.ForeColor = ThemeColor.SecondaryColor;

            lbl1tab2.ForeColor = ThemeColor.SecondaryColor;

            lbl2tab2.ForeColor = ThemeColor.SecondaryColor;

            lblTab3.ForeColor = ThemeColor.SecondaryColor;
            lblTab3Save.ForeColor = ThemeColor.SecondaryColor;
            lblTab3Time.ForeColor = ThemeColor.SecondaryColor;
            lblTire.ForeColor = ThemeColor.SecondaryColor;
        }
        /// tab1
        private void btnBrowseInput1_Click(object sender, EventArgs e)
        {
            // Відкрити діалог вибору файлу для першого відеофайлу
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*.mp4;*.avi)|*.mp4;*.avi|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputFile1 = openFileDialog.FileName;
                txtBoxInput1.Text = inputFile1;
            }
        }

        private void btnBrowseInput2_Click(object sender, EventArgs e)
        {
            // Відкрити діалог вибору файлу для другого відеофайлу
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*.mp4;*.avi)|*.mp4;*.avi|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputFile2 = openFileDialog.FileName;
                txtBoxInput2.Text = inputFile2;
            }
        }

        

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            // Відкрити діалог вибору файлу для результуючого відеофайлу
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFile = Path.Combine(folderBrowserDialog.SelectedPath, "output.mp4");
                    txtBoxOutput.Text = outputFile;
                }
            }
        }


        private void btnConcatenate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputFile1) || string.IsNullOrWhiteSpace(inputFile2) || string.IsNullOrWhiteSpace(outputFile))
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Заповніть усі поля!", Properties.Resources.Error_ICO30);
                }
                else if (!File.Exists(inputFile1) || !File.Exists(inputFile2))
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Виберіть існуючі відеофайли!", Properties.Resources.Error_ICO30);
                }
                else
                {
                    // Шлях до FFmpeg
                    string ffmpegPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");


                    // Шлях до першого відеофайлу
                    string input1 = @$"{inputFile1}";

                    // Шлях до другого відеофайлу
                    string input2 = @$"{inputFile2}";


                    // Шлях до файлу списку вхідних файлів
                    // Створюємо тимчасовий файл списку вхідних файлів
                    string listFile = Path.GetTempFileName();
                    string listContent = $"file '{input1}'\r\nfile '{input2}'";
                    File.WriteAllText(listFile, listContent);

                    // Шлях до вихідного файлу
                    string output = @$"{outputFile}";

                    // Створюємо процес для виконання команди FFmpeg
                    var processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = ffmpegPath;
                    processStartInfo.Arguments = $"-safe 0 -f concat -i \"{listFile}\" -c copy \"{output}\"";
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.RedirectStandardOutput = true;
                    processStartInfo.CreateNoWindow = true;
                    if (File.Exists(output))
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                        AlertBox(Color.LightGoldenrodYellow, Color.Gold, "Попередження :O", "Файл вже існує.", Properties.Resources.Warning_ICO30);
                    }
                    else if (!File.Exists(output))
                    {
                        // Запускаємо процес
                        Process process = new Process();
                        process.StartInfo = processStartInfo;
                        process.Start();

                        // Очікуємо завершення процесу й отримуємо висновок команди FFmpeg
                        string outputMessage = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        // Видаляємо тимчасовий файл списку вхідних файлів
                        File.Delete(listFile);

                        System.Media.SystemSounds.Asterisk.Play();
                        if (process.ExitCode == 0)
                        {
                            AlertBox(Color.LightGray, Color.SeaGreen, "Успіх :)", "Операція виконана Успішно", Properties.Resources.Success_ICO30);
                        }
                        else
                        {
                            AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", $"FFmpeg завершився з помилкою (код {process.ExitCode}).", Properties.Resources.Error_ICO30);
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", $"Сталася помилка під час злиття відео: {ex.Message}", Properties.Resources.Error_ICO30);
            }




        }

       // tab2

        private void btnOpenVideo_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = false, Filter = "MP4 File|*mp4" };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                videoPath = openFileDialog.FileName;
                videoName = openFileDialog.SafeFileName;
                txtOpenVideo.Text = videoPath;
            }
        }

       

        private void btnSaveAudio_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(videoName))
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Спочатку виберіть відео!", Properties.Resources.Error_ICO30);
                return;
            }

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                musicPath = folderBrowserDialog.SelectedPath;
                musicName = Path.GetFileNameWithoutExtension(videoName);
                musicPath += ("\\" + musicName + ".mp3");
                txtSaveVideo.Text = musicPath;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOpenVideo.Text) || string.IsNullOrWhiteSpace(txtSaveVideo.Text))
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Шляхи до файлів не вказані!", Properties.Resources.Error_ICO30);
                return;
            }
            else if (File.Exists(musicPath))
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightGoldenrodYellow, Color.Gold, "Попередження :O", "Файл вже існує.", Properties.Resources.Warning_ICO30);
            }
            else
            {
                var convert = new NReco.VideoConverter.FFMpegConverter();
                convert.ConvertMedia(txtOpenVideo.Text.Trim(), txtSaveVideo.Text.Trim(), "mp3");
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightGray, Color.SeaGreen, "Успіх :)", "Операція виконана Успішно", Properties.Resources.Success_ICO30);
            }
            
        
        }

        /// Tab3

        private void btnTab3Input_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 files (*.mp4)|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputFileTab3 = openFileDialog.FileName;
                txtTab3Input.Text = inputFileTab3;
            }
        }

        private void btnTab3Output_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFileTab3 = Path.Combine(folderBrowserDialog.SelectedPath, "clip.mp4");
                    txtTab3Output.Text = outputFileTab3;
                }
            }
        }




        private void btnTrim_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputFileTab3) || string.IsNullOrWhiteSpace(outputFileTab3))
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Заповніть усі поля!", Properties.Resources.Error_ICO30);
            }
            else if (!File.Exists(inputFileTab3))
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Виберіть існуючі відеофайли!", Properties.Resources.Error_ICO30);
            }
            else if (File.Exists(outputFileTab3))
            {
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightGoldenrodYellow, Color.Gold, "Попередження :O", "Файл вже існує.", Properties.Resources.Warning_ICO30);
            }
            else
            {
                // Перевіряємо початковий і кінцевий час обрізки
                if (!TrimRangeValidator.TryValidate(txtStartTime.Text, txtEndTime.Text, out var trimRange, out string? error))
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightGoldenrodYellow, Color.Gold, "Попередження :O", error!, Properties.Resources.Warning_ICO30);
                    return;
                }

                try
                {
                    // Шлях до FFmpeg
                    string ffmpegPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");

                    // Отримуємо шляхи до вхідного і вихідного файлу
                    string inputFile = @$"{inputFileTab3}";
                    string outputFile = @$"{outputFileTab3}";

                    // Формуємо аргументи команди FFmpeg для обрізки відео
                    string arguments = $"-i \"{inputFile}\" -ss {trimRange.Start} -to {trimRange.End} -c copy \"{outputFile}\"";

                    // Створюємо процес для виконання команди FFmpeg
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = ffmpegPath;
                    startInfo.Arguments = arguments;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;

                    // Запускаємо процес
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();

                    // Очікуємо завершення процесу
                    process.WaitForExit();

                    System.Media.SystemSounds.Asterisk.Play();
                    if (process.ExitCode == 0)
                    {
                        AlertBox(Color.LightGray, Color.SeaGreen, "Успіх :)", "Операція виконана Успішно!", Properties.Resources.Success_ICO30);
                    }
                    else
                    {
                        AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", $"FFmpeg завершився з помилкою (код {process.ExitCode}).", Properties.Resources.Error_ICO30);
                    }
                }
                catch (Exception ex)
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", $"Сталася помилка під час обрізки відео: {ex.Message}", Properties.Resources.Error_ICO30);
                }
            }
        }
    }



}

