using MyDoctorAppointment.Data.Configuration;
using MyDoctorAppointment.Data.Interfaces;
using MyDoctorAppointment.Domain.Entities;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace MyDoctorAppointment.Data.Repositories
{
    public abstract class GenericRepository<TSource> : IGenericRepository<TSource> where TSource : Auditable
    {
        public abstract string Path { get; set; }

        public abstract int LastId { get; set; }

        private readonly string dataExt;

        protected GenericRepository(string dataExt)
        {
            this.dataExt = dataExt;
        }

        public TSource Create(TSource source)
        {
            source.Id = ++LastId;
            source.CreatedAt = DateTime.Now;

            if (this.dataExt == "json")
            {
                File.WriteAllText(Path, JsonConvert.SerializeObject(((IGenericRepository<TSource>)this).GetAll().Append(source), Formatting.Indented));
            }

            if (this.dataExt == "xml")
            {
                List<TSource> dataList = ((IGenericRepository<TSource>)this).GetAll().ToList();
                dataList.Add(source);

                XmlSerializer serializer = new XmlSerializer(typeof(List<TSource>));
                using (FileStream fileStream = new FileStream(Path, FileMode.Create))
                {
                    serializer.Serialize(fileStream, dataList);
                }
            }
            SaveLastId();

            return source;
        }

        public bool Delete(int id)
        {
            if (GetById(id) is null)
                return false;

            if (dataExt == "json")
            {
                File.WriteAllText(Path, JsonConvert.SerializeObject(((IGenericRepository<TSource>)this).GetAll().Where(x => x.Id != id), Formatting.Indented));
            }
            if (dataExt == "xml")
            {
                List<TSource> dataList = ((IGenericRepository<TSource>)this).GetAll().Where(x => x.Id != id).ToList();

                XmlSerializer serializer = new XmlSerializer(typeof(List<TSource>));
                using (FileStream fileStream = new FileStream(Path, FileMode.Create))
                {
                    serializer.Serialize(fileStream, dataList);
                }                
            }

            return true;
        }

        public IEnumerable<TSource> GetAll()
        {
            if (File.Exists(Path))
            {                

                switch (dataExt)
                {
                    case "json":                        
                        return JsonConvert.DeserializeObject<List<TSource>>(File.ReadAllText(Path))!;                        
                    case "xml":
                        //return ReadXmlData();
                        XmlSerializer serializer = new XmlSerializer(typeof(List<TSource>));
                        using (FileStream fileStream = new FileStream(Path, FileMode.Open))
                        {
                            return (List<TSource>)serializer.Deserialize(fileStream);
                        }
                    default:
                        throw new NotSupportedException($"File format '{dataExt}' is not supported.");
                }
            }
            else
            {
                return new List<TSource>(); // or throw an exception if needed
            }
            
        }

        public TSource? GetById(int id)
        {
            return ((IGenericRepository<TSource>)this).GetAll().FirstOrDefault(x => x.Id == id);
        }

        public TSource Update(int id, TSource source)
        {
            source.UpdatedAt = DateTime.Now;
            source.Id = id;

            if (dataExt == "json")
            {
                File.WriteAllText(Path, JsonConvert.SerializeObject(((IGenericRepository<TSource>)this).GetAll().Select(x => x.Id == id ? source : x), Formatting.Indented));
            }
            if (dataExt == "xml")
            {
                List<TSource> dataList = ((IGenericRepository<TSource>)this).GetAll().Select(x => x.Id == id ? source : x).ToList();

                XmlSerializer serializer = new XmlSerializer(typeof(List<TSource>));
                using (FileStream fileStream = new FileStream(Path, FileMode.Create))
                {
                    serializer.Serialize(fileStream, dataList);
                }
            }

            return source;
        }

        public abstract void ShowInfo(TSource source);

        protected abstract void SaveLastId();

        protected dynamic ReadFromAppSettings() => JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Constants.AppSettingsPath))!;
    }
}
