using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace MyLib
{
    public static class WorkWithDoc
    {
        public static void OutputDocFile(User user, Contract contract)
        {
            string pathDocument = AppDomain.CurrentDomain.BaseDirectory + "first.docx";
            DocX document = DocX.Create(pathDocument);
            document.MarginLeft = 60.0f;
            document.MarginRight = 60.0f;
            document.MarginTop = 60.0f;
            document.MarginBottom = 60.0f;
            document.InsertParagraph("ОНЛАЙН СЕРВИС ПРОДАЖИ БИЛЕТОВ МАССОВЫХ МЕРОПРИЯТИЙ").Bold()
                .Font("Times New Roman").FontSize(14).Alignment = Alignment.center;
            document.InsertParagraph("\n" + "Кассир.ру" + "\n\n").Font("Times New Roman")
                .FontSize(14).Alignment = Alignment.center;
            document.InsertParagraph("Ч Е К").Font("Times New Roman").FontSize(16).Alignment = Alignment.center;
            document.InsertParagraph("на покупку билета").Font("Times New Roman").FontSize(12).Bold().Alignment = Alignment.center;

            Table table = document.AddTable(4, 2);
            table.Alignment = Alignment.center;
            table.Design = TableDesign.TableGrid;
            table.SetWidths(new float[] { 180.0f, 600.0f });
            table.Rows[0].Cells[0].Paragraphs[0].Append("Покупатель").Font("Times New Roman").FontSize(12).Bold();
            table.Rows[0].Cells[1].Paragraphs[0].Append(user.Email).Font("Times New Roman").FontSize(12).Bold();
            table.Rows[1].Cells[0].Paragraphs[0].Append("Номер чека").Font("Times New Roman").FontSize(12).Bold();
            table.Rows[1].Cells[1].Paragraphs[0].Append(contract.Id.ToString()).Font("Times New Roman").FontSize(12).Bold();
            table.Rows[2].Cells[0].Paragraphs[0].Append("Сумма чека").Font("Times New Roman").FontSize(12).Bold();
            table.Rows[2].Cells[1].Paragraphs[0].Append(contract.Sum.ToString()).Font("Times New Roman").FontSize(12).Bold();
            table.Rows[3].Cells[0].Paragraphs[0].Append("Дата покупки").Font("Times New Roman").FontSize(12).Bold();
            table.Rows[3].Cells[1].Paragraphs[0].Append(contract.Registration_Date.ToString()).Font("Times New Roman").FontSize(12).Bold();

            document.InsertParagraph().InsertTableAfterSelf(table);
            document.Save();
        }
    }
}
