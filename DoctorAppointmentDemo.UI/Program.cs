using MyDoctorAppointment.Domain.Entities;
using MyDoctorAppointment.Service.Interfaces;
using MyDoctorAppointment.Service.Services;

namespace MyDoctorAppointment
{
    public class DoctorAppointment
    {
        private readonly IDoctorService _doctorService;

        public DoctorAppointment(string dataExt)
        {
            _doctorService = new DoctorService(dataExt);
        }

        public void Menu()
        {
            //while (true)
            //{
            //    // add Enum for menu items and describe menu
            //}

            Console.WriteLine("Current doctors list: ");
            var docs = _doctorService.GetAll();

            foreach (var doc in docs)
            {
                Console.WriteLine(doc.Name);
            }

            Console.WriteLine("Adding doctor: ");

            var newDoctor = new Doctor
            {
                Name = "Vasya",
                Surname = "Petrov",
                Experience = 20,
                DoctorType = Domain.Enums.DoctorTypes.Dentist
            };

            _doctorService.Create(newDoctor);

            Console.WriteLine("Current doctors list: ");
            docs = _doctorService.GetAll();

            foreach (var doc in docs)
            {
                Console.WriteLine(doc.Name);
            }
        }
    }

    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Выберите тип файла (json/xml):");
            string fileType = Console.ReadLine()?.ToLower(); 

            if (fileType != "json" && fileType != "xml") { 
                    Console.WriteLine("Неподдерживаемый тип файла.");
                    return; 
            }
            var doctorAppointment = new DoctorAppointment(fileType);
            doctorAppointment.Menu();
        }
    }
}