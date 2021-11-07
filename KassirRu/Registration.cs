using MyLib;
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

namespace KassirRu
{
    public partial class Registration : Form
    {
        private TcpClient client = new TcpClient("127.0.0.1", 8888);
        private Byte[] data;
        private NetworkStream stream;
        private MyLib.Message m1, m2, m;
        private ComplexMessage cm = new ComplexMessage();

        public Registration()
        {
            InitializeComponent();
            this.stream = client.GetStream();
            //using (EntityModelContainer db = new EntityModelContainer())
            //{
            //    User user = new User("Admin", this.GetHashString("Admin"), "Admin");
            //    db.UserSet.Add(user);
            //    db.SaveChanges();
            //}
        }

        private void InitComponentMessage(object first, object second, int status)
        {
            this.m1 = SerializeAndDeserialize.Serialize(first);
            this.m2 = SerializeAndDeserialize.Serialize(second);
            cm.First = m1;
            cm.Second = m2;
            cm.NumberStatus = status;
            m = SerializeAndDeserialize.Serialize(cm);
            this.data = m.Data;
        }


        // переход на форму авторизации
        private void labelHaveAccount_Click(object sender, EventArgs e)
        {
            Authorization authorization = new Authorization();
            authorization.Show();
            this.Hide();
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

        // обработчик события кнопки "Зарегистрироваться"
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (textBoxEnterEMail.Text != "" && textBoxEnterPassword.Text != "" && textBoxRepeatPassword.Text != "")
            {
                if (textBoxEnterPassword.Text == textBoxRepeatPassword.Text)
                {
                    User user = new User(textBoxEnterEMail.Text, textBoxEnterPassword.Text, "User");
                    this.InitComponentMessage(user, user, 0);

                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                else
                {
                    MessageBox.Show("Пароли не совпадают");
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
        }
    }
}
