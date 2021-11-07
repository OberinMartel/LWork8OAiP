using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Net.Mail;
using MyLib;


namespace LR8_Server
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        TcpClient client;
        ServerObject server;
        MyLib.Message m, m1, m2;
        MyLib.ComplexMessage cm = new MyLib.ComplexMessage();

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            Stream = client.GetStream();
        }

        public void Process()
        {
            while (true)
            {
                if (Stream.CanRead)
                {
                    byte[] myReadBuffer = new byte[6297630];
                    do
                    {
                        Stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    }
                    while (Stream.DataAvailable);

                    TicketPack ticketPack;
                    Contract contract;
                    User user;

                    ComplexMessage complexMessage = new ComplexMessage();
                    Message message = new Message();

                    message.Data = myReadBuffer;
                    complexMessage = (ComplexMessage) SerializeAndDeserialize.Deserialize(message);

                    if (complexMessage.NumberStatus == 0) // если 0, регистрация
                    {
                        try
                        {
                            ticketPack = (TicketPack)SerializeAndDeserialize.Deserialize(complexMessage.First);
                        }
                        catch
                        {
                            ticketPack = null;
                        }
                        try
                        {
                            contract = (Contract)SerializeAndDeserialize.Deserialize(complexMessage.First);
                        }
                        catch
                        {
                            contract = null;
                        }
                        user = (User)SerializeAndDeserialize.Deserialize(complexMessage.Second);

                        using (EntityModelContainer db = new EntityModelContainer())
                        {
                            db.UserSet.Add(user);
                            db.SaveChanges();
                        }
                    }
                    else if (complexMessage.NumberStatus == 1)
                    {
                        using (EntityModelContainer db = new EntityModelContainer())
                        {
                            byte[] responseData;

                            for (int i = 0; i < db.UserSet.ToList().Count; i++)
                            {
                                if (db.UserSet.ToList()[i].Email == Convert.ToString(SerializeAndDeserialize.Deserialize(complexMessage.First)) 
                                    && db.UserSet.ToList()[i].Password == Convert.ToString(SerializeAndDeserialize.Deserialize(complexMessage.Second)))
                                {
                                    User user1 = db.UserSet.ToList()[i];
                                    User user2 = new User()
                                    {
                                        Email = user1.Email,
                                        Password = user1.Password,
                                        Role = user1.Role
                                    };

                                    m1 = SerializeAndDeserialize.Serialize(user1);
                                    m2 = SerializeAndDeserialize.Serialize(user2);

                                    cm.First = m1;
                                    cm.Second = m2;
                                    cm.NumberStatus = 2;
                                    m = SerializeAndDeserialize.Serialize(cm);
                                    responseData = m.Data;
                                    Stream.Write(responseData, 0, responseData.Length);
                                    goto label;
                                }
                            }
                            cm.NumberStatus = 3;
                            m = SerializeAndDeserialize.Serialize(cm);

                            responseData = m.Data;
                            Stream.Write(responseData, 0, responseData.Length);
                        label:
                            responseData = null;
                        }
                    }
                    else if (complexMessage.NumberStatus == 4) // документ
                    {
                        byte[] responseData;

                        user = (User)SerializeAndDeserialize.Deserialize(complexMessage.First);
                        contract = (Contract)SerializeAndDeserialize.Deserialize(complexMessage.Second);

                        m1 = SerializeAndDeserialize.Serialize(user);
                        m2 = SerializeAndDeserialize.Serialize(contract);

                        WorkWithDoc.OutputDocFile(user, contract);

                        cm.First = m1;
                        cm.Second = m2;
                        cm.NumberStatus = 2;
                        m = SerializeAndDeserialize.Serialize(cm);
                        responseData = m.Data;
                        Stream.Write(responseData, 0, responseData.Length);
                        responseData = null;
                    }
                    else if (complexMessage.NumberStatus == 5) // документ
                    {
                        byte[] responseData;

                        string EMail = (String)SerializeAndDeserialize.Deserialize(complexMessage.First);
                        int code = (Int32)SerializeAndDeserialize.Deserialize(complexMessage.Second);

                        m1 = SerializeAndDeserialize.Serialize(EMail);
                        m2 = SerializeAndDeserialize.Serialize(code);

                        try
                        {
                            MailAddress from = new MailAddress("m79172942106@gmail.com", "Служба продажи билетов");
                            MailAddress to = new MailAddress(EMail);
                            MailMessage messageE = new MailMessage(from, to);
                            messageE.Subject = "Проверочный код";
                            messageE.Body = $"<h2>Проверочный код: {code}</h2>";
                            messageE.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                            smtp.Credentials = new NetworkCredential("m79172942106@gmail.com", "rabota000doma");
                            smtp.EnableSsl = true;
                            smtp.Send(messageE);
                            cm.First = m1;
                            cm.Second = m2;
                            cm.NumberStatus = 2;
                            m = SerializeAndDeserialize.Serialize(cm);
                            responseData = m.Data;
                            Stream.Write(responseData, 0, responseData.Length);
                        }
                        catch
                        {
                            cm.First = m1;
                            cm.Second = m2;
                            cm.NumberStatus = 3;
                            m = SerializeAndDeserialize.Serialize(cm);
                            responseData = m.Data;
                            Stream.Write(responseData, 0, responseData.Length);
                        }

                        
                        responseData = null;
                    }
                }
            }
        }



    }
}