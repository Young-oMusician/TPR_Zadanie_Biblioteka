﻿using System;
using System.Collections.Generic;
using System.Linq;
using DL;
using DL.DataObjects;
using DL.Interfaces;
using DL.DataObjects.EventsObjects;
using System.ComponentModel;

namespace BLL
{
    public class DataService
    {
        private IDataLayerAPI _dataLayer;

        public DataService(IDataLayerAPI dataLayer)
        {
            this._dataLayer = dataLayer;
        }

        // Authors methods

        public void AddAuthor(string name, string surname)
        {
            _dataLayer.AddAuthor(new Author(Guid.NewGuid(), name, surname));
        }

        public void DeleteAuthor(Author author)
        {
            if (_dataLayer.GetAllBooks().FirstOrDefault(b => b.Author.Equals(author)) != default)
                throw new ArgumentException("At least one book has reference to author!");
            _dataLayer.DeleteAuthor(author);
        }

        public Author GetAuthor(Author author)
        {
            return _dataLayer.GetAuthor(author.Id);
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            return _dataLayer.GetAllAuthors();
        }

        public Author FindAuthor(Predicate<Author> parameter)
        {
            return _dataLayer.FindAuthor(parameter);
        }

        // Books methods

        public void AddBook(string name, Author author, string description, Book.BookType bookType)
        {
            if (_dataLayer.GetAuthor(author.Id) == null)
                throw new ArgumentException("Author doesn't exists in repository!");
            _dataLayer.AddBook(new Book(name, author, description, bookType));
        }

        public void DeleteBook(Book book)
        {
            if (_dataLayer.GetAllCopiesOfBook().FirstOrDefault(b => b.Book.Equals(book)) != default)
                throw new ArgumentException("At least one copy of book has reference to book!");
            _dataLayer.DeleteBook(book);
        }

        public Book GetBook(Book book)
        {
            return _dataLayer.GetBook(_dataLayer.GetBookPosition(book));
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _dataLayer.GetAllBooks();
        }

        public Book FindBook(Predicate<Book> parameter)
        {
            return _dataLayer.FindBook(parameter);
        }

        // Copies of Books methods

        public void AddCopyOfBook(Book book, DateTime purchaseDate, double pricePerDay)
        {
            if (book == null)
                throw new ArgumentException("This book doesn't exists in repository!");
            _dataLayer.AddCopyOfBook(new CopyOfBook(Guid.NewGuid(), book, purchaseDate, pricePerDay));
        } 

        public void DeleteCopyOfBook(CopyOfBook copyOfBook)
        {
            if (IsCopyOfBookRented(copyOfBook))
            {
                throw new ArgumentException("This copy of book is already rented!");
            }
            _dataLayer.DeleteCopyOfBook(copyOfBook);
        }

        public bool IsCopyOfBookRented(CopyOfBook copyOfBook)
        {
            IEnumerable<CopyOfBook> rentedBooks = GetRentedCopiesOfBooks();
            return rentedBooks.Contains(copyOfBook);
        } 
    
        public CopyOfBook GetCopyOfBook(CopyOfBook copyOfBook)
        {
            return _dataLayer.GetCopyOfBook(copyOfBook.Id);
        }

        public IEnumerable<CopyOfBook> GetAllCopiesOfBook()
        {
            return _dataLayer.GetAllCopiesOfBook();
        }

        public CopyOfBook FindCopyOfBook(Predicate<CopyOfBook> parameter)
        {
            return _dataLayer.FindCopyOfBook(parameter);
        }

        // Employees methods

        public void AddEmployee(string name, string surname, DateTime birthDate,
            string phoneNumber, string email, Employee.Gender gender, DateTime dateOfEmployment)
        {
            if ((_dataLayer.GetAllEmployees().FirstOrDefault(e => e.PhoneNumber.Equals(phoneNumber)) != default) || (_dataLayer.GetAllEmployees().FirstOrDefault(e => e.Email.Equals(email)) != default))
                throw new ArgumentException("Person with the same email or phone number exists!");
            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format!");
            if (!IsValidPhoneNumber(phoneNumber))
                throw new ArgumentException("Invalid phone number format!");
            _dataLayer.AddEmployee(new Employee(Guid.NewGuid(), name, surname, birthDate, phoneNumber, email, gender, dateOfEmployment));
        }

        public void DeleteEmployee(Employee employee)
        {
            _dataLayer.DeleteEmployee(employee);
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _dataLayer.GetAllEmployees();
        }

        public void UpdateEmployee(Guid id, Employee employee)
        {
            _dataLayer.UpdateEmployee(id, employee);
        }

        public Employee FindEmployee(Predicate<Employee> parameter)
        {
            return _dataLayer.FindEmployee(parameter);
        }

        // Readers methods

        public void AddReader(string name, string surname, DateTime birthDate,
            string phoneNumber, string email, Employee.Gender gender, DateTime dateOfRegistration)
        {
            if ((_dataLayer.GetAllReaders().FirstOrDefault(e => e.PhoneNumber.Equals(phoneNumber)) != default || (_dataLayer.GetAllEmployees().FirstOrDefault(e => e.Email.Equals(email)) != default)))
                throw new ArgumentException("Person with the same email or phone number exists!");
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format!");
            }
            if (!IsValidPhoneNumber(phoneNumber))
            {
               throw new ArgumentException("Invalid phone number format!");
            }
            _dataLayer.AddReader(new Reader(Guid.NewGuid(), name, surname, birthDate, phoneNumber, email, gender, dateOfRegistration));
        }

