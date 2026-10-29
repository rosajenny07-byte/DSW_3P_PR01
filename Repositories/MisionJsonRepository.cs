
using System.Text.Json;
using DSW_3P_PR01.Models;

namespace DSW_3P_PR01.Repositories
{
    public class MisionJsonRepository : IMisionRepository
    {
        private readonly string _rutaArchivo;

        private readonly JsonSerializerOptions _opciones = new()
        {
            WriteIndented = true
        };

        public MisionJsonRepository(IWebHostEnvironment environment)
        {
            string carpeta = Path.Combine(
                environment.ContentRootPath,
                "Data"
            );

            Directory.CreateDirectory(carpeta);

            _rutaArchivo = Path.Combine(carpeta, "misiones.json");

            if (!File.Exists(_rutaArchivo))
            {
                File.WriteAllText(_rutaArchivo, "[]");
            }
        }

        private List<Mision> LeerMisiones()
        {
            string json = File.ReadAllText(_rutaArchivo);

            return JsonSerializer.Deserialize<List<Mision>>(json)
                   ?? new List<Mision>();
        }

        private void GuardarMisiones(List<Mision> misiones)
        {
            string json = JsonSerializer.Serialize(
                misiones,
                _opciones
            );

            File.WriteAllText(_rutaArchivo, json);
        }

        public List<Mision> ObtenerTodas()
        {
            return LeerMisiones();
        }

        public Mision? ObtenerPorId(int id)
        {
            return LeerMisiones()
                .FirstOrDefault(m => m.Id == id);
        }

        public Mision Crear(Mision mision)
        {
            List<Mision> misiones = LeerMisiones();

            mision.Id = misiones.Count == 0
                ? 1
                : misiones.Max(m => m.Id) + 1;

            misiones.Add(mision);

            GuardarMisiones(misiones);

            return mision;
        }

        public bool Actualizar(Mision mision)
        {
            List<Mision> misiones = LeerMisiones();

            int indice = misiones.FindIndex(
                m => m.Id == mision.Id
            );

            if (indice == -1)
            {
                return false;
            }

            misiones[indice] = mision;

            GuardarMisiones(misiones);

            return true;
        }

        public bool Eliminar(int id)
        {
            List<Mision> misiones = LeerMisiones();

            Mision? mision = misiones.FirstOrDefault(
                m => m.Id == id
            );

            if (mision == null)
            {
                return false;
            }

            misiones.Remove(mision);

            GuardarMisiones(misiones);

            return true;
        }
    }
}
