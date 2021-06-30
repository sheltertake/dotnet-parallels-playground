using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class MyTest
    {
        private List<MySaga> Sagas;

        [SetUp]
        public void Setup()
        {
            Sagas = Enumerable.Range(1, 10).Select(x => new MySaga
            {
                Id = x
            })
            .ToList();
        }

        [Test]
        public void SagasDoesNotThrowTest()
        {
            Action action = () =>
            {
                Sagas.ForEach(saga => MyWorkflow.ProcessSaga(saga));
            };

            action.Should().NotThrow();

        }

        [Test]
        public void SagasContainsExpectedBits()
        {

            Sagas.ForEach(saga => MyWorkflow.ProcessSaga(saga));

            foreach (var saga in Sagas)
            {
                if(saga.Id == 3)
                {
                    saga.IsFirst.Should().BeTrue();
                    saga.IsSecond.Should().BeFalse();
                    saga.IsProcessed.Should().BeFalse();
                }
                else
                {
                    saga.IsFirst.Should().BeTrue();
                    saga.IsSecond.Should().BeTrue();
                    saga.IsProcessed.Should().BeTrue();
                }
            }
        }


        [Test]
        public void SagasWithTasksDoesNotThrowTest()
        {
            Action action = () =>
            {
                Sagas.ForEach(saga => MyWorkflow.ProcessSagaWithTask(saga));
            };

            action.Should().NotThrow();

        }

        [Test]
        public void SagasWithTasksContainsExpectedBits()
        {

            Sagas.ForEach(saga => MyWorkflow.ProcessSagaWithTask(saga));

            foreach (var saga in Sagas)
            {
                if (saga.Id == 3)
                {
                    saga.IsFirst.Should().BeTrue();
                    saga.IsSecond.Should().BeFalse();
                    saga.IsProcessed.Should().BeFalse();
                }
                else
                {
                    saga.IsFirst.Should().BeTrue();
                    saga.IsSecond.Should().BeTrue();
                    saga.IsProcessed.Should().BeTrue();
                }
            }
        }
    }

    public static class MyWorkflow
    {
        public static void ProcessSagaWithTask(MySaga saga)
        {
            
            try
            {
                var task1 = Task.Factory.StartNew(() =>
                {
                    MyWorkflow.DoFirst(saga);
                });
                var task2 = Task.Factory.StartNew(() =>
                {
                    MyWorkflow.DoSecond(saga);
                });

                Task.WaitAll(task1, task2);
                MyWorkflow.SetProcessed(saga);
            }
            catch (Exception)
            {
                Console.WriteLine("Task Error " + saga.Id);
            }
        }

        public static void ProcessSaga(MySaga saga)
        {
            try
            {
                MyWorkflow.DoFirst(saga);
                MyWorkflow.DoSecond(saga);
                MyWorkflow.SetProcessed(saga);
            }
            catch (Exception)
            {
                Console.WriteLine("Error " + saga.Id);
            }            
        }
        public static void DoFirst(MySaga saga)
        {
            saga.IsFirst = true;
        }

        public static void DoSecond(MySaga saga)
        {
            if(saga.Id != 3)
            {
                saga.IsSecond = true;
            }
            else
            {
                throw new Exception();
            }            
        }

        public static void SetProcessed(MySaga saga)
        {
            if(saga.IsFirst && saga.IsSecond)
            {
                saga.IsProcessed = true;
            }
            
        }
    }
    public class MySaga
    {
        public int Id { get; set; }
        public bool IsFirst { get; set; }
        public bool IsSecond { get; set; }
        public bool IsProcessed { get; set; }
    }
}
