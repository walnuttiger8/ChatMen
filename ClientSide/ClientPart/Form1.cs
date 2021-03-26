using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net.Sockets;

namespace ClientPart
{
    public partial class Form1 : Form
    {
        string message = "";
        private const string host = "25.101.238.49";
        private const int port = 8888;
        private string clientName = "";
        static TcpClient client;
        static NetworkStream stream;
        bool flag = true;


        public Form1()
        {
            InitializeComponent();
            client = new TcpClient();
            client.Connect(host, port);
            richTextBoxChat.Text += "Введите свое имя: " + '\n';

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if(flag)
            {
                this.message = richTextBoxMessage.Text;
                this.clientName = this.message;
                timer1.Enabled = true;
                richTextBoxMessage.Text = "";
            }
            else
            {
                this.message = richTextBoxMessage.Text;
                timer1.Enabled = true;
                richTextBoxChat.Text += this.clientName + ":" + this.message + "\n";
                richTextBoxMessage.Text = "";

            }
        }
        void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        void ReceiveMessage()
        {
            while(true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = stream.Read(data,0,data.Length);
                        builder.Append(Encoding.Unicode.GetString(data,0,bytes));
                    }
                    while(stream.DataAvailable);
                    message = builder.ToString();
                    richTextBoxChat.Invoke(new Action(() => richTextBoxChat.Text += message + '\n'));
                }
                catch
                {
                    richTextBoxChat.Invoke(new Action(() => richTextBoxChat.Text += "Подключение прервано!" + "\n"));
                    Disconnect();
                }
            }
        }
        static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Environment.Exit(0);
        }
        

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                if (flag)
                {
                    stream = client.GetStream(); // получаем поток
                    Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                    receiveThread.Start(); //старт потока
                    SendMessage(message);
                    richTextBoxChat.Text += "Добро пожаловать, " + message + "\n";
                    flag = false;
                    timer1.Enabled = false; //остановка таймера
                }
                else
                {
                    stream = client.GetStream(); // получаем поток
                    Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                    receiveThread.Start(); //старт потока
                    SendMessage(message);
                    timer1.Enabled = false; //остановка таймера
                }
            }
            catch
            {

            }
        }
    }
}
