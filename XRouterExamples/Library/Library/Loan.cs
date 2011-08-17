using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Text;

namespace Library
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid ReaderId { get; set; }
        public Guid BookId { get; set; }
        public string Status { get; set; }
        public DateTime DateLoaned { get; set; }
        public DateTime DateLoanDeadline { get; set; }
        public DateTime DateReturned { get; set; }

        public static Loan FromXml(XElement xml)
        {
            Loan loan = new Loan();
            loan.Id = Guid.Parse(xml.Attribute("id").Value);
            loan.ReaderId = Guid.Parse(xml.Attribute("readerId").Value);
            loan.BookId = Guid.Parse(xml.Attribute("bookId").Value);
            loan.Status = xml.XPathSelectElement("/loan/status").Value;
            loan.DateLoaned = DateTime.Parse(xml.XPathSelectElement("/loan/dateLoaned").Value);
            loan.DateLoanDeadline = DateTime.Parse(xml.XPathSelectElement("/loan/dateLoanDeadline").Value);
            loan.DateReturned = DateTime.Parse(xml.XPathSelectElement("/loan/dateReturned").Value);
            return loan;
        }

        public XElement ToXml()
        {
            XElement xLoan = new XElement("loan");
            xLoan.SetAttributeValue("id", Id);
            xLoan.SetAttributeValue("readerId", ReaderId);
            xLoan.SetAttributeValue("bookId", BookId);
            xLoan.Add(new XElement("status") { Value = Status });
            xLoan.Add(new XElement("dateLoaned") { Value = DateLoaned.ToString() });
            xLoan.Add(new XElement("dateLoanDeadline") { Value = DateLoanDeadline.ToString() });
            xLoan.Add(new XElement("dateReturned") { Value = DateReturned.ToString() });
            return xLoan;
        }
    }
}
