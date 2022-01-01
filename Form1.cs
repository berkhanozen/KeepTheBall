using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CollidingBalls
{
    public partial class Form1 : Form
    {
        Ball ball;
        Ball addedBalls;
        public static List<Ball> balls = new List<Ball>();
        RootObject<BallModel> backupGameInfo;
        Stick stick;
        Timer timerAddBall;
        Timer timerUpdate;
        public int borderWidth;
        public int uppersWidth;
        public int upperRightLocation;
        bool isGamePaused = false;
        public static int score = 0;
        public static int counter;
        public static int ballInTheScreen = 0;
        public static DialogResult result;
        Save save; //Periyodik olarak JSON'a save yapan timer'ı silip, Task kullanarak periyodik save yapan bir Save class'ı oluşturdum.
        //Form1_Load fonksiyonunun en altında çağırdım.

        public Form1()
        {
            InitializeComponent();
            borderWidth = 25;
            uppersWidth = this.Width/3;
            upperRightLocation = this.Width - uppersWidth;
            stick = new Stick(this.Width, this.Height);

            if (File.Exists("gp_yedek.json"))
            {
                ReadData();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(File.Exists("gp_yedek.json"))
            {
                const string message = "Kaydedilen yerden başlatılsın mı?";
                const string caption = "Bir karar verin.";
                result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                string decryptedScore = Cryptography.Decrypt(backupGameInfo.score, "berkhan");
                string decryptedCounter = Cryptography.Decrypt(backupGameInfo.counter, "berkhan");
                score = Int32.Parse(decryptedScore);
                counter = Int32.Parse(decryptedCounter);
                ballInTheScreen += backupGameInfo.balls.Length;
                for (int backupBallCount = 0; backupBallCount < backupGameInfo.balls.Length; backupBallCount++)
                {
                    ball = new Ball(this.Width, this.Height);
                    balls.Add(ball);
                    balls[backupBallCount].brush = backupGameInfo.balls[backupBallCount].brush;
                    balls[backupBallCount].location.X = backupGameInfo.balls[backupBallCount].location.X;
                    balls[backupBallCount].location.Y = backupGameInfo.balls[backupBallCount].location.Y;
                    balls[backupBallCount].direction.X = backupGameInfo.balls[backupBallCount].direction.X;
                    balls[backupBallCount].direction.Y = backupGameInfo.balls[backupBallCount].direction.Y;
                    if (backupGameInfo.balls[backupBallCount].isBallOutFromBottomForBackup == true || backupGameInfo.balls[backupBallCount].isBallOutFromTop == true)
                    {
                        ballInTheScreen--;
                    }
                }
            }
            else if (result == DialogResult.No)
            {
                counter = 0;
            }

            this.WindowState = FormWindowState.Normal;
            this.DoubleBuffered = true;
            timerUpdate = new Timer();
            timerUpdate.Tick += TimerUpdate_tick;
            timerUpdate.Interval = 1;
            timerUpdate.Start();

            timerAddBall = new Timer();
            timerAddBall.Tick += TimerAddBall_tick;
            timerAddBall.Interval = 1000;
            timerAddBall.Start();

            save = new Save(5); //her 5 saniyede 1 save çalışacak.
            save.AutoSave();
        }

        private void TimerUpdate_tick(object sender, EventArgs e)
        {
            label_score.Text = "Score: " + score.ToString();

            if (ballInTheScreen >= 10 || ballInTheScreen <= 0 && timerAddBall.Enabled == false)
            {
                timerUpdate.Stop();
                timerUpdate.Enabled = false;
                if (ballInTheScreen >= 10)
                    MessageBox.Show("Kaybettin");
                if(ballInTheScreen <= 0 && timerAddBall.Enabled == false)
                    MessageBox.Show("Kazandın");
            }
            Invalidate();
        }

        private void TimerAddBall_tick(object sender, EventArgs e)
        {
            if(counter <= 0)
            {
                ball = new Ball(this.Width, this.Height);
                balls.Add(ball);
                ballInTheScreen++;
                counter = 10;

                if(balls.Count == 1)
                {
                    Backup();
                }

                if (balls.Count > 4)
                {
                    timerAddBall.Stop();
                    timerAddBall.Enabled = false;
                }
            }
            counter--;
        }

        private void DrawObjects(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, borderWidth, this.Height); //sınırlar
            e.Graphics.FillRectangle(Brushes.Black, this.Width - borderWidth - 16, 0, borderWidth, this.Height);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, uppersWidth, borderWidth);
            e.Graphics.FillRectangle(Brushes.Black, this.Width - uppersWidth, 0, uppersWidth, borderWidth);

            stick.Draw(e.Graphics);
            stick.Update();

            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].Draw(e.Graphics);
                balls[i].Update();
                balls[i].CollideControl(stick);

                if (balls[i].isBallOutFromBottomControl)
                {
                    for(int j=0; j<2; j++)
                    {
                        addedBalls = new Ball(this.Width, this.Height);
                        balls.Add(addedBalls);
                        ballInTheScreen++;
                    }
                    balls[i].isBallOutFromBottomControl = false;
                }
            }
        }

        private void Controller(object sender, KeyEventArgs e)
        {
            if(e.KeyCode.Equals(Keys.Escape))
            {
                if (isGamePaused)
                {
                    Play();
                }
                else
                {
                    Pause();
                }
            }
            if(e.KeyCode.Equals(Keys.S))
            {
                Backup();
            }
            if(e.KeyCode.Equals(Keys.R))
            {
                Restore();
            }
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void backupBtn_Click(object sender, EventArgs e)
        {
            Backup();
        }

        private void restoreBtn_Click(object sender, EventArgs e)
        {
            Restore();
        }
        public void Play()
        {
            timerUpdate.Start();
            isGamePaused = false;
        }
        public void Pause()
        {
            timerUpdate.Stop();
            isGamePaused = true;
        }
        public void ReadData()
        {
            backupGameInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject<BallModel>>(File.ReadAllText("gp_yedek.json"));
        }
        public static void Backup()
        {
            string fileName = "gp_yedek.json";
            string score = Cryptography.Encrypt(Form1.score.ToString(), "berkhan");
            string counter = Cryptography.Encrypt(Form1.counter.ToString(), "berkhan");
            string gameInfo = Newtonsoft.Json.JsonConvert.SerializeObject(new { balls, score, counter });
            File.WriteAllText(fileName, gameInfo);
        }
        public void Restore()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Exit();
        }
    }

    public class Save
    {
        int saveInterval;
        public Save(int interval)
        {
            saveInterval = interval * 1000;
        }
        public async void AutoSave()
        {
            while (true)
            {
                await Task.Delay(saveInterval);
                Form1.Backup();
            }
        }
    }

    public class Stick
    {
        public int stickWidth = 200;
        public int stickHeight = 25;
        int formHeight;
        public Vector2 location = new Vector2();

        public Stick(int width, int height)
        {
            formHeight = height;
            location = new Vector2(width / 2 - stickWidth / 2, formHeight - (stickHeight + 50));
        }
        public void Draw(Graphics e)
        {
            e.FillRectangle(Brushes.Red, location.X, location.Y, stickWidth, stickHeight);
        }
        public void Update()
        {
            location = new Vector2(Cursor.Position.X - stickWidth, formHeight - (stickHeight + 50));
        }
    }

    public class Ball
    {
        public Vector2 location = new Vector2();
        public Vector2 direction = new Vector2();
        double velocity = 8;
        double velocityX;
        int formWidth;
        int ballWidth = 60;
        int ballHeight = 60;
        Color randomColor;
        public SolidBrush brush;
        Form1 form1 = new Form1();
        public bool isBallOutFromBottomControl = false;
        public bool isBallOutFromBottomForBackup = false;
        public bool isBallOutFromTop = false;

        public Ball(int width, int height)
        {
            Random rand = new Random();
            formWidth = width;
            location = new Vector2(rand.Next(ballWidth/2, width/2), rand.Next(ballHeight/2, height/2));

            velocityX = rand.NextDouble() * (velocity*2) - velocity; //Tek bir vektör, bileşik hızdan uzun olamaz. X vektörünün hız aralığı -velocity, velocity aralığında.
            direction = new Vector2((float)velocityX, CalculateVelocity((float)velocityX, (float)velocity)); //Her topun hızı eşit olacağı için X leri rastgele alıyorum. Tek vektörü ve hızı aldıktan sonra diğer vektörü hesaplıyorum.

            randomColor = Color.FromArgb(rand.Next(byte.MaxValue), rand.Next(byte.MaxValue), rand.Next(byte.MaxValue));
            brush = new SolidBrush(randomColor);
    }

        public void Update()
        {
            location = Vector2.Add(location, direction);
        }

        public void Draw(Graphics e)
        {
            e.FillEllipse(brush, location.X, location.Y, ballWidth, ballHeight);
        }

        public void CollideControl(Stick stick)
        {
            if (location.X < form1.borderWidth || location.X + ballWidth > formWidth - form1.borderWidth - 16)
            {
                direction.X = -direction.X;
            }

            if ((location.X + ballWidth / 2 < form1.uppersWidth && location.Y < form1.borderWidth) ||
                (location.X + ballWidth / 2 > form1.upperRightLocation && location.Y < form1.borderWidth))
            {
                direction.Y = -direction.Y;
            }

            if(location.X + ballWidth / 2 < stick.location.X + stick.stickWidth && location.X + ballWidth / 2 > stick.location.X && location.Y + ballWidth > stick.location.Y && location.Y + ballWidth < stick.location.Y + stick.stickHeight)
            {
                direction.Y = -direction.Y;
                Form1.score++;
            }

            if (location.Y <= -2*ballWidth && (direction.X > 0 || direction.Y > 0))
            {
                isBallOutFromTop = true;
                Form1.ballInTheScreen--;
                direction = new Vector2(0, 0);
                Form1.score += 10;
            }

            if (location.Y + ballWidth > stick.location.Y + stick.stickHeight + 2*ballWidth && (direction.X > 0 || direction.Y > 0))
            {
                isBallOutFromBottomControl = true;
                isBallOutFromBottomForBackup = true;
                Form1.ballInTheScreen--;
                direction = new Vector2(0, 0);
                Form1.score -= 20;
            }
        }

        public float CalculateVelocity(float x, float velocity)
        {
            float y = (float)Math.Sqrt(velocity * velocity - x * x);
            return y;
        }
    }

    public static class Cryptography
    {
        #region Settings

        private static int _iterations = 2;
        private static int _keySize = 256;

        private static string _hash = "SHA1";
        private static string _salt = "aselrias38490a32"; // Random
        private static string _vector = "8947az34awl34kjq"; // Random

        #endregion

        public static string Encrypt(string value, string password)
        {
            return Encrypt<AesManaged>(value, password);
        }
        public static string Encrypt<T>(string value, string password)
                where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);

            byte[] encrypted;
            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes =
                    new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string value, string password)
        {
            return Decrypt<AesManaged>(value, password);
        }
        public static string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream from = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }
    }

    public class Location
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class Direction
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class Brush
    {
        public string Color { get; set; }
    }

    public class BallModel
    {
        public Location location { get; set; }
        public Direction direction { get; set; }
        public SolidBrush brush { get; set; }
        public bool isBallOutFromBottomForBackup { get; set; }
        public bool isBallOutFromTop { get; set; }
    }

    public class RootObject<T>
    {
        public string score { get; set; }
        public string counter { get; set; }
        public T[] balls { get; set; }
    }
}