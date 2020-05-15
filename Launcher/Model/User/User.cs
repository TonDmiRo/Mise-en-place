using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Launcher.Model {
    [JsonObject(MemberSerialization.OptIn)]
    public class User : INotifyPropertyChanged {
        public User() {
            Projects = new UserProjects(new ObservableCollection<Project>());
            _listProjects = new List<string>();
            ListProjects = new ReadOnlyCollection<string>(_listProjects);
            _launcherLaunchDate = DateTime.Now;
            UserSerializer = new DefaultSerializer();
        }
        public User(string name) : this() {
            /// конструктор для создании нового пользователя
            Name = name;
            TotalUsageTime = TimeSpan.Zero;
            ///новый пользователь не вызывает метод SetTimeSpanMagnifier 
            magnifierForUsageTimeTotal = new TimeSpanMagnifier(TotalUsageTime, ChangedUsageTimeTotal);

        }

        public void SetHandlers(PropertyChangedEventHandler PropUserChanged, NotifyCollectionChangedEventHandler NotifyCollectionChanged) {
            PropertyChanged += PropUserChanged;
            Projects.AddPropertyChangedEventHandler(NotifyCollectionChanged);
        }

        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public TimeSpan TotalUsageTime { get; private set; }
        [JsonProperty]
        public TimeSpan ProductiveTime { get; private set; }

        public ISerializer UserSerializer { get; set; }
        //поля для учета времени
        private readonly DateTime _launcherLaunchDate;

        public UserProjects Projects { get; private set; }

        public ReadOnlyCollection<string> ListProjects { get; private set; }
        [JsonProperty]
        private readonly List<string> _listProjects;

        #region Magnifier
        /// <summary>
        /// Выполняемый проект.
        /// Если его нет пользователь свободен.
        /// </summary>
        public Project ProjectInProgress { get; private set; }
        private DateTime _iterationStartTime;

        /// <summary>
        /// Инициирует начало работы над проектом.
        /// Пользователь может работать только над одним проектом за итерацию.
        /// Для завершения работы используйте StopWorkingOnProject.
        /// </summary>
        /// <param name="project">Проект для текущей итерации.</param>
        public void StartWorkingOnProject(Project project) {

            if (ProjectInProgress == null) {
                project.ProvideMaterials();
                // Назначить выполняемый проект
                ProjectInProgress = project;
                // Зафиксировать время старта для проверки
                _iterationStartTime = DateTime.Now;
            }
            else {
                throw new Exception($"Пользователь работает над {ProjectInProgress.ProjectName}");
            }
        }
        public void StopWorkingOnProject(TimeSpan workTime) {
            // работа начинается с 25 минут
            if (workTime >= TimeSpan.FromMinutes(25)) {
                // Определить время работы над проектом
                TimeSpan internalWorkTime = ( DateTime.Now - _iterationStartTime ) + TimeSpan.FromSeconds(1);
                // Переданное время работы должно быть меньше
                if (workTime < internalWorkTime) {
                    if (workTime.TotalMinutes < 90) { AddTimeSpentOnProject(workTime); }
                    // минуты после 90 считаем не эффективными
                    // если одна итерация была больше 90 минут срезаем до 1:20
                    else { AddTimeSpentOnProject(TimeSpan.FromMinutes(80)); }
                }
            }
            // освобождаем объект блокировки
            ProjectInProgress = null;
        }
        private void AddTimeSpentOnProject(TimeSpan time) {
            ProductiveTime += time;
            OnPropertyChanged(nameof(ProductiveTime));
            // TODO: решить как обновить время проекта
            // ProjectInProgress.TimeSpentOnProject += workTime;
        }



        private TimeSpanMagnifier magnifierForUsageTimeTotal;
        private void ChangedUsageTimeTotal(object sender, TimeSpanHasChangedEventArgs e) {
            TotalUsageTime = e.IncreasedTimeSpan;
            OnPropertyChanged(nameof(TotalUsageTime));
        }
        [OnDeserialized]
        private void SetTimeSpanMagnifier(StreamingContext context) {
            magnifierForUsageTimeTotal = new TimeSpanMagnifier(TotalUsageTime, ChangedUsageTimeTotal);
        }
        #endregion

        #region User Serializer
        public void SaveUser() {
            if (UserSerializer != null) {
                SerializeUser();
                Projects.SerializeCollection(Name, UserSerializer);
            }
            else {
                throw new Exception("UserSerializer не задан!");
            }
        }
        private void SerializeUser() {
            UpdateListProjects();
            string userPath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["FilesPath"], Name, Path.ChangeExtension(Name, "json"));
            UserSerializer.Serialize(userPath, this);
        }
        private void UpdateListProjects() {
            _listProjects.Clear();

            var list = Projects.Select(p => p.ProjectName);
            _listProjects.AddRange(list);
            /// TODO: добавлять проекты отсутствующие в списке в архив
            /// или это делать на этапе проверки папки?
        }
        #endregion

        #region INotifyPropertyChanged
        //TODO: delete
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            string sfad = propertyName;
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
