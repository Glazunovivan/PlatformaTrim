using Microsoft.Win32;
using PlatformaTrim.Core;
using PlatformaTrim.DigitalSignature;
using PlatformaTrim.Models;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace PlatformaTrim.ViewModels
{
    internal class UserDataViewModel : ObservableObject
    {
        private string _fullname;
        private string _number;
        private string _filePath;
        private DateTime _dateStart = DateTime.Now;
        private DateTime _dateEnd = DateTime.Now.AddMonths(12);

        public string Fullname
        {
            get { return _fullname; }
            set 
            { 
                _fullname = value;
                OnPropertyChange();
            }
        }

        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChange();
            }
        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChange();
            }
        }

        public DateTime DateStart
        {
            get { return _dateStart; }
            set
            {
                _dateStart = value;
                OnPropertyChange();
            }
        } 

        public DateTime DateEnd
        {
            get { return _dateEnd; }
            set
            {
                _dateEnd = value;
                OnPropertyChange();
            }
        }

        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandOpen { get; set; }

        public bool IsProcessed { get; set; } = false;

        public UserDataViewModel()
        {
            CommandOpen = new RelayCommand(c =>
            {
                OpenFileDialog();
            });

            CommandEdit = new RelayCommand(c =>
            {
                Accept();
            });
        }


        /// <summary>
        /// Подписать докумет цифровой подписью
        /// </summary>
        private async void Accept()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Fullname))
                {
                    throw new Exception("Поле \"Фамилия Имя Отчество\" должно быть заполнено.");
                }
                if (string.IsNullOrWhiteSpace(Number))
                {
                    throw new Exception("Поле \"Номер сертификата\" должно быть заполнено.");
                }
                if (string.IsNullOrWhiteSpace(FilePath))
                {
                    throw new Exception("Укажите путь к подписываемому файлу.");
                }

                await Task.Run(() =>
                {
                    IsProcessed = true;
                    OnPropertyChange(nameof(IsProcessed));
                    using DigitalSignatureConvertor convertor = new(new UserData
                    {
                        Fullname = Fullname,
                        NumberCertificate = Number,
                        DateEnd = DateEnd,
                        DateStart = DateStart
                    },
                    FilePath);

                    convertor.Accept();
                });

                MessageBox.Show("Документ успешно подписан.", "Документ подписан.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessed = false;
                OnPropertyChange(nameof(IsProcessed));
            }
            
        }

        private void OpenFileDialog()
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Office files |*.docx; *.doc; *.tiff; *.pdf";
                if (dialog.ShowDialog() == true)
                {
                    FilePath = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
