using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Text;

namespace Library
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Desciption { get; set; }
        public string YearPublished { get; set; }
        public string Publisher { get; set; }
        public bool Available { get; set; }

        public static Book FromXml(XElement xml)
        {
            Book book = new Book();
            book.Id = Guid.Parse(xml.Attribute("id").Value);
            book.Isbn = xml.XPathSelectElement("/book/isbn").Value;
            book.Title = xml.XPathSelectElement("/book/title").Value;
            book.Authors = xml.XPathSelectElement("/book/authors").Value;
            book.Desciption = xml.XPathSelectElement("/book/desciption").Value;
            book.YearPublished = xml.XPathSelectElement("/book/yearPublished").Value;
            book.Publisher = xml.XPathSelectElement("/book/publisher").Value;
            book.Available = Boolean.Parse(xml.XPathSelectElement("/book/available").Value);
            return book;
        }

        public XElement ToXml()
        {
            XElement xBook = new XElement("reader");
            xBook.SetAttributeValue("id", Id);
            xBook.Add(new XElement("isbn") { Value = Isbn });
            xBook.Add(new XElement("title") { Value = Title });
            xBook.Add(new XElement("authors") { Value = Authors });
            xBook.Add(new XElement("desciption") { Value = Desciption });
            xBook.Add(new XElement("yearPublished") { Value = YearPublished });
            xBook.Add(new XElement("publisher") { Value = Publisher });
            xBook.Add(new XElement("available") { Value = Available.ToString() });
            return xBook;
        }
    }
}
