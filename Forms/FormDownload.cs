using ClipLab.Core;
using ClipLab.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NReco.VideoConverter;
using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

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

    


        private static readonly YoutubeClient YouTube = new();

        private void ShowError(string text) =>
            AlertBox(Color.LightPink, Color.DarkRed, "Помилка :(", text, Properties.Resources.Error_ICO30);

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            string videoUrl = txtUrl.Text;
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                System.Media.SystemSounds.Asterisk.Play();
                ShowError("Вставте посилання на відео!");
                return;
            }

            if (!YouTubeUrlValidator.IsValid(videoUrl))
            {
                System.Media.SystemSounds.Asterisk.Play();
                ShowError("Посилання не вірне.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSavePath.Text))
            {
                System.Media.SystemSounds.Asterisk.Play();
                ShowError("Вкажіть шлях збереження!");
                return;
            }

            try
            {
                // Дістаємо метадані відео та список доступних потоків (якостей)
                var video = await YouTube.Videos.GetAsync(videoUrl);
                var streamManifest = await YouTube.Videos.Streams.GetManifestAsync(videoUrl);

                bool convertToMp3 = chkAudioOnly.Checked;
                string safeTitle = FileNaming.SanitizeFileName(video.Title);
                string extension = convertToMp3 ? "mp3" : "mp4";
                string savePath = FileNaming.BuildSavePath(txtSavePath.Text, $"{safeTitle}.{extension}");

                if (File.Exists(savePath))
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    AlertBox(Color.LightGoldenrodYellow, Color.Gold, "Попередження :O", "Файл вже існує.", Properties.Resources.Warning_ICO30);
                    return;
                }

                AlertBox(Color.LightBlue, Color.DodgerBlue, "Очікуйте ( ͡° ͜ʖ ͡°)", "завантаження файлу", Properties.Resources.Hint_ICO30);

                if (convertToMp3)
                {
                    // Завантажуємо найкращий доступний аудіопотік і конвертуємо в mp3
                    var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
                    if (audioStreamInfo == null)
                    {
                        ShowError("Для цього відео немає доступного аудіопотоку.");
                        return;
                    }

                    string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.{audioStreamInfo.Container.Name}");
                    await YouTube.Videos.Streams.DownloadAsync(audioStreamInfo, tempPath);

                    // Конвертація ffmpeg - важка синхронна робота, виконуємо її поза UI-потоком
                    await Task.Run(() =>
                    {
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        ffMpeg.ConvertMedia(tempPath, savePath, "mp3");
                    });
                    File.Delete(tempPath);
                }
                else
                {
                    // Спершу пробуємо потік, де аудіо+відео вже об'єднані - найпростіший випадок.
                    var muxedStreamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();
                    if (muxedStreamInfo != null)
                    {
                        await YouTube.Videos.Streams.DownloadAsync(muxedStreamInfo, savePath);
                    }
                    else
                    {
                        // YouTube часто не має об'єднаного потоку вище 720p - тоді відео і аудіо
                        // йдуть окремими доріжками, і їх треба завантажити та об'єднати через ffmpeg.
                        var videoOnlyStreamInfo = streamManifest.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();
                        var audioOnlyStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
                        if (videoOnlyStreamInfo == null || audioOnlyStreamInfo == null)
                        {
                            ShowError("Для цього відео немає доступних потоків для завантаження.");
                            return;
                        }

                        string tempVideoPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.{videoOnlyStreamInfo.Container.Name}");
                        string tempAudioPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.{audioOnlyStreamInfo.Container.Name}");
                        await YouTube.Videos.Streams.DownloadAsync(videoOnlyStreamInfo, tempVideoPath);
                        await YouTube.Videos.Streams.DownloadAsync(audioOnlyStreamInfo, tempAudioPath);

                        // Об'єднання доріжок ffmpeg-ом - важка синхронна робота, виконуємо поза UI-потоком
                        await Task.Run(() =>
                        {
                            string ffmpegPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");
                            var startInfo = new ProcessStartInfo
                            {
                                FileName = ffmpegPath,
                                Arguments = $"-i \"{tempVideoPath}\" -i \"{tempAudioPath}\" -c copy \"{savePath}\"",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                            };
                            using var process = Process.Start(startInfo);
                            process!.WaitForExit();
                        });

                        File.Delete(tempVideoPath);
                        File.Delete(tempAudioPath);
                    }
                }

                // Назад на UI-потоці - тут можна безпечно показувати діалоги
                System.Media.SystemSounds.Asterisk.Play();
                AlertBox(Color.LightGray, Color.SeaGreen, "Успіх :)", "Відео успішно завантажено!", Properties.Resources.Success_ICO30);
            }
            catch (Exception ex)
            {
                System.Media.SystemSounds.Asterisk.Play();
                if (ex.Message.Contains("403"))
                {
                    AlertBox(Color.LightBlue, Color.DodgerBlue, "Помилка доступу", "Спробуйте ще раз! Якщо не допомогло, перезапустіть програму.", Properties.Resources.Hint_ICO30);
                }
                else
                {
                    ShowError("Невідома Помилка!");
                }
            }
        }
    }
}

