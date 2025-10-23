namespace SerialLCD
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.OPEN = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.CLOSE = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bUndoClear = new System.Windows.Forms.Button();
            this.bUndo = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.tbIpClient = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.bMeasureSet = new System.Windows.Forms.Button();
            this.edit_windowW = new Iocomp.Instrumentation.Standard.EditInteger();
            this.label5 = new System.Windows.Forms.Label();
            this.edit_windowH = new Iocomp.Instrumentation.Standard.EditInteger();
            this.bMeasureClear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.bFill = new System.Windows.Forms.Button();
            this.bClear = new System.Windows.Forms.Button();
            this.bLoad = new System.Windows.Forms.Button();
            this.bFastSave = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bFastLoad = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbHLine = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.bMassiv = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.cbVidelenie = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.cbVLine = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.kryptonCheckSet1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckSet(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(9, 2);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(76, 56);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // OPEN
            // 
            this.OPEN.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OPEN.Location = new System.Drawing.Point(86, 29);
            this.OPEN.Margin = new System.Windows.Forms.Padding(2);
            this.OPEN.Name = "OPEN";
            this.OPEN.Size = new System.Drawing.Size(75, 27);
            this.OPEN.TabIndex = 1;
            this.OPEN.Text = "Открыть";
            this.OPEN.UseVisualStyleBackColor = true;
            this.OPEN.Click += new System.EventHandler(this.OPEN_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.ReceivedBytesThreshold = 1024;
            this.serialPort1.WriteBufferSize = 20480;
            // 
            // CLOSE
            // 
            this.CLOSE.Enabled = false;
            this.CLOSE.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CLOSE.Location = new System.Drawing.Point(163, 29);
            this.CLOSE.Margin = new System.Windows.Forms.Padding(2);
            this.CLOSE.Name = "CLOSE";
            this.CLOSE.Size = new System.Drawing.Size(75, 27);
            this.CLOSE.TabIndex = 2;
            this.CLOSE.Text = "Закрыть";
            this.CLOSE.UseVisualStyleBackColor = true;
            this.CLOSE.Click += new System.EventHandler(this.CLOSE_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(86, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Закрыт";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(261, 75);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 18);
            this.label3.TabIndex = 18;
            this.label3.Text = "{}";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(355, 75);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 18);
            this.label4.TabIndex = 19;
            this.label4.Text = "{}";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel2.Controls.Add(this.bUndoClear);
            this.panel2.Controls.Add(this.bUndo);
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.cbHLine);
            this.panel2.Controls.Add(this.kryptonPanel1);
            this.panel2.Controls.Add(this.cbVLine);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(1);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel2.Size = new System.Drawing.Size(1025, 96);
            this.panel2.TabIndex = 22;
            // 
            // bUndoClear
            // 
            this.bUndoClear.Location = new System.Drawing.Point(391, 47);
            this.bUndoClear.Margin = new System.Windows.Forms.Padding(1);
            this.bUndoClear.Name = "bUndoClear";
            this.bUndoClear.Size = new System.Drawing.Size(42, 23);
            this.bUndoClear.TabIndex = 57;
            this.bUndoClear.Text = "Clear";
            this.bUndoClear.UseVisualStyleBackColor = true;
            this.bUndoClear.Click += new System.EventHandler(this.bUndoClear_Click);
            // 
            // bUndo
            // 
            this.bUndo.Location = new System.Drawing.Point(435, 45);
            this.bUndo.Margin = new System.Windows.Forms.Padding(1);
            this.bUndo.Name = "bUndo";
            this.bUndo.Size = new System.Drawing.Size(69, 24);
            this.bUndo.TabIndex = 56;
            this.bUndo.Text = "Undo: 0";
            this.bUndo.UseVisualStyleBackColor = true;
            this.bUndo.Click += new System.EventHandler(this.bUndo_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(4, 5);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(248, 87);
            this.tabControl1.TabIndex = 55;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.tbIpClient);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(1);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(1);
            this.tabPage1.Size = new System.Drawing.Size(240, 61);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Сеть";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(196, 43);
            this.label7.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "UDP 82";
            // 
            // tbIpClient
            // 
            this.tbIpClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbIpClient.Location = new System.Drawing.Point(2, 6);
            this.tbIpClient.Margin = new System.Windows.Forms.Padding(1);
            this.tbIpClient.Name = "tbIpClient";
            this.tbIpClient.Size = new System.Drawing.Size(136, 26);
            this.tbIpClient.TabIndex = 1;
            this.tbIpClient.Text = "192.168.0.100";
            this.tbIpClient.TextChanged += new System.EventHandler(this.tbIpClient_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.CLOSE);
            this.tabPage2.Controls.Add(this.OPEN);
            this.tabPage2.Controls.Add(this.listBox1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(1);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(1);
            this.tabPage2.Size = new System.Drawing.Size(240, 61);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "COM";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.bMeasureSet);
            this.panel4.Controls.Add(this.edit_windowW);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.edit_windowH);
            this.panel4.Controls.Add(this.bMeasureClear);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(792, 7);
            this.panel4.Margin = new System.Windows.Forms.Padding(1);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(94, 50);
            this.panel4.TabIndex = 53;
            // 
            // bMeasureSet
            // 
            this.bMeasureSet.Location = new System.Drawing.Point(52, 2);
            this.bMeasureSet.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bMeasureSet.Name = "bMeasureSet";
            this.bMeasureSet.Size = new System.Drawing.Size(41, 21);
            this.bMeasureSet.TabIndex = 33;
            this.bMeasureSet.Text = "Set";
            this.bMeasureSet.UseVisualStyleBackColor = true;
            this.bMeasureSet.Click += new System.EventHandler(this.bMeasureSet_Click);
            // 
            // edit_windowW
            // 
            this.edit_windowW.LoadingBegin();
            this.edit_windowW.Location = new System.Drawing.Point(21, 3);
            this.edit_windowW.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.edit_windowW.MaxLength = 128;
            this.edit_windowW.Name = "edit_windowW";
            this.edit_windowW.Size = new System.Drawing.Size(30, 20);
            this.edit_windowW.TabIndex = 38;
            this.edit_windowW.Value.AsString = "16";
            this.edit_windowW.LoadingEnd();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "W";
            // 
            // edit_windowH
            // 
            this.edit_windowH.LoadingBegin();
            this.edit_windowH.Location = new System.Drawing.Point(21, 21);
            this.edit_windowH.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.edit_windowH.MaxLength = 64;
            this.edit_windowH.Name = "edit_windowH";
            this.edit_windowH.Size = new System.Drawing.Size(30, 20);
            this.edit_windowH.TabIndex = 39;
            this.edit_windowH.Value.AsString = "16";
            this.edit_windowH.LoadingEnd();
            // 
            // bMeasureClear
            // 
            this.bMeasureClear.Location = new System.Drawing.Point(52, 26);
            this.bMeasureClear.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bMeasureClear.Name = "bMeasureClear";
            this.bMeasureClear.Size = new System.Drawing.Size(41, 21);
            this.bMeasureClear.TabIndex = 40;
            this.bMeasureClear.Text = "Clr";
            this.bMeasureClear.UseVisualStyleBackColor = true;
            this.bMeasureClear.Click += new System.EventHandler(this.bMeasureClear_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 21);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "H";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.bFill);
            this.panel3.Controls.Add(this.bClear);
            this.panel3.Controls.Add(this.bLoad);
            this.panel3.Controls.Add(this.bFastSave);
            this.panel3.Controls.Add(this.bSave);
            this.panel3.Controls.Add(this.bFastLoad);
            this.panel3.Location = new System.Drawing.Point(260, 6);
            this.panel3.Margin = new System.Windows.Forms.Padding(1);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(314, 34);
            this.panel3.TabIndex = 52;
            // 
            // bFill
            // 
            this.bFill.Location = new System.Drawing.Point(10, 3);
            this.bFill.Margin = new System.Windows.Forms.Padding(1);
            this.bFill.Name = "bFill";
            this.bFill.Size = new System.Drawing.Size(50, 31);
            this.bFill.TabIndex = 50;
            this.bFill.Text = "Fill";
            this.bFill.UseVisualStyleBackColor = true;
            this.bFill.Click += new System.EventHandler(this.bFill_Click);
            // 
            // bClear
            // 
            this.bClear.Location = new System.Drawing.Point(66, 3);
            this.bClear.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bClear.Name = "bClear";
            this.bClear.Size = new System.Drawing.Size(45, 27);
            this.bClear.TabIndex = 25;
            this.bClear.Text = "Clear";
            this.bClear.UseVisualStyleBackColor = true;
            this.bClear.Click += new System.EventHandler(this.bClear_Click);
            // 
            // bLoad
            // 
            this.bLoad.Font = new System.Drawing.Font("Consolas", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bLoad.Location = new System.Drawing.Point(115, 3);
            this.bLoad.Margin = new System.Windows.Forms.Padding(2);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(45, 27);
            this.bLoad.TabIndex = 46;
            this.bLoad.Text = "Load";
            this.bLoad.UseVisualStyleBackColor = true;
            this.bLoad.Click += new System.EventHandler(this.bLoad_Click);
            // 
            // bFastSave
            // 
            this.bFastSave.Font = new System.Drawing.Font("Consolas", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bFastSave.Location = new System.Drawing.Point(261, 3);
            this.bFastSave.Margin = new System.Windows.Forms.Padding(2);
            this.bFastSave.Name = "bFastSave";
            this.bFastSave.Size = new System.Drawing.Size(45, 27);
            this.bFastSave.TabIndex = 49;
            this.bFastSave.Text = "FSave";
            this.bFastSave.UseVisualStyleBackColor = true;
            this.bFastSave.Click += new System.EventHandler(this.bFastSave_Click);
            // 
            // bSave
            // 
            this.bSave.Font = new System.Drawing.Font("Consolas", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bSave.Location = new System.Drawing.Point(164, 3);
            this.bSave.Margin = new System.Windows.Forms.Padding(2);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(45, 27);
            this.bSave.TabIndex = 47;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bFastLoad
            // 
            this.bFastLoad.Font = new System.Drawing.Font("Consolas", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bFastLoad.Location = new System.Drawing.Point(212, 3);
            this.bFastLoad.Margin = new System.Windows.Forms.Padding(2);
            this.bFastLoad.Name = "bFastLoad";
            this.bFastLoad.Size = new System.Drawing.Size(45, 27);
            this.bFastLoad.TabIndex = 48;
            this.bFastLoad.Text = "FLoad";
            this.bFastLoad.UseVisualStyleBackColor = true;
            this.bFastLoad.Click += new System.EventHandler(this.bFastLoad_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(943, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 51;
            this.label1.Text = "label1";
            // 
            // cbHLine
            // 
            this.cbHLine.Location = new System.Drawing.Point(260, 43);
            this.cbHLine.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbHLine.Name = "cbHLine";
            this.cbHLine.Size = new System.Drawing.Size(45, 27);
            this.cbHLine.TabIndex = 45;
            this.cbHLine.Values.Text = "H Line";
            this.cbHLine.Click += new System.EventHandler(this.cbHLine_Click);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.bMassiv);
            this.kryptonPanel1.Controls.Add(this.listBox2);
            this.kryptonPanel1.Controls.Add(this.cbVidelenie);
            this.kryptonPanel1.Location = new System.Drawing.Point(630, 3);
            this.kryptonPanel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(142, 71);
            this.kryptonPanel1.TabIndex = 25;
            // 
            // bMassiv
            // 
            this.bMassiv.Enabled = false;
            this.bMassiv.Location = new System.Drawing.Point(65, 42);
            this.bMassiv.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.bMassiv.Name = "bMassiv";
            this.bMassiv.Size = new System.Drawing.Size(75, 27);
            this.bMassiv.TabIndex = 43;
            this.bMassiv.Text = "Массив";
            this.bMassiv.UseVisualStyleBackColor = true;
            this.bMassiv.Click += new System.EventHandler(this.bMassiv_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(2, 3);
            this.listBox2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(62, 69);
            this.listBox2.TabIndex = 42;
            // 
            // cbVidelenie
            // 
            this.cbVidelenie.Location = new System.Drawing.Point(65, 2);
            this.cbVidelenie.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbVidelenie.Name = "cbVidelenie";
            this.cbVidelenie.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.cbVidelenie.Size = new System.Drawing.Size(75, 27);
            this.cbVidelenie.TabIndex = 41;
            this.cbVidelenie.Values.Text = "Выделить";
            this.cbVidelenie.Click += new System.EventHandler(this.cbVidelenie_Click);
            // 
            // cbVLine
            // 
            this.cbVLine.Location = new System.Drawing.Point(309, 43);
            this.cbVLine.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cbVLine.Name = "cbVLine";
            this.cbVLine.Size = new System.Drawing.Size(45, 27);
            this.cbVLine.TabIndex = 44;
            this.cbVLine.Values.Text = "V Line";
            this.cbVLine.Click += new System.EventHandler(this.cbVLine_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(298, 520);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 96);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(298, 512);
            this.panel1.TabIndex = 21;
            // 
            // kryptonCheckSet1
            // 
            this.kryptonCheckSet1.AllowUncheck = true;
            this.kryptonCheckSet1.CheckButtons.Add(this.cbHLine);
            this.kryptonCheckSet1.CheckButtons.Add(this.cbVLine);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "|*dat";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1025, 608);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serial OLED V12 23.10.20.25";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonCheckSet1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button OPEN;
        private System.Windows.Forms.Button CLOSE;
        private System.Windows.Forms.Label label2;
        public System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bClear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bMeasureSet;
        private Iocomp.Instrumentation.Standard.EditInteger edit_windowH;
        private Iocomp.Instrumentation.Standard.EditInteger edit_windowW;
        private System.Windows.Forms.Button bMeasureClear;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton cbVidelenie;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button bMassiv;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckSet kryptonCheckSet1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton cbHLine;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton cbVLine;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.Button bFastSave;
        private System.Windows.Forms.Button bFastLoad;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button bUndo;
        private System.Windows.Forms.Button bUndoClear;
        private System.Windows.Forms.TextBox tbIpClient;
        private System.Windows.Forms.Button bFill;
        private System.Windows.Forms.Label label7;
    }
}

