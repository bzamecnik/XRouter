using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Text;

namespace Library
{
    public class LibraryStorage
    {
        private XDocument storage;
        public XDocument Storage
        {
            get { return storage; }
            set { storage = value; }
        }

        public LibraryStorage()
        {
            storage = CreateInitialStorage();
        }

        public Reader GetReader(Guid id)
        {
            XElement xReader = storage.XPathSelectElement(
                string.Format("/library/readers/reader[@id='{0}']", id));
            return (xReader != null) ? Reader.FromXml(xReader) : null;
        }

        public Guid CreateReader(Reader reader)
        {
            XElement xReaders = storage.XPathSelectElement("/library/readers");
            reader.Id = Guid.NewGuid();
            xReaders.Add(reader.ToXml());
            return reader.Id;
        }

        public void UpdateReader(Reader reader)
        {
            XElement xReaders = storage.XPathSelectElement("/library/readers");
            DeleteReader(reader.Id);
            xReaders.Add(reader.ToXml());
        }

        public void DeleteReader(Guid id)
        {
            XElement xReader = storage.XPathSelectElement(
                string.Format("/library/readers/reader[@id='{0}']", id));
            if (xReader != null)
            {
                xReader.Remove();
            }
        }

        public Reader GetBook(Guid id)
        {
            XElement xBook = storage.XPathSelectElement(
                string.Format("/library/books/book[@id='{0}']", id));
            return (xBook != null) ? Reader.FromXml(xBook) : null;
        }

        public Guid CreateBook(Book book)
        {
            XElement xBooks = storage.XPathSelectElement("/library/books");
            book.Id = Guid.NewGuid();
            xBooks.Add(book.ToXml());
            return book.Id;
        }

        public void UpdateBook(Book book)
        {
            XElement xBooks = storage.XPathSelectElement("/library/books");
            DeleteBook(book.Id);
            xBooks.Add(book.ToXml());
        }

        public void DeleteBook(Guid id)
        {
            XElement xBook = storage.XPathSelectElement(
                string.Format("/library/books/book[@id='{0}']", id));
            if (xBook != null)
            {
                xBook.Remove();
            }
        }

        public Reader GetLoan(Guid id)
        {
            XElement xLoan = storage.XPathSelectElement(
                string.Format("/library/loans/loan[@id='{0}']", id));
            return (xLoan != null) ? Reader.FromXml(xLoan) : null;
        }

        public Guid CreateLoan(Loan loan)
        {
            XElement xLoans = storage.XPathSelectElement("/library/loans");
            loan.Id = Guid.NewGuid();
            xLoans.Add(loan.ToXml());
            return loan.Id;
        }

        public void UpdateLoan(Loan loan)
        {
            XElement xLoans = storage.XPathSelectElement("/library/loans");
            DeleteLoan(loan.Id);
            xLoans.Add(loan.ToXml());
        }

        public void DeleteLoan(Guid id)
        {
            XElement xLoan = storage.XPathSelectElement(
                string.Format("/library/loans/loan[@id='{0}']", id));
            if (xLoan != null)
            {
                xLoan.Remove();
            }
        }

        private XDocument CreateInitialStorage()
        {
            return XDocument.Parse(
@"<library>
  <readers>
  </readers>
  <books>
  </books>
  <loans>
  </loans>
</library>");
        }
    }
}
