using System;

namespace Data.DataBuffer {

    [Serializable]
    public class CourseData {
        public int ElectiveID;
        public string CourseID;
        public string CourseName;
        public int CourseScore;
        public string Elective;
        public string CourseClass;

        public string DateOfWeek1;
        public string Section1;
        public string Location1;

        public string DateOfWeek2;
        public string Section2;
        public string Location2;

        public string DateOfWeek3;
        public string Section3;
        public string Location3;

        public string Teacher;
        public int Received;
        public int Quota;

        public string Degree;
        public string College;
        public string Department;
    }

    [Serializable]
    public class SearchData {
        public string degree;
        public string college;
        public string department;
        public string classs;
    }
}
