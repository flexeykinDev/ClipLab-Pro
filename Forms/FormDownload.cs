using ClipLab.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using System.IO;
using NReco.VideoConverter;
using System.Diagnostics;

namespace ClipLab.Forms
{
    public partial class FormDownload : Form
    {
        public FormDownload()
        {
            InitializeComponent();
            LoadTheme();

        }
        private void FormDownload_Load(object sender, EventArgs e)
        {
            LoadTheme();
        }

        private void LoadTheme()
        {
            foreach (Control btns in this.Controls)
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
             lblSaveAs.ForeColor = ThemeColor.SecondaryColor;
        }

        void AlertBox(Color backColor, Color color, string title, string text, Image icon)
        {
            AlertBox aRt = new AlertBox();
            aRt.BackColor = backColor;
            aRt.ColorAlertBox = color;
            aRt.TitleAlertBox = title;
            aRt.TextAlertBox = text;
            aRt.IconeAlertBox = icon;
            aRt.ShowDialog();
            aRt.TopMost = true;


        }




        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = folderBrowserDialog.SelectedPath;
                }
            }

        }

    


        private async void btnDownload_Click(object sender, EventArgs e)
        {
            
            try
            {
                // Отримуємо URL відео з текстового поля
                string videoUrl = txtUrl.Text;
                if (string.IsNullOrWhiteSpace(videoUrl))
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Вставте посилання на відео!", Properties.Resources.Error_ICO30);
                }
                Regex regex = new Regex(@"^(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+$");
                if (regex.IsMatch(videoUrl))
                {
                    // Створюємо об'єкт для роботи з YouTube
                    var youTube = YouTube.Default;

                    // Отримуємо інформацію про відео
                    var video = youTube.GetVideo(videoUrl);
                    // Отримуємо назву відео

                    string videoTitle = video.Title;

                    string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                    string safeTitle = string.Join("_", video.Title.Split(invalidChars.ToCharArray()));
                    string fullName = safeTitle + video.FileExtension;

                    if (string.IsNullOrWhiteSpace(txtSavePath.Text))
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                        AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Вкажіть шлях збереження!", Properties.Resources.Error_ICO30);
                    }
                    if (!string.IsNullOrWhiteSpace(txtSavePath.Text))
                    {
                        // Отримуємо шлях для збереження відео
                        string savePath = txtSavePath.Text;

                        // Додаємо назву файлу до шляху

                        if (!savePath.EndsWith("\\"))
                        {
                            savePath += "\\";
                        }
                        savePath += fullName;


                        // Запускаємо операцію завантаження відео в окремому потоці

                            await Task.Run(() =>
                            {
                                AlertBox(Color.LightBlue, Color.DodgerBlue, "Очікуйте ( ͡° ͜ʖ ͡°)", "завантаження файлу", Properties.Resources.Hint_ICO30);

                                // Отримуємо дані відео у вигляді масиву байт
                                byte[] videoData = video.GetBytes();


                                // Зберігаємо відео на диск
                                if (File.Exists(savePath))
                                {

                                    System.Media.SystemSounds.Asterisk.Play();
                                    AlertBox(Color.LightGoldenrodYellow, Color.Gold, "Попередження :O", "Файл вже існує.", Properties.Resources.Warning_ICO30);
                                }
                                else
                                {
                                    
                                    bool convertToMp3 = chkAudioOnly.Checked;

                                    if (convertToMp3)
                                    {
                                        // Створюємо об'єкт для конвертації відео
                                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                                        var conv = new FFMpegConverter();
                                        // Зберігаємо відео на диск
                                        File.WriteAllBytes(savePath, videoData);
                                        Thread.Sleep(500);
                                        if (File.Exists(savePath))
                                        {
                                            // Шлях до вихідного файлу
                                            var sourcePath = savePath;
                                            
                                            var outputPath = Path.Combine(Path.GetDirectoryName(savePath), Path.GetFileNameWithoutExtension(savePath)) + ".mp3";
                                            // Конвертуємо відео у MP3
                                            ffMpeg.ConvertMedia(sourcePath.Trim(), outputPath.Trim(), "mp3");
                                            if (File.Exists(outputPath))
                                            {
                                                File.Delete(sourcePath);
                                            }
                                        }


                                    }
                                    else
                                    {
                                        // Зберігаємо відео на диск
                                        File.WriteAllBytes(savePath, videoData);
                                    }


                                    System.Media.SystemSounds.Asterisk.Play();
                                    AlertBox(Color.LightGray, Color.SeaGreen, "Успіх :)", "Відео успішно завантажено!", Properties.Resources.Success_ICO30);
                                }
                            
                            });

                    }
                }
                else
                {
                    
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Посилання не вірне.", Properties.Resources.Error_ICO30);
                }
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("403")) // Якщо повідомлення про помилку містить "403"
                {
                    
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightBlue, Color.DodgerBlue, "Помилка доступу", "Спробуйте ще раз!", Properties.Resources.Hint_ICO30);
                    AlertBox(Color.LightBlue, Color.DodgerBlue, "Помилка доступу", "якщо не допомогло, перезапустіть програму!",  Properties.Resources.Hint_ICO30);

                }
                else // Якщо інша помилка
                {

                    System.Media.SystemSounds.Asterisk.Play();
                    
                    AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", "Невідома Помилка!", Properties.Resources.Error_ICO30);
                    
                }
            }

        }
    }
}

