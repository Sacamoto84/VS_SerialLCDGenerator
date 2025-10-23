using BitmapProcessing; //our assembly
using ComponentFactory.Krypton.Toolkit;
using Iocomp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialLCD
{
    public struct _contur
    {
        public short x1;
        public short y1;
        public short x2;
        public short y2;
        public short w;
        public short h;
        public bool enable;
        public bool visible;
        public byte state;
    }

    public partial class Form1 : Form
    {

        private Task Proceso1;
        private Task tSendToSTM32;
        private Task tSendToESP32;
        private Task tSendToESP32Udp;


        const int scale = 8;
        const int GREEN = 0x07E0;

        const byte NONE = 0;
        const byte VLINE = 1;
        const byte HLINE = 2;
        byte ModeLine;

        public ushort[,] frameBufferLayer_Contur = new ushort[128, 64];
        public ushort[,] frameBufferLayer_Mouse_Cursor = new ushort[128, 64];
        public ushort[,] fbMeasure = new ushort[128, 64]; //Рамка


        public ushort[,] fbMain = new ushort[128, 64];

        public ushort[,] frameBufferRender = new ushort[128, 64];

        private LifoBuffer lifoBuffer = new LifoBuffer(128, 64); // Новый буфер

        public _contur contur;

        byte LCD_W = 128;
        byte LCD_H = 64;

        Bitmap newBmp = new Bitmap(128 * scale, 64 * scale);

        Bitmap newBmpMini = new Bitmap(128, 64);

        int mouse_x, mouse_y;

        int window_H = 16, window_W = 16, window_Visible = 0;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private bool isUDPSelect = true;

        private DateTime _lastConnectionCheck = DateTime.MinValue; // Время последней проверки соединения
        
   
        /// <summary>
        /// Валидация IP-адреса
        /// </summary>
        private bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            string[] parts = ip.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0 || num > 255)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Сохранение IP-адреса в настройки
        /// </summary>
        private void SaveIPAddress(string ip)
        {
            try
            {
                if (!IsValidIPAddress(ip))
                {
                    throw new ArgumentException("Некорректный IP-адрес");
                }
                Properties.Settings.Default.LastIPAddress = ip;
                Properties.Settings.Default.Save();
                Console.Beep(1000, 100); // Звуковой сигнал, как вы просили ранее
            }
            catch (Exception ex)
            {
                // Логируем полную информацию об ошибке
                Console.WriteLine($"Ошибка при сохранении IP-адреса: {ex}");
                // Показываем сообщение пользователю (опционально)
                MessageBox.Show($"Не удалось сохранить IP-адрес: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private const string DefaultIPAddress = "192.168.0.100"; // Константа для IP по умолчанию

        private void LoadIPAddress()
        {
            try
            {
                string savedIP = Properties.Settings.Default.LastIPAddress;
                tbIpClient.Text = (!string.IsNullOrEmpty(savedIP) && IsValidIPAddress(savedIP))
                    ? savedIP
                    : DefaultIPAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке IP-адреса: {ex}");
                if (tbIpClient.InvokeRequired)
                {
                    // Используем Action для явного указания типа делегата
                    tbIpClient.Invoke(new Action(() => tbIpClient.Text = DefaultIPAddress));
                }
                else
                {
                    tbIpClient.Text = DefaultIPAddress;
                }
                // Опционально: уведомление через MessageBox
                MessageBox.Show($"Не удалось загрузить IP-адрес: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void lifoPush()
        {
            lifoBuffer.Push(fbMain);
            int capacity = lifoBuffer.Count;
            bUndo.Text = "Undo: " + capacity;
        }

        void lifoPop()
        {
            ushort[,] temp = new ushort[128, 64];
            bool res = lifoBuffer.Pop(temp);
            if (res)
            {
                fbMain = temp;
            }
            int capacity = lifoBuffer.Count;
            bUndo.Text = "Undo: " + capacity;
        }

        void lifoClear()
        {
            lifoBuffer.Clear();
            int capacity = lifoBuffer.Count;
            bUndo.Text = "Undo: " + capacity;
        }


        #region Не менять
        public Form1()
        {
            InitializeComponent();

            pictureBox1.Image = newBmp;
            pictureBox1.Size = new Size(128 * scale, 64 * scale);

            this.Width = 128 * scale;
            this.Height = 64 * scale + panel2.Height + 48;

            Proceso1 = Task.Run(() => RenderAsync(cts.Token), cts.Token);
            tSendToSTM32 = Task.Run(() => SendToSTM32Async(cts.Token), cts.Token);
            tSendToESP32Udp = Task.Run(() => SendToESP32UdpAsync(cts.Token), cts.Token); ;

        contur = new _contur { x1 = 0, y1 = 0, x2 = 1, y2 = 1, w = 1, h = 1, enable = false, visible = false, state = 0 };

        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            // Получить список COM портов 
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                listBox1.Items.Add(port);
            }

            if (listBox1.Items.Count > 0) listBox1.SetSelected(0, true);
            
            // Загружаем сохраненный IP-адрес
            LoadIPAddress();
   
        }
        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            cts.Cancel();
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при закрытии порта: {ex.Message}");
                }
            }
            try
            {
                await Task.WhenAll(Proceso1, tSendToSTM32);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при завершении задач: {ex.Message}");
            }
            cts.Dispose();
            newBmp.Dispose();
            newBmpMini.Dispose();
        }

        private void OPEN_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return; //Количество елементов в списке
            serialPort1.PortName = listBox1.SelectedItem.ToString();

            serialPort1.Open();
            OPEN.Enabled = false;
            CLOSE.Enabled = true;

            bool _opened;
            _opened = serialPort1.IsOpen;
            if (_opened)
                label2.Text = "Открыт";
            else
                label2.Text = "Закрыт";
        }
        private void CLOSE_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            OPEN.Enabled = true;
            CLOSE.Enabled = false;

            bool _opened;
            _opened = serialPort1.IsOpen;
            if (_opened)
                label2.Text = "Открыт";
            else
                label2.Text = "Закрыт";
        }
        #endregion

        #region Утилиты setArray

        public void LineH(ushort[,] array, short y, short x, short w, ushort color)
        {
            for (ushort i = 0; i < w; i++)
                array[x + i, y] = color;
        }
        public void LineV(ushort[,] array, short x, short y, short h, ushort color)
        {
            for (ushort i = 0; i < h; i++)
                array[x, y + i] = color;
        }
        public void Rectagle(ushort[,] array, short x, short y, short w, short h, ushort color)
        {
            LineH(array, y, x, (short)(w), color);
            LineH(array, (short)(y + h - 1), x, (short)(w), color);

            LineV(array, x, y, (short)(h), color);
            LineV(array, (short)(x + w - 1), y, (short)(h), color);
        }
        private ushort RGB565(byte R, byte G, byte B)
        {
            ushort Cout;
            Cout = (ushort)((R >> 3) << 11);
            Cout |= (ushort)((G >> 2) << 5);
            Cout |= (ushort)(B >> 3);

            return Cout;
        }
        #endregion


        private void cbVidelenie_Click(object sender, EventArgs e)
        {
            if (cbVidelenie.Checked)
            {
                Array.Clear(frameBufferLayer_Contur, 0, frameBufferLayer_Contur.Length);
                contur.enable = true;
                Rectagle(frameBufferLayer_Contur, contur.x1, contur.y1, contur.w, contur.h, GREEN);
                bMassiv.Enabled = true;

            }
            else
            {
                contur.enable = false;
                bMassiv.Enabled = false;
            }
        }


        #region Кнопки
        private void bClear_Click(object sender, EventArgs e)
        {
            Array.Clear(fbMain, 0, fbMain.Length);
            //fbMainChanged = true; // Отмечаем изменение
        }
        private void bMassiv_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2(this);
            newForm.Show();
            newForm.CreateArray();
        }
        private void cbVLine_Click(object sender, EventArgs e)
        {
            if (cbVLine.Checked)
                ModeLine = VLINE;
            else
                ModeLine = NONE;
        }
        private void cbHLine_Click(object sender, EventArgs e)
        {
            if (cbHLine.Checked)
                ModeLine = HLINE;
            else
                ModeLine = NONE;
        }
        private void bMeasureClear_Click(object sender, EventArgs e) { window_Visible = 0; }
        private void bMeasureSet_Click(object sender, EventArgs e)
        {
            Array.Clear(fbMeasure, 0, fbMeasure.Length);
            window_H = edit_windowH.Value;
            window_W = edit_windowW.Value;
            window_Visible = 1;
        }
        #endregion

        #region Мышка
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mouse_x = e.Location.X / scale;
            mouse_y = e.Location.Y / scale;

            if (mouse_x < 0) mouse_x = 0;
            if (mouse_y < 0) mouse_y = 0;
            if (mouse_x > 127) mouse_x = 127;
            if (mouse_y > 63) mouse_y = 63;

            label3.Text = mouse_x.ToString() + " " + mouse_y.ToString();

            Array.Clear(frameBufferLayer_Mouse_Cursor, 0, frameBufferLayer_Mouse_Cursor.Length);
            frameBufferLayer_Mouse_Cursor[mouse_x, mouse_y] = 0xF81F;

            Array.Clear(frameBufferLayer_Contur, 0, frameBufferLayer_Contur.Length);
            frameBufferLayer_Contur[mouse_x, mouse_y] = 0x1456;

            if (contur.enable)
            {
                Rectagle(frameBufferLayer_Contur, contur.x1, contur.y1, contur.w, contur.h, GREEN);
                if (contur.state == 1)
                {
                    short x1, y1, x2, y2;

                    x1 = contur.x1;
                    y1 = contur.y1;

                    x2 = (short)mouse_x;
                    y2 = (short)mouse_y;

                    if (x1 > x2)
                    {
                        x2 = contur.x1;
                        x1 = (short)mouse_x;
                    }
                    if (y1 > y2)
                    {
                        y2 = contur.y1;
                        y1 = (short)mouse_y;
                    }

                    Rectagle(frameBufferLayer_Contur, x1, y1, (short)(x2 - x1 + 1), (short)(y2 - y1 + 1), GREEN);
                }
            }

            //if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            //    fbMainChanged = true; // Отмечаем изменение

            if ((ModeLine == VLINE) || (ModeLine == HLINE))
                return;

                if (e.Button == MouseButtons.Left)
            {
                fbMain[mouse_x, mouse_y] = 0xFFFF;
            }

            if (e.Button == MouseButtons.Right)
            {
                fbMain[mouse_x, mouse_y] = 0x0000;
            }

        
            //if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right || ModeLine != NONE)
            //    fbMainChanged = true; // Отмечаем изменение при рисовании линий или пикселей

        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            mouse_x = e.Location.X / scale;
            mouse_y = e.Location.Y / scale;

            if (contur.enable == true)
            {
                if (contur.state == 0)
                {
                    contur.state = 1;
                    contur.h = 1;
                    contur.w = 1;
                    contur.x1 = (short)mouse_x;
                    contur.y1 = (short)mouse_y;
                    contur.state = 1;
                    listBox2.Items.Clear();
                    listBox2.Items.Add("state: " + contur.state);
                    listBox2.Items.Add("x1: " + contur.x1);
                    listBox2.Items.Add("y1: " + contur.y1);
                    listBox2.Items.Add("x2: " + contur.x2);
                    listBox2.Items.Add("y2: " + contur.y2);
                    listBox2.Items.Add("h: " + contur.h);
                    listBox2.Items.Add("w: " + contur.w);
                    return;
                }

                if (contur.state == 1)
                {
                    contur.state = 0;

                    contur.x2 = (short)mouse_x;
                    contur.y2 = (short)mouse_y;

                    if (contur.x1 > contur.x2)
                    {
                        contur.x2 = contur.x1;
                        contur.x1 = (short)mouse_x;
                    }
                    if (contur.y1 > contur.y2)
                    {
                        contur.y2 = contur.y1;
                        contur.y1 = (short)mouse_y;
                    }

                    contur.w = (short)(contur.x2 - contur.x1 + 1);
                    contur.h = (short)(contur.y2 - contur.y1 + 1);

                    Rectagle(frameBufferLayer_Contur, contur.x1, contur.y1, contur.w, contur.h, GREEN);

                    listBox2.Items.Clear();
                    listBox2.Items.Add("state: " + contur.state);
                    listBox2.Items.Add("x1: " + contur.x1);
                    listBox2.Items.Add("y1: " + contur.y1);
                    listBox2.Items.Add("x2: " + contur.x2);
                    listBox2.Items.Add("y2: " + contur.y2);
                    listBox2.Items.Add("h: " + contur.h);
                    listBox2.Items.Add("w: " + contur.w);
                    return;
                }
            }
            else
            {
                label4.Text = mouse_x.ToString() + " " + mouse_y.ToString();

                if (ModeLine == VLINE)
                {
                if (e.Button == MouseButtons.Left)
                {
                    lifoPush();
                    LineV(fbMain, (short)mouse_x, 0, 64, 0xFFFF);
                    return;
                }
                if (e.Button == MouseButtons.Right)
                {
                    lifoPush();
                    LineV(fbMain, (short)mouse_x, 0, 64, 0);
                    return;
                }
                }

                if (ModeLine == HLINE)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        lifoPush();
                        LineH(fbMain, (short)mouse_y, 0, 128, 0xFFFF);
                        return;
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        lifoPush();
                        LineH(fbMain, (short)mouse_y, 0, 128, 0);
                        return;
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    lifoPush();
                    fbMain[mouse_x, mouse_y] = 0xFFFF;
                    return;
                }

                if (e.Button == MouseButtons.Right)
                {
                    lifoPush();
                    fbMain[mouse_x, mouse_y] = 0;
                    return;
                }

                if (e.Button == MouseButtons.Middle)
                {
                    lifoPop();
                    return;
                }

              
            }
        }
        #endregion

        #region Сохранение и загрузка
        private void bSave_Click(object sender, EventArgs e)
        {
            //Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "|*.dat";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;

                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
                {
                    writer.Write(serial_transmit_buffer, 0, 1024);
                }
            }



        }

        private void bFastSave_Click(object sender, EventArgs e)
        {
            string path = @"fast.dat";
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                writer.Write(serial_transmit_buffer, 0, 1024);
            }
        }

        private void bFastLoad_Click(object sender, EventArgs e)
        {
            lifoPush();

            string path = @"fast.dat";

            if (File.Exists(path))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    ushort pix;
                    byte[] byteBuffer = new byte[1024];
                    byteBuffer = reader.ReadBytes(1024);

                    for (ushort y = 0; y < 64; y++)
                    {
                        for (ushort x = 0; x < 128; x++)
                        {
                            pix = 0;

                            pix = byteBuffer[x + (y / 8) * LCD_W];
                            pix &= (ushort)(1 << (y & 7));

                            if (pix > 0)
                            {
                                fbMain[x, y] = 0xFFFF;
                            }
                            else
                            {
                                fbMain[x, y] = 0;
                            }

                        }
                    }
                }
            }
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            lifoPush();

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "";
                openFileDialog.Filter = "|*.dat";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                    {
                        ushort pix;
                        byte[] byteBuffer = new byte[1024];
                        byteBuffer = reader.ReadBytes(1024);

                        for (ushort y = 0; y < 64; y++)
                        {
                            for (ushort x = 0; x < 128; x++)
                            {
                                pix = 0;

                                pix = byteBuffer[x + (y / 8) * LCD_W];
                                pix &= (ushort)(1 << (y & 7));

                                if (pix > 0)
                                {
                                    fbMain[x, y] = 0xFFFF;
                                }
                                else
                                {
                                    fbMain[x, y] = 0;
                                }

                            }
                        }
                    }
                }
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            listBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                listBox1.Items.Add(port);
            }
            if (listBox1.Items.Count > 0) listBox1.SetSelected(0, true);
        }

        private void bUndo_Click(object sender, EventArgs e)
        {
            lifoPop();
            int capacity = lifoBuffer.Count;
            bUndo.Text = "Undo: "+capacity;
        }

        private void bUndoClear_Click(object sender, EventArgs e)
        {
            lifoClear();
        }

        #endregion

        #region Потоки отрисовки и отсылки
        byte[] serial_transmit_buffer = new byte[1024];


        private void bFill_Click(object sender, EventArgs e)
        {
            // Заполнение массива
            for (int i = 0; i < fbMain.GetLength(0); i++)
            {
                for (int j = 0; j < fbMain.GetLength(1); j++)
                {
                    fbMain[i, j] = 0xFFFF;
                }
            }
        }

        private void tbIpClient_TextChanged(object sender, EventArgs e)
        {
            String values = tbIpClient.Text;
            if (!IsValidIPAddress(values)) return;
            SaveIPAddress(values);
            Console.Beep(); // Воспроизводит звуковой сигнал
        }

        private async Task RenderAsync(CancellationToken token)
        {
            int i = 0;
            Bitmap newBmpLocal = new Bitmap(128 * scale, 64 * scale);
            Graphics graphicsTarget = Graphics.FromImage(newBmp);
            SolidBrush brush = new SolidBrush(Color.Black);
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    stopwatch.Start();
                    i++;
                    ushort x, y;
                    ushort pix;
                    for (x = 0; x < LCD_W; x++) fbMeasure[x, window_H] = 0xF81F;
                    for (y = 0; y < LCD_H; y++) fbMeasure[window_W, y] = 0xF81F;
                    for (x = 0; x < LCD_W; x++)
                        for (y = 0; y < LCD_H; y++)
                        {
                            frameBufferRender[x, y] = fbMain[x, y];
                            if (window_Visible == 1) frameBufferRender[x, y] |= fbMeasure[x, y];
                            if (contur.enable)
                            {
                                if (frameBufferLayer_Contur[x, y] == GREEN)
                                {
                                    if (frameBufferRender[x, y] == 0) frameBufferRender[x, y] = 0x654C;
                                    else frameBufferRender[x, y] = 0x8F51;
                                }
                            }
                            if (frameBufferLayer_Mouse_Cursor[x, y] > 0)
                                frameBufferRender[x, y] = frameBufferLayer_Mouse_Cursor[x, y];
                        }
                    try
                    {
                        System.Drawing.Imaging.BitmapData bmpData = newBmpLocal.LockBits(
                            new Rectangle(0, 0, newBmpLocal.Width, newBmpLocal.Height),
                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        unsafe
                        {
                            byte* ptr = (byte*)bmpData.Scan0;
                            for (y = 0; y < LCD_H; y++)
                            {
                                for (x = 0; x < LCD_W; x++)
                                {
                                    pix = frameBufferRender[x, y];
                                    int r = ((pix & 0xF800) >> 11) << 3;
                                    int g = ((pix & 0x07E0) >> 5) << 2;
                                    int b = (pix & 0x001F) << 3;
                                    for (int sy = 0; sy < scale; sy++)
                                    {
                                        for (int sx = 0; sx < scale; sx++)
                                        {
                                            int offset = ((y * scale + sy) * bmpData.Stride) + ((x * scale + sx) * 4);
                                            ptr[offset] = (byte)b;     // B
                                            ptr[offset + 1] = (byte)g; // G
                                            ptr[offset + 2] = (byte)r; // R
                                            ptr[offset + 3] = 255;     // A
                                        }
                                    }
                                }
                            }
                        }
                        newBmpLocal.UnlockBits(bmpData);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обработке изображения: {ex.Message}");
                        break;
                    }
                    try
                    {
                        graphicsTarget.DrawImage(newBmpLocal, 0, 0, 128 * scale, 64 * scale);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при копировании изображения: {ex.Message}");
                        break;
                    }
                    stopwatch.Stop();
                    long renderTimeMs = stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    if (!IsDisposed && IsHandleCreated)
                    {
                        try
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                label1.Text = $"{i},{renderTimeMs}ms";
                                pictureBox1?.Refresh();
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при обновлении UI: {ex.Message}");
                            break;
                        }
                    }
                    long delay = 16 - renderTimeMs;
                    if (delay < 0) delay = 1;
                    await Task.Delay((int)delay, token);
                }
            }
            finally
            {
                graphicsTarget?.Dispose();
                brush?.Dispose();
                newBmpLocal?.Dispose();
            }
        }

 
        private async Task SendToSTM32Async(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {

                    if (!serialPort1.IsOpen)
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }

                    for (int x1 = 0; x1 < 128; x1++)
                        for (int y1 = 0; y1 < 64; y1++)
                        {
                            if (fbMain[x1, y1] == 0)
                                serial_transmit_buffer[x1 + (y1 / 8) * 128] &= (byte)(~(1 << (y1 % 8)));
                            else
                                serial_transmit_buffer[x1 + (y1 / 8) * 128] |= (byte)(1 << (y1 % 8));
                        }
                    if (serialPort1.IsOpen)
                        serialPort1.Write(serial_transmit_buffer, 0, 1024);

                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
                        });
                        await Task.Delay(1000, token);
                    }
                }
                await Task.Delay(100, token);
            }
        }

        private async Task SendToESP32UdpAsync(CancellationToken token)
        {
          
            UdpClient udpClient = new UdpClient();

            while (!token.IsCancellationRequested)
            {

                try
                {

                    if (!isUDPSelect)
                    {
                        await Task.Delay(500, token);
                        continue;
                    }
                    // Подготавливаем данные для отправки
                    for (int x1 = 0; x1 < 128; x1++)
                        for (int y1 = 0; y1 < 64; y1++)
                        {
                            if (fbMain[x1, y1] == 0)
                                serial_transmit_buffer[x1 + (y1 / 8) * 128] &= (byte)(~(1 << (y1 % 8)));
                            else
                                serial_transmit_buffer[x1 + (y1 / 8) * 128] |= (byte)(1 << (y1 % 8));
                        }


                    string ip = tbIpClient.Text.Trim();

                    // Валидация IP-адреса
                    if (!IsValidIPAddress(ip))
                    {
                        await Task.Delay(500, token);
                        continue;
                    }

                    udpClient.Send(serial_transmit_buffer, serial_transmit_buffer.Length, tbIpClient.Text, 82);

                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show($"Ошибка при отправке данных: {ex.Message}");
                        });
                        await Task.Delay(1000, token);
                    }
                }

                await Task.Delay(16, token);
            }

            udpClient.Dispose();
        }

        //private async Task SendToESP32Async(CancellationToken token)
        //{
        //    byte[] lastSentBuffer = new byte[1024]; // Буфер для сравнения
        //    bool hasChanges = false;
        //    int consecutiveNoChanges = 0; // Счетчик последовательных циклов без изменений
        //    bool forceSend = false; // Флаг принудительной отправки
            
        //    while (!token.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            // Проверяем и обновляем статус соединения
        //            CheckAndUpdateConnectionStatus();
        //            // Если нет соединения, проверяем режим работы
        //            if (!senderNet.IsConnected)
        //            {
        //                if (_autoReconnectEnabled)
        //                {
        //                    // Автоматическое переподключение включено - пытаемся переподключиться
        //                    UpdateConnectionStatus("Переподключение...");
        //                    Console.WriteLine("Соединение разорвано, пытаемся переподключиться автоматически...");
        //                    if (!senderNet.IsConnected) // Дополнительная проверка
        //                    {
        //                        // Получаем сохраненный IP из настроек
        //                        string savedIP = Properties.Settings.Default.LastIPAddress;
        //                        if (!string.IsNullOrEmpty(savedIP) && IsValidIPAddress(savedIP))
        //                        {
        //                            senderNet.Configure(savedIP, 81);
        //                            if (senderNet.Connect())
        //                            {
        //                                UpdateConnectionStatus("Подключено");
        //                                Console.WriteLine($"Автоматически переподключились к {savedIP}:81");
        //                            }
        //                            else
        //                            {
        //                                UpdateConnectionStatus("Готов к передаче");
        //                                Console.WriteLine("Не удалось автоматически переподключиться");
        //                                await Task.Delay(2000, token);
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            UpdateConnectionStatus("Готов к передаче");
        //                            Console.WriteLine("Нет сохраненного IP-адреса для автоматического переподключения");
        //                            await Task.Delay(2000, token);
        //                            continue;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // Автоматическое переподключение отключено - ждем подключения пользователем
        //                    await Task.Delay(2000, token);
        //                    continue;
        //                }
        //            }

        //            // Подготавливаем данные для отправки
        //            for (int x1 = 0; x1 < 128; x1++)
        //                for (int y1 = 0; y1 < 64; y1++)
        //                {
        //                    if (fbMain[x1, y1] == 0)
        //                        serial_transmit_buffer[x1 + (y1 / 8) * 128] &= (byte)(~(1 << (y1 % 8)));
        //                    else
        //                        serial_transmit_buffer[x1 + (y1 / 8) * 128] |= (byte)(1 << (y1 % 8));
        //                }

        //            // Проверяем, изменились ли данные (оптимизированное сравнение)
        //            hasChanges = false;
        //            // Используем unsafe код для быстрого сравнения
        //            unsafe
        //            {
        //                fixed (byte* ptr1 = serial_transmit_buffer, ptr2 = lastSentBuffer)
        //                {
        //                    for (int i = 0; i < 1024; i++)
        //                    {
        //                        if (ptr1[i] != ptr2[i])
        //                        {
        //                            hasChanges = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            // Проверяем флаг принудительной отправки
        //            if (_forceSendFlag)
        //            {
        //                forceSend = true;
        //                _forceSendFlag = false;
        //            }
                    
        //            // Отправляем данные при изменении или принудительно
        //            if (hasChanges || forceSend)
        //            {
        //                consecutiveNoChanges = 0; // Сбрасываем счетчик при изменениях
        //                forceSend = false; // Сбрасываем флаг принудительной отправки
                        
        //                bool success = senderNet.SendByteArray(serial_transmit_buffer);
        //                if (success)
        //                {
        //                    // Копируем отправленные данные для сравнения
        //                    Array.Copy(serial_transmit_buffer, lastSentBuffer, 1024);
        //                }
        //                else
        //                {
        //                    // Не показываем ошибку сразу - это может быть нормальное поведение ESP32
        //                    Console.WriteLine("Не удалось отправить данные, соединение разорвано");
        //                    await Task.Delay(100, token); // Еще больше уменьшаем задержку при ошибке
        //                }
        //            }
        //            else
        //            {
        //                consecutiveNoChanges++;
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            if (!token.IsCancellationRequested)
        //            {
        //                // Не показываем ошибку соединения - это может быть нормальное поведение
        //                Console.WriteLine($"Ошибка в SendToESP32Async: {ex.Message}");
        //                await Task.Delay(500, token); // Уменьшаем задержку при ошибке
        //            }
        //        }
                
        //        // Адаптивная задержка: еще быстрее при изменениях
        //        int delay = hasChanges ? 20 : Math.Min(50 + consecutiveNoChanges * 5, 200);
        //        await Task.Delay(delay, token);
        //    }
        //}













        #endregion

    }
}
