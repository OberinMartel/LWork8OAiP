using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using MyLib;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace KassirRu
{
    public partial class ForgotPasswordFirst : Form
    {
        public Authorization authorization;
        int code;
        Random rnd = new Random();

        TcpClient client = new TcpClient("127.0.0.1", 8888);
        Byte[] data;
        NetworkStream stream;
        MyLib.Message m, m1, m2;
        ComplexMessage cm = new ComplexMessage();

        public ForgotPasswordFirst()
        {
            InitializeComponent();
            this.stream = client.GetStream();
        }
        public ForgotPasswordFirst(Authorization authorization)
        {
            InitializeComponent();
            this.authorization = authorization;
            this.stream = client.GetStream();
        }

        // обработчик события клика по кнопке "Отправить код"
        private void buttonSendTheCode_Click(object sender, EventArgs e)
        {
            if (textBoxEMail.Text != "")
            {
                using (EntityModelContainer db = new EntityModelContainer())
                {
                    bool flag = true;
                    foreach (User user in db.UserSet)
                    {
                        if (textBoxEMail.Text == user.Email)
                        {
                            code = rnd.Next(1000, 9999);

                            this.m1 = SerializeAndDeserialize.Serialize(user.Email);
                            this.m2 = SerializeAndDeserialize.Serialize(code);
                            this.cm.First = this.m1;
                            this.cm.Second = this.m2;
                            this.cm.NumberStatus = 5;
                            this.m = SerializeAndDeserialize.Serialize(this.cm);
                            data = this.m.Data; //Передача массива байтов Data из объекта m в массив data 
                            stream.Write(data, 0, data.Length); //Отправка массива байтов data серверу 
                            if (stream.CanRead) //Если поток доступен для чтения 
                            {
                                int numberOfBytesRead = 0;
                                byte[] readingData = new byte[6297630];
                                do
                                {
                                    numberOfBytesRead = stream.Read(readingData, 0, readingData.Length); //Считываем данные, полученные от сервера, в массив байтов readingData 
                                }
                                while (stream.DataAvailable);
                                this.m.Data = readingData;
                                this.cm = (ComplexMessage)SerializeAndDeserialize.Deserialize(m); //Поэтапно десериализуем массив байтов для восстановления объектов из полученного пакета 
                                if(cm.NumberStatus == 3) MessageBox.Show("Проверьте корректность введдённой почты");
                            }
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        MessageBox.Show("Введённая почта не зарегистрирована");
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите электронную почту");
            }
        }

        // обработчик события клика по кнопке "Изменить пароль"
        private void buttonChangePassword_Click(object sender, EventArgs e)
        {
            if (textBoxCode.Text != "")
            {
                if (int.Parse(textBoxCode.Text) == code)
                {
                    ForgotPasswordSecond forgotPasswordSecond = new ForgotPasswordSecond(this);
                    forgotPasswordSecond.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Код неверный");
                }
            }
            else
            {
                MessageBox.Show("Введите проверочный код");
            }
        }

        private void ForgotPasswordFirst_FormClosed(object sender, FormClosedEventArgs e)
        {
            authorization.Show();
        }
    }
}
