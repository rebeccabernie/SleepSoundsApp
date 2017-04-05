﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SleepSounds
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Settings
        Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;

        List<string> _listNames;
        List<string> _playListsFromFile;


        String[] songs = new String[]
            { "birds", "cat", "city", "fire", "forrain", "rain", "thunder", "waves,", "whitenoise" };
        String playing; // currently playing, set to empty

        public MainPage()
        {
            this.InitializeComponent();
            initPlaylists();
        }

        private async void initPlaylists()
        {
            StorageFolder storageFolder =
                ApplicationData.Current.LocalFolder;    // set folder to current working directory
            StorageFile playlists =
                await storageFolder.CreateFileAsync("playlists.txt", CreationCollisionOption.OpenIfExists); // Make new

            //System.Diagnostics.Debug.Write("menu clicked");
            var text = await FileIO.ReadLinesAsync(playlists); // read playlists file as lines
            foreach (var line in text) // loop through each line, adapted from http://stackoverflow.com/questions/22922403/not-looping-through-every-line-only-looking-at-first-line-c-sharp
            {
                string name = "" + line.Split('|')[0]; // split each line at the |, set name to string at index 0 (before the |, i.e. the name)

                itemNew = new MenuFlyoutItem();
                itemNew.Name = name;
                itemNew.Text = name;
                itemNew.Tapped += ItemNew_Tapped;

                if (!xMenuFlyout.Items.Contains(itemNew))
                    xMenuFlyout.Items.Add(itemNew);
                else { } // do nothing

            }
        }

        // Sound buttons
        private void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            Button curr = (Button)sender; // get the click event object (i.e. current sound)
            string name = curr.Name.Substring(0, curr.Name.IndexOf("_")); // get the name of the click event before the _
            MediaElement me = (MediaElement)FindName(name); // set media element = new name

            // If / Else for stop/start on buttons
            if (me.Tag.ToString() == "N")
            {
                // if the tag is set to N (i.e. not playing), play sound and set to Y (playing)
                me.Play();
                me.Tag = "Y";
                if( playing == "")
                {
                    playing += me.Name;
                }
                else
                {
                    playing += ("," + me.Name); // Add to playing list
                }
            }
            else
            {
                // Tag is set to Y (i.e. is playing), stop the sound and set tag to N
                me.Stop();
                me.Tag = "N";
                playing.Replace(me.Name, "");   // Remove from playing list / replace with nothing

            }

        } // end sound buttons

        MenuFlyoutItem itemNew;
        private async void createCombo_Click(object sender, RoutedEventArgs e)
        {
            // Read/Write functions adapted from https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-reading-and-writing-files

            // Create a folder and file if it doesn't exist
            StorageFolder storageFolder =
                ApplicationData.Current.LocalFolder;    // set folder to current working directory
            StorageFile playlists =
                await storageFolder.CreateFileAsync("playlists.txt", CreationCollisionOption.OpenIfExists); // Make new

            // Insert name of playlist at start of playing string
            String comboName = this.inputText.Text.ToString();
            String playlist = comboName + "|" + playing;

            // Write data to the file
            await FileIO.AppendTextAsync(playlists, playlist + Environment.NewLine); // Environment.NewLine sets pointer to new line for next entry
            // Read data from file
            //await FileIO.ReadTextAsync(playlists);

            System.Diagnostics.Debug.WriteLine(await FileIO.ReadTextAsync(playlists)); // test, print to the debug console

            playing = ""; // reset the playing string when user saves a combo

           
            itemNew = new MenuFlyoutItem();
            itemNew.Name = comboName;
            itemNew.Text = comboName;
            itemNew.Tapped += ItemNew_Tapped;

            if (!xMenuFlyout.Items.Contains(itemNew))
                xMenuFlyout.Items.Add(itemNew);
            else { } // do nothing

        }

        //private async void populateMenu()
        //{
        //    // Open and read contents of file
        //    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        //    StorageFile playlists = await storageFolder.CreateFileAsync("playlists.txt", CreationCollisionOption.OpenIfExists);
        //    var text = await FileIO.ReadLinesAsync(playlists); // read playlists file as lines

        //    var lineCount = File.ReadLines(@"C:\playlists.txt").Count();

        //    foreach (var line in text) // loop through each line, adapted from http://stackoverflow.com/questions/22922403/not-looping-through-every-line-only-looking-at-first-line-c-sharp
        //    {
        //        string name = "" + line.Split('|')[0]; // split each line at the |, set name to string at index 0 (before the |, i.e. the name)

        //        itemNew = new MenuFlyoutItem();
        //        itemNew.Name = name;
        //        itemNew.Text = name;
        //        itemNew.Tapped += ItemNew_Tapped;
        //        itemNew.Holding += item_Holding;
        //        xMenuFlyout.Items.Add(itemNew);
        //    }


        //}

        //MenuFlyoutItem itemNew;
        //private async void flyout_btn_Click(object sender, RoutedEventArgs e)
        //{

            
        //}

        private async void ItemNew_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyoutItem curr = (MenuFlyoutItem)sender;
            // Open and read contents of file
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile playlists = await storageFolder.CreateFileAsync("playlists.txt", CreationCollisionOption.OpenIfExists);

            var text = await FileIO.ReadLinesAsync(playlists); // read playlists file as lines

            List<string> songNames = new List<string>();
            foreach (var line in text)
            {
                if (line.StartsWith(curr.Text) == true)
                {
                    string name = line.Split('|')[0]; // split each line at the |, set name to string at index 0 (before the |, i.e. the name)
                    string songs = line.Split('|')[1];
                    System.Diagnostics.Debug.WriteLine("name: " + name + ", " + curr.Text);

                    string[] split = songs.Split(',');

                    //int start = 1;
                    //while (songs != null)
                    //{
                    //    string song = songs.Substring(start, songs.IndexOf(",") - start);
                    //    songNames.Add(song);
                    //    start = songs.IndexOf(",") + 1;
                    //}

                }
                else { }

            }
        }

        //private async void item_Click(object sender, RoutedEventArgs e)
        //{
        //    // Open and read contents of file
        //    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        //    StorageFile playlists = await storageFolder.CreateFileAsync("playlists.txt", CreationCollisionOption.OpenIfExists);

        //    var text = await FileIO.ReadLinesAsync(playlists); // read playlists file as lines

        //    List<string> songNames = new List<string>();
        //    foreach (var line in text)
        //    {
        //        if (line.StartsWith(item.Text) == true)
        //        {
        //            string name = "" + line.Split('|')[0]; // split each line at the |, set name to string at index 0 (before the |, i.e. the name)
        //            string songs = "" + line.Split('|')[1];
        //            System.Diagnostics.Debug.WriteLine("name: " + name + ", " + item.Text);

        //            int start = 1;
        //            while (songs != null)
        //            {
        //                string song = songs.Substring(start, songs.IndexOf(","));
        //                songNames.Add(song);
        //                start = songs.IndexOf(",") + 1;
        //            }
        //        }
        //        else { }

        //    }

        //    //System.Diagnostics.Debug.WriteLine("name: " + name + ", " + item.Text);

        //}

        private async void flyout_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }// end mainpage
} // end app

