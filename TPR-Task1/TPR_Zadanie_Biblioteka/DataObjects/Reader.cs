﻿using System;
using System.Runtime.Serialization;

namespace DL.DataObjects
{
    [Serializable]
    public class Reader : Person
    {
        private DateTime _dateOfRegistration;
        private double _balance;

        public Reader(Guid id, string name, string surname, DateTime birthDate,
            string phoneNumber, string email, Gender gender, DateTime dateOfRegistration)
            : base(id, name, surname, birthDate, phoneNumber, email, gender)
        {
            _dateOfRegistration = dateOfRegistration;
            Balance = 0.0;
        }

        public Reader(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            //base.Id = (Guid)info.GetValue("id", typeof(Guid));
            //base.Name = info.GetString("name");
            //base.Surname = info.GetString("surname");
            //base.PhoneNumber = info.GetString("phoneNumber");
            //base.BirthDate = info.GetDateTime("birthDate");
            //base.Email = info.GetString("email");
            //base.Gender1 = (Gender)info.GetValue("gender", typeof(Gender));
            _dateOfRegistration = info.GetDateTime("dateOfRegistration");
            _balance = info.GetDouble("balance");
        }

        public DateTime DateOfRegistration { get => _dateOfRegistration; set => _dateOfRegistration = value; }
        public double Balance { get => _balance; set => _balance = value; }

        public override bool Equals(object obj)
        {
            return obj is Reader reader &&
                   base.Equals(obj) &&
                   Id.Equals(reader.Id);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue("id", base.Id, typeof(Guid));
            //info.AddValue("name", base.Name);
            //info.AddValue("surname", base.Surname);
            //info.AddValue("phoneNumber", base.PhoneNumber);
            //info.AddValue("birthDate", base.BirthDate);
            //info.AddValue("email", base.Email);
            //info.AddValue("gender", base.Gender1, typeof(Gender));
            info.AddValue("dateOfRegistration", _dateOfRegistration);
            info.AddValue("balance", _balance);
        }

        public override string ToString()
        {
            return $"{{{nameof(DateOfRegistration)}={DateOfRegistration.ToString()}, {nameof(Balance)}={Balance.ToString()}, {nameof(Id)}={Id.ToString()}, {nameof(Name)}={Name}, {nameof(Surname)}={Surname}, {nameof(BirthDate)}={BirthDate.ToString()}, {nameof(PhoneNumber)}={PhoneNumber}, {nameof(Email)}={Email}, {nameof(Gender1)}={Gender1.ToString()}}}";
        }
    }
}
