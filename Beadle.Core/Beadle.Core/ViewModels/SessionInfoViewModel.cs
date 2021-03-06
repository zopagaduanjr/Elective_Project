﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Beadle.Core.Models;
using Beadle.Core.Repository;
using Beadle.Core.Services;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace Beadle.Core.ViewModels
{
    public class SessionInfoViewModel : ViewModelBase
    {
            

        //constructor
        public SessionInfoViewModel(IRepository repository, MainViewModel mainViewModel, INavigationService navigationService)
        {
            mainViewModel.SessionInfoViewModel = this;
            MainViewModel = mainViewModel;
            Repository = repository;
            NavigationService = navigationService;
            SelectedPerson = mainViewModel.SelectedSession.Persons.FirstOrDefault();
            //SelectedPerson = null;
            //commands
            ShowDeletePopUpCommand = new Command(async () => await ShowDeletePopUpAsync(), () => true);
        }

        //private fields
        private readonly INavigationService NavigationService;
        private MainViewModel _mainViewModel;
        private readonly IRepository Repository;
        private Person _selectedPerson;
        private bool _isSelectedPersonTrue;

        //properties
        public ICommand ShowDeletePopUpCommand { get; private set; }
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                if (value != null)
                    IsSelectedPersonTrue = true;
                else
                {
                    IsSelectedPersonTrue = false;
                }
                RaisePropertyChanged(nameof(SelectedPerson));

            }
        }
        public MainViewModel MainViewModel
        {
            get => _mainViewModel;
            set
            {
                _mainViewModel = value;
                RaisePropertyChanged(nameof(MainViewModel));
            }
        }

        public bool IsSelectedPersonTrue
        {
            get => _isSelectedPersonTrue;
            set
            {
                _isSelectedPersonTrue = value;
                RaisePropertyChanged(nameof(IsSelectedPersonTrue));

            }
        }

        //methods

        public async Task ShowDeletePopUpAsync()
        {
            var displaytitle = "Delete " + MainViewModel.SelectedSession.Name +"?";
            var answer = await Application.Current.MainPage.DisplayAlert(displaytitle, "Would you like to delete this session?","Yes","No");
            if (answer)
            {
                var persons = MainViewModel.SelectedSession.Persons;
                var personinTable = await Repository.Person.GetAllItemsAsync();
                foreach (var person in persons)
                {
                    foreach (var item in personinTable)
                    {
                        if (person.Id == item.Id)
                        {
                            await Repository.Person.DeleteItemAsync(item);
                        }
                    }
                }
                var records = MainViewModel.SelectedSession.Records;
                var recordinTable = await Repository.Record.GetAllItemsAsync();
                foreach (var record in records)
                {
                    var recordid = record.Id;
                    var recordsaz = await Repository.Record.GetItemFromIdAsync(recordid);
                    foreach (var recordItem in recordsaz.Items)
                    {
                        await Repository.Item.DeleteItemAsync(recordItem);
                    }
                    foreach (var record1 in recordinTable)
                    {
                        if (record.Id == record1.Id)
                        {
                            await Repository.Record.DeleteItemAsync(record1);
                        }
                    }
                }


                await Repository.Session.DeleteItemAsync(MainViewModel.SelectedSession);
                MainViewModel.SelectedSession = null;

                await Task.Run(() => MainViewModel.DeleteRefresher());

                await NavigationService.GoBack();
            }
        }

        //canclick


    }

}

