﻿namespace WMS_Online.Models.CustomerViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(string name)
        {
            SelectedName = name;
        }
        public string SelectedName { get; } // введенное имя
    }
}
