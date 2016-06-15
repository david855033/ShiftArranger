using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    public class ViewModel
    {
        MainLogic mainLogic;
        public ViewModel(MainLogic mainLogic)
        {
            this.mainLogic = mainLogic;
        }

        public ObservableCollection<DoctorInformation> DoctorList
        {
            get
            {
                return new ObservableCollection<DoctorInformation>(mainLogic.doctorList);
            }
        }

        // Property Change Logic  
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
