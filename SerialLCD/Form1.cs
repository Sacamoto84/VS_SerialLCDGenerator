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

    public partial class Form1 : KryptonForm


    {

        private Task Proceso1;
        private Task tSendToSTM32;


        const int scale = 10;
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

        public _contur contur;

        byte[] start = new byte[4] { 1, 2, 3, 4 };
        byte LCD_W = 128;
        byte LCD_H = 64;

        Bitmap newBmp = new Bitmap(128 * scale, 64 * scale);

        Bitmap newBmpMini = new Bitmap(128, 64);

        int mouse_x, mouse_y;

        int window_H = 16, window_W = 16, window_Visible = 0;

        //Thread Proceso1;
        //Thread tSendToSTM32;

        private CancellationTokenSource cts = new CancellationTokenSource();


        #region Не менять
        public Form1()
        {
            InitializeComponent();

            pictureBox1.Image = newBmp;

            pictureBox1.Size = new Size(128 * scale, 64 * scale);

            this.Width = 128 * scale + panel2.Width + 32;
            this.Height = 64 * scale + 32;



            // Start threads with cancellation token
            //Proceso1 = new Thread(() => Render(cts.Token));
            //Proceso1.Priority = ThreadPriority.Highest;
            //Proceso1.IsBackground = true; // Mark as background thread
            //Proceso1.Start();

            //tSendToSTM32 = new Thread(() => SendToSTM32(cts.Token));
            //tSendToSTM32.IsBackground = true; // Mark as background thread
            //tSendToSTM32.Start();

            Proceso1 = Task.Run(() => RenderAsync(cts.Token), cts.Token);
            tSendToSTM32 = Task.Run(() => SendToSTM32Async(cts.Token), cts.Token);

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
                        LineV(fbMain, (short)mouse_x, 0, 64, 0xFFFF);
                    if (e.Button == MouseButtons.Right)
                        LineV(fbMain, (short)mouse_x, 0, 64, 0);
                }

                if (ModeLine == HLINE)
                {
                    if (e.Button == MouseButtons.Left)
                        LineH(fbMain, (short)mouse_y, 0, 128, 0xFFFF);
                    if (e.Button == MouseButtons.Right)
                        LineH(fbMain, (short)mouse_y, 0, 128, 0);
                }

                if (e.Button == MouseButtons.Left)
                    fbMain[mouse_x, mouse_y] = 0xFFFF;

                if (e.Button == MouseButtons.Right)
                    fbMain[mouse_x, mouse_y] = 0;
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

        #endregion

        #region Потоки отрисовки и отсылки
        byte[] serial_transmit_buffer = new byte[1024];

        //public void Render(CancellationToken token)
        //{
        //    int i = 0;
        //    Bitmap newBmpLocal = new Bitmap(128 * scale, 64 * scale);
        //    Graphics graphics = Graphics.FromImage(newBmpLocal); // Создаём Graphics один раз
        //    Graphics graphicsTarget = Graphics.FromImage(newBmp); // Graphics для глобального Bitmap
        //    Stopwatch stopwatch = new Stopwatch(); // Для измерения времени
        //    SolidBrush brush = new SolidBrush(Color.Black);

        //    while (!token.IsCancellationRequested)
        //    {
        //        stopwatch.Start(); // Начало измерения времени

        //        i++;
        //        ushort x, y;
        //        ushort pix;

        //        // Обновление буфера рендера
        //        for (x = 0; x < LCD_W; x++) fbMeasure[x, window_H] = 0xF81F;
        //        for (y = 0; y < LCD_H; y++) fbMeasure[window_W, y] = 0xF81F;
        //        for (x = 0; x < LCD_W; x++)

        //            for (y = 0; y < LCD_H; y++)
        //            {
        //                frameBufferRender[x, y] = fbMain[x, y];
        //                // Рамка окна
        //                if (window_Visible == 1) frameBufferRender[x, y] |= fbMeasure[x, y];
        //                // Контур для выделения
        //                if (contur.enable)
        //                {
        //                    if (frameBufferLayer_Contur[x, y] == GREEN)
        //                    {
        //                        if (frameBufferRender[x, y] == 0) frameBufferRender[x, y] = 0x654C;
        //                        else
        //                            frameBufferRender[x, y] = 0x8F51; // Показать если выбран режим
        //                    }
        //                }
        //                if (frameBufferLayer_Mouse_Cursor[x, y] > 0)
        //                    frameBufferRender[x, y] = frameBufferLayer_Mouse_Cursor[x, y];
        //            }

        //        try
        //        {                  
        //            // Очистка изображения
        //            graphics.Clear(Color.Black);
        //            // Рисование квадратов
        //            for (x = 0; x < LCD_W; x++)
        //                for (y = 0; y < LCD_H; y++)
        //                {
        //                    pix = frameBufferRender[x, y];
        //                    int r = (((pix & 0xF800) >> 11) << 3);
        //                    int g = (((pix & 0x07E0) >> 5) << 2);
        //                    int b = ((pix & 0x001F) << 3);

        //                    brush.Color = Color.FromArgb(r, g, b);
        //                    int scaledX = x * scale;
        //                    int scaledY = y * scale;
        //                    graphics.FillRectangle(brush, scaledX, scaledY, scale, scale);

        //                }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Ошибка при обработке изображения: " + ex.Message);
        //            break;
        //        }

        //        // Копирование newBmpLocal в newBmp
        //        try
        //        {
        //            graphicsTarget.DrawImage(newBmpLocal, 0, 0, 128 * scale, 64 * scale);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Ошибка при копировании изображения: " + ex.Message);
        //            break;
        //        }

        //        stopwatch.Stop(); // Окончание измерения времени
        //        long renderTimeMs = stopwatch.ElapsedMilliseconds; // Время рендера в миллисекундах
        //        stopwatch.Reset(); // Сброс для следующего измерения

        //        // Обновление UI
        //        if (!this.IsDisposed && this.IsHandleCreated)
        //        {
        //            try
        //            {
        //                this.Invoke((MethodInvoker)delegate
        //                {
        //                    label1.Text = $"{i},{renderTimeMs}ms";
        //                    pictureBox1?.Refresh();
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Ошибка при обновлении UI: " + ex.Message);
        //                break;
        //            }
        //        }

        //        long delay = 16 - renderTimeMs;
        //        if (delay < 0) delay = 1;

        //        Thread.Sleep((int)delay);
        //    }


        //    // Clean up resources
        //    graphics.Dispose();
        //    graphicsTarget.Dispose();
        //    newBmpLocal.Dispose();
        //    brush.Dispose
        //}

        private async Task RenderAsync(CancellationToken token)
        {
            int i = 0;
            Bitmap newBmpLocal = new Bitmap(128 * scale, 64 * scale);
            Graphics graphics = Graphics.FromImage(newBmpLocal);
            Graphics graphicsTarget = Graphics.FromImage(newBmp);
            SolidBrush brush = new SolidBrush(Color.Black);
            Stopwatch stopwatch = new Stopwatch();

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
                    graphics.Clear(Color.Black);
                    for (x = 0; x < LCD_W; x++)
                        for (y = 0; y < LCD_H; y++)
                        {
                            pix = frameBufferRender[x, y];
                            int r = ((pix & 0xF800) >> 11) << 3;
                            int g = ((pix & 0x07E0) >> 5) << 2;
                            int b = (pix & 0x001F) << 3;
                            brush.Color = Color.FromArgb(r, g, b);
                            graphics.FillRectangle(brush, x * scale, y * scale, scale, scale);
                        }
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
        #endregion

    }
}
