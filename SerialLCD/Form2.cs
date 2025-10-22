using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SerialLCD
{
    public partial class Form2 : Form
    {
        Form1 f1;

        ushort[,] frameBufferSelect = new ushort[128, 64];
        byte[] byteBuffer= new byte[1024];

        public Form2(Form1 f)
        {
            InitializeComponent();
            //f.BackColor = Color.Yellow;
            f1 = f;
        }

        public void setPixel(byte[] array, short x, short y, short w)
        {
            array[x + (y / 8) * w] |= (byte)(1 << (y % 8));
        }

        public void CreateArray()
        {

            Array.Clear(byteBuffer, 0, byteBuffer.Length);
            Array.Clear(frameBufferSelect, 0, frameBufferSelect.Length);

            _contur contur;
            contur = f1.contur;
       

            for (short x = 0; x < contur.w; x++)
                for (short y = 0; y < contur.h; y++)
                {
                    frameBufferSelect[x, y] = f1.fbMain[contur.x1 + x, contur.y1 + y];
                    if (frameBufferSelect[x, y] == 0xFFFF)
                        setPixel(byteBuffer, x, y, contur.w);
                }
            string str;
            short temp;
            ushort utemp;

            str  = "#include \"TFT.h\" \r\n";
            str += "const uhsigned char _xxx[]={\r\n";

            int p = 0;

            for (short y = 0; y <= ((contur.h - 1) / 8); y++)
            {

                for (short x = 0; x < contur.w; x++)
                {

                    utemp = byteBuffer[p];
                    str += "0x" + Convert.ToString(utemp, 16) + ", ";
                    p++;
                }

                str += "\r\n";
            }




            str += "}\n\n";
            str += "Bitmap xxx = {\n  ";

            temp = contur.w;
            str += temp.ToString() + ", \r\n  ";
            temp = contur.h;
            str += temp.ToString() + ", \r\n  ";
            str += "&_xxx[0], \r\n  NULL,\r\n  NULL,\r\n  1\r\n};";

            kryptonRichTextBox1.Text = str;





        }



        private void Generate_Click(object sender, EventArgs e)
        {
            CreateArray();
        }

        private void kryptonRichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateArray();
        }
    }
}
