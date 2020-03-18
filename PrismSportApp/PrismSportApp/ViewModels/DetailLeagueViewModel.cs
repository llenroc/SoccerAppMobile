﻿using Prism.Navigation;
using Prism.Services;
using PrismSportApp.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace PrismSportApp.ViewModels
{
    public class DetailLeagueViewModel: INotifyPropertyChanged ,INavigatedAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public League League { get; set; } = new League();
        public Standings LeagueStandings { get; set; } = new Standings();
        public IEnumerable<Table> Table { get; set; } = new ObservableCollection<Table>();
        public Teamm Teamm { get; set; } = new Teamm();
        public Table TeamTable { get; set; } = new Table();
        public Links Links { get; set; } = new Links();
        public string NameLeague { get; set; }
        public string Code { get; set; }
        public string Logo { get; set; }

        public ICommand Tap { get; set; }
        INavigationService navigation;

        IPageDialogService dialogService;

        IApiServices apiServices;
        public DetailLeagueViewModel(INavigationService navigationService, IApiServices api, IPageDialogService pageDialog)
        {
            navigation = navigationService;
            dialogService = pageDialog;
            apiServices = api;
            Tap = new Command(SelectTeam);
        }        


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            this.NameLeague = parameters.GetValue<string>("Name");
            var key = parameters.GetValue<string>("LeagueId");
            this.Code = key;
            GetTable(Convert.ToInt32(key));
        }

        async Task GetTable(int param)
        {           
            try
            {
                RestService.For<IApiServices>(Links.url);
                var response = await apiServices.GetStandings(param);
                LeagueStandings = response;               
                this.Table = LeagueStandings.standings.First().table;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Error en el metodo Leagues: {ex.Message}");
            }

        }

        async void SelectTeam(object sender)
        {
            NavConstants navConstants = new NavConstants();
            TeamTable = (Table)sender;
            var parameters = new NavigationParameters();
            parameters.Add("TeamName", TeamTable.Team.Name);          
            parameters.Add("TeamId", TeamTable.Team.Id);          
            parameters.Add("Logo", TeamTable.Team.CrestUrl);          
            parameters.Add("Win", TeamTable.Won);    
            parameters.Add("Draws", TeamTable.Draw);                
            parameters.Add("Losts", TeamTable.Lost);                
            await navigation.NavigateAsync(new Uri(navConstants.TeamInfo, UriKind.Relative), parameters);
        }
    }
}
