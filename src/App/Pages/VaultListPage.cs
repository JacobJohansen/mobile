﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bit.App.Abstractions;
using Bit.App.Models.View;
using Xamarin.Forms;
using XLabs.Ioc;

namespace Bit.App.Views
{
    public class VaultListPage : ContentPage
    {
        private readonly IFolderService _folderService;
        private readonly ISiteService _siteService;
        private ListView _listView;

        public VaultListPage()
        {
            _folderService = Resolver.Resolve<IFolderService>();
            _siteService = Resolver.Resolve<ISiteService>();

            var addSiteToolBarItem = new ToolbarItem("+", null, async () =>
            {
                var addSitePage = new VaultAddSitePage();
                await Navigation.PushAsync(addSitePage);
            }, ToolbarItemOrder.Default, 0);

            ToolbarItems.Add(addSiteToolBarItem);

            _listView = new ListView
            {
                IsGroupingEnabled = true,
                GroupDisplayBinding = new Binding("Name")
            };

            _listView.ItemSelected += FolderSelected;
            _listView.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new TextCell();
                cell.SetBinding<VaultView.Site>(TextCell.TextProperty, s => s.Name);
                cell.SetBinding<VaultView.Site>(TextCell.DetailProperty, s => s.Username);
                return cell;
            });

            Title = "My Vault";
            Content = _listView;
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var folders = _folderService.GetAllAsync().GetAwaiter().GetResult();
            var sites = _siteService.GetAllAsync().GetAwaiter().GetResult();

            var folderItems = folders.Select(f => new VaultView.Folder(f, sites.Where(s => s.FolderId == f.Id))).ToList();
            // add the sites with no folder
            folderItems.Add(new VaultView.Folder(sites.Where(s => !s.FolderId.HasValue)));
            _listView.ItemsSource = folderItems;
        }

        void FolderSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        void SiteSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}