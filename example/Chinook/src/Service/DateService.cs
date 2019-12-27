using System;

namespace Chinook.Service {

    public interface IDateTimeService {
        DateTime CurrentDateTime { get; }
    }

    public class DateTimeService : IDateTimeService {
        private readonly DateTime _dateTime = new DateTime(2013, 12, 22) + DateTime.Now.TimeOfDay;
        public DateTime CurrentDateTime { get => _dateTime; }
    }
}