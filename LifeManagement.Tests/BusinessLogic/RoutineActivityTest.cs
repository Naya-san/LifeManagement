using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LifeManagement.ObsoleteBusinessLogic;
using LifeManagement.ObsoleteModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LifeManagement.Tests.BusinessLogic
{
    [TestClass]
    public class RoutineActivityTest
    {
        private DateTimeProvider SetupDateTimeProvider(DateTime utcNow)
        {
            var dateTimeProvider = new Mock<DateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(utcNow);
            return dateTimeProvider.Object;
        }

        [TestMethod]
        public void AddTaskToRoutine_ShouldAddTaskAfterCurrentTime_WhenDayLimitAlreadyStarted()
        {
            var date = DateTime.UtcNow.Date;
            var dayLimit = new DayLimit { Id = Guid.NewGuid(), StartDate = date.AddHours(8), EndDate = date.AddHours(22) };

            var data = new List<Routine>
            {
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate,
                    EndDate = dayLimit.StartDate.AddHours(1)
                },
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate.AddHours(1).AddMinutes(5),
                    EndDate = dayLimit.StartDate.AddHours(3)
                }
            }.AsQueryable();

            TestTaskWasAdded(SetupDateTimeProvider(dayLimit.StartDate.AddHours(5)), dayLimit, data, Times.Once());
        }

        [TestMethod]
        public void AddTaskToRoutine_ShouldNotAddTask_WhenDayLimitInThePast()
        {
            var date = DateTime.UtcNow.Date;
            var dayLimit = new DayLimit { Id = Guid.NewGuid(), StartDate = date.AddHours(8), EndDate = date.AddHours(22) };

            var data = new List<Routine>
            {
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate,
                    EndDate = dayLimit.StartDate.AddHours(1)
                },
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate.AddHours(1).AddMinutes(5),
                    EndDate = dayLimit.EndDate
                }
            }.AsQueryable();

            TestTaskWasAdded(SetupDateTimeProvider(date.AddDays(1)), dayLimit, data, Times.Never(), false);
        }

        [TestMethod]
        public void AddTaskToRoutine_ShouldAddTask_WhenNoRoutinesPlanned()
        {
            var date = DateTime.UtcNow.Date;
            var dayLimit = new DayLimit { Id = Guid.NewGuid(), StartDate = date.AddHours(8), EndDate = date.AddHours(22) };

            var data = new List<Routine>().AsQueryable();

            TestTaskWasAdded(SetupDateTimeProvider(date), dayLimit, data, Times.Once());
        }

        [TestMethod]
        public void AddTaskToRoutine_ShouldAddTask_WhenFreeTimeAfterLastRoutine()
        {
            var date = DateTime.UtcNow.Date;
            var dayLimit = new DayLimit { Id = Guid.NewGuid(), StartDate = date.AddHours(8), EndDate = date.AddHours(22) };

            var data = new List<Routine>{ new Routine{ DayLimit = dayLimit, StartDate = dayLimit.StartDate, EndDate = dayLimit.StartDate.AddHours(1)}}.AsQueryable();

            TestTaskWasAdded(SetupDateTimeProvider(date), dayLimit, data, Times.Once());
        }

        [TestMethod]
        public void AddTaskToRoutine_ShouldAddTask_WhenFreeTimeBetweenRoutines()
        {
            var date = DateTime.UtcNow.Date;
            var dayLimit = new DayLimit { Id = Guid.NewGuid(), StartDate = date.AddHours(8), EndDate = date.AddHours(22) };

            var data = new List<Routine>
            {
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate,
                    EndDate = dayLimit.StartDate.AddHours(1)
                },
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate.AddHours(2),
                    EndDate = dayLimit.EndDate
                }
            }.AsQueryable();

            TestTaskWasAdded(SetupDateTimeProvider(date), dayLimit, data, Times.Once());
        }

        [TestMethod]
        public void AddTaskToRoutine_ShouldNotAddTask_WhenNoFreeTime()
        {
            var date = DateTime.UtcNow.Date;
            var dayLimit = new DayLimit { Id = Guid.NewGuid(), StartDate = date.AddHours(8), EndDate = date.AddHours(22) };

            var data = new List<Routine>
            {
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate,
                    EndDate = dayLimit.StartDate.AddHours(1)
                },
                new Routine
                {
                    DayLimit = dayLimit,
                    StartDate = dayLimit.StartDate.AddHours(1).AddMinutes(5),
                    EndDate = dayLimit.EndDate
                }
            }.AsQueryable();

            TestTaskWasAdded(SetupDateTimeProvider(date), dayLimit, data, Times.Never(), false);
        }

        private void TestTaskWasAdded(DateTimeProvider dateTimeProvider, DayLimit dayLimit, IQueryable<Routine> data, Times times, bool expectedResult = true)
        {
            var activityLength = TimeSpan.FromMinutes(20);
    
            var task = new Task { Id = Guid.NewGuid(), Estimation = activityLength };

            var mockSet = new Mock<DbSet<Routine>>();
            var mockContext = SetupContext(mockSet, data);

            var result = new RoutineActivity(mockContext.Object, dateTimeProvider).AddTaskToRoutine(dayLimit, task);

            MultiAssert.Aggregate(
                () => Assert.AreEqual(expectedResult, result),
                () => mockSet.Verify(m => m.Add(It.IsAny<Routine>()), times),
                () => mockContext.Verify(m => m.SaveChanges(), times));
        }

        private Mock<LifeManagementContext> SetupContext(Mock<DbSet<Routine>> mockSet, IQueryable<Routine> data)
        {
            mockSet.As<IQueryable<Routine>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Routine>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Routine>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Routine>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<LifeManagementContext>();
            mockContext.Setup(c => c.Routines).Returns(mockSet.Object);

            return mockContext;
        } 
    }
}
