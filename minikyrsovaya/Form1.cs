using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace minikyrsovaya
{
    public partial class Form1 : Form
    {
        private const int BoardSize = 8;
        private const int CellSize = 60;
        private const int AnimationDuration = 50;

        private char[,] board;
        private int score;
        private int targetScore;
        private int timeLeft;
        private Random random = new Random();
        private Point selectedCell = new Point(-1, -1);
        private Button[,] cellButtons;
        private bool isAnimating = false;
        private List<int> highScores = new List<int>();
        private const string HighScoresFile = "highscores.txt";
        private GameMode currentGameMode = GameMode.Endless;
        private System.Windows.Forms.Timer gameTimer;
        private Image trophyNormal;
        private Image trophyDark;
        private Image settingsNormal;
        private Image settingsDark;
        private Dictionary<char, Image> symbolImages = new Dictionary<char, Image>();



        public enum GameMode
        {
            Endless,
            Easy,
            Medium,
            Hard
        }

        public Form1()
        {
            InitializeComponent();
            // Включение двойной буферизации для всей формы
            this.DoubleBuffered = true;

            
            InitializeTimer();
            InitializeGameData();

            LoadSymbolImages();
            InitializeSounds();


            ShowStartScreen();
            currentColor = Color.Red;
            targetColor = Color.OrangeRed;
            animationStep = 0;
            InitializeColorAnimation();

            
            LoadVolumeSettings();
            PlayBackgroundMusic();


            trophyNormal = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\trofeii.png"); // Обычная
            trophyDark = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\trofeii_dark.png");     // Затемненная

            picTrophy.Image = trophyNormal;
            // Загружаем картинки для настроек
            settingsNormal = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\settings_normal.png");
            settingsDark = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\settings_dark.png");
            picSettings.Image = settingsNormal;


        }
        private System.Windows.Forms.Timer colorAnimationTimer;
        private Color currentColor = Color.Red;
        private Color targetColor = Color.Orange;
        private int animationStep = 0;
        private const int animationSteps = 30; // Количество шагов для плавного перехода

        private WaveOutEvent backgroundMusicPlayer;
        private AudioFileReader backgroundMusicFile;
        private SoundPlayer buttonHoverSound;
        private SoundPlayer buttonClickSound;
        private List<SoundPlayer> matchSounds = new List<SoundPlayer>();
        private Random soundRandom = new Random();
        private bool isMusicPlaying = false;

        private void InitializeColorAnimation()
        {
            colorAnimationTimer = new System.Windows.Forms.Timer();
            colorAnimationTimer.Interval = 30; // Быстрее для плавности
            colorAnimationTimer.Tick += ColorAnimationTimer_Tick;
            colorAnimationTimer.Start();
        }

        private void InitializeSounds()
        {
            try
            {
                // Фоновая музыка (WAV)
                // Фоновая музыка через NAudio (поддерживает MP3)
                backgroundMusicPlayer = new WaveOutEvent();
                backgroundMusicFile = new AudioFileReader("D:\\visualstusio_projects\\minikyrsovaya\\sounds\\backsound.mp3"); // Используйте MP3
                backgroundMusicPlayer.Init(backgroundMusicFile);
                backgroundMusicPlayer.Volume = 0.5f; // Громкость 50%

                // Звук наведения на кнопку
                buttonHoverSound = new SoundPlayer("D:\\visualstusio_projects\\minikyrsovaya\\sounds\\hover.wav");

                // Звук клика по кнопке
                buttonClickSound = new SoundPlayer("D:\\visualstusio_projects\\minikyrsovaya\\sounds\\click.wav");

                // Звуки совпадения (два разных звука)
                matchSounds.Add(new SoundPlayer("D:\\visualstusio_projects\\minikyrsovaya\\sounds\\match1.wav"));
                matchSounds.Add(new SoundPlayer("D:\\visualstusio_projects\\minikyrsovaya\\sounds\\match2.wav"));

                // Предзагрузка звуков
                
                buttonHoverSound.LoadAsync();
                buttonClickSound.LoadAsync();
                foreach (var sound in matchSounds)
                {
                    sound.LoadAsync();
                }

                Console.WriteLine("Все звуки загружены"); // Для отладки
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки звуков: {ex.Message}");
            }
        }

        private void SaveVolumeSettings()
        {
            
            {
                // Сохраняем в файл в папке приложения
                string settingsPath = Path.Combine(Application.StartupPath, "game_settings.txt");
                string volumeValue = backgroundMusicPlayer.Volume.ToString();
                File.WriteAllText(settingsPath, $"Volume={volumeValue}");
            }
            
        }

        private void LoadVolumeSettings()
        {
            
            {
                string settingsPath = Path.Combine(Application.StartupPath, "game_settings.txt");
                if (File.Exists(settingsPath))
                {
                    string settings = File.ReadAllText(settingsPath);
                    // Ищем значение Volume=
                    if (settings.Contains("Volume="))
                    {
                        string volumeStr = settings.Split('=')[1];
                        if (float.TryParse(volumeStr, out float volume))
                        {
                            volume = Math.Max(0, Math.Min(1, volume)); // Ограничиваем от 0 до 1
                            backgroundMusicPlayer.Volume = volume;
                        }
                    }
                }
            }
           
        }
        private void PlayBackgroundMusic()
        {
            try
            {
                if (backgroundMusicPlayer != null && !isMusicPlaying)
                {
                    backgroundMusicPlayer.Play();
                    isMusicPlaying = true;

                    // Подписываемся на событие окончания трека для зацикливания
                    backgroundMusicPlayer.PlaybackStopped += (s, e) =>
                    {
                        if (isMusicPlaying)
                        {
                            backgroundMusicFile.Position = 0; // Перематываем в начало
                            backgroundMusicPlayer.Play();
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка воспроизведения музыки: {ex.Message}");
            }
        }

        private void StopBackgroundMusic()
        {
            if (backgroundMusicPlayer != null && isMusicPlaying)
            {
                backgroundMusicPlayer.Stop();
                isMusicPlaying = false;
            }
        }


        private void PlayButtonHoverSound()
        {
            buttonHoverSound?.Play();
        }

        private void PlayButtonClickSound()
        {
            buttonClickSound?.Play();
        }

        private void PlayRandomMatchSound()
        {
            if (matchSounds.Count > 0)
            {
                int randomIndex = soundRandom.Next(matchSounds.Count);
                matchSounds[randomIndex]?.Play();
            }
        }

        public class BufferedPanel : Panel
        {
            public BufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                             ControlStyles.UserPaint |
                             ControlStyles.OptimizedDoubleBuffer, true);
            }
        }

        private void LoadSymbolImages()
        {
            try
            {
                // Замените пути на ваши PNG файлы
                symbolImages['A'] = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\bone.png");
                symbolImages['B'] = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\candy.png");
                symbolImages['C'] = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\spider.png");
                symbolImages['D'] = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\mish.png");
                symbolImages['E'] = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\kotel.png");
                symbolImages['F'] = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\tikva2.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки картинок: {ex.Message}");
            }
        }

        private void picTrophy_MouseEnter(object sender, EventArgs e)
        {
            PlayButtonHoverSound();
            picTrophy.Image = trophyDark; // Затемненная картинка
        }

        private void picTrophy_MouseLeave(object sender, EventArgs e)
        {
            picTrophy.Image = trophyNormal; // Обычная картинка
        }

        private void picSettings_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            ShowSettingsWindow();
        }

        private void picSettings_MouseEnter(object sender, EventArgs e)
        {
            PlayButtonHoverSound();
            picSettings.Image = settingsDark;
        }

        private void picSettings_MouseLeave(object sender, EventArgs e)
        {
            picSettings.Image = settingsNormal;
        }

        private void ColorAnimationTimer_Tick(object sender, EventArgs e)
        {
            if (lblScore.Visible)
            {
                // Плавный переход между цветами
                int r = currentColor.R + (targetColor.R - currentColor.R) * animationStep / animationSteps;
                int g = currentColor.G + (targetColor.G - currentColor.G) * animationStep / animationSteps;
                int b = currentColor.B + (targetColor.B - currentColor.B) * animationStep / animationSteps;

                lblScore.ForeColor = Color.FromArgb(r, g, b);

                animationStep++;

                if (animationStep > animationSteps)
                {
                    // Переходим к следующему цвету
                    animationStep = 0;
                    currentColor = targetColor;

                    // Выбираем следующий целевой цвет
                    targetColor = GetNextRainbowColor(targetColor);
                }
            }
        }

        private Color GetNextRainbowColor(Color current)
        {
            // Плавная радужная последовательность
            if (current == Color.Red) return Color.OrangeRed;
            if (current == Color.OrangeRed) return Color.Orange;
            if (current == Color.Orange) return Color.Gold;
            if (current == Color.Gold) return Color.YellowGreen;
            if (current == Color.YellowGreen) return Color.LimeGreen;
            if (current == Color.LimeGreen) return Color.DodgerBlue;
            if (current == Color.DodgerBlue) return Color.Blue;
            if (current == Color.Blue) return Color.BlueViolet;
            if (current == Color.BlueViolet) return Color.Violet;
            return Color.Red; // Зацикливаем
        }

        private void InitializeTimer()
        {
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
        }
        private void RemoveBlockPanel()
        {
            // Ищем и удаляем блокирующую панель
            foreach (Control control in this.Controls.OfType<Panel>().ToList())
            {
                if (control.Name == "GameBlockPanel")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }
        }

        private void ShowStartScreen()
        {
            RemoveBlockPanel();

            // Показываем панель выбора уровня и скрываем игровые элементы
            startPanel.Visible = true;
            //PlayBackgroundMusic();
            gamePanel.Visible = false;
            lblScore.Visible = false;
            picNewGame.Visible = false;
            picHighScores.Visible = false;
            picTitle.Visible = false;
            lblTime.Visible = false;
            lblTarget.Visible = false;
            progressBar.Visible = false;

            // Скрываем картинку уровня
            if (picTitle != null)
                picTitle.Visible = false;

            //lblScore.ForeColor = Color.Black;
        }

        private void StartGame(string level)
        {
            // Убираем блокирующую панель если она есть
            foreach (Control control in this.Controls.OfType<Panel>().ToList())
            {
                if (control.Name == "GameBlockPanel")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }
            // Конвертируем текстовый уровень в GameMode
            currentGameMode = level switch
            {
                "Легкий" => GameMode.Easy,
                "Средний" => GameMode.Medium,
                "Сложный" => GameMode.Hard,
                "Бесконечный" => GameMode.Endless,
                _ => GameMode.Endless
            };

            // Скрываем стартовое окно
            startPanel.Visible = false;

            // Показываем игровые элементы
            gamePanel.Visible = true;
            lblScore.Visible = true;
            picNewGame.Visible = true;
            picHighScores.Visible = true;
            picTitle.Visible = true;
            lblTime.Visible = true;
            lblTarget.Visible = true;
            progressBar.Visible = true;

            // Скрываем заголовок и показываем картинку уровня
            picTitle.Visible = false;
            ShowLevelImage(level);





            // Запускаем уровень с выбранными настройками
            StartLevel();
        }

        private void ShowLevelImage(string level)
        {
            try
            {
                string imagePath = level switch
                {
                    "Легкий" => "D:\\visualstusio_projects\\minikyrsovaya\\png_images\\easyt.png",
                    "Средний" => "D:\\visualstusio_projects\\minikyrsovaya\\png_images\\midt.png",
                    "Сложный" => "D:\\visualstusio_projects\\minikyrsovaya\\png_images\\hardt.png",
                    "Бесконечный" => "D:\\visualstusio_projects\\minikyrsovaya\\png_images\\endlesst.png",
                    _ => "D:\\visualstusio_projects\\minikyrsovaya\\png_images\\endlesst.png"
                };

                // Создаем или обновляем PictureBox для отображения картинки уровня
                if (picTitle == null)
                {
                    picTitle = new PictureBox();
                    picTitle.Size = new Size(300, 100);
                    picTitle.SizeMode = PictureBoxSizeMode.Zoom;
                    picTitle.Location = new Point(250, 2);
                    picTitle.BackColor = Color.Transparent;
                    this.Controls.Add(picTitle);
                }

                picTitle.Image = Image.FromFile(imagePath);
                picTitle.Visible = true;
                picTitle.BringToFront(); // Чтобы была поверх других элементов
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить картинку уровня: {ex.Message}");
            }
        }
        private void ShowSettingsWindow()
        {
            // Создаем хэллоуинское окно настроек
            Form settingsForm = new Form()
            {
                Text = "Магические настройки",
                Size = new Size(500, 500),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(50, 0, 0),
                Icon = this.Icon
            };

            // Заголовок
            Label titleLabel = new Label()
            {
                Text = "🔮 МАГИЧЕСКИЕ НАСТРОЙКИ 🎃",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Transparent,
                Location = new Point(35, 20),
                AutoSize = true
            };

            // Громкость музыки
            Label volumeLabel = new Label()
            {
                Text = "🔊 Громкость музыки:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Transparent,
                Location = new Point(10, 80),
                AutoSize = true
            };

            // Ползунок громкости
            TrackBar volumeTrackBar = new TrackBar()
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(backgroundMusicPlayer.Volume * 100),
                Location = new Point(10, 120),
                Size = new Size(390, 45),
                BackColor = Color.FromArgb(60, 0, 0),
                ForeColor = Color.Orange
            };

            // Метка текущего значения громкости
            Label volumeValueLabel = new Label()
            {
                Text = $"{volumeTrackBar.Value}%",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                Location = new Point(402, 125),
                AutoSize = true
            };

            // Обработчик изменения громкости
            volumeTrackBar.Scroll += (s, e) =>
            {
                float volume = volumeTrackBar.Value / 100f;
                backgroundMusicPlayer.Volume = volume;
                UpdateVolumeLabel(volumeValueLabel, volumeTrackBar.Value);
            };

            // Информационный текст
            //Label infoLabel = new Label()
            //{
            //    Text = "Другие магические настройки будут доступны\nв следующем полнолунии! 🌕",
            //    Font = new Font("Segoe UI", 11, FontStyle.Regular),
            //    ForeColor = Color.OrangeRed,
            //    BackColor = Color.Transparent,
            //    Location = new Point(80, 180),
            //    AutoSize = true,
            //    TextAlign = ContentAlignment.MiddleCenter
            //};

            // Кнопка закрытия
            Button closeButton = new Button()
            {
                Text = "💀 ЗАКРЫТЬ И СОХРАНИТЬ 💀",
                BackColor = Color.DarkRed,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(80, 380),
                Size = new Size(340, 35),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderColor = Color.Orange;
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(120, 0, 0);

            closeButton.Click += (s, e) =>
            {
                PlayButtonClickSound();
                SaveVolumeSettings(); // Сохраняем настройки перед закрытием
                settingsForm.Close();
            };

            // Добавляем элементы на форму
            settingsForm.Controls.Add(titleLabel);
            settingsForm.Controls.Add(volumeLabel);
            settingsForm.Controls.Add(volumeTrackBar);
            settingsForm.Controls.Add(volumeValueLabel);
            //settingsForm.Controls.Add(infoLabel);
            settingsForm.Controls.Add(closeButton);

            // Обновляем иконку громкости при загрузке
            UpdateVolumeLabel(volumeValueLabel, volumeTrackBar.Value);

            // Сохраняем настройки даже если закрыли крестиком
            settingsForm.FormClosing += (s, e) =>
            {
                SaveVolumeSettings();
            };

            // Показываем как диалоговое окно
            settingsForm.ShowDialog();
        }

        // Вспомогательный метод для обновления метки громкости
        private void UpdateVolumeLabel(Label label, int value)
        {
            if (value == 0)
                label.Text = "🔇 0%";
            else if (value < 30)
                label.Text = $"🔈 {value}%";
            else if (value < 70)
                label.Text = $"🔉 {value}%";
            else
                label.Text = $"🔊 {value}%";
        }

        

        private void ShowNewGameConfirmation()
        {
            // Создаем хэллоуинское окно подтверждения
            Form confirmForm = new Form()
            {
                Text = "Подтверждение",
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(40, 0, 0),
                ForeColor = Color.Orange,
                Icon = this.Icon
            };

            // Заголовок
            Label titleLabel = new Label()
            {
                Text = "🎃 НОВАЯ ИГРА 🎃",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Transparent,
                Location = new Point(50, 10),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Текст подтверждения
            Label messageLabel = new Label()
            {
                Text = "Вы уверены, что хотите начать новую игру?\nТекущий прогресс будет потерян!",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.OrangeRed,
                BackColor = Color.Transparent,
                Location = new Point(20, 50),
                Size = new Size(350, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Кнопка Да
            Button yesButton = new Button()
            {
                Text = "👻 ДА",
                BackColor = Color.DarkRed,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(80, 140),
                Size = new Size(100, 35),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Yes
            };
            yesButton.FlatAppearance.BorderColor = Color.Orange;
            yesButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(160, 0, 0);

            // Кнопка Нет
            Button noButton = new Button()
            {
                Text = "💀 НЕТ",
                BackColor = Color.DarkGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                Location = new Point(220, 140),
                Size = new Size(100, 35),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.No
            };
            noButton.FlatAppearance.BorderColor = Color.LightGreen;
            noButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 100, 0);

            // Анимация для кнопок
            yesButton.MouseEnter += (s, e) => yesButton.Text = "🔥 ДА";
            yesButton.MouseLeave += (s, e) => yesButton.Text = "👻 ДА";

            noButton.MouseEnter += (s, e) => noButton.Text = "🕸️ НЕТ";
            noButton.MouseLeave += (s, e) => noButton.Text = "💀 НЕТ";

            yesButton.Click += (s, e) => PlayButtonClickSound();
            noButton.Click += (s, e) => PlayButtonClickSound();

            // Добавляем элементы на форму
            confirmForm.Controls.Add(titleLabel);
            confirmForm.Controls.Add(messageLabel);
            confirmForm.Controls.Add(yesButton);
            confirmForm.Controls.Add(noButton);

            // Устанавливаем кнопки по умолчанию
            confirmForm.AcceptButton = yesButton;
            confirmForm.CancelButton = noButton;

            // Показываем диалог и обрабатываем результат
            DialogResult result = confirmForm.ShowDialog();

            if (result == DialogResult.Yes)
            {
                // Подтверждено - начинаем новую игру
                if (score > 0)
                {
                    AddHighScore(score);
                }
                gameTimer.Stop();
                ShowStartScreen();
            }
            // Если Нет или крестик - просто закрываем окно, игра продолжается
        }

        private void picEasy_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            StartGame("Легкий");
        }

        private void picMedium_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            StartGame("Средний");
        }

        private void picHard_Click(object sender, EventArgs e)
        {
            
            StartGame("Сложный");
            PlayButtonClickSound();
        }

        private void picEndless_Click(object sender, EventArgs e)
        {
            StartGame("Бесконечный");
            PlayButtonClickSound();
        }

        private void picExit_Click(object sender, EventArgs e)
        {
            if (score > 0)
            {
                AddHighScore(score);
            }
            PlayButtonClickSound();
            Application.Exit();

        }

        private void btnHighScoresStart_Click(object sender, EventArgs e)
        {
            ShowHighScores();
            PlayButtonClickSound();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (currentGameMode != GameMode.Endless)
            {
                timeLeft--;
                UpdateTimeDisplay();

                if (timeLeft <= 0)
                {
                    gameTimer.Stop();
                    EndGame(false);
                }
                else if (score >= targetScore)
                {
                    gameTimer.Stop();
                    EndGame(true);
                }
            }
        }

        private void UpdateTimeDisplay()
        {
            if (currentGameMode != GameMode.Endless)
            {
                TimeSpan time = TimeSpan.FromSeconds(timeLeft);
                lblTime.Text = $"Время: {time:mm\\:ss}";
                lblTime.ForeColor = Color.Maroon;
            }
            else
            {
                lblTime.Text = "Режим: Бесконечный";
            }
        }

        private void StartLevel()
        {
            switch (currentGameMode)
            {
                case GameMode.Easy:
                    timeLeft = 120;
                    targetScore = 500;
                    break;
                case GameMode.Medium:
                    timeLeft = 180;
                    targetScore = 1000;
                    break;
                case GameMode.Hard:
                    timeLeft = 240;
                    targetScore = 1500;
                    break;
                case GameMode.Endless:
                    timeLeft = 0;
                    targetScore = 0;
                    break;
            }

            score = 0;
            UpdateScoreDisplay();
            UpdateTimeDisplay();
            UpdateTargetDisplay();

            if (currentGameMode != GameMode.Endless)
            {
                gameTimer.Start();
            }
            else
            {
                gameTimer.Stop();
            }

            InitializeGame();
        }

        private void UpdateTargetDisplay()
        {
            if (currentGameMode != GameMode.Endless)
            {
                lblTarget.BackColor = Color.Transparent;
                lblTarget.Text = $"Цель: {targetScore}";
                lblTarget.Visible = true;  // Показываем цель для режимов с целью
            }
            else
            {
                lblTarget.Visible = false;  // Скрываем lblTarget в бесконечном режиме
            }
        }

        private void EndGame(bool isWin)
        {
            
            string title, message, icon;

            if (isWin)
            {
                title = "ПОБЕДА! 🎉";
                message = $"Поздравляем! Вы набрали {score} очков\nза отведенное время!";
                icon = "🏆";
            }
            else
            {
                title = "ВРЕМЯ ВЫШЛО! ⏰";
                message = $"Время вышло! Вы набрали {score} очков\nиз {targetScore} необходимых";
                icon = "💀";
            }

            ShowBeautifulMessage(title, message, icon);

            if (score > 0)
            {
                AddHighScore(score);
            }

            BlockGameBoard();
        }

        private void LoadHighScores()
        {
            highScores.Clear();
            try
            {
                // Загружаем только из основного файла
                if (File.Exists(HighScoresFile))
                {
                    var lines = File.ReadAllLines(HighScoresFile);
                    foreach (var line in lines)
                    {
                        if (int.TryParse(line, out int score))
                        {
                            highScores.Add(score);
                        }
                    }
                    highScores = highScores.OrderByDescending(s => s).Take(10).ToList();
                }

                // Удаляем дубликаты из файла с режимами, если они есть
                RemoveDuplicateScores();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки рекордов: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Метод для удаления дубликатов
        private void RemoveDuplicateScores()
        {
            try
            {
                if (File.Exists("scores_with_modes.txt"))
                {
                    var lines = File.ReadAllLines("scores_with_modes.txt");
                    var uniqueScores = new HashSet<string>();
                    var cleanedLines = new List<string>();

                    foreach (var line in lines)
                    {
                        if (!uniqueScores.Contains(line))
                        {
                            uniqueScores.Add(line);
                            cleanedLines.Add(line);
                        }
                    }

                    // Перезаписываем файл без дубликатов
                    if (cleanedLines.Count != lines.Length)
                    {
                        File.WriteAllLines("scores_with_modes.txt", cleanedLines);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления дубликатов: {ex.Message}");
            }
        }

        private void SaveHighScores()
        {
            try
            {
                var topScores = highScores.OrderByDescending(s => s).Take(10).ToList();
                File.WriteAllLines(HighScoresFile, topScores.Select(s => s.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения рекордов: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddHighScore(int newScore)
        {
            if (newScore > 0)
            {
                // Проверяем, нет ли уже такого счета
                if (!highScores.Contains(newScore))
                {
                    highScores.Add(newScore);
                    highScores = highScores.OrderByDescending(s => s).Take(10).ToList();
                    SaveHighScores();

                    // Сохраняем с информацией о режиме
                    string mode = currentGameMode.ToString();
                    SaveScoreWithMode(newScore, mode);
                }
            }
        }

        private void SaveScoreWithMode(int score, string mode)
        {
            try
            {
                File.AppendAllText("scores_with_modes.txt", $"{score}|{mode}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения рекорда: {ex.Message}");
            }
        }

        private void ShowHighScores()
        {
            // Создаем хэллоуинское окно рекордов
            Form scoresForm = new Form()
            {
                Text = "Таблица рекордов",
                Size = new Size(700, 550), // Увеличили ширину для боковой панели
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(40, 0, 0),
                ForeColor = Color.Orange,
                Icon = this.Icon
            };

            // Заголовок
            Label titleLabel = new Label()
            {
                Text = "💀 КОЛДОВСКИЕ РЕКОРДЫ 🎃",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Transparent,
                Location = new Point(0, 0),
                Size = new Size(430, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Основная панель рекордов
            Panel scoresPanel = new Panel()
            {
                Location = new Point(20, 70),
                Size = new Size(350, 350),
                BackColor = Color.FromArgb(60, 0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.Orange,
                AutoScroll = true
            };

            // Боковая панель лучших результатов по уровням
            Panel bestScoresPanel = new Panel()
            {
                Location = new Point(390, 70),
                Size = new Size(280, 350),
                BackColor = Color.FromArgb(60, 0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.Orange
            };

            // Заголовок боковой панели
            Label bestScoresTitle = new Label()
            {
                Text = "🏆 ЛУЧШИЕ РЕЗУЛЬТАТЫ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                Location = new Point(400, 0),
                Size = new Size(250, 65),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Загружаем все рекорды и группируем по уровням
            var allScores = LoadAllScores();
            var bestEasy = allScores.Where(s => s.Mode == "Easy").OrderByDescending(s => s.Score).FirstOrDefault();
            var bestMedium = allScores.Where(s => s.Mode == "Medium").OrderByDescending(s => s.Score).FirstOrDefault();
            var bestHard = allScores.Where(s => s.Mode == "Hard").OrderByDescending(s => s.Score).FirstOrDefault();
            var bestEndless = allScores.Where(s => s.Mode == "Endless").OrderByDescending(s => s.Score).FirstOrDefault();

            int yPos = 20;

            // Лучший результат - Легкий
            AddBestScorePanel(bestScoresPanel, "🎯 ЛЕГКИЙ", bestEasy?.Score ?? 0, Color.LightGreen, ref yPos);

            // Лучший результат - Средний
            AddBestScorePanel(bestScoresPanel, "⚡ СРЕДНИЙ", bestMedium?.Score ?? 0, Color.Gold, ref yPos);

            // Лучший результат - Сложный
            AddBestScorePanel(bestScoresPanel, "🔥 СЛОЖНЫЙ", bestHard?.Score ?? 0, Color.OrangeRed, ref yPos);

            // Лучший результат - Бесконечный
            AddBestScorePanel(bestScoresPanel, "♾️ БЕСКОНЕЧНЫЙ", bestEndless?.Score ?? 0, Color.LightBlue, ref yPos);

            // Создаем список рекордов в основной панели
            int mainYPos = 15;
            if (highScores.Count == 0)
            {
                Label noScoresLabel = new Label()
                {
                    Text = "🕸️ Пока нет рекордов!\nСыграйте в игру! 🦇",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.OrangeRed,
                    BackColor = Color.Transparent,
                    Location = new Point(40, 120),
                    Size = new Size(270, 80),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                scoresPanel.Controls.Add(noScoresLabel);
            }
            else
            {
                // Заголовок таблицы
                Label headerLabel = new Label()
                {
                    Text = "Место       Очки       Режим",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.Gold,
                    BackColor = Color.Transparent,
                    Location = new Point(20, mainYPos),
                    Size = new Size(300, 25),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                scoresPanel.Controls.Add(headerLabel);
                mainYPos += 30;

                // Разделитель
                Label separator = new Label()
                {
                    Text = "─────────────────────────────────────",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.DarkOrange,
                    BackColor = Color.Transparent,
                    Location = new Point(20, mainYPos),
                    Size = new Size(300, 20),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                scoresPanel.Controls.Add(separator);
                mainYPos += 25;

                // Показываем топ-10 рекордов
                var topScores = highScores.Take(10).ToList();
                for (int i = 0; i < topScores.Count; i++)
                {
                    string icon = "";
                    Color textColor = Color.Orange;

                    if (i == 0)
                    {
                        icon = "👑 ";
                        textColor = Color.Gold;
                    }
                    else if (i == 1)
                    {
                        icon = "⚡";
                        textColor = Color.Silver;
                    }
                    else if (i == 2)
                    {
                        icon = "🔮";
                        textColor = Color.OrangeRed;
                    }
                    else if (i < 5)
                    {
                        icon = "🎃 ";
                        textColor = Color.Orange;
                    }
                    else
                    {
                        icon = "💀 ";
                        textColor = Color.DarkOrange;
                    }

                    // Получаем режим для этого счета (если есть информация)
                    string mode = GetScoreMode(topScores[i]);

                    Label scoreLabel = new Label()
                    {
                        Text = $"{icon} {i + 1}.      {topScores[i]}       {mode}",
                        Font = new Font("Segoe UI", 11, i < 3 ? FontStyle.Bold : FontStyle.Regular),
                        ForeColor = textColor,
                        BackColor = Color.Transparent,
                        Location = new Point(25, mainYPos),
                        Size = new Size(300, 25),
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    scoresPanel.Controls.Add(scoreLabel);
                    mainYPos += 30;
                }
            }

            // Отключаем горизонтальный скролл
            scoresPanel.HorizontalScroll.Enabled = false;
            scoresPanel.HorizontalScroll.Visible = false;

            // Кнопка закрытия
            Button closeButton = new Button()
            {
                Text = "🕷️ ЗАКРЫТЬ",
                BackColor = Color.DarkRed,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(275, 440),
                Size = new Size(180, 35),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderColor = Color.Orange;
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(160, 0, 0);
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 0, 0);

            closeButton.Click += (s, e) =>
            {
                PlayButtonClickSound();
                scoresForm.Close();
            };
            // Кнопка очистки рекордов
            Button clearButton = new Button()
            {
                Text = "🧹 ОЧИСТИТЬ РЕКОРДЫ",
                BackColor = Color.DarkSlateGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.LightGray,
                Location = new Point(20, 440),
                Size = new Size(180, 35),
                Cursor = Cursors.Hand
            };
            clearButton.FlatAppearance.BorderColor = Color.Gray;
            clearButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 80, 80);
            clearButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 60, 60);

            clearButton.Click += (s, e) =>
            {
                PlayButtonClickSound();
                ClearHighScores(scoresForm);
            };


            // Анимация для кнопки
            closeButton.MouseEnter += (s, e) =>
            {
                closeButton.BackColor = Color.FromArgb(160, 0, 0);
                closeButton.Text = "👻 ЗАКРЫТЬ 👻";
            };

            closeButton.MouseLeave += (s, e) =>
            {
                closeButton.BackColor = Color.DarkRed;
                closeButton.Text = "🕷️ ЗАКРЫТЬ";
            };

            clearButton.MouseEnter += (s, e) =>
            {
                clearButton.BackColor = Color.FromArgb(120, 0, 0);
                clearButton.Text = "💥 ОЧИСТИТЬ 💥";
            };

            clearButton.MouseLeave += (s, e) =>
            {
                clearButton.BackColor = Color.DarkSlateGray;
                clearButton.Text = "🧹 ОЧИСТИТЬ";
            };

            // Добавляем элементы на форму
            scoresForm.Controls.Add(titleLabel);
            scoresForm.Controls.Add(scoresPanel);
            scoresForm.Controls.Add(bestScoresPanel);
            scoresForm.Controls.Add(bestScoresTitle);
            scoresForm.Controls.Add(closeButton);
            scoresForm.Controls.Add(clearButton);

            // Показываем как диалоговое окно
            scoresForm.ShowDialog();
        }

        private void ShowBeautifulMessage(string title, string message, string icon)
        {
            Form messageForm = new Form()
            {
                Text = title,
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(40, 0, 0),
                ForeColor = Color.Orange,
                Icon = this.Icon
            };

            // Иконка сообщения
            Label iconLabel = new Label()
            {
                Text = icon,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Transparent,
                Location = new Point(160, 10),
                AutoSize = true
            };

            // Текст сообщения
            Label messageLabel = new Label()
            {
                Text = message,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.OrangeRed,
                BackColor = Color.Transparent,
                Location = new Point(5, 50),
                Size = new Size(380, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Кнопка OK
            Button okButton = new Button()
            {
                Text = "👻 ПОНЯТНО",
                BackColor = Color.DarkRed,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(115, 160),
                Size = new Size(155, 35),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            okButton.FlatAppearance.BorderColor = Color.Orange;
            okButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(160, 0, 0);

            okButton.Click += (s, e) =>
            {
                PlayButtonClickSound();
                messageForm.Close();
            };

            messageForm.Controls.Add(iconLabel);
            messageForm.Controls.Add(messageLabel);
            messageForm.Controls.Add(okButton);

            messageForm.ShowDialog();
        }

        private void ClearHighScores(Form parentForm)
        {
            // Создаем окно подтверждения
            Form confirmForm = new Form()
            {
                Text = "Подтверждение",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(40, 0, 0),
                ForeColor = Color.Orange
            };

            Label messageLabel = new Label()
            {
                Text = "Вы уверены, что хотите очистить\nвсе рекорды?",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.OrangeRed,
                BackColor = Color.Transparent,
                Location = new Point(10, 10),
                Size = new Size(370, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button yesButton = new Button()
            {
                Text = "💀 ДА",
                BackColor = Color.DarkRed,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 100),
                Size = new Size(100, 30),
                Cursor = Cursors.Hand
            };

            Button noButton = new Button()
            {
                Text = "❌ ОТМЕНА",
                BackColor = Color.DarkGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                Location = new Point(200, 100),
                Size = new Size(130, 30),
                Cursor = Cursors.Hand
            };

            yesButton.Click += (s, e) =>
            {
                // Очищаем рекорды
                highScores.Clear();
                SaveHighScores();

                // Очищаем файл с режимами
                try
                {
                    if (File.Exists("scores_with_modes.txt"))
                        File.Delete("scores_with_modes.txt");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка очистки файла режимов: {ex.Message}");
                }

                PlayButtonClickSound();
                confirmForm.Close();
                parentForm.Close(); // Закрываем окно рекордов
                ShowHighScores(); // Открываем заново с пустыми рекордами
            };

            noButton.Click += (s, e) =>
            {
                PlayButtonClickSound();
                confirmForm.Close();
            };

            confirmForm.Controls.Add(messageLabel);
            confirmForm.Controls.Add(yesButton);
            confirmForm.Controls.Add(noButton);

            confirmForm.ShowDialog();
        }

        // Вспомогательный метод для добавления панели лучшего результата
        private void AddBestScorePanel(Panel parent, string modeName, int score, Color color, ref int yPos)
        {
            Panel scorePanel = new Panel()
            {
                Location = new Point(20, yPos),
                Size = new Size(240, 75),
                BackColor = Color.FromArgb(80, 0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                
            };

            Label modeLabel = new Label()
            {
                Text = modeName,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent,
                Location = new Point(10, 5),
                Size = new Size(220, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label scoreLabel = new Label()
            {
                Text = score > 0 ? $"{score} очков" : "Нет результата",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = score > 0 ? Color.White : Color.Gray,
                BackColor = Color.Transparent,
                Location = new Point(10, 30),
                Size = new Size(220, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            scorePanel.Controls.Add(modeLabel);
            scorePanel.Controls.Add(scoreLabel);
            parent.Controls.Add(scorePanel);

            yPos += 80;
        }

        // Вспомогательный метод для загрузки всех рекордов с режимами
        private List<ScoreRecord> LoadAllScores()
        {
            var scores = new List<ScoreRecord>();
            try
            {
                if (File.Exists("scores_with_modes.txt"))
                {
                    var lines = File.ReadAllLines("scores_with_modes.txt");
                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int score))
                        {
                            // Проверяем на дубликаты
                            if (!scores.Any(s => s.Score == score && s.Mode == parts[1]))
                            {
                                scores.Add(new ScoreRecord { Score = score, Mode = parts[1] });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки рекордов с режимами: {ex.Message}");
            }
            return scores;
        }

        // Вызовите этот метод в конструкторе после LoadHighScores()
        private void InitializeGameData()
        {
            LoadHighScores();
            RemoveDuplicateScores(); // Очищаем дубликаты при запуске
        }

        // Вспомогательный метод для получения режима счета
        // Вспомогательный метод для получения режима счета
        private string GetScoreMode(int score)
        {
            try
            {
                if (File.Exists("scores_with_modes.txt"))
                {
                    var lines = File.ReadAllLines("scores_with_modes.txt");
                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int savedScore) && savedScore == score)
                        {
                            return parts[1] switch
                            {
                                "Easy" => "Легкий",
                                "Medium" => "Средний",
                                "Hard" => "Сложный",
                                "Endless" => "Бесконечный",
                                _ => parts[1]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка определения режима: {ex.Message}");
            }
            return "Неизвестно";
        }

        // Класс для хранения рекордов с режимами
        public class ScoreRecord
        {
            public int Score { get; set; }
            public string Mode { get; set; } = "Unknown";
        }

        


        private void InitializeGame()
        {
            // Очистка предыдущих кнопок
            if (cellButtons != null)
            {
                for (int i = 0; i < BoardSize; i++)
                {
                    for (int j = 0; j < BoardSize; j++)
                    {
                        if (cellButtons[i, j] != null)
                        {
                            gamePanel.Controls.Remove(cellButtons[i, j]);
                            cellButtons[i, j].Dispose();
                        }
                    }
                }
            }

            gamePanel.Controls.Clear();

            cellButtons = new Button[BoardSize, BoardSize];
            board = new char[BoardSize, BoardSize];

            // Создание кнопок
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var button = new Button
                    {
                        Size = new Size(CellSize, CellSize),
                        Location = new Point(j * CellSize, i * CellSize),
                        Tag = new Point(j, i),
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat
                    };

                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = Color.Gray;

                    button.Click += CellButton_Click;
                    gamePanel.Controls.Add(button);
                    cellButtons[i, j] = button;
                }
            }

            // Заполнение доски и проверка на начальные совпадения
            do
            {
                FillBoardWithRandomSymbols();
            }
            while (CheckMatches()); // Повторяем пока есть начальные совпадения

            UpdateBoardDisplay();
        }

        private void FillBoardWithRandomSymbols()
        {
            char[] symbols = GetSymbolsForCurrentMode();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = symbols[random.Next(symbols.Length)];
                }
            }
        }

        private char[] GetSymbolsForCurrentMode()
        {
            return currentGameMode == GameMode.Hard ?
                new char[] { 'A', 'B', 'C', 'D', 'E', 'F' } :
                new char[] { 'A', 'B', 'C', 'D', 'E' };
        }
        private void BlockGameBoard()
        {
            // Создаем полупрозрачную панель поверх игрового поля
            Panel blockPanel = new Panel();
            blockPanel.Name = "GameBlockPanel"; // Даем имя чтобы потом можно было найти
            blockPanel.Location = gamePanel.Location;
            blockPanel.Size = gamePanel.Size;
            blockPanel.BackColor = Color.FromArgb(150, 0, 0, 0); // Полупрозрачный черный
            blockPanel.Cursor = Cursors.No; // Курсор "недоступно"

            // Добавляем текст
            Label messageLabel = new Label()
            {
                Text = "ИГРА ЗАВЕРШЕНА\nНажмите 'Новая игра'",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(70, 200),
                Size = new Size(350, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            blockPanel.Controls.Add(messageLabel);
            this.Controls.Add(blockPanel);
            blockPanel.BringToFront(); // Поверх всех элементов
        }

        private async void CellButton_Click(object sender, EventArgs e)
        {
            if (isAnimating) return;
            if (currentGameMode != GameMode.Endless && timeLeft <= 0) return;

            var button = (Button)sender;
            var cell = (Point)button.Tag;

            if (selectedCell.X == -1)
            {
                // Первое нажатие - выделяем клетку
                selectedCell = cell;
                button.BackColor = Color.LightBlue;
            }
            else
            {
                // Снимаем выделение с предыдущей клетки
                cellButtons[selectedCell.Y, selectedCell.X].BackColor = Color.Transparent;
                // Второе нажатие
                if (selectedCell.X == cell.X && selectedCell.Y == cell.Y)
                {
                    // Нажали на ту же клетку - отменяем выделение
                    //button.BackColor = GetColor(board[cell.Y, cell.X]);

                    selectedCell = new Point(-1, -1);
                    return;
                }

                // Снимаем выделение с предыдущей
                

                if (IsValidMove(selectedCell.X, selectedCell.Y, cell.X, cell.Y))
                {
                    // Пробуем сделать ход
                    SwapCells(selectedCell.X, selectedCell.Y, cell.X, cell.Y);
                    UpdateBoardDisplay();

                    if (CheckMatches())
                    {
                        // Ход валидный - обрабатываем совпадения
                        await ProcessMatches();
                    }
                    else
                    {
                        // Ход невалидный - возвращаем обратно
                        SwapCells(selectedCell.X, selectedCell.Y, cell.X, cell.Y);
                        UpdateBoardDisplay();
                        ShowBeautifulMessage("НЕВЕРНЫЙ ХОД! ❌", "Этот ход не создает комбинацию!\nПопробуйте другой ход.", "⚠️");
                    }
                }
                else
                {
                    ShowBeautifulMessage("НЕВЕРНЫЙ ХОД! ❌", "Неверный ход! Можно менять только\nсоседние клетки по горизонтали или вертикали.", "🚫");
                }
                button.BackColor = Color.Transparent;
                selectedCell = new Point(-1, -1);
            }
        }

        private bool IsValidMove(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        private void SwapCells(int x1, int y1, int x2, int y2)
        {
            char temp = board[y1, x1];
            board[y1, x1] = board[y2, x2];
            board[y2, x2] = temp;
        }

        private bool CheckMatches()
        {
            // Проверка горизонтальных совпадений
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize - 2; j++)
                {
                    if (board[i, j] != ' ' &&
                        board[i, j] == board[i, j + 1] &&
                        board[i, j] == board[i, j + 2])
                    {
                        return true;
                    }
                }
            }

            // Проверка вертикальных совпадений
            for (int i = 0; i < BoardSize - 2; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] != ' ' &&
                        board[i, j] == board[i + 1, j] &&
                        board[i, j] == board[i + 2, j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task ProcessMatches()
        {
            isAnimating = true;

            // Обрабатываем все совпадения пока они есть
            while (CheckMatches())
            {
                PlayRandomMatchSound();
                await AnimateMatchesDisappear();
                RemoveMatches();
                await AnimateFallAndFill();
            }

            isAnimating = false;
        }

        private async Task AnimateMatchesDisappear()
        {
            var matches = FindMatches();
            var tasks = new List<Task>();

            foreach (var point in matches)
            {
                tasks.Add(AnimateCellDisappear(point.Y, point.X));
            }

            await Task.WhenAll(tasks);
        }

        private async Task AnimateCellDisappear(int row, int col)
        {
            Button button = cellButtons[row, col];
            if (button == null) return;

            for (int size = CellSize; size >= 0; size -= 10)
            {
                if (button == null || button.IsDisposed) break;

                button.Size = new Size(size, size);
                button.Location = new Point(
                    col * CellSize + (CellSize - size) / 2,
                    row * CellSize + (CellSize - size) / 2
                );
                await Task.Delay(4);
            }

            if (button != null && !button.IsDisposed)
            {
                button.Visible = false;
            }
        }

        private async Task AnimateFallAndFill()
        {
            bool hasFallingElements = false;

            // Обработка падения символов
            for (int col = 0; col < BoardSize; col++)
            {
                int emptySpaces = 0;
                for (int row = BoardSize - 1; row >= 0; row--)
                {
                    if (board[row, col] == ' ')
                    {
                        emptySpaces++;
                    }
                    else if (emptySpaces > 0)
                    {
                        int newRow = row + emptySpaces;
                        board[newRow, col] = board[row, col];
                        board[row, col] = ' ';
                        await AnimateCellFall(row, col, newRow, col);
                        hasFallingElements = true;
                    }
                }
            }

            if (hasFallingElements)
            {
                await Task.Delay(30);
            }

            // Заполнение пустых мест новыми символами
            char[] symbols = GetSymbolsForCurrentMode();
            for (int col = 0; col < BoardSize; col++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    if (board[row, col] == ' ')
                    {
                        board[row, col] = symbols[random.Next(symbols.Length)];
                        await AnimateNewCellAppear(row, col);
                    }
                }
            }

            UpdateBoardDisplay();
        }

        private async Task AnimateCellFall(int fromRow, int fromCol, int toRow, int toCol)
        {
            Button button = cellButtons[fromRow, fromCol];
            if (button == null || button.IsDisposed) return;

            // Обновляем ссылки в массиве кнопок
            cellButtons[toRow, toCol] = button;
            cellButtons[fromRow, fromCol] = null;
            button.Tag = new Point(toCol, toRow);

            int startY = fromRow * CellSize;
            int targetY = toRow * CellSize;

            int steps = 5;
            for (int i = 1; i <= steps; i++)
            {
                if (button == null || button.IsDisposed) break;

                int newY = startY + (targetY - startY) * i / steps;
                button.Location = new Point(fromCol * CellSize, newY);
                await Task.Delay(AnimationDuration / steps);
            }

            if (button != null && !button.IsDisposed)
            {
                button.Location = new Point(fromCol * CellSize, targetY);
            }
        }

        private async Task AnimateNewCellAppear(int row, int col)
        {
            var button = new Button
            {
                Size = new Size(0, 0),
                Location = new Point(col * CellSize + CellSize / 2, -CellSize),
                Tag = new Point(col, row),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.Transparent, // Прозрачный фон
                ForeColor = Color.Black,
                Text = "", // Без текста
                FlatStyle = FlatStyle.Flat
            };

            // Устанавливаем картинку
            char symbol = board[row, col];
            if (symbolImages.ContainsKey(symbol))
            {
                button.BackgroundImage = symbolImages[symbol];
                button.BackgroundImageLayout = ImageLayout.Stretch;
            }

            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.Gray;
            button.Click += CellButton_Click;

            gamePanel.Controls.Add(button);
            cellButtons[row, col] = button;

           
            int targetY = row * CellSize;
            int steps = 5;

            for (int i = 1; i <= steps; i++)
            {
                if (button == null || button.IsDisposed) break;

                int newY = -CellSize + (targetY + CellSize) * i / steps;
                int newSize = CellSize * i / steps;
                button.Size = new Size(newSize, newSize);
                button.Location = new Point(col * CellSize + (CellSize - newSize) / 2, newY);
                await Task.Delay(AnimationDuration / steps);
            }

            if (button != null && !button.IsDisposed)
            {
                button.Size = new Size(CellSize, CellSize);
                button.Location = new Point(col * CellSize, targetY);
            }
        }

        private List<Point> FindMatches()
        {
            var matches = new List<Point>();
            bool[,] toRemove = new bool[BoardSize, BoardSize];

            // Горизонтальные совпадения
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize - 2; j++)
                {
                    if (board[i, j] != ' ' &&
                        board[i, j] == board[i, j + 1] &&
                        board[i, j] == board[i, j + 2])
                    {
                        toRemove[i, j] = true;
                        toRemove[i, j + 1] = true;
                        toRemove[i, j + 2] = true;
                    }
                }
            }

            // Вертикальные совпадения
            for (int i = 0; i < BoardSize - 2; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] != ' ' &&
                        board[i, j] == board[i + 1, j] &&
                        board[i, j] == board[i + 2, j])
                    {
                        toRemove[i, j] = true;
                        toRemove[i + 1, j] = true;
                        toRemove[i + 2, j] = true;
                    }
                }
            }

            // Собираем все клетки для удаления
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (toRemove[i, j])
                    {
                        matches.Add(new Point(j, i));
                    }
                }
            }

            return matches;
        }

        private void RemoveMatches()
        {
            var matches = FindMatches();
            int removedCount = 0;

            foreach (var point in matches)
            {
                var button = cellButtons[point.Y, point.X];
                if (button != null)
                {
                    gamePanel.Controls.Remove(button);
                    button.Dispose();
                    cellButtons[point.Y, point.X] = null;
                }

                board[point.Y, point.X] = ' ';
                removedCount++;
            }

            if (removedCount > 0)
            {
                score += removedCount * 10;
                UpdateScoreDisplay();
            }
        }

        private void UpdateBoardDisplay()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var button = cellButtons[i, j];
                    if (button != null && !button.IsDisposed)
                    {
                        char symbol = board[i, j];

                        // Убираем текст
                        button.Text = "";

                        // Устанавливаем картинку
                        if (symbolImages.ContainsKey(symbol))
                        {
                            button.BackgroundImage = symbolImages[symbol];
                            button.BackgroundImageLayout = ImageLayout.Stretch;
                        }

                        // Убираем цвет фона, так как теперь есть картинка
                        button.BackColor = Color.Transparent;
                        button.FlatAppearance.BorderColor = Color.Gray;
                        button.Size = new Size(CellSize, CellSize);
                        button.Location = new Point(j * CellSize, i * CellSize);
                        button.Visible = true;
                    }
                }
            }
        }

        private Color GetColor(char symbol)
        {
            return symbol switch
            {
                'A' => Color.LightCoral,
                'B' => Color.LightGreen,
                'C' => Color.LightBlue,
                'D' => Color.LightYellow,
                'E' => Color.Plum,
                'F' => Color.Violet,
                _ => Color.White
            };
        }

        private void UpdateScoreDisplay()
        {
            lblScore.Text = $"Счет: {score}";

            if (currentGameMode != GameMode.Endless && targetScore > 0)
            {
                int progress = (int)((double)score / targetScore * 100);
                progressBar.Value = Math.Min(progress, 100);
                progressBar.Visible = true;  // Показываем прогресс-бар для режимов с целью
            }
            else
            {
                progressBar.Value = 0;
                progressBar.Visible = false;  // Скрываем прогресс-бар для бесконечного режима
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            if (isAnimating) return;
            // Проверяем, есть ли блокирующая панель (игра завершена)
            bool isGameBlocked = this.Controls.OfType<Panel>().Any(p => p.Name == "GameBlockPanel");

            if (isGameBlocked)
            {
                // Игра уже завершена - сразу начинаем новую без подтверждения
                ShowStartScreen();
            }
            else
            {
                // Игра активна - показываем подтверждение
                ShowNewGameConfirmation();
            }

            //if (score > 0)
            //{
            //    AddHighScore(score);
            //}

            //gameTimer.Stop();
            //ShowNewGameConfirmation();
            //ShowStartScreen();
        }

        private void highScoresButton_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            ShowHighScores();

        }

        private void picTrophy_Click(object sender, EventArgs e)
        {
            PlayButtonClickSound();
            ShowHighScores();
        }

        private void picEasy_MouseEnter(object sender, EventArgs e)
        {
            picEasy.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\easy_hover.png");
            PlayButtonHoverSound();
        }

        private void picEasy_MouseLeave(object sender, EventArgs e)
        {
            picEasy.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\easy.png");
        }

        private void picMedium_MouseEnter(object sender, EventArgs e)
        {
            picMedium.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\medium_hover.png");
            PlayButtonHoverSound();
        }

        private void picMedium_MouseLeave(object sender, EventArgs e)
        {
            picMedium.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\medium.png");
        }

        private void picHard_MouseEnter(object sender, EventArgs e)
        {
            picHard.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\hard_hover.png");
            PlayButtonHoverSound();
        }

        private void picHard_MouseLeave(object sender, EventArgs e)
        {
            picHard.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\hard.png");
        }

        private void picEndless_MouseEnter(object sender, EventArgs e)
        {
            picEndless.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\endless_hover.png");
            PlayButtonHoverSound();
        }

        private void picEndless_MouseLeave(object sender, EventArgs e)
        {
            picEndless.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\endless.png");
        }

        private void picExit_MouseEnter(object sender, EventArgs e)
        {
            picExit.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\exit_hover.png");
            PlayButtonHoverSound();
        }

        private void picExit_MouseLeave(object sender, EventArgs e)
        {
            picExit.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\exit.png");
        }

        private void picNewGame_MouseEnter(object sender, EventArgs e)
        {
            picNewGame.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\newgameingame_hover.png");
            PlayButtonHoverSound();
        }

        private void picNewGame_MouseLeave(object sender, EventArgs e)
        {
            picNewGame.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\newgameingame.png");
        }

        private void picHighScores_MouseEnter(object sender, EventArgs e)
        {
            picHighScores.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\recordsingame_hover.png");
            PlayButtonHoverSound();
        }

        private void picHighScores_MouseLeave(object sender, EventArgs e)
        {
            picHighScores.Image = Image.FromFile("D:\\visualstusio_projects\\minikyrsovaya\\png_images\\recordsingame.png");
        }

     

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (score > 0)
            {
                AddHighScore(score);
            }
            gameTimer.Stop();
            StopBackgroundMusic();
            SaveVolumeSettings();

            // Освобождаем ресурсы NAudio
            backgroundMusicPlayer?.Dispose();
            backgroundMusicFile?.Dispose();
            base.OnFormClosing(e);
        }

      
    }
}