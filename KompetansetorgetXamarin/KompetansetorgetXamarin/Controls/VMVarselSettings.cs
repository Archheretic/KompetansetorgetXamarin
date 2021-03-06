﻿using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Controls
{
    class VMVarselSettings : BaseContentPage
    {
        public ObservableCollection<fagområdeSetting> varslerSettings { get; set; } //items in GUI
        public ObservableCollection<Location> locationsSettings = new ObservableCollection<Location>(); //items in GUI
        public ObservableCollection<Course> coursesSettings = new ObservableCollection<Course>(); //items in GUI

        private List<string> checkedStudyGroups = new List<string>();
        

        private List<StudyGroup> studyGroupsFilter = new List<StudyGroup>(); //gets used when retreiving projects/oppgaver in CarouselOppgaver

        public Dictionary<string, string> studyDict { private set; get; }
        public Dictionary<string, string> coursesFilter = new Dictionary<string, string>();

        public static bool cs = false; //changed setting bool to detect when to save settings

        public VMVarselSettings()
        {
            studyDict = new Dictionary<string, string>();
        }

        public async Task InitializeSettings()
        {
            await GetAllFilters();
            SetSettings();

            foreach (var fagområdeSetting in varslerSettings)
            {
                fagområdeSetting.OnToggled += ToggleSelection;
            }

        }

        public void ToggleSelection(object sender, EventArgs e)
        {
            var fagområdeSetting = sender as fagområdeSetting;
            System.Diagnostics.Debug.WriteLine("{0} has been toggled to {1}", fagområdeSetting.Name, fagområdeSetting.IsSelected);
        }

        public List<string> GetCheckedStudyGroups()
        {
            checkedStudyGroups.Clear();
            //if (varslerSettings == null)
            //{
            //    SetSettings();
            //    return null;
            //}
            //else
            foreach (fagområdeSetting setting in varslerSettings)
            {
                bool checkSwitch = setting.IsSelected;
                if (checkSwitch == true)
                {
                    System.Diagnostics.Debug.WriteLine("setting.Name: " + setting.Name);
                    checkedStudyGroups.Add(setting.id);
                }
            }

            return checkedStudyGroups;
        }

        public async void SetSettings()
        {

            if (varslerSettings == null)
            {
                varslerSettings = new ObservableCollection<fagområdeSetting> { };

                foreach (var sg in studyGroupsFilter)
                {
                    System.Diagnostics.Debug.WriteLine("string name in studyDict.Values: " + sg.name);
                    varslerSettings.Add(new fagområdeSetting(sg.name, sg.filterChecked, sg.id));
                }
            }
        }

        public async Task PostToServer()
        {
            StudentsController sc = new StudentsController();
            await sc.PostStudentsStudyGroupToServer(GetCheckedStudyGroups());

            if (!Authenticater.Authorized)
            {
                GoToLogin();
            }
        }

        public void SaveSettings()
        {
            //DbLocation lc = new DbLocation();
            //DbCourse cc = new DbCourse();
            //if (cs == true)
            //{
                DbStudyGroup sgc = new DbStudyGroup();

                foreach (fagområdeSetting setting in varslerSettings)
                {
                    //gets the name and setting from 
                    string setName = setting.Name;
                    bool setSwitch = setting.IsSelected;

                    foreach (StudyGroup studygroup in studyGroupsFilter)
                    {
                        if (studygroup.name == setName)
                        {
                            studygroup.name = setName;
                            studygroup.filterChecked = setSwitch;
                            break;
                        }
                    }
                }
                sgc.UpdateStudyGroups(studyGroupsFilter);
                //cs = false;
            //}
        }

        public async Task GetAllFilters()
        {

            DbStudyGroup sgc = new DbStudyGroup();
            StudentsController sc = new StudentsController();

            studyGroupsFilter = sgc.GetAllStudyGroups(); //set checked to false
            List<StudyGroup> checkedStudyGroups = await sc.GetStudentsStudyGroupFromServer(); // use these to check some to true

            System.Diagnostics.Debug.WriteLine("studyGroupsFilter.Count: " + studyGroupsFilter.Count); 
            foreach (var sg in studyGroupsFilter)
            {
                if (checkedStudyGroups.Contains(sg))
                {
                    sg.filterChecked = true;
                }

                else
                {
                     sg.filterChecked = false;
                }
                //studyGroupsFilter.Add(sg);
            }
            System.Diagnostics.Debug.WriteLine("Another studyGroupsFilter.Count: " + studyGroupsFilter.Count);
            foreach (var studyGroup in studyGroupsFilter)
            {
                studyDict.Add(studyGroup.name, studyGroup.id);
            }
        }
    }
}