        public void DeleteReader(Reader reader)
        {
            if(GetAllCurrentRents().FirstOrDefault(r => r.Reader.Equals(reader)) != default)
            {
                throw new ArgumentException("Reader has got rent currently!");
            }
            _dataLayer.DeleteReader(reader);
        }

        public IEnumerable<Reader> GetAllReaders()
        {
            return _dataLayer.GetAllReaders();
        }

        public void UpdateReader(Guid id, Reader reader)
        {
            _dataLayer.UpdateReader(id, reader);
        }

        public Reader FindReader(Predicate<Reader> parameter)
        {
            return _dataLayer.FindReader(parameter);
        }

        // Rents methods

        public void AddRent(Reader reader, Employee employee, List<CopyOfBook> books)
        {
            if (!_dataLayer.GetAllReaders().Contains(reader))
            {
                throw new ArgumentException("Reader doesn't exist in repository!");
            }
            if (!_dataLayer.GetAllEmployees().Contains(employee))
            {
                throw new ArgumentException("Employee doesn't exist in repository!");
            }
            foreach(CopyOfBook book in books)
            {
                if (!_dataLayer.GetAllCopiesOfBook().Contains(book))
                {
                    throw new ArgumentException("At least one of book doesn't exists in repository!");
                }
                if (IsCopyOfBookRented(book))
                {
                    throw new ArgumentException("This copy of book is already rented!");
                }
            }
            IEnumerable<Rent> rents = GetAllCurrentRents();
            if(rents.FirstOrDefault(rent => rent.Reader.Id.Equals(reader.Id)) != default)
            {
                throw new ArgumentException("This reader already has rent");
            }
            _dataLayer.AddEvent(new Rent(Guid.NewGuid(), reader, employee, books, DateTime.Now));
        }

        public IEnumerable<Rent> GetAllRents() 
        {
            List<Rent> rents = new List<Rent>();
            foreach(Event _event in _dataLayer.GetAllEvents())
            {
                if (typeof(Rent).Equals(_event.GetType()))
                {
                    rents.Add((Rent)_event);
                }
            }
            return rents;
        }

        public IEnumerable<Rent> GetAllCurrentRents()
        {
            List<Rent> rents = GetAllRents().ToList();
            foreach(Rent rent in GetAllRents())
            {
                if (rent.DateOfReturn != DateTime.MinValue)
                {
                    rents.Remove(rent);
                }
            }
            return rents;
        }

        public Event FindEvent(Predicate<Event> parameter)
        {
            return _dataLayer.FindEvent(parameter);
        }

        // Returns methods

        public void AddReturn(Rent rent, List<CopyOfBook> books)
        {
            if (!GetAllRents().Contains(rent))
            {
                throw new ArgumentException("Rent didn't happen!");
            }
            if (!GetAllCurrentRents().Contains(rent))
            {
                throw new ArgumentException("Rent is closed!");
            }
            foreach (CopyOfBook book in books)
            {
                if (!GetRentedCopiesOfBooks(rent).Contains(book))
                {
                    throw new ArgumentException("At least one of returned copies doesn't contain in rent or has been already returned!");
                }
            }
            _dataLayer.AddEvent(new Return(Guid.NewGuid(), DateTime.Now, books, rent));
            if(GetRentedCopiesOfBooks(rent).Count() == 0)
            {
                rent.DateOfReturn = DateTime.Now;
            }
        }

        public IEnumerable<Return> GetAllReturns()
        {
            List<Return> returns = new List<Return>();
            foreach(Event _event in _dataLayer.GetAllEvents())
            {
                if (typeof(Return).Equals(_event.GetType()))
                {
                    returns.Add((Return)_event);
                }
            }
            return returns;
        }

        public IEnumerable<Return> GetAllReturnsByRent(Rent rent)
        {
            List<Return> returns = new List<Return>();
            foreach (Return _return in GetAllReturns())
            {
                if (_return.Rent.Equals(rent))
                {
                    returns.Add(_return);
                }
            }
            return returns;
        }

        public IEnumerable<CopyOfBook> GetRentedCopiesOfBooks()
        {
            List<CopyOfBook> rentedCopiesOfBooks = new List<CopyOfBook>();
            foreach(Rent rent in GetAllRents())
            {
                if(rent.DateOfReturn == DateTime.MinValue)
                {
                    rentedCopiesOfBooks = MergeCollections<CopyOfBook>(rentedCopiesOfBooks, GetRentedCopiesOfBooks(rent));
                }
                
            }
            return rentedCopiesOfBooks;
        }

        public IEnumerable<CopyOfBook> GetRentedCopiesOfBooks(Rent rent)
        {
            List<CopyOfBook> rentedCopiesOfBooks = new List<CopyOfBook>(rent.Book.Keys);
            IEnumerable<Return> returns = GetAllReturnsByRent(rent);
            foreach(Return _return in returns)
            {
                foreach(CopyOfBook copy in _return.Books)
                {
                    rentedCopiesOfBooks.Remove(copy);
                }
            }
            return rentedCopiesOfBooks;
        }

        private List<T> MergeCollections<T>(List<T> enum1, IEnumerable<T> enum2)
        {
            foreach(T item in enum2)
            {
                enum1.Add(item);
            }
            return enum1;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if(phone.Length != 9)
            {
                return false;
            }
            return true;
        }
    }
}
