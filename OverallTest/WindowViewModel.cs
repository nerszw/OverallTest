using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using OverallTest.Accessory;
using PropertyChanged;
using System.Windows.Data;
using System.Threading;
using System.IO;

namespace OverallTest
{
    [ImplementPropertyChanged]
    class Record : NotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Data { get; set; }

        public Record() { }

        public Record(Record record)
        {
            Id = record.Id;
            Time = record.Time;
            Data = record.Data;
        }

        public object Clone()
        {
            return new Record(this);
        }

        public override string ToString()
        {
            return String.Format("{0} | {1} | {2}", Id, Time, Data);
        }
    }
    
    class WindowViewModel : IDisposable
    {
        private static readonly Random Rand = new Random();

        public ObservableCollection<Record> Records { get; set; }
        public ICollectionView RecordsView { get; set; }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand SortViewCommand { get; set; }
        public ICommand ResetViewCommand { get; set; }

        private readonly Scheduler Scheduler;

        public WindowViewModel()
        {
            var tasks = new List<MultipleTask>()
            {
                new MultipleTask(1, 5, CreateRecords),
                new MultipleTask(1, UpdateRecords),
                new MultipleTask(1, 5, DeleteRecords),
                new MultipleTask(5, 10, SortRecords),
                new MultipleTask(5, WriteRecords)
            };

            Scheduler = new Scheduler(tasks);

            Records = new ObservableCollection<Record>();
            RecordsView = CollectionViewSource.GetDefaultView(Records);

            StartCommand = new RelayCommand(x => Scheduler.Start(), x => !Scheduler.IsRunning);
            StopCommand = new RelayCommand(x => Scheduler.Stop(), x => Scheduler.IsRunning);

            SortViewCommand = new RelayCommand<string>(SortViewExecuted);
            ResetViewCommand = new RelayCommand(x => ResetViewExecuted());
        }

        private void SortViewExecuted(string field)
        {
            var sort = new SortDescription(field, ListSortDirection.Ascending);

            using (RecordsView.DeferRefresh())
            {
                RecordsView.SortDescriptions.Clear();
                RecordsView.SortDescriptions.Add(sort);
            }
            RecordsView.Refresh();
        }

        private void ResetViewExecuted()
        {
            RecordsView.SortDescriptions.Clear();
            RecordsView.Refresh();
        }

        private void CreateRecords()
        {
            //Создаём элементы
            var count = Rand.Next(5, 10 + 1);
            var recs = new List<Record>(count);
            for (int i = 0; i < count; ++i)
                recs.Add(GenRecord());

            //Добавляем в наблюд. коллекцию
            InGUI(() =>
            {
                recs.ForEach(x => Records.Add(x));
                RecordsView.Refresh();
            });
        }

        private void UpdateRecords()
        {
            InGUI(() =>
            {
                //Берём 20 случайных элементов
                var count = 20;
                var recs = Records.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

                //Обновляем данные
                recs.ForEach(x => x.Data = GenData());
                RecordsView.Refresh();
            });
        }

        private void DeleteRecords()
        {
            InGUI(() =>
            {
                while (Records.Count > 100)
                {
                    //Берём 5-10 случайных элементов
                    var count = Rand.Next(5, 10 + 1);
                    var recs = Records.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

                    //Удаляем записи
                    recs.ForEach(x => Records.Remove(x));
                }
            });
        }

        private void SortRecords()
        {
            InGUI(() =>
            {
                //Получаем отсортированные записи по Id
                var recs = Records.OrderBy(x => x.Id).ToList();

                //Перезаполняем коллекцию записей
                Records.Clear();
                recs.ForEach(x => Records.Add(x));

                //Обновляем представление
                RecordsView.Refresh();
            });
        }

        private async void WriteRecords()
        {
            //Копируем элементы
            List<Record> recs = null;
            await InGUI(() =>
            {
                recs = Records.Select(x => new Record(x)).ToList();
            });

            //Gen название файла
            var time = DateTime.Now.ToString("yy-MM-dd hh.mm.ss.f");
            var path = string.Format("{0}.txt", time);

            //Запись в файл
            File.WriteAllLines(path, recs.Select(x => x.ToString()));
        }

        private string GenData()
        {
            var length = Rand.Next(50, 100 + 1);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var data = new char[length];
            for (int i = 0; i < length; ++i)
                data[i] = chars[Rand.Next(chars.Length)];

            return new string(data);
        }

        private Record GenRecord()
        {
            return new Record()
            {
                Id = Rand.Next(1, 1000 + 1),
                Time = DateTime.Now,
                Data = GenData()
            };
        }
        
        private Task InGUI(Action run)
        {
            return App.Current.Dispatcher.InvokeAsync(run).Task;
        }

        public void Dispose()
        {
            Scheduler.Dispose();
        }
    }
}
