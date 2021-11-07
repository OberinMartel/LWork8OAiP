using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyLib;

namespace KassirRu
{
    public partial class Authorization : Form
    {
        TcpClient client = new TcpClient("127.0.0.1", 8888);
        Byte[] data;
        NetworkStream stream;
        MyLib.Message m, m1, m2;
        ComplexMessage cm = new ComplexMessage();

        public Authorization()
        {
            InitializeComponent();
            this.stream = client.GetStream();
        }

        // обработчик события строчки "Зарегистрироваться"
        private void labelNoAccount_Click(object sender, EventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Hide();
        }

        // обработчик события строчки "Забыл(а) пароль"
        private void labelForgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPasswordFirst forgotPasswordFirst = new ForgotPasswordFirst(this);
            forgotPasswordFirst.Show();
            this.Hide();
        }

        // обработчик события кнопки "Авторизоваться"
        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            if (textBoxEnterEMail.Text != "" && textBoxEnterPassword.Text != "")
            {
                this.m1 = SerializeAndDeserialize.Serialize(textBoxEnterEMail.Text);
                this.m2 = SerializeAndDeserialize.Serialize(textBoxEnterPassword.Text);
                this.cm.First = this.m1;
                this.cm.Second = this.m2;
                this.cm.NumberStatus = 1;
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
                    if (cm.NumberStatus == 2) //Если статус запроса «Успешная авторизация» 
                    {
                        ComplexMessage complexMessage = (ComplexMessage)SerializeAndDeserialize.Deserialize(m);
                        User user1 = (User)SerializeAndDeserialize.Deserialize(complexMessage.First);
                        if (user1.Role == "User      ")
                        {
                            MessageBox.Show("Вход успешен!");
                            UserPanel userPanel = new UserPanel(this);
                            userPanel.Text = user1.Email;
                            userPanel.Show();
                            this.Visible = false;
                            textBoxEnterEMail.Text = "";
                            textBoxEnterPassword.Text = "";
                            return;
                        }
                        else if (user1.Role == "Manager   ")
                        {
                            MessageBox.Show("Вход успешен!");
                            ManagerPanel managerPanel = new ManagerPanel(this);
                            managerPanel.Text = user1.Email;
                            managerPanel.Show();
                            this.Visible = false;
                            textBoxEnterEMail.Text = "";
                            textBoxEnterPassword.Text = "";
                            return;
                        }
                        else if (user1.Role == "Admin     ")
                        {
                            MessageBox.Show("Вход успешен!");
                            AdminPanel adminPanel = new AdminPanel(this);
                            adminPanel.Text = user1.Email;
                            adminPanel.Show();
                            this.Visible = false;
                            textBoxEnterEMail.Text = "";
                            textBoxEnterPassword.Text = "";
                            return;
                        }
                    }
                    else if (cm.NumberStatus == 3) //Если статус запроса «Ошибка авторизации» 
                    {
                        MessageBox.Show("Неверный логин или пароль");
                    }

                }
                else
                {
                    MessageBox.Show("Заполните все поля");
                }
            }
        }

        // метод для хэширования пароля
        private string GetHashString(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = "";
            foreach (byte b in byteHash)
            {
                hash += string.Format("{0:x2}", b);
            }
            return hash;
        }
    }
}
