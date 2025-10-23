using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialLCD
{
    public class LifoBuffer
    {
        private Stack<ushort[,]> bufferStack;
        private readonly int maxDepth = 100; // Максимальное количество сохранённых копий
        private ushort[,] currentBuffer;

        public LifoBuffer(int width=128, int height=64)
        {
            bufferStack = new Stack<ushort[,]>(maxDepth);
            currentBuffer = new ushort[width, height];
        }

        // Сохранить текущую копию fbMain в буфер
        public void Push(ushort[,] fbMain)
        {
            if (bufferStack.Count < maxDepth)
            {
                ushort[,] copy = new ushort[fbMain.GetLength(0), fbMain.GetLength(1)];
                Array.Copy(fbMain, copy, fbMain.Length);
                bufferStack.Push(copy);
            }
            else
            {
                // Удаляем самую старую копию, если достигнут максимум
                bufferStack.Pop();
                ushort[,] copy = new ushort[fbMain.GetLength(0), fbMain.GetLength(1)];
                Array.Copy(fbMain, copy, fbMain.Length);
                bufferStack.Push(copy);
            }
        }

        // Извлечь последнюю копию и восстановить её в fbMain
        public bool Pop(ushort[,] fbMain)
        {
            if (bufferStack.Count > 0)
            {
                ushort[,] restored = bufferStack.Pop();
                Array.Copy(restored, fbMain, fbMain.Length);
                return true;
            }
            return false;
        }

        // Получить количество сохранённых копий
        public int Count
        {
            get { return bufferStack.Count; }
        }

        // Очистка буфера
        public void Clear()
        {
            bufferStack.Clear();
        }
    }
}
