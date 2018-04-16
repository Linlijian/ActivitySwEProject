using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using AcSwE.Models;
using iTextSharp.text.pdf.draw;
using System.Text.RegularExpressions;

namespace AcSwE.Areas.Admin.Controllers
{
    public class ManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Manager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPDF(int id)
        {
            if(id == null)
            {
                string baseUrl1 = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/" + "Error/BadRequest/";
                return Redirect(baseUrl1);
            }

            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();            
            pdfDoc.Add(GetHeader(id));
            pdfDoc.Add(GetSubHeader(id));
            pdfDoc.Add(GetLineSeparator());
            pdfDoc.Add(GetBody(id));
            pdfDoc.Add(GetBodyStudent(id));
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=2th-pdf.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return View();
        }

        private PdfPTable GetHeader(int id)
        {
            //query
            Activity data = db.Activitys.Find(id);
            //basefornt th
            BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew Bold.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font h1 = new Font(bf, 18);
            Font bold = new Font(bf, 16);
            Font smallBold = new Font(bf, 14);
            BaseFont nf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(nf, 16);
            Font smallNormal = new Font(nf, 14);

            PdfPTable headerTable = new PdfPTable(2);
            headerTable.TotalWidth = 530f;
            headerTable.HorizontalAlignment = 0;
            headerTable.SpacingAfter = 20;

            float[] headerTableColWidth = new float[2];
            headerTableColWidth[0] = 320;
            headerTableColWidth[1] = 210;

            headerTable.SetWidths(headerTableColWidth);
            headerTable.LockedWidth = true;

            Image headerTableCell_1 = Image.GetInstance(Server.MapPath("~/Content/img/pdf/logoinfor.png"));
            headerTableCell_1.ScaleAbsolute(80, 40);
            PdfPCell a = new PdfPCell(headerTableCell_1);
            a.HorizontalAlignment = Element.ALIGN_RIGHT;
            a.Border = Rectangle.NO_BORDER;
            headerTable.AddCell(a);

            PdfPCell headerTableCell_2 = new PdfPCell(new Phrase("No." + data.id, h1));
            headerTableCell_2.HorizontalAlignment = Element.ALIGN_RIGHT;
            //headerTableCell_2.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerTableCell_2.Border = Rectangle.NO_BORDER;
            headerTable.AddCell(headerTableCell_2);

            return headerTable;
        }

        private PdfPTable GetSubHeader(int id)
        {
            //query
            Activity data = db.Activitys.Find(id);
            var teacher = (from s in db.Teachers
                           join d in db.Joins on s.id equals d.idTea
                           where d.idActivity == id
                           select s).ToList();
            //basefornt th
            BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew Bold.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font h1 = new Font(bf, 18);
            Font bold = new Font(bf, 16);
            Font smallBold = new Font(bf, 14);
            BaseFont nf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(nf, 16);
            Font smallNormal = new Font(nf, 14);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 530f;
            table.HorizontalAlignment = 0;
            table.SpacingAfter = 20;

            float[] headerTableColWidth = new float[2];
            headerTableColWidth[0] = 215;
            headerTableColWidth[1] = 215;

            table.SetWidths(headerTableColWidth);
            table.LockedWidth = true;

            Chunk blank = new Chunk(" ", normal);

            Phrase p = new Phrase();

            p.Add(new Chunk("School", bold));
            p.Add(new Chunk(blank));
            p.Add(new Chunk("School of Informatics", normal));

            PdfPCell cell0 = new PdfPCell(p);
            cell0.Border = Rectangle.NO_BORDER;
            table.AddCell(cell0);

            p = new Phrase();

            p.Add(new Chunk("Major", bold));
            p.Add(new Chunk(blank));
            p.Add(new Chunk("Software Engineering", normal));
           

            PdfPCell cell1 = new PdfPCell(p);
            cell1.Border = Rectangle.NO_BORDER;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell1);

            p = new Phrase();

            p.Add(new Chunk("Adviser", bold));
            p.Add(new Chunk(blank));
            p.Add(new Chunk(teacher[0].FullName, normal));

            cell0 = new PdfPCell(p);
            cell0.Border = Rectangle.NO_BORDER;

            //cell0.Colspan = 2;
            table.AddCell(cell0);


            p = new Phrase();

            p.Add(new Chunk("Time ", bold));
            p.Add(new Chunk(blank));
            p.Add(new Chunk(data.startDate  + " to " + data.endDate, normal));


