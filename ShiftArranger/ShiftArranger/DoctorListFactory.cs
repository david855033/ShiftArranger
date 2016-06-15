using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    public class DoctorListFactory
    {
        List<DoctorInformation> doctorList;
        public List<DoctorInformation> getDoctorList()
        {
            doctorList = new List<DoctorInformation>();
            for (int i = 0; i < 12; i++)
                addDoctor("PGY" + i, WardSets.allWards, WardType.NICU, DoctorType.PGY);

            for (int i = 0; i < 5; i++)
                addDoctor("R1" + i, WardSets.allWards, WardType.NICU, DoctorType.R1);

            for (int i = 0; i < 3; i++)
                addDoctor("R2" + i, WardSets.allWards, WardType.NICU, DoctorType.R2);

            for (int i = 0; i < 2; i++)
                addDoctor("R3" + i, WardSets.allWards, WardType.NICU, DoctorType.R3);

            return doctorList;
        }

        void addDoctor(string id, IEnumerable<WardType> WardSet, WardType mainWard, DoctorType doctorType)
        {
            doctorList.Add(new DoctorInformation
            {
                ID = id,
                capableOf = WardSet,
                mainWard = mainWard,
                doctorType = doctorType
            });
        }
    }
}
