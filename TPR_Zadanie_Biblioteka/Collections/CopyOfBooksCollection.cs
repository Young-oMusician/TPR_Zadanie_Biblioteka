﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DL.Interfaces;
using DL.DataObjects;

namespace DL.Collections
{
    class CopyOfBooksCollection : ICrudOperations<CopyOfBook>
    {
        private List<CopyOfBook> _copiesOfBooks;

        public CopyOfBooksCollection()
        {
            _copiesOfBooks = new List<CopyOfBook>();
        }

        public void Add(CopyOfBook obj)
        {
            if (_copiesOfBooks.Find(copyOfBook => copyOfBook.Id.Equals(obj.Id)) != null)
            {
                throw new Exception("Copy of book with this ID already exists!");
            }
            if(obj.PurchaseDate.CompareTo(DateTime.Now) > 0)
            {
                throw new Exception("Invalid purchase date! (date from future)");
            }
            if(obj.PricePerDay < 0)
            {
                throw new Exception("Price cannot be negative!");
            }
            _copiesOfBooks.Add(obj);                       
        }

        public void Delete(CopyOfBook obj)
        {
            _copiesOfBooks.Remove(obj);
        }

        public CopyOfBook Get(Guid id)
        {
            return _copiesOfBooks.Find(copyOfBook => copyOfBook.Id.Equals(id));
        }

        public IEnumerable<CopyOfBook> GetAll()
        {
            return _copiesOfBooks;
        }

        public void Update(Guid id, int option, Object newValue)
        {
            //if (!id.Equals(obj.Id))
            //{
            //    throw new Exception("ID is permament, it cant be different from old object");
            //}
            //for (int i = 0; i < _copiesOfBooks.Count; i++)
            //{
            //    if (_copiesOfBooks[i].Id.Equals(id))
            //    {
            //        _copiesOfBooks[i] = obj;
            //    }
            //}
            CopyOfBook updatingCopyOfBook = _copiesOfBooks.Find(copyOfBook => copyOfBook.Id.Equals(id));
            if(updatingCopyOfBook == null)
            {
                throw new Exception("Copy of book with this ID doesn't exist");
            }
            switch (option)
            {
                case Consts.CopyOfBookBook:
                    updatingCopyOfBook.Book = (Book)newValue;
                    break;
                case Consts.CopyOfBookPurchaseDate:
                    updatingCopyOfBook.PurchaseDate = (DateTime)newValue;
                    break;
                case Consts.CopyOfBookPricePerDay:
                    updatingCopyOfBook.PricePerDay = (double)newValue;
                    break;
                default:
                    throw new Exception("Invalid option!");
            }
        }
    }
}