            cell1 = new PdfPCell(p);
            cell1.Border = Rectangle.NO_BORDER;
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell1);

            return table;
        }

        private LineSeparator GetLineSeparator()
        {
            LineSeparator line = new LineSeparator();
            return line;
        }

        private PdfPTable GetBody(int id)
        {
            //query
            Activity data = db.Activitys.Find(id);
            //basefornt th
            BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew Bold.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font h1 = new Font(bf, 18);
            Font bold = new Font(bf, 16);
            Font smallBold = new Font(bf, 14);
            BaseFont nf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(nf, 16);
            Font smallNormal = new Font(nf, 14);

            PdfPTable Table = new PdfPTable(2);
            Table.TotalWidth = 530f;
            Table.HorizontalAlignment = 0;
            Table.SpacingAfter = 20;

            float[] headerTableColWidth = new float[2];
            headerTableColWidth[0] = 215;
            headerTableColWidth[1] = 215;

            Table.SetWidths(headerTableColWidth);
            Table.LockedWidth = true;

            PdfPCell ACname = new PdfPCell(new Phrase("\nActivity name " + data.activityname, normal));
            ACname.HorizontalAlignment = Element.ALIGN_LEFT;
            ACname.Border = Rectangle.NO_BORDER;
            ACname.Colspan = 2;
            Table.AddCell(ACname);

            string replaceWith = " ";
            string result = Regex.Replace(data.detail, @"\r\n?|\n", replaceWith);
            

            PdfPCell ACdetail = new PdfPCell(new Phrase("     " + result + " Activity at " + data.location + " Building "
                + data.locationPoint + " Room " + data.room + " student join the " + data.yearStd
                + " in " + data.yearStudy + " term. And time in join start " + data.startDate
                + " end " + data.endDate, normal));
            ACdetail.HorizontalAlignment = Element.ALIGN_LEFT;
            ACdetail.Border = Rectangle.NO_BORDER;
            ACdetail.Colspan = 2;
            Table.AddCell(ACdetail);

            

            return Table;
        }

        private PdfPTable GetBodyStudent(int id)
        {
            //query
            Activity activity = db.Activitys.Find(id);
            var data = (from a in db.Joins where a.idActivity == id select a).ToList();
            StudentTemp t = new StudentTemp();
            for (int i = 0; i < data.Count(); i++)
            {
                t.idStd = data[i].idStd;
                db.StudentTemps.Add(t);
                db.SaveChanges();
            }

            var s = (from q in db.StudentTemps join w in db.Students on q.idStd equals w.idStd select w).ToList();
                      
            var temp = db.StudentTemps.ToList();
            StudentTemp std = new StudentTemp();
            for (int i = 0; i < temp.Count(); i++)
            {
                std = db.StudentTemps.Find(temp[i].id);
                db.StudentTemps.Remove(std);
                db.SaveChanges();
            }
            //basefornt th
            BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew Bold.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font h1 = new Font(bf, 18);
            Font bold = new Font(bf, 16);
            Font smallBold = new Font(bf, 14);
            BaseFont nf = BaseFont.CreateFont(Server.MapPath("~/fonts/THSarabunNew.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font normal = new Font(nf, 16);
            Font smallNormal = new Font(nf, 14);

            PdfPTable Table = new PdfPTable(4);
            Table.TotalWidth = 400f;
            Table.HorizontalAlignment = 0;
            Table.SpacingAfter = 20;
            Table.HorizontalAlignment = Element.ALIGN_CENTER;

            float[] headerTableColWidth = new float[4];
            headerTableColWidth[0] = 30;
            headerTableColWidth[1] = 100;
            headerTableColWidth[2] = 150;
            headerTableColWidth[3] = 20;

            Table.SetWidths(headerTableColWidth);
            Table.LockedWidth = true;

            PdfPCell cell0 = new PdfPCell(new Phrase("NO.", bold));
            cell0.HorizontalAlignment = Element.ALIGN_LEFT;
            cell0.Border = Rectangle.NO_BORDER;
            Table.AddCell(cell0);

            PdfPCell cell1 = new PdfPCell(new Phrase("ID Student", bold));
            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
            cell1.Border = Rectangle.NO_BORDER;
            Table.AddCell(cell1);

            PdfPCell cell2 = new PdfPCell(new Phrase("Name", bold));
            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
            cell2.Border = Rectangle.NO_BORDER;
            Table.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("Year", bold));
            cell3.HorizontalAlignment = Element.ALIGN_LEFT;
            cell3.Border = Rectangle.NO_BORDER;
            Table.AddCell(cell3);

            for (int i = 0; i < s.Count(); i++)
            {
                cell0 = new PdfPCell(new Phrase( (i+1) + ".", normal));
                cell0.HorizontalAlignment = Element.ALIGN_LEFT;
                cell0.Border = Rectangle.NO_BORDER;
                Table.AddCell(cell0);

                cell1 = new PdfPCell(new Phrase(s[i].idStd.ToString(), normal));
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.Border = Rectangle.NO_BORDER;
                Table.AddCell(cell1);

                cell2 = new PdfPCell(new Phrase(s[i].title + s[i].firstName + " " + s[i].lastName, normal));
                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                cell2.Border = Rectangle.NO_BORDER;
                Table.AddCell(cell2);

                cell3 = new PdfPCell(new Phrase(s[i].year.ToString(), normal));
                cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                cell3.Border = Rectangle.NO_BORDER;
                Table.AddCell(cell3);
            }
            
            return Table;
        }
    }
}